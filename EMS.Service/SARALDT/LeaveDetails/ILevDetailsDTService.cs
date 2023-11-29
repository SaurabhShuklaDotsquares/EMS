using System;
using System.Collections.Generic;
using System.Text;
using EMS.Data.saralDT;

namespace EMS.Service.SARALDT
{
    public interface ILevDetailsDTService : IDisposable
    {
        List<LevDetails> GetDTLeaveDetailsByAttendanceId(int? AttendanceId);
        List<LevDetails> GetDTLeaveDetailsByLeaveDate(int? AttendanceId, DateTime startDate, DateTime endDate);
        List<AttDefinition> GetDTLeaveTypeList();
        List<AttDefinition> GetDTLeaveTypeListByGender(string gender);
        MasEmployee GetDTEmployeeInfo(string email);
        void Save(LevDetails entity);
        void Delete(LevDetails entity);
        System.Data.DataTable GetDTLeaveBalance(int? AttendanceId, int monthYear);
        void SetDTUserDesignation(string EMPID, string EMPNAME, string DESIGNATION, string EMAILID, DateTime EFFFROM, string DBNAME);
    }
}
