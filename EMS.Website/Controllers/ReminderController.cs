using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Data.Model;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.Controllers;
using EMS.Web.LIBS;
using EMS.Web.Modals;
using EMS.Web.Models.Others;
using EMS.Website.Code.LIBS;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Rewrite.Internal.UrlActions;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.SqlServer.Server;
using NPOI.POIFS.FileSystem;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static EMS.Core.Enums;

namespace EMS.Website.Controllers
{
    public class ReminderController : BaseController
    {
        private readonly IReminderService _reminderService;
        private readonly IUserLoginService userLoginService;
        private readonly IDepartmentService DepartmentService;
        private readonly ILeaveService _leaveService;

        public ReminderController(IReminderService reminderService, IUserLoginService _userLoginService, IDepartmentService _DepartmentService, ILeaveService leaveService)
        {
            _reminderService = reminderService;
            userLoginService = _userLoginService;
            DepartmentService = _DepartmentService;
            _leaveService = leaveService;
        }

        #region For Reminder Popup
        [CustomActionAuthorization]
        public ActionResult Index()
        {
            int Minutes = 0;

            var meetingTitleByUser = _reminderService.ReminderList(CurrentUser.Uid);

            List<string> meetingTitleName = new List<string>();

            if (meetingTitleByUser != null && meetingTitleByUser.Count > 0)
            {
                foreach (var item in meetingTitleByUser)
                {
                    meetingTitleName.Add(item.Title + ", " + item.ReminderDate.Value.ToString("dd/MM/yyyy") + "," + item.Id);
                }

                DateTime currentTime = DateTime.Now;
                TimeSpan startTime = TimeSpan.FromHours(9);
                TimeSpan endTime = TimeSpan.FromHours(20); // 7 PM

                // Compare the time component of the current time with the specified range
                if (currentTime.TimeOfDay >= startTime && currentTime.TimeOfDay <= endTime)
                {
                    DateTime? lstEmailTime = _reminderService.GetLastEmailTimeByUId(CurrentUser.Uid);

                    if (lstEmailTime != null)
                    {
                        DateTime currentTimes = DateTime.Now;
                        TimeSpan timeDifference = (TimeSpan)(currentTimes - lstEmailTime);
                        int minutesDifference = (int)timeDifference.TotalMinutes;

                        if (CurrentUser.PMUid != null)
                        {
                            var _prefereces = _leaveService.GetPreferecesByPMUid(CurrentUser.PMUid);
                            if (_prefereces.ReviewNotificationMinutes != null)
                            {
                                Minutes = _prefereces.ReviewNotificationMinutes.Value;
                            }
                        }

                        if (minutesDifference > Minutes)
                        {
                            ReminderEmail(CurrentUser.Name, SiteKey.IsLive != true ? "jagatpal.tomar@dotsquares.com" : CurrentUser.EmailOffice, meetingTitleName);
                            if (CurrentUser != null && CurrentUser.Uid != 0)
                            {
                                var reminderList = _reminderService.GetReminderByUId(CurrentUser.Uid);

                                if (reminderList != null)
                                {
                                    foreach (var item in reminderList)
                                    {
                                        item.LastEmail = DateTime.Now;
                                        _reminderService.Save(item);
                                    }

                                }

                            }
                        }
                    }

                    else
                    {
                        ReminderEmail(CurrentUser.Name, SiteKey.IsLive != true ? "jagatpal.tomar@dotsquares.com" : CurrentUser.EmailOffice, meetingTitleName);
                        if (CurrentUser != null && CurrentUser.Uid != 0)
                        {
                            var reminderList = _reminderService.GetReminderByUId(CurrentUser.Uid);

                            if (reminderList != null)
                            {
                                foreach (var item in reminderList)
                                {
                                    item.LastEmail = DateTime.Now;
                                    _reminderService.Save(item);
                                }

                            }

                        }
                    }

                }
            }

            return Json(meetingTitleName);
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult AddEditReminder(int? reminderId)
        {


            ReminderDto reminderViewModel = new ReminderDto();


            if (reminderId != null)
            {
                Reminder reminder = _reminderService.GetReminderById(Convert.ToInt32(reminderId));
                reminderViewModel.Id = (int)reminder.Id;
                reminderViewModel.Title = reminder.Title;
                reminderViewModel.ReminderDate = reminder.ReminderDate.HasValue?reminder.ReminderDate.Value.ToFormatDateString("dd/MM/yyyy") : "";
                reminderViewModel.IsExcludeMe = reminder.IsExcludeMe;
                reminderViewModel.IsActive = false;


                var users = userLoginService.GetUsers(true);
                var Department = DepartmentService.GetActiveDepartments();
                reminderViewModel.PaticipantList = users.Select(n => new SelectListItem { Text = n.Name + ' ' + (n.Pmu != null ? '(' + n.Pmu.Name + ')' : ""), Value = n.Uid.ToString(), Selected = reminder.ReminderUser.Any(s => s.Uid == n.Uid) }).ToList();
                reminderViewModel.GroupList = Department.Select(n => new SelectListItem { Text = n.Name, Value = n.DeptId.ToString() }).ToList();
            }


            return PartialView("_AddEditReminder", reminderViewModel);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult AddEditReminder(ReminderDto reminderViewModel)
        {
            if (CurrentUser != null && CurrentUser.Uid != 0)
            {
                if (ModelState.IsValid)
                {
                    bool success = false;

                    if (reminderViewModel.Id != null && reminderViewModel.Id > 0)
                    {

                        Reminder reminder = new Reminder();
                        reminder = _reminderService.GetReminderById(Convert.ToInt32(reminderViewModel.Id));
                        reminder.Title = reminderViewModel.Title;
                        DateTime remDate;
                        bool isDate = DateTime.TryParseExact(reminderViewModel.ReminderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out remDate);
                        reminder.ReminderDate = remDate;
                        reminder.IsExcludeMe = reminderViewModel.IsExcludeMe;
                        if (reminderViewModel.IsActive == true)
                        {
                            reminder.IsActive = false;
                        }

                        if (reminder != null && reminder.ReminderUser != null && reminder.ReminderUser.Any())
                        {
                            // call delete loop service reminder.ReminderUser
                            _reminderService.DeleteCollection(reminder.ReminderUser.ToList());



                            if (reminderViewModel.Paticipants != null && reminderViewModel.Paticipants.Any())
                            {
                                foreach (var item in reminderViewModel.Paticipants)
                                {
                                    ReminderUser reminderUser = new ReminderUser();
                                    reminderUser.ReminderId = reminderViewModel.Id;
                                    reminderUser.Uid = item;
                                    reminder.ReminderUser.Add(reminderUser);
                                }

                            }



                        }

                        success = _reminderService.Save(reminder);
                        if (success)
                        {
                            ShowSuccessMessage("Success", "Reminder has been successfully updated.", false);
                        }
                        else
                        {
                            ShowErrorMessage("Error", "Failed..!!", false);

                        }
                    }
                }
            }

            return RedirectToAction("ReminderIndex", "Reminder");

        }

        #endregion

        #region Private Method
        public async Task<IActionResult> ReminderEmail(string Name, string EmailTo, List<string> reminderList)
        {

            var titleReminder = string.Empty;



            StringBuilder BodyContent = new StringBuilder();
            BodyContent.Append("<table style='width:100%;font-family:Arial; font-size:13px;' cellpadding='0' cellspacing='0'>");
            BodyContent.Append("<tr><td>Dear " + Name + ",<br /></td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");

            BodyContent.Append("<tr><td>Please check EMS reminders as per your schedules and take appropriate action accordingly.</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            if (reminderList != null && reminderList.Any())
            {
                foreach (var item in reminderList)
                {
                    BodyContent.Append("<tr><td><b>Title: </b>" + item.Split(',')[0] + "</td></tr>");
                    BodyContent.Append("<tr><td><b>Date: </b>" + item.Split(',')[1] + "</td></tr>");
                    BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                }
            }
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td>Thanks & Regards</td></tr>");
            BodyContent.Append("<tr><td><b>Team EMS</b></td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td><span style='font-weight:bold; font-family:Arial; font-size:11px; font-style:italic; color:gray;'>[Note : Please do not reply to this automated message; replies to this message are undeliverable. Contact your senior for any query.]</span></td></tr>");
            BodyContent.Append("</table>");

            FlexiMail objMail = new FlexiMail
            {
                From = SiteKey.From,
                To = EmailTo,
                CC = "",
                BCC = "",
                Subject = "EMS: Reminder(s)",
                MailBodyManualSupply = true,
                MailBody = BodyContent.ToString()
            };

            objMail.Send();
            return null;

        }

        #endregion
        [CustomActionAuthorization]
        public ActionResult ReminderIndex(ReminderDto model)
        {
            int currentUserUid = CurrentUser.Uid;
            var reminderIds = _reminderService.GetRecordsByUserId(currentUserUid).Select(x => x.Id).ToList();
            List<int> employeeIds = _reminderService.FindRecordsByParameters(reminderIds).Select(x => x.Uid.Value).ToList();

            var employeeList = new List<SelectListItem>();
            employeeList = userLoginService.GetReminderUserList(employeeIds).Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.Uid.ToString() }).ToList();

            employeeList.Insert(0, new SelectListItem() { Text = "Reminder With", Value = "0" });
            ViewBag.EmployeeList = employeeList;

            model.ActiveStatus = WebExtensions.GetSelectList<Enums.RunningPriority>();
            return View(model);

        }
        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult ReminderIndex(IDataTablesRequest request, int ActiveStatusId, int EmployeeId, ExpensesFilter expenseFilter)
        {
            var pagingServices = new PagingService<Reminder>(request.Start, request.Length);
            var expr = PredicateBuilder.True<Reminder>();

            //var filterExpr = GetExpenseFilterExpression(expenseFilter);
            //pagingServices.Filter = filterExpr;




            DateTime? startDate = expenseFilter.DateFrom.ToDateTime();
            DateTime? endDate = expenseFilter.DateTo.ToDateTime();

            if (startDate.HasValue && endDate.HasValue)
            {
                expr = expr.And(L => L.ReminderDate >= startDate && L.ReminderDate <= endDate);
            }
            else if (startDate.HasValue)
            {
                expr = expr.And(L => L.ReminderDate >= startDate);
            }
            else if (endDate.HasValue)
            {
                expr = expr.And(L => L.ReminderDate <= endDate);
            }

            if (ActiveStatusId > 0)
            {
                if (ActiveStatusId == 1)
                {
                    expr = expr.And(e => e.IsActive == true);
                }
                else
                {
                    expr = expr.And(e => e.IsActive == false);
                }

            }

            if (EmployeeId > 0)
            {
                expr = expr.And(e => e.ReminderUser.Any(x => x.Uid.Value == EmployeeId));
            }

            expr = expr.And(e => e.CreatedBy == CurrentUser.Uid);

            if (request.Search.Value.HasValue())
            {
                string searchValue = request.Search.Value.Trim().ToLower();
                expr = expr.And(x => x.Title != null && x.Title.ToLower().Contains(searchValue));
            }

            bool isPMUser = CurrentUser.RoleId == (int)Enums.UserRoles.PM;
            int currentUserId = CurrentUser.Uid;


            pagingServices.Filter = expr;
            //pagingServices.Sort = (o) =>
            //{
            //    foreach (var item in request.SortedColumns())
            //    {
            //        switch (item.Name)
            //        {
            //            case "ReminderDate":
            //                return o.OrderByColumn(item, c => c.ReminderDate);
            //        }
            //    }

            //    return o.OrderByDescending(c => c.CreateDate);
            //};


            pagingServices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.CreateDate);
            };
            TempData.Put("ExpensesPagingFilter", expenseFilter);
            int totalCount = 0;

            var response = _reminderService.GetReminderByPaging(out totalCount, pagingServices);

            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                rowIndex = (index + 1),
                Id = r.Id,
                Title = r.Title,
                ReminderDate = r.ReminderDate.ToFormatDateString("MMM dd, yyyy"),
                ReminderWith = ReminderUser(r.ReminderUser),
                RunningStatus = r.IsActive.Value ? "Running" : "Completed"
            }));


        }

        public string ReminderUser(ICollection<ReminderUser> ReminderUser)
        {
            string name = string.Empty;

            foreach (var item in ReminderUser)
            {
                name += userLoginService.GetUserInfoByID(item.Uid.Value).Name + ", ";
            }
            return name = name.Remove(name.LastIndexOf(','));
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Add(int? id)
        {

            ReminderDto reminderViewModel = new ReminderDto();

            var users = userLoginService.GetUsers(true);
            var Department = DepartmentService.GetActiveDepartments();

            reminderViewModel.PaticipantList = users.Select(n => new SelectListItem { Text = n.Name + ' ' + (n.Pmu != null ? '(' + n.Pmu.Name + ')' : ""), Value = n.Uid.ToString() }).ToList();
            reminderViewModel.GroupList = Department.Select(n => new SelectListItem { Text = n.Name, Value = n.DeptId.ToString() }).ToList();


            if (id != null)
            {
                Reminder reminder = _reminderService.GetReminderById(Convert.ToInt32(id));
                reminderViewModel.Id = (int)reminder.Id;
                reminderViewModel.Title = reminder.Title;
                reminderViewModel.ReminderDate = reminder.ReminderDate.HasValue ? reminder.ReminderDate.Value.ToFormatDateString("dd/MM/yyyy") : "";
                 

                reminderViewModel.IsExcludeMe = reminder.IsExcludeMe;
                if (reminder.IsActive == true)
                {
                    reminderViewModel.IsActive = false;
                }
                else
                {
                    reminderViewModel.IsActive = true;
                }



                var userss = userLoginService.GetUsers(true);
                var Departments = DepartmentService.GetActiveDepartments();
                reminderViewModel.PaticipantList = userss.Select(n => new SelectListItem { Text = n.Name + ' ' + (n.Pmu != null ? '(' + n.Pmu.Name + ')' : ""), Value = n.Uid.ToString(), Selected = reminder.ReminderUser.Any(s => s.Uid == n.Uid) }).ToList();
                reminderViewModel.GroupList = Departments.Select(n => new SelectListItem { Text = n.Name, Value = n.DeptId.ToString() }).ToList();

            }


            return PartialView("_Add", reminderViewModel);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult Add(ReminderDto reminderViewModel)
        {
            if (CurrentUser != null && CurrentUser.Uid != 0)
            {
                if (ModelState.IsValid)
                {
                    bool success = false;
                    Reminder reminder = new Reminder();
                    reminder = _reminderService.GetReminderById(Convert.ToInt32(reminderViewModel.Id));

                    if (reminder != null)
                    {
                        reminder.Title = reminderViewModel.Title;
                        //reminderViewModel.ReminderDate = reminder.ReminderDate;
                        DateTime remDate;
                        bool isDate = DateTime.TryParseExact(reminderViewModel.ReminderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out remDate);
                        reminder.ReminderDate = remDate;
                        reminder.IsExcludeMe = reminderViewModel.IsExcludeMe;
                        if (reminderViewModel.IsActive == true)
                        {
                            reminder.IsActive = false;
                        }

                        if (reminder != null && reminder.ReminderUser != null && reminder.ReminderUser.Any())
                        {
                            // call delete loop service reminder.ReminderUser
                            _reminderService.DeleteCollection(reminder.ReminderUser.ToList());

                            if (reminderViewModel.Paticipants != null && reminderViewModel.Paticipants.Any())
                            {
                                foreach (var item in reminderViewModel.Paticipants)
                                {
                                    ReminderUser reminderUser = new ReminderUser();
                                    reminderUser.ReminderId = reminderViewModel.Id;
                                    reminderUser.Uid = item;
                                    reminder.ReminderUser.Add(reminderUser);
                                }

                            }

                        }

                        success = _reminderService.Save(reminder);
                        if (success)
                        {
                            ShowSuccessMessage("Success", "Reminder has been successfully updated.", false);
                        }
                        else
                        {
                            ShowErrorMessage("Error", "Failed..!!", false);

                        }
                    }
                    else
                    {
                        reminder = new Reminder();
                        reminder.Title = reminderViewModel.Title;
                        DateTime remDate;
                        bool isDate = DateTime.TryParseExact(reminderViewModel.ReminderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out remDate);
                        reminder.ReminderDate = remDate;
                        //reminder.ReminderDate= DateTime.ParseExact(reminderViewModel.ReminderDate.ToString().Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        reminder.IsExcludeMe = reminderViewModel.IsExcludeMe;
                        reminder.CreatedBy = CurrentUser.Uid;
                        reminder.CreateDate = DateTime.Now;
                        reminder.IsActive = true;

                        if (reminderViewModel.Paticipants != null && reminderViewModel.Paticipants.Any())
                        {
                            foreach (var item in reminderViewModel.Paticipants)
                            {
                                ReminderUser reminderUser = new ReminderUser();
                                reminderUser.ReminderId = reminderViewModel.Id;
                                reminderUser.Uid = item;
                                reminder.ReminderUser.Add(reminderUser);
                            }

                        }

                        success = _reminderService.Save(reminder);
                        if (success)
                        {
                            ShowSuccessMessage("Success", "Reminder has been successfully created.", false);
                        }
                        else
                        {
                            ShowErrorMessage("Error", "Failed..!!", false);

                        }
                    }

                }
            }
            return RedirectToAction("ReminderIndex", "Reminder");
        }

        #region Delete Reminder

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this?",
                Size = Enums.ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Appraise?" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult delete(int id)
        {
            try
            {
                if (id > 0)
                {
                    Reminder reminder = new Reminder();
                    reminder = _reminderService.GetReminderById(Convert.ToInt32(id));

                    if (reminder != null)
                    {
                        if (reminder != null && reminder.ReminderUser != null && reminder.ReminderUser.Any())
                        {
                            _reminderService.DeleteCollection(reminder.ReminderUser.ToList());
                        }

                        _reminderService.Delete(id);

                        ShowSuccessMessage("Success", "Record deleted successfully.", false);
                        return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = "Reminder/ReminderIndex" , IsSuccess = true });
                    }

                }

                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = "Unable to delete this reminder" });
               
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = ex.Message });
            }
        }

        #endregion


        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult isComplete(int id)
        {
            try
            {
                if (id > 0)
                {
                    Reminder reminder = new Reminder();
                    reminder = _reminderService.GetReminderById(Convert.ToInt32(id));

                    if (reminder != null)
                    {
                        reminder.IsActive = reminder.IsActive.Value ? false : true;
                        _reminderService.Save(reminder);
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record updated successfully.", IsSuccess = true });
                    }

                }

                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = "Unable to update this reminder" });
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = ex.Message });
            }
        }

        private Expression<Func<Reminder, bool>> GetExpenseFilterExpression(ExpensesFilter expenseFilter)
        {
            var expr = PredicateBuilder.True<Reminder>();

            if (expenseFilter.DateFrom.HasValue())
            {
                var startDate = expenseFilter.DateFrom.ToDateTime();
                expr = expr.And(e => e.ReminderDate == startDate);

            }

            if (expenseFilter.DateTo.HasValue())
            {
                var endDate = expenseFilter.DateTo.ToDateTime();
                expr = expr.And(e => e.ReminderDate == endDate);
            }

            return expr;
        }
    }
}
