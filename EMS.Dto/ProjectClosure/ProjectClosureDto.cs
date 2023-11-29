using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Core;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
    public class ProjectClosureIndexDto
    {
        public ProjectClosureIndexDto()
        {
            CRMStatus = new List<SelectListItem>();
            BAList = new List<SelectListItem>();
            TLList = new List<SelectListItem>();
            PMList = new List<SelectListItem>();
            ReviewPercentageList = new List<SelectListItem>();
            CountryList = new List<SelectListItem>();
        }

        public int PMUid { get; set; }
        public int Uid_BA { get; set; }
        public int Uid_TL { get; set; }
        public int CRMStatusId { get; set; }
        public Enums.CloserType? ProjectStatus { get; set; }

        public string DateFrom { get; set; }
        public string DateTo { get; set; }

        public bool IsDirector { get; set; }

        public int? ReviewPercentageId { get; set; }
        public Enums.ClientCountry Country { get; set; }

        public List<SelectListItem> CRMStatus { get; set; }
        public List<SelectListItem> BAList { get; set; }
        public List<SelectListItem> TLList { get; set; }
        public List<SelectListItem> PMList { get; set; }
        public List<SelectListItem> ReviewPercentageList { get; set; }
        public List<SelectListItem> CountryList { get; set; }
        //public ProjectionReportWeeksDto projectionReportWeeksDto { get; set; }
    }

    public class ProjectClosureDto
    {
        public ProjectClosureDto()
        {
            ProjectList = new List<SelectListItem>();
            CRMStatus = new List<SelectListItem>();
            ActualLeadDevelopers = new List<SelectListItem>();
            BAList = new List<SelectListItem>();
            TLList = new List<SelectListItem>();
            PMList = new List<SelectListItem>();
            AbroadPMModel = new AbroadPMModel();
            AbroadPMList = new List<AbroadPMModel>();
            //ProjectClosureAbroadPm = new List<ProjectClosureAbroadPmModel>();
        }
        public int Id { get; set; }
        public string DateOfClosing { get; set; }
        public string NextStartDate { get; set; }
        public List<SelectListItem> ProjectList { get; set; }
        public List<SelectListItem> CRMStatus { get; set; }
        public List<SelectListItem> ActualLeadDevelopers { get; set; }
        public List<SelectListItem> BAList { get; set; }
        public List<SelectListItem> TLList { get; set; }
        public List<SelectListItem> PMList { get; set; }
        public Enums.CloserType? ProjectStatus { get; set; }
        public bool LiveUrl { get; set; }
        public int Uid_Dev { get; set; }
        public int Uid_BA { get; set; }
        public int AddedBy { get; set; }
        public int PMUid { get; set; }
        public int Uid_TL { get; set; }
        public int ProjectID { get; set; }
        public DateTime Created { get; set; }
        public int Status { get; set; }
        public int CRMStatusId { get; set; }
        public string OtherActualDeveloper { get; set; }
        public string ProjectUrlAbsenseReason { get; set; }
        //[AllowHtml]
        public string Reason { get; set; }
        public string ProjectLiveUrl { get; set; }
        //[AllowHtml]
        public string Suggestion { get; set; }
        public Enums.ClientCountry? Country { get; set; }
        public Enums.ClientQualtiy? ClientQuality { get; set; }

        public bool CRMUpdated { get; set; }
        public string Referrer { get; set; }

        [DisplayName("Change Status")]
        public int ChangeStatusId { get; set; }
        public List<SelectListItem> ChangeStatus { get; set; }

        public Enums.ClientBadge ClientBadge { get; set; }
        public AbroadPMModel AbroadPMModel { get; set; }
        public List<AbroadPMModel> AbroadPMList { get; set; }
        public bool isAshishTeamMember { get; set; }
        public string ProjectName { get; set; }
        public string ClientName { get; set; }
        public string BATLName { get; set; }
        public bool IsCovid19 { get; set; }

        [DisplayName("Permanent Dead")]
        public bool IsPermanentDead { get; set; } = false;
        public string DeadResponseDate { get; set; }
    }

    public class BAConversionSummaryDto
    {
        public int BA_ID { get; set; }
        public string BAName { get; set; }
        public int Total { get; set; }
        public int Pending { get; set; }
        public int Converted { get; set; }
        public int Escalated { get; set; }
        public string Per { get; set; }
    }

    public class ProjectClosureSummaryDto
    {
        public int PMId { get; set; }
        public string PMName { get; set; }

        public int RecurringProjectClosed { get; set; }
        public int RecurringProjectRestarted { get; set; }

        public int ProjectNotStarted { get; set; }
        public int ProjectRestarted { get; set; }
        public int TotalProjectClosed { get; set; }

        public int ProjectPromising { get; set; }
        public int ProjectLessPromising { get; set; }
        public int ProjectNotSure { get; set; }
    }

    public class ProjectionReportWeeksDto
    {
        public ProjectionReportWeeksDto()
        {
            months = new List<ProjectionMonth>();
        }
        public List<ProjectionMonth> months { get; set; }
    }



    public class ProjectionMonth
    {
        public ProjectionMonth()
        {
            Weeks = new List<ProjectionWeek>();
        }
        public List<ProjectionWeek> Weeks { get; set; }
        public string MonthName { get; set; }
        public int Month { get; set; }
        public int Colspan { get; set; }
        public string Color { get; set; }
        public int TotalMonthOccupancy { get; set; }
    }
    public class ProjectionWeek
    {
        public int Month { get; set; }
        public int WeekNo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string StartDateEndDate { get; set; }

        public int DeveloperCount { get; set; }

        public int ConvertedCount { get; set; }
    }


    public class AbroadPMModel {
   
        public int AutoId { get; set; }
        public string GroupName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Selected { get; set; }

        public List<AbroadPMModel> AbroadPMList { get; set; }
    }
    //public class ProjectClosureAbroadPmModel
    //{
   
    //    public string GroupName { get; set; }
    //    public string Name { get; set; }
    //    public string Email { get; set; }
    //    public bool Selected { get; set; }

    //    public List<ProjectClosureAbroadPmModel> ProjectClosureAbroadPmList { get; set; }
    //}


    //public class ProjectionReportWeeksDto
    //{
    //    public ProjectionReportWeeksDto()
    //    {
    //        weeks = new List<ProjectionWeek>();
    //    }
    //    public List<ProjectionWeek> weeks { get; set; }
    //}



    //public class ProjectionWeek
    //{
    //    public int Month { get; set; }
    //    public int WeekNo { get; set; }
    //    public DateTime StartDate { get; set; }
    //    public DateTime EndDate { get; set; }

    //    public string StartDateEndDate { get; set; }

    //    public int DeveloperCount { get; set; }

    //}
}
