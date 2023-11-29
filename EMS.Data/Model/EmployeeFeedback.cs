using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EmployeeFeedback
    {
        public EmployeeFeedback()
        {
            EmployeeFeedbackRankStatus = new HashSet<EmployeeFeedbackRankStatus>();
            EmployeeFeedbackReasonMapping = new HashSet<EmployeeFeedbackReasonMapping>();
        }

        public int Id { get; set; }
        public int EmpUid { get; set; }
        public DateTime? LeavingDate { get; set; }
        public string Comment { get; set; }
        public int? EmpPmuid { get; set; }
        public string ReviewLink { get; set; }
        public bool Lfprofile { get; set; }
        public string Suggestion { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public bool EmailSkypePassReset { get; set; }
        public int? SaveBy { get; set; }
        public virtual UserLogin UserLogin { get; set; }
        public virtual ICollection<EmployeeFeedbackRankStatus> EmployeeFeedbackRankStatus { get; set; }
        public virtual ICollection<EmployeeFeedbackReasonMapping> EmployeeFeedbackReasonMapping { get; set; }
    }
}
