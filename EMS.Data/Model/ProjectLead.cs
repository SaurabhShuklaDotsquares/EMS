using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectLead
    {
        public ProjectLead()
        {
            EstimateDocuments = new HashSet<EstimateDocument>();
            Forecastings = new HashSet<Forecasting>();
            LeadTechnicians = new HashSet<LeadTechnician>();
            LeadTransactions = new HashSet<LeadTransaction>();
            ProjectLeadTeches = new HashSet<ProjectLeadTech>();
            ProjectLeadIndustry = new HashSet<ProjectLeadIndustry>();
        }

        public int LeadId { get; set; }
        public int? LeadClientId { get; set; }
        public int OwnerId { get; set; }
        public int CommunicatorId { get; set; }
        public string Technologies { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? QuoteSubmittedDate { get; set; }
        public int Status { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string IP { get; set; }
        public string InitalRequirement { get; set; }
        public string Notes { get; set; }
        public int LeadType { get; set; }
        public string Conclusion { get; set; }
        public string Title { get; set; }
        public int TitleCheckSum { get; set; }
        public int AbroadPMID { get; set; }
        public int ChaseRequests { get; set; }
        public bool IsNewClient { get; set; }
        public bool IsUnread { get; set; }
        public int? EstimateTimeinDay { get; set; }
        public bool? Isdelivered { get; set; }
        public DateTime? StatusUpdateDate { get; set; }
        public DateTime? NextChasedDate { get; set; }
        public string Tag { get; set; }
        public int? PMID { get; set; }
        public string LeadCRMId { get; set; }
        public string ProposalDocument { get; set; }
        public string Remark { get; set; }
        public string MockupDocument { get; set; }
        public string OtherDocument { get; set; }
        public string Wireframe_MockupsDoc { get; set; }
        public bool? IsCovid19 { get; set; }
        public int? ProjectClosureId { get; set; }
        public DateTime? ConversionDate { get; set; }

        public virtual AbroadPM AbroadPM { get; set; }
        public virtual UserLogin UserLogin { get; set; }
        public virtual LeadClient LeadClient { get; set; }
        public virtual TypeMaster TypeMaster { get; set; }
        //public virtual UserLogin UserLogin { get; set; }
        public virtual UserLogin UserLogin1 { get; set; }

        public virtual LeadStatu LeadStatu { get; set; }
        public virtual ICollection<EstimateDocument> EstimateDocuments { get; set; }
        public virtual ICollection<Forecasting> Forecastings { get; set; }
        public virtual ICollection<LeadTechnician> LeadTechnicians { get; set; }
        public virtual ICollection<LeadTransaction> LeadTransactions { get; set; }
        public virtual ICollection<ProjectLeadTech> ProjectLeadTeches { get; set; }
        public virtual ICollection<ProjectLeadIndustry> ProjectLeadIndustry { get; set; }
    }
}
