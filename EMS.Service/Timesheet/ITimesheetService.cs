using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Dto;
using EMS.Repo.Search;

namespace EMS.Service
{
    public interface ITimesheetService : IDisposable
    {
        List<UserTimeSheet> GetTimesheetList(int uid);
        KeyValuePair<int, List<UserTimeSheet>> GetTimesheetListByUidWithPaging(int uid, int pageSize, int pageNumber, DateTime? dateFrom, DateTime? dateTo);

        void Save(UserTimeSheet entity);
        List<UserTimeSheet> GetTimesheetMonthly(DateTime dateFrom, DateTime dateTo, long userID);

        List<UserTimeSheet> GetTimeSheetByAllRole(int ID, int _RoleId, int _DesignationId);
        List<UserTimeSheet> GetProjectTimesheetListByUid(int uid);

        List<UserTimeSheet> GetTimeSheetPendingReviewsByUid(int Uid);
        List<UserTimeSheet> GetTimesheetListByTimesheetId(decimal[] UserTimeSheetIds);

        UserTimeSheet GetTimesheetByTimesheetId(decimal UserTimeSheetId);
        List<UserTimeSheet> GetTimesheetAccordingSearchingForExcel(DateTime? dateFrom, DateTime? dateTo, long projectID, long virtualdevID, long userID);

        AjaxResponseDto DeleteTimesheetById(decimal[] userTimeSheetIds);

        List<UserTimeSheet> GetTimesheetAccordingSearching(out int total, out TimeSpan totalTime, PagingService<UserTimeSheet> pagingService);
        UserTimeSheet GetLatestTimeSheetEntry(int id);
        List<GetMonthTimesheets_Result> GetMonthTimesheetsDataFromSP(DateTime startDate, DateTime endDate, string CRMIds);       
        List<GetMonthTimesheetsNew_Result> GetMonthTimesheetsDataFromSPNew(DateTime startDate, DateTime endDate, string CRMIds);       
        List<GetTimesheetsByTimesheetIds_Result> GetTimesheetsByIdSP(string timesheetIds, int? pageNumber, int? pageSize);
        List<UserTimeSheet> GetAllProjectUserTimeSheetByCRMId(int CrmId, DateTime? dtfrom, DateTime? dtTo);

        List<UserTimeSheet> GetAllTimeSheetByProjectCRMId(int CrmId, DateTime? dateFrom, DateTime? dateTo);
        PagedListResult<UserTimeSheet> GetTotalTimeSheetHours(SearchQuery<UserTimeSheet> query, out int totalItems);
        bool EmailIdExists(string email);
        IEnumerable<UserTimeSheet> GetAllTimeSheetHours(int month, int year, string userName);
        IEnumerable<UserTimeSheet> GetTimeSheetByProjectId(int ProjectId);
        List<UserTimeSheet> GetTimeSheetsByFilter(Expression<Func<UserTimeSheet, bool>> filter);
    }
}
