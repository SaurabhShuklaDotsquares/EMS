using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectOtherPm
    {
        public int ProjectId { get; set; }
        public int Pmuid { get; set; }

        public virtual UserLogin Pmu { get; set; }
        public virtual Project Project { get; set; }
    }
}
