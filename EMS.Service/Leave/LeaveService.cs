using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Repo;
using System.Linq.Expressions;
using EMS.Core;
using static EMS.Core.Enums;

namespace EMS.Service
{
    public class LeaveService : ILeaveService
    {
        #region "Fields"
        private IRepository<LeaveActivity> repoLeaveActivity;
        private IRepository<LeaveActivityAdjust> repoLeaveActivityAdjust;
        private IRepository<LeaveAdjust> repoLeaveAdjust;
        private IRepository<Preference> repoPreferences;
        private IRepository<UserLogin> repoUserMaster;
        private IRepository<OfficialLeave> repoOfficialLeave;
        private IRepository<LateHour> repoLateHour;
        private IRepository<LeaveTypesMaster> repoLeaveTypesMaster;
        #endregion

        #region "Cosntructor"
        public LeaveService(IRepository<LeaveActivity> _repoLeaveActivity, IRepository<LeaveActivityAdjust> _repoLeaveActivityAdjust, IRepository<LeaveAdjust> _repoLeaveAdjust, IRepository<Preference> _repoPreferences, IRepository<UserLogin> _repoUserMaster, IRepository<OfficialLeave> _repoOfficialLeave, IRepository<LateHour> _repoLateHour, IRepository<LeaveTypesMaster> _repoLeaveTypesMaster)
        {
            this.repoLeaveActivity = _repoLeaveActivity;
            this.repoLeaveActivityAdjust = _repoLeaveActivityAdjust;
            this.repoLeaveAdjust = _repoLeaveAdjust;
            this.repoPreferences = _repoPreferences;
            this.repoUserMaster = _repoUserMaster;
            this.repoOfficialLeave = _repoOfficialLeave;
            this.repoLateHour = _repoLateHour;
            this.repoLeaveTypesMaster = _repoLeaveTypesMaster;
        }
        #endregion

        public List<LeaveActivity> GetLeaves(out int total, PagingService<LeaveActivity> pagingService) // List<LeaveActivity>
        {
            return repoLeaveActivity.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetPage(pagingService.Start, pagingService.Length, out total)
                .ToList();
        }

        public double GetTotalLeaves(PagingService<LeaveActivity> pagingService)
        {
            return (repoLeaveActivity.Query()
                .Filter(pagingService.Filter).GetQuerable().Select(l => new { l.StartDate, l.EndDate, l.IsHalf }).ToList().Sum(l => (l.IsHalf.HasValue && l.IsHalf.Value) ? (((l.EndDate - l.StartDate).TotalDays + 1) / 2) : ((l.EndDate - l.StartDate).TotalDays + 1)));
        }
        public double GetApprovedLeaves(int uid, int leaveCategory, DateTime startDate, DateTime endDate)
        {
            return (repoLeaveActivity.Query()
                .Filter(x => x.Uid == uid && x.LeaveCategory == leaveCategory && x.StartDate >= startDate && x.EndDate <= endDate && x.Status == (int)LeaveStatus.Approved).GetQuerable().Select(l => new { l.StartDate, l.EndDate, l.IsHalf }).ToList().Sum(l => (l.IsHalf.HasValue && l.IsHalf.Value) ? (((l.EndDate - l.StartDate).TotalDays + 1) / 2) : ((l.EndDate - l.StartDate).TotalDays + 1)));
        }
        public double GetApprovedLeaves(int uid, int leaveCategory, DateTime startDate, DateTime endDate, bool isPrev = false)
        {
            if (!isPrev) // for hidden and display during apply
            {
                return (repoLeaveActivity.Query()
                    .Filter(x => x.Uid == uid && x.LeaveCategory == leaveCategory && x.StartDate >= startDate /*&& x.EndDate <= endDate*/ && (x.Status == (int)LeaveStatus.Approved || x.Status == (int)LeaveStatus.Pending)).GetQuerable().Select(l => new { l.StartDate, l.EndDate, l.IsHalf }).ToList().Sum(l => (l.IsHalf.HasValue && l.IsHalf.Value) ? (((l.EndDate - l.StartDate).TotalDays + 1) / 2) : ((l.EndDate - l.StartDate).TotalDays + 1)));
            }
            else
            {
                return (repoLeaveActivity.Query()
                   .Filter(x => x.Uid == uid && x.LeaveCategory == leaveCategory && x.StartDate >= startDate /*&& x.EndDate <= endDate*/ && (x.Status == (int)LeaveStatus.Pending || x.Status == (int)LeaveStatus.Approved)).GetQuerable().Select(l => new { l.StartDate, l.EndDate, l.IsHalf }).ToList().Sum(l => (l.IsHalf.HasValue && l.IsHalf.Value) ? (((l.EndDate - l.StartDate).TotalDays + 1) / 2) : ((l.EndDate - l.StartDate).TotalDays + 1)));
            }


        }
        public double GetAllApprovedLeaves(int uid, int leaveCategory)
        {
            return (repoLeaveActivity.Query()
                .Filter(x => x.Uid == uid && x.LeaveCategory == leaveCategory && x.Status == (int)LeaveStatus.Approved).GetQuerable().Select(l => new { l.StartDate, l.EndDate, l.IsHalf }).ToList().Sum(l => (l.IsHalf.HasValue && l.IsHalf.Value) ? (((l.EndDate - l.StartDate).TotalDays + 1) / 2) : ((l.EndDate - l.StartDate).TotalDays + 1)));
        }

        public double GetAllSickLeavesForYear(int uid, int leaveCategory)
        {
            var year = DateTime.Now.Year;
            return (repoLeaveActivity.Query()
                .Filter(x => x.Uid == uid && x.LeaveCategory == leaveCategory && x.StartDate.Year == year && x.EndDate.Year == year && (x.Status == (int)LeaveStatus.Approved || x.Status == (int)LeaveStatus.Pending)).GetQuerable().Select(l => new { l.StartDate, l.EndDate, l.IsHalf }).ToList().Sum(l => (l.IsHalf.HasValue && l.IsHalf.Value) ? (((l.EndDate - l.StartDate).TotalDays + 1) / 2) : ((l.EndDate - l.StartDate).TotalDays + 1)));
        }
        public List<LeaveActivity> GetAllSICKLeaves(int uid, int leaveCategory)
        {
            var year = DateTime.Now.Year;
            return repoLeaveActivity.Query().Get()
               .Where(x => x.Uid == uid && x.LeaveCategory == leaveCategory && x.StartDate.Year == year && x.EndDate.Year == year && (x.Status == (int)LeaveStatus.Approved || x.Status == (int)LeaveStatus.Pending))
               .ToList();
        }
        public double GetLeaves(int uid, int leaveCategory, DateTime startDate, DateTime endDate)
        {
            return (repoLeaveActivity.Query()
                .Filter(x => x.Uid == uid && x.LeaveCategory == leaveCategory && x.StartDate >= startDate && x.EndDate <= endDate && x.Status != (int)LeaveStatus.Cancelled && x.Status != (int)LeaveStatus.UnApproved).GetQuerable().Select(l => new { l.StartDate, l.EndDate, l.IsHalf }).ToList().Sum(l => (l.IsHalf.HasValue && l.IsHalf.Value) ? (((l.EndDate - l.StartDate).TotalDays + 1) / 2) : ((l.EndDate - l.StartDate).TotalDays + 1)));
        }
        public double? GetPendingLeaves(int uid, int leaveCategory = 0, bool isPrev = false)
        {
            if (leaveCategory > 0)
            {
                return (repoLeaveActivity.Query()
                    .Filter(x => x.Uid == uid && x.LeaveCategory == leaveCategory && (isPrev ? (x.Status == (int)LeaveStatus.Pending || x.Status == (int)LeaveStatus.Approved) : x.Status == (int)LeaveStatus.Pending)).GetQuerable().Select(l => new { l.StartDate, l.EndDate, l.IsHalf }).ToList().Sum(l => (l.IsHalf.HasValue && l.IsHalf.Value) ? (((l.EndDate - l.StartDate).TotalDays + 1) / 2) : ((l.EndDate - l.StartDate).TotalDays + 1)));
            }
            else
            {
                return (repoLeaveActivity.Query()
                   .Filter(x => x.Uid == uid && x.LeaveCategory != (int)Enums.LeaveCategory.UnpaidLeave && (isPrev ? (x.Status == (int)LeaveStatus.Pending || x.Status == (int)LeaveStatus.Approved) : x.Status == (int)LeaveStatus.Pending)).GetQuerable().Select(l => new { l.StartDate, l.EndDate, l.IsHalf }).ToList().Sum(l => (l.IsHalf.HasValue && l.IsHalf.Value) ? (((l.EndDate - l.StartDate).TotalDays + 1) / 2) : ((l.EndDate - l.StartDate).TotalDays + 1)));
            }
        }

        public double GetTotalApprovedPendingLeaves(PagingService<LeaveActivity> pagingService)
        {
            return (repoLeaveActivity.Query()
                .Filter(pagingService.Filter).GetQuerable().Where(l => l.Status == (int)Enums.LeaveStatus.Approved || l.Status == (int)Enums.LeaveStatus.Pending)
                .Select(l => new { l.StartDate, l.EndDate, l.IsHalf }).ToList().Sum(l => (l.IsHalf.HasValue && l.IsHalf.Value) ? (((l.EndDate - l.StartDate).TotalDays + 1) / 2) : ((l.EndDate - l.StartDate).TotalDays + 1)));
        }

        public LeaveActivity GetLeaveById(int LeaveId)
        {
            return repoLeaveActivity.FindById(LeaveId);
        }

        public List<LeaveActivityAdjust> GetLeaveAdjustedById(int leaveId)
        {
            return repoLeaveActivityAdjust.Query().Filter(l => l.LeaveId == leaveId).Get().ToList();
        }

        public List<LeaveAdjust> GetAdjustLeavesByUid(int uId)
        {
            return repoLeaveAdjust.Query().Filter(L => L.Uid == uId && L.IsCancel == false).Get().ToList();
        }

        public List<LeaveActivity> GetLeaveByUId(int uId)
        {
            return repoLeaveActivity.Query().Filter(l => l.Uid == uId).Get().ToList();
        }

        public LeaveActivity GetLeaveByUIdDateRange(int uId, DateTime startDate, DateTime endDate)
        {
            return repoLeaveActivity.Query().Filter(l => l.Uid == uId && ((startDate >= l.StartDate && startDate <= l.EndDate) || (endDate >= l.StartDate && endDate <= l.EndDate) || (startDate <= l.StartDate && endDate >= l.EndDate))).Get().FirstOrDefault();
        }

        public bool CanApplyLeave(int userid, int leaveid, DateTime startDate, DateTime endDate)
        {
            return repoLeaveActivity.Query().Filter(l => l.Uid == userid && l.LeaveId != leaveid && (l.Status.HasValue ? (l.Status.Value == (int)Enums.LeaveStatus.Approved || l.Status.Value == (int)Enums.LeaveStatus.Pending) : true) && ((startDate >= l.StartDate && startDate <= l.EndDate) || (endDate >= l.StartDate && endDate <= l.EndDate) || (startDate <= l.StartDate && endDate >= l.EndDate))).Get().Count() == 0;
        }

        public Preference GetPreferencesByPmuid(int pmuid)
        {
            return repoPreferences.Query().Filter(x => x.pmid == pmuid).Get().FirstOrDefault();
        }

        public Preference GetPreferecesByPMUid(int pmId)
        {
            return repoPreferences.Query().Filter(p => p.pmid == pmId).Get().FirstOrDefault();
        }


        public List<LeaveActivityAdjust> GetLeaveActivityAdjListById(int leavId)
        {
            return repoLeaveActivityAdjust.Query().Filter(l => l.LeaveId == leavId).Get().ToList();
        }

        public LeaveActivityAdjust GetLeaveActivityAdjById(int adjustId)
        {
            return repoLeaveActivityAdjust.Query().Filter(l => l.LeaveActivityAdjustId == adjustId).Get().FirstOrDefault();
        }

        public List<LeaveActivity> GetLeaveActivity(Expression<Func<LeaveActivity, bool>> expr)
        {
            return repoLeaveActivity.Query().Filter(expr).Get().ToList();
        }


        public void Save(LeaveActivity entity)
        {
            if (entity.LeaveId == 0)
            {
                repoLeaveActivity.ChangeEntityState<LeaveActivity>(entity, ObjectState.Added);
                repoLeaveActivity.InsertGraph(entity);
            }
            else
            {
                repoLeaveActivity.ChangeEntityState<LeaveActivity>(entity, ObjectState.Modified);
                repoLeaveActivity.SaveChanges();
            }

        }

        public void Save(LeaveAdjust entity)
        {
            if (entity.AdjustId == 0)
            {
                repoLeaveAdjust.ChangeEntityState<LeaveAdjust>(entity, ObjectState.Added);
                repoLeaveAdjust.InsertGraph(entity);
            }
            else
            {
                repoLeaveAdjust.ChangeEntityState<LeaveAdjust>(entity, ObjectState.Modified);
                repoLeaveAdjust.SaveChanges();
            }

        }

        public void Save(LeaveActivityAdjust entity)
        {
            if (entity.Adjustid == 0)
            {
                repoLeaveActivityAdjust.ChangeEntityState<LeaveActivityAdjust>(entity, ObjectState.Added);
                repoLeaveActivityAdjust.InsertGraph(entity);
            }
            else
            {
                repoLeaveActivityAdjust.ChangeEntityState<LeaveActivityAdjust>(entity, ObjectState.Modified);
                repoLeaveActivityAdjust.SaveChanges();
            }

        }

        public void Delete(LeaveActivityAdjust entity)
        {
            if (entity.Adjustid > 0)
            {
                repoLeaveActivityAdjust.ChangeEntityState<LeaveActivityAdjust>(entity, ObjectState.Deleted);
                repoLeaveActivityAdjust.Delete(entity.Adjustid);
            }
        }

        public List<OfficialLeave> GetOfficialLeaves(byte CountryId, bool onlyHoliday = false)
        {
            return repoOfficialLeave.Query().Filter(l => l.IsActive && (CountryId == 0 || l.CountryId == CountryId || l.CountryId == 0) && (onlyHoliday ? (l.LeaveType.ToLower() == "holiday") : true)).Get().OrderBy(l => l.LeaveDate).ToList();
        }
        public List<OfficialLeave> GetOfficialLeavesList(int CountryId)
        {
            return repoOfficialLeave.Query().Filter(l => l.IsActive && (CountryId > 0 ? l.CountryId == CountryId : true)).Get().OrderBy(l => l.LeaveDate).ToList();
        }
        public List<OfficialLeave> GetOfficialLeavesListNew(int CountryId)
        {
            return repoOfficialLeave.Query().Filter(l => l.IsActive && (CountryId > 0 ? l.CountryId == CountryId : true) && l.LeaveType != "Event").Get().OrderBy(l => l.LeaveDate).ToList();
        }
        public List<OfficialLeave> GetOfficialEventLeaveList(int CountryId)
        {
            return repoOfficialLeave.Query().Filter(l => l.IsActive == true && l.CountryId == CountryId && (l.LeaveType == "holiday" || l.LeaveType == "Event")).Get().OrderBy(l => l.LeaveDate).ToList();
        }
        public IQueryable<LeaveActivity> GetLeaveActivityByUidAndMonth(PagingService<LeaveActivity> pagingService) // List<LeaveActivity>
        {
            return repoLeaveActivity.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetQuerable();
        }
        public List<LeaveTypesMaster> GetLeaveCategoryList()
        {
            return repoLeaveTypesMaster.Query().Filter(x => x.IsActive == true).Get().OrderBy(x => x.Id).ToList();
        }
        public List<LeaveActivity> GetLeaveActivityByUidAndMonth(int year, int month, int roleId, int uid = 0, int leaveType = 0, int uidFilter = 0, int pmidFilter = 0)
        {

            var leaves = repoLeaveActivity.Query().Filter(x => (x.StartDate.Year == year || x.EndDate.Year == year) &&
                                            (x.StartDate.Month == month || x.EndDate.Month == month)
                                            && x.UserLogin.RoleId != (int)Enums.UserRoles.HRBP
                                            ).Get().ToList();

            //Get TL list of particular PM 
            int?[] tlList = repoUserMaster.Query().Filter(T => T.PMUid == (pmidFilter > 0 ? pmidFilter : uid)).Get().Select(T => T.TLId).Where(x => x != null).ToArray();

            if (roleId == (int)Enums.UserRoles.HRBP)
            {
                if (pmidFilter > 0 && uidFilter == 0)
                    leaves = leaves.Where(x => x.Uid == pmidFilter || x.UserLogin1.PMUid == pmidFilter).ToList();

                else if (uidFilter > 0)
                    leaves = leaves.Where(x => x.Uid == uidFilter).ToList();
            }

            else if (roleId == (int)Enums.UserRoles.PM || roleId == (int)Enums.UserRoles.PMO)
            {
                if (uidFilter > 0)
                    leaves = leaves.Where(x => x.Uid == uidFilter).ToList();
                else
                    leaves = leaves.Where(x => x.Uid == uid || x.UserLogin1.PMUid == uid).ToList();
            }
            else if (roleId == (int)Enums.UserRoles.UKPM || roleId == (int)Enums.UserRoles.UKBDM)
            {
                if (uidFilter > 0)
                    leaves = leaves.Where(x => x.Uid == uidFilter).ToList();
                else
                    leaves = leaves.Where(x => x.UserLogin1.PMUid == pmidFilter).ToList();
            }
            else
            {
                if (uidFilter > 0)
                    leaves = leaves.Where(x => x.Uid == uidFilter).ToList();
                else
                {
                    int[] sdList = repoUserMaster.Query().Filter(T => T.TLId == uid && T.IsActive == true).Get().Select(T => T.Uid).ToArray();
                    leaves = leaves.Where(x => x.Uid == uid || x.UserLogin1.TLId == uid || (sdList.Contains((int)x.UserLogin1.TLId))).ToList();
                }

            }

            if (leaveType > 0)
                leaves = leaves.Where(x => x.LeaveType == leaveType).ToList();
            return leaves;
        }

        public LeaveActivity GetLeaveActivityById(int LeaveActivityId)
        {
            return repoLeaveActivity.Query().Get().FirstOrDefault(x => x.LeaveId == LeaveActivityId);
        }

        public List<LeaveActivity> GetLeavesEmployeeListByUid(int ID, int _RoleId, bool IsActive = false)
        {
            if (_RoleId == (int)Enums.UserRoles.PM)
            {
                return repoLeaveActivity.Query().Filter(T => T.UserLogin1.IsActive == true && T.UserLogin1.PMUid == ID && T.UserLogin1.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.UserLogin1.Name).ToList();
            }
            else if (_RoleId == (int)Enums.UserRoles.HRBP)
            {
                return repoLeaveActivity.Query().Filter(T => T.UserLogin1.IsActive == true).Get().OrderBy(T => T.UserLogin1.Name).ToList();
            }
            else if (RoleValidator.TL_RoleIds.Contains(_RoleId))
            {
                int[] sdList = repoUserMaster.Query().Filter(T => T.TLId == ID || T.Uid == ID).Get().Select(T => T.Uid).ToArray();

                return repoLeaveActivity.Query().Filter(T => (T.UserLogin1.IsActive == true) && (T.UserLogin1.TLId == ID || sdList.Contains((int)T.UserLogin1.Uid)) && T.UserLogin1.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(A => A.UserLogin1.Name).ToList();
            }
            else if (RoleValidator.DV_RoleIds.Contains(_RoleId))
            {
                return repoLeaveActivity.Query().Filter(T => (T.UserLogin1.IsActive == true) && T.UserLogin1.TLId == ID && T.UserLogin1.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(A => A.UserLogin1.Name).ToList();
            }
            else
                return repoLeaveActivity.Query().Filter(T => T.UserLogin1.IsActive == true && T.UserLogin1.PMUid == ID && T.UserLogin1.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.UserLogin1.Name).ToList();
        }
        public List<LeaveActivity> GetLeavesEmployeeListByUidDesignation(int ID, int _RoleId, int _DesignationId, bool IsActive = false)
        {
            if (_RoleId == (int)Enums.UserRoles.PM)
            {
                return repoLeaveActivity.Query().Filter(T => T.UserLogin1.IsActive == true && T.UserLogin1.PMUid == ID && T.UserLogin1.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.UserLogin1.Name).ToList();
            }
            else if (_RoleId == (int)Enums.UserRoles.HRBP)
            {
                return repoLeaveActivity.Query().Filter(T => T.UserLogin1.IsActive == true).Get().OrderBy(T => T.UserLogin1.Name).ToList();
            }
            else if (RoleValidator.TL_Technical_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(_DesignationId)
            )
            {
                int[] sdList = repoUserMaster.Query().Filter(T => T.TLId == ID || T.Uid == ID).Get().Select(T => T.Uid).ToArray();

                return repoLeaveActivity.Query().Filter(T => (T.UserLogin1.IsActive == true) && (T.UserLogin1.TLId == ID || sdList.Contains((int)T.UserLogin1.Uid)) && T.UserLogin1.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(A => A.UserLogin1.Name).ToList();
            }
            else
                return repoLeaveActivity.Query().Filter(T => T.UserLogin1.IsActive == true && T.UserLogin1.PMUid == ID && T.UserLogin1.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.UserLogin1.Name).ToList();
        }

        public void SaveAdjustHour(List<LateHour> lateHour)
        {
            List<int> UpdateIds = new List<int>();
            List<int> DeleteIds = new List<int>();
            List<int> AddIds = new List<int>();
            if (lateHour.Any())
            {
                UpdateIds = lateHour.Where(s => s.Id > 0 && (s.EarlyLeaveTimeDiff.HasValue || s.LateStartTimeDiff.HasValue || !string.IsNullOrWhiteSpace(s.WorkAtHome) || !string.IsNullOrWhiteSpace(s.WorkFromHome))).Select(x => x.Id).ToList();
                DeleteIds = lateHour.Where(s => s.Id > 0 && !s.EarlyLeaveTimeDiff.HasValue && !s.LateStartTimeDiff.HasValue && string.IsNullOrWhiteSpace(s.WorkAtHome) && string.IsNullOrWhiteSpace(s.WorkFromHome)).Select(x => x.Id).ToList();
                AddIds = lateHour.Where(s => s.Id == 0 && (s.EarlyLeaveTimeDiff.HasValue || s.LateStartTimeDiff.HasValue || !(string.IsNullOrWhiteSpace(s.WorkAtHome)) || !(string.IsNullOrWhiteSpace(s.WorkFromHome)))).Select(x => x.Id).ToList();
            }
            if (UpdateIds.Any())
                repoLateHour.ChangeEntityCollectionState(lateHour.Where(x => UpdateIds.Contains(x.Id)).ToList(), ObjectState.Modified);
            if (DeleteIds.Any())
                repoLateHour.ChangeEntityCollectionState(lateHour.Where(x => DeleteIds.Contains(x.Id)).ToList(), ObjectState.Deleted);
            if (AddIds.Any())
                repoLateHour.ChangeEntityCollectionState(lateHour.Where(x => AddIds.Contains(x.Id)).ToList(), ObjectState.Added);
            repoLateHour.SaveChanges();
        }


        public List<LateHour> getLateHour(DateTime modified)
        {
            return repoLateHour.Query().Filter(x => x.DayOfDate == modified.Date).Get().ToList();
        }

        public List<LateHour> GetLateHourList()
        {
            return repoLateHour.Query().Get().ToList();
        }

        public IQueryable<LateHour> GetFilterLateHourList(PagingService<LateHour> pagingService)
        {
            return repoLateHour.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetQuerable();
        }
        public List<LateHour> GetLateHourList(Expression<Func<LateHour, bool>> expr)
        {
            return repoLateHour.Query().Filter(expr).Get().ToList();
        }

        public List<LateHour> GetLateHour(out int total, PagingService<LateHour> pagingService)
        {
            return repoLateHour.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetPage(pagingService.Start, pagingService.Length, out total)
                .ToList();
        }

        public LeaveActivity GetUserLastLeave(int Uid, DateTime timeSheetDate)
        {
            var leaves = repoLeaveActivity.Query().Filter(la => la.Uid == Uid && la.Status.HasValue && la.Status.Value == 7).Get().OrderByDescending(e => e.EndDate).ToList();
            if (leaves != null && leaves.Count > 0)
            {
                LeaveActivity obj = new LeaveActivity();
                var _leaves = leaves.Where(l => l.StartDate > timeSheetDate).OrderBy(l => l.StartDate).FirstOrDefault();
                if (_leaves != null)
                {
                    obj.StartDate = _leaves.StartDate;
                    obj.EndDate = _leaves.EndDate;
                }
                return obj;
            }
            return null;
        }

        public List<LeaveActivity> GetLeaveActivitiesInDuration(int uid, DateTime? startDate, DateTime? endDate)
        {
            var filter = PredicateBuilder.True<LeaveActivity>();
            filter = filter.And(x => x.Uid == uid
            && (x.Status == (int)Enums.LeaveStatus.Pending || x.Status == (int)Enums.LeaveStatus.Approved));
            if (startDate.HasValue && endDate.HasValue)
            {
                filter = filter.And((x => (x.StartDate >= startDate && x.StartDate <= endDate) // start date fall between specified range
                                    || (x.EndDate >= startDate && x.EndDate <= endDate) // end date fall between specified range
                                    || (x.StartDate <= startDate && x.EndDate >= endDate)// covers date range
                                    || (x.StartDate >= startDate && x.EndDate <= endDate) // Inside specified date range
                                    ));
            }
            else if (startDate.HasValue)
            {
                filter = filter.And(x => x.EndDate >= startDate); // ended after or equal filter start
            }
            else if (endDate.HasValue)
            {
                filter = filter.And(x => x.EndDate >= endDate && x.StartDate <= endDate);// ended after or equal filter end date and started equal or before filter and date
            }

            //return repoLeaveActivity.Query().Filter(x => x.Uid == uid
            //&& (x.Status == (int)Enums.LeaveStatus.Pending || x.Status == (int)Enums.LeaveStatus.Approved)
            //&& ((x.StartDate >= startDate && x.StartDate <= endDate) // start date fall between specified range
            //       || (x.EndDate >= startDate && x.EndDate <= endDate) // end date fall between specified range
            //       || (x.StartDate <= startDate && x.EndDate >= endDate)// covers date range
            //       || (x.StartDate >= startDate && x.EndDate <= endDate) // Inside specified date range
            //                        )).Get().ToList();

            return repoLeaveActivity.Query().Filter(filter).Get().ToList();
        }

        public List<LeaveActivity> GetLeaveActivitiesByDate(DateTime? AttendanceDate)
        {
            var filter = PredicateBuilder.True<LeaveActivity>();
            filter = filter.And(x => AttendanceDate >= x.StartDate && AttendanceDate <= x.EndDate);

            return repoLeaveActivity.Query().Filter(filter).Get().ToList();
        }

        #region "Dispose"
        public void Dispose()
        {
            if (repoLeaveActivity != null)
            {
                repoLeaveActivity.Dispose();
                repoLeaveActivity = null;
            }
            if (repoLeaveActivityAdjust != null)
            {
                repoLeaveActivityAdjust.Dispose();
                repoLeaveActivityAdjust = null;
            }
            if (repoLeaveAdjust != null)
            {
                repoLeaveAdjust.Dispose();
                repoLeaveAdjust = null;
            }
        }

        //public List<OfficialLeave> GetOfficialLeavesInDuration(DateTime startDate ,DateTime endDate ,byte CountryId,bool onlyHoliday=false)
        //{
        //    return repoOfficialLeave.Query().Filter(l => l.IsActive && (CountryId > 0 ? l.CountryId == CountryId :true) 
        //    && (l.LeaveDate>=startDate && l.LeaveDate <=endDate)
        //    && (onlyHoliday ? (l.LeaveType.ToLower() == "holiday") : true)).Get().OrderBy(l => l.LeaveDate).ToList();
        //}
        public List<OfficialLeave> GetOfficialLeavesInDuration(Expression<Func<OfficialLeave, bool>> expression)
        {
            return repoOfficialLeave.Query().Filter(expression).Get().OrderBy(l => l.LeaveDate).ToList();
        }
        #endregion

        public LeaveActivity GetFirstLeaveActivityForDate(DateTime dateFrom, DateTime dateTo, int uid)
        {
            return repoLeaveActivity.Query()
                .Filter(la => la.Uid == uid
                && la.Status != (int)Enums.LeaveStatus.Cancelled
                && (la.StartDate >= dateFrom && la.StartDate <= dateTo
                || la.EndDate >= dateFrom && la.EndDate <= dateTo
                || la.StartDate <= dateFrom && la.EndDate >= dateTo
                || la.StartDate >= dateFrom && la.EndDate <= dateTo)).Get().OrderByDescending(la => la.StartDate).FirstOrDefault();
        }

    }
}
