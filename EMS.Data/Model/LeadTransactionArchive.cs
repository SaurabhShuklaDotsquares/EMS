using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LeadTransactionArchive
    {
        public int TransId { get; set; }
        public int LeadId { get; set; }
        public string Doc { get; set; }
        public string Notes { get; set; }
        public int StatusId { get; set; }
        public int AddedBy { get; set; }
        public DateTime AddDate { get; set; }

        public virtual UserLogin AddedByNavigation { get; set; }
        public virtual ProjectLeadArchive Lead { get; set; }
        public virtual LeadStatu Status { get; set; }
    }
}
