using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Data.Model
{
    public partial class LateHourWFH
    {
        public int Id { get; set; }
        public DateTime DayOfDate { get; set; }
        public int Uid { get; set; }
        public TimeSpan? LateStartTimeDiff { get; set; }
        public TimeSpan? EarlyWFHTimeDiff { get; set; }
        public DateTime Modified { get; set; }
        public int ModifiedByUid { get; set; }
        public int WFHCategory { get; set; }
        public string WorkAtHome { get; set; }
        public string LateReason { get; set; }
        public string EarlyReason { get; set; }

        public virtual UserLogin UserLogin { get; set; }
        public virtual UserLogin UserLogin1 { get; set; }
        public string WorkFromHome { get; set; }
    }
}
