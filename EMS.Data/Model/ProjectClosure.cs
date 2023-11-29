using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectClosure
    {
        public ProjectClosure()
        {
            ProjectClosureDetails = new HashSet<ProjectClosureDetail>();
            ProjectClosureAbroadPm = new HashSet<ProjectClosureAbroadPm>();
        }

        public int Id { get; set; }
        public int? ProjectID { get; set; }
        public DateTime? DateofClosing { get; set; }
        public int Status { get; set; }
        public int? CRMStatus { get; set; }
        public int? Uid_Dev { get; set; }
        public string OtherActualDeveloper { get; set; }
        public int? Uid_BA { get; set; }
        public int? Uid_TL { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime? NextStartDate { get; set; }
        public string Reason { get; set; }
        public string Suggestion { get; set; }
        public int? ClientQuality { get; set; }
        public int? AddedBy { get; set; }
        public string Country { get; set; }
        public int? PMID { get; set; }
        public string ProjectLiveUrl { get; set; }
        public string ProjectUrlAbsenseReason { get; set; }
        public bool CRMUpdated { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? EstimateDays { get; set; }
        public bool? IsTimeMaterial { get; set; }
        public int? InvoiceDays { get; set; }
        public int? OldCrmstatus { get; set; }
        public double? BucketHours { get; set; }

        public int? ClientBadge { get; set; }
        public bool? IsCovid19 { get; set; }

        public DateTime? DeadResponseDate { get; set; }
        public bool? IsNewLeadGenerated { get; set; }


        public virtual UserLogin UserLogin4 { get; set; }
        public virtual UserLogin UserLogin1 { get; set; }
        public virtual Project Project { get; set; }
        public virtual UserLogin UserLogin { get; set; }
        public virtual UserLogin UserLogin2{ get; set; }
        public virtual UserLogin UserLogin3 { get; set; }
        public virtual ProjectClosureReview ProjectClosureReview { get; set; }
        public virtual ICollection<ProjectClosureDetail> ProjectClosureDetails { get; set; }
        public virtual ICollection<LeadTransaction> LeadTransaction { get; set; }

        public virtual ICollection<ProjectClosureAbroadPm> ProjectClosureAbroadPm { get; set; }
    }
}
