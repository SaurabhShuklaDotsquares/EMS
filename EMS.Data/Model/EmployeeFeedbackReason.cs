using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EmployeeFeedbackReason
    {
        public EmployeeFeedbackReason()
        {
            EmployeeFeedbackRankStatus = new HashSet<EmployeeFeedbackRankStatus>();
            EmployeeFeedbackReasonMapping = new HashSet<EmployeeFeedbackReasonMapping>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }

        public virtual ICollection<EmployeeFeedbackRankStatus> EmployeeFeedbackRankStatus { get; set; }
        public virtual ICollection<EmployeeFeedbackReasonMapping> EmployeeFeedbackReasonMapping { get; set; }
    }
}
