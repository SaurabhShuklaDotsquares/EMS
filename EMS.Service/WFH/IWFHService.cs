using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using System.Linq.Expressions;
namespace EMS.Service
{
    public interface IWFHService : IDisposable
    {

        List<Wfhactivity> GetWFH(out int total, PagingService<Wfhactivity> pagingService);
        Wfhactivity GetWFHById(int WFHId);
        int GetWFHCategory();
      //  IQueryable<LateHour> GetFilterLateHourList(PagingService<LateHour> pagingService);  
        List<Wfhactivity> GetWFHByUId(int uId);
        Preference GetPreferecesByPMUid(int pmId);      
        void Save(Wfhactivity entity);       
        List<Wfhactivity> GetWFHActivity(Expression<Func<Wfhactivity, bool>> expr);
        IQueryable<Wfhactivity> GetWFHActivityByUidAndMonth(PagingService<Wfhactivity> pagingService);
        List<Wfhactivity> GetWFHActivityByUidAndMonth(int year, int month, int roleId, int uid, int WFHCategory = 0, int uidFilter = 0, int pmidFilter = 0);
        Wfhactivity GetWFHActivityById(int id);
        List<Wfhactivity> GetWFHEmployeeListByUid(int ID, int _RoleId, int _DesignationId, bool IsActive = false);
        //Preference GetPreferencesByPmuid(int pmuid);
        bool CanApplyWFH(int userid, int WFHid, DateTime startDate, DateTime endDate);       
        List<WfhtypesMaster> GetWFHCategoryList();
        double GetTotalWFH(PagingService<Wfhactivity> pagingService);
        double GetApprovedWFH(int uid, int WFHCategory, DateTime startDate, DateTime endDate);
        double GetAllApprovedWFH(int uid, int WFHCategory);
        double GetWFH(int uid, int WFHCategory, DateTime startDate, DateTime endDate);
        double? GetPendingWFH(int uid, int WFHCategory = 0);
        double GetTotalApprovedPendingWFH(PagingService<Wfhactivity> pagingService);
        //void SaveAdjustHour(List<LateHour> lateHour);
        //List<LateHour> getLateHour(DateTime modified);
        //  List<LateHour> GetLateHourList();
        // List<LateHour> GetLateHourList(Expression<Func<LateHour, bool>> expr);
        // List<LateHour> GetLateHour(out int total, PagingService<LateHour> pagingService);
        Wfhactivity GetUserLastWFH(int Uid, DateTime timeSheetDate);       
        List<Wfhactivity> GetWFHActivitiesInDuration(int uid, DateTime? startDate, DateTime? endDate);
        List<Wfhactivity> GetWFHActivitiesByDate(DateTime? AtendnaceDate);
        Wfhactivity GetFirstWFHActivityForDate(DateTime dateFrom, DateTime dateTo, int uid);
    }
}
