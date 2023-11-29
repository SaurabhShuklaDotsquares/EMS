using System;
using System.Collections.Generic;
using System.Text;
using EMS.Data.saralDT;

namespace EMS.Service.SARALDT
{
    public interface ILevMonthdetDTService : IDisposable
    {
        List<LevMonthdet> GetLeaveMonthDetailsByAttendanceId(int? AttendanceId);
        LevMonthdet GetLeaveMonthDetail(int? AttendanceId,int monthYear);
        void Save(LevMonthdet entity);
    }
}
