using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LateHour
    {
        public int Id { get; set; }
        public DateTime DayOfDate { get; set; }
        public int Uid { get; set; }
        public TimeSpan? LateStartTimeDiff { get; set; }
        public TimeSpan? EarlyLeaveTimeDiff { get; set; }
        public DateTime Modified { get; set; }
        public int ModifiedByUid { get; set; }
        public int LeaveType { get; set; }
        public string WorkAtHome { get; set; }
        public string LateReason { get; set; }
        public string EarlyReason { get; set; }

        public virtual UserLogin UserLogin { get; set; }
        public virtual UserLogin UserLogin1 { get; set; }
        public string WorkFromHome { get; set; }
    }
}
