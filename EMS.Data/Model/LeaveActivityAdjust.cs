using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LeaveActivityAdjust
    {
        public int LeaveActivityAdjustId { get; set; }
        public int? LeaveId { get; set; }
        public int? Adjustid { get; set; }
        public DateTime AddDate { get; set; }

        public virtual LeaveAdjust Adjust { get; set; }
        public virtual LeaveActivity Leave { get; set; }
    }
}
