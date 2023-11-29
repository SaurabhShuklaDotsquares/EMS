using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectClientFeedbackDocument
    {
        public int Id { get; set; }
        public string DocumentPath { get; set; }
        public DateTime AddDate { get; set; }
        public int? FeedbackId { get; set; }

        public virtual ProjectClientFeedback Feedback { get; set; }
    }
}
