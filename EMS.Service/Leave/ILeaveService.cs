using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using System.Linq.Expressions;

namespace EMS.Service
{
    public interface ILeaveService : IDisposable
    {

        List<LeaveActivity> GetLeaves(out int total, PagingService<LeaveActivity> pagingService);
        LeaveActivity GetLeaveById(int LeaveId);
        IQueryable<LateHour> GetFilterLateHourList(PagingService<LateHour> pagingService);
        List<LeaveActivityAdjust> GetLeaveAdjustedById(int leaveId);
        List<LeaveAdjust> GetAdjustLeavesByUid(int uId);
        LeaveActivity GetLeaveByUIdDateRange(int uId, DateTime startDate, DateTime endDate);       
        List<LeaveActivity> GetLeaveByUId(int uId);
        Preference GetPreferecesByPMUid(int pmId);
        List<LeaveActivityAdjust> GetLeaveActivityAdjListById(int leavId);
        LeaveActivityAdjust GetLeaveActivityAdjById(int adjustId);
        void Save(LeaveActivity entity);
        void Save(LeaveAdjust entity);
        void Save(LeaveActivityAdjust entity);
        void Delete(LeaveActivityAdjust entity);
        List<LeaveActivity> GetLeaveActivity(Expression<Func<LeaveActivity, bool>> expr);
        IQueryable<LeaveActivity> GetLeaveActivityByUidAndMonth(PagingService<LeaveActivity> pagingService);
        List<LeaveActivity> GetLeaveActivityByUidAndMonth(int year, int month, int roleId, int uid, int leaveType = 0, int uidFilter = 0, int pmidFilter = 0);
        LeaveActivity GetLeaveActivityById(int id);
        List<LeaveActivity> GetLeavesEmployeeListByUid(int ID, int _RoleId, bool IsActive = false);
        List<LeaveActivity> GetLeavesEmployeeListByUidDesignation(int ID, int _RoleId, int _DesignationId, bool IsActive = false);
        Preference GetPreferencesByPmuid(int pmuid);
        bool CanApplyLeave(int userid, int leaveid, DateTime startDate, DateTime endDate);
        List<OfficialLeave> GetOfficialLeaves(byte CountryId, bool onlyHoliday = false);
        List<OfficialLeave> GetOfficialLeavesList(int CountryId);
        List<LeaveTypesMaster> GetLeaveCategoryList();
        double GetTotalLeaves(PagingService<LeaveActivity> pagingService);
        double GetApprovedLeaves(int uid,int leaveCategory, DateTime startDate, DateTime endDate);
        double GetApprovedLeaves(int uid, int leaveCategory, DateTime startDate, DateTime endDate, bool isPrev = false);
        double GetAllApprovedLeaves(int uid,int leaveCategory);
        double GetAllSickLeavesForYear(int uid, int leaveCategory);
        List<LeaveActivity> GetAllSICKLeaves(int uid, int leaveCategory);
        double GetLeaves(int uid,int leaveCategory, DateTime startDate, DateTime endDate);
        double? GetPendingLeaves(int uid,int leaveCategory=0, bool isPrev = false);
        double GetTotalApprovedPendingLeaves(PagingService<LeaveActivity> pagingService);        
        void SaveAdjustHour(List<LateHour> lateHour);      
        List<LateHour> getLateHour(DateTime modified);
        List<LateHour> GetLateHourList();
        List<LateHour> GetLateHourList(Expression<Func<LateHour, bool>> expr);
        List<LateHour> GetLateHour(out int total, PagingService<LateHour> pagingService);
        LeaveActivity GetUserLastLeave(int Uid, DateTime timeSheetDate);
        //List<OfficialLeave> GetOfficialLeavesInDuration(DateTime startDate, DateTime endDate, byte CountryId, bool onlyHoliday = false);
        List<OfficialLeave> GetOfficialLeavesInDuration(Expression<Func<OfficialLeave, bool>> expression);
        List<LeaveActivity> GetLeaveActivitiesInDuration(int uid, DateTime? startDate, DateTime? endDate);
        List<LeaveActivity> GetLeaveActivitiesByDate(DateTime? AtendnaceDate);
        LeaveActivity GetFirstLeaveActivityForDate(DateTime dateFrom, DateTime dateTo, int uid);
        List<OfficialLeave> GetOfficialLeavesListNew(int CountryId);
        List<OfficialLeave> GetOfficialEventLeaveList(int CountryId);
    }
}
