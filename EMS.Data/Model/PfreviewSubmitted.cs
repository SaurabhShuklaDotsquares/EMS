using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class PfreviewSubmitted
    {
        public PfreviewSubmitted()
        {
            PfreviewResult = new HashSet<PfreviewResult>();
        }

        public int Id { get; set; }
        public int ReviewOnUid { get; set; }
        public int ReviewByUid { get; set; }
        public int ReviewQuarter { get; set; }
        public string Comments { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public bool? IsActive { get; set; }
        public int? ReviewYear { get; set; }
        public bool? IsSatisfied { get; set; }
        public decimal Score { get; set; }

        public virtual UserLogin ReviewByU { get; set; }
        public virtual UserLogin ReviewOnU { get; set; }
        public virtual PfreviewQuarter ReviewQuarterNavigation { get; set; }
        public virtual ICollection<PfreviewResult> PfreviewResult { get; set; }
    }
}
