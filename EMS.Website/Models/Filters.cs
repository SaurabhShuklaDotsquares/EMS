using EMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EMS.Core.Enums;

namespace EMS.Web.Modals
{
    public class ProjectClosureSearchFilter
    {
        public string textSearch { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int? chaseStatus { get; set; }
        public int? BA { get; set; }
        public int? TL { get; set; }
        public int? CRMStatusId { get; set; }
        public int ProjectStatus { get; set; }
        public int? PMUid { get; set; }
        public int? ReviewPercentageId { get; set; }
        public ProjectionData ProjectionData { get; set; }
        public bool IsForcastOccupancy { get; set; }
        public ClientCountry Country { get; set; }
        public int TechnologyId { get; set; }
    }
    public class EmployeeActivityReportFilter
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int? PmId { get; set; }
        public int? DeptId { get; set; }
        public int? NoOfFreeDays { get; set; }
    }

    public class EmployeeLoginReportFilter
    {
        public string MonthYear { get; set; }
        public string UserId { get; set; }

        public int PMId { get; set; }

    }
    public class ExpensesFilter
    {
        public int? UserId { get; set; }
        public Enums.ExpensePaymentStatus? Status { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }

    public class AdditionalSupportFilter
    {
        public int? pmUid { get; set; }
        public int? projectId { get; set; }
        public Enums.AddSupportRequestStatus? status { get; set; }
    }

    public class MedicalDataSearchFilter
    {
        public string txtEmployee { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }

        public string status { get; set; }
        public int? pmid { get; set; }

    }

    public class ProjectClientFeedbackFilter
    {
        public int projectId { get; set; }
    }

    public class LibraryManagementSearchFilter
    {
        public string SearchText { get; set; }
        public LibraryType LibraryType { get; set; }

        public int[] Technologies { get; set; }
        public int[] Domains { get; set; }
        public bool IsAdvanceSearch { get; set; }
        public bool? IsNDA { get; set; }
        public bool? Featured { get; set; }

    }
    public class EmployeeFeedbackFilter
    {
        public string txtEmployee { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
        public int? pmId { get; set; }
        public int? deptId { get; set; }
        public int[] reasons { get; set; }
        public bool? isEligibleForRehire { get; set; }
        public bool? isVoluntryExit { get; set; }

    }

    public class OnNoticeFilter
    {
        public string txtEmployee { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
        public int? pmId { get; set; }
        public int? deptId { get; set; }

        public bool? IsVoluntaryExit { get; set; }
    }
    
    public class EstimateHourSearchFilter
    {
        public string textSearch { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int? BA { get; set; }
        public int? TL { get; set; }
        public int? TechnologyId { get; set; }
        public int? IndustryId { get; set; }
        public int? FileId { get; set; }


    }

    public class LibraryManagementDownloadHistoryFilter
    {
        public int? Users { get; set; }
        public string LibraryTitle { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }

    }

}
