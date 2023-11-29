using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LeadTransaction
    {
        public int TransId { get; set; }
        public int LeadId { get; set; }
        public string Doc { get; set; }
        public string Notes { get; set; }
        public int StatusId { get; set; }
        public int AddedBy { get; set; }
        public DateTime AddDate { get; set; }
        public int? ProjectClosureId { get; set; }
        public DateTime? ConversionDate { get; set; }

        public virtual UserLogin UserLogin { get; set; }
        public virtual ProjectLead Lead { get; set; }
        public virtual LeadStatu LeadStatu{ get; set; }

        public virtual ProjectClosure ProjectClosure { get; set; }
    }
}
