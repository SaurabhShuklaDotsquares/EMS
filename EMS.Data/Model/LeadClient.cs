using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LeadClient
    {
        public LeadClient()
        {
            ProjectLead = new HashSet<ProjectLead>();
            ProjectLeadArchive = new HashSet<ProjectLeadArchive>();
        }

        public int LeadClientId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int PMUid { get; set; }

        public virtual ICollection<ProjectLead> ProjectLead { get; set; }
        public virtual ICollection<ProjectLeadArchive> ProjectLeadArchive { get; set; }
    }
}
