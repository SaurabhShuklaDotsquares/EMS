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
    public class WFHService : IWFHService
    {
        #region "Fields"
        private IRepository<Wfhactivity> repoWFHActivity;   
        
        private IRepository<Preference> repoPreferences;
        private IRepository<UserLogin> repoUserMaster;
        private IRepository<WfhtypesMaster> repoWFHTypesMaster;
        #endregion

        #region "Cosntructor"
        public WFHService(IRepository<Wfhactivity> _repoWFHActivity,   IRepository<UserLogin> _repoUserMaster,IRepository<WfhtypesMaster> _repoWFHTypesMaster, IRepository<Preference> _repoPreferences)
        {
            this.repoWFHActivity = _repoWFHActivity;            
            this.repoUserMaster = _repoUserMaster; 
            this.repoPreferences= _repoPreferences;
            this.repoWFHTypesMaster = _repoWFHTypesMaster;

        }
        #endregion

        public List<Wfhactivity> GetWFH(out int total, PagingService<Wfhactivity> pagingService) // List<Wfhactivity>
        {
            return repoWFHActivity.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetPage(pagingService.Start, pagingService.Length, out total)
                .ToList();
        }

        public double GetTotalWFH(PagingService<Wfhactivity> pagingService)
        {
            return (repoWFHActivity.Query()
                .Filter(pagingService.Filter).GetQuerable().Select(l => new { l.StartDate, l.EndDate, l.IsHalf }).ToList().Sum(l => (l.IsHalf.HasValue && l.IsHalf.Value) ? (((l.EndDate - l.StartDate).TotalDays + 1) / 2) : ((l.EndDate - l.StartDate).TotalDays + 1)));
        }
        public double GetApprovedWFH(int uid, int WFHCategory, DateTime startDate, DateTime endDate)
        {
            return (repoWFHActivity.Query()
                .Filter(x => x.Uid == uid && x.Wfhcategory == WFHCategory && x.StartDate >= startDate && x.EndDate <= endDate && x.Status == (int)WFHStatus.Approved).GetQuerable().Select(l => new { l.StartDate, l.EndDate, l.IsHalf }).ToList().Sum(l => (l.IsHalf.HasValue && l.IsHalf.Value) ? (((l.EndDate - l.StartDate).TotalDays + 1) / 2) : ((l.EndDate - l.StartDate).TotalDays + 1)));
        }
        public double GetAllApprovedWFH(int uid, int WFHCategory)
        {
            return (repoWFHActivity.Query()
                .Filter(x => x.Uid == uid && x.Wfhcategory == WFHCategory && x.Status == (int)WFHStatus.Approved).GetQuerable().Select(l => new { l.StartDate, l.EndDate, l.IsHalf }).ToList().Sum(l => (l.IsHalf.HasValue && l.IsHalf.Value) ? (((l.EndDate - l.StartDate).TotalDays + 1) / 2) : ((l.EndDate - l.StartDate).TotalDays + 1)));
        }
        public double GetWFH(int uid, int WFHCategory, DateTime startDate, DateTime endDate)
        {
            return (repoWFHActivity.Query()
                .Filter(x => x.Uid == uid && x.Wfhcategory == WFHCategory && x.StartDate >= startDate && x.EndDate <= endDate && x.Status != (int)WFHStatus.Cancelled && x.Status != (int)WFHStatus.UnApproved).GetQuerable().Select(l => new { l.StartDate, l.EndDate, l.IsHalf }).ToList().Sum(l => (l.IsHalf.HasValue && l.IsHalf.Value) ? (((l.EndDate - l.StartDate).TotalDays + 1) / 2) : ((l.EndDate - l.StartDate).TotalDays + 1)));
        }

        public int GetWFHCategory()
        {
            return repoWFHTypesMaster.Query().Get().Select(x=>x.Id).FirstOrDefault();
        }

        public double? GetPendingWFH(int uid, int WFHCategory = 0)
        {
            if (WFHCategory > 0)
            {
                return (repoWFHActivity.Query()
                    .Filter(x => x.Uid == uid && x.Wfhcategory == WFHCategory && x.Status == (int)WFHStatus.Pending).GetQuerable().Select(l => new { l.StartDate, l.EndDate, l.IsHalf }).ToList().Sum(l => (l.IsHalf.HasValue && l.IsHalf.Value) ? (((l.EndDate - l.StartDate).TotalDays + 1) / 2) : ((l.EndDate - l.StartDate).TotalDays + 1)));
            }
            else
            {
                return (repoWFHActivity.Query()
                   .Filter(x => x.Uid == uid && x.Wfhcategory != (int)Enums.WFHCategory.Full && x.Status == (int)WFHStatus.Pending).GetQuerable().Select(l => new { l.StartDate, l.EndDate, l.IsHalf }).ToList().Sum(l => (l.IsHalf.HasValue && l.IsHalf.Value) ? (((l.EndDate - l.StartDate).TotalDays + 1) / 2) : ((l.EndDate - l.StartDate).TotalDays + 1)));
            }
        }

        public double GetTotalApprovedPendingWFH(PagingService<Wfhactivity> pagingService)
        {
            return (repoWFHActivity.Query()
                .Filter(pagingService.Filter).GetQuerable().Where(l => l.Status == (int)Enums.WFHStatus.Approved || l.Status == (int)Enums.WFHStatus.Pending)
                .Select(l => new { l.StartDate, l.EndDate, l.IsHalf }).ToList().Sum(l => (l.IsHalf.HasValue && l.IsHalf.Value) ? (((l.EndDate - l.StartDate).TotalDays + 1) / 2) : ((l.EndDate - l.StartDate).TotalDays + 1)));
        }

        public Wfhactivity GetWFHById(int WFHId)
        {
            return repoWFHActivity.FindById(WFHId);
        }      

        public List<Wfhactivity> GetWFHByUId(int uId)
        {
            return repoWFHActivity.Query().Filter(l => l.Uid == uId).Get().ToList();
        }

        public bool CanApplyWFH(int userid, int WFHid, DateTime startDate, DateTime endDate)
        {
            return repoWFHActivity.Query().Filter(l => l.Uid == userid && l.Wfhid != WFHid && (l.Status.HasValue ? (l.Status.Value == (int)Enums.WFHStatus.Approved || l.Status.Value == (int)Enums.WFHStatus.Pending) : true) && ((startDate >= l.StartDate && startDate <= l.EndDate) || (endDate >= l.StartDate && endDate <= l.EndDate) || (startDate <= l.StartDate && endDate >= l.EndDate))).Get().Count() == 0;
        }
        
        public Preference GetPreferecesByPMUid(int pmId)
        {
            return repoPreferences.Query().Filter(p => p.pmid.Value == pmId).Get().FirstOrDefault();
        }

        public List<Wfhactivity> GetWFHActivity(Expression<Func<Wfhactivity, bool>> expr)
        {
            return repoWFHActivity.Query().Filter(expr).Get().ToList();
        }

        public void Save(Wfhactivity entity)
        {
            if (entity.Status == 0)
            {
                entity.Status = (int)Enums.WFHStatus.Pending;
            }
            if (entity.Wfhid == 0)
            {
                repoWFHActivity.ChangeEntityState<Wfhactivity>(entity, ObjectState.Added);
                repoWFHActivity.InsertGraph(entity);
            }
            else
            {
                repoWFHActivity.ChangeEntityState<Wfhactivity>(entity, ObjectState.Modified);
                repoWFHActivity.SaveChanges();
            }
        }
        
        public IQueryable<Wfhactivity> GetWFHActivityByUidAndMonth(PagingService<Wfhactivity> pagingService) // List<WFHActivity>
        {
            return repoWFHActivity.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetQuerable();
        }
        public List<WfhtypesMaster> GetWFHCategoryList()
        {
            return repoWFHTypesMaster.Query().Filter(x => x.IsActive == true).Get().OrderBy(x => x.Id).ToList();
        }
        public List<Wfhactivity> GetWFHActivityByUidAndMonth(int year, int month, int roleId, int uid = 0, int WFHCategory = 0, int uidFilter = 0, int pmidFilter = 0)
        {

            var WFH = repoWFHActivity.Query().Filter(x => (x.StartDate.Year == year || x.EndDate.Year == year) &&
                                            (x.StartDate.Month == month || x.EndDate.Month == month)
                                            && x.U.RoleId != (int)Enums.UserRoles.HRBP
                                            ).Get().ToList();

            //Get TL list of particular PM 
            int?[] tlList = repoUserMaster.Query().Filter(T => T.PMUid == (pmidFilter > 0 ? pmidFilter : uid)).Get().Select(T => T.TLId).Where(x => x != null).ToArray();

            if (roleId == (int)Enums.UserRoles.HRBP)
            {
                if (pmidFilter > 0 && uidFilter == 0)
                    WFH = WFH.Where(x => x.Uid == pmidFilter || x.U.PMUid == pmidFilter).ToList();

                else if (uidFilter > 0)
                    WFH = WFH.Where(x => x.Uid == uidFilter).ToList();
            }

            else if (roleId == (int)Enums.UserRoles.PM || roleId == (int)Enums.UserRoles.PMO)
            {
                if (uidFilter > 0)
                    WFH = WFH.Where(x => x.Uid == uidFilter).ToList();
                else
                    WFH = WFH.Where(x => x.Uid == uid || x.U.PMUid == uid).ToList();
            }
            else if (roleId == (int)Enums.UserRoles.UKPM || roleId == (int)Enums.UserRoles.UKBDM)
            {
                if (uidFilter > 0)
                    WFH = WFH.Where(x => x.Uid == uidFilter).ToList();
                else
                    WFH = WFH.Where(x => x.U.PMUid == pmidFilter).ToList();
            }
            else
            {
                if (uidFilter > 0)
                    WFH = WFH.Where(x => x.Uid == uidFilter).ToList();
                else
                {
                    int[] sdList = repoUserMaster.Query().Filter(T => T.TLId == uid && T.IsActive == true).Get().Select(T => T.Uid).ToArray();
                    WFH = WFH.Where(x => x.Uid == uid || x.U.TLId == uid || (sdList.Contains((int)x.U.TLId))).ToList();
                }
            }

            if (WFHCategory > 0)
                WFH = WFH.Where(x => x.Wfhcategory == WFHCategory).ToList();
            return WFH;
        }

        public Wfhactivity GetWFHActivityById(int WFHActivityId)
        {
            return repoWFHActivity.Query().Get().FirstOrDefault(x => x.Wfhid == WFHActivityId);
        }

        public List<Wfhactivity> GetWFHEmployeeListByUid(int ID, int _RoleId, int _DesignationId, bool IsActive = false)
        {            
            if (_RoleId == (int)Enums.UserRoles.PM)
            {
                return repoWFHActivity.Query().Filter(T => T.U.IsActive == true && T.U.PMUid == ID && T.U.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.U.Name).ToList();
            }
            else if (_RoleId == (int)Enums.UserRoles.HRBP)
            {
                return repoWFHActivity.Query().Filter(T => T.U.IsActive == true).Get().OrderBy(T => T.U.Name).ToList();
            }
            else if (RoleValidator.TL_Technical_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(_DesignationId)           
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(_DesignationId))
            {
                int[] sdList = repoUserMaster.Query().Filter(T => T.TLId == ID || T.Uid == ID).Get().Select(T => T.Uid).ToArray();

                return repoWFHActivity.Query().Filter(T => (T.U.IsActive == true) && (T.U.TLId == ID || sdList.Contains((int)T.U.Uid)) && T.U.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(A => A.U.Name).ToList();
            }
            else if (RoleValidator.DV_RoleIds.Contains(_RoleId))
            {
                return repoWFHActivity.Query().Filter(T => (T.U.IsActive == true) && T.U.TLId == ID && T.U.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(A => A.U.Name).ToList();
            }
            else
                return repoWFHActivity.Query().Filter(T => T.U.IsActive == true && T.U.PMUid == ID && T.U.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.U.Name).ToList();
        }
           
        public Wfhactivity GetUserLastWFH(int Uid, DateTime timeSheetDate)
        {
            var WFH = repoWFHActivity.Query().Filter(la => la.Uid == Uid && la.Status.HasValue && la.Status.Value == 7).Get().OrderByDescending(e => e.EndDate).ToList();
            if (WFH != null && WFH.Count > 0)
            {
                Wfhactivity obj = new Wfhactivity(); 
                var _WFH = WFH.Where(l => l.StartDate > timeSheetDate).OrderBy(l => l.StartDate).FirstOrDefault();
                if (_WFH != null)
                {
                    obj.StartDate = _WFH.StartDate;
                    obj.EndDate = _WFH.EndDate;
                }
                return obj;
            }
            return null;
        }

        public List<Wfhactivity> GetWFHActivitiesInDuration(int uid, DateTime? startDate, DateTime? endDate)
        {
            var filter = PredicateBuilder.True<Wfhactivity>();
            filter = filter.And(x => x.Uid == uid
            && (x.Status == (int)Enums.WFHStatus.Pending || x.Status == (int)Enums.WFHStatus.Approved));
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

            return repoWFHActivity.Query().Filter(filter).Get().ToList();
        }

        public List<Wfhactivity> GetWFHActivitiesByDate(DateTime? AttendanceDate)
        {
            var filter = PredicateBuilder.True<Wfhactivity>();
            filter = filter.And(x => AttendanceDate >= x.StartDate && AttendanceDate <= x.EndDate);

            return repoWFHActivity.Query().Filter(filter).Get().ToList();
        }

        #region "Dispose"
        public void Dispose()
        {
            if (repoWFHActivity != null)
            {
                repoWFHActivity.Dispose();
                repoWFHActivity = null;
            }            
        }
               
        #endregion

        public Wfhactivity GetFirstWFHActivityForDate(DateTime dateFrom, DateTime dateTo, int uid)
        {
            return repoWFHActivity.Query()
                .Filter(la => la.Uid == uid
                && la.Status != (int)Enums.WFHStatus.Cancelled
                && (la.StartDate >= dateFrom && la.StartDate <= dateTo
                || la.EndDate >= dateFrom && la.EndDate <= dateTo
                || la.StartDate <= dateFrom && la.EndDate >= dateTo
                || la.StartDate >= dateFrom && la.EndDate <= dateTo)).Get().OrderByDescending(la => la.StartDate).FirstOrDefault();
        }
    }
}
