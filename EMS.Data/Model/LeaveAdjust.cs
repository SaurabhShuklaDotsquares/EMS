using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LeaveAdjust
    {
        public LeaveAdjust()
        {
            LeaveActivity = new HashSet<LeaveActivity>();
            LeaveActivityAdjust = new HashSet<LeaveActivityAdjust>();
        }

        public int AdjustId { get; set; }
        public int Uid { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Reason { get; set; }
        public bool? IsHalf { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateModify { get; set; }
        public bool? IsCancel { get; set; }
        public string Ip { get; set; }
        public bool? IsCl { get; set; }
        public bool? Isadjust { get; set; }
        public int? ClhalfAdjustId { get; set; }

        public virtual UserLogin U { get; set; }
        public virtual ICollection<LeaveActivity> LeaveActivity { get; set; }
        public virtual ICollection<LeaveActivityAdjust> LeaveActivityAdjust { get; set; }
    }
}
