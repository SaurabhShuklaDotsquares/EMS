using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LeadTechnicianArchive
    {
        public int AutoId { get; set; }
        public int LeadId { get; set; }
        public int TechnicianId { get; set; }

        public virtual ProjectLeadArchive Lead { get; set; }
        public virtual UserLogin Technician { get; set; }
    }
}
