using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class AbroadPM
    {
        public AbroadPM()
        {
            ProjectLead = new HashSet<ProjectLead>();
            ProjectLeadArchive = new HashSet<ProjectLeadArchive>();
            ProjectClosureAbroadPm = new HashSet<ProjectClosureAbroadPm>();
        }

        public int AutoID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public bool isDefaultForEmail { get; set; }
        public bool isActive { get; set; }
        public int? Uid { get; set; }

        public virtual ICollection<ProjectLead> ProjectLead { get; set; }
        public virtual ICollection<ProjectLeadArchive> ProjectLeadArchive { get; set; }

        public virtual UserLogin UserLogin { get; set; }
        public virtual ICollection<ProjectClosureAbroadPm> ProjectClosureAbroadPm { get; set; }
    }
}
