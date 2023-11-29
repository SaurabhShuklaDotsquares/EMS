using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using EMS.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
    public class EscalationDto
    {
        public EscalationDto()
        {
            EscalationForUserList = new List<SelectListItem>();
            EscalationFoundForUserList = new List<SelectListItem>();
            EscalationDocumentsList = new List<EscalationDocumentDto>();
            EscalationRootCauseList = new List<SelectListItem>();
            EscalationTypeList = new List<SelectListItem>();
            Project = new List<SelectListItem>();
        }

        [DisplayName("Severity Level")]
        public List<SelectListItem> EscalationForUserList { get; set; }
        [DisplayName("Severity Level")]
        public List<SelectListItem> EscalationFoundForUserList { get; set; }
        public List<SelectListItem> EscalationRootCauseList { get; set; }
        public List<SelectListItem> EscalationTypeList { get; set; }
        public List<EscalationDocumentDto> EscalationDocumentsList { get; set; }
        public List<SelectListItem> Project { get; set; }

        public List<EscalationUserDTO> EscalationUsers { get; set; }
        public List<int> DeletedEscalationUsers { get; set; } = new List<int>();
        public int Id { get; set; }

        [DisplayName("Escalation Date")]
        public string DateofEscalation { get; set; }

        [DisplayName("Type")]
        public int EscalationType { get; set; }

        [DisplayName("Severity Level")]
        public int SeverityLevel { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; }

        [DisplayName("Details")]
        public string EscalationDetails { get; set; }

        [DisplayName("Images")]
        public List<IFormFile> EscalationImages { get; set; }

        [DisplayName("Escalation Received For")]
        public string[] ReceivedFor { get; set; }

        [DisplayName("Status")]
        public byte Status { get; set; }

        [DisplayName("Root Cause Analysis")]
        public int RootCauseAnalysisId { get; set; }

        [DisplayName("Root Cause Description")]
        public string RootCauseAnalysisDesctiption { get; set; }

        [DisplayName("Escalation Found")]
        public string[] EscalationFound { get; set; }

        public DateTime AddDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public bool SendEmail { get; set; }
        public int AddedByUid { get; set; }
    }
    public class EscalationDocumentDto
    {
        public int Id { get; set; }
        public int EscalationId { get; set; }
        public string DocumentPath { get; set; }
    }

    public class EscalationUserDTO
    {
        public int EscalationId { get; set; }
        public int UserId { get; set; }
    }

    public class EscalationConclusionDTO
    {
        public int Id { get; set; }
        public int AddedByUid { get; set; }
        [DisplayName("Lesson Learn Explanation")]
        public string LessonLearnExplanation { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string Resolution { get; set; }
        public int EscalationId { get; set; }
    }
}
