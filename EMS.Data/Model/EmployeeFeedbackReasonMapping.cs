using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EmployeeFeedbackReasonMapping
    {
        public int EmployeeFeedbackId { get; set; }
        public int EmployeeFeedbackReasonId { get; set; }

        public virtual EmployeeFeedback EmployeeFeedback { get; set; }
        public virtual EmployeeFeedbackReason EmployeeFeedbackReason { get; set; }
    }
}
