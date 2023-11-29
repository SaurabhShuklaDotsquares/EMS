using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ElanceAssignedJob
    {
        public decimal ElanceAssignedJobId { get; set; }
        public decimal ElanceJobId { get; set; }
        public int UserId { get; set; }

        public virtual ElanceJobDetails ElanceJob { get; set; }
        public virtual UserLogin User { get; set; }
    }
}
