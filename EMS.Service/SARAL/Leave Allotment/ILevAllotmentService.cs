using System;
using System.Collections.Generic;
using System.Text;
using EMS.Data.saral;

namespace EMS.Service.SARAL
{
    public interface ILevAllotmentService: IDisposable
    {
        List<LevAllotment> GetLeaveAllotmentByAttendanceId(int? AttendanceId);
        int GetMaxAllotmentValue(int AttendanceId);
    }
}
