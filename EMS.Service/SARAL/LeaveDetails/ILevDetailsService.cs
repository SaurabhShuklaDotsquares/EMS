using System;
using System.Collections.Generic;
using System.Text;
using EMS.Data.saral;

namespace EMS.Service.SARAL
{
    public interface ILevDetailsService : IDisposable
    {
        List<LevDetails> GetLeaveDetailsByAttendanceId(int? AttendanceId);
        List<LevDetails> GetLeaveDetailsByLeaveDate(int? AttendanceId, DateTime startDate, DateTime endDate);
        List<AttDefinition> GetLeaveTypeList();
        List<AttDefinition> GetLeaveTypeListByGender(string gender);
        MasEmployee GetEmployeeInfo(string email);
        void Save(LevDetails entity);
        void Delete(LevDetails entity);
        System.Data.DataTable GetLeaveBalance(int? AttendanceId, int monthYear);
        void SetUserDesignation(string EMPID, string EMPNAME, string DESIGNATION, string EMAILID, DateTime EFFFROM, string DBNAME);
    }
}
