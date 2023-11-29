using EMS.Dto;
using EMS.Service;
using EMS.Data;
using EMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using EMS.Web.Code.Attributes;
using DataTables.AspNet.Core;
using EMS.Web.Code.LIBS;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class TimesheetController : BaseController
    {
        #region "Fields"
        private ITimesheetService timesheetService;
        private IProjectService projectService;
        private IVirtualDeveloperService virtualdeveloperService;
        private IUserLoginService userLoginService;
        private IPreferenceService preferenceService;
        private bool IfAshishTeamPMUId { get { return (CurrentUser.PMUid == SiteKey.AshishTeamPMUId || CurrentUser.Uid == SiteKey.AshishTeamPMUId) ? true : false; } }
        #endregion

        #region "Constructor"
        public TimesheetController(ITimesheetService _timesheetService, IProjectService _projectService, IVirtualDeveloperService _virtualdeveloperService, 
            IUserLoginService _userLoginService, IPreferenceService _preferenceService)
        {
            this.timesheetService = _timesheetService;
            this.projectService = _projectService;
            this.virtualdeveloperService = _virtualdeveloperService;
            this.userLoginService = _userLoginService;
            this.preferenceService = _preferenceService;
        }
        #endregion


        // GET: Timesheet
        public ActionResult Index()
        {
            ViewBag.TimeSheetEditableDays = GetTimesheetEditDays();
            return View(IfAshishTeamPMUId);
        }


        public ActionResult GetProjectList()
        {

            //int pmUId = Convert.ToInt32(CurrentUser.RoleId == (int)Enums.UserRoles.PM ? CurrentUser.Uid : CurrentUser.PMUid);
            List<ProjectListDto> getProjects = new List<ProjectListDto>();
            var getProjectList = IfAshishTeamPMUId? projectService.GetProjectListByPmuid(PMUserId, CurrentUser.Uid) : projectService.GetProjectListByPmuid(PMUserId, CurrentUser.Uid);
            getProjects = getProjectList.Select(x => new ProjectListDto { Id = x.ProjectId, Name = x.Name }).ToList();
            return Json(getProjects);
        }
        public List<DeveloperListDto> GetVirtualDevelperByProjectID(int projectId)
        {
            List<DeveloperListDto> returnList = new List<DeveloperListDto>();
            if (projectId > 0)
            {
                bool isConcat = true;
                var Record = projectService.GetDevelopersByProjectId(projectId).Select(A => new { VDName = A.VirtualDeveloper.VirtualDeveloper_Name, VirtualDeveloper_ID = String.Concat(A.VirtualDeveloper.VirtualDeveloper_ID) });
                var deflist = virtualdeveloperService.GetDefaultVDeveloper();
                if ((CurrentUser.RoleId == (int)Enums.UserRoles.PMO || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM))
                {
                    if (CurrentUser.Uid == Convert.ToInt32(SiteKey.UKDeveloperUID))
                    {
                        deflist = deflist.Where(a => a.Ismain.Value == false).ToList();
                    }
                    else
                    {
                        isConcat = false;
                        deflist = deflist.Where(a => a.PMUid == (CurrentUser.PMUid == 0 ? CurrentUser.Uid : CurrentUser.PMUid) || (a.Ismain.Value == false && a.PMUid == null)).ToList();
                    }
                }
                else if (RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId))
                {
                    isConcat = false;
                    deflist = deflist.Where(a => a.PMUid == (CurrentUser.PMUid == 0 ? CurrentUser.Uid : CurrentUser.PMUid) || (a.Ismain.Value == false && a.PMUid == null)).ToList();
                }
                else
                {
                    deflist = deflist.Where(a => a.PMUid == 0 && a.Ismain.Value == false || (a.Ismain.Value == false && a.PMUid == null)).ToList();
                }

                var defaultREcord = deflist.Select(A => new { VDName = A.VirtualDeveloper_Name, VirtualDeveloper_ID = A.VirtualDeveloper_ID.ToString() });
                var resultDev = defaultREcord.ToList();
                if (isConcat)
                {
                    if (Record != null && Record.Count() > 0)
                    {
                        resultDev = defaultREcord.Concat(Record).ToList();
                    }
                }
                returnList = resultDev.Select(x => new DeveloperListDto { Id = x.VirtualDeveloper_ID, Name = x.VDName }).ToList();

            }
            return returnList;
        }
        public ActionResult GetDeveloperByProject(int projectId)
        {
            return Json(GetVirtualDevelperByProjectID(projectId));
        }

        //public ActionResult GetData(int? pageSize, int? pageNumber)
        public ActionResult GetData([FromBody] TimeSheetData data)
        {
            //int skipCount = pageSize.Value * (pageNumber.Value - 1);
            ResponseData model = new ResponseData();
            DateTime? fromDate = !string.IsNullOrWhiteSpace(data.DateFrom) ? DateTime.ParseExact(data.DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;
            DateTime? toDate = !string.IsNullOrWhiteSpace(data.DateTo) ? DateTime.ParseExact(data.DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;


            List<TimesheetDto> returnList = new List<TimesheetDto>();
            int timesheetEditDays = - GetTimesheetEditDays();
            KeyValuePair<int, List<UserTimeSheet>> userTimesheetData = timesheetService.GetTimesheetListByUidWithPaging(CurrentUser.Uid, data.pageSize, data.pageNumber, fromDate, toDate);
            bool nonEligibleForCMMIByPass=!(CurrentUser.RoleId == (int)Enums.UserRoles.PMO || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM
                || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM || CurrentUser.RoleId == (int)Enums.UserRoles.Director);
            returnList = userTimesheetData.Value.Select((x, Index) => new TimesheetDto
            {
                SNo = (data.pageSize * (data.pageNumber - 1)) + (Index + 1),
                Id = Convert.ToInt32(x.UserTimeSheetID),
                Description = x.Description + (x.IsFillByPMS ? "<sup style = 'color:#ca1198;font-weight:bold;'><b>PMS</b></sup> " : ""),
                Name = x.VirtualDeveloper != null ? x.VirtualDeveloper.VirtualDeveloper_Name : "",
                AddedDate = x.AddDate.ToString("ddd, MMM, dd yyyy"),
                project = x.Project != null ? (x.Project.Name == "Others" ? x.Project.Name : x.Project.Name + " [" + Convert.ToString(x.Project.CRMProjectId) + "]") : "",
                ProjectId = x.Project != null ? x.Project.ProjectId : 0,
                DeveloperId = x.VirtualDeveloper != null ? x.VirtualDeveloper.VirtualDeveloper_ID : 0,
                WorkHours = (x.WorkHours.Hours > 0 ? x.WorkHours.Hours + " hr " : "") + (x.WorkHours.Minutes > 0 ? x.WorkHours.Minutes + " min" : ""),
                AddedDateEdit = x.AddDate.ToString("dd/MM/yyyy"),
                WorkHoursEdit = x.WorkHours.ToString(@"hh\:mm"),
                IsReviewed = x.IsReviewed.HasValue ? x.IsReviewed.Value : false,
                ReviewedBy = x.IsReviewed.HasValue && x.IsReviewed.Value ? (userLoginService.GetUserInfoByID(x.ReviewedByUid.Value).Name) : "",
                ReviewedDate = x.IsReviewed.HasValue && x.IsReviewed.Value ? (x.ReviewedDate.HasValue ? x.ReviewedDate.Value.ToString("dd/MM/yyyy hh:mm tt") : null) : "",
                ReviewStatus = x.IsReviewed.HasValue && x.IsReviewed.Value ? (userLoginService.GetUserInfoByID(x.ReviewedByUid.Value).Name) + " " + x.ReviewedDate.Value.ToString("dd/MM/yyyy hh:mm tt") : "Pending",
                IsNotWithin10DaysRange = !(x.AddDate.Date<=DateTime.Today.Date && x.AddDate.Date >= DateTime.Today.Date.AddDays(timesheetEditDays)),                
                Source = x.IsFillByPMS ? "PMS" : "EMS",
                IsRelatedProjectCMMI = x.IsFillByPMS ||(nonEligibleForCMMIByPass &&(x.Project.IsCmmi != null ? x.Project.IsCmmi.Value : false)),
                InsertDate = x.InsertedDate.HasValue ? x.InsertedDate.Value.ToString("MMM, dd yyyy hh:mm tt") : null,


            }).ToList();
            //double totalTime = 0;
            //userTimesheetData.Value.ForEach(x => { totalTime += x.WorkHours.TotalMinutes; });
            //model.TotalWorkingHours = Extensions.DateTimeConversion(totalTime);

            model.TotalRecords = userTimesheetData.Key;
            model.PageSize = data.pageSize;
            model.CurrentPage = data.pageNumber;
            model.TimeSheetList = returnList;


            int pmUId = Convert.ToInt32(CurrentUser.RoleId == (int)Enums.UserRoles.PM ? CurrentUser.Uid : CurrentUser.PMUid);
            List<ProjectListDto> getProjects = new List<ProjectListDto>();
            
            List<Project> getProjectList = IfAshishTeamPMUId ? projectService.GetProjectListByPmuidOnlyforTimeSheet(PMUserId, CurrentUser.Uid) 
                : projectService.GetTimeSheetProjectListByPmuid(pmUId, CurrentUser.Uid,CurrentUser.RoleId);
            if (getProjectList.Count > 0)
            {
                Project otherProject = getProjectList.Find(f => f.Name == "Others");
                if (otherProject != null)
                {
                    getProjectList.Remove(otherProject);
                    getProjectList.Add(otherProject);
                }
            }

            getProjects = getProjectList.Select(x => new ProjectListDto { Id = x.ProjectId, Name = x.Name == "Others" ? x.Name : x.Name + " [" + Convert.ToString(x.CRMProjectId) + "]" }).ToList();
            model.ProjectList = getProjects;

            var timesheetRecord = returnList.OrderByDescending(m => m.Id).FirstOrDefault();
            if (timesheetRecord != null)
            {
                model.DefaultProjectID = timesheetRecord.ProjectId;
                model.DeveloperList = GetVirtualDevelperByProjectID(timesheetRecord.ProjectId);
                model.DefaultVirtualDeveloperID = timesheetRecord.DeveloperId;
                model.IsUKPM = false;
                if ((CurrentUser.RoleId == (int)Enums.UserRoles.PMO || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM))
                {
                    //if (getProjects.FirstOrDefault(x => x.Id == timesheetRecord.ProjectId).Name != "Others" && CurrentUser.Uid != Convert.ToInt32(EMS.Web.Code.LIBS.SiteKey.UKDeveloperUID))
                    if (CurrentUser.Uid != Convert.ToInt32(EMS.Web.Code.LIBS.SiteKey.UKDeveloperUID))
                    {
                        model.DefaultVirtualDeveloperID = Convert.ToInt32(SiteKey.UKPMVirtualDeveloperID);
                        model.IsUKPM = true;
                    }
                }

            }
            // if ((CurrentUser.RoleId == (int)Enums.UserRoles.PMO || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM))
            if ((CurrentUser.RoleId == (int)Enums.UserRoles.PMO || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM) && CurrentUser.Uid != Convert.ToInt32(EMS.Web.Code.LIBS.SiteKey.UKDeveloperUID))
            {
                model.IsUKPM = true;
            }

            return Json(model);
        }

        public ActionResult AddTimesheetData([FromBody] TimeSheetModel model)
        {
            Boolean isvalid = true;
            TimesheetResponse res = new TimesheetResponse();
            string[] formats = { "dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy", "dd/MM/yyyy h:mm:ss tt", "d/MM/yyyy h:mm:ss tt", "dd/M/yyyy h:mm:ss tt", "yyyy-MM-dd", "yyyy-M-dd", "yyyy-MM-d", "yyyy-MM-dd h:mm:ss tt", "yyyy-M-dd h:mm:ss tt", "yyyy-MM-d h:mm:ss tt", "dd-MM-yyyy", "d-MM-yyyy", "dd-M-yyyy", "dd-MM-yyyy h:mm:ss tt", "d-MM-yyyy h:mm:ss tt", "dd-M-yyyy h:mm:ss tt", "yyyy/MM/dd", "yyyy/M/dd", "yyyy/MM/d", "yyyy/MM/dd  h:mm:ss tt", "yyyy/M/dd  h:mm:ss tt", "yyyy/MM/d  h:mm:ss tt", "d/M/yyyy h:mm:ss tt", "M/dd/yyyy h:mm:ss tt", "MM/dd/yyyy h:mm:ss tt", "MM/d/yyyy h:mm:ss tt", "M/dd/yyyy", "MM/dd/yyyy", "MM/d/yyyy", "yyyyMMdd", "d/M/yy" };
            string retVal = "";
            try
            {
                if (model.ProjectId <= 0)
                {
                    isvalid = false;
                    res.success = "no";
                    res.message = "Please select the project";
                }

                if (String.IsNullOrEmpty(model.AddedDate))
                {
                    isvalid = false;
                    res.success = "no";
                    res.message = "Please select the correct date";
                }
                else
                {
                    var timesheetDate=DateTime.ParseExact(model.AddedDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                    if(DateTime.Today.Date.AddDays(- GetTimesheetEditDays()) <= timesheetDate.Date && DateTime.Today.Date >= timesheetDate.Date)
                    {
                        // Date is in valid range
                    }
                    else
                    {
                        isvalid = false;
                        res.success = "no";
                        res.message = $"Employees can add / edit timesheet only for the past {GetTimesheetEditDays() } days.";
                    }
                }

                if (model.DeveloperId == 0)
                {
                    isvalid = false;
                    res.success = "no";
                    res.message = "Please select the type/virtual developer";
                }

                if (String.IsNullOrEmpty(model.WorkHours))
                {
                    isvalid = false;
                    res.success = "no";
                    res.message = "Please enter the working hours";
                }
                TimeSpan time = new TimeSpan();
                TimeSpan.TryParse("00:00", out time);
                TimeSpan.TryParse(model.WorkHours, out time);

                if (time.TotalMinutes == 0)
                {
                    isvalid = false;
                    res.success = "no";
                    res.message = "Please enter the correct working hours";
                }
                if(model.ProjectId>0)
                {
                    var project = projectService.GetProjectById(model.ProjectId);
                    bool CMMIByPassAllowed = (CurrentUser.RoleId == (int)Enums.UserRoles.PMO 
                        || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM
                        || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM || CurrentUser.RoleId == (int)Enums.UserRoles.Director);
                    if (!CMMIByPassAllowed && project != null && project.IsCmmi == true)
                    {
                        isvalid = false;
                        res.success = "no";
                        res.message = "CMMI Project can't be edited from EMS";
                    }
                }

                if (isvalid)
                {

                    UserTimeSheet obj = new UserTimeSheet();

                    if (model.Id > 0)
                    {
                        obj = timesheetService.GetTimesheetByTimesheetId(model.Id);
                    }
                    else
                    {
                        obj.IsFillByPMS = false;
                        obj.InsertedDate = DateTime.Now;
                        obj.UserTimeSheetID = Convert.ToInt32(model.Id);
                    }
                    obj.ProjectID = model.ProjectId;
                    obj.AddDate = DateTime.ParseExact(model.AddedDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                    obj.VirtualDeveloper_id = model.DeveloperId;
                    obj.Description = (model.Description ?? "").Replace("<", "&lt;").Replace(">", "&gt;");
                    obj.WorkHours = time;
                    //if (model.WorkHours.Trim() != "")
                    //{
                    //    TimeSpan.TryParse(model.WorkHours, out time);
                    //    obj.WorkHours = time;
                    //}
                    //else
                    //    obj.WorkHours = time;

                    obj.UID = CurrentUser.Uid;
                    obj.ModifyDate = DateTime.Now;
                    obj.IsReviewed = false;
                    // obj.IsFillByPMS = false;
                    timesheetService.Save(obj);

                    res.success = "yes";
                    res.message = "Timesheet has been added successfully";
                }
            }
            catch (Exception ex)
            {
                res.success = "no";
                res.message = ex.Message.ToString();
            }

            return Json(res);
        }


        private int GetTimesheetEditDays()
        {
            int pmUId = Convert.ToInt32(CurrentUser.RoleId == (int)Enums.UserRoles.PM ? CurrentUser.Uid : CurrentUser.PMUid);
            Preference preference = preferenceService.GetDataByPmid(pmUId);
            if (preference != null && preference.TimesheetModifyDay>0)
            {
                return (int)preference.TimesheetModifyDay;
            }
            else
            {
                return 10;
            }
        }
    }

    public class TimeSheetData
    {
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }

    public class TimesheetResponse
    {
        public string success { get; set; }

        public string message { get; set; }
    }

    public class TimeSheetModel
    {
        public string AddedDate { get; set; }
        public int ProjectId { get; set; }
        public int DeveloperId { get; set; }
        public string WorkHours { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        
    }
}