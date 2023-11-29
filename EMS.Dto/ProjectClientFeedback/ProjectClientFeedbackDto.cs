using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class ProjectClientFeedbackDto
    {
        public List<SelectListItem> ProjectList { get; set; }
    }
    public class ProjectClientFeedbackDetailDto
    {
        public ProjectClientFeedbackDetailDto()
        {
            ProjectList = new List<SelectListItem>();
            Statuses = new List<SelectListItem>();
            ClientFeedbackDocuments = new List<ProjectClientFeedbackDocumentDto>();
        }
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string Comment { get; set; }
        public string WebsiteName { get; set; }
        public string WebUrl { get; set; }
        public string CompanyName { get; set; }
        public string ClientName { get; set; }
        public string BusinessDescription { get; set; }
        public string ProjectScope { get; set; }
        public string MeetRequirements { get; set; }
        public string ValueAboutDotsquares { get; set; }
        public string Commentdate { get; set; }
        public string Status { get; set; }

        public int ProjectId { get; set; }
        public List<SelectListItem> ProjectList { get; set; }

        public List<SelectListItem> Statuses { get; set; }

        public List<ProjectClientFeedbackDocumentDto> ClientFeedbackDocuments { get; set; }

    }
}
