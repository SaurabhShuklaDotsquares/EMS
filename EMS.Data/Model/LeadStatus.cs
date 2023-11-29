using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LeadStatu
    {
        public LeadStatu()
        {
            LeadTransaction = new HashSet<LeadTransaction>();
            LeadTransactionArchive = new HashSet<LeadTransactionArchive>();
            ProjectLead = new HashSet<ProjectLead>();
            ProjectLeadArchive = new HashSet<ProjectLeadArchive>();
        }

        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public int? ParentId { get; set; }
        public string MailContent { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string IP { get; set; }
        public string FromEmail { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }

        public virtual ICollection<LeadTransaction> LeadTransaction { get; set; }
        public virtual ICollection<LeadTransactionArchive> LeadTransactionArchive { get; set; }
        public virtual ICollection<ProjectLead> ProjectLead { get; set; }
        public virtual ICollection<ProjectLeadArchive> ProjectLeadArchive { get; set; }
    }
}
