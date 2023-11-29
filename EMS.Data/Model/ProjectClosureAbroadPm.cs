using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectClosureAbroadPm
    {
        public int ProjectClosureId { get; set; }
        public int AbroadPmid { get; set; }
        
        public virtual AbroadPM AbroadPm { get; set; }
        public virtual ProjectClosure ProjectClosure { get; set; }
    }
}
