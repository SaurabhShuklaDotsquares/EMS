using System;
using System.Collections.Generic;
using System.Text;
using EMS.Data.saralDT;

namespace EMS.Service.SARALDT
{
    public interface ILevAllotmentDTService: IDisposable
    {
        List<LevAllotment> GetDTLeaveAllotmentByAttendanceId(int? AttendanceId);
        int GetDTMaxAllotmentValue(int AttendanceId);
    }
}
