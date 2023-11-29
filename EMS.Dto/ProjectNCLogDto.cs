using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
    public class ProjectNCLogDto
    {
        public ProjectNCLogDto()
        {
            AuditPAList = new List<SelectListItem>();
            ProjectList = new List<SelectListItem>();
            AuditeeList = new List<SelectListItem>();
            AuditCycleList = new List<SelectListItem>();
            AuditStatusList = new List<SelectListItem>();
            AuditTypeList = new List<SelectListItem>();
        }
        
        public int Id { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; }

        [DisplayName("Audit Cycle")]
        public byte AuditCycle { get; set; }

        [DisplayName("NC Type")]
        public byte AuditType { get; set; }

        [DisplayName("Audit Process Area")]
        public int? ProjectAuditPAId { get; set; }

        [DisplayName("Description of NC")]
        public string AuditDesc { get; set; }

        [DisplayName("Date of Audit")]
        public string AuditDate { get; set; }

        [DisplayName("Auditee")]
        public int AuditeeUid { get; set; }

        [DisplayName("Follow-up Date")]
        public string FollowUpDate { get; set; }
        
        [DisplayName("Status")]
        public byte Status { get; set; }

        [DisplayName("Closed Date")]
        public string ClosedDate { get; set; }

        public int CurrentUserId { get; set; }

        public List<SelectListItem> AuditPAList { get; set; }
        public List<SelectListItem> ProjectList { get; set; }
        public List<SelectListItem> AuditeeList { get; set; }
        public List<SelectListItem> AuditCycleList { get; set; }
        public List<SelectListItem> AuditStatusList { get; set; }
        public List<SelectListItem> AuditTypeList { get; set; }
    }

    public class ProjectNCLogAuditeeDto
    {
        public ProjectNCLogAuditeeDto()
        {
            AuditStatusList = new List<SelectListItem>();
        }

        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string AuditCycle { get; set; }

        [DisplayName("NC Type")]
        public string AuditType { get; set; }

        [DisplayName("Audit Process Area")]
        public string ProjectAuditPA { get; set; }

        [DisplayName("Description of NC")]
        public string AuditDesc { get; set; }

        [DisplayName("Auditor")]
        public string AuditorName { get; set; }

        [DisplayName("Auditee")]
        public string AuditeeName { get; set; }

        [DisplayName("Date of Audit")]
        public string AuditDate { get; set; }
        public string FollowUpDate { get; set; }

        [DisplayName("Date of Complete")]
        public string CompletedDate { get; set; }
        public string ClosedDate { get; set; }
        
        [DisplayName("Root Cause Analysis")]
        public string RootCause { get; set; }

        [DisplayName("Corrective/ Preventive Action")]
        public string AuditAction { get; set; }


        public byte Status { get; set; }
       
        public int CurrentUserId { get; set; }

        public bool EditAllowed { get; set; }

        public bool CloseAllowed { get; set; }

        public List<SelectListItem> AuditStatusList { get; set; }
    }

    public class ProjectNCLogIndexDto
    {
        public bool ShowAddNewOption { get; set; }
        public List<SelectListItem> PMUserList { get; set; }
        public List<SelectListItem> ProjectList { get; set; }

        public ProjectNCLogIndexDto()
        {
            PMUserList = new List<SelectListItem>();
            ProjectList = new List<SelectListItem>();
        }
    }
}
