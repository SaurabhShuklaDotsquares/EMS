using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class PILog
    {
        public int Id { get; set; }
        public string ProcessName { get; set; }
        public string PotentialArea { get; set; }
        public byte Status { get; set; }
        public string Remarks { get; set; }
        public DateTime? EstimatedSchedule { get; set; }
        public string CancelReason { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateByUid { get; set; }
        public DateTime ModifyDate { get; set; }
        public int ModifyByUid { get; set; }
        public int? MomMeetingTaskId { get; set; }
        public int ProcessId { get; set; }

        public virtual Process Process { get; set; }
        public virtual UserLogin UserLogin  { get; set; }
        public virtual UserLogin ModifyByU { get; set; }
        public virtual MomMeetingTask MomMeetingTask { get; set; }
    }
}
