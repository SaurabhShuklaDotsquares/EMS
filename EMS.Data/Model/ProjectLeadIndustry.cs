using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectLeadIndustry
    {
        public int ProjectLeadId { get; set; }
        public int IndustryId { get; set; }

        public virtual DomainType Industry { get; set; }
        public virtual ProjectLead ProjectLead { get; set; }
    }
}
