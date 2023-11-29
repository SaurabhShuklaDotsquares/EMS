using DataTables.AspNet.Core;
using DataTables.AspNet.AspNetCore;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Dto.SARAL;
using EMS.Service;
using EMS.Service.SARAL;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using static EMS.Core.Enums;
using EMS.Website.Models.Others;
using System.Net.Http;
using Newtonsoft.Json;
using System.Data;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using EMS.Website.LIBS;
using System.IO;
using Microsoft.EntityFrameworkCore.Internal;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class HomeController : BaseController
    {
        #region Fileds and Constructor

        private readonly IUserLoginService userLoginService;
        private readonly ITaskService taskService;
        private readonly IProjectClosureService projectClosureService;
        private readonly IProjectInvoiceService projectInvoiceService;
        private readonly IPreferenceService preferenceService;
        private readonly ILeaveService leaveService;
        private readonly ILeadServices leadServices;
        private readonly ITimesheetService timeSheetService;
        private readonly ILevDetailsService levDetailsService;
        private readonly string[] allLeadsUsers;
        //private readonly string[] UKAUUserIDToShowAshishTeamActivity;
        private bool IfAshishTeamPMUId { get { return (CurrentUser.PMUid == SiteKey.AshishTeamPMUId || CurrentUser.Uid == SiteKey.AshishTeamPMUId) ? true : false; } }

        public HomeController(ITimesheetService timeSheetService, ILeadServices leadServices, ILeaveService leaveService, IPreferenceService preferenceService, IUserLoginService userLoginService, ITaskService taskService, IProjectClosureService projectClosureService, IProjectInvoiceService projectInvoiceService, ILevDetailsService levDetailsService)
        {
            this.userLoginService = userLoginService;
            this.projectInvoiceService = projectInvoiceService;
            this.taskService = taskService;
            this.projectClosureService = projectClosureService;
            this.preferenceService = preferenceService;
            this.leaveService = leaveService;
            this.leadServices = leadServices;
            this.timeSheetService = timeSheetService;
            this.levDetailsService = levDetailsService;
            this.allLeadsUsers = SiteKey.AccessAllLeads.Split(',');
            //this.UKAUUserIDToShowAshishTeamActivity = SiteKey.UKAUUserIDToShowAshishTeamActivity.Split(',');
        }

        public bool IsUKAUUserIDToShowAshishTeamActivity
        {
            get { return (SiteKey.UKAUUserIDToShowAshishTeamActivity != null && SiteKey.UKAUUserIDToShowAshishTeamActivity.Split(',').ToList().Contains(CurrentUser.Uid.ToString())); }
        }

        #endregion

        public string GetDaySuffix(DateTime joiningDate)
        {
            var year = DateTime.Now.Year - joiningDate.Year;
            switch (year)
            {
                case 1:
                case 21:
                case 31:
                    return string.Format("{0}{1}", year, "st Work Anniversary");
                case 2:
                case 22:
                    return string.Format("{0}{1}", year, "nd Work Anniversary");
                case 3:
                case 23:
                    return string.Format("{0}{1}", year, "rd Work Anniversary");
                default:
                    return string.Format("{0}{1}", year, "th Work Anniversary");
            }
        }

        public ActionResult Index()
        {           
            HomeDto model = new HomeDto
            {
                IsPMUser = CurrentUser.RoleId == (int)Enums.UserRoles.PM,
                IsDirector = CurrentUser.RoleId == (int)Enums.UserRoles.Director,
                IsAshishTeamMember = (CurrentUser.PMUid == SiteKey.AshishTeamPMUId || CurrentUser.Uid == SiteKey.AshishTeamPMUId),
                IsUKAUUserIDToShowAshishTeamActivity = this.IsUKAUUserIDToShowAshishTeamActivity
            };
            int PmUid = CurrentUser.PMUid;
            var DailyThought = userLoginService.GetDailyThought();

            if (DailyThought != null)
            {
                model.DailyThought1 = DailyThought.Thought1.HasValue() ? DailyThought.Thought1 : SiteKey.DailyThought1;
                model.DailyThought2 = DailyThought.Thought2.HasValue() ? DailyThought.Thought2 : SiteKey.DailyThought2;
            }

            if (!model.IsDirector)
            {
                var users = userLoginService.GetUsersByPM(PMUserId);

                model.UserBirthday = string.Join(" / ", users.Where(u => u.DOB.HasValue && u.DOB.Value.Day == DateTime.Today.Day && u.DOB.Value.Month == DateTime.Today.Month)
                                                           .Select(x => string.Format("{0} {1}", x.Title, x.Name)));

                model.UserMarriage = string.Join(" / ", users.Where(u => u.MarraigeDate.HasValue && u.MarraigeDate.Value.Day == DateTime.Today.Day && u.MarraigeDate.Value.Month == DateTime.Today.Month)
                                                           .Select(x => string.Format("{0} {1}", x.Title, x.Name)));

                model.UserWorkanniversary = string.Join(" / ", users.Where(u => u.JoinedDate.HasValue && u.JoinedDate.Value.Day == DateTime.Today.Day && u.JoinedDate.Value.Month == DateTime.Today.Month && u.JoinedDate.Value.Year < DateTime.Today.Year && u.IsResigned == false)
                                                          .Select(x => string.Format("{0} {1} {2} {3} {4} {5}", x.Title, "<a style='cursor:pointer;' title='", GetDaySuffix(x.JoinedDate.Value), "'>", x.Name, "</a>")));

                model.BAList = users.Where(x => RoleValidator.BA_RoleIds.Contains(x.RoleId.Value) || x.RoleId == (int)UserRoles.PM)
                                        .Select(n => new SelectListItem { Text = n.Name, Value = n.Uid.ToString() })
                                        .ToList();

                if (CurrentUser.RoleId != (int)Enums.UserRoles.PM && CurrentUser.RoleId != (int)Enums.UserRoles.HRBP)
                {
                    Preference objPreference = preferenceService.GetDataByPmid(PMUserId);
                    if (objPreference != null)
                    {
                        var userTimeSheet = timeSheetService.GetLatestTimeSheetEntry(CurrentUser.Uid);

                        int Timesheet = objPreference.TimeSheetDay ?? 1;
                        int weekday = Convert.ToInt32(DateTime.Now.DayOfWeek);
                        Timesheet = weekday <= Timesheet ? Timesheet + 2 : Timesheet;

                        DateTime DT = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(((Timesheet) * -1));
                        if (userTimeSheet != null)
                        {
                            if (userTimeSheet.AddDate < DT)
                            {
                                DateTime maxDate = userTimeSheet.AddDate;
                                bool sent = false;
                                var expr = PredicateBuilder.True<OfficialLeave>();
                                expr = expr.And(e => e.CountryId == (CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM
                                || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM ? (int)Enums.Country.UK : (int)Enums.Country.India));
                                expr = expr.And(e => e.LeaveType.ToLower() == "holiday" && e.IsActive);
                                expr = expr.And(e => e.LeaveDate.Date >= userTimeSheet.AddDate.Date);
                                expr = expr.And(e => e.LeaveDate.Date <= DateTime.Now.Date);
                                var allofficialLeaves= leaveService.GetOfficialLeavesInDuration(expr);
                                var officialLeaves = allofficialLeaves.Count();
                                var objLeave = leaveService.GetUserLastLeave(CurrentUser.Uid, maxDate);
                                if (objLeave != null && maxDate < objLeave.EndDate && objLeave.StartDate < DateTime.Now.Date)
                                {
                                    if (DateTime.Now.Subtract(objLeave.EndDate).Days > Timesheet)
                                    {
                                        if (maxDate.AddDays(1) == objLeave.StartDate)
                                        {
                                            if (objLeave.EndDate.DayOfWeek == DayOfWeek.Friday || objLeave.EndDate.DayOfWeek == DayOfWeek.Saturday)
                                            {
                                                int daysUntilMonday = ((int)DayOfWeek.Monday - (int)objLeave.EndDate.DayOfWeek + 7) % 7;
                                                objLeave.EndDate = objLeave.EndDate.AddDays(daysUntilMonday - 1);
                                            }
                                            maxDate = objLeave.EndDate;
                                        }
                                        sent = true;
                                    }
                                }
                                if (officialLeaves > 0 && allofficialLeaves.FirstOrDefault().LeaveDate< maxDate)
                                {                                    
                                    maxDate = DateTime.Now.AddDays(officialLeaves * -1);
                                    sent = true;
                                }
                                else
                                {
                                    sent = true;
                                }

                                if (CurrentUser.RoleId != (int)Enums.UserRoles.PMO)
                                {
                                    if (sent == true && IfAshishTeamPMUId == true)
                                    {
                                        model.UserTimeSheet = " Your timesheet has not been submitted since " + maxDate.AddDays(1).ToFormatDateString("dd MMM yyyy") + ", to avoid forced leave please fill your timesheet on daily basis.";
                                    }
                                    else if (sent == true && IfAshishTeamPMUId == false)
                                    {
                                        model.UserTimeSheet = " Your timesheet has not been submitted since " + maxDate.AddDays(1).ToFormatDateString("dd MMM yyyy");
                                    }
                                }
                            }
                        }
                        else
                        {
                            var user = userLoginService.GetUserInfoByID(CurrentUser.Uid);
                            if (CurrentUser.RoleId != (int)Enums.UserRoles.PMO)
                            {
                                if (user.JoinedDate.HasValue && user.JoinedDate.Value < DT)
                                {
                                    model.UserTimeSheet = " Your timesheet has not been submitted ";
                                }
                            }
                        }
                    }
                }
            }
            if (IfAshishTeamPMUId)
            {
                try
                {
                    UserLogin objUser = userLoginService.GetUserInfoByID(PMUserId);
                    RequestObject reqObj = new RequestObject
                    {
                        FromDate = DateTime.Now.ToString("dd-MMM-yyyy"),
                        ToDate = DateTime.Now.ToString("dd-MMM-yyyy"),
                        emailid = objUser.EmailOffice
                    };
                    ResponseRoot apiResult = GetDataFromAPI(reqObj);
                    AssignPlannedAndActualHrs(ref model, apiResult);
                }
                catch (Exception ex)
                {

                }
            }

            return View(model);
        }

        #region TaskList

        [HttpPost]
        public IActionResult TaskList(IDataTablesRequest request)
        {
            var pagingServices = new PagingService<Task>(request.Start, request.Length > 0 ? request.Length : int.MaxValue);
            var expr = PredicateBuilder.True<Task>();
            expr = expr.And(x => x.AddedUid == CurrentUser.Uid || x.TaskAssignedToes.Any(m => m.AssignUid == CurrentUser.Uid));
            expr = expr.And(x => x.TaskStatusID != (int)Enums.TaskStatusType.Closed && x.TaskAssignedToes.Any(t => t.AssignUid == CurrentUser.Uid && t.TaskStatusId != (int)Enums.TaskStatusType.Completed));

            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "Name":
                            return o.OrderByColumn(item, c => c.TaskName);

                        default:
                            return o.OrderByColumn(item, c => c.TaskID);
                    }
                }
                return o.OrderByDescending(c => c.LastUpdatedDate);
            };
            int totalCount = 0;
            var response = taskService.GettaskByPaging(out totalCount, pagingServices);
            //  return null;
            // change in to dataable json result

            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                Id = r.TaskID,
                TaskId = "Task ID :" + r.TaskID,
                rowIndex = (index + 1) + (request.Start),
                TaskName = r.TaskName,
                Source = (r.MomMeetingTaskId != null && r.MomMeetingTaskId > 0 ? "MOM" : "TO-DO"),
                meetingId = (r.MomMeetingTaskId != null && r.MomMeetingTaskId > 0 ? r.MomMeetingTask.MomMeetingId : 0),
                UpdatedDate = r.LastUpdatedDate.ToFormatDateString("dd/MM/yyyy"),
                TaskEndDate = r.TaskEndDate.ToFormatDateString("dd/MM/yyyy"),
                //Status = r.TaskStatu.TaskStatus,
                Status = taskService.GetStatus(r.TaskID, CurrentUser.Uid),
                MeetingStatus = (r.MomMeetingTaskId != null && r.MomMeetingTaskId > 0 ? Extensions.GetDescription((Enums.MomMeetingStatus)r.MomMeetingTask.Status) : ""),
                AssignTo = string.Join(", ", r.TaskAssignedToes.Select(x => x.UserLogin.Name)),
                AssignBy = r.UserLogin.Name,
                Priority = ((Enums.Priority)r.Priority).GetDescription(),
                AllowChase = r.UserLogin.Uid == CurrentUser.Uid
            }));
        }

        #endregion

        #region Expected New Work

        [HttpPost]
        public IActionResult ExpectedNewWork(IDataTablesRequest request, int? BAUser, HomeReportingDays reportingDays)
        {
            int totalCount;
            int currentUserId = CurrentUser.Uid;
            bool isPMUser = CurrentUser.RoleId == (int)Enums.UserRoles.PM;
            bool isAllLeadUser = allLeadsUsers.Contains(Convert.ToString(currentUserId));
            bool IsUKAUUserIDToShowAshishTeamActivity = this.IsUKAUUserIDToShowAshishTeamActivity;

            var pagingServcices = request != null ? new PagingService<ProjectLead>(request.Start, request.Length) : new PagingService<ProjectLead>(0, int.MaxValue);

            var expr = PredicateBuilder.True<ProjectLead>().And(x => x.LeadStatu.StatusName == "Chase Request" || x.LeadStatu.StatusName == "Action Required From (Team)" || x.LeadStatu.StatusName == "Action Required From (Out of India PM)");

            if (IsUKAUUserIDToShowAshishTeamActivity)
            {
                expr = expr.And(x => x.PMID == currentUserId || x.PMID == SiteKey.AshishTeamPMUId);
            }
            else if (isPMUser)
            {
                expr = expr.And(x => x.PMID == currentUserId);
            }
            else
            {
                expr = expr.And(l => (l.PMID == PMUserId || l.AbroadPM.Uid == currentUserId) && (l.OwnerId == currentUserId || l.CommunicatorId == currentUserId || l.LeadTechnicians.Any(t => t.TechnicianId == currentUserId) || l.AbroadPM.Uid == currentUserId));
            }

            //expr = expr.Or(x => x.AbroadPM.Uid == currentUserId);

            expr = expr.And(x => x.ProjectClosureId != null);

            if (BAUser.HasValue)
            {
                expr = expr.And(b => b.OwnerId == BAUser.Value || b.CommunicatorId == BAUser.Value || b.LeadTechnicians.Any(x => x.TechnicianId == BAUser.Value));
            }

            switch (reportingDays)
            {
                case HomeReportingDays.Today:
                    {
                        var startDate = DateTime.Today;
                        expr = expr.And(L => L.NextChasedDate.Value <= startDate || L.ConversionDate <= startDate);
                        //expr = expr.And(L => L.NextChasedDate.Value <= startDate);
                    }
                    break;

                case HomeReportingDays.Tomorrow:
                    {
                        var startDate = DateTime.Today;
                        var endDate = startDate.AddDays(1);
                        //expr = expr.And(L => L.NextChasedDate > startDate && L.NextChasedDate <= endDate);
                        expr = expr.And(L => (L.NextChasedDate > startDate && L.NextChasedDate <= endDate) || (L.ConversionDate > startDate && L.ConversionDate <= endDate));
                    }
                    break;

                case HomeReportingDays.Week:
                    {
                        var startOfWeek = DateTime.Today;
                        var endOfWeek = startOfWeek.AddDays(7);
                        //expr = expr.And(L => L.NextChasedDate >= startOfWeek && L.NextChasedDate <= endOfWeek);
                        expr = expr.And(L => (L.NextChasedDate >= startOfWeek && L.NextChasedDate <= endOfWeek) || (L.ConversionDate >= startOfWeek && L.ConversionDate <= endOfWeek));
                    }
                    break;

                default:
                    {
                        //DateTime endOfWeek = DateTime.Today.AddDays(5 - (1 * (int)(DateTime.Today.DayOfWeek)));
                        //expr = expr.And(L => L.NextChasedDate > endOfWeek);
                    }
                    break;
            }

            pagingServcices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.ModifyDate);
            };

            pagingServcices.Filter = expr;

            List<ProjectLead> response = leadServices.GetLeadsByPaging(out totalCount, pagingServcices);

            var AlmostConvertedId = leadServices.GetLeadType("Lead").Where(x => x.TypeName == "Almost Converted").FirstOrDefault().TypeId;

            string[] actionRequired = new string[] { "Action", "Chase" };

            var returnResult = response.Select((r, index) => new
            {
                LeadId = r.LeadId,
                rowIndex = (index + 1) + (request.Start),
                crmid = r.LeadCRMId ?? "",
                ClientId = r.LeadClientId.HasValue ? r.LeadClientId.ToString() : "",
                Client = r.LeadClient?.Name ?? "",
                LeadTitle = r.Title ?? "",
                NewClient = r.IsNewClient ? "New Client" : "Existing Client",
                AbroadPM = r.AbroadPM?.Country ?? "",
                GeneratedDate = r.AddDate.ToFormatDateString("MMM, dd yyyy hh:mm tt"),
                ModifiedDate = r.ModifyDate.ToFormatDateString("MMM, dd yyyy hh:mm tt"),
                ProposalDocument = r.ProposalDocument ?? "",
                Title = "",
                LastConversation = r.LeadTransactions.Any() ? r.LeadTransactions.OrderByDescending(x => x.AddDate).FirstOrDefault()?.Notes : "",
                LastConversationFull = r.LeadTransactions.Any() ? r.LeadTransactions.OrderByDescending(x => x.AddDate).FirstOrDefault()?.Notes : "",
                showLastConversationInDiv = r.LeadTransactions.Any() ? r.LeadTransactions.OrderByDescending(x => x.AddDate).FirstOrDefault()?.Notes?.Length > 100 : false,
                EstimateTimeinDay = r.EstimateTimeinDay != null ? WeekAndDay(Convert.ToInt32(r.EstimateTimeinDay)).ToString() : "",
                OwnersName = r.UserLogin1.Name + "/" + r.UserLogin.Name,
                NextChaseDate = r.NextChasedDate.ToFormatDateString("MMM, dd yyyy") ?? "",
                ConversionDate = r.ConversionDate.ToFormatDateString("MMM, dd yyyy") ?? "",
                ShowConversionDate = r.LeadType == AlmostConvertedId,
                Technologies = string.Join(", ", r.ProjectLeadTeches
                                                   .Where(x => x.Technology.Title.HasValue())
                                                   .Select(x => x.Technology.Title)
                                                   .Concat(r.Technologies.HasValue() ? r.Technologies.Split(',') : new string[] { })
                                                   .Distinct()),
                Source = r.AbroadPM?.Name ?? "",
                AssignedDate = r.AssignedDate.ToFormatDateString("MMM, dd yyyy hh:mm tt"),
                Status = r.LeadStatu?.StatusName ?? "",
                Remark = r.Remark ?? "",
                AllowConclusionAndEdit = (isPMUser && !IsUKAUUserIDToShowAshishTeamActivity) || isAllLeadUser || r.OwnerId == currentUserId,
                AllowClientAndDelete = (isPMUser && !IsUKAUUserIDToShowAshishTeamActivity),
                AllowAction = (isPMUser || IsUKAUUserIDToShowAshishTeamActivity || isAllLeadUser || r.OwnerId == currentUserId) && (r.LeadStatu.StatusName.Contains("Action") || r.LeadStatu.StatusName.Contains("Chase"))
            });
            // return null;
            // change in to dataable json result

            //return DataTablesJsonResult(totalCount, request, returnResult);

            IDictionary<string, object> additionalParameters = new Dictionary<string, object>();
            if (reportingDays == HomeReportingDays.Today)
            {
                additionalParameters.Add(new KeyValuePair<string, object>("futureOccupancy", response.Where(f => f.ConversionDate != null && f.ConversionDate <= DateTime.Now && f.LeadType == AlmostConvertedId).ToList().Count()));
            }
            else
            {
                additionalParameters.Add(new KeyValuePair<string, object>("futureOccupancy", response.Where(f => f.ConversionDate != null && f.LeadType == AlmostConvertedId).ToList().Count()));
            }

            return DataTablesJsonResult(totalCount, request, returnResult, additionalParameters);
        }


        #endregion


        #region Estimate

        [HttpPost]
        public IActionResult Estimate(IDataTablesRequest request, int? BAUser, HomeReportingDays reportingDays)
        {
            int totalCount;
            int currentUserId = CurrentUser.Uid;
            bool isPMUser = CurrentUser.RoleId == (int)Enums.UserRoles.PM;
            bool isAllLeadUser = allLeadsUsers.Contains(Convert.ToString(currentUserId));
            bool IsUKAUUserIDToShowAshishTeamActivity = this.IsUKAUUserIDToShowAshishTeamActivity;

            var pagingServcices = request != null ? new PagingService<ProjectLead>(request.Start, request.Length) : new PagingService<ProjectLead>(0, int.MaxValue);


            var expr = PredicateBuilder.True<ProjectLead>().And(x => x.LeadStatu.StatusName == "Chase Request" || x.LeadStatu.StatusName == "Action Required From (Team)" || x.LeadStatu.StatusName == "Action Required From (Out of India PM)");

            if (IsUKAUUserIDToShowAshishTeamActivity)
            {
                expr = expr.And(x => x.PMID == currentUserId || x.PMID == SiteKey.AshishTeamPMUId);
            }
            else if (isPMUser)
            {
                expr = expr.And(x => x.PMID == currentUserId);
            }
            else
            {
                expr = expr.And(l => (l.PMID == PMUserId || l.AbroadPM.Uid == currentUserId) && (l.OwnerId == currentUserId || l.CommunicatorId == currentUserId || l.LeadTechnicians.Any(t => t.TechnicianId == currentUserId) || l.AbroadPM.Uid == currentUserId));
            }

            //expr = expr.Or(x => x.AbroadPM.Uid == currentUserId);
            expr = expr.And(x => x.ProjectClosureId == null);

            if (BAUser.HasValue)
            {
                expr = expr.And(b => b.OwnerId == BAUser.Value || b.CommunicatorId == BAUser.Value || b.LeadTechnicians.Any(x => x.TechnicianId == BAUser.Value));
            }

            switch (reportingDays)
            {
                case HomeReportingDays.Today:
                    {
                        var startDate = DateTime.Today;
                        expr = expr.And(L => L.NextChasedDate.Value <= startDate);
                    }
                    break;

                case HomeReportingDays.Tomorrow:
                    {
                        var startDate = DateTime.Today;
                        var endDate = startDate.AddDays(1);
                        expr = expr.And(L => L.NextChasedDate > startDate && L.NextChasedDate <= endDate);
                    }
                    break;

                case HomeReportingDays.Week:
                    {
                        //var startOfWeek = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek));
                        var startOfWeek = DateTime.Today;
                        var endOfWeek = startOfWeek.AddDays(7);
                        //expr = expr.And(L => L.NextChasedDate > startOfWeek && L.NextChasedDate <= endOfWeek);
                        expr = expr.And(L => L.NextChasedDate >= startOfWeek && L.NextChasedDate <= endOfWeek);
                    }
                    break;

                default:
                    {
                        //DateTime endOfWeek = DateTime.Today.AddDays(5 - (1 * (int)(DateTime.Today.DayOfWeek)));
                        //expr = expr.And(L => L.NextChasedDate > endOfWeek);
                    }
                    break;
            }

            pagingServcices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.ModifyDate);
            };

            pagingServcices.Filter = expr;

            List<ProjectLead> response = leadServices.GetLeadsByPaging(out totalCount, pagingServcices);

            string[] actionRequired = new string[] { "Action", "Chase" };

            var returnResult = response.Select((r, index) => new
            {
                LeadId = r.LeadId,
                rowIndex = (index + 1) + (request.Start),
                crmid = r.LeadCRMId ?? "",
                ClientId = r.LeadClientId.HasValue ? r.LeadClientId.ToString() : "",
                Client = r.LeadClient?.Name ?? "",
                LeadTitle = r.Title ?? "",
                NewClient = r.IsNewClient ? "New Client" : "Existing Client",
                AbroadPM = r.AbroadPM?.Country ?? "",
                GeneratedDate = r.AddDate.ToFormatDateString("MMM, dd yyyy hh:mm tt"),
                ModifiedDate = r.ModifyDate.ToFormatDateString("MMM, dd yyyy hh:mm tt"),
                ProposalDocument = r.ProposalDocument ?? "",
                Title = "",
                LastConversation = r.LeadTransactions.Any() ? r.LeadTransactions.OrderByDescending(x => x.AddDate).FirstOrDefault().Notes.TrimLength(100) : "",
                LastConversationFull = r.LeadTransactions.Any() ? r.LeadTransactions.OrderByDescending(x => x.AddDate).FirstOrDefault().Notes : "",
                showLastConversationInDiv = r.LeadTransactions.Any() ? r.LeadTransactions.OrderByDescending(x => x.AddDate).FirstOrDefault().Notes?.Length > 100 : false,
                EstimateTimeinDay = r.EstimateTimeinDay != null ? WeekAndDay(Convert.ToInt32(r.EstimateTimeinDay)).ToString() : "",
                OwnersName = r.UserLogin1.Name + "/" + r.UserLogin.Name,
                NextChaseDate = r.NextChasedDate.ToFormatDateString("MMM, dd yyyy") ?? "",
                Technologies = string.Join(", ", r.ProjectLeadTeches
                                                   .Where(x => x.Technology.Title.HasValue())
                                                   .Select(x => x.Technology.Title)
                                                   .Concat(r.Technologies.HasValue() ? r.Technologies.Split(',') : new string[] { })
                                                   .Distinct()),
                Source = r.AbroadPM?.Name ?? "",
                AssignedDate = r.AssignedDate.ToFormatDateString("MMM, dd yyyy hh:mm tt"),
                Status = r.LeadStatu?.StatusName ?? "",
                Remark = r.Remark ?? "",
                //AllowConclusionAndEdit = isPMUser || isAllLeadUser || r.OwnerId == currentUserId,
                //AllowClientAndDelete = isPMUser,
                //AllowAction = (isPMUser || isAllLeadUser || r.OwnerId == currentUserId) && (r.LeadStatu.StatusName.Contains("Action") || r.LeadStatu.StatusName.Contains("Chase"))
                AllowConclusionAndEdit = (isPMUser && !IsUKAUUserIDToShowAshishTeamActivity) || isAllLeadUser || r.OwnerId == currentUserId,
                AllowClientAndDelete = (isPMUser && !IsUKAUUserIDToShowAshishTeamActivity),
                //AllowAction = ((isPMUser && !IsUKAUUserIDToShowAshishTeamActivity) || isAllLeadUser || r.OwnerId == currentUserId) && (r.LeadStatu.StatusName.Contains("Action") || r.LeadStatu.StatusName.Contains("Chase"))
                AllowAction = (isPMUser || IsUKAUUserIDToShowAshishTeamActivity || isAllLeadUser || r.OwnerId == currentUserId) && (r.LeadStatu.StatusName.Contains("Action") || r.LeadStatu.StatusName.Contains("Chase"))
            });
            // return null;
            // change in to dataable json result
            return DataTablesJsonResult(totalCount, request, returnResult);
        }

        public string WeekAndDay(int noofday)
        {
            int weeks = noofday / 5;
            int days = noofday % 5;
            string result = ""; ;
            string weektypes = "Weeks";
            string daytypes = "Days";

            if (days == 1) { daytypes = "Day"; }
            if (weeks == 1) { weektypes = "Week"; }

            if (days == 0) { daytypes = ""; }
            if (weeks == 0) { weektypes = ""; }

            if (weeks == 0)
                result = days + " " + daytypes;
            if (days == 0)
                result = weeks.ToString() + " " + weektypes;
            if (weeks != 0 && days != 0)
                result = weeks.ToString() + " " + weektypes + "  " + days + " " + daytypes;
            if (weeks == 0 && days == 0)
                result = "";

            return result;
        }

        #endregion

        #region Project Closure

        [HttpPost]
        public IActionResult ProjectClosure(IDataTablesRequest request, int? BAUser, HomeReportingDays reportingDays)
        {
            var pagingServices = new PagingService<ProjectClosure>(request.Start, request.Length);
            var expr = PredicateBuilder.True<ProjectClosure>().And(x => x.CRMUpdated);
            bool isPMUser = CurrentUser.RoleId == (int)Enums.UserRoles.PM;
            int currentUserId = CurrentUser.Uid;

            bool IsUKAUUserIDToShowAshishTeamActivity = this.IsUKAUUserIDToShowAshishTeamActivity;
            if (IsUKAUUserIDToShowAshishTeamActivity)
            {
                expr = expr.And(x => x.PMID == currentUserId || x.PMID == SiteKey.AshishTeamPMUId);
            }
            else if (isPMUser)
            {
                expr = expr.And(x => x.PMID == currentUserId);
            }
            else
            {
                expr = expr.And(x => x.Uid_Dev == currentUserId ||
                                x.Uid_BA == currentUserId ||
                                x.Uid_TL == currentUserId ||
                                x.ProjectClosureAbroadPm.Select(a => a.AbroadPm.Uid).Contains(currentUserId)
                );
            }

            int projectStatusId = (int)Enums.CloserType.Pending;
            int DeadResponseId = (int)Enums.CloserType.DeadResponse;
            //expr = expr.And(x => x.Status == projectStatusId || x.Status == 0 || (x.Status == DeadResponseId && x.DeadResponseDate != null && x.DeadResponseDate <= DateTime.Today));
            expr = expr.And(x => x.Status == projectStatusId || x.Status == 0);

            if (BAUser.HasValue)
            {
                expr = expr.And(x => x.Uid_BA == BAUser);
            }

            switch (reportingDays)
            {
                case HomeReportingDays.Today:
                    {
                        var startDate = DateTime.Today;
                        expr = expr.And(L => L.NextStartDate <= DateTime.Now);
                    }
                    break;

                case HomeReportingDays.Tomorrow:
                    {
                        var startDate = DateTime.Today;
                        var endDate = DateTime.Now.AddDays(1);
                        expr = expr.And(L => L.NextStartDate > startDate && L.NextStartDate <= endDate);
                    }
                    break;

                case HomeReportingDays.Week:
                    {
                        //var startOfWeek = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek));
                        var startOfWeek = DateTime.Today;
                        var endOfWeek = startOfWeek.AddDays(7);
                        //expr = expr.And(L => L.NextStartDate > startOfWeek && L.NextStartDate <= endOfWeek);
                        expr = expr.And(L => L.NextStartDate >= startOfWeek && L.NextStartDate <= endOfWeek);
                    }
                    break;

                default:
                    {
                        //DateTime endOfWeek = DateTime.Today.AddDays(5 - (1 * (int)(DateTime.Today.DayOfWeek)));
                        //expr = expr.And(L => L.NextStartDate > endOfWeek);
                    }
                    break;
            }


            pagingServices.Filter = expr;

            pagingServices.Sort = (o) =>
            {
                return o.OrderBy(c => c.NextStartDate);
            };

            int totalCount = 0;

            var response = projectClosureService.GetProjectClosurePaging(out totalCount, pagingServices);
            //  return null;
            // change in to dataable json result

            return DataTablesJsonResult(totalCount, request, response.Select((p, index) => new
            {
                Id = p.Id,
                rowIndex = (index + 1) + (request.Start),
                ClientName = p.Project.Client?.Name ?? string.Empty,
                ProjectName = p.Project.Name,
                CRMProjectId = p.Project.CRMProjectId,
                PCountry = p.Country,
                EngagementDate = p.NextStartDate.ToFormatDateString("MMM d, yyyy"),
                BA = p.UserLogin?.Name,
                TL = p.UserLogin3?.Name,
                BaId = p.Uid_BA,
                TlId = p.Uid_TL,
                User = currentUserId,
                AddedBy = p.AddedBy,
                ClientQuality = p.ClientQuality.Value,
                Status = p.Status,
                CreatedDate = p.Created.ToFormatDateString("MMM d, yyyy hh:mm tt"),
                ModifyDate = p.Modified.ToFormatDateString("MMM d, yyyy hh:mm tt"),
                AllowUpdateStatus = p.AddedBy == currentUserId || p.Uid_BA == currentUserId || p.Uid_TL == currentUserId || IsUKAUUserIDToShowAshishTeamActivity,
                HasDeadResponse = (p.Status == (int)Enums.CloserType.DeadResponse && p.DeadResponseDate != null),
                DeadResponseDate = p.DeadResponseDate?.ToFormatDateString("MMM d, yyyy")
            }));
        }

        #endregion

        #region Invoice

        public IActionResult InvoiceList(IDataTablesRequest request)
        {
            int currentUserId = CurrentUser.Uid;

            var pagingServices = new PagingService<ProjectInvoice>(request.Start, request.Length);
            var expr = PredicateBuilder.True<ProjectInvoice>();
            //bool IsUKAUUserIDToShowAshishTeamActivity = this.IsUKAUUserIDToShowAshishTeamActivity;

            //if (IsUKAUUserIDToShowAshishTeamActivity)
            //{
            //    expr = expr.And(x => (x.PMID == CurrentUser.PMUid || x.PMID == SiteKey.AshishTeamPMUId) && x.InvoiceStatus != (int)ProjectInvoiceStatus.Paid && x.InvoiceStatus != (int)ProjectInvoiceStatus.Cancelled);
            //}
            //else
            if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
            {
                expr = expr.And(x => x.PMID == currentUserId && x.InvoiceStatus != (int)ProjectInvoiceStatus.Paid && x.InvoiceStatus != (int)ProjectInvoiceStatus.Cancelled);
            }
            else
            {
                expr = expr.And(x => x.PMID == PMUserId && (x.Uid_BA == currentUserId || x.Uid_TL == currentUserId) &&
                                     x.InvoiceStatus != (int)ProjectInvoiceStatus.Paid &&
                                     x.InvoiceStatus != (int)ProjectInvoiceStatus.Cancelled);
            }

            pagingServices.Filter = expr;

            pagingServices.Sort = (o) =>
            {
                return o.OrderBy(c => c.Modified);
            };

            int totalCount = 0;

            var response = projectInvoiceService.GetInvoicesByPaging(out totalCount, pagingServices);
            //return null;
            // change in to dataable json result
            return DataTablesJsonResult(totalCount, request, response.Select((i, index) => new
            {
                Id = i.Id,
                rowIndex = (index + 1) + (request.Start),
                ProjectId = i.ProjectId,
                CRMProjectId = $"CRM ID:[{i.Project?.CRMProjectId}] <br/> {i.Project?.Client?.Name}",
                ProjectName = $"<b>{i.Project?.Name}</b><br/>Last Activity: {i.Modified.ToFormatDateString("MMM d, yyyy  hh:mm tt")}",
                InvoiceNumber = i.InvoiceNumber,
                InvoiceDate = $"{i.InvoiceStartDate.ToFormatDateString("MMM d, yyyy")} /<br/> {i.InvoiceEndDate.ToFormatDateString("MMM d, yyyy")}",
                BAName = i.UserLogin?.Name,
                TLName = i.UserLogin1?.Name,
                InvoiceStatus = ((ProjectInvoiceStatus)i.InvoiceStatus).ToString(),
                Class = DatatableClass(i.Id),
                Role = ((UserRoles)(CurrentUser.RoleId)).ToString(),
                ShowChaseIcon = (i.Uid_BA == currentUserId || i.Uid_TL == currentUserId)
            }));
        }

        private string DatatableClass(int invoiceId)
        {
            string colorclass = "lightyellow";
            var objProjectInvoice = projectInvoiceService.GetInvoiceByID(invoiceId);
            if (objProjectInvoice != null)
            {
                if (DateTime.Now.Date > objProjectInvoice.InvoiceStartDate.Date && DateTime.Now.Date.Subtract(objProjectInvoice.InvoiceStartDate.Date).TotalDays >= 5)
                {
                    int DaysCnt = 0;
                    DateTime checkDate = DateTime.Now.Date;
                    DateTime startDate = objProjectInvoice.InvoiceStartDate.Date;
                    while (checkDate > startDate && DaysCnt < 5)
                    {
                        bool r = (checkDate.DayOfWeek == DayOfWeek.Sunday || checkDate.DayOfWeek == DayOfWeek.Saturday);
                        checkDate = checkDate.AddDays(-1);
                        if (!r)
                        {
                            DaysCnt += 1;
                        }
                    }
                    if (DaysCnt >= 5)
                    {
                        if (objProjectInvoice.ProjectInvoiceComments.Count(x => x.ChaseDate.HasValue && x.ChaseDate.Value.Date >= DateTime.Now.Date) > 0)
                        {
                            colorclass = "lightred";
                        }
                        else
                        {
                            colorclass = "darkred";
                        }
                    }
                }
            }
            return colorclass;
        }

        #endregion
        private ResponseRoot GetDataFromAPI(RequestObject request)
        {
            string url = SiteKey.PMSMemberListWithTimeLogServiceApiURL;
            ResponseRoot apiResult = null;
            if (SiteKey.IsLive)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
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
                }
                catch (Exception ex)
                {
                    apiResult.Message = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                }
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

        private void AssignPlannedAndActualHrs(ref HomeDto model, ResponseRoot apiResult)
        {
            List<ResponseObject> response = null;
            try
            {
                if (apiResult != null)
                {
                    response = apiResult != null && apiResult.ResponsePacket != null
                    ? apiResult.ResponsePacket.Where(x => x.MemberEmail == CurrentUser.EmailOffice).OrderBy(r => r.MemberEmail).ThenBy(r => r.ProjectName).ToList() : new List<ResponseObject>();

                    if (response != null)
                    {
                        List<string> timesPlanned = response.Select(r => r.PlanHourEMS).ToList();
                        List<string> timesActual = response.Select(r => r.ActualHoursEMS).ToList();

                        string TotalPlannedHoursFormatted = CalculateTotalHours(timesPlanned);

                        string TotalActualHoursFormatted = CalculateTotalHours(timesActual);

                        model.PlanHours = TotalPlannedHoursFormatted;
                        model.ActualHours = TotalActualHoursFormatted;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        //public ApprovedLeaveDto GetApprovedLeave()
        //{
        //    ApprovedLeaveDto dto = new ApprovedLeaveDto();
        //    try
        //    {
        //        DateTime now = DateTime.Now;
        //        var startDate = new DateTime(now.Year, now.Month, 1);
        //        var endDate = startDate.AddMonths(1).AddDays(-1);
        //        dto.CasualLeave = leaveService.GetApprovedLeaves(CurrentUser.Uid, (int)LeaveCategory.CasualLeave, startDate, endDate);
        //        dto.EarnedLeave = leaveService.GetApprovedLeaves(CurrentUser.Uid, (int)LeaveCategory.EarnedLeave, startDate, endDate);
        //        dto.LossPayLeave = leaveService.GetApprovedLeaves(CurrentUser.Uid, (int)LeaveCategory.UnpaidLeave, startDate, endDate);
        //        dto.SickLeave = leaveService.GetApprovedLeaves(CurrentUser.Uid, (int)LeaveCategory.SickLeave, startDate, endDate);
        //        dto.BereavementLeave = leaveService.GetApprovedLeaves(CurrentUser.Uid, (int)LeaveCategory.BereavementLeave, startDate, endDate);
        //        dto.WeddingLeave = leaveService.GetApprovedLeaves(CurrentUser.Uid, (int)LeaveCategory.WeddingLeave, startDate, endDate);
        //        dto.CompensatoryOff = leaveService.GetApprovedLeaves(CurrentUser.Uid, (int)LeaveCategory.CompensatoryOff, startDate, endDate);
        //        dto.LoyaltyLeave = leaveService.GetApprovedLeaves(CurrentUser.Uid, (int)LeaveCategory.LoyaltyLeave, startDate, endDate);
        //        dto.MaternityLeave = leaveService.GetApprovedLeaves(CurrentUser.Uid, (int)LeaveCategory.MaternityLeave, startDate, endDate);
        //        dto.PaternityLeave = leaveService.GetApprovedLeaves(CurrentUser.Uid, (int)LeaveCategory.PaternityLeave, startDate, endDate);

        //        return dto;
        //    }
        //    catch (Exception ex)
        //    {
        //        return dto;
        //    }
        //}
        //public LeaveTypesDto GetAllLeaveBalance()
        //{
        //    LeaveTypesDto dto = new LeaveTypesDto();
        //    try
        //    {
        //        var empAttendanceId = userLoginService.GetUserInfoByID(CurrentUser.Uid).AttendenceId;
        //        if (empAttendanceId.HasValue)
        //        {
        //            var monthYearValue = (DateTime.Now.Year * 12) + DateTime.Now.Month;
        //            System.Data.DataTable leaveBalance = levDetailsService.GetLeaveBalance(empAttendanceId, monthYearValue);
        //            var leaveBalanceList = DtoBinder(leaveBalance);
        //            foreach (var item in leaveBalanceList)
        //            {
        //                switch (item.LeaveName)
        //                {
        //                    case "Loss Of Pay":
        //                        dto.LossPayLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.UnpaidLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
        //                        dto.LossPayLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.UnpaidLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
        //                        break;
        //                    case "CL":
        //                        dto.CasualLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.CasualLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
        //                        dto.CasualLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.CasualLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
        //                        break;
        //                    case "Compensatory Off":
        //                        dto.CompensatoryOff_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.CompensatoryOff.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
        //                        dto.CompensatoryOff_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.CompensatoryOff.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
        //                        break;
        //                    case "Paternity Leave":
        //                        dto.PaternityLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.PaternityLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
        //                        dto.PaternityLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.PaternityLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
        //                        break;
        //                    case "Sick Leave":
        //                        dto.SickLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.SickLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
        //                        dto.SickLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.SickLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
        //                        break;
        //                    case "Maternity Leave":
        //                        dto.MaternityLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.MaternityLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
        //                        dto.MaternityLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.MaternityLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
        //                        break;
        //                    case "Earned Leave":
        //                        dto.EarnedLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.EarnedLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
        //                        dto.EarnedLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.EarnedLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
        //                        break;
        //                    case "Bereavement Leave":
        //                        dto.BereavementLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.BereavementLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
        //                        dto.BereavementLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.BereavementLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
        //                        break;
        //                    case "Wedding Leave":
        //                        dto.WeddingLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.WeddingLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
        //                        dto.WeddingLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.WeddingLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
        //                        break;
        //                    case "Loyalty Leave":
        //                        dto.LoyaltyLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.LoyaltyLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
        //                        dto.LoyaltyLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.LoyaltyLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
        //                        break;
        //                }
        //            }

        //        }
        //        return dto;
        //    }
        //    catch (Exception ex)
        //    {
        //        return dto;
        //    }
        //}
        //public CurrentLeaveDto GetCurrentLeaveBalance(LeaveTypesDto leaveTypes, ApprovedLeaveDto approvedLeave)
        //{
        //    CurrentLeaveDto dto = new CurrentLeaveDto();
        //    try
        //    {
        //        dto.CasualLeave = Convert.ToDouble(leaveTypes.CasualLeave_CB) - approvedLeave.CasualLeave;
        //        dto.LossPayLeave = Convert.ToDouble(leaveTypes.LossPayLeave_CB) - approvedLeave.LossPayLeave;
        //        dto.CompensatoryOff = Convert.ToDouble(leaveTypes.CompensatoryOff_CB) - approvedLeave.CompensatoryOff;
        //        dto.PaternityLeave = Convert.ToDouble(leaveTypes.PaternityLeave_CB) - approvedLeave.PaternityLeave;
        //        dto.SickLeave = Convert.ToDouble(leaveTypes.SickLeave_CB) - approvedLeave.SickLeave;
        //        dto.MaternityLeave = Convert.ToDouble(leaveTypes.MaternityLeave_CB) - approvedLeave.MaternityLeave;
        //        dto.EarnedLeave = Convert.ToDouble(leaveTypes.EarnedLeave_CB) - approvedLeave.EarnedLeave;
        //        dto.BereavementLeave = Convert.ToDouble(leaveTypes.BereavementLeave_CB) - approvedLeave.BereavementLeave;
        //        dto.WeddingLeave = Convert.ToDouble(leaveTypes.WeddingLeave_CB) - approvedLeave.WeddingLeave;
        //        dto.LoyaltyLeave = Convert.ToDouble(leaveTypes.LoyaltyLeave_CB) - approvedLeave.LoyaltyLeave;
        //        return dto;
        //    }
        //    catch (Exception ex)
        //    {
        //        return dto;
        //    }
        //}
        //public List<LeaveBalanceDetailsDto> DtoBinder(System.Data.DataTable data)
        //{
        //    List<LeaveBalanceDetailsDto> leaveBalancelist = new List<LeaveBalanceDetailsDto>();
        //    if (data.Rows.Count > 0)
        //    {
        //        foreach (DataRow dr in data.Rows)
        //        {
        //            leaveBalancelist.Add(new LeaveBalanceDetailsDto
        //            {
        //                EmpId = Convert.ToInt32(dr["EMPID"]),
        //                EmpName = dr["EMPNAME"].ToString(),
        //                LeaveName = dr["LEVNAME"].ToString(),
        //                OpeningBalance = Convert.ToDecimal(dr["OPENING_BALANCE"]),
        //                Allotted = Convert.ToDecimal(dr["ALLOTED"]),
        //                Lapsed = Convert.ToDecimal(dr["LAPSE"]),
        //                EnchaseDays = Convert.ToDecimal(dr["ENCHASEDAYS"]),
        //                LeaveAvailed = Convert.ToDecimal(dr["LEAVEAVAILED"]),
        //                ClosingBalance = Convert.ToDecimal(dr["CB"])
        //            });
        //        }
        //    }
        //    return leaveBalancelist;
        //}
    }
}