using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Escalation
    {
        public Escalation()
        {
            EscalationDocument = new HashSet<EscalationDocument>();
            EsculationForUser = new HashSet<EsculationForUser>();
            EsculationFoundForUser = new HashSet<EsculationFoundForUser>();
            EscalationConclusion = new HashSet<EscalationConclusion>();
        }

        public int Id { get; set; }
        public DateTime DateofEscalation { get; set; }
        public int? EscalationType { get; set; }
        public int SeverityLevel { get; set; }
        public string EscalationDescription { get; set; }
        public int ProjectId { get; set; }
        public int? RootCause { get; set; }
        public string RootCauseAnalysis { get; set; }
        public byte Status { get; set; }
        public bool? IsActive { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public int AddedByUid { get; set; }

        public virtual UserLogin AddedByU { get; set; }
        public virtual EscalationType EscalationTypeNavigation { get; set; }
        public virtual Project Project { get; set; }
        public virtual EscalationRootCause RootCauseNavigation { get; set; }
        public virtual ICollection<EscalationDocument> EscalationDocument { get; set; }
        public virtual ICollection<EsculationForUser> EsculationForUser { get; set; }
        public virtual ICollection<EsculationFoundForUser> EsculationFoundForUser { get; set; }
        public virtual ICollection<EscalationConclusion> EscalationConclusion { get; set; }
    }
}
