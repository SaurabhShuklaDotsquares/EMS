using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectClientFeedback
    {
        public ProjectClientFeedback()
        {
            ProjectClientFeedbackDocument = new HashSet<ProjectClientFeedbackDocument>();
        }
        public int Id { get; set; }
        public int? CrmfeedbackId { get; set; }
        public int ProjectId { get; set; }
        public string Comment { get; set; }
        public string WebsiteName { get; set; }
        public string WebUrl { get; set; }
        public string CompanyName { get; set; }
        public string ClientName { get; set; }
        public string BusinessDescription { get; set; }
        public string ProjectScope { get; set; }
        public string MeetRequirements { get; set; }
        public string ValueAboutDotsquares { get; set; }
        public DateTime? CommentDate { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Status { get; set; }
        public virtual Project Project { get; set; }
        public int? AddedBy { get; set; }
        public virtual UserLogin AddedByNavigation { get; set; }

        public virtual ICollection<ProjectClientFeedbackDocument> ProjectClientFeedbackDocument { get; set; }
    }
}
