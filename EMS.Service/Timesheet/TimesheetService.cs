using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using EMS.Repo.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EMS.Service {
    public class TimesheetService : ITimesheetService {
        #region "Fields"
        private IRepository<UserTimeSheet> repoUserTimesheet;
        private IRepository<UserLogin> repoUserMaster;
        private IRepository<GetMonthTimesheets_Result> repoGetMonthTimesheetsSP;
        private IRepository<GetMonthTimesheetsNew_Result> repoGetMonthTimesheetsSPNew;
        private IRepository<GetTimesheetsByTimesheetIds_Result> repoGetTimesheetsByTimesheetIdsSP;
        #endregion

        #region "Cosntructor"
        public TimesheetService(IRepository<UserTimeSheet> _repoUserTimesheet, IRepository<UserLogin> _repoUserMaster
            , IRepository<GetMonthTimesheets_Result> _repoGetMonthTimesheetsSP, IRepository<GetMonthTimesheetsNew_Result> _repoGetMonthTimesheetsSPNew, IRepository<GetTimesheetsByTimesheetIds_Result> repoGetTimesheetsByTimesheetIdsSP) {
            repoUserTimesheet = _repoUserTimesheet;
            repoUserMaster = _repoUserMaster;
            repoGetMonthTimesheetsSP = _repoGetMonthTimesheetsSP;
            repoGetMonthTimesheetsSPNew = _repoGetMonthTimesheetsSPNew;
            this.repoGetTimesheetsByTimesheetIdsSP = repoGetTimesheetsByTimesheetIdsSP;
        }
        #endregion

        public List<UserTimeSheet> GetTimesheetList(int uid) {
            return repoUserTimesheet.Query().Filter(x => x.UID == uid).Get().OrderBy(T => T.InsertedDate).ToList();
        }

        public KeyValuePair<int, List<UserTimeSheet>> GetTimesheetListByUidWithPaging(int uid, int pageSize, int pageNumber, DateTime? dateFrom, DateTime? dateTo) {
            int totalCount = 0;
            List<UserTimeSheet> results = repoUserTimesheet.Query()
                  .Filter(x => x.UID == uid && (dateFrom != null ? x.AddDate >= dateFrom : true) && (dateTo != null ? x.AddDate <= dateTo : true))
                .OrderBy(T => T.OrderByDescending(o => o.AddDate))
                .GetPage(pageNumber, pageSize, out totalCount).OrderByDescending(x => x.AddDate).ThenByDescending(x => x.InsertedDate).ToList();

            KeyValuePair<int, List<UserTimeSheet>> resultResponse = new KeyValuePair<int, List<UserTimeSheet>>(totalCount, results);

            return resultResponse;
        }

        public List<UserTimeSheet> GetTimesheetMonthly(DateTime dateFrom, DateTime dateTo, long userID) {
            return repoUserTimesheet.Query().Get().Where(x => x.AddDate >= dateFrom && x.AddDate <= dateTo && x.UID == userID).OrderByDescending(x => x.AddDate).ToList();
        }

        public List<UserTimeSheet> GetTimesheetAccordingSearching(out int total, out TimeSpan totalTime, PagingService<UserTimeSheet> pagingService) {
            var records = repoUserTimesheet.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort);

            TimeSpan totalWorkingTime = totalTime = TimeSpan.Zero;
            if (pagingService.Start == 1) {
                records.GetQuerable().Select(x => x.WorkHours)
                    .ToList()
                    .ForEach(x => totalWorkingTime = totalWorkingTime.Add(x));
                totalTime = totalWorkingTime;
            }

            return records.GetPage(pagingService.Start, pagingService.Length, out total).ToList();
        }

        public List<UserTimeSheet> GetTimesheetAccordingSearchingForExcel(DateTime? dateFrom, DateTime? dateTo, long projectID, long virtualdevID, long userID) {
            return repoUserTimesheet.Query().Get().Where(x => (dateFrom != null ? x.AddDate >= dateFrom : true)
              && (dateTo != null ? x.AddDate <= dateTo : true) && (projectID > 0 ? x.ProjectID == projectID : true)
              && (virtualdevID > 0 ? x.VirtualDeveloper_id == virtualdevID : true)
              && (userID > 0 ? x.UID == userID : true)).OrderBy(A => A.IsReviewed).ThenByDescending(A => A.AddDate).ToList();
        }

        public void Save(UserTimeSheet entity) {
            if (entity.UserTimeSheetID == 0) {
                repoUserTimesheet.ChangeEntityState<UserTimeSheet>(entity, ObjectState.Added);
                entity.InsertedDate = DateTime.Now;
            }
            else {
                repoUserTimesheet.ChangeEntityState<UserTimeSheet>(entity, ObjectState.Modified);
            }
            //entity.Created = DateTime.UtcNow;
            entity.ModifyDate = DateTime.UtcNow;
            repoUserTimesheet.SaveChanges();
        }



        public List<UserTimeSheet> GetTimeSheetByAllRole(int ID, int _RoleId,int _DesignationId) {
            
            if (_RoleId == (int)Enums.UserRoles.PM) {
                return repoUserTimesheet.Query().Filter(T => T.UserLogin.IsActive == true && T.UserLogin.PMUid == ID && T.UserLogin.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderByDescending(T => T.AddDate).ThenByDescending(T => T.UserTimeSheetID).ToList();
            }
            else if (_RoleId == (int)Enums.UserRoles.HRBP) {
                return repoUserTimesheet.Query().Filter(T => T.UserLogin.IsActive == true).Get().OrderByDescending(T => T.AddDate).ThenByDescending(T => T.UserTimeSheetID).ToList();
            }
            else if (RoleValidator.TL_Technical_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(_DesignationId)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(_DesignationId)) {
                int[] sdList = repoUserMaster.Query().Filter(T => T.TLId == ID).Get().Select(T => T.Uid).ToArray();
                return repoUserTimesheet.Query().Filter(T => (T.UserLogin.IsActive == true) && (T.UserLogin.TLId == ID || sdList.Contains((int)T.UserLogin.TLId)) && T.UserLogin.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderByDescending(T => T.AddDate).ThenByDescending(T => T.UserTimeSheetID).ToList();
            }
            //else if (RoleValidator.DV_RoleIds.Contains(_RoleId)) {
            //    return repoUserTimesheet.Query().Filter(T => (T.UserLogin.IsActive == true) && T.UserLogin.TLId == ID && T.UserLogin.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderByDescending(T => T.AddDate).ThenByDescending(T => T.UserTimeSheetID).ToList();
            //}
            else {
                return repoUserTimesheet.Query().Filter(T => T.UserLogin.IsActive == true && T.UserLogin.PMUid == ID && T.UserLogin.RoleId != (int)Enums.UserRoles.HRBP).Get().OrderBy(T => T.UserLogin.Name).OrderByDescending(T => T.AddDate).ThenByDescending(T => T.UserTimeSheetID).ToList();
            }
        }

        #region Project TimeSheet Summary Report Methods Start... ...
        public List<UserTimeSheet> GetProjectTimesheetListByUid(int uid) {
            return repoUserTimesheet.Query()
                  .Filter(x => x.UID == uid).Get()
                  .OrderByDescending(T => T.AddDate).ToList();
        }
        #endregion


        #region TimeSheetReview
        public List<UserTimeSheet> GetTimeSheetPendingReviewsByUid(int Uid) {
            return repoUserTimesheet.Query().Filter(A => A.UID == Uid && A.IsReviewed == false).Get().OrderByDescending(A => A.AddDate).ThenByDescending(x => x.InsertedDate).ToList();
        }
        public List<UserTimeSheet> GetTimesheetListByTimesheetId(decimal[] UserTimeSheetIds) {
            return repoUserTimesheet.Query().Filter(A => UserTimeSheetIds.Contains(A.UserTimeSheetID) && A.IsReviewed == false).Get().OrderByDescending(A => A.AddDate).ThenByDescending(x => x.InsertedDate).ToList();
        }

        public UserTimeSheet GetTimesheetByTimesheetId(decimal UserTimeSheetId) {
            return repoUserTimesheet.Query().Filter(A => A.UserTimeSheetID == UserTimeSheetId).Get().FirstOrDefault();
        }

        public AjaxResponseDto DeleteTimesheetById(decimal[] userTimeSheetIds) {
            AjaxResponseDto ajaxResponseDto = new AjaxResponseDto();
            try {
                List<UserTimeSheet> usertimeshhet = repoUserTimesheet.Query().Filter(A => userTimeSheetIds.Contains(A.UserTimeSheetID)).Get().OrderByDescending(A => A.AddDate).ThenByDescending(x => x.InsertedDate).ToList();
                repoUserTimesheet.ChangeEntityCollectionState(usertimeshhet, ObjectState.Deleted);
                repoUserTimesheet.SaveChanges();
                ajaxResponseDto.Success = true;
                ajaxResponseDto.Message = "Record has been successfully deleted.";
            }
            catch (Exception ex) {
                ajaxResponseDto.Success = false;
                ajaxResponseDto.Message = ex.Message;
            }
            return ajaxResponseDto;
        }


        #endregion

        public UserTimeSheet GetLatestTimeSheetEntry(int id) {
            return repoUserTimesheet.Query()
                .Filter(u => u.UID == id)
                .OrderBy(o => o.OrderByDescending(x => x.AddDate))
                .GetQuerable().FirstOrDefault();
        }
        public List<GetMonthTimesheets_Result> GetMonthTimesheetsDataFromSP(DateTime startDate, DateTime endDate, string CRMIds) {
            return System.Threading.Tasks.Task.Run(() => repoGetMonthTimesheetsSP.GetDbContext().GetMonthTimesheets(startDate, endDate, CRMIds)).Result.ToList();
        }
        public List<GetMonthTimesheetsNew_Result> GetMonthTimesheetsDataFromSPNew(DateTime startDate, DateTime endDate, string CRMIds)
        {
            return System.Threading.Tasks.Task.Run(() => repoGetMonthTimesheetsSPNew.GetDbContext().GetMonthTimesheets_New(startDate, endDate, CRMIds)).Result.ToList();
        }

        public List<GetTimesheetsByTimesheetIds_Result> GetTimesheetsByIdSP(string timesheetIds, int? pageNumber, int? pageSize) {
            return repoGetTimesheetsByTimesheetIdsSP.GetDbContext().GetTimesheetsByTimesheetIds(timesheetIds, pageNumber.Value, pageSize.Value).Result.ToList();
        }

        public List<UserTimeSheet> GetAllProjectUserTimeSheetByCRMId(int CrmId, DateTime? dtfrom, DateTime? dtTo) {
            return repoUserTimesheet.Query().Filter(p => p.Project.CRMProjectId == CrmId && (dtfrom != null ? p.AddDate >= dtfrom : true) && (dtTo != null ? p.AddDate <= dtTo : true) && p.IsFillByPMS == false).Get().ToList();

        }

        public List<UserTimeSheet> GetAllTimeSheetByProjectCRMId(int CrmId, DateTime? dateFrom, DateTime? dateTo) {
            return repoUserTimesheet.Query().Filter(t => t.Project.CRMProjectId == CrmId && (dateFrom != null ? t.AddDate >= dateFrom : true) && (dateTo != null ? t.AddDate <= dateTo : true)).Get().ToList();
        }

        public PagedListResult<UserTimeSheet> GetTotalTimeSheetHours(SearchQuery<UserTimeSheet> query, out int totalItems) {
            return repoUserTimesheet.Search(query, out totalItems);
        }
        public IEnumerable<UserTimeSheet> GetAllTimeSheetHours(int month, int year, string email) {
            if (!string.IsNullOrEmpty(email) && month == 0 && year == 0) {
                return repoUserTimesheet.Query().Filter(t => t.IsFillByPMS == false && t.UserLogin1.EmailOffice == email).Get();
            }

            else if (!string.IsNullOrEmpty(email) && month > 0 && year == 0) {
                return repoUserTimesheet.Query().Filter(t => t.IsFillByPMS == false && t.UserLogin1.EmailOffice == email && t.AddDate.Month == month).Get();

            }
            else if (!string.IsNullOrEmpty(email) && month == 0 && year > 0) {
                return repoUserTimesheet.Query().Filter(t => t.IsFillByPMS == false && t.UserLogin1.EmailOffice == email && t.AddDate.Year == year).Get();

            }
            else if (string.IsNullOrEmpty(email) && month == 0 && year > 0) {
                return repoUserTimesheet.Query().Filter(t => t.IsFillByPMS == false && t.AddDate.Year == year).Get();

            }
            else {
                return repoUserTimesheet.Query().Filter(t => t.IsFillByPMS == false && t.UserLogin1.EmailOffice == email && t.AddDate.Month == month && t.AddDate.Year == year).Get();
            }
        }

        public IEnumerable<UserTimeSheet> GetTimeSheetByProjectId(int ProjectId)
        {
            return repoUserTimesheet.Query().Filter(t => t.ProjectID == ProjectId).Get();
        }

        public bool EmailIdExists(string email) {
            return repoUserTimesheet.Query().Get().Any(entity => entity.UserLogin1.EmailOffice.ToLower().Equals(email.ToLower()));
        }

        public List<UserTimeSheet> GetTimeSheetsByFilter(Expression<Func<UserTimeSheet,bool>> filter)
        {
            return repoUserTimesheet.Query()
                  .Filter(filter).Get()
                  .OrderByDescending(T => T.AddDate).ToList();
        }
        #region "Dispose"
        public void Dispose() {
            if (repoUserTimesheet != null) {
                repoUserTimesheet.Dispose();
                repoUserTimesheet = null;
            }
        }
        #endregion

    }
}
