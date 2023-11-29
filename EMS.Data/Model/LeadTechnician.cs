using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LeadTechnician
    {
        public int AutoId { get; set; }
        public int LeadId { get; set; }
        public int TechnicianId { get; set; }

        public virtual ProjectLead Lead { get; set; }
        public virtual UserLogin UserLogin { get; set; }
    }
}
