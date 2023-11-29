using System;
using System.Collections.Generic;
using System.Text;
using EMS.Data.saral;

namespace EMS.Service.SARAL
{
    public interface ILevMonthdetService : IDisposable
    {
        List<LevMonthdet> GetLeaveMonthDetailsByAttendanceId(int? AttendanceId);
        LevMonthdet GetLeaveMonthDetail(int? AttendanceId,int monthYear);
        void Save(LevMonthdet entity);
    }
}
