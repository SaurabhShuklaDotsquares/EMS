using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
//using System.Web.Script.Serialization;
using EMS.Web.Modals;
using EMS.Web.Models;
using EMS.Web.Models.Others;
using EMS.Website.Code.LIBS;
using EMS.Website.Models;
using EMS.Website.Models.Others;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class ReportController : BaseController
    {
        #region "Fields"
        private IUserLoginService userLoginService;
        private IUserActivityService userActivityService;
        private readonly IDepartmentService departmentService;
        private readonly IProjectService projectService;
        private readonly ITechnologyService technologyService;
        private readonly IProjectInvoiceService projectInvoiceService;
        private readonly ITimesheetService timesheetService;
        private readonly IVaccinationService iVccinationService;
        //private readonly IComplaintService complaintService;
        //private readonly IProjectClosureService projectClosureService;
        //private readonly IAppraiseService appraiseService;
        private readonly ILeaveService leaveService;
        private readonly ITeamHierarchyService teamHierarchyService;
        private bool IfAshishTeamPMUId { get { return (CurrentUser.PMUid == SiteKey.AshishTeamPMUId || CurrentUser.Uid == SiteKey.AshishTeamPMUId) ? true : false; } }
        #endregion

        #region "Constructor"
        public ReportController(IUserLoginService _userLoginService,
            IUserActivityService _userActivityService,
            IDepartmentService _departmentService, IProjectService _projectService, ITechnologyService _technologyService,
            IProjectInvoiceService _projectInvoiceService, ITimesheetService _timesheetService, ILeaveService _leaveService,
            IVaccinationService _iVccinationService, ITeamHierarchyService _teamHierarchyService
            //IComplaintService _complaintService, IProjectClosureService _projectClosureService,
            //IAppraiseService _appraiseService, ILeadServices _leadService
            )
        {
            this.userLoginService = _userLoginService;
            this.userActivityService = _userActivityService;
            this.departmentService = _departmentService;
            this.projectService = _projectService;
            this.technologyService = _technologyService;
            this.projectInvoiceService = _projectInvoiceService;
            this.timesheetService = _timesheetService;
            //this.complaintService = _complaintService;
            //this.projectClosureService = _projectClosureService;
            //this.appraiseService = _appraiseService;
            this.leaveService = _leaveService;
            this.iVccinationService = _iVccinationService;
            this.teamHierarchyService = _teamHierarchyService;
            //this.leadService = _leadService;
        }
        #endregion        

        [CustomActionAuthorization]
        [HttpGet]
        public ActionResult EmployeeStatusReport()
        {
            var userList = userLoginService.GetUsersListByAllDesignation(Convert.ToInt32(CurrentUser.Uid), Convert.ToInt32(CurrentUser.RoleId),CurrentUser.DesignationId).Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.Uid.ToString() }).ToList();
            ViewBag.UserList = userList;
            ViewBag.PM = userLoginService.GetPMAndPMOHRUsers().Select(p => new SelectListItem { Text = p.Name, Value = p.Uid.ToString() }).ToList();
            return View();
        }



        public string GetProjectName(string ProjectName, List<UserActivityLogProjectDto> projects, int i, DateTime DateAdded, string status)
        {
            if (i == projects.Count() - 1)
            {
                return "<b>" + status + "</b> " + (string.IsNullOrEmpty(ProjectName) ? "" : "(" + ProjectName + ")") + " <b class='text-orange'> till end of the day</b>";
            }
            else
            {
                TimeSpan time = (projects.Skip(i + 1).Take(1).FirstOrDefault().DateAdded - DateAdded);
                if (time.Hours == 0 && time.Minutes == 0)
                {
                    return "";
                }
                else
                {
                    return "<b>" + status + "</b> " + (string.IsNullOrEmpty(ProjectName) ? "" : "(" + ProjectName + ")") + " <b class='text-orange'> for " + time.GetDetailedTimeString() + "</b><br><br>";
                }
            }
        }

        public IActionResult GetEmployeeStatusReport(IDataTablesRequest request, int? user, string startDate, string endDate)
        {

            var pagingService = new PagingService<UserActivityLog>(request.Start, request.Length);
            var expr = user.HasValue ? PredicateBuilder.True<UserActivityLog>() : PredicateBuilder.False<UserActivityLog>();
            if (user.HasValue)
            {
                expr = expr.And(u => u.Uid == user.Value);
            }

            DateTime? dateStart = startDate.ToDateTime("dd/MM/yyyy");
            DateTime? dateEnd = endDate.ToDateTime("dd/MM/yyyy");

            if (dateStart.HasValue && dateEnd.HasValue)
            {
                expr = expr.And(l => l.Date >= dateStart && l.Date <= dateEnd);
            }
            else if (dateStart.HasValue)
            {
                expr = expr.And(l => l.Date >= dateStart);
            }
            else if (dateEnd.HasValue)
            {
                expr = expr.And(l => l.Date <= dateEnd);
            }

            pagingService.Filter = expr;

            bool descSort = true;
            foreach (var item in request.SortedColumns())
            {
                if (item.Name == "date" && item.Sort.Direction == SortDirection.Ascending)
                {
                    descSort = false;
                }
            }

            IDictionary<string, object> additionalParameters = new Dictionary<string, object>();

            int totalpaiddays = 0, totalfreedays = 0;

            userActivityService.GetTotalUserActivityLog(out totalpaiddays, out totalfreedays, pagingService);

            if (user.HasValue && user.Value != 0)
            {
                additionalParameters.Add(new KeyValuePair<string, object>("totalpaiddays", totalpaiddays));
                additionalParameters.Add(new KeyValuePair<string, object>("totalfreedays", totalfreedays));
            }

            int totalCount = 0;

            var response = userActivityService.GetUserActivityLog(out totalCount, pagingService, descSort);
            return DataTablesJsonResult(totalCount, request, response.Select((x, index) => new
            {
                rowId = request.Start + index + 1,
                date = x.Date.ToString("ddd, MMM dd, yyyy"),
                loginTime = x.Projects.Count() > 0 ? x.Projects.OrderBy(p => p.DateAdded).FirstOrDefault().DateAdded.ToString("hh:mm tt") : "",
                status = string.Join("", x.Projects.Select((p, i) => GetProjectName(p.ProjectName, x.Projects, i, p.DateAdded, p.Status)))
            }), additionalParameters);
        }

        [HttpGet]
        public ActionResult EmployeeActivityReport()
        {
            var model = new EmployeeReportIndexDto();

            model.DepartmentList = departmentService.GetActiveDepartments()
                        .Where(x => x.DeptId != (int)Enums.ProjectDepartment.HRDepartment && x.DeptId != (int)Enums.ProjectDepartment.NetworkHardwareDepartment &&
                                   x.DeptId != (int)Enums.ProjectDepartment.Other)
                        .OrderBy(x => x.Name)
                        .Select(x => new SelectListItem { Value = x.DeptId.ToString(), Text = x.Name })
                        .ToList();

            if (CurrentUser.RoleId == (int)Enums.UserRoles.Director)
            {
                model.PMUserList = userLoginService.GetUserByRole((int)Enums.UserRoles.PM, true)
                                    .Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() })
                                    .ToList();
            }

            model.StartDate = DateTime.Now.Date.AddDays(-10).ToFormatDateString("dd/MM/yyyy");
            model.EndDate = DateTime.Now.Date.AddDays(-1).ToFormatDateString("dd/MM/yyyy");

            return View(model);
        }

        [HttpPost]
        public IActionResult EmployeeActivityReport(IDataTablesRequest request, EmployeeActivityReportFilter reportFilter)
        {
            var pagingServices = new PagingService<UserActivityLog>(request.Start, request.Length);
            IDictionary<string, object> additionalParameters = new Dictionary<string, object>();
            pagingServices.Filter = GetEmployeeActivityReportFilterExpression(reportFilter, ref additionalParameters);
            TempData.Put("EmployeeActivityReportFilters", reportFilter);

            int totalCount = 0;
            var response = userActivityService.GetUserActivityLog(out totalCount, reportFilter.NoOfFreeDays ?? 0, pagingServices);

            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                rowId = (index + 1) + (request.Start),
                Id = r.UserId,
                Name = r.Name,
                Designation = r.Designation,
                Department = r.Department,
                TotalFreeDays = r.TotalFreeDays,
                TeamManager = r.TeamManager
            }), additionalParameters);
        }
        [HttpGet]
        public ActionResult EmployeeActivities(int id, string startDate, string endDate)
        {
            if (id > 0)
            {
                DateTime? sDate = startDate.ToDateTime("dd/MM/yyyy");
                DateTime? eDate = endDate.ToDateTime("dd/MM/yyyy");

                if (!sDate.HasValue || !eDate.HasValue)
                {
                    sDate = DateTime.Now.Date.AddDays(-10);
                    eDate = DateTime.Now.Date.AddDays(-1);
                }

                var compareDate = sDate.Value;
                List<DateTime> excludingDates = new List<DateTime>();

                while (compareDate <= eDate.Value)
                {
                    if (compareDate.DayOfWeek == DayOfWeek.Saturday || compareDate.DayOfWeek == DayOfWeek.Sunday)
                    {
                        excludingDates.Add(compareDate);
                    }

                    compareDate = compareDate.AddDays(1);
                }

                var activityDetails = userActivityService.GetUserFreeActivityDetails(id, sDate.Value, eDate.Value, excludingDates);

                if (activityDetails != null)
                {
                    return PartialView("_ActivityDetails", activityDetails);
                }
            }
            return MessagePartialView("No details found for selected user");
        }

        [HttpGet]
        public ActionResult DownloadEmployeeActivityReport()
        {
            var reportFilter = TempData.Get<EmployeeActivityReportFilter>("EmployeeActivityReportFilters");
            if (reportFilter != null)
            {
                var pagingServices = new PagingService<UserActivityLog>(0, int.MaxValue);
                IDictionary<string, object> additionalParameters = new Dictionary<string, object>();
                pagingServices.Filter = GetEmployeeActivityReportFilterExpression(reportFilter, ref additionalParameters);
                int totalCount = 0;
                var response = userActivityService.GetUserActivityLog(out totalCount, reportFilter.NoOfFreeDays ?? 0, pagingServices);

                var report = response.Select(x => new
                {
                    Name = x.Name,
                    Designation = x.Designation,
                    Department = x.Department,
                    TeamManager = x.TeamManager,
                    TotalFreeDays = x.TotalFreeDays
                }).ToList();

                var columns = new string[] { "Name", "Designation", "Department", "TeamManager", "TotalFreeDays" };

                byte[] filecontent = ExportExcelHelper.ExportExcel(report, "Employee Activity Report", true, columns);
                string fileName = $"EmployeeActivityReport_{DateTime.Now.Ticks}.xlsx";
                return File(filecontent, ExportExcelHelper.ExcelContentType, fileName);
            }
            return Content("Unable to get filters");
        }

        [CustomActionAuthorization]
        [HttpGet]
        public ActionResult EmployeeLoginReport()
        {
            List<SelectListItem> userList = new List<SelectListItem>();
            if (CurrentUser.RoleId == (int)Enums.UserRoles.HRBP)
            {
                userList = userLoginService.GetUsers(true).Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.Uid.ToString() }).ToList();
            }
            else
            {
                userList = userLoginService.GetUsers(true).Where(x => x.PMUid == PMUserId).Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.Uid.ToString() }).ToList();
            }
            ViewBag.UserList = userList;

            DateTime now = DateTime.Today;
            List<SelectListItem> MonthYear = new List<SelectListItem>();
            for (int i = 0; i < 12; i++)
            {
                string Month = now.ToString("MMM") + " " + now.Year.ToString();
                MonthYear.Add(new SelectListItem { Text = Month, Value = Month });
                now = now.AddMonths(-1);
            }
            ViewBag.MonthYear = MonthYear.ToList();
            var pmList1 = userLoginService.GetPMAndPMOHRUsers(true);

            if (RoleValidator.HROperations_RoleIds.Contains(CurrentUser.RoleId))
            {
                pmList1 = pmList1.Where(p => p.RoleId != (int)Enums.UserRoles.HRBP).ToList();
            }
            var pmList = pmList1.Select(x => new SelectListItem { Text = x.Name == null ? "" : x.Name.ToString(), Value = x.Uid.ToString(), Selected = (x.Uid == pmList1.FirstOrDefault().Uid ? true : false) }).ToList();
            pmList.Insert(0, new SelectListItem() { Text = "All Project Managers", Value = "0" });
            ViewBag.PMList = pmList;

            return View();
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult EmployeeLoginReport(EmployeeLoginReportFilter searchFilter)
        {
            String[] strmonthyear = searchFilter.MonthYear.Split(' ');
            int month = DateTime.ParseExact(strmonthyear[0], "MMM", CultureInfo.CurrentCulture).Month;
            int daysinmonth = System.DateTime.DaysInMonth(Convert.ToInt32(strmonthyear[1]), month);

            List<UserLogin> userList = new List<UserLogin>();
            if ((searchFilter.UserId == null || searchFilter.UserId == "") && CurrentUser.RoleId == (int)Enums.UserRoles.HRBP && searchFilter.PMId == 0)
            {
                userList = userLoginService.GetUsers(true).ToList();
            }
            else if ((searchFilter.UserId == null || searchFilter.UserId == "") && CurrentUser.RoleId == (int)Enums.UserRoles.HRBP && searchFilter.PMId != 0)
            {
                userList = userLoginService.GetUsers(true).Where(x => x.PMUid == searchFilter.PMId).ToList();
            }
            else if (searchFilter.UserId == null || searchFilter.UserId == "")
            {
                userList = userLoginService.GetUsers(true).Where(x => x.PMUid == PMUserId).ToList();
            }
            else
            {
                UserLogin userl = userLoginService.GetUserInfoByID(Convert.ToInt32(searchFilter.UserId));
                userList.Add(userl);
            }

            List<ActivityEmpLoginReportDto> emplogins = new List<ActivityEmpLoginReportDto>();

            List<UserActivityLog> useractivity = new List<UserActivityLog>();
            useractivity = userActivityService.GetUserLoginActivityDetails(month, Convert.ToInt32(strmonthyear[1]));

            foreach (var user in userList)
            {
                ActivityEmpLoginReportDto emplogin = new ActivityEmpLoginReportDto();
                emplogin.UserId = user.Uid;
                emplogin.Name = user.Name;
                emplogin.Department = user.Department?.Name;
                emplogin.Email = user.EmailOffice;
                emplogin.TeamManager = userLoginService.GetUserInfoByID(user.PMUid.Value)?.Name;
                emplogin.LoginDetails = new List<ActivityLoginReportDto>();
                for (int i = 1; i <= daysinmonth; i++)
                {
                    ActivityLoginReportDto empactivity = new ActivityLoginReportDto();
                    DateTime date = new DateTime(Convert.ToInt32(strmonthyear[1]), month, i);
                    empactivity.day = i;
                    empactivity.Logintime = "-";
                    var firstActivity = useractivity.Where(O => O.Uid == user.Uid && O.Date == date).OrderBy(O => O.DateAdded).FirstOrDefault();
                    if (firstActivity != null)
                    {
                        empactivity.Logintime = firstActivity.DateAdded.ToShortTimeString();
                    }
                    emplogin.LoginDetails.Add(empactivity);
                }

                emplogins.Add(emplogin);
            }
            return new JsonResult(emplogins);
        }

        [CustomActionAuthorization]
        public IActionResult Vaccination()
        {
            return View();
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult VaccinationDetails(IDataTablesRequest request)
        {
            var pagingServices = new PagingService<VaccinationStatus>(request.Start, request.Length);
            var expr = PredicateBuilder.True<VaccinationStatus>();
            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "employeeName":
                            return o.OrderByColumn(item, c => c.U.Name);
                        default:
                            return o.OrderByColumn(item, c => c.AddedDate);
                    }
                }
                return o.OrderByDescending(c => c.AddedDate);
            };

            int totalCount = 0;
            var response = iVccinationService.GetVaccinationDocByPaging(out totalCount, pagingServices);

            return DataTablesJsonResult(totalCount, request, response.Select((r, Index) => new
            {
                r.Id,
                rowIndex = (Index + 1) + (request.Start),
                employeeName = r.U.Name,//Employee Name
                projectManagerName = r.U.Pmu != null ? r.U.Pmu.Name : "---",//Employee Project Manager Name
                phoneNumber = r.U.PhoneNumber,//Employee Phone
                email = r.U.EmailOffice,//Employee Email
                vaccinationStatus = r.VaccinatedTypeId == 1 ? "Partially Vaccinated" : "Fully Vaccinated",
                certificate = r.UpdatedCertificate
            }));
        }


        public ActionResult DownloadVaccinationReportExcel()
        {
            try
            {
                var expr = PredicateBuilder.True<VaccinationStatus>();
                var model = iVccinationService.GetVaccinationCertificateList(expr);
                List<VaccinationReportDto> vaccinationreport = new List<VaccinationReportDto>();
                foreach (var item in model)
                {

                    vaccinationreport.Add(new VaccinationReportDto()
                    {
                        EmployeeName = item.U.Name,
                        ManagerName = item.U.Pmu != null ? item.U.Pmu.Name : "---",
                        PhoneNumber = item.U.PhoneNumber,//Employee Phone
                        Email = item.U.EmailOffice,//Employee Phone
                        VaccinationStatus = item.VaccinatedTypeId == 1 ? "Partially Vaccinated" : "Fully Vaccinated",
                        //Certificate = item.UpdatedCertificate
                        //"<a download style='color:#101ee5;text-decoration:underline;' href='Upload/Vaccination_Files/'" +item.UpdatedCertificate+ " target='_blank'> </a>"
                    });
                }

                string TotalLateTime = String.Empty;

                if (vaccinationreport.Count > 0)
                {
                    string certificatePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/Upload/Vaccination_Files/");
                    string filename = "VaccinationReport_" + DateTime.Now.Ticks.ToString() + ".xlsx";
                    string[] columns = { "EmployeeName", "ManagerName", "PhoneNumber", "Email", "VaccinationStatus" };
                    byte[] filecontent = ExportExcelHelper.VaccinationExportExcel(vaccinationreport, "Vaccination Report ", true, columns);

                    string fileName = filename;
                    return File(filecontent, ExportExcelHelper.ExcelContentType, fileName);



                }
            }
            catch (Exception e)
            {
            }
            return CreateModelStateErrors();
        }



        public ActionResult FillEmployee(int PMid)
        {
            var userList = userLoginService.GetUsers(true).Where(x => x.PMUid == PMid || PMid == 0).Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.Uid.ToString() }).ToList();
            return new JsonResult(userList);
        }

        [HttpGet]
        public ActionResult DownloadAttendanceReport(string AttendanceDate, int PMId, string UserId)
        {
            DateTime? AttDate = AttendanceDate.ToDateTime("dd/MM/yyyy");
            List<UserLogin> userList = new List<UserLogin>();
            if ((UserId == null || UserId == "") && CurrentUser.RoleId == (int)Enums.UserRoles.HRBP && PMId == 0)
            {
                userList = userLoginService.GetUsers(true).ToList();
            }
            else if ((UserId == null || UserId == "") && CurrentUser.RoleId == (int)Enums.UserRoles.HRBP && PMId != 0)
            {
                userList = userLoginService.GetUsers(true).Where(x => x.PMUid == PMId).ToList();
            }
            else if (UserId == null || UserId == "")
            {
                userList = userLoginService.GetUsers(true).Where(x => x.PMUid == PMUserId).ToList();
            }
            else
            {
                UserLogin userl = userLoginService.GetUserInfoByID(Convert.ToInt32(UserId));
                userList.Add(userl);
            }

            List<UserActivityLog> useractivity = new List<UserActivityLog>();
            useractivity = userActivityService.GetUserLoginActivityDetails(AttDate);

            var expr = PredicateBuilder.True<UserTimeSheet>();
            expr = expr.And(e => e.InsertedDate.HasValue && e.InsertedDate.Value.Date == AttDate);

            //expr = expr.And(e => e.UID == id && e.AddDate >= sDate.Value && e.AddDate <= eDate.Value);
            var timesheets = timesheetService.GetTimeSheetsByFilter(expr);

            List<LeaveActivity> leaveActivities = new List<LeaveActivity>();
            leaveActivities = leaveService.GetLeaveActivitiesByDate(AttDate);

            List<EmployeeAttendnaceDto> empatts = new List<EmployeeAttendnaceDto>();
            int i = 1;
            foreach (var user in userList)
            {
                EmployeeAttendnaceDto empatt = new EmployeeAttendnaceDto();
                empatt.SNo = i;
                empatt.Date = AttDate?.ToString("MMM dd,yyyy");
                empatt.EmpCode = user?.EmpCode;
                empatt.HRMId = user?.HRMId;

                empatt.Status = "A";
                empatt.Name = user.Name;
                var firstActivity = useractivity.Where(O => O.Uid == user.Uid).OrderBy(O => O.DateAdded).FirstOrDefault();
                if (firstActivity != null)
                {
                    empatt.InTime = firstActivity.DateAdded.ToShortTimeString();
                    empatt.Status = "P";
                }

                var lasttimesheet = timesheets.Where(T => T.UID == user.Uid).OrderByDescending(T => T.InsertedDate).FirstOrDefault();
                if (lasttimesheet != null)
                {
                    empatt.OutTime = lasttimesheet.InsertedDate?.ToShortTimeString();
                    empatt.Status = "P";
                }

                var leaveactivity = leaveActivities.Where(L => L.Uid == user.Uid && (L.Status == (int)Enums.LeaveStatus.Pending || L.Status == (int)Enums.LeaveStatus.Approved)).FirstOrDefault();
                if (leaveactivity != null)
                {
                    if (leaveactivity.IsHalf == true)
                    {
                        empatt.Status = "HD";
                    }
                    else
                    {
                        empatt.Status = "L";
                    }
                }
                i++;
                empatts.Add(empatt);
            }
            var columns = new string[] { "SNo", "Date", "HRMId", "EmpCode", "InTime", "OutTime", "Status", "Name" };

            byte[] filecontent = ExportExcelHelper.ExportExcel(empatts, "", false, columns);
            string fileName = $"EmployeeAttendnaceReport_{DateTime.Now.Ticks}.xlsx";
            return File(filecontent, ExportExcelHelper.ExcelContentType, fileName);
        }

        private Expression<Func<UserActivityLog, bool>> GetEmployeeActivityReportFilterExpression(EmployeeActivityReportFilter reportFilter, ref IDictionary<string, object> additionalParameters)
        {
            var expr = PredicateBuilder.True<UserActivityLog>();
            if (CurrentUser.RoleId != (int)Enums.UserRoles.Director)
            {
                reportFilter.PmId = CurrentUser.RoleId == (int)Enums.UserRoles.PM ? CurrentUser.Uid : CurrentUser.PMUid;
                expr = expr.And(x => x.Uid.HasValue && x.UserLogin.PMUid == reportFilter.PmId.Value);
            }
            else if (reportFilter.PmId.HasValue && reportFilter.PmId.Value > 0)
            {
                expr = expr.And(x => x.Uid.HasValue && x.UserLogin.PMUid == reportFilter.PmId.Value);
            }
            expr = expr.And(x => x.Uid.HasValue && x.UserLogin.DeptId != (int)Enums.ProjectDepartment.HRDepartment &&
                        x.UserLogin.DeptId != (int)Enums.ProjectDepartment.Other && x.UserLogin.DeptId != (int)Enums.ProjectDepartment.NetworkHardwareDepartment);
            if (reportFilter.DeptId.HasValue && reportFilter.DeptId.Value > 0)
            {
                expr = expr.And(x => x.UserLogin.DeptId == reportFilter.DeptId.Value);
            }
            DateTime? sDate = reportFilter.StartDate.ToDateTime("dd/MM/yyyy");
            DateTime? eDate = reportFilter.EndDate.ToDateTime("dd/MM/yyyy");
            if (!sDate.HasValue || !eDate.HasValue)
            {
                sDate = DateTime.Now.Date.AddDays(-10);
                eDate = DateTime.Now.Date.AddDays(-1);
            }
            expr = expr.And(x => x.Date >= sDate && x.Date <= eDate && x.Status.ToLower() == "free" && x.Uid.HasValue);
            // expr = expr.And(x => x.UserLogin.IsActive == true && !x.UserLogin.IsResigned && x.UserLogin.RoleId != (int)Enums.UserRoles.TL && x.UserLogin.RoleId != (int)Enums.UserRoles.QA && x.UserLogin.RoleId != (int)Enums.UserRoles.BA && x.UserLogin.RoleId != (int)Enums.UserRoles.DG);
            int totalWorkingDays = 0;
            var compareDate = sDate.Value;
            List<DateTime> excludingDates = new List<DateTime>();
            while (compareDate <= eDate.Value)
            {
                if (compareDate.DayOfWeek != DayOfWeek.Saturday && compareDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    totalWorkingDays++;
                }
                else
                {
                    excludingDates.Add(compareDate);
                }
                compareDate = compareDate.AddDays(1);
            }

            if (excludingDates.Any())
            {
                expr = expr.And(x => !excludingDates.Contains(x.Date));
            }
            additionalParameters.Add(new KeyValuePair<string, object>("totalWorkingDays", totalWorkingDays));
            additionalParameters.Add(new KeyValuePair<string, object>("filteredDates", $"{sDate.ToFormatDateString("dd MMM, yyyy")} to {eDate.ToFormatDateString("dd MMM, yyyy")}"));
            return expr;
        }
        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult UserReport()
        {
            var userList = userLoginService.GetUsersListByAllRole(CurrentUser.RoleId == (int)Enums.UserRoles.PM
                ? CurrentUser.Uid : CurrentUser.PMUid, (int)Enums.UserRoles.PM);
            var userSelectList = new List<SelectListItem>();
            if (userList != null)
            {
                userSelectList = userList.Select(u => new SelectListItem { Text = u.Name, Value = u.Uid.ToString() }).ToList();
            }

            return View(userSelectList);
        }
        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult UserReport(IDataTablesRequest request, int? user)
        {
            var pagingServices = new PagingService<UserLogin>(request.Start, request.Length);

            var filterExpr = PredicateBuilder.True<UserLogin>().And(x => x.IsActive == true && x.RoleId != (int)Enums.UserRoles.HRBP);
            if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
            {
                filterExpr = filterExpr.And(x => x.PMUid == CurrentUser.Uid || x.ProjectOtherPm.Any(po => po.Pmuid == CurrentUser.Uid));
            }
            else
            {
                filterExpr = filterExpr.And(x => x.PMUid == CurrentUser.PMUid || x.ProjectOtherPm.Any(po => po.Pmuid == CurrentUser.PMUid));
            }
            filterExpr.And(x => x.RoleId != (int)Enums.UserRoles.HRBP);


            if (user.HasValue)
            {
                filterExpr = filterExpr.And(x => x.Uid.Equals(user.Value));
            }


            pagingServices.Filter = filterExpr;

            pagingServices.Sort = (o) =>
            {
                return o.OrderBy(c => c.Name);
            };

            int totalCount = 0;
            var response = userLoginService.GetUserReportByPaging(out totalCount, pagingServices);
            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                Uid = r.Uid,
                rowIndex = (index + 1) + (request.Start),
                Name = r.Name,
                Department = r?.Department.Name ?? null,
                ReportTo = r.TLId.HasValue ? userLoginService.GetUserInfoByID(r.TLId.Value)?.Name ?? null : null,
                AssignedProject = getProjectOrUserList(r)
            }));
        }

        /// <summary>
        /// Gets project list for user
        /// </summary>
        /// <param name="userlogin">UserLogin</param>
        /// <returns>Project list or User List</returns>
        public string getProjectOrUserList(UserLogin userlogin, Project project = null)
        {
            string projectList = string.Empty;
            if (userlogin != null)
            {
                foreach (var projectDeveloper in userlogin.ProjectDevelopers.Where(S => S.Uid == userlogin.Uid && S.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running && S.Project.Status == "R"))
                {
                    int TotDeveloper = projectService.TotalDeveloperWorking(projectDeveloper.ProjectId);
                    string VName = !String.IsNullOrEmpty(Convert.ToString(projectDeveloper.VD_id)) ? projectDeveloper.VirtualDeveloper.VirtualDeveloper_Name : "";
                    string textDeveloper = TotDeveloper <= 1 ? "Developer" : "Developers";


                    if (VName == "")
                    {
                        projectList += $"{projectDeveloper.Project.Name} [{projectDeveloper.Project.CRMProjectId}] ({TotDeveloper} {textDeveloper})<BR />";
                    }
                    else
                    {
                        projectList += $"{projectDeveloper.Project.Name} [{projectDeveloper.Project.CRMProjectId}] ( {VName} ) ({TotDeveloper} {textDeveloper})<BR />";
                    }
                }
                return projectList;
            }
            if (project != null)
            {
                List<ProjectDeveloper> plist = project.ProjectDevelopers.Where(S => S.WorkStatus == 12).ToList();
                string UserList = string.Empty;

                foreach (var projectDeveloper in plist)
                {
                    string VName = projectDeveloper.VD_id.HasValue ? projectDeveloper.VirtualDeveloper.VirtualDeveloper_Name : "";
                    if (VName == "")
                    {
                        UserList += projectDeveloper.UserLogin != null ? projectDeveloper.UserLogin.Name + "<BR />" : "";
                    }
                    else
                    {
                        UserList += (projectDeveloper.UserLogin != null ? projectDeveloper.UserLogin.Name : "N/A") + " ( " + VName + " ) " + "<BR />";
                    }
                }
                return UserList;
            }
            return string.Empty;
        }
        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult ProjectUserReport()
        {
            ProjectDto ProjectDto = new ProjectDto();
            ProjectDto.TechnologySelectList = technologyService.GetTechnologyList().OrderBy(t => t.Title).Select(x => new SelectListItem
            {
                Text = x.Title,
                Value = x.TechId.ToString()
            }).ToList(); ;
            ProjectDto.ModelList = projectService.GetBucketModels().OrderBy(x => x.ModelName).Select(x => new SelectListItem
            {
                Text = x.ModelName,
                Value = x.BucketId.ToString()
            }).ToList();
            return View(ProjectDto);
        }
        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult ProjectUserReport(IDataTablesRequest request, int? technology, int? model, string name)
        {
            var pagingServices = new PagingService<Project>(request.Start, request.Length);

            var filterExpr = PredicateBuilder.True<Project>().And(x => x.Status == "R");
            if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
            {
                filterExpr = filterExpr.And(x => x.PMUid == CurrentUser.Uid || x.ProjectOtherPm.Any(po => po.Pmuid == CurrentUser.Uid));
            }
            else
            {
                filterExpr = filterExpr.And(x => x.PMUid == CurrentUser.PMUid || x.ProjectOtherPm.Any(po => po.Pmuid == CurrentUser.PMUid));
            }

            if (technology.HasValue)
            {
                filterExpr = filterExpr.And(x => x.Project_Tech.Any(i => i.TechId == technology.Value));
            }
            if (model.HasValue)
            {
                filterExpr = filterExpr.And(x => x.Model.Value == model.Value);
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                string trimmedName = name.Trim();
                filterExpr = filterExpr.And(x => (x.Name.Contains(trimmedName) || x.CRMProjectId.ToString().Contains(trimmedName)));
            }

            pagingServices.Filter = filterExpr;

            pagingServices.Sort = (o) =>
            {
                return o.OrderBy(c => c.Name);
            };

            int totalCount = 0;
            var response = projectService.GetProjectsByPaging(out totalCount, pagingServices);
            return DataTablesJsonResult(totalCount, request, response.Select((p, index) => new
            {
                Uid = p.Uid,
                ProjectId = p.ProjectId,
                rowIndex = (index + 1) + (request.Start),
                Name = $"{p.Name} [{p.CRMProjectId}]",
                AssignedUsers = getProjectOrUserList(null, p),
                ModelName = p.Model == null ? "" : p.BucketModel.ModelName,
                Technology = string.Join("<BR />", p.Project_Tech?.Select(pd => pd.Technology?.Title)),
                ProjectStartDate = p.StartDate.ToFormatDateString("MMM d, yyyy")
            }));

        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult WorkingHour()
        {
            WorkingHourDto workingHourDto = new WorkingHourDto();
            List<SelectListItem> data = new List<SelectListItem>();
            var date = DateTime.Today.AddMonths(-11);
            var currentdate = DateTime.Today;
            while (true)
            {
                workingHourDto.MonthsList.Add(new SelectListItem { Text = ((Enums.Month)currentdate.Month).GetDescription() + " - " + currentdate.Year, Value = currentdate.Month + "-" + currentdate.Year });

                if (currentdate.Month == date.Month && currentdate.Year == date.Year)
                {
                    break;
                }
                currentdate = currentdate.AddMonths(-1);
            }
            return View(workingHourDto);
        }

        public HttpWebResponse GetTimeSheetResponse(DateTime monthStartDate, DateTime monthEndDate)
        {
            var request = WebRequest.CreateHttp(SiteKey.CRMApiWorkingHourUrl);
            StringBuilder postData = new StringBuilder();
            postData.Append($"crmid={0}&");
            postData.Append($"invoiceStartDate={monthStartDate.ToFormatDateString("yyyy-MM-dd")}&");
            postData.Append($"invoiceEndDate={monthEndDate.ToFormatDateString("yyyy-MM-dd")}");

            request.Headers.Add("userid", SiteKey.CRMApiUser);
            request.Headers.Add("password", SiteKey.CRMApiPassword);

            var data = Encoding.ASCII.GetBytes(postData.ToString());

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            var result = (HttpWebResponse)(request.GetResponse());
            return result;
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult WorkingHour(IDataTablesRequest dataTablesRequest, string month)
        {
            try
            {
                if (string.IsNullOrEmpty(month))
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "", IsSuccess = true, Data = "" });
                }
                string year = "";
                var monthdata = (month ?? "").Split('-');
                month = monthdata[0].Trim();
                if (monthdata.Count() > 1)
                {
                    year = monthdata[1].Trim();
                }

                DateTime monthStartDate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
                DateTime monthEndDate = monthStartDate.AddMonths(1).AddDays(-1);

                var response = GetTimeSheetResponse(monthStartDate, monthEndDate);

                string responseData = "";
                var crmResponse = new ResponseModel<string>();

                TimeSpan DevloperHours = new TimeSpan();
                TimeSpan TLHours = new TimeSpan();
                TimeSpan DesignerHours = new TimeSpan();
                TimeSpan BAHours = new TimeSpan();
                Dictionary<decimal, int?> TimesheetsIdsAndRoleList = new Dictionary<decimal, int?>();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    if (responseData.HasValue())
                    {
                        if (responseData != null)
                        {
                            JObject obj = JObject.Parse(responseData);

                            String json = null;

                            json = JsonConvert.SerializeObject(obj.SelectToken("Data.invoiceDate"));

                            List<InvoiceWorkingHourjson> invoiceList = null;

                            invoiceList = JsonConvert.DeserializeObject<List<InvoiceWorkingHourjson>>(json);



                            if (crmResponse != null && invoiceList.Count > 0)
                            {
                                var startDate = new DateTime();
                                var endDate = new DateTime();
                                var crmIds = string.Join(",", invoiceList.Select(i => i.crm_id).Distinct());

                                var timesheetData = new TimeSheetWorkHour()
                                {
                                    month = month,
                                    year = year,
                                    crmIds = crmIds
                                };

                                TempData.Put<TimeSheetWorkHour>("TimeSheetData", timesheetData);

                                List<GetMonthTimesheetsNew_Result> timesheets = timesheetService.GetMonthTimesheetsDataFromSPNew(monthStartDate, monthEndDate, crmIds);
                                foreach (var invoice in invoiceList)
                                {
                                    DateTime invoiceStartEndDate = Convert.ToDateTime(invoice.start_date);
                                    if (invoiceStartEndDate < monthStartDate)
                                    {
                                        startDate = monthStartDate;
                                    }
                                    else
                                    {
                                        startDate = invoiceStartEndDate;
                                    }
                                    invoiceStartEndDate = Convert.ToDateTime(invoice.end_date);
                                    if (invoiceStartEndDate > monthEndDate)
                                    {
                                        endDate = monthEndDate;
                                    }
                                    else
                                    {
                                        endDate = invoiceStartEndDate;
                                    }

                                    var CRMId = !string.IsNullOrWhiteSpace(invoice.crm_id) ? Convert.ToInt32(invoice.crm_id) : 0;
                                    foreach (var timesheet in timesheets.Where(t => t.AddDate >= startDate && t.AddDate <= endDate && t.CRMProjectId == CRMId))
                                    {
                                        if (RoleValidator.DV_Technical_DesignationIds.Contains(timesheet.DesignationId.Value))
                                        {
                                            if (!TimesheetsIdsAndRoleList.ContainsKey(timesheet.UserTimeSheetId))
                                            {
                                                TimesheetsIdsAndRoleList.Add(timesheet.UserTimeSheetId, timesheet.RoleId);
                                                DevloperHours = DevloperHours.Add(timesheet.WorkHours);
                                            }
                                        }
                                        else if (RoleValidator.DV_UIUX_DesignationIds.Contains(timesheet.DesignationId.Value))
                                        {
                                            if (!TimesheetsIdsAndRoleList.ContainsKey(timesheet.UserTimeSheetId))
                                            {
                                                TimesheetsIdsAndRoleList.Add(timesheet.UserTimeSheetId, timesheet.RoleId);
                                                DesignerHours = DesignerHours.Add(timesheet.WorkHours);
                                            }
                                        }
                                        else if (RoleValidator.TL_Technical_DesignationIds.Contains(timesheet.DesignationId.Value)
                                         || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(timesheet.DesignationId.Value)
                                         || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(timesheet.DesignationId.Value)
                                         || RoleValidator.TL_UIUX_DesignationIds.Contains(timesheet.DesignationId.Value)
                                         || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(timesheet.DesignationId.Value)            
                                         || RoleValidator.TL_ITInfra_DesignationIds.Contains(timesheet.DesignationId.Value)
                                                )
                                        {
                                            if (!TimesheetsIdsAndRoleList.ContainsKey(timesheet.UserTimeSheetId))
                                            {
                                                TimesheetsIdsAndRoleList.Add(timesheet.UserTimeSheetId, timesheet.RoleId);
                                                TLHours = TLHours.Add(timesheet.WorkHours);
                                            }
                                        }                                        
                                        else if (RoleValidator.BA_RoleIds.Contains(timesheet.RoleId.Value))
                                        {
                                            if (!TimesheetsIdsAndRoleList.ContainsKey(timesheet.UserTimeSheetId))
                                            {
                                                TimesheetsIdsAndRoleList.Add(timesheet.UserTimeSheetId, timesheet.RoleId);
                                                BAHours = BAHours.Add(timesheet.WorkHours);
                                            }
                                        }
                                        //switch (timesheet.RoleId)
                                        //{
                                        //    case (int)Enums.UserRoles.DV:
                                        //        if (!TimesheetsIdsAndRoleList.ContainsKey(timesheet.UserTimeSheetId))
                                        //        {
                                        //            TimesheetsIdsAndRoleList.Add(timesheet.UserTimeSheetId, timesheet.RoleId);
                                        //            DevloperHours = DevloperHours.Add(timesheet.WorkHours);
                                        //        }
                                        //        break;
                                        //    case (int)Enums.UserRoles.DVManagerial:
                                        //    case (int)Enums.UserRoles.DVPManagerial:
                                        //    case (int)Enums.UserRoles.QAManagerial:
                                        //    case (int)Enums.UserRoles.QAPManagerial:
                                        //    case (int)Enums.UserRoles.UIUXManagerial:
                                        //    case (int)Enums.UserRoles.GamingManagerial:
                                        //    case (int)Enums.UserRoles.UIUXDesigner:
                                        //        if (!TimesheetsIdsAndRoleList.ContainsKey(timesheet.UserTimeSheetId))
                                        //        {
                                        //            TimesheetsIdsAndRoleList.Add(timesheet.UserTimeSheetId, timesheet.RoleId);
                                        //            TLHours = TLHours.Add(timesheet.WorkHours);
                                        //        }
                                        //        break;
                                        //    case (int)Enums.UserRoles.UIUXDeveloper:
                                        //    case (int)Enums.UserRoles.UIUXFrontEndDeveloper:
                                        //    case (int)Enums.UserRoles.UIUXMeanStackDeveloper:
                                        //        if (!TimesheetsIdsAndRoleList.ContainsKey(timesheet.UserTimeSheetId))
                                        //        {
                                        //            TimesheetsIdsAndRoleList.Add(timesheet.UserTimeSheetId, timesheet.RoleId);
                                        //            DesignerHours = DesignerHours.Add(timesheet.WorkHours);
                                        //        }
                                        //        break;
                                        //    case (int)Enums.UserRoles.BAPreSales:
                                        //    case (int)Enums.UserRoles.BAPrePostSales:
                                        //        if (!TimesheetsIdsAndRoleList.ContainsKey(timesheet.UserTimeSheetId))
                                        //        {
                                        //            TimesheetsIdsAndRoleList.Add(timesheet.UserTimeSheetId, timesheet.RoleId);
                                        //            BAHours = BAHours.Add(timesheet.WorkHours);
                                        //        }
                                        //        break;
                                        //}

                                    }


                                }
                            }
                        }
                    }
                    else
                    {
                        //WriteLogFile($"Response : No response from API");
                    }
                }
                else
                {
                    //WriteLogFile($"Error Response : Code = {response.StatusCode} Description = {response.StatusDescription}");
                }
                WorkingHourDto workingHourDto = new WorkingHourDto();
                workingHourDto.DeveloperHours = DevloperHours.TotalHours > 0 ? Math.Ceiling(DevloperHours.TotalHours).ToString() : "-";
                workingHourDto.TLHours = TLHours.TotalHours > 0 ? Math.Ceiling(TLHours.TotalHours).ToString() : "-";
                workingHourDto.DesignerHours = DesignerHours.TotalHours > 0 ? Math.Ceiling(DesignerHours.TotalHours).ToString() : "-";
                workingHourDto.BAHours = BAHours.TotalHours > 0 ? Math.Ceiling(BAHours.TotalHours).ToString() : "-";
                List<WorkingHourDto> workingHourList = new List<WorkingHourDto>();
                workingHourList.Add(workingHourDto);

                return DataTablesJsonResult(1, dataTablesRequest, workingHourList.Select((r, index) => new
                {
                    Developer = r.DeveloperHours,
                    TL = r.TLHours,
                    Designer = r.DesignerHours,
                    BA = r.BAHours
                }));
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(
                            new RequestOutcome<string>
                            {
                                ErrorMessage = "Error",
                                IsSuccess = false,
                                Data = ""
                            });
            }
        }

        /// <summary>
        /// Action for returning Timesheet data by role
        /// </summary>
        /// <param name="dataTablesRequest">request object</param>
        /// <param name="roleIds">roleIds from js to find Timesheet list</param>
        /// <returns></returns>
        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult TimesheetsByRole(IDataTablesRequest dataTablesRequest, decimal[] roleIds)
        {
            try
            {
                var timeSheetData = TempData.Get<TimeSheetWorkHour>("TimeSheetData");
                TempData.Keep("TimeSheetData");
                DateTime monthStartDate = new DateTime(Convert.ToInt32(timeSheetData.year), Convert.ToInt32(timeSheetData.month), 1);
                DateTime monthEndDate = monthStartDate.AddMonths(1).AddDays(-1);

                var response = GetTimeSheetResponse(monthStartDate, monthEndDate);


                string responseData = "";
                var crmResponse = new ResponseModel<string>();

                TimeSpan DevloperHours = new TimeSpan();
                TimeSpan TLHours = new TimeSpan();
                TimeSpan DesignerHours = new TimeSpan();
                TimeSpan BAHours = new TimeSpan();
                Dictionary<decimal, int?> TimesheetsIdsAndRoleList = new Dictionary<decimal, int?>();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    if (responseData.HasValue())
                    {
                        if (responseData != null)
                        {
                            JObject obj = JObject.Parse(responseData);

                            var json = JsonConvert.SerializeObject(obj.SelectToken("Data.invoiceDate"));

                            List<InvoiceWorkingHourjson> invoiceList = null;

                            invoiceList = JsonConvert.DeserializeObject<List<InvoiceWorkingHourjson>>(json);

                            if (crmResponse != null && invoiceList.Count > 0)
                            {
                                var startDate = new DateTime();
                                var endDate = new DateTime();
                                var crmIds = timeSheetData.crmIds;
                                List<GetMonthTimesheetsNew_Result> timesheets = timesheetService.GetMonthTimesheetsDataFromSPNew(monthStartDate, monthEndDate, crmIds);
                                foreach (var invoice in invoiceList)
                                {
                                    DateTime invoiceStartEndDate = Convert.ToDateTime(invoice.start_date);
                                    if (invoiceStartEndDate < monthStartDate)
                                    {
                                        startDate = monthStartDate;
                                    }
                                    else
                                    {
                                        startDate = invoiceStartEndDate;
                                    }
                                    invoiceStartEndDate = Convert.ToDateTime(invoice.end_date);
                                    if (invoiceStartEndDate > monthEndDate)
                                    {
                                        endDate = monthEndDate;
                                    }
                                    else
                                    {
                                        endDate = invoiceStartEndDate;
                                    }

                                    var CRMId = !string.IsNullOrWhiteSpace(invoice.crm_id) ? Convert.ToInt32(invoice.crm_id) : 0;
                                    foreach (var timesheet in timesheets.Where(t => t.AddDate >= startDate && t.AddDate <= endDate && t.CRMProjectId == CRMId))
                                    {
                                        if (RoleValidator.DV_Technical_DesignationIds.Contains(timesheet.DesignationId.Value))
                                        {
                                            if (!TimesheetsIdsAndRoleList.ContainsKey(timesheet.UserTimeSheetId))
                                            {
                                                TimesheetsIdsAndRoleList.Add(timesheet.UserTimeSheetId, timesheet.RoleId);
                                                DevloperHours = DevloperHours.Add(timesheet.WorkHours);
                                            }
                                        }
                                        else if (RoleValidator.DV_UIUX_DesignationIds.Contains(timesheet.DesignationId.Value))
                                        {
                                            if (!TimesheetsIdsAndRoleList.ContainsKey(timesheet.UserTimeSheetId))
                                            {
                                                TimesheetsIdsAndRoleList.Add(timesheet.UserTimeSheetId, timesheet.RoleId);
                                                DesignerHours = DesignerHours.Add(timesheet.WorkHours);
                                            }
                                        }
                                        else if (RoleValidator.TL_Technical_DesignationIds.Contains(timesheet.DesignationId.Value)
                                         || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(timesheet.DesignationId.Value)
                                         || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(timesheet.DesignationId.Value)
                                         || RoleValidator.TL_UIUX_DesignationIds.Contains(timesheet.DesignationId.Value)
                                         || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(timesheet.DesignationId.Value)
                                         || RoleValidator.TL_ITInfra_DesignationIds.Contains(timesheet.DesignationId.Value)) {
                                            if (!TimesheetsIdsAndRoleList.ContainsKey(timesheet.UserTimeSheetId))
                                            {
                                                TimesheetsIdsAndRoleList.Add(timesheet.UserTimeSheetId, timesheet.RoleId);
                                                TLHours = TLHours.Add(timesheet.WorkHours);
                                            }
                                        }
                                        
                                        else if (RoleValidator.BA_RoleIds.Contains(timesheet.RoleId.Value))
                                        {
                                            if (!TimesheetsIdsAndRoleList.ContainsKey(timesheet.UserTimeSheetId))
                                            {
                                                TimesheetsIdsAndRoleList.Add(timesheet.UserTimeSheetId, timesheet.RoleId);
                                                BAHours = BAHours.Add(timesheet.WorkHours);
                                            }
                                        }
                                          
                                        //switch (timesheet.RoleId)
                                        //{
                                        //    case (int)Enums.UserRoles.DV:
                                        //        if (!TimesheetsIdsAndRoleList.ContainsKey(timesheet.UserTimeSheetId))
                                        //        {
                                        //            TimesheetsIdsAndRoleList.Add(timesheet.UserTimeSheetId, timesheet.RoleId);
                                        //            DevloperHours = DevloperHours.Add(timesheet.WorkHours);
                                        //        }
                                        //        break;
                                        //    case (int)Enums.UserRoles.DVManagerial:
                                        //    case (int)Enums.UserRoles.DVPManagerial:
                                        //    case (int)Enums.UserRoles.QAManagerial:
                                        //    case (int)Enums.UserRoles.QAPManagerial:
                                        //    case (int)Enums.UserRoles.UIUXManagerial:
                                        //    case (int)Enums.UserRoles.GamingManagerial:
                                        //    case (int)Enums.UserRoles.UIUXDesigner:
                                        //        if (!TimesheetsIdsAndRoleList.ContainsKey(timesheet.UserTimeSheetId))
                                        //        {
                                        //            TimesheetsIdsAndRoleList.Add(timesheet.UserTimeSheetId, timesheet.RoleId);
                                        //            TLHours = TLHours.Add(timesheet.WorkHours);
                                        //        }
                                        //        break;
                                        //    case (int)Enums.UserRoles.UIUXDeveloper:
                                        //    case (int)Enums.UserRoles.UIUXFrontEndDeveloper:
                                        //    case (int)Enums.UserRoles.UIUXMeanStackDeveloper:
                                        //        if (!TimesheetsIdsAndRoleList.ContainsKey(timesheet.UserTimeSheetId))
                                        //        {
                                        //            TimesheetsIdsAndRoleList.Add(timesheet.UserTimeSheetId, timesheet.RoleId);
                                        //            DesignerHours = DesignerHours.Add(timesheet.WorkHours);
                                        //        }
                                        //        break;
                                        //    case (int)Enums.UserRoles.BAPreSales:
                                        //    case (int)Enums.UserRoles.BAPrePostSales:
                                        //        if (!TimesheetsIdsAndRoleList.ContainsKey(timesheet.UserTimeSheetId))
                                        //        {
                                        //            TimesheetsIdsAndRoleList.Add(timesheet.UserTimeSheetId, timesheet.RoleId);
                                        //            BAHours = BAHours.Add(timesheet.WorkHours);
                                        //        }
                                        //        break;
                                        //}

                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //WriteLogFile($"Response : No response from API");
                    }
                }
                else
                {
                    //WriteLogFile($"Error Response : Code = {response.StatusCode} Description = {response.StatusDescription}");
                }

                if (roleIds != null && roleIds.Length > 0)
                {

                    var TimeSheetIds = TimesheetsIdsAndRoleList?.Where(d => roleIds.Contains(d.Value.Value))
                        .Select(t => t.Key);
                    if (TimeSheetIds != null && TimeSheetIds.Count() > 0)
                    {

                        var result = timesheetService.GetTimesheetsByIdSP(string.Join(",", TimeSheetIds), dataTablesRequest.Start, dataTablesRequest.Length);
                        return DataTablesJsonResult(result.FirstOrDefault().TotalCount.Value, dataTablesRequest, result
                            .Select((r, index) => new
                            {
                                Date = r.AddDate.ToFormatDateString("yyyy-MM-dd"),
                                r.Name,
                                Project = r.ProjectName,
                                r.VirtualDeveloper,
                                Hours = r.WorkHours.TotalHours.ToString("0.##"),
                                r.Description
                            }));
                    }
                    else
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "", IsSuccess = true, Data = "" });
                    }

                }
                else
                {
                    return NewtonSoftJsonResult(
                        new RequestOutcome<string> { ErrorMessage = "Role not available", IsSuccess = false, Data = "" });
                }
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(
                            new RequestOutcome<string> { ErrorMessage = "Error on page", IsSuccess = false, Data = "" });
            }

        }

        //public IActionResult GetPerformance(int id,string startDate, string endDate)
        //{
        //    var user = userLoginService.GetUserInfoByID(id);
        //    if(user!=null)
        //    {
        //        DateTime? sDate = startDate.ToDateTime("dd/MM/yyyy");
        //        DateTime? eDate = endDate.ToDateTime("dd/MM/yyyy");

        //        if (!sDate.HasValue || !eDate.HasValue)
        //        {
        //            sDate = DateTime.Now.Date.AddDays(-10);
        //            eDate = DateTime.Now.Date.AddDays(-1);
        //        }

        //        PerformanceDto performanceDto = new PerformanceDto();
        //        performanceDto.StartDate = startDate;
        //        performanceDto.EndDate = endDate;
        //        performanceDto.Uid = user.Uid;
        //        performanceDto.EmployeeCode = user.EmpCode;
        //        performanceDto.Name = user.Name;
        //        performanceDto.Address = user.Address;
        //        performanceDto.PhoneNumber = user.PhoneNumber;
        //        performanceDto.DateOfJoining = user.JoinedDate.HasValue? ((DateTime)user.JoinedDate).ToString("dd/MM/yyyy",CultureInfo.InvariantCulture):"";
        //        performanceDto.Position = user.RoleId!=null? user.Role.RoleName:"";
        //        performanceDto.DepartmentName = user.DeptId!=null?user.Department.Name:"";

        //        //performanceDto.Appreciation
        //        performanceDto.ReportingPerson = user.TLId.HasValue ? userLoginService.GetNameById((int)user.TLId) : "";
        //        var complaints = complaintService.GetComplaints(id, (DateTime)sDate, (DateTime)eDate);
        //        if (complaints!=null)
        //        {
        //            performanceDto.NoOfComplaints = complaints.Count;

        //        }

        //        var projects = GetTimesheetSummary(id, startDate, endDate);
        //        if(projects!=null)
        //        {
        //            performanceDto.NoOfProjectWorking = projects.Count;
        //        }



        //        int freedays, workingDays,paidDays;
        //        GetTotalWorkingdays(id,sDate, eDate, out workingDays, out freedays,out paidDays);
        //        performanceDto.NoOfWorkingDays = workingDays;
        //        performanceDto.NoOfFreeDays = freedays;
        //        performanceDto.NoOfTotalWorkingDays = paidDays;

        //        var closureUnique = projectClosureService.GetProjectClosedList(id,user.PMUid, (DateTime)sDate, (DateTime)eDate,"unique");
        //        if (closureUnique!=null && closureUnique.Count>0)
        //        {
        //            performanceDto.NoOfUniqueProjectClosed = closureUnique.Count; 
        //        }
        //        var closure = projectClosureService.GetProjectClosedList(id, user.PMUid, (DateTime)sDate, (DateTime)eDate, "all");
        //        if (closure != null && closure.Count > 0)
        //        {
        //            performanceDto.NoOfProjectClosed = closure.Count;
        //        }

        //        var appraises= appraiseService.GetAppraises(GetAppraiseFilterExpersion(id, sDate.Value, eDate.Value));

        //        if(appraises !=null)
        //        {
        //            performanceDto.Appreciation = appraises.Count;
        //        }
        //        var leads = leadService.GetLeads(id,user.PMUid,sDate.Value, eDate.Value);

        //        if(leads!=null)
        //        {
        //            performanceDto.EstimateGiven = leads.Count;
        //        }
        //        var leadsAwarded = leadService.GetLeadsAwarded(id, user.PMUid, sDate.Value, eDate.Value);
        //        if (leadsAwarded != null)
        //        {
        //            performanceDto.EstimateAwarded = leadsAwarded.Count;
        //        }
        //        var additionalSupport = projectService.GetAdditionalSupportInDuration(id, user.PMUid, sDate.Value, eDate.Value);
        //        if(additionalSupport!=null)
        //        {
        //            var addStartDate = new DateTime();
        //            var addEndDate = new DateTime();
        //            List<DateTime> allDates = new List<DateTime>();
        //            foreach (var item in additionalSupport)
        //            {
        //                //setting start date and end date in our searched range
        //                addStartDate = item.StartDate >= sDate.Value ? item.StartDate : sDate.Value;
        //                addEndDate = item.EndDate >= eDate.Value ? eDate.Value : item.EndDate;
        //                for (DateTime date = addStartDate; date <= addEndDate; date = date.AddDays(1))
        //                {
        //                    allDates.Add(date);
        //                }
        //            }
        //            performanceDto.AdditionalSupportReceived = allDates.Distinct().Count();
        //        }


        //        return PartialView("_Performance",performanceDto);
        //    }
        //    else
        //    {
        //        return MessagePartialView("No details found for selected user");
        //    }

        //}
        //private void GetTotalWorkingdays(int id,DateTime? sDate,DateTime? eDate,out int workingDays, 
        //    out int freeDays,out int paidDays)
        //{
        //    workingDays = 0;
        //    freeDays = paidDays =0;
        //    byte countryId = 2;
        //    //List<OfficialLeave> _leaveList = leaveService.GetOfficialLeavesList(countryId);
        //    var compareDate = sDate.Value;
        //    List<DateTime> excludingDates = new List<DateTime>();
        //    var leaves = leaveService.GetOfficialLeavesInDuration(sDate.Value,eDate.Value, countryId,true)?.Select(l => l.LeaveDate).ToList();

        //    while (compareDate <= eDate.Value)
        //    {
        //        if (compareDate.DayOfWeek != DayOfWeek.Saturday && compareDate.DayOfWeek != DayOfWeek.Sunday)
        //        {
        //            if(leaves !=null && leaves.Contains(compareDate))
        //            {
        //                excludingDates.Add(compareDate);
        //            }
        //            else
        //            {
        //                workingDays++;
        //            }
        //        }
        //        else
        //        {
        //            excludingDates.Add(compareDate);
        //        }
        //        compareDate = compareDate.AddDays(1);
        //    }

        //    if(leaves!=null)
        //    {

        //    }

        //    userActivityService.GetTotalPaidDays(id, sDate.Value, eDate.Value, excludingDates, out paidDays);

        //        freeDays= workingDays - paidDays;
        //}
        //private Expression<Func<EmployeeAppraise, bool>> GetAppraiseFilterExpersion(int uid, DateTime startdate, DateTime endDate)
        //{
        //    var expr = PredicateBuilder.True<EmployeeAppraise>();
        //    expr = expr.And(e => e.IsActive == true && e.IsDelete == false);
        //    expr = expr.And(e => e.AddDate >= startdate && e.AddDate <= endDate && e.EmployeeId==uid);

        //    if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
        //    {
        //        expr = expr.And(e => e.UserId == CurrentUser.Uid || e.UserLogin1.PMUid == CurrentUser.Uid);
        //    }
        //    else if (CurrentUser.RoleId == (int)Enums.UserRoles.TL)
        //    {
        //        expr = expr.And(x => x.UserId == CurrentUser.Uid || x.UserLogin.TLId == CurrentUser.Uid);
        //    }
        //    else
        //    {
        //        expr = expr.And(x => x.UserId == CurrentUser.Uid);
        //    }
        //    return expr;
        //}

        //public IActionResult GetDetails(int id,string startDate,string endDate, Enums.ActivityDetail activityDetail)
        //{
        //    if (id > 0)
        //    {
        //        DateTime? sDate = startDate.ToDateTime("dd/MM/yyyy");
        //        DateTime? eDate = endDate.ToDateTime("dd/MM/yyyy");

        //        if (!sDate.HasValue || !eDate.HasValue)
        //        {
        //            sDate = DateTime.Now.Date.AddDays(-10);
        //            eDate = DateTime.Now.Date.AddDays(-1);
        //        }

        //        var compareDate = sDate.Value;
        //        List<DateTime> excludingDates = new List<DateTime>();

        //        while (compareDate <= eDate.Value)
        //        {
        //            if (compareDate.DayOfWeek == DayOfWeek.Saturday || compareDate.DayOfWeek == DayOfWeek.Sunday)
        //            {
        //                excludingDates.Add(compareDate);
        //            }

        //            compareDate = compareDate.AddDays(1);
        //        }


        //        var activityDetails = userActivityService.GetUserActivityDetails(id, sDate.Value, eDate.Value, excludingDates, activityDetail);

        //        if (activityDetails != null)
        //        {
        //            return PartialView("_Freedays", activityDetails);
        //        }
        //    }
        //    return MessagePartialView("No details found for selected user");
        //}
        //public IActionResult GetAppraiseDetail(int id, string startDate, string endDate)
        //{
        //    DateTime? sDate, eDate;
        //    CheckDates(startDate,endDate,out sDate,out eDate);
        //    List<AppraiseDto> model=null;
        //    var appraises = appraiseService.GetAppraises(GetAppraiseFilterExpersion(id, sDate.Value, eDate.Value));
        //    if(appraises !=null)
        //    {
        //        model = appraises.Select(a =>
        //         new AppraiseDto
        //         {
        //             AppraiseType = (Enums.AppraiseType)a.AppraiseType,
        //             ClientDate = a.ClientDate.HasValue ? ((DateTime)a.ClientDate).ToString("dd/MM/yyyy",CultureInfo.InvariantCulture) : "",
        //             ClientComment=a.ClientComment,
        //             TlComment=a.TlComment,
        //             AddedDate=a.AddDate.HasValue ? ((DateTime)a.AddDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "",
        //         }).ToList();
        //    }
        //    return PartialView("_AppraiseDetail",model);
        //}
        //public IActionResult GetComplaintDetail(int id, string startDate, string endDate)
        //{
        //    DateTime? sDate, eDate;
        //    CheckDates(startDate, endDate, out sDate, out eDate);
        //    List<ComplaintDto> model = null;
        //    var complaints = complaintService.GetComplaints(id, (DateTime)sDate, (DateTime)eDate);
        //    if (complaints != null)
        //    {
        //        model = complaints.Select(c => new ComplaintDto
        //        {
        //            Id = c.Id,
        //            AreaofImprovement = c.AreaofImprovement,
        //            ComplaintTypeId = c.ComplaintType,
        //            PriorityId = c.Priority,
        //            ClientComplain = c.ClientComplain,
        //            AddedDate = c.AddedDate.ToString("dd/MM/yyyy")
        //        }).ToList();
        //    }
        //    return PartialView("_ComplaintDetail", model);

        //}

        //public IActionResult GetProjectsDetails(int id, string startDate, string endDate)
        //{
        //    var model=GetTimesheetSummary(id, startDate, endDate);
        //    return PartialView("_ProjectWorked",model);

        //    //SearchProjectTimesheetReport()
        //}
        //private void CheckDates(string startDate, string endDate,out DateTime? sDate,out DateTime? eDate)
        //{
        //    sDate = startDate.ToDateTime("dd/MM/yyyy");
        //    eDate = endDate.ToDateTime("dd/MM/yyyy");

        //    if (!sDate.HasValue || !eDate.HasValue)
        //    {
        //        sDate = DateTime.Now.Date.AddDays(-10);
        //        eDate = DateTime.Now.Date.AddDays(-1);
        //    }
        //}

        //private List<TimeSheetSummaryReportDto> GetTimesheetSummary(int id, string startDate, string endDate)
        //{
        //    DateTime? sDate, eDate;
        //    CheckDates(startDate, endDate, out sDate, out eDate);
        //    var expr = PredicateBuilder.True<UserTimeSheet>();
        //    expr = expr.And(e => e.UID == id && e.AddDate >= sDate.Value && e.AddDate <= eDate.Value);
        //    var timesheets = timesheetService.GetTimeSheetsByFilter(expr);
        //    if (timesheets != null)
        //    {
        //        return timesheets.GroupBy(x => new { x.Project.Name, x.Project.CRMProjectId }).OrderByDescending(T => T.Min(s => s.AddDate)).Select(x => new TimeSheetSummaryReportDto
        //        {
        //            ProjectName = x.Key.Name,
        //            CrmId = x.Key.CRMProjectId.ToString()
        //        }).ToList();
        //    }
        //    return null;
        //}

        //public IActionResult GetProjectClosed(int id, string startDate, string endDate, string type)
        //{
        //    var user = userLoginService.GetUserInfoByID(id);
        //    if (user != null)
        //    {
        //        DateTime? sDate, eDate;
        //        CheckDates(startDate, endDate, out sDate, out eDate);
        //        List<ProjectClosureDto> model = null;
        //        var closures = projectClosureService.GetProjectClosedList(id, user.PMUid, sDate.Value, eDate.Value, type);
        //        if (closures != null)
        //        {
        //            model = closures.Select(c => new ProjectClosureDto
        //            {
        //                ClientName = c.Project.Client?.Name ?? string.Empty,
        //                ProjectName = c.Project?.Name ?? string.Empty,
        //                BATLName = BATLName(c.UserLogin?.Name, c.UserLogin3?.Name),
        //                DateOfClosing = c.DateofClosing.HasValue ? c.DateofClosing.ToFormatDateString("dd/MM/yyyy"):""
        //            }).ToList();
        //        }
        //        return PartialView("_ProjectClosedDetails", model);
        //    }
        //    return MessagePartialView("User not found");
        //}
        ////private string StartEndDates(DateTime? startDate, DateTime? endDate)
        ////{
        ////    if (startDate.HasValue && endDate.HasValue)
        ////    {
        ////        return $"{startDate.ToFormatDateString("MMM d, yyyy")} / {endDate.ToFormatDateString("MMM d, yyyy")}";
        ////    }
        ////    else if (startDate.HasValue)
        ////    {
        ////        return startDate.ToFormatDateString("MMM d, yyyy");
        ////    }

        ////    return "";
        ////}
        //private string BATLName(string BAName,string TLName)
        //{
        //    if(!string.IsNullOrWhiteSpace(BAName) && !string.IsNullOrWhiteSpace(TLName))
        //    {
        //        return $"{BAName}/{TLName}";
        //    }
        //    else if(!string.IsNullOrWhiteSpace(TLName))
        //    {
        //        return TLName;
        //    }
        //    else if(!string.IsNullOrWhiteSpace(BAName))
        //    {
        //        return BAName;
        //    }
        //    return string.Empty;
        //}

        //public IActionResult GetPerformanceEstimateDetails(int id, string startDate, string endDate, string type)
        //{
        //    var user = userLoginService.GetUserInfoByID(id);
        //    DateTime? sDate, eDate;
        //    CheckDates(startDate, endDate, out sDate, out eDate);
        //    List<LeadDto> model = null;
        //    if (user != null)
        //    {
        //        List<ProjectLead> leads = null;
        //        if(type.Equals("converted"))
        //        {
        //            leads = leadService.GetLeadsAwarded(id, user.PMUid, sDate.Value, eDate.Value);
        //                            }
        //        else
        //        {
        //            leads = leadService.GetLeads(id, user.PMUid, sDate.Value, eDate.Value);
        //        }
        //        if (leads != null)
        //        {
        //            model = leads.Select(l => new LeadDto
        //            {
        //                Title = l.Title,
        //                ClientName = l.LeadClient?.Name
        //                  ?? "",
        //                EstimateTimeInDays = l.EstimateTimeinDay,
        //                AddDate =l.AddDate
        //            }).ToList();
        //        }
        //        return PartialView("_PerformanceLeadsDetail",model);
        //    }
        //    else
        //    {
        //        return MessagePartialView("User not found");
        //    }
        //}

        //public IActionResult GetPerformanceAdditionalSupportDetails(int id, string startDate, string endDate)
        //{
        //    var user = userLoginService.GetUserInfoByID(id);
        //    DateTime? sDate, eDate;
        //    CheckDates(startDate, endDate, out sDate, out eDate);
        //    List<ProjectAdditionalSupportDto> model = null;
        //    if (user != null)
        //    {
        //        List<ProjectAdditionalSupport> additionalSupports = null;

        //        additionalSupports = projectService .GetAdditionalSupportInDuration(id, user.PMUid, sDate.Value, eDate.Value);

        //        if (additionalSupports != null)
        //        {
        //            model = additionalSupports.Select(l => new ProjectAdditionalSupportDto
        //            {
        //                ProjectName = l.Project.Name,
        //                Description = l.Description,
        //                Period= $"{l.StartDate.ToFormatDateString("dd MMM yyyy")} to {l.EndDate.ToFormatDateString("dd MMM yyyy")}",
        //                StatusText = ((Enums.AddSupportRequestStatus)l.Status).GetEnumDisplayName()
        //            }).ToList();
        //        }
        //        return PartialView("_PerformanceAdditionalSupportDetails", model);
        //    }
        //    else
        //    {
        //        return MessagePartialView("User not found");
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        //-----------------------------------------------------------------
        [HttpGet]
        [CustomAuthorization(IsAshishTeam: true)]
        public IActionResult TeamStatusReportData()
        {
            if (!IfAshishTeamPMUId)
            {
                return AccessDenied();
            }
            else
            {
                TeamStatusReportDto teamStatusReportDto = new TeamStatusReportDto();

                teamStatusReportDto.TlList = userLoginService.GetTLSDUsers((CurrentUser.RoleId == (int)Core.Enums.UserRoles.PM || CurrentUser.RoleId == (int)Core.Enums.UserRoles.PMO) ? CurrentUser.Uid : CurrentUser.PMUid)
                    .Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.Uid.ToString() }).ToList();

                CultureInfo culture = new CultureInfo("en-IN");
                teamStatusReportDto.DateFrom = Common.GetStartDateOfWeek(DateTime.Today, culture).AddDays(-7).ToFormatDateString("dd/MM/yyyy");
                teamStatusReportDto.DateTo = Common.GetStartDateOfWeek(DateTime.Today, culture).AddDays(-3).ToFormatDateString("dd/MM/yyyy");

                return View("TeamStatusReport", teamStatusReportDto);
            }
        }


        [HttpPost]
        [CustomActionAuthorization]
        [CustomAuthorization(IsAshishTeam: true)]
        public IActionResult TeamStatusReport(TeamStatusReportDto model)
        {
            if (!IfAshishTeamPMUId)
            {
                return AccessDenied();
            }
            if (model.Uid != null)
            {
                var Uid = Convert.ToInt32(model.Uid);
                DateTime DateFrom = new DateTime();
                DateTime DateTo = new DateTime();
                if (model.IsCurrentWeek)
                {
                    DateFrom = DateTime.Today.AddDays((int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - (int)DateTime.Today.DayOfWeek);
                    DateTo = DateTime.Now.Date;
                }
                else if (model.IsCurrentMonth)
                {
                    DateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    DateTo = DateTime.Now.Date;
                }
                else
                {
                    DateFrom = (model.DateFrom.ToDateTime().Value);
                    DateTo = (model.DateTo.ToDateTime().Value);
                }
                var users = userActivityService.GetTeamStatusReportGraph(Uid, DateFrom, DateTo);
                List<TeamStatusReportGraphDto> objlst = new List<TeamStatusReportGraphDto>();
                foreach (var i in users)
                {
                    var totalEmp = teamHierarchyService.GetMemberCountOnDate(Convert.ToInt32(model.Uid), i.Date);
                    objlst.Add(new TeamStatusReportGraphDto
                    {
                        NoOfEmployee = i.NoOfEmployee,
                        Date = i.Date.ToString("dd/MM/yyyy"),
                        TlName = i.TlName,
                        //MemberOfTeam = i.MemberOfTeam,
                        MemberOfTeam = totalEmp > 0 ? totalEmp : i.MemberOfTeam,
                    });
                }
                return Json(objlst);
            }
            else
            {
                var Uid = SiteKey.AshishTeamPMUId;
                DateTime DateFrom = new DateTime();
                DateTime DateTo = new DateTime();
                if (model.IsCurrentWeek)
                {
                    DateFrom = DateTime.Today.AddDays((int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - (int)DateTime.Today.DayOfWeek);
                    DateTo = DateTime.Now.Date;
                }
                else if (model.IsCurrentMonth)
                {
                    DateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    DateTo = DateTime.Now.Date;
                }
                else
                {
                    DateFrom = (model.DateFrom.ToDateTime().Value);
                    DateTo = (model.DateTo.ToDateTime().Value);
                }
                var users = userActivityService.GetAllTeamStatusReportGraph(Uid, DateFrom, DateTo);
                List<TeamStatusReportGraphDto> objlst = new List<TeamStatusReportGraphDto>();
                foreach (var i in users)
                {
                    var totalEmp = teamHierarchyService.GetMemberCountOnDate(Uid, i.Date, true);
                    objlst.Add(new TeamStatusReportGraphDto
                    {
                        NoOfEmployee = i.NoOfEmployee,
                        Date = i.Date.ToString("dd/MM/yyyy"),
                        TlName = i.PMName,
                        MemberOfTeam = totalEmp > 0 ? totalEmp : i.MemberOfTeam,
                    });
                }
                return Json(objlst);
            }
        }

        [HttpPost]
        [CustomAuthorization(IsAshishTeam: true)]
        public IActionResult TeamStatusReportDetails(TeamStatusReportDto model)
        {
            if (!IfAshishTeamPMUId)
            {
                return AccessDenied();
            }

            var Uid = model.Uid != null ? Convert.ToInt32(model.Uid) : SiteKey.AshishTeamPMUId;
            var DateFrom = (model.DateFrom.ToDateTime().Value);
            var users = model.Uid != null ? userActivityService.GetTeamStatusReportGraphDetails(Uid, DateFrom) : userActivityService.GetAllTeamStatusReportGraphDetails(Uid, DateFrom);
            List<TeamStatusReportGraphDetailsDto> objlst = new List<TeamStatusReportGraphDetailsDto>();
            foreach (var u in users)
            {
                objlst.Add(new TeamStatusReportGraphDetailsDto
                {
                    Date = u.Date.ToString("dd/MM/yyyy"),
                    Employee = u.Employee,
                    ProjectName = u.ProjectName,
                    Status = u.Status
                });
            }
            return Json(objlst);
        }


        //---------------------------------------------------------------

        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult EmployeeWorkingHoursData()
        {
            if (!IfAshishTeamPMUId)
            {
                return AccessDenied();
            }
            else
            {
                EmployeeWorkingHoursDto workingHourDto = new EmployeeWorkingHoursDto();
                workingHourDto.WorkingHourTypes = WebExtensions.GetSelectList<Enums.WorkingHourType>();
                workingHourDto.Employees = userLoginService.GetSelfAndUsersListByAllRole(Convert.ToInt32(CurrentUser.Uid), Convert.ToInt32(CurrentUser.RoleId),CurrentUser.DesignationId).Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.Uid.ToString() }).ToList();
                workingHourDto.Departments = departmentService.GetActiveDepartments().ToSelectList(x => x.Name, x => x.DeptId);
                workingHourDto.DateFrom = DateTime.Today.ToFormatDateString("dd/MM/yyyy");
                //workingHourDto.DateTo = DateTime.Today.ToFormatDateString("dd/MM/yyyy");
                return View("Index", workingHourDto);
            }
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult GetEmployeeWorkingHoursData(IDataTablesRequest request, EmployeeWorkingHoursDto model)
        {

            if (!IfAshishTeamPMUId)
            {
                return AccessDenied();
            }
            var users = userLoginService.GetUserTLTogether(PMUserId);
            if (model.WorkingHourTypeId == Enums.WorkingHourType.NoPlannedHours && users != null)
            {
                users = users.Where(u => u.Uid != PMUserId).ToList();
                FillUserLeaveActivity(users, model.DateFrom.ToDateTime().Value, model.DateFrom.ToDateTime().Value);
            }

            int deptId, uid;
            List<string> emails = new List<string>();
            if (model.WorkingHourTypeId == 0 || model.WorkingHourTypeId == Enums.WorkingHourType.NoPlannedHours)
            {
                // Get all data
            }
            else if (model.WorkingHourTypeId == Enums.WorkingHourType.Department)
            {
                deptId = !string.IsNullOrWhiteSpace(model.DepartmentId) ? Convert.ToInt32(model.DepartmentId) : 0;
                if (deptId > 0)
                {
                    emails = userLoginService.GetUserEmailIdsByDepartment(deptId);
                }
            }
            else
            {
                uid = !string.IsNullOrWhiteSpace(model.Uid) ? Convert.ToInt32(model.Uid) : 0;
                var user = userLoginService.GetUserInfoByID(uid);
                if (user != null && !string.IsNullOrWhiteSpace(user.EmailOffice))
                {
                    emails.Add(user.EmailOffice.Trim().ToLower());
                }

            }
            UserLogin objUser = userLoginService.GetUserInfoByID(PMUserId);

            RequestObject reqObj = new RequestObject
            {
                FromDate = model.DateFrom.ToDateTime().Value.ToString("dd-MMM-yyyy"),
                ToDate = model.DateTo.ToDateTime().Value.ToString("dd-MMM-yyyy"),
                emailid = objUser.EmailOffice
            };
            ResponseRoot apiResult = GetDataFromAPI(reqObj);

            List<ResponseObject> response = null;
            if (emails != null && emails.Count > 0) // department or user specific data
            {
                response = apiResult != null && apiResult.ResponsePacket != null
                    ? apiResult.ResponsePacket.Where(r => r.MemberEmail != null && emails.Contains(r.MemberEmail.Trim().ToLower()))
                    .OrderBy(r => r.MemberEmail).ThenBy(r => r.ProjectName).ToList() : new List<ResponseObject>();
            }
            else //All data
            {
                response = apiResult != null && apiResult.ResponsePacket != null
                    ? apiResult.ResponsePacket.OrderBy(r => r.MemberEmail).ThenBy(r => r.ProjectName).ToList() : new List<ResponseObject>();
            }

            if (model.WorkingHourTypeId == Enums.WorkingHourType.NoPlannedHours)
            {
                var responseUsers = response.Where(r => !string.IsNullOrWhiteSpace(r.MemberEmail)).Select(r => r.MemberEmail.Trim().ToLower()).Distinct().ToList();
                var unPlannedUsers = users.Where(u => !responseUsers.Contains(u.EmployeeEmail))
                    .OrderBy(u => u.EmployeeName).ThenBy(u => u.EmployeeEmail).ToList();
                return DataTablesJsonResult(unPlannedUsers.Count, request, unPlannedUsers.Select((r, index) => new
                {
                    rowIndex = (index + 1) + (request.Start),
                    ProjectName = r.LeaveActivity != null ? r.LeaveActivity : "N/A",
                    MemberName = $"<strong>{r.EmployeeName}</strong><br/><small><i class='glyphicon glyphicon-envelope'></i> {r.EmployeeEmail}</small>" +
                    //$"<br/><i class='glyphicon glyphicon-phone'></i> {r.MobileNumber}" +
                    $"<br/>Reporting To: <strong>{r.TLName}</strong>",
                    TaskName = "N/A",
                    PlanHour = "N/A",
                    ActualHours = "N/A",
                })
                );
            }
            else
            {
                IDictionary<string, object> additionalParameters = new Dictionary<string, object>();
                var plannedHours = response.Sum(r => r.PlanHour);
                var actualHours = response.Sum(r => r.ActualHours);

                List<string> timesPlanned = response.Select(r => r.PlanHourEMS).ToList();
                List<string> timesActual = response.Select(r => r.ActualHoursEMS).ToList();

                string TotalPlannedHoursFormatted = CalculateTotalHours(timesPlanned);

                string TotalActualHoursFormatted = CalculateTotalHours(timesActual);

                foreach (var res in response)
                {
                    res.PlanHourEMS = res.PlanHourEMS.Replace("00", "0") + " M";
                    res.PlanHourEMS = res.PlanHourEMS.Replace("00:", "0:").Replace(":", " H ");
                    res.ActualHoursEMS = res.ActualHoursEMS.Replace("00", "0") + " M";
                    res.ActualHoursEMS = res.ActualHoursEMS.Replace("00:", "0:").Replace(":", " H ");

                }

                var query = from res in response
                            join user in users
                            on res.MemberEmail.ToLower() equals user.EmployeeEmail.ToLower() into gj
                            from subUser in gj.DefaultIfEmpty()
                            select new
                            {
                                res.MemberEmail,
                                res.MemberName,
                                res.PlanHour,
                                res.PlanHourEMS,
                                res.ActualHours,
                                res.ActualHoursEMS,
                                res.TaskName,
                                res.ProjectName,
                                res.CRMID,
                                subUser?.TLEmail,
                                subUser?.TLName,
                                string.Empty
                                //subUser?.MobileNumber
                            };
                var joinedResult = query.ToList();

                additionalParameters.Add(new KeyValuePair<string, object>("TotalPlannedHours", plannedHours));
                additionalParameters.Add(new KeyValuePair<string, object>("TotalActualHours", actualHours));
                additionalParameters.Add(new KeyValuePair<string, object>("TotalPlannedHoursFormatted", TotalPlannedHoursFormatted));
                additionalParameters.Add(new KeyValuePair<string, object>("TotalActualHoursFormatted", TotalActualHoursFormatted));

                return DataTablesJsonResult(joinedResult.Count, request, joinedResult.Select((r, index) => new
                {
                    rowIndex = (index + 1) + (request.Start),
                    CrmId = r.CRMID,
                    ProjectName = $"{r.ProjectName} [{r.CRMID}]",
                    r.MemberEmail,
                    MemberName = $"<strong>{r.MemberName}</strong><br/><small><i class='glyphicon glyphicon-envelope'></i> {r.MemberEmail}</small>" +
                    //$"<br/><i class='glyphicon glyphicon-phone'></i> {r.MobileNumber}" +
                    $"<br/>Reporting To: <strong>{r.TLName}</strong>",
                    r.TaskName,
                    // TLName=$"{r.TLName}<br/><small>{r.TLEmail}</small>",               
                    PlanHour = r.PlanHourEMS,
                    ActualHours = r.ActualHoursEMS
                }), additionalParameters);
            }

        }


        private ResponseRoot GetDataFromAPI(RequestObject request)
        {
            string url = SiteKey.PMSMemberListWithTimeLogServiceApiURL;
            ResponseRoot apiResult = null;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var content = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");
                    content.Headers.Add("ApiKey", SiteKey.PMSApiKey);
                    content.Headers.Add("ApiPassword", SiteKey.PMSApiPassword);
                    var result = client.PostAsync(url, content).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var jsonres = result.Content.ReadAsStringAsync().Result;
                        apiResult = JsonConvert.DeserializeObject<ResponseRoot>(jsonres);
                    }
                }
                catch (Exception ex) { }
            }
            return apiResult;
        }

        private string CalculateTotalHours(List<string> times)
        {
            long hours = 0;
            long minutes = 0;
            if (times != null)
            {
                foreach (string time in times)
                {
                    var timeParts = time.Split(':');
                    hours = hours + Convert.ToInt64(timeParts[0]);
                    minutes = minutes + Convert.ToInt64(timeParts[1]);
                }
            }
            long hoursFromMinutes = minutes / 60;
            hours = hours + hoursFromMinutes;
            minutes = minutes % 60;

            return $"{hours} H {minutes} M";
        }

        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult EmployeeWorkingHoursSummary()
        {
            if (!IfAshishTeamPMUId)
            {
                return AccessDenied();
            }
            else
            {
                EmployeeWorkingHoursSummaryDto workingHourDto = new EmployeeWorkingHoursSummaryDto();
                workingHourDto.WorkingHourTypes = WebExtensions.GetSelectList<Enums.WorkingHourSummaryType>();
                workingHourDto.Employees = userLoginService.GetSelfAndUsersListByAllRole(Convert.ToInt32(CurrentUser.Uid), Convert.ToInt32(CurrentUser.RoleId), Convert.ToInt32(CurrentUser.DesignationId)).Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.Uid.ToString() }).ToList();
                workingHourDto.Departments = departmentService.GetActiveDepartments().ToSelectList(x => x.Name, x => x.DeptId);
                workingHourDto.DateFrom = DateTime.Today.ToFormatDateString("dd/MM/yyyy");
                workingHourDto.DateTo = DateTime.Today.ToFormatDateString("dd/MM/yyyy");
                return View(workingHourDto);
            }
        }
        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult GetEmployeeWorkingHoursSummary(IDataTablesRequest request, EmployeeWorkingHoursSummaryDto model)
        {
            if (!IfAshishTeamPMUId)
            {
                return AccessDenied();
            }

            TempData.Put("EmployeeWorkingHoursSummaryFilter", model);
            int deptId, uid;
            List<string> emails = new List<string>();
            if (model.WorkingHourTypeId == Enums.WorkingHourSummaryType.Department)
            {
                deptId = !string.IsNullOrWhiteSpace(model.DepartmentId) ? Convert.ToInt32(model.DepartmentId) : 0;
                if (deptId > 0)
                {
                    emails = userLoginService.GetUserEmailIdsByDepartment(deptId);
                }
            }
            else if (model.WorkingHourTypeId == Enums.WorkingHourSummaryType.Employee)
            {
                uid = !string.IsNullOrWhiteSpace(model.Uid) ? Convert.ToInt32(model.Uid) : 0;
                var user = userLoginService.GetUserInfoByID(uid);
                if (user != null && !string.IsNullOrWhiteSpace(user.EmailOffice))
                {
                    emails.Add(user.EmailOffice);
                }

            }
            //else Get All Data

            List<WorkingHoursSummaryObject> summaryList;
            UserLogin objUser = userLoginService.GetUserInfoByID(PMUserId);
            var users = userLoginService.GetUserTLTogether(CurrentUser.PMUid);
            // excluding PM from users list
            if (users != null)
                users = users.Where(a => a.Uid != CurrentUser.PMUid && a.EmployeeName != null && a.EmployeeEmail != null).ToList();
            if (model.WorkingHourTypeId == Enums.WorkingHourSummaryType.Department && emails != null && users != null)
            {
                users = users.Where(a => a.Uid != CurrentUser.PMUid && a.EmployeeName != null && a.EmployeeEmail != null && emails.Contains(a.EmployeeEmail)).ToList();

            }
            RequestObject reqObj = new RequestObject
            {
                FromDate = model.DateFrom.ToDateTime().Value.ToString("dd-MMM-yyyy"),
                ToDate = model.DateTo.ToDateTime().Value.ToString("dd-MMM-yyyy"),
                emailid = objUser.EmailOffice //m25@mailinator.com
            };
            ResponseRoot apiResult = GetDataFromAPI(reqObj);

            List<ResponseObject> response = null;
            if (emails != null && emails.Count > 0) // department or user specific data
            {
                response = apiResult != null && apiResult.ResponsePacket != null
                ? apiResult.ResponsePacket.Where(r => emails.Contains(r.MemberEmail))
                .OrderBy(r => r.MemberEmail).ThenBy(r => r.ProjectName).ToList() : new List<ResponseObject>();
            }
            else //All data
            {
                response = apiResult != null && apiResult.ResponsePacket != null
                ? apiResult.ResponsePacket.OrderBy(r => r.MemberEmail).ThenBy(r => r.ProjectName).ToList() : new List<ResponseObject>();
            }

            // getting projectmanager email
            var pmEmailId = userLoginService.GetPmEmailId(CurrentUser.PMUid).ToLower();
            response = response.Where(a => !a.MemberEmail.Equals(pmEmailId)).ToList();

            if (model.WorkingHourTypeId == Enums.WorkingHourSummaryType.NoPlannedHours)
            {
                var responseUsers = response.Where(r => !string.IsNullOrWhiteSpace(r.MemberEmail)).Select(r => r.MemberEmail.Trim().ToLower()).Distinct().ToList();
                var unPlannedUsers = users.Where(u => !responseUsers.Contains(u.EmployeeEmail.ToLower()))
                .OrderBy(u => u.EmployeeName).ThenBy(u => u.EmployeeEmail).ToList();


                if (unPlannedUsers != null)
                {
                    FillUserLeaveActivity(unPlannedUsers, model.DateFrom.ToDateTime().Value, model.DateTo.ToDateTime().Value);
                }

                return DataTablesJsonResult(unPlannedUsers.Count, request, unPlannedUsers.Select((r, index) => new
                {
                    rowIndex = (index + 1) + (request.Start),
                    ColorCode = "white",
                    MemberName = $"<strong>{r.EmployeeName}</strong><br/><small><i class='glyphicon glyphicon-envelope'></i> {r.EmployeeEmail}</small><br/>" +
                    //$"<br/><i class='glyphicon glyphicon-phone'></i> {r.MobileNumber}<br/>" +
                    $"Reporting To: <strong>{r.TLName}</strong>"
                + $"{(!string.IsNullOrEmpty(r.LeaveActivity) ? "<br/><span class='label label-primary' style='font-size:12px;margin-top: 2px;display: inline-block;'>" + r.LeaveActivity + "</span>" : string.Empty)}",
                    ProjectHtml = "NA",
                    TotalPlanHours = "NA",
                    TotalActualHours = "NA"
                })
                );
            }

            if (users != null)
            {
                //call to fill leave data
                FillUserLeaveActivity(users, model.DateFrom.ToDateTime().Value, model.DateTo.ToDateTime().Value);
            }


            var joinedResult = new List<ResponseSummary>();

            if (model.WorkingHourTypeId == Enums.WorkingHourSummaryType.Department)
            {
                joinedResult = (from user in users
                                join res in response
                                on user.EmployeeEmail.ToLower().Trim() equals res.MemberEmail.ToLower().Trim()
                                into gj
                                from subUser in gj.DefaultIfEmpty()
                                select new ResponseSummary
                                {
                                    MemberEmail = subUser?.MemberEmail ?? user.EmployeeEmail,
                                    MemberName = subUser?.MemberName ?? user.EmployeeName,
                                    PlanHour = subUser?.PlanHour ?? 0,
                                    PlanHourEMS = subUser?.PlanHourEMS ?? "N/A",
                                    ActualHours = subUser?.ActualHours ?? 0,
                                    ActualHoursEMS = subUser?.ActualHoursEMS ?? "N/A",
                                    TaskName = subUser?.TaskName ?? "N/A",
                                    ProjectName = subUser?.ProjectName,
                                    CRMID = subUser?.CRMID,
                                    TLEmail = user?.TLEmail,
                                    TLName = user?.TLName,
                                    MobileNumber = string.Empty,
                                    LeaveActivity = user?.LeaveActivity
                                }).ToList();
                //queries =query.ToList();

            }
            else
            {
                joinedResult = (from res in response
                                join user in users
                                on res.MemberEmail.ToLower().Trim() equals user.EmployeeEmail.ToLower().Trim() into gj
                                from subUser in gj.DefaultIfEmpty()
                                select new ResponseSummary
                                {
                                    MemberEmail = res.MemberEmail,
                                    MemberName = res.MemberName,
                                    PlanHour = res.PlanHour,
                                    PlanHourEMS = res.PlanHourEMS,
                                    ActualHours = res.ActualHours,
                                    ActualHoursEMS = res.ActualHoursEMS,
                                    TaskName = res.TaskName,
                                    ProjectName = res.ProjectName,
                                    CRMID = res.CRMID,
                                    TLEmail = subUser?.TLEmail,
                                    TLName = subUser?.TLName,
                                    MobileNumber = string.Empty,
                                    LeaveActivity = subUser?.LeaveActivity
                                }).ToList();
                //queries = query.ToList();
            }
            //var joinedResult = queries.ToList();

            summaryList = joinedResult.GroupBy(x => x.MemberEmail).Select((a, index) => new WorkingHoursSummaryObject
            {
                rowIndex = (index + 1) + (request.Start),
                MemberEmail = a.Key,
                MemberName = $"<strong>{a.First().MemberName}</strong><br/>" +
            $"<small><i class='glyphicon glyphicon-envelope'></i> {a.First().MemberEmail}</small>" +
            //$"<br/><i class='glyphicon glyphicon-phone'></i> {a.First().MobileNumber}" +
            $"<br/>Reporting To: <strong>{a.First().TLName}</strong>" +
            $"{(!string.IsNullOrEmpty(a.First().LeaveActivity) ? "<br/><span class='label label-primary' style='font-size:12px;margin-top: 2px;display: inline-block;'>" + a.First().LeaveActivity + "</span>" : string.Empty)}",
                TotalActualHours = a.Sum(b => b.ActualHours),//Convert.ToInt32((a.Sum(b => b.ActualHours)).ToString()=="0"?"N/A": (a.Sum(b => b.ActualHours)).ToString()),
                TotalPlanHours = a.Sum(b => b.PlanHour),
                ColorCode = "white",
                ProjectHtml = "NA"
            }).ToList();

            //summaryList.ForEach(a => { a.TotalPlanHours = 16.30; a.TotalActualHours = 20; });


            double AboveAllowedPer = 20;
            int days = model.DateTo.ToDateTime().Value.Date.Subtract(model.DateFrom.ToDateTime().Value.Date).Days + 1;

            double minimumHours = days * 8.0;
            if (model.WorkingHourTypeId.Equals(Enums.WorkingHourSummaryType.LessThenEight))
            {
                summaryList = summaryList.Where(x => x.TotalPlanHours < minimumHours).ToList();
            }
            else if (model.WorkingHourTypeId.Equals(Enums.WorkingHourSummaryType.PlansEqualsActual))
            {
                summaryList = summaryList.Where(x => x.TotalPlanHours.Equals(x.TotalActualHours) && x.TotalPlanHours >= minimumHours).ToList();
            }
            else if (model.WorkingHourTypeId.Equals(Enums.WorkingHourSummaryType.MoreThenTwentyPer))
            {
                summaryList = summaryList.Where(x => x.TotalActualHours > (x.TotalPlanHours + (AboveAllowedPer / 100 * x.TotalPlanHours)) && x.TotalPlanHours >= minimumHours).ToList();
            }
            else if (model.WorkingHourTypeId.Equals(Enums.WorkingHourSummaryType.NoActualHours))
            {
                summaryList = summaryList.Where(x => x.TotalActualHours == 0).ToList();
            }
            foreach (var item in summaryList)
            {
                item.ColorCode = item.TotalPlanHours < minimumHours ? "red" : item.TotalPlanHours == item.TotalActualHours ? "orange" : "white";
                if (item.TotalPlanHours > 0 && item.TotalActualHours > (item.TotalPlanHours + (AboveAllowedPer / 100 * item.TotalPlanHours)))
                    item.ColorCode = "yellow";

                //item.ProjectHtml = "<table class='table table-stats table-condensed' style='margin:0px;border:1px solid black;;'>";
                item.ProjectHtml = "<table class='table' style='margin:0px;color:black;'>";
                var userProjects = response.Where(x => x.MemberEmail == item.MemberEmail).ToList();
                foreach (var prj in userProjects.GroupBy(a => a.ProjectName).Select(a => a.Key).ToList())
                {
                    double planned = userProjects.Where(a => a.ProjectName == prj).Sum(a => a.PlanHour);
                    double actual = userProjects.Where(a => a.ProjectName == prj).Sum(a => a.ActualHours);
                    string rowBackColor = item.ColorCode;
                    int prjid = userProjects.Where(a => a.ProjectName == prj).First().CRMID ?? 0;

                    item.ProjectHtml += $"<tr style='background:{rowBackColor}'><td style='width:50%;text-align:left;'>{userProjects.Where(a => a.ProjectName == prj).First().ProjectName} [{prjid}]</td>";
                    item.ProjectHtml += $"<td style='width:24%;text-align:right;'>{this.GetFormattedTime(planned)}</td>";
                    item.ProjectHtml += $"<td style='text-align:right;'>{this.GetFormattedTime(actual)}</td></tr>";
                }
                item.ProjectHtml += "</table>";
                if (item.ProjectHtml == "<table class='table' style='margin:0px;color:black;'></table>")
                {
                    item.ProjectHtml = "N/A";
                }
            }
            IDictionary<string, object> additionalParameters = new Dictionary<string, object>();
            var plannedHours = summaryList.Sum(r => r.TotalPlanHours);
            var actualHours = summaryList.Sum(r => r.TotalActualHours);
            var TotalPlannedHoursFormatted = this.GetFormattedTime(plannedHours);
            var TotalActualHoursFormatted = this.GetFormattedTime(actualHours);

            additionalParameters.Add(new KeyValuePair<string, object>("TotalPlannedHours", plannedHours));
            additionalParameters.Add(new KeyValuePair<string, object>("TotalActualHours", actualHours));
            additionalParameters.Add(new KeyValuePair<string, object>("TotalPlannedHoursFormatted", TotalPlannedHoursFormatted));
            additionalParameters.Add(new KeyValuePair<string, object>("TotalActualHoursFormatted", TotalActualHoursFormatted));
            return DataTablesJsonResult(summaryList.Count, request, summaryList.Select((r, index) => new
            {
                days = days,
                rowIndex = (index + 1),
                r.ColorCode,
                r.MemberName,
                r.ProjectHtml,
                TotalPlanHours = this.GetFormattedTime(r.TotalPlanHours) == "-" ? "N/A" : this.GetFormattedTime(r.TotalPlanHours),
                TotalActualHours = this.GetFormattedTime(r.TotalActualHours) == "-" ? "N/A" : this.GetFormattedTime(r.TotalActualHours),
            }), additionalParameters);
        }

        //[HttpPost]
        //[CustomActionAuthorization]
        //public IActionResult GetEmployeeWorkingHoursSummary(IDataTablesRequest request, EmployeeWorkingHoursSummaryDto model)
        //{
        //    if (!IfAshishTeamPMUId)
        //    {
        //        return AccessDenied();
        //    }

        //    TempData.Put("EmployeeWorkingHoursSummaryFilter", model);
        //    int deptId, uid;
        //    List<string> emails = new List<string>();
        //    if (model.WorkingHourTypeId == Enums.WorkingHourSummaryType.Department)
        //    {
        //        deptId = !string.IsNullOrWhiteSpace(model.DepartmentId) ? Convert.ToInt32(model.DepartmentId) : 0;
        //        if (deptId > 0)
        //        {
        //            emails = userLoginService.GetUserEmailIdsByDepartment(deptId);
        //        }
        //    }
        //    else if(model.WorkingHourTypeId == Enums.WorkingHourSummaryType.Employee)
        //    {
        //        uid = !string.IsNullOrWhiteSpace(model.Uid) ? Convert.ToInt32(model.Uid) : 0;
        //        var user = userLoginService.GetUserInfoByID(uid);
        //        if (user != null && !string.IsNullOrWhiteSpace(user.EmailOffice))
        //        {
        //            emails.Add(user.EmailOffice);
        //        }

        //    }
        //    //else Get All Data

        //    List<WorkingHoursSummaryObject> summaryList;
        //    UserLogin objUser = userLoginService.GetUserInfoByID(PMUserId);
        //    var users = userLoginService.GetUserTLTogether(CurrentUser.PMUid);
        //    // excluding PM from users list
        //    if (users != null)
        //        users = users.Where(a => a.Uid != CurrentUser.PMUid).ToList();

        //    RequestObject reqObj = new RequestObject
        //    {
        //        FromDate = model.DateFrom.ToDateTime().Value.ToString("dd-MMM-yyyy"),
        //        ToDate = model.DateTo.ToDateTime().Value.ToString("dd-MMM-yyyy"),
        //        emailid = objUser.EmailOffice //m25@mailinator.com
        //    };
        //    ResponseRoot apiResult = GetDataFromAPI(reqObj);

        //    List<ResponseObject> response = null;
        //    if (emails != null && emails.Count > 0) // department or user specific data
        //    {
        //        response = apiResult != null && apiResult.ResponsePacket != null
        //            ? apiResult.ResponsePacket.Where(r => emails.Contains(r.MemberEmail))
        //            .OrderBy(r => r.MemberEmail).ThenBy(r => r.ProjectName).ToList() : new List<ResponseObject>();
        //    }
        //    else //All data
        //    {
        //        response = apiResult != null && apiResult.ResponsePacket != null
        //            ? apiResult.ResponsePacket.OrderBy(r => r.MemberEmail).ThenBy(r => r.ProjectName).ToList() : new List<ResponseObject>();
        //    }

        //    // getting projectmanager email
        //    var pmEmailId = userLoginService.GetPmEmailId(CurrentUser.PMUid).ToLower();
        //    response = response.Where(a => !a.MemberEmail.Equals(pmEmailId)).ToList();

        //    if (model.WorkingHourTypeId == Enums.WorkingHourSummaryType.NoPlannedHours)
        //    {
        //        var responseUsers = response.Where(r => !string.IsNullOrWhiteSpace(r.MemberEmail)).Select(r => r.MemberEmail.Trim().ToLower()).Distinct().ToList();
        //        var unPlannedUsers = users.Where(u => !responseUsers.Contains(u.EmployeeEmail.ToLower()))
        //            .OrderBy(u => u.EmployeeName).ThenBy(u => u.EmployeeEmail).ToList();
        //        if(unPlannedUsers!=null)
        //        {
        //            FillUserLeaveActivity(unPlannedUsers, model.DateFrom.ToDateTime().Value, model.DateTo.ToDateTime().Value);
        //        }

        //        return DataTablesJsonResult(unPlannedUsers.Count, request, unPlannedUsers.Select((r, index) => new
        //        {
        //            rowIndex = (index + 1) + (request.Start),
        //            ColorCode="white",
        //            MemberName = $"<strong>{r.EmployeeName}</strong><br/><small><i class='glyphicon glyphicon-envelope'></i> {r.EmployeeEmail}</small><br/><i class='glyphicon glyphicon-phone'></i> {r.MobileNumber}<br/>Reporting To: <strong>{r.TLName}</strong>"
        //            + $"{(!string.IsNullOrEmpty(r.LeaveActivity) ? "<br/><span class='label label-primary' style='font-size:12px;margin-top: 2px;display: inline-block;'>" + r.LeaveActivity + "</span>" : string.Empty)}",
        //            ProjectHtml = "NA",
        //            TotalPlanHours = "NA",
        //            TotalActualHours = "NA"
        //        })
        //        );                
        //    }
        //    if (users != null)
        //    {
        //        //call to fill leave data
        //        FillUserLeaveActivity(users, model.DateFrom.ToDateTime().Value, model.DateTo.ToDateTime().Value);
        //    }

        //    var query = from res in response
        //                join user in users
        //                on res.MemberEmail.ToLower().Trim() equals user.EmployeeEmail.ToLower().Trim() into gj
        //                from subUser in gj.DefaultIfEmpty()
        //                select new
        //                {
        //                    res.MemberEmail,
        //                    res.MemberName,
        //                    res.PlanHour,
        //                    res.PlanHourEMS,
        //                    res.ActualHours,
        //                    res.ActualHoursEMS,
        //                    res.TaskName,
        //                    res.ProjectName,
        //                    res.CRMID,
        //                    subUser?.TLEmail,
        //                    subUser?.TLName,
        //                    subUser?.MobileNumber,
        //                    subUser?.LeaveActivity
        //                };
        //    var joinedResult = query.ToList();

        //    summaryList = joinedResult.GroupBy(x => x.MemberEmail).Select((a, index) => new WorkingHoursSummaryObject
        //    {
        //        rowIndex = (index + 1) + (request.Start),
        //        MemberEmail = a.Key,
        //        MemberName = $"<strong>{a.First().MemberName}</strong><br/>" +
        //        $"<small><i class='glyphicon glyphicon-envelope'></i> {a.First().MemberEmail}</small>" +
        //        $"<br/><i class='glyphicon glyphicon-phone'></i> {a.First().MobileNumber}" +
        //        $"<br/>Reporting To: <strong>{a.First().TLName}</strong>" +
        //        $"{(!string.IsNullOrEmpty(a.First().LeaveActivity) ? "<br/><span class='label label-primary' style='font-size:12px;margin-top: 2px;display: inline-block;'>" + a.First().LeaveActivity + "</span>" : string.Empty)}",
        //        TotalActualHours = a.Sum(b => b.ActualHours),
        //        TotalPlanHours = a.Sum(b => b.PlanHour),
        //        ColorCode =  "white",
        //        ProjectHtml = ""
        //    }).ToList();

        //    //summaryList.ForEach(a => { a.TotalPlanHours = 16.30; a.TotalActualHours = 20; });


        //    double AboveAllowedPer = 20;
        //    int days = model.DateTo.ToDateTime().Value.Date.Subtract(model.DateFrom.ToDateTime().Value.Date).Days + 1;

        //    double minimumHours = days * 8.0;
        //    if (model.WorkingHourTypeId.Equals(Enums.WorkingHourSummaryType.LessThenEight))
        //    {
        //        summaryList = summaryList.Where(x => x.TotalPlanHours < minimumHours).ToList();
        //    }
        //    else if (model.WorkingHourTypeId.Equals(Enums.WorkingHourSummaryType.PlansEqualsActual))
        //    {
        //        summaryList = summaryList.Where(x => x.TotalPlanHours.Equals(x.TotalActualHours) && x.TotalPlanHours >= minimumHours).ToList();
        //    }
        //    else if (model.WorkingHourTypeId.Equals(Enums.WorkingHourSummaryType.MoreThenTwentyPer))
        //    {
        //        summaryList = summaryList.Where(x => x.TotalActualHours > (x.TotalPlanHours + (AboveAllowedPer / 100 * x.TotalPlanHours)) && x.TotalPlanHours >= minimumHours).ToList();
        //    }
        //    else if (model.WorkingHourTypeId.Equals(Enums.WorkingHourSummaryType.NoActualHours))
        //    {
        //        summaryList = summaryList.Where(x => x.TotalActualHours == 0).ToList();
        //    }
        //    foreach (var item in summaryList)
        //    {
        //        item.ColorCode = item.TotalPlanHours < minimumHours ? "red" : item.TotalPlanHours == item.TotalActualHours ? "orange" : "white";
        //        if (item.TotalPlanHours > 0 && item.TotalActualHours > (item.TotalPlanHours + (AboveAllowedPer / 100 * item.TotalPlanHours)))
        //            item.ColorCode = "yellow";

        //        //item.ProjectHtml = "<table class='table table-stats table-condensed' style='margin:0px;border:1px solid black;;'>";
        //        item.ProjectHtml = "<table class='table' style='margin:0px;color:black;'>";
        //        var userProjects = response.Where(x => x.MemberEmail == item.MemberEmail).ToList();
        //        foreach (var prj in userProjects.GroupBy(a => a.ProjectName).Select(a => a.Key).ToList())
        //        {
        //            double planned = userProjects.Where(a => a.ProjectName == prj).Sum(a => a.PlanHour);
        //            double actual = userProjects.Where(a => a.ProjectName == prj).Sum(a => a.ActualHours);
        //            string rowBackColor = item.ColorCode;
        //            int prjid = userProjects.Where(a => a.ProjectName == prj).First().CRMID ?? 0;

        //            item.ProjectHtml += $"<tr style='background:{rowBackColor}'><td style='width:50%;text-align:left;'>{userProjects.Where(a => a.ProjectName == prj).First().ProjectName} [{prjid}]</td>";
        //            item.ProjectHtml += $"<td style='width:24%;text-align:right;'>{this.GetFormattedTime(planned)}</td>";
        //            item.ProjectHtml += $"<td style='text-align:right;'>{this.GetFormattedTime(actual)}</td></tr>";
        //        }
        //        item.ProjectHtml += "</table>";
        //    }
        //    IDictionary<string, object> additionalParameters = new Dictionary<string, object>();
        //    var plannedHours = summaryList.Sum(r => r.TotalPlanHours);
        //    var actualHours = summaryList.Sum(r => r.TotalActualHours);
        //    var TotalPlannedHoursFormatted = this.GetFormattedTime(plannedHours);
        //    var TotalActualHoursFormatted = this.GetFormattedTime(actualHours);

        //    additionalParameters.Add(new KeyValuePair<string, object>("TotalPlannedHours", plannedHours));
        //    additionalParameters.Add(new KeyValuePair<string, object>("TotalActualHours", actualHours));
        //    additionalParameters.Add(new KeyValuePair<string, object>("TotalPlannedHoursFormatted", TotalPlannedHoursFormatted));
        //    additionalParameters.Add(new KeyValuePair<string, object>("TotalActualHoursFormatted", TotalActualHoursFormatted));
        //    return DataTablesJsonResult(summaryList.Count, request, summaryList.Select((r, index) => new
        //    {
        //        days=days,
        //        rowIndex= (index + 1),
        //        r.ColorCode,
        //        r.MemberName,
        //        r.ProjectHtml,
        //        TotalPlanHours = this.GetFormattedTime(r.TotalPlanHours),
        //        TotalActualHours= this.GetFormattedTime(r.TotalActualHours),
        //    }), additionalParameters);
        //}

        private string GetFormattedTime(double time)
        {
            int hours = 0, minutes = 0;
            time = time * 60;
            hours = (int)time / 60;
            minutes = (int)time % 60;
            if (hours == 0 && minutes == 0)
                return "-";
            return $"{hours} H {minutes} M";
        }


        [HttpGet]
        public IActionResult DownloadEmployeeWorkingHoursSummary()
        {
            if (!IfAshishTeamPMUId)
            {
                return AccessDenied();
            }

            var model = TempData.Get<EmployeeWorkingHoursSummaryDto>("EmployeeWorkingHoursSummaryFilter");
            if (model != null)
                TempData.Put("EmployeeWorkingHoursSummaryFilter", model);

            int deptId, uid;
            List<string> emails = new List<string>();
            if (model.WorkingHourTypeId == Enums.WorkingHourSummaryType.Department)
            {
                deptId = !string.IsNullOrWhiteSpace(model.DepartmentId) ? Convert.ToInt32(model.DepartmentId) : 0;
                if (deptId > 0)
                {
                    emails = userLoginService.GetUserEmailIdsByDepartment(deptId);
                }
            }
            else if (model.WorkingHourTypeId == Enums.WorkingHourSummaryType.Employee)
            {
                uid = !string.IsNullOrWhiteSpace(model.Uid) ? Convert.ToInt32(model.Uid) : 0;
                var user = userLoginService.GetUserInfoByID(uid);
                if (user != null && !string.IsNullOrWhiteSpace(user.EmailOffice))
                {
                    emails.Add(user.EmailOffice);
                }

            }
            //else Get All Data

            List<WorkingHoursSummaryObject> summaryList;
            UserLogin objUser = userLoginService.GetUserInfoByID(PMUserId);
            var users = userLoginService.GetUserTLTogether(CurrentUser.PMUid);
            // excluding PM from users list
            if (users != null)
                users = users.Where(a => a.Uid != CurrentUser.PMUid).ToList();

            RequestObject reqObj = new RequestObject
            {
                FromDate = model.DateFrom.ToDateTime().Value.ToString("dd-MMM-yyyy"),
                ToDate = model.DateTo.ToDateTime().Value.ToString("dd-MMM-yyyy"),
                emailid = objUser.EmailOffice //m25@mailinator.com
            };
            ResponseRoot apiResult = GetDataFromAPI(reqObj);

            List<ResponseObject> response = null;
            if (emails != null && emails.Count > 0) // department or user specific data
            {
                response = apiResult != null && apiResult.ResponsePacket != null
                    ? apiResult.ResponsePacket.Where(r => emails.Contains(r.MemberEmail))
                    .OrderBy(r => r.MemberEmail).ThenBy(r => r.ProjectName).ToList() : new List<ResponseObject>();
            }
            else //All data
            {
                response = apiResult != null && apiResult.ResponsePacket != null
                    ? apiResult.ResponsePacket.OrderBy(r => r.MemberEmail).ThenBy(r => r.ProjectName).ToList() : new List<ResponseObject>();
            }

            // getting projectmanager email
            var pmEmailId = userLoginService.GetPmEmailId(CurrentUser.PMUid).ToLower();
            response = response.Where(a => !a.MemberEmail.Equals(pmEmailId)).ToList();


            DataTable table = new DataTable("EmployeeWorkingHoursSummary");
            table.Columns.Add("Sr.No");
            table.Columns.Add("Name");
            table.Columns.Add("MobileNo");
            table.Columns.Add("Email");
            table.Columns.Add("Department");
            table.Columns.Add("ReportingTo");
            table.Columns.Add("Planned");
            table.Columns.Add("ActualHours");
            table.Columns.Add("Variance");
            table.Columns.Add("LeaveStatus");

            if (model.WorkingHourTypeId == Enums.WorkingHourSummaryType.NoPlannedHours)
            {
                var responseUsers = response.Where(r => !string.IsNullOrWhiteSpace(r.MemberEmail)).Select(r => r.MemberEmail.Trim().ToLower()).Distinct().ToList();
                var unPlannedUsers = users.Where(u => !responseUsers.Contains(u.EmployeeEmail.ToLower()))
                    .OrderBy(u => u.EmployeeName).ThenBy(u => u.EmployeeEmail).ToList();
                FillUserLeaveActivity(unPlannedUsers, model.DateFrom.ToDateTime().Value, model.DateTo.ToDateTime().Value);

                int i = 1;
                foreach (var item in unPlannedUsers)
                {
                    DataRow dr = table.NewRow();
                    dr["Sr.No"] = i.ToString();
                    dr["Name"] = item.EmployeeName;
                    dr["MobileNo"] = string.Empty;
                    dr["Email"] = item.EmployeeEmail;
                    dr["Department"] = item.DepartmentName;
                    dr["ReportingTo"] = item.TLName;
                    dr["Planned"] = "";
                    dr["ActualHours"] = "";
                    dr["Variance"] = "";
                    dr["LeaveStatus"] = !string.IsNullOrEmpty(item.LeaveActivity) ? item.LeaveActivity : string.Empty;
                    table.Rows.Add(dr);
                    i++;
                }
            }
            else
            {
                //call to fill leave data
                FillUserLeaveActivity(users, model.DateFrom.ToDateTime().Value, model.DateTo.ToDateTime().Value);

                var query = from res in response
                            join user in users
                            on res.MemberEmail.ToLower().Trim() equals user.EmployeeEmail.ToLower().Trim() into gj
                            from subUser in gj.DefaultIfEmpty()
                            select new
                            {
                                res.MemberEmail,
                                res.MemberName,
                                res.PlanHour,
                                res.PlanHourEMS,
                                res.ActualHours,
                                res.ActualHoursEMS,
                                res.TaskName,
                                res.ProjectName,
                                res.CRMID,
                                subUser?.DepartmentName,
                                subUser?.TLEmail,
                                subUser?.TLName,
                                string.Empty,
                                subUser?.LeaveActivity
                            };
                var joinedResult = query.ToList();

                summaryList = joinedResult.GroupBy(x => x.MemberEmail).Select((a, index) => new WorkingHoursSummaryObject
                {
                    rowIndex = (index + 1),
                    MemberEmail = a.Key,
                    MemberName = a.First().MemberName,
                    MobileNumber = string.Empty,
                    DepartmentName = a.First().DepartmentName,
                    TLName = a.First().TLName,
                    TotalActualHours = a.Sum(b => b.ActualHours),
                    TotalPlanHours = a.Sum(b => b.PlanHour),
                    ColorCode = "",
                    ProjectHtml = "",
                    LeaveStatus = !string.IsNullOrEmpty(a.First().LeaveActivity) ? a.First().LeaveActivity : string.Empty
                }).ToList();

                //summaryList.ForEach(a => { a.TotalPlanHours = a.TotalPlanHours + 5; a.TotalActualHours = a.TotalActualHours+10; });

                double AboveAllowedPer = 20;
                int days = model.DateTo.ToDateTime().Value.Date.Subtract(model.DateFrom.ToDateTime().Value.Date).Days + 1;

                double minimumHours = days * 8.0;
                if (model.WorkingHourTypeId.Equals(Enums.WorkingHourSummaryType.LessThenEight))
                {
                    summaryList = summaryList.Where(x => x.TotalPlanHours < minimumHours).ToList();
                }
                else if (model.WorkingHourTypeId.Equals(Enums.WorkingHourSummaryType.PlansEqualsActual))
                {
                    summaryList = summaryList.Where(x => x.TotalPlanHours.Equals(x.TotalActualHours) && x.TotalPlanHours >= minimumHours).ToList();
                }
                else if (model.WorkingHourTypeId.Equals(Enums.WorkingHourSummaryType.MoreThenTwentyPer))
                {
                    summaryList = summaryList.Where(x => x.TotalActualHours > (x.TotalPlanHours + (AboveAllowedPer / 100 * x.TotalPlanHours)) && x.TotalPlanHours >= minimumHours).ToList();
                }
                else if (model.WorkingHourTypeId.Equals(Enums.WorkingHourSummaryType.NoActualHours))
                {
                    summaryList = summaryList.Where(x => x.TotalActualHours == 0).ToList();
                }

                var plannedHours = summaryList.Sum(r => r.TotalPlanHours);
                var actualHours = summaryList.Sum(r => r.TotalActualHours);

                int i = 1;
                foreach (var item in summaryList)
                {
                    DataRow dr = table.NewRow();
                    dr["Sr.No"] = i.ToString();
                    dr["Name"] = item.MemberName;
                    dr["MobileNo"] = "NA";
                    dr["Email"] = item.MemberEmail;
                    dr["Department"] = item.DepartmentName;
                    dr["ReportingTo"] = item.TLName;
                    dr["Planned"] = GetFormattedTime(item.TotalPlanHours);
                    dr["ActualHours"] = GetFormattedTime(item.TotalActualHours);
                    dr["Variance"] = GetFormattedTime(item.TotalPlanHours - item.TotalActualHours);
                    dr["LeaveStatus"] = !string.IsNullOrWhiteSpace(item.LeaveStatus) ? item.LeaveStatus : string.Empty;
                    table.Rows.Add(dr);
                    i++;
                }

                DataRow dr1 = table.NewRow();
                dr1["Sr.No"] = "";
                dr1["Name"] = "";
                dr1["MobileNo"] = "";
                dr1["Email"] = "";
                dr1["Department"] = "";
                dr1["ReportingTo"] = "Total";
                dr1["Planned"] = GetFormattedTime(plannedHours);
                dr1["ActualHours"] = GetFormattedTime(actualHours);
                dr1["Variance"] = GetFormattedTime(plannedHours - actualHours);
                dr1["LeaveStatus"] = "";
                table.Rows.Add(dr1);

            }

            if (table.Rows.Count < 1)
            {
                DataRow dr = table.NewRow();
                dr["Sr.No"] = "";
                dr["Name"] = "";
                dr["MobileNo"] = "";
                dr["Email"] = "";
                dr["Department"] = "";
                dr["ReportingTo"] = "";
                dr["Planned"] = "";
                dr["ActualHours"] = "";
                dr["Variance"] = "";
                dr["LeaveStatus"] = "";
                table.Rows.Add(dr);
            }

            string filename = "EmployeeWorkingHoursSummary_" + DateTime.Now.Ticks.ToString() + ".xlsx";
            string[] columns = { "Sr.No", "Name", "MobileNo", "Email", "Department", "ReportingTo", "Planned", "ActualHours", "Variance", "LeaveStatus" };
            byte[] filecontent = ExportExcelHelper.ExportExcel(table, "Employee Working Hours Summary Report ", false, columns);
            string fileName = filename;
            return File(filecontent, ExportExcelHelper.ExcelContentType, fileName);

        }

        private void FillUserLeaveActivity(List<TLEmployeeDto> users, DateTime dateFrom, DateTime dateTo)
        {
            foreach (var user in users)
            {
                var leave = leaveService.GetFirstLeaveActivityForDate(dateFrom, dateTo, user.Uid);
                if (leave != null)
                {
                    if (leave.Status == (int)Enums.LeaveStatus.Approved)
                    {
                        if (leave.IsHalf.HasValue && leave.IsHalf.Value)
                        {
                            user.LeaveActivity = "Leave-[Half]";
                        }
                        else
                        {
                            user.LeaveActivity = "Leave";
                        }
                    }
                    else if (leave.Status == (int)Enums.LeaveStatus.UnApproved)
                    {
                        user.LeaveActivity = "Leave-[UnApproved]";
                    }
                    else
                    {
                        user.LeaveActivity = "Leave-Pending";
                    }
                }
            }
        }

        public ActionResult ExportEmpWorkingReportExcel(EmployeeWorkingHoursDto model)
        {
            if (!IfAshishTeamPMUId)
            {
                return AccessDenied();
            }
            var users = userLoginService.GetUserTLTogether(PMUserId);
            string TotalPlannedHoursFormatted = string.Empty, TotalActualHoursFormatted = string.Empty;
            double plannedHours = 0, actualHours = 0;
            if (model.WorkingHourTypeId == Enums.WorkingHourType.NoPlannedHours && users != null)
            {
                users = users.Where(u => u.Uid != PMUserId).ToList();
                FillUserLeaveActivity(users, model.DateFrom.ToDateTime().Value, model.DateFrom.ToDateTime().Value);
            }

            int deptId, uid;
            List<string> emails = new List<string>();
            if (model.WorkingHourTypeId == 0 || model.WorkingHourTypeId == Enums.WorkingHourType.NoPlannedHours)
            {
                // Get all data
            }
            else if (model.WorkingHourTypeId == Enums.WorkingHourType.Department)
            {
                deptId = !string.IsNullOrWhiteSpace(model.DepartmentId) ? Convert.ToInt32(model.DepartmentId) : 0;
                if (deptId > 0)
                {
                    emails = userLoginService.GetUserEmailIdsByDepartment(deptId);
                }
            }
            else
            {
                uid = !string.IsNullOrWhiteSpace(model.Uid) ? Convert.ToInt32(model.Uid) : 0;
                var user = userLoginService.GetUserInfoByID(uid);
                if (user != null && !string.IsNullOrWhiteSpace(user.EmailOffice))
                {
                    emails.Add(user.EmailOffice.Trim().ToLower());
                }
            }
            UserLogin objUser = userLoginService.GetUserInfoByID(PMUserId);

            RequestObject reqObj = new RequestObject
            {
                FromDate = model.DateFrom.ToDateTime().Value.ToString("dd-MMM-yyyy"),
                ToDate = model.DateTo.ToDateTime().Value.ToString("dd-MMM-yyyy"),
                emailid = objUser.EmailOffice
            };
            ResponseRoot apiResult = GetDataFromAPI(reqObj);

            List<ResponseObject> response = null;
            if (emails != null && emails.Count > 0) // department or user specific data
            {
                response = apiResult != null && apiResult.ResponsePacket != null
                    ? apiResult.ResponsePacket.Where(r => r.MemberEmail != null && emails.Contains(r.MemberEmail.Trim().ToLower()))
                    .OrderBy(r => r.MemberEmail).ThenBy(r => r.ProjectName).ToList() : new List<ResponseObject>();
            }
            else //All data
            {
                response = apiResult != null && apiResult.ResponsePacket != null
                    ? apiResult.ResponsePacket.OrderBy(r => r.MemberEmail).ThenBy(r => r.ProjectName).ToList() : new List<ResponseObject>();
            }

            List<EmployeeWorkingHourReportDto> report = null;
            if (model.WorkingHourTypeId == Enums.WorkingHourType.NoPlannedHours)
            {
                var responseUsers = response.Where(r => !string.IsNullOrWhiteSpace(r.MemberEmail)).Select(r => r.MemberEmail.Trim().ToLower()).Distinct().ToList();
                var unPlannedUsers = users.Where(u => !responseUsers.Contains(u.EmployeeEmail))
                    .OrderBy(u => u.EmployeeName).ThenBy(u => u.EmployeeEmail).ToList();

                report = unPlannedUsers.Select(r => new EmployeeWorkingHourReportDto
                {
                    EmployeeName = r.EmployeeName,
                    EmployeeEmail = r.EmployeeEmail,
                    MobileNo = "NA",
                    ReportingTo = r.TLName,
                    ProjectName = r.LeaveActivity ?? "N/A",
                    TaskName = "N/A",
                    PlanHour = "N/A",
                    ActualHours = "N/A"
                }).ToList();
            }
            else
            {
                plannedHours = response.Sum(r => r.PlanHour);
                actualHours = response.Sum(r => r.ActualHours);

                List<string> timesPlanned = response.Select(r => r.PlanHourEMS).ToList();
                List<string> timesActual = response.Select(r => r.ActualHoursEMS).ToList();

                TotalPlannedHoursFormatted = CalculateTotalHours(timesPlanned);

                TotalActualHoursFormatted = CalculateTotalHours(timesActual);

                foreach (var res in response)
                {
                    res.PlanHourEMS = res.PlanHourEMS.Replace("00", "0") + " M";
                    res.PlanHourEMS = res.PlanHourEMS.Replace("00:", "0:").Replace(":", " H ");
                    res.ActualHoursEMS = res.ActualHoursEMS.Replace("00", "0") + " M";
                    res.ActualHoursEMS = res.ActualHoursEMS.Replace("00:", "0:").Replace(":", " H ");
                }

                var query = from res in response
                            join user in users
                            on res.MemberEmail equals user.EmployeeEmail into gj
                            from subUser in gj.DefaultIfEmpty()
                            select new
                            {
                                res.MemberEmail,
                                res.MemberName,
                                res.PlanHour,
                                res.PlanHourEMS,
                                res.ActualHours,
                                res.ActualHoursEMS,
                                res.TaskName,
                                res.ProjectName,
                                res.CRMID,
                                subUser?.TLEmail,
                                subUser?.TLName,
                                subUser?.MobileNumber
                            };
                var joinedResult = query.ToList();

                report = joinedResult.Select((r, index) => new EmployeeWorkingHourReportDto
                {
                    EmployeeName = r.MemberName,
                    EmployeeEmail = r.MemberEmail,
                    MobileNo = string.Empty,
                    ReportingTo = r.TLName,
                    ProjectName = $"{r.ProjectName} [{r.CRMID}]",
                    TaskName = r.TaskName,
                    PlanHour = r.PlanHourEMS,
                    ActualHours = r.ActualHoursEMS
                }).ToList();
            }
            string Reportname = $"EmployeeWorkingHourReport";
            Int32 subsheet = 0;
            List<ExportExcelColumn> excelColumn = new List<ExportExcelColumn>();
            ExportExcelColumn column1 = new ExportExcelColumn { ColumnName = "Name", PropertyName = "EmployeeName", ColumnWidth = 3500 };
            ExportExcelColumn column2 = new ExportExcelColumn { ColumnName = "Email", PropertyName = "EmployeeEmail", ColumnWidth = 3500 };
            ExportExcelColumn column3 = new ExportExcelColumn { ColumnName = "Mobile No.", PropertyName = "MobileNo", ColumnWidth = 3500 };
            ExportExcelColumn column4 = new ExportExcelColumn { ColumnName = "Reporting To", PropertyName = "ReportingTo", ColumnWidth = 3500 };
            ExportExcelColumn column5 = new ExportExcelColumn { ColumnName = "Project Name", PropertyName = "ProjectName", ColumnWidth = 5000 };
            ExportExcelColumn column6 = new ExportExcelColumn { ColumnName = "Task Name", PropertyName = "TaskName", ColumnWidth = 5000 };
            ExportExcelColumn column7 = new ExportExcelColumn { ColumnName = "Planned Hours", PropertyName = "PlanHour", ColumnWidth = 6000 };
            ExportExcelColumn column8 = new ExportExcelColumn { ColumnName = "Actual Hours", PropertyName = "ActualHours", ColumnWidth = 5000 };
            excelColumn.Add(column1);
            excelColumn.Add(column2);
            excelColumn.Add(column3);
            excelColumn.Add(column4);
            excelColumn.Add(column5);
            excelColumn.Add(column6);
            excelColumn.Add(column7);
            excelColumn.Add(column8);
            if (report != null && report.Count > 0 && plannedHours > 0 && actualHours > 0)
            {
                report.Add(new EmployeeWorkingHourReportDto
                {
                    PlanHour = plannedHours > 0 ? $"Total Planned Hours : {TotalPlannedHoursFormatted}" : "",
                    ActualHours = actualHours > 0 ? $"Total Actual Hours : {TotalActualHoursFormatted}" : ""
                });
            }
            return DownloadExcelFile(report, Reportname, subsheet, excelColumn);
        }

        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult EmployeeWorkingHoursData2()
        {
            if (!IfAshishTeamPMUId)
            {
                return AccessDenied();
            }
            else
            {
                EmployeeWorkingHoursDto workingHourDto = new EmployeeWorkingHoursDto();
                workingHourDto.WorkingHourTypes = WebExtensions.GetSelectList<Enums.WorkingHourType>();
                workingHourDto.Employees = userLoginService.GetSelfAndUsersListByAllRole(Convert.ToInt32(CurrentUser.Uid), Convert.ToInt32(CurrentUser.RoleId), Convert.ToInt32(CurrentUser.DesignationId)).Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.Uid.ToString() }).ToList();
                workingHourDto.Departments = departmentService.GetActiveDepartments().ToSelectList(x => x.Name, x => x.DeptId);
                workingHourDto.DateFrom = DateTime.Today.ToFormatDateString("dd/MM/yyyy");
                //workingHourDto.DateTo = DateTime.Today.ToFormatDateString("dd/MM/yyyy");
                return View("Index2", workingHourDto);
            }


        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult GetEmployeeWorkingHoursData2(IDataTablesRequest request, EmployeeWorkingHoursDto model)
        {

            if (!IfAshishTeamPMUId)
            {
                return AccessDenied();
            }
            var users = userLoginService.GetUserTLTogether(PMUserId);
            if (model.WorkingHourTypeId == Enums.WorkingHourType.NoPlannedHours && users != null)
            {
                users = users.Where(u => u.Uid != PMUserId).ToList();
                FillUserLeaveActivity(users, model.DateFrom.ToDateTime().Value, model.DateFrom.ToDateTime().Value);
            }

            int deptId, uid;
            List<string> emails = new List<string>();
            if (model.WorkingHourTypeId == 0 || model.WorkingHourTypeId == Enums.WorkingHourType.NoPlannedHours)
            {
                // Get all data
            }
            else if (model.WorkingHourTypeId == Enums.WorkingHourType.Department)
            {
                deptId = !string.IsNullOrWhiteSpace(model.DepartmentId) ? Convert.ToInt32(model.DepartmentId) : 0;
                if (deptId > 0)
                {
                    emails = userLoginService.GetUserEmailIdsByDepartment(deptId);
                }
            }
            else
            {
                uid = !string.IsNullOrWhiteSpace(model.Uid) ? Convert.ToInt32(model.Uid) : 0;
                var user = userLoginService.GetUserInfoByID(uid);
                if (user != null && !string.IsNullOrWhiteSpace(user.EmailOffice))
                {
                    emails.Add(user.EmailOffice.Trim().ToLower());
                }

            }
            UserLogin objUser = userLoginService.GetUserInfoByID(PMUserId);

            RequestObject reqObj = new RequestObject
            {
                FromDate = model.DateFrom.ToDateTime().Value.ToString("dd-MMM-yyyy"),
                ToDate = model.DateTo.ToDateTime().Value.ToString("dd-MMM-yyyy"),
                emailid = objUser.EmailOffice
            };
            ResponseRoot apiResult = GetDataFromAPI(reqObj);

            List<ResponseObject> response = null;
            if (emails != null && emails.Count > 0) // department or user specific data
            {
                response = apiResult != null && apiResult.ResponsePacket != null
                    ? apiResult.ResponsePacket.Where(r => r.MemberEmail != null && emails.Contains(r.MemberEmail.Trim().ToLower()))
                    .OrderBy(r => r.MemberEmail).ThenBy(r => r.ProjectName).ToList() : new List<ResponseObject>();
            }
            else //All data
            {
                response = apiResult != null && apiResult.ResponsePacket != null
                    ? apiResult.ResponsePacket.OrderBy(r => r.MemberEmail).ThenBy(r => r.ProjectName).ToList() : new List<ResponseObject>();
            }

            List<EmployeeWorkingHourReportDto> summaryList;
            if (model.WorkingHourTypeId == Enums.WorkingHourType.NoPlannedHours)
            {
                var responseUsers = response.Where(r => !string.IsNullOrWhiteSpace(r.MemberEmail)).Select(r => r.MemberEmail.Trim().ToLower()).Distinct().ToList();
                var unPlannedUsers = users.Where(u => !responseUsers.Contains(u.EmployeeEmail))
                    .OrderBy(u => u.EmployeeName).ThenBy(u => u.EmployeeEmail).ToList();
                return DataTablesJsonResult(unPlannedUsers.Count, request, unPlannedUsers.Select((r, index) => new
                {
                    rowIndex = (index + 1) + (request.Start),
                    ProjectName = r.LeaveActivity != null ? r.LeaveActivity : "N/A",
                    MemberName = $"<strong>{r.EmployeeName}</strong><br/><small><i class='glyphicon glyphicon-envelope'></i> {r.EmployeeEmail}</small>" +
                    //$"<br/><i class='glyphicon glyphicon-phone'></i> {r.MobileNumber}" +
                    $"<br/>Reporting To: <strong>{r.TLName}</strong>",
                    HTML = r.LeaveActivity != null ? r.LeaveActivity : "N/A",
                    //TaskName = "N/A",
                    //PlanHour = "N/A",
                    //ActualHours = "N/A",
                })
                );
            }
            else
            {
                IDictionary<string, object> additionalParameters = new Dictionary<string, object>();
                var plannedHours = response.Sum(r => r.PlanHour);
                var actualHours = response.Sum(r => r.ActualHours);

                List<string> timesPlanned = response.Select(r => r.PlanHourEMS).ToList();
                List<string> timesActual = response.Select(r => r.ActualHoursEMS).ToList();

                string TotalPlannedHoursFormatted = CalculateTotalHours(timesPlanned);

                string TotalActualHoursFormatted = CalculateTotalHours(timesActual);

                foreach (var res in response)
                {
                    res.PlanHourEMS = res.PlanHourEMS.Replace(":", " H ").Replace("00", "0") + " M";
                    res.ActualHoursEMS = res.ActualHoursEMS.Replace(":", " H ").Replace("00", "0") + " M";
                }

                var query = from res in response
                            join user in users
                            on res.MemberEmail equals user.EmployeeEmail into gj
                            from subUser in gj.DefaultIfEmpty()
                            select new
                            {
                                res.MemberEmail,
                                res.MemberName,
                                res.PlanHour,
                                res.PlanHourEMS,
                                res.ActualHours,
                                res.ActualHoursEMS,
                                res.TaskName,
                                res.ProjectName,
                                res.CRMID,
                                subUser?.TLEmail,
                                subUser?.TLName,
                                subUser ?.MobileNumber
                            };
                var joinedResult = query.ToList();

                additionalParameters.Add(new KeyValuePair<string, object>("TotalPlannedHours", plannedHours));
                additionalParameters.Add(new KeyValuePair<string, object>("TotalActualHours", actualHours));
                additionalParameters.Add(new KeyValuePair<string, object>("TotalPlannedHoursFormatted", TotalPlannedHoursFormatted));
                additionalParameters.Add(new KeyValuePair<string, object>("TotalActualHoursFormatted", TotalActualHoursFormatted));

                summaryList = joinedResult.GroupBy(x => new { x.MemberEmail, x.MemberName })
                    .OrderBy(x => x.Key.MemberName).ThenBy(x => x.Key.MemberEmail).Select((a, index) => new EmployeeWorkingHourReportDto
                    {
                        EmployeeEmail = a.Key.MemberEmail,
                        EmployeeName = $"<strong>{a.First().MemberName}</strong><br/><small><i class='glyphicon glyphicon-envelope'></i> {a.First().MemberEmail}</small>" +
                        //$"<br/><i class='glyphicon glyphicon-phone'></i> {a.First().MobileNumber}" +
                        $"<br/>Reporting To: <strong>{a.First().TLName}</strong>",
                        HTML = ""
                    }).ToList();

                foreach (var item in summaryList)
                {
                    item.HTML = "<table class='table' style='margin:0px;color:black;'>";
                    var taskRecords = response.Where(x => x.MemberEmail == item.EmployeeEmail).ToList();

                    foreach (var record in taskRecords)
                    {
                        item.HTML += $"<tr'><td style='width:30%;text-align:left;'>{record.ProjectName} [{record.CRMID}]</td>";
                        item.HTML += $"<td style='width:39%;text-align:left;'>{record.TaskName}</td>";
                        item.HTML += $"<td style='text-align:right;width:15%;'>{record.PlanHourEMS}</td>";
                        item.HTML += $"<td style='text-align:right;width:15%;'>{record.ActualHoursEMS}</td></tr>";
                    }
                    item.HTML += "</table>";
                }

                return DataTablesJsonResult(summaryList.Count, request, summaryList.Select((r, index) => new
                {
                    rowIndex = (index + 1) + (request.Start),
                    //CrmId = r.CRMID,
                    //ProjectName = $"{r.ProjectName} [{r.CRMID}]",
                    //r.MemberEmail,
                    MemberName = r.EmployeeName,
                    r.TaskName,
                    // TLName=$"{r.TLName}<br/><small>{r.TLEmail}</small>",               
                    //PlanHour = r.PlanHourEMS,
                    //ActualHours = r.ActualHoursEMS,
                    r.HTML
                }), additionalParameters);
            }

        }

        #region ProjectActualHourReport

        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult GetProjectActualHourReport()
        {
            ProjectActualHoursDto projectActualHoursDto = new ProjectActualHoursDto();
            projectActualHoursDto.Projects = GetProjects();
            projectActualHoursDto.DateFrom = ""; // DateTime.Today.AddMonths(-1).ToFormatDateString("dd/MM/yyyy");
            projectActualHoursDto.DateTo = ""; //DateTime.Today.ToFormatDateString("dd/MM/yyyy");
            var StatusList = from Enums.ProjectStatus p in Enum.GetValues(typeof(Enums.ProjectStatus)) select new { Name = p.ToString(), Id = (char)p };
            projectActualHoursDto.ProjectStatusList = StatusList.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString(),
            }).ToList();
            projectActualHoursDto.ProjectStatus = "R";
            return View("ProjectActualHour", projectActualHoursDto);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult GetProjectActualHourReport(IDataTablesRequest request, ProjectActualHoursDto filter)
        {
            //var pagingServicesForProject = new PagingService<Project>(0, int.MaxValue);
            var pagingServicesForProject = new PagingService<Project>(request?.Start ?? 0, request?.Length ?? int.MaxValue);

            //var filterExpr = PredicateBuilder.True<Project>().And(x => x.Status == "R");
            var filterExpr = PredicateBuilder.True<Project>();
            if (filter.ProjectStatus.HasValue() && filter.ProjectStatus != "" && filter.ProjectStatus != "0")
            {
                filterExpr = filterExpr.And(e => e.Status == filter.ProjectStatus);
            }
            if (filter.ProjectId.HasValue() && filter.ProjectId != "" && filter.ProjectId != "0")
            {
                filterExpr = filterExpr.And(e => e.ProjectId == Convert.ToInt32(filter.ProjectId));
            }
            pagingServicesForProject.Filter = filterExpr;

            pagingServicesForProject.Sort = (o) =>
            {
                return o.OrderBy(c => c.Name);
            };

            int totalProjectCount = 0;
            var responseProject = projectService.GetProjectsByPaging(out totalProjectCount, pagingServicesForProject);
            /////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////

            var pagingServicesForInvoice = GetPagingAndFiltersForInvoice(request, filter, responseProject);
            int totalInvoiceCount = 0;
            var responseInvoice = projectInvoiceService.GetInvoicesByPaging(out totalInvoiceCount, pagingServicesForInvoice);

            var pagingServicesForTimeSheet = GetPagingAndFiltersForTimesheet(request, filter, responseProject, responseInvoice);
            var timeSheet = timesheetService.GetTimeSheetsByFilter(pagingServicesForTimeSheet.Filter);


            Dictionary<string, string> dicProjectInvoice = new Dictionary<string, string>();
            Dictionary<string, DateTime> dicProjectInvoiceStartDate = new Dictionary<string, DateTime>();
            Dictionary<string, double> dicTimeSheet = new Dictionary<string, double>();
            foreach (var invoice in responseInvoice)
            {
                string PlanHour = "";
                if (dicProjectInvoice.TryGetValue(invoice.ProjectId.ToString(), out PlanHour))
                {
                    var intPlanHour = Convert.ToDouble(PlanHour) + CalculateHours(invoice.InvoiceStartDate, invoice.InvoiceEndDate, filter.DateFrom, filter.DateTo);
                    dicProjectInvoice[invoice.ProjectId.ToString()] = intPlanHour.ToString();
                }
                else
                {
                    dicProjectInvoice.Add(invoice.ProjectId.ToString(), CalculateHours(invoice.InvoiceStartDate, invoice.InvoiceEndDate, filter.DateFrom, filter.DateTo).ToString());
                    dicProjectInvoiceStartDate.Add(invoice.ProjectId.ToString(), invoice.InvoiceStartDate);
                }




                if (!dicTimeSheet.ContainsKey(invoice.ProjectId.ToString()))
                {
                    double dTimeSheetActualHour = timeSheet.Where(p => p.ProjectID == invoice.ProjectId)
                    .Where(d => d.AddDate >= invoice.InvoiceStartDate)
                    .OrderBy(o => o.AddDate).ToList().Sum(w => w.WorkHours.TotalHours);

                    dicTimeSheet.Add(invoice.ProjectId.ToString(), dTimeSheetActualHour);
                }

                //double dTimeSheetActualHour = timeSheet.Where(p => p.ProjectID == invoice.ProjectId)
                //    .Where(d => d.AddDate >= invoice.InvoiceStartDate && d.AddDate <= invoice.InvoiceEndDate)
                //    .OrderBy(o => o.AddDate).ToList().Sum(w => w.WorkHours.TotalHours);
                //double ActualHour = 0;
                //if (dicTimeSheet.TryGetValue(invoice.ProjectId.ToString(), out ActualHour))
                //{
                //    var intActualHour = ActualHour + dTimeSheetActualHour;
                //    dicTimeSheet[invoice.ProjectId.ToString()] = intActualHour;
                //}
                //else
                //{
                //    dicTimeSheet.Add(invoice.ProjectId.ToString(), dTimeSheetActualHour);
                //}
            }

            return DataTablesJsonResult(totalProjectCount, request, responseProject.Select((r, index) => new
            {
                ProjectName = r.Name,
                ProjectId = r.ProjectId,
                PlanHour = (dicProjectInvoice.ContainsKey(r.ProjectId.ToString()) ? dicProjectInvoice[r.ProjectId.ToString()] : "0"),
                //PlanHour = responseInvoice.Where(i=>i.ProjectId == r.ProjectId) dicProjectInvoice[r.ProjectId.ToString()],
                ActualHours = (dicTimeSheet.ContainsKey(r.ProjectId.ToString()) ? dicTimeSheet[r.ProjectId.ToString()] : 0).GetTimeHoursFormatString(),

                ActualHours1 = (timeSheet.Where(x => x.ProjectID == r.ProjectId).Sum(w => w.WorkHours.TotalHours)).GetTimeHoursFormatString(),
                //ActualHours = (dicTimeSheet.ContainsKey(r.ProjectId.ToString()) ? DoFormat(dicTimeSheet[r.ProjectId.ToString()]) : "0")
                //ActualHours1 = (dicProjectInvoice.ContainsKey(r.ProjectId.ToString()) ? DoFormat(timeSheet.Where(x => x.ProjectID == r.ProjectId)
                //.Where(d => d.AddDate >= dicProjectInvoiceStartDate[r.ProjectId.ToString()] &&
                // d.AddDate <= dicProjectInvoiceEndDate[r.ProjectId.ToString()]
                //).Sum(w => w.WorkHours.TotalHours)) : "0") 
            }).Select((ir, index) => new
            {
                ir.PlanHour,
                ir.ActualHours,
                ir.ProjectId,
                ir.ProjectName,
                Variance = (Convert.ToDouble(ir.ActualHours) - Convert.ToDouble(ir.PlanHour)).GetTimeHoursFormatString(),
                VarianceD = (Convert.ToDouble(ir.ActualHours) - Convert.ToDouble(ir.PlanHour)), // Variance Difference
                VarianceP = ((Convert.ToDouble(ir.PlanHour) == 0 || Convert.ToDouble(ir.ActualHours) == 0) ? "N/A" : ((Math.Abs(Convert.ToDouble(ir.ActualHours) - Convert.ToDouble(ir.PlanHour)) / Convert.ToDouble(ir.PlanHour)) * 100).GetTimeHoursFormatString())  // Variance Difference Percent (%)

            }).OrderBy(a => a.VarianceD).Select((ir, index) => new
            {
                rowIndex = (index + 1) + (request.Start),
                ir.PlanHour,
                ir.ActualHours,
                ir.ProjectId,
                ir.ProjectName,
                ir.Variance,
                ir.VarianceD,
                ir.VarianceP
            })
            .ToList());

            //return DataTablesJsonResult(totalProjectCount, request, response.Select((r, index) => new
            //{
            //    PlanHour = dicProjectInvoice[r.ProjectId.ToString()],
            //    ActualHours = DoFormat(timesheetService.GetTimeSheetByProjectId(r.ProjectId).
            //    Where(d=>d.AddDate >= (filter.DateFrom.HasValue()? filter.DateFrom.ToDateTime(): d.AddDate) && 
            //             d.AddDate <= (filter.DateTo.HasValue() ? filter.DateTo.ToDateTime() : d.AddDate)).
            //    Sum(w => w.WorkHours.TotalHours)),
            //    ProjectId = r.ProjectId,
            //    ProjectName = r.Project?.Name
            //}).Distinct().Select((ir, index) => new
            //{
            //    rowIndex = (index + 1) + (request.Start),
            //    ir.PlanHour,
            //    ir.ActualHours,
            //    ir.ProjectId,
            //    ir.ProjectName,
            //    Variance = DoFormat(Convert.ToDouble(ir.ActualHours) - Convert.ToDouble(ir.PlanHour)),
            //    VarianceD = (Convert.ToDouble(ir.ActualHours) - Convert.ToDouble(ir.PlanHour)), // Variance Difference
            //    VarianceP = DoFormat((Math.Abs(Convert.ToDouble(ir.ActualHours) - Convert.ToDouble(ir.PlanHour))/Convert.ToDouble(ir.PlanHour))*100)  // Variance Difference Percent (%)

            //}).OrderBy(a => a.VarianceD).ToList());
        }

        [HttpGet]
        public ActionResult ProjectActualHourDetails(int id)
        {
            int ProjectId = id;
            ProjectActualHoursDetailsDto actualHoursDetailsDto = new ProjectActualHoursDetailsDto();
            var project = projectService.GetProjectById(ProjectId);
            actualHoursDetailsDto.ProjectId = project.ProjectId;
            actualHoursDetailsDto.ProjectName = project.Name;
            actualHoursDetailsDto.invoiceDetails = new List<InvoiceDetails>();
            var pagingServices = new PagingService<ProjectInvoice>(0, int.MaxValue);
            var expr = PredicateBuilder.True<ProjectInvoice>();
            expr = expr.And(e => e.Project.ProjectId == Convert.ToInt32(ProjectId));
            pagingServices.Filter = expr;
            pagingServices.Sort = o => o.OrderBy(c => c.InvoiceStartDate);

            var projectInvoices = projectInvoiceService.GetInvoiceList(pagingServices);
            var timeSheet = timesheetService.GetTimeSheetByProjectId(ProjectId).ToList();
            //foreach (var invoice in projectInvoices)
            int totalInvoiceCount = projectInvoices.Count();
            for (int i = 1; i <= totalInvoiceCount; i++)
            {
                var invoice = projectInvoices[i - 1];

                InvoiceDetails invoiceDetails = new InvoiceDetails();
                invoiceDetails.Invoice = invoice;

                invoiceDetails.dInvoicePlanHours = CalculateHours(invoice.InvoiceStartDate, invoice.InvoiceEndDate, String.Empty, String.Empty);
                invoiceDetails.InvoicePlanHours = invoiceDetails.dInvoicePlanHours.ToString();

                //invoiceDetails.TimeSheets = timesheetService.GetTimeSheetByProjectId(ProjectId)
                if (i < totalInvoiceCount)
                {
                    //InvoiceEndDate = projectInvoices[i].InvoiceStartDate;
                    invoiceDetails.TimeSheets = timeSheet.Where(d => d.AddDate >= invoice.InvoiceStartDate && d.AddDate < projectInvoices[i].InvoiceStartDate)
                    .OrderBy(o => o.AddDate).ToList();
                }
                else
                {
                    invoiceDetails.TimeSheets = timeSheet.Where(d => d.AddDate >= invoice.InvoiceStartDate)
                    .OrderBy(o => o.AddDate).ToList();
                }
                invoiceDetails.dInvoiceActualHours = invoiceDetails.TimeSheets.Sum(w => w.WorkHours.TotalHours);
                invoiceDetails.InvoiceActualHours = (invoiceDetails.dInvoiceActualHours).GetTimeHoursFormatString();

                actualHoursDetailsDto.invoiceDetails.Add(invoiceDetails);
            }
            return View(actualHoursDetailsDto);
        }


        #region  ExtraMethods  

        private double CalculateHours(DateTime InvoiceStartDate, DateTime InvoiceEndDate, string DateFrom, string DateTo)
        {
            if (DateFrom.HasValue() && InvoiceStartDate < DateFrom.ToDateTime())
            {
                InvoiceStartDate = Convert.ToDateTime(DateFrom.ToDateTime());
            }
            if (DateTo.HasValue() && InvoiceEndDate > DateTo.ToDateTime())
            {
                InvoiceEndDate = Convert.ToDateTime(DateTo.ToDateTime());
            }
            return (((InvoiceEndDate - InvoiceStartDate).TotalDays + 1) * 8);
        }

        private List<SelectListItem> GetProjects()
        {
            //List<Project> lsProject = projectService.GetProjectListByPmuid(CurrentUser.RoleId == (int)Enums.UserRoles.PM ? CurrentUser.Uid : CurrentUser.PMUid);
            List<Project> lsProject = projectService.GetProjectList();
            var res = lsProject;
            //bind project
            var obj = res.Select(P => new ClientDto
            {
                ProjectName = P.Name + "" + (P.ClientId != null ? " (CRM Id : " + P.CRMProjectId + ") - " + P.Client.Name : ""),
                CrmId = P.CRMProjectId,
                PMUid = P.PMUid,
                ProjectId = P.ProjectId,
                ClientOrProjectId = "Project," + P.ProjectId
            });
            var list = obj.Select(x => new SelectListItem
            {
                Text = x.ProjectName,
                Value = x.ProjectId.ToString(),
                //Value = x.ClientOrProjectId.ToString(),
            }).ToList();
            return list;
        }

        private PagingService<ProjectInvoice> GetPagingAndFiltersForInvoice(IDataTablesRequest request, ProjectActualHoursDto filter, List<Project> projects)
        {
            var pagingServices = new PagingService<ProjectInvoice>(0, int.MaxValue);

            var expr = PredicateBuilder.True<ProjectInvoice>();

            if (filter.ProjectId.HasValue() && filter.ProjectId != "" && filter.ProjectId != "0")
            {
                expr = expr.And(e => e.ProjectId == Convert.ToInt32(filter.ProjectId));
            }
            else
            {
                var arrProject = projects.Select(p => p.ProjectId).ToArray();
                expr = expr.And(e => arrProject.Contains(e.ProjectId));
            }

            if (filter.DateFrom.HasValue() && filter.DateTo.HasValue())
            {
                var startDate = filter.DateFrom.ToDateTime();
                var endDate = filter.DateTo.ToDateTime();
                expr = expr.And(e => (e.InvoiceStartDate >= startDate && e.InvoiceStartDate <= endDate) ||
                (e.InvoiceEndDate <= endDate && e.InvoiceEndDate >= startDate));
            }
            else if (filter.DateFrom.HasValue())
            {
                var startDate = filter.DateFrom.ToDateTime();
                expr = expr.And(e => e.InvoiceStartDate >= startDate);
            }
            else if (filter.DateTo.HasValue())
            {
                var endDate = filter.DateTo.ToDateTime();
                expr = expr.And(e => e.InvoiceEndDate <= endDate);
            }

            pagingServices.Filter = expr;
            //pagingServices.Sort = o => o.OrderByDescending(c => c.Modified);
            pagingServices.Sort = o => o.OrderBy(c => c.InvoiceStartDate);
            return pagingServices;
        }

        private PagingService<UserTimeSheet> GetPagingAndFiltersForTimesheet(IDataTablesRequest request, ProjectActualHoursDto filter, List<Project> projects, List<ProjectInvoice> invoices)
        {
            var pagingServices = new PagingService<UserTimeSheet>(0, int.MaxValue);

            var expr = PredicateBuilder.True<UserTimeSheet>();

            if (filter.ProjectId.HasValue() && filter.ProjectId != "" && filter.ProjectId != "0")
            {
                expr = expr.And(e => e.ProjectID == Convert.ToInt32(filter.ProjectId));
            }
            else
            {
                var arrProject = projects.Select(p => p.ProjectId).ToArray();
                expr = expr.And(e => arrProject.Contains(e.ProjectID));
            }

            if (filter.DateFrom.HasValue() && filter.DateTo.HasValue())
            {
                var startDate = filter.DateFrom.ToDateTime();
                var endDate = filter.DateTo.ToDateTime();
                expr = expr.And(e => (e.AddDate >= startDate && e.AddDate <= endDate));
            }
            else if (filter.DateFrom.HasValue())
            {
                var startDate = filter.DateFrom.ToDateTime();
                expr = expr.And(e => e.AddDate >= startDate);
            }
            else if (filter.DateTo.HasValue())
            {
                var endDate = filter.DateTo.ToDateTime();
                expr = expr.And(e => e.AddDate <= endDate);
            }

            pagingServices.Filter = expr;
            pagingServices.Sort = o => o.OrderByDescending(c => c.AddDate);
            return pagingServices;
        }

        #endregion


        #endregion
    }
}