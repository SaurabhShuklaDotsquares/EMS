using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace EMS.Dto
{
    public class ActivityDto
    {
        public ActivityDto()
        {
            Technologies = new List<ActivityCheckboxDto>();
            Specialist = new List<ActivityCheckboxDto>();
            Department = new List<ActivityCheckboxDto>();
            Status = new List<ActivityCheckboxDto>();
            Availability = new List<ActivityCheckboxDto>();
            PmFilter = new List<SelectListItem>();
            TeamLead = new List<SelectListItem>();
        }
        public List<ActivityCheckboxDto> Technologies { get; set; }
        public List<ActivityCheckboxDto> Specialist { get; set; }
        public List<ActivityCheckboxDto> Department { get; set; }
        public List<ActivityCheckboxDto> Status { get; set; }
        public List<ActivityCheckboxDto> Availability { get; set; }
        public List<SelectListItem> PmFilter { get; set; }
        public List<SelectListItem> TeamLead { get; set; }
        public List<ActivityGrid> DataGrid { get; set; }

        public string ActualDevlpers { get; set; }
        public string WorkDevlpers { get; set; }

        public List<DropdownListDto> SpecTypeList { get; set; }
        public List<DomainExpertDto> DomainExpert { get; set; }
        public string OtherTechnology { get; set; }
    }

    public class ActivityGrid
    {
        public int ActivityID { get; set; }
        public string Name { get; set; }
        public int? UserID { get; set; }
        public int? PmUID { get; set; }
        public bool IsResigned { get; set; }

        public int? ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectModel { get; set; }
        public string Status { get; set; }
        public string Specialist { get; set; }
        public string[] Specialisties { get; set; }
        public string[] Technologies { get; set; }
        public List<KeyValuePair<string, string>> TechnologiesWithSpecility { get; set; }
        public string[] TechSpecilization { get; set; }
        public List<string> ExpertExcelTechSpecilization { get; set; } = new List<string>();
        public List<string> BeignExcelTechSpecilization { get; set; } = new List<string>();
        public List<string> InterExcelTechSpecilization { get; set; } = new List<string>();
        public List<string> InteresExcelTechSpecilization { get; set; } = new List<string>();
        public string[] DomainExperts { get; set; }
        public string[] DomainExpertName { get; set; }
        public string LoginTime { get; set; }
        public string Comment { get; set; }
        public string AvailabilityStatus { get; set; }

        public string AdditionalSupportPeriod { get; set; }
        public string AdditionalSupportReason { get; set; }
        public Core.Enums.AddSupportRequestStatus? AdditionalSupportStatus { get; set; }

        public int? UserDepartmentID { get; set; }
        public string UserDepartmentName { get; set; }
        public Core.Enums.ProjectDepartment UserDepartment { get; set; }
        public string DeveloperStatus { get; set; }
        public int AvailabilityStatusOrder { get; set; }
        public string CRM_ProjectStatus { get; set; }
        public string TeamManager { get; set; }

        public string UserDesignation { get; set; }

        public int? PmRole { get; set; }
        public ActivityGrid()
        {
            AvailabilityStatusOrder = 100;
        }
        public int OnLeaveButRunning { get; set; }
        public int NotLogInButRunning { get; set; }
        public bool IsBucketProject { get; set; }
        public int SEOProjectRunningDev { get; set; }
        public bool IsSEOProject { get; set; }
        public int AssignedAddon { get; set; }
        public int RoleId { get; set; }
        public int DesignationId { get; set; }
        public string OtherTechnology { get; set; }
        public int RunningProjects { get; set; }
        public int RunningDevelopers { get; set; }
    }

    public class ActivityCheckboxDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ActivitySearch
    {
        public int[] department { get; set; }
        public string[] status { get; set; }
        public string[] domainexpert { get; set; }
        public string othertechnology { get; set; }
        public string[] specialist { get; set; }
        public string[] technologies { get; set; }
        public string[] Avail { get; set; }
        public string search { get; set; }
        public int leadId { get; set; }
        public int pmId { get; set; }
        public string activityDate { get; set; }
        public bool noticePeriod { get; set; }
        public int[] employeesdept { get; set; }
    }

    public class ActivityExcepExpoert
    {
        public string Name { get; set; }
        public string ProjectName { get; set; }
        public string Status { get; set; }
        public string Department { get; set; }
        public string RunningProjects { get; set; }
        public string RunningDevelopers { get; set; }
        public string Technology_Expert { get; set; }
        public string Technology_Intermediate { get; set; }
        public string Technology_Beginner { get; set; }
        public string Technology_Interested { get; set; }
        public string OtherTechnology { get; set; }
        public string DomainExpert { get; set; }
        public string AvailabilityStatus { get; set; }
        public bool IsActivityExcel { get; set; } = true;
    }

    public class EmployeeReportIndexDto
    {
        public EmployeeReportIndexDto()
        {
            PMUserList = new List<SelectListItem>();
            DepartmentList = new List<SelectListItem>();
            StatusList = new List<SelectListItem>();
        }

        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public List<SelectListItem> PMUserList { get; set; }
        public List<SelectListItem> DepartmentList { get; set; }
        public List<SelectListItem> StatusList { get; set; }
    }

    public class ActivityFreeReportDto
    {
        public ActivityFreeReportDto()
        {
            FreeDetails = new List<ActivityFreeDetaiDto>();
        }

        public int UserId { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }

        public string TeamManager { get; set; }

        public int TotalFreeDays { get; set; }

        public List<ActivityFreeDetaiDto> FreeDetails { get; set; }
    }

    public class ActivityLoginReportDto
    {
        public string Logintime { get; set; }
        public int day { get; set; }

    }

    public class ActivityEmpLoginReportDto
    {
        public ActivityEmpLoginReportDto()
        {
            LoginDetails = new List<ActivityLoginReportDto>();
        }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }

        public string TeamManager { get; set; }

        public List<ActivityLoginReportDto> LoginDetails { get; set; }

    }

    public class EmployeeAttendnaceDto
    {
        public int SNo { get; set; }
        public string Date { get; set; }
        public int? HRMId { get; set; }
        public string EmpCode { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string Status { get; set; }
        public string  Name { get; set; }
    }

    public class ActivityFreeDetaiDto
    {
        public string Date { get; set; }
        public string Description { get; set; }
        public string TimeSheet { get; set; }
    }

    public class RunningProjectsDto
    {
        public int PMUid { get; set; }
        public int TotalDevelopers { get; set; }
        public int UnassignedActualDevelopers { get; set; }

        public int[] UnassignedProjectIds { get; set; }
    }

    public class RunningProjectWithDeveloperDto
    {
        public int ProjectId { get; set; }
        public int CRMProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ModelName { get; set; }

        public List<AssignedDeveloperDto> ProjectDevelopers { get; set; }
    }

    public class AssignedDeveloperDto
    {
        public int ProjectId { get; set; }
        public int VirDevId { get; set; }
        public string VirDevName { get; set; }

        public int DevId { get; set; }
        public string DevName { get; set; }

        public string AddDate { get; set; }
    }

    public class BonusProjectDeveloperDto
    {
        public int DevId { get; set; }
        public string DevName { get; set; }
        public string Department { get; set; }

        public int VirDevId { get; set; }
        public string VirDevName { get; set; }

        public int ProjectId { get; set; }
        public int CRMProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ModelName { get; set; }

        public bool IsSelected { get; set; }
    }
}
