using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LeaveActivity
    {
        public LeaveActivity()
        {
            LeaveActivityAdjust = new HashSet<LeaveActivityAdjust>();
        }

        public int LeaveId { get; set; }
        public int Uid { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public int? Status { get; set; }
        public string WorkAlternator { get; set; }
        public string Remark { get; set; }
        public bool? IsHalf { get; set; }
        public bool? FirstHalf { get; set; }
        public bool? SecondHalf { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateModify { get; set; }
        public int? WorkAlterId { get; set; }
        public int? LeaveType { get; set; }
        public string Ip { get; set; }
        public int? AdjustId { get; set; }
        public int? ModifyBy { get; set; }


        public int? HolidayType { get; set; }
        public int? LeaveCategory { get; set; }
        public virtual UserLogin ModifyByNavigation { get; set; }
        public virtual LeaveTypesMaster LeaveCategoryNavigation { get; set; }

        public virtual LeaveAdjust Adjust { get; set; }
        public virtual TypeMaster TypeMaster { get; set; }
        public virtual TypeMaster TypeMaster1 { get; set; }
        public virtual UserLogin UserLogin1 { get; set; }
        public virtual UserLogin UserLogin { get; set; }
        public virtual ICollection<LeaveActivityAdjust> LeaveActivityAdjust { get; set; }
    }
}
