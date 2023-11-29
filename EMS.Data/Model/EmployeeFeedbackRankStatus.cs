using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EmployeeFeedbackRankStatus
    {
        public int EmployeeFeedbackRankId { get; set; }
        public int EmployeeFeedbackId { get; set; }
        public byte FeedBackStatus { get; set; }

        public virtual EmployeeFeedback EmployeeFeedback { get; set; }
        public virtual EmployeeFeedbackReason EmployeeFeedbackRank { get; set; }
    }
}
