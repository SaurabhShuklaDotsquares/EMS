using EMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using static EMS.Core.Enums;
using EMS.Core;
using EMS.Web.Code.LIBS;
using EMS.Web.Models.Attendance;
using DataTables.AspNet.Core;
using System.Globalization;
using EMS.Web.Code.Attributes;
using EMS.Dto;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataTables.AspNet.AspNetCore;
    
namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class AttendanceController : BaseController
    {
        private readonly IUserLoginService userLoginService;
        private readonly ILeaveService leaveService;
        public AttendanceController(IUserLoginService _userLoginService, ILeaveService _leaveService)
        {
            userLoginService = _userLoginService;
            leaveService = _leaveService;
        }

        [CustomActionAuthorization]
        public ActionResult Index()
        {
            BindViewBags();
            return View();
        }

        [HttpPost]
        public IActionResult GetAttendance(IDataTablesRequest request, int userId, string month, bool displayOnDashBoard = false)
        {
            //System.Threading.Thread.Sleep(5000);
            string year = "";
            var monthdata = (month ?? "").Split('-');
            month = monthdata[0].Trim();
            if (monthdata.Count() > 1)
            {
                year = monthdata[1].Trim();
            }
            var user = userLoginService.GetUserInfoByID(userId);
            EmployeeAttendanceModel data = new EmployeeAttendanceModel();
            PostManager postRequest = new PostManager(SiteKey.HrmServiceURL + "/getattendance");
            postRequest.AddHeader("Hrmapikey", SiteKey.HrmApiKey);
            postRequest.AddHeader("Hrmapipassword", SiteKey.HrmApiPassword);

            //PostManager postRequest = new PostManager("https://hrm.dotsquares.com/webservice/getattendance");
            //postRequest.AddHeader("Hrmapikey", "HRMAPIKey5Ds");
            //postRequest.AddHeader("Hrmapipassword", "hRm#4@55R");


            if (user != null && user.HRMId.HasValue && !string.IsNullOrEmpty(user.EmailOffice))
            {
                var postData = new EmployeeAttendanceRequestModel
                {
                    EmailId = user.EmailOffice,
                    EmsId = user.Uid.ToString(),
                    HrmId = user.HRMId.Value,
                    Month = month,
                    Year = year,
                    Page = displayOnDashBoard ? "dashboard" : "all"
                };
                try
                {
                    //Response.Write(JsonConvert.SerializeObject(postData));
                    data = postRequest.PostData<EmployeeAttendanceRequestModel, EmployeeAttendanceModel>(postData);
                }
                catch { }
            }
            try
            {
                if (!SiteKey.IsLive)
                {
                    data = postRequest.PostData<EmployeeAttendanceRequestModel, EmployeeAttendanceModel>(new EmployeeAttendanceRequestModel { EmsId = "376", HrmId = 57, EmailId = "giriraj.vyas@dotsquares.com", Month = "07", Year = "2018", Page = displayOnDashBoard ? "dashboard" : "all" });
                }

                data.AttendanceData = data.AttendanceData ?? new List<AttendanceModel>();

                var officialleaves = leaveService.GetOfficialLeaves((byte)Country.India, true).Select(l => new OfficialLeaveDto { LeaveDate = l.LeaveDate, LeaveId = l.LeaveId, Title = l.Title }).ToList();

                if (CurrentUser.RoleId == (int)UserRoles.UKPM || CurrentUser.RoleId == (int)UserRoles.PMO || CurrentUser.RoleId == (int)UserRoles.UKBDM)
                {
                    officialleaves = leaveService.GetOfficialLeaves((byte)Country.UK, true).Select(l => new OfficialLeaveDto { LeaveDate = l.LeaveDate, LeaveId = l.LeaveId, Title = l.Title }).ToList();
                }
                IDictionary<string, object> additionalParameters = new Dictionary<string, object>();
                //additionalParameters.Add(new KeyValuePair<string, object>("totalWorkingDays", data.MonthData.TotalWorkingDays));
                // additionalParameters.Add(new KeyValuePair<string, object>("totalPresent", data.LeaveData.TotalPresent));
                // additionalParameters.Add(new KeyValuePair<string, object>("totalAbsent", data.LeaveData.TotalAbsent));
                //additionalParameters.Add(new KeyValuePair<string, object>("totalHalfDays", data.LeaveData.TotalHalfday));
                
                bool showInOutTime = CurrentUser.RoleId == (int)Enums.UserRoles.PM
                    || RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_Sales_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_HR_DesignationIds.Contains(CurrentUser.DesignationId);

                return DataTablesJsonResult(data.AttendanceData.Count, request, data.AttendanceData.Select((x, index) => new
                {
                    //date = DateTime.ParseExact(x.Date?.Trim() ?? "", "dd-MM-yyyy", CultureInfo.CreateSpecificCulture("en-GB")).ToString("ddd, MMM dd yyyy"),
                    date = DateTime.Parse(x.Date?.Trim() ?? "", CultureInfo.CreateSpecificCulture("en-GB")).ToString("ddd, MMM dd yyyy"),
                    intime = showInOutTime ? (x.InTime ?? "").ToLower().Replace(".", "").Replace("-", "").Trim() : "",
                    outtime = showInOutTime ? (x.OutTime ?? "").ToLower().Replace(".", "").Replace("-", "").Trim() : "",
                    status = GetStatus(x.Status, x.Date, officialleaves, x.InTime, x.OutTime),
                    color = GetColorClass(x.Status, x.Date, officialleaves),
                    totalWorkingHours = showInOutTime ? (x.Status ?? "").ToLower() == "a" ? "" : ((string.IsNullOrEmpty(x.InTime) && string.IsNullOrEmpty(x.OutTime)) || (x.HoursWorked ?? "").Trim() == "-") ? "" : x.HoursWorked : ""
                }), additionalParameters);
            }
            catch
            {
                return null;
            }
        }


        private void BindViewBags()
        {
            ViewBag.Users = GetEmployees();
            ViewBag.PM = userLoginService.GetPMAndPMOHRUsers().Select(p => new SelectListItem { Text = p.Name, Value = p.Uid.ToString() }).ToList();
            List<SelectListItem> data = new List<SelectListItem>();
            var date = DateTime.Today.AddMonths(-1);
            var currentdate = DateTime.Today;
            while (true)
            {
                if (((currentdate.Month >= 5 && currentdate.Year == 2018) || currentdate.Year >= 2018))
                {
                    data.Add(new SelectListItem { Text = ((Month)currentdate.Month).GetDescription() + " - " + currentdate.Year, Value = currentdate.Month + "-" + currentdate.Year });
                }
                if (currentdate.Month == date.Month && currentdate.Year == date.Year)
                {
                    break;
                }
                currentdate = currentdate.AddMonths(-1);
            }
            ViewBag.Month = data;
        }

        private List<SelectListItem> GetEmployees(bool selectDefault = true)
        {
            var EmployeeList = userLoginService.GetUsersListByAllDesignation(CurrentUser.Uid, CurrentUser.RoleId, CurrentUser.DesignationId);
            if (CurrentUser.RoleId == (int)Enums.UserRoles.HRBP)
            {
                EmployeeList = new List<Data.UserLogin>();
            }
            var selectEmpList = EmployeeList.Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.Uid.ToString() }).ToList();
            if (CurrentUser.RoleId != (int)Enums.UserRoles.HRBP)
            {
                selectEmpList.Insert(0, new SelectListItem { Text = CurrentUser.Name, Value = CurrentUser.Uid.ToString(), Selected = true });
            }
            return selectEmpList;
        }
        public string GetStatus(string status, string date, List<OfficialLeaveDto> officialleaves, string inTime, string outTime)
        {
            if (string.IsNullOrEmpty(date))
            {
                return "";
            }

            //DateTime day = DateTime.ParseExact(date.Trim(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime day = DateTime.Parse(date.Trim(), CultureInfo.CreateSpecificCulture("en-GB"));

            if (day.ToString("dd/MM/yyyy") == DateTime.Now.ToString("dd/MM/yyyy"))
            {
                return "In Process";
            }
            if (officialleaves.Any(l => l.LeaveDate.Date == day.Date))
            {
                return "Holiday (" + officialleaves.FirstOrDefault(l => l.LeaveDate.Date == day.Date).Title + ")";
            }
            if (day.DayOfWeek == DayOfWeek.Sunday)
            {
                return "";
            }
            switch ((status ?? "").ToLower())
            {
                case "a":
                    status = "Absent";
                    break;
                case "p":
                    status = "Present";
                    break;
                case "hd":
                    status = "Half Day";
                    break;
                case "ms":
                    status = outTime == "-" ? "In Process" : inTime == "-" ? "In Process" : "";
                    //status = outTime=="-" ? "Missing punch-out Time" : inTime=="-" ? "Missing Punch-in time" : "";
                    break;
            }

            if (day.DayOfWeek == DayOfWeek.Saturday && (status ?? "").ToLower() == "absent" && (inTime ?? "").Replace("-", "").Trim() == "" && (outTime ?? "").Replace("-", "").Trim() == "")
            {
                return "";
            }
            return status;

        }

        public string GetColorClass(string status, string date, List<OfficialLeaveDto> officialleaves)
        {
            if (string.IsNullOrEmpty(date))
            {
                return "";
            }

            //DateTime day = DateTime.ParseExact(date.Trim(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime day = DateTime.Parse(date.Trim(), CultureInfo.CreateSpecificCulture("en-GB"));

            if (officialleaves.Any(l => l.LeaveDate.Date == day.Date))
            {
                return "bg-holiday";
            }
            if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
            {
                if ((status ?? "").ToLower() == "p")
                    return "bg-workingweekend";
                return "bg-weekend";
            }
            var bgstatus = "";
            switch ((status ?? "").ToLower())
            {
                case "a":
                    bgstatus = "bg-red";
                    break;
                case "hd":
                    bgstatus = "";
                    break;
                case "ms":
                    bgstatus = "";
                    break;
                default:
                    bgstatus = "";
                    break;
            }
            return bgstatus;
        }
    }
}