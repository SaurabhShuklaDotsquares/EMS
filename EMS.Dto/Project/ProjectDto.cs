using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;

namespace EMS.Dto
{
    public class ProjectDto
    {
        public int ProjectId { get; set; }
        [DisplayName("Name*")]
        public string Name { get; set; }
        public int CRMId { get; set; }

        [DisplayName("Client Id:")]
        public int? ClientId { get; set; }

        [DisplayName("Department")]
        public List<DropdownListDto> DepartmentList { get; set; }

        [DisplayName("Technology")]
        public List<DropdownListDto> TechnologyList { get; set; }
        [DisplayName("Model*")]
        public int Model { get; set; }

        [DisplayName("Estimate Time(Day's)")]
        public int EstimatedDays { get; set; }

        [DisplayName("Billing Team*")]
        public string BillingTeam { get; set; }

        [DisplayName("No. of Actual Developers")]
        public int ActualDevelopers { get; set; }
        [DisplayName("Start Date*")]
        public string StartDate { get; set; }

        [DisplayName("In-House Project")]
        public bool IsInHouse { get; set; }

        [DisplayName("Status*")]
        public string Status { get; set; }
        [DisplayName("Select PM")]
        public int PMUid { get; set; }
        public string Notes { get; set; }

        [DisplayName("Project Details")]
        public string ProjectDetailsDoc { get; set; }
        public string ModifyDate { get; set; }
        public string CreateDate { get; set; }
        public string[] Department { get; set; }
        public string[] Technology { get; set; }
        public string AddDate { get; set; }
        public List<SelectListItem> ModelList { get; set; }
        public List<SelectListItem> PMList { get; set; }
        public List<SelectListItem> ProjectStatusList { get; set; }
        public List<SelectListItem> TechnologySelectList { get; set; }
        public ProjectDto()
        {
            DepartmentList = new List<DropdownListDto>();
            TechnologyList = new List<DropdownListDto>();
            ModelList = new List<SelectListItem>();
            PMList = new List<SelectListItem>();
            ProjectStatusList = new List<SelectListItem>();
            TechnologySelectList = new List<SelectListItem>();
        }
    }
    public class ProjectIndexDto
    {
        public List<SelectListItem> PMList { get; set; }
        public List<SelectListItem> ProjectStatusList { get; set; }
        private bool _IsSuperAdmin = false;
        public bool IsSuperAdmin
        {
            get { return _IsSuperAdmin; }
            set { _IsSuperAdmin = value; }
        }
        private bool _IsHr = false;
        public bool IsHR
        {
            get { return _IsHr; }
            set { _IsHr = value; }
        }
        public ProjectIndexDto()
        {
            PMList = new List<SelectListItem>();
            ProjectStatusList = new List<SelectListItem>();
        }
    }

    public class ProjectDeveloperMapping
    {
        public int? Uid { get; set; }
        public int VirtualDeveloperID { get; set; }
        public string Remark { get; set; }
        public int Status { get; set; }
    }

    public class ProjectDeveloperDto
    {

        public List<SelectListItem> VirtualDeveloperList;
        public List<SelectListItem> DeveloperList;
        public List<SelectListItem> Status;
        public List<ProjectDeveloperMapping> projectDeveloperMappingList;
        public string ProjectId { get; set; }
        public ProjectDeveloperDto()
        {
            projectDeveloperMappingList = new List<ProjectDeveloperMapping>();
            DeveloperList = new List<SelectListItem>();
            Status = new List<SelectListItem>();
            VirtualDeveloperList = new List<SelectListItem>();
        }
    }
}
