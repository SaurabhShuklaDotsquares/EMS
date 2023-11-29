using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectPm
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }

        public virtual Project Project { get; set; }
        public virtual UserLogin User { get; set; }
    }
}
