using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class DesignerManagement
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string DesignerDesription { get; set; }
        public int AssignUid { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public DateTime AddDate { get; set; }
        public int? TaskTime { get; set; }
        public bool? IsPaid { get; set; }
        public int? AddedUid { get; set; }
        public DateTime? TaskCompletedDate { get; set; }

        public virtual UserLogin AddedU { get; set; }
        public virtual UserLogin AssignU { get; set; }
        public virtual Project Project { get; set; }
    }
}
