using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class HalfLeave
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? AppraisalId { get; set; }
        public int? TotalLeave { get; set; }
        public int? Approved { get; set; }
        public int? NotApproved { get; set; }
        public int? Adjustment { get; set; }
        public DateTime? AddDate { get; set; }
        public string Ip { get; set; }

        public virtual Appraisal Appraisal { get; set; }
        public virtual UserLogin User { get; set; }
    }
}
