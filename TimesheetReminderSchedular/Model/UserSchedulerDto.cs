using EMS.Data;
using System;
using System.Collections.Generic;

namespace TimesheetReminderSchedular
{
    public class UserSchedulerDto
    {
        public int Uid { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public DateTime? TimeSheetDate { get; set; }
        public DateTime? LeaveEndDate { get; set; }
        public DateTime? LeaveStartDate { get; set; }
        public DateTime? JoinedDate { get; set; }
        public int? PMUid { get; set; }
        public int? TimesheetPrefrence { get; set; }
        public int? Role { get; set; }
        public bool Isactive { get; set; }
        public List<LeaveActivity> UserLeaves { get; set; }
    }
}
