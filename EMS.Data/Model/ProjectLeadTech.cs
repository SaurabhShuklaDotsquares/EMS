using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EMS.Data
{
    public partial class ProjectLeadTech
    {
        [Key]
        public int ProjectLeadTechId { get; set; }
        public int LeadId { get; set; }
        public int TechId { get; set; }

        public virtual ProjectLead Lead { get; set; }
        public virtual Technology Technology { get; set; }
    }
}
