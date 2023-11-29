using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class PfreviewQuarter
    {
        public PfreviewQuarter()
        {
            PfreviewSubmitted = new HashSet<PfreviewSubmitted>();
        }

        public int Id { get; set; }
        public string QuarterName { get; set; }
        public int StartMonth { get; set; }
        public int EndMonth { get; set; }
        public int? QuarterNumber { get; set; }

        public virtual ICollection<PfreviewSubmitted> PfreviewSubmitted { get; set; }
    }
}
