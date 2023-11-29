using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Web.Controllers
{
    public class PerformanceController : BaseController
    {
        #region "Constructor and fields"
        private readonly IUserLoginService userLoginService;
        private readonly IComplaintService complaintService;
        private readonly IProjectClosureService projectClosureService;
        private readonly IAppraiseService appraiseService;
        private readonly ILeadServices leadService;
        private readonly IProjectService projectService;
        private readonly ILeaveService leaveService;
        private readonly IUserActivityService userActivityService;
        private readonly ITimesheetService timesheetService;
        private readonly ITaskService taskService;
        private readonly IOrgImprovementService orgImprovementService;
        #endregion
        public PerformanceController(IUserLoginService _userLoginService, IComplaintService _complaintService,
            IProjectClosureService _projectClosureService, IAppraiseService _appraiseService, ILeadServices _leadService,
            IProjectService _projectService, ILeaveService _leaveService, IUserActivityService _userActivityService,
            ITimesheetService _timesheetService, ITaskService _taskService, IOrgImprovementService _orgImprovementService)
        {
            this.userLoginService = _userLoginService;
            this.complaintService = _complaintService;
            this.projectClosureService = _projectClosureService;
            this.appraiseService = _appraiseService;
            this.leadService = _leadService;
            this.projectService = _projectService;
            this.leaveService = _leaveService;
            this.userActivityService = _userActivityService;
            this.timesheetService = _timesheetService;
            this.taskService = _taskService;
            this.orgImprovementService = _orgImprovementService;
        }
        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult Index()
        {
            PerformanceIndexDto model = new PerformanceIndexDto();
            model.users = userLoginService.GetUsersByPM(PMUserId)?
                    .ToSelectList(u => u.Name, u => u.Uid);
            model.StartDate = DateTime.Now.Date.AddDays(-30).ToFormatDateString("dd/MM/yyyy");
            model.EndDate = DateTime.Now.Date.ToFormatDateString("dd/MM/yyyy");
            return View(model);
        }

        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult GetPerformance(int uid, string startDate, string endDate)
        {
            var user = userLoginService.GetUserInfoByID(uid);
            if (user != null)
            {
                DateTime? sDate = startDate.ToDateTime("dd/MM/yyyy");
                DateTime? eDate = endDate.ToDateTime("dd/MM/yyyy");

                if (!sDate.HasValue)
                {
                    sDate = user.JoinedDate.HasValue ? user.JoinedDate : null;
                    //eDate = DateTime.Now.Date.AddDays(-1);
                }

                PerformanceDto performanceDto = new PerformanceDto();
                performanceDto.StartDate = startDate;
                performanceDto.EndDate = endDate;
                performanceDto.Uid = user.Uid;
                performanceDto.EmployeeCode = user.EmpCode;
                performanceDto.Name = user.Name;
                performanceDto.Address = user.Address;
                performanceDto.PhoneNumber = user.PhoneNumber;
                performanceDto.DateOfJoining = user.JoinedDate.HasValue ? ((DateTime)user.JoinedDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                performanceDto.Position = user.RoleId != null ? user.Role.RoleName : "";
                performanceDto.DepartmentName = user.DeptId != null ? user.Department.Name : "";

                performanceDto.ReportingPerson = user.TLId.HasValue ? userLoginService.GetNameById((int)user.TLId) : "";
                //Gets complaints
                //var complaints = complaintService.GetComplaints(uid, (DateTime)sDate, (DateTime)eDate);
                var complaints = GetComplaintsList(uid, startDate, endDate);
                if (complaints != null)
                {
                    performanceDto.NoOfComplaints = complaints.Count;
                }

                var projects = GetTimesheetSummary(uid, startDate, endDate);
                if (projects != null)
                {
                    performanceDto.NoOfProjectWorking = projects.Count;
                }

                int freeDays, workingDays, paidDays;

                List<DateTime> excludingDates = GetWorkingAndExcludingDays(sDate, ((eDate == null || (eDate.HasValue && eDate > DateTime.Today))
                    ? DateTime.Today : eDate), out workingDays);

                userActivityService.GetTotalPaidDays(uid, sDate.Value.Date, ((eDate == null || (eDate.HasValue && eDate > DateTime.Today))
                    ? DateTime.Today : eDate.Value.Date), excludingDates, out paidDays);

                freeDays = workingDays - paidDays;
                performanceDto.NoOfWorkingDays = workingDays;
                performanceDto.NoOfFreeDays = freeDays;
                performanceDto.NoOfTotalWorkingDays = paidDays;



                var closureUnique = projectClosureService.GetProjectClosedList(uid, sDate, eDate, "unique");
                if (closureUnique != null && closureUnique.Count > 0)
                {
                    performanceDto.NoOfUniqueProjectClosed = closureUnique.Count;
                }
                var closure = projectClosureService.GetProjectClosedList(uid, sDate, eDate, "all");
                if (closure != null && closure.Count > 0)
                {
                    performanceDto.NoOfProjectClosed = closure.Count;
                }

                var appraises = appraiseService.GetAppraises(GetAppraiseFilterExpersion(uid, sDate, eDate));

                if (appraises != null)
                {
                    performanceDto.Appreciation = appraises.Count;
                }
                var leads = leadService.GetLeads(uid, sDate, eDate);

                if (leads != null)
                {
                    performanceDto.EstimateGiven = leads.Count;
                }
                var leadsAwarded = leadService.GetLeadsAwarded(uid, sDate, eDate);
                if (leadsAwarded != null)
                {
                    performanceDto.EstimateAwarded = leadsAwarded.Count;
                }
                var additionalSupport = projectService.GetAdditionalSupportInDuration(uid, sDate, eDate);
                if (additionalSupport != null)
                {
                    var addStartDate = new DateTime();
                    var addEndDate = new DateTime();
                    List<DateTime> allDates = new List<DateTime>();
                    foreach (var item in additionalSupport)
                    {
                        //setting start date and end date according our searched range
                        if (sDate.HasValue)
                        {
                            addStartDate = item.StartDate >= sDate.Value ? item.StartDate : sDate.Value;
                        }
                        else
                        {
                            addStartDate = item.StartDate;
                        }
                        if (eDate.HasValue)
                        {
                            addEndDate = item.EndDate >= eDate.Value ? eDate.Value : item.EndDate;
                        }
                        else
                        {
                            addEndDate = item.EndDate;
                        }

                        //addStartDate = item.StartDate >= sDate.Value ? item.StartDate : sDate.Value;
                        //addEndDate = item.EndDate >= eDate.Value ? eDate.Value : item.EndDate;
                        for (DateTime date = addStartDate; date <= addEndDate; date = date.AddDays(1))
                        {
                            allDates.Add(date);
                        }
                    }
                    performanceDto.AdditionalSupportReceived = allDates.Distinct().Count();
                }
                if (user.JoinedDate.HasValue)
                {

                    AgeDto ageDto = new AgeDto(user.JoinedDate.Value);

                    string years = (ageDto.Years > 0 ? ageDto.Years.ToString() : "") + "{0} "
                        + (ageDto.Months > 0 ? ageDto.Months.ToString() : "") + "{1} "
                        + (ageDto.Days > 0 ? ageDto.Days.ToString() : "") + "{2}";

                    performanceDto.YearsInCompany = string.Format(years, ageDto.Years > 0 ? ageDto.Years > 1 ? " Years" : " Year" : "",
                        ageDto.Months > 0 ? ageDto.Months > 1 ? " Months" : " Month" : "",
                        ageDto.Days > 0 ? ageDto.Days > 1 ? " Days" : " Day" : "");
                }
                var tasks = taskService.GetTasksInDuration(uid, sDate, eDate);
                if (tasks != null)
                {
                    performanceDto.Assignments = tasks.Count;
                }
                //organization improvement
                var improvements = orgImprovementService.GetImprovementsInDuration(uid, sDate, eDate, Enums.ImprovementType.Organization);
                if (improvements != null)
                {
                    performanceDto.OrgImprovements = improvements.Count;
                }
                //Individual improvement
                improvements = orgImprovementService.GetImprovementsInDuration(uid, sDate, eDate, Enums.ImprovementType.Individual);
                if (improvements != null)
                {
                    performanceDto.IndividualImprovements = improvements.Count;
                }

                var leaves = leaveService.GetLeaveActivitiesInDuration(uid, sDate, eDate);
                if (leaves != null)
                {

                    var addStartDate = new DateTime();
                    var addEndDate = new DateTime();
                    List<DateTime> allDates = new List<DateTime>();
                    foreach (var item in leaves)
                    {
                        //setting start date and end date according our searched range
                        if (sDate.HasValue)
                        {
                            addStartDate = item.StartDate >= sDate.Value ? item.StartDate : sDate.Value;
                        }
                        else
                        {
                            addStartDate = item.StartDate;
                        }
                        if (eDate.HasValue)
                        {
                            addEndDate = item.EndDate >= eDate.Value ? eDate.Value : item.EndDate;
                        }
                        else
                        {
                            addEndDate = item.EndDate;
                        }
                        //addStartDate = item.StartDate >= sDate.Value ? item.StartDate : sDate.Value;
                        //addEndDate = item.EndDate >= eDate.Value ? eDate.Value : item.EndDate;

                        int countryId = (CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM
                        || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM ? (int)Enums.Country.UK : (int)Enums.Country.India);
                        List<OfficialLeave> _leaveList = leaveService.GetOfficialLeavesList(countryId);
                        foreach (DateTime day in Common.EachDay(addStartDate, addEndDate))
                        {
                            if ((day.DayOfWeek.Equals(DayOfWeek.Sunday) ||
                          day.DayOfWeek.Equals(DayOfWeek.Saturday) ||
                          _leaveList.Where(l => l.LeaveDate.Equals(day)).Count() > 0) && (user.RoleId == (int)Enums.UserRoles.PMO || user.RoleId == (int)Enums.UserRoles.UKPM || user.RoleId == (int)Enums.UserRoles.UKBDM))
                            {
                                // don't include these days for the role specified
                            }
                            else
                            {
                                if (item.LeaveType == (int)Enums.LeaveType.Urgent)
                                {
                                    performanceDto.UrgentLeaves = performanceDto.UrgentLeaves + (item.IsHalf == true ? 0.5 : 1);
                                }
                                else
                                {
                                    performanceDto.NormalLeaves = performanceDto.NormalLeaves + (item.IsHalf == true ? 0.5 : 1);
                                }
                            }
                        }
                    }
                }

                performanceDto.TentativeArrivalTime = GetAverageLoginTime(uid, startDate, endDate);
                return PartialView("_Performance", performanceDto);
            }
            else
            {
                return MessagePartialView("No details found for selected user");
            }

        }

        private List<DateTime> GetWorkingAndExcludingDays(DateTime? sDate, DateTime? eDate, out int workingDays)
        {
            workingDays = 0;
            //byte countryId = 2;
            var compareDate = sDate.Value.Date;
            List<DateTime> excludingDates = new List<DateTime>();

            var expr = PredicateBuilder.True<OfficialLeave>();
            expr = expr.And(e => e.CountryId == (CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM
            || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM ? (int)Enums.Country.UK : (int)Enums.Country.India));
            expr = expr.And(e => e.LeaveType.ToLower() == "holiday" && e.IsActive);
            if (sDate.HasValue)
            {
                expr = expr.And(e => e.LeaveDate.Date >= sDate.Value.Date);
            }
            if (eDate.HasValue)
            {
                expr = expr.And(e => e.LeaveDate.Date <= eDate.Value.Date);
            }

            //var leaves = leaveService.GetOfficialLeavesInDuration(sDate.Value, eDate.Value, countryId, true)?.Select(l => l.LeaveDate).ToList();
            var leaves = leaveService.GetOfficialLeavesInDuration(expr)?.Select(l => l.LeaveDate.Date).ToList();

            while (compareDate <= eDate.Value.Date)
            {
                if (compareDate.DayOfWeek != DayOfWeek.Saturday && compareDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    //if week day is there in leaves adds it to excluding days
                    if (leaves != null && leaves.Contains(compareDate))
                    {
                        excludingDates.Add(compareDate);
                    }
                    else
                    {
                        workingDays++;
                    }
                }
                else
                {
                    excludingDates.Add(compareDate);
                }
                compareDate = compareDate.AddDays(1);
            }
            return excludingDates;
        }

        private Expression<Func<EmployeeAppraise, bool>> GetAppraiseFilterExpersion(int uid, DateTime? startdate, DateTime? endDate)
        {
            var expr = PredicateBuilder.True<EmployeeAppraise>();
            expr = expr.And(e => e.IsActive == true && e.IsDelete == false && e.EmployeeId == uid);
            if (startdate.HasValue)
            {
                expr = expr.And(e => e.AddDate >= startdate.Value);
            }
            if (endDate.HasValue)
            {
                expr = expr.And(e => e.AddDate <= endDate.Value);
            }
            return expr;
        }

        /// <summary>
        /// Gets list of TimeSheetSummaryReportDto with unique project
        /// </summary>
        /// <param name="id"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private List<TimeSheetSummaryReportDto> GetTimesheetSummary(int id, string startDate, string endDate)
        {
            DateTime? sDate, eDate;
            CheckDates(startDate, endDate, out sDate, out eDate);
            var expr = PredicateBuilder.True<UserTimeSheet>();
            expr = expr.And(e => e.UID == id);
            if (sDate.HasValue)
            {
                expr = expr.And(e => e.AddDate >= sDate.Value);
            }
            else if (eDate.HasValue)
            {
                expr = expr.And(e => e.AddDate <= eDate.Value);
            }

            //expr = expr.And(e => e.UID == id && e.AddDate >= sDate.Value && e.AddDate <= eDate.Value);
            var timesheets = timesheetService.GetTimeSheetsByFilter(expr);
            if (timesheets != null)
            {
                return timesheets.GroupBy(x => new { x.Project.Name, x.Project.CRMProjectId }).OrderByDescending(T => T.Min(s => s.AddDate)).Select(x => new TimeSheetSummaryReportDto
                {
                    ProjectName = x.Key.Name,
                    CrmId = x.Key.CRMProjectId.ToString()
                }).ToList();
            }
            return null;
        }
        private void CheckDates(string startDate, string endDate, out DateTime? sDate, out DateTime? eDate)
        {
            sDate = startDate.ToDateTime("dd/MM/yyyy");
            eDate = endDate.ToDateTime("dd/MM/yyyy");

            //if (!sDate.HasValue || !eDate.HasValue)
            //{
            //    sDate = DateTime.Now.Date.AddDays(-10);
            //    eDate = DateTime.Now.Date.AddDays(-1);
            //}
        }
        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult GetDetails(int id, string startDate, string endDate, Enums.ActivityDetail activityDetail)
        {
            var user = userLoginService.GetUserInfoByID(id);
            if (user != null)
            {

                DateTime? sDate = startDate.ToDateTime("dd/MM/yyyy");
                DateTime? eDate = endDate.ToDateTime("dd/MM/yyyy");

                if (!sDate.HasValue)
                {
                    sDate = user.JoinedDate.HasValue ? user.JoinedDate : sDate;
                }
                if (!eDate.HasValue)
                {
                    eDate = DateTime.Today;
                }
                int workingDays; // just need to pass we are not using it here
                List<DateTime> excludingDates = GetWorkingAndExcludingDays(sDate, eDate, out workingDays);


                var activityDetails = userActivityService.GetUserActivityDetails(id, sDate.Value, eDate.Value, excludingDates, activityDetail);

                if (activityDetails != null)
                {
                    return PartialView("_Freedays", activityDetails);
                }
                return MessagePartialView("No data found");
            }
            return MessagePartialView("No details found for selected user");
        }
        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult GetAppraiseDetail(int id, string startDate, string endDate)
        {
            var user = userLoginService.GetUserInfoByID(id);
            if (user != null)
            {
                DateTime? sDate, eDate;
                CheckDates(startDate, endDate, out sDate, out eDate);
                List<AppraiseDto> model = null;
                var appraises = appraiseService.GetAppraises(GetAppraiseFilterExpersion(id, sDate, eDate));
                if (appraises != null)
                {
                    model = appraises.Select(a =>
                     new AppraiseDto
                     {
                         AppraiseType = (Enums.AppraiseType)a.AppraiseType,
                         ClientDate = a.ClientDate.HasValue ? ((DateTime)a.ClientDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "",
                         ClientComment = a.ClientComment,
                         TlComment = a.TlComment,
                         AddedDate = a.AddDate.HasValue ? ((DateTime)a.AddDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "",
                     }).ToList();
                }
                return PartialView("_AppraiseDetail", model);
            }
            return MessagePartialView("User not found");
        }
        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult GetComplaintDetail(int id, string startDate, string endDate)
        {
            var user = userLoginService.GetUserInfoByID(id);
            if (user != null)
            {
                //DateTime? sDate, eDate;
                //CheckDates(startDate, endDate, out sDate, out eDate);
                List<ComplaintDto> model = null;
                //var complaints = complaintService.GetComplaints(id, (DateTime)sDate, (DateTime)eDate);
                var complaints = GetComplaintsList(id, startDate, endDate);
                if (complaints != null)
                {
                    model = complaints.Select(c => new ComplaintDto
                    {
                        Id = c.Id,
                        AreaofImprovement = c.AreaofImprovement,
                        ComplaintTypeId = c.ComplaintType,
                        PriorityId = c.Priority,
                        ClientComplain = c.ClientComplain,
                        AddedDate = c.AddedDate.ToString("dd/MM/yyyy")
                    }).ToList();
                }
                return PartialView("_ComplaintDetail", model);
            }
            return MessagePartialView("User not found");


        }
        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult GetProjectsDetails(int id, string startDate, string endDate)
        {
            var user = userLoginService.GetUserInfoByID(id);
            if (user != null)
            {
                var model = GetTimesheetSummary(id, startDate, endDate);
                return PartialView("_ProjectWorked", model);
            }
            return MessagePartialView("User not found");


        }
        public IActionResult GetProjectClosed(int id, string startDate, string endDate, string type)
        {
            var user = userLoginService.GetUserInfoByID(id);
            if (user != null)
            {
                DateTime? sDate, eDate;
                CheckDates(startDate, endDate, out sDate, out eDate);
                List<ProjectClosureDto> model = null;
                var closures = projectClosureService.GetProjectClosedList(id, sDate, eDate, type);
                if (closures != null)
                {
                    model = closures.Select(c => new ProjectClosureDto
                    {
                        ClientName = c.Project.Client?.Name ?? string.Empty,
                        ProjectName = c.Project?.Name ?? string.Empty,
                        BATLName = BATLName(c.UserLogin?.Name, c.UserLogin3?.Name),
                        DateOfClosing = c.DateofClosing.HasValue ? c.DateofClosing.ToFormatDateString("dd/MM/yyyy") : ""
                    }).ToList();
                }
                return PartialView("_ProjectClosedDetails", model);
            }
            return MessagePartialView("User not found");
        }
        private string BATLName(string BAName, string TLName)
        {
            if (!string.IsNullOrWhiteSpace(BAName) && !string.IsNullOrWhiteSpace(TLName))
            {
                return $"{BAName}/{TLName}";
            }
            else if (!string.IsNullOrWhiteSpace(TLName))
            {
                return TLName;
            }
            else if (!string.IsNullOrWhiteSpace(BAName))
            {
                return BAName;
            }
            return string.Empty;
        }
        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult GetPerformanceEstimateDetails(int id, string startDate, string endDate, string type)
        {
            var user = userLoginService.GetUserInfoByID(id);
            DateTime? sDate, eDate;
            CheckDates(startDate, endDate, out sDate, out eDate);
            List<LeadDto> model = null;
            if (user != null)
            {
                List<ProjectLead> leads = null;
                if (type.Equals("converted"))
                {
                    leads = leadService.GetLeadsAwarded(id, sDate, eDate);
                }
                else
                {
                    leads = leadService.GetLeads(id, sDate, eDate);
                }
                if (leads != null)
                {
                    model = leads.Select(l => new LeadDto
                    {
                        Title = l.Title,
                        ClientName = l.LeadClient?.Name
                          ?? "",
                        EstimateTimeInDays = l.EstimateTimeinDay,
                        AddDate = l.AddDate
                    }).ToList();
                }
                return PartialView("_PerformanceLeadsDetail", model);
            }
            else
            {
                return MessagePartialView("User not found");
            }
        }

        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult GetPerformanceAdditionalSupportDetails(int id, string startDate, string endDate)
        {
            var user = userLoginService.GetUserInfoByID(id);
            DateTime? sDate, eDate;
            CheckDates(startDate, endDate, out sDate, out eDate);
            List<ProjectAdditionalSupportDto> model = null;
            if (user != null)
            {
                List<ProjectAdditionalSupport> additionalSupports = null;

                additionalSupports = projectService.GetAdditionalSupportInDuration(id, sDate, eDate);

                if (additionalSupports != null)
                {
                    model = additionalSupports.Select(l => new ProjectAdditionalSupportDto
                    {
                        ProjectName = l.Project.Name,
                        Description = l.Description,
                        Period = $"{l.StartDate.ToFormatDateString("dd MMM yyyy")} to {l.EndDate.ToFormatDateString("dd MMM yyyy")}",
                        StatusText = ((Enums.AddSupportRequestStatus)l.Status).GetEnumDisplayName()
                    }).ToList();
                }
                return PartialView("_PerformanceAdditionalSupportDetails", model);
            }
            else
            {
                return MessagePartialView("User not found");
            }
        }

        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult GetAssignment(int id, string startDate, string endDate)
        {
            var user = userLoginService.GetUserInfoByID(id);
            DateTime? sDate, eDate;
            CheckDates(startDate, endDate, out sDate, out eDate);
            List<CreateTaskDto> model = null;
            if (user != null)
            {
                var tasks = taskService.GetTasksInDuration(id, sDate, eDate);

                if (tasks != null)
                {
                    model = tasks.Select(l => new CreateTaskDto
                    {
                        TaskName = l.TaskName,
                        TaskID = l.TaskID,
                        TaskEndDate = l.TaskEndDate.HasValue ? l.TaskEndDate.ToFormatDateString("dd/MM/yyyy") : "",
                        Priority = (Enums.Priority)l.Priority
                    }).ToList();
                }
                return PartialView("_Assignment", model);
            }
            else
            {
                return MessagePartialView("User not found");
            }
        }
        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult GetImprovements(int uid, string startDate, string endDate, Enums.ImprovementType typeId)
        {
            var user = userLoginService.GetUserInfoByID(uid);
            DateTime? sDate, eDate;
            CheckDates(startDate, endDate, out sDate, out eDate);
            List<OrgImprovementDto> model = null;
            if (user != null)
            {
                var improvements = orgImprovementService.GetImprovementsInDuration(uid, sDate, eDate, typeId);
                if (improvements != null)
                {
                    model = improvements.Select(i => new OrgImprovementDto
                    {
                        Id = i.Id,
                        TypeId = (Enums.ImprovementType)i.TypeId,
                        Title = i.Title,
                        ImprovementDate = i.ImprovementDate.ToFormatDateString("dd/MM/yyyy"),
                        Description = i.Description
                    }).ToList();
                }

                return PartialView("_OrgImprovementsDetails", model);
            }
            else
            {
                return MessagePartialView("User not found");
            }
        }

        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult GetLeaves(int uid, string startDate, string endDate, Enums.LeaveType typeId)
        {
            var user = userLoginService.GetUserInfoByID(uid);
            DateTime? sDate, eDate;
            CheckDates(startDate, endDate, out sDate, out eDate);
            List<LeaveActivityDto> model = null;
            if (user != null)
            {

                var leaves = leaveService.GetLeaveActivitiesInDuration(uid, sDate, eDate);
                if (leaves != null)
                {
                    if (typeId == Enums.LeaveType.Urgent)
                    {
                        leaves = leaves.Where(l => l.LeaveType == (int)Enums.LeaveType.Urgent).ToList();
                    }
                    else
                    {
                        leaves = leaves.Where(l => l.LeaveType != (int)Enums.LeaveType.Urgent).ToList();
                    }
                    model = leaves.Select(l => new LeaveActivityDto
                    {
                        LeaveId = l.LeaveId,
                        StartDate = l.StartDate.ToFormatDateString("dd/MM/yyyy"),
                        EndDate = l.EndDate.ToFormatDateString("dd/MM/yyyy"),
                        Reason = l.Reason,
                        Status = (int)l.Status,
                        DateAdded = (DateTime)l.DateAdded,
                        FullOrHalf = l.IsHalf.HasValue? l.IsHalf==true?"Half":"Full" : ""
                    }).ToList();
                }

                return PartialView("_LeaveDetail", model);
            }
            else
            {
                return MessagePartialView("User not found");
            }
        }

        private List<Complaint> GetComplaintsList(int uid, string startDate, string endDate)
        {
            DateTime? sDate, eDate;
            CheckDates(startDate, endDate, out sDate, out eDate);
            var expr = PredicateBuilder.True<Complaint>();
            expr = expr.And(e => e.ComplaintUser.Any(cu => cu.UserLoginId == uid));
            if (sDate.HasValue)
            {
                expr = expr.And(e => e.AddedDate >= sDate.Value);
            }
            else if (eDate.HasValue)
            {
                expr = expr.And(e => e.AddedDate <= eDate.Value);
            }

            return complaintService.GetComplaintsByFilter(expr);

        }

        private string GetAverageLoginTime(int uid, string startDate, string endDate)
        {
            TimeSpan baseTimeSpan = new TimeSpan(10, 00, 00, 00);
            var user = userLoginService.GetUserInfoByID(uid);
            DateTime? sDate, eDate;
            CheckDates(startDate, endDate, out sDate, out eDate);
            if (user != null)
            {
                if (!sDate.HasValue)
                {
                    sDate = user.JoinedDate;
                }
                if (!eDate.HasValue)
                {
                    eDate = DateTime.Today;
                }
                int workingDays; // just need to pass we are not using it here
                List<DateTime> excludingDates = GetWorkingAndExcludingDays(sDate, eDate, out workingDays);
                var activities = userActivityService.GetActivitiesInDuration(user.Uid, sDate, eDate, excludingDates);
                if (activities != null && activities.Count > 0)
                {
                    var average=activities.Select(a => a.DateAdded.TimeOfDay).Average(a=>a.TotalSeconds);
                    var averageTime = TimeSpan.FromSeconds(average);

                    DateTime time = DateTime.Today.Add(averageTime);
                    string displayTime = time.ToString("hh:mm tt"); // It will give "03:00 AM"
                    return displayTime;
                }
                return null;
            }
            else
            {
                return null;
            }
        }
    }
}