using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class UserTimeSheet
    {
        public decimal UserTimeSheetID { get; set; }
        public DateTime AddDate { get; set; }
        public int ProjectID { get; set; }
        public int? VirtualDeveloper_id { get; set; }
        public string Description { get; set; }
        public int UID { get; set; }
        public DateTime? ModifyDate { get; set; }
        public DateTime? InsertedDate { get; set; }
        public TimeSpan WorkHours { get; set; }
        public DateTime? ReviewedDate { get; set; }
        public int? ReviewedByUid { get; set; }
        public bool? IsReviewed { get; set; }
        public bool IsFillByPMS { get; set; }
        public virtual Project Project { get; set; }
        public virtual UserLogin UserLogin { get; set; }
        public virtual UserLogin UserLogin1 { get; set; }
        public virtual VirtualDeveloper VirtualDeveloper { get; set; }
    }
}
