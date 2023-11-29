using EMS.Core;
using EMS.Data;

using EMS.Data.Model;
using EMS.Dto;
using System;
using System.Collections.Generic;

namespace EMS.Service
{
    public interface IUserActivityService : IDisposable
    {
        UserActivity GetUserActivityByUid(int Uid, DateTime? date = null);
        double GetUserActivityByUidDateRange(int Uid, DateTime? dateFrom, DateTime? dateTo);
        List<UserLiteActivityLogDto> GetUserActivityLog(out int total, PagingService<UserActivityLog> pagingService, bool descOrder = true);
        List<ActivityFreeReportDto> GetUserActivityLog(out int total, int noOfFreeDays, PagingService<UserActivityLog> pagingService);

        ActivityFreeReportDto GetUserFreeActivityDetails(int uid, DateTime startDate, DateTime endDate, List<DateTime> excludingDates);

        void GetTotalUserActivityLog(out int totalpaid, out int totalfree, PagingService<UserActivityLog> pagingService);
        void Save(UserActivity entity);
        void Delete(UserActivity entity);
        List<UserActivity> GetUserActivityForResourcePoolByDepartment(int pmuid, int? deptId = null);
        List<BucketModel> GetBucketModelsForResourcePool();
        List<ActivityGrid> GetActivities(DateTime date, int exceptUserID, int PMUid);
        List<ActivityGrid> GetActivitiesnew(DateTime date, int exceptUserID, int PMUid);

        List<UserActivity> GetUserActivitiesdataBetweenTwoDate(DateTime fromDate, DateTime toDate);

        void DeleteUserActivityOfLastDay();
        void GetTotalPaidDays(int uid, DateTime startDate, DateTime endDate, List<DateTime> excludingDates, out int paidDays);
        List<ActivityFreeDetaiDto> GetUserActivityDetails(int uid, DateTime startDate, DateTime endDate, List<DateTime> excludingDates, Enums.ActivityDetail activityDetail);
        List<UserActivityLog> GetActivitiesInDuration(int uid, DateTime? startDate, DateTime? endDate, List<DateTime> excludingDates);
        List<UserActivityLog> GetUserLoginActivityDetails(int month, int year);
        List<UserActivityLog> GetUserLoginActivityDetails(DateTime? AttendanceDate);

        List<TeamStatusReportGraph_Result> GetTeamStatusReportGraph(int TLId, DateTime? startDate, DateTime? endDate);
        List<AllTeamStatusReportGraph_Result> GetAllTeamStatusReportGraph(int PMUid, DateTime? startDate, DateTime? endDate);
        List<TeamStatusReportGraphDetail_Result> GetTeamStatusReportGraphDetails(int TLId, DateTime? selectedDate);
        List<TeamStatusReportGraphDetail_Result> GetAllTeamStatusReportGraphDetails(int TLId, DateTime? selectedDate);
        UserActivity GetUserActivityByUidDesc(int Uid);
    }
}
