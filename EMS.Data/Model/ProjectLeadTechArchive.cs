using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectLeadTechArchive
    {
        public int ProjectLeadTechId { get; set; }
        public int LeadId { get; set; }
        public int TechId { get; set; }

        public virtual ProjectLeadArchive Lead { get; set; }
        public virtual Technology Tech { get; set; }
    }
}
