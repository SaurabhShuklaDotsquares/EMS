using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectLeadArchive
    {
        public ProjectLeadArchive()
        {
            LeadTechnicianArchive = new HashSet<LeadTechnicianArchive>();
            LeadTransactionArchive = new HashSet<LeadTransactionArchive>();
            ProjectLeadTechArchive = new HashSet<ProjectLeadTechArchive>();
        }

        public int LeadId { get; set; }
        public int? LeadClientId { get; set; }
        public string Title { get; set; }
        public int TitleCheckSum { get; set; }
        public int OwnerId { get; set; }
        public int CommunicatorId { get; set; }
        public string Technologies { get; set; }
        public int AbroadPmid { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? QuoteSubmittedDate { get; set; }
        public string InitalRequirement { get; set; }
        public string Notes { get; set; }
        public int LeadType { get; set; }
        public int Status { get; set; }
        public string Conclusion { get; set; }
        public int ChaseRequests { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string Ip { get; set; }
        public bool? IsNewClient { get; set; }
        public bool IsUnread { get; set; }
        public int? EstimateTimeinDay { get; set; }
        public bool? Isdelivered { get; set; }
        public DateTime? StatusUpdateDate { get; set; }

        public virtual AbroadPM AbroadPm { get; set; }
        public virtual UserLogin Communicator { get; set; }
        public virtual LeadClient LeadClient { get; set; }
        public virtual TypeMaster LeadTypeNavigation { get; set; }
        public virtual UserLogin Owner { get; set; }
        public virtual LeadStatu StatusNavigation { get; set; }
        public virtual ICollection<LeadTechnicianArchive> LeadTechnicianArchive { get; set; }
        public virtual ICollection<LeadTransactionArchive> LeadTransactionArchive { get; set; }
        public virtual ICollection<ProjectLeadTechArchive> ProjectLeadTechArchive { get; set; }
    }
}
