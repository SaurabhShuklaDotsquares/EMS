using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using EMS.Dto;
using EMS.Web.Code;
using EMS.Web.Code.LIBS;
using EMS.Web.LIBS;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static EMS.Core.Enums;
using EMS.Core;
using EMS.Website.Code.LIBS;

namespace EMS.Web.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class BaseController : Controller
    {
        #region "Public Properties"

        public ClaimsPrincipal LoggedinUser => HttpContext.User;

        public CustomPrincipal CurrentUser => new CustomPrincipal(HttpContext.User);

        public int PMUserId
        {
            get
            {
                if (this.CurrentUser.IsAuthenticated)
                {
                    return this.CurrentUser.RoleId == (int)UserRoles.PM || this.CurrentUser.RoleId == (int)UserRoles.PMO ? this.CurrentUser.Uid : this.CurrentUser.PMUid;
                }
                return 0;
            }
        }
        public bool IsDirector
        {
            get
            {
                if (this.CurrentUser.IsAuthenticated)
                {
                    return this.CurrentUser.RoleId == (int)UserRoles.Director;
                }
                return false;
            }
        }

        public bool IsPM
        {
            get
            {
                if (this.CurrentUser.IsAuthenticated)
                {
                    return this.CurrentUser.RoleId == (int)UserRoles.PM || this.CurrentUser.RoleId == (int)UserRoles.PMO;
                }
                return false;
            }
        }
        public bool IsPMEvent
        {
            get
            {
                if (this.CurrentUser.IsAuthenticated)
                {
                    return this.CurrentUser.RoleId == (int)UserRoles.PM || this.CurrentUser.RoleId == (int)UserRoles.PMO || this.CurrentUser.RoleId == (int)UserRoles.UKBDM;
                }
                return false;
            }
        }
        public bool IsAjaxRequest
        {
            get { return HttpContext.Request.IsAjaxRequest(); }
        }

        #endregion

        #region "Notificatons"

        private void ShowMessages(string title, string message, MessageType messageType, bool isCurrentView)
        {
            Notification model = new Notification
            {
                Heading = title,
                Message = message,
                Type = messageType
            };

            if (isCurrentView)
                this.ViewData.AddOrReplace("NotificationModel", model);
            else
            {
                TempData.Put("NotificationModel", model);


                //this.TempData["NotificationModel"] = JsonConvert.SerializeObject(model);
                //TempData.Keep("NotificationModel");
            }
        }

        protected void ShowErrorMessage(string title, string message, bool isCurrentView = true)
        {
            ShowMessages(title, message, MessageType.Danger, isCurrentView);
        }

        protected void ShowSuccessMessage(string title, string message, bool isCurrentView = true)
        {
            ShowMessages(title, message, MessageType.Success, isCurrentView);
        }

        protected void ShowWarningMessage(string title, string message, bool isCurrentView = true)
        {
            ShowMessages(title, message, MessageType.Warning, isCurrentView);
        }

        protected void ShowInfoMessage(string title, string message, bool isCurrentView = true)
        {
            ShowMessages(title, message, MessageType.Info, isCurrentView);
        }

        protected PartialViewResult MessagePartialView(string message, string title = "", MessageType messageType = MessageType.Danger)
        {
            ShowMessages(title, message, messageType, true);

            return PartialView("_Notification");
        }

        protected ViewResult CustomErrorView(string message, string title = "", MessageType messageType = MessageType.Danger)
        {
            ShowMessages(title, message, messageType, true);

            return View("CustomError");
        }

        #endregion

        #region "Authentication"

        public async Task CreateAuthenticationTicket(UserSessionDto user)
        {
            if (user != null)
            {
                var claims = new List<Claim>{
                        new Claim(ClaimTypes.PrimarySid, Convert.ToString(user.Uid)),
                        new Claim(ClaimTypes.Email, user.EmailOffice),
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(nameof(user.UserName), user.UserName),
                        new Claim(nameof(user.JobTitle), user.JobTitle),
                        new Claim(nameof(user.DeptId), Convert.ToString(user.DeptId)),
                        new Claim(nameof(user.RoleId), Convert.ToString(user.RoleId)),
                        new Claim(nameof(user.TLId), Convert.ToString(user.TLId)),
                        new Claim(nameof(user.EmailOffice), user.EmailOffice),
                        new Claim(nameof(user.MobileNumber), user.MobileNumber),
                        new Claim(nameof(user.PMUid), Convert.ToString(user.PMUid)),
                        new Claim(nameof(user.CRMUserId), Convert.ToString(user.CRMUserId)),
                        new Claim(nameof(user.IsSuperAdmin), Convert.ToString(user.IsSuperAdmin)),
                        new Claim(nameof(user.ApiPassword), user.ApiPassword ?? string.Empty),
                        new Claim(nameof(user.IsSPEG), Convert.ToString(user.IsSPEG)),
                        new Claim(nameof(user.AttendenceId), Convert.ToString(user.AttendenceId)),
                        new Claim(nameof(user.DesignationId), Convert.ToString(user.DesignationId)),
                        new Claim(nameof(user.DesignationName), Convert.ToString(user.DesignationName)),
                        new Claim(nameof(user.Gender), Convert.ToString(user.Gender)),
                        new Claim(nameof(user.IsAllowLeave), Convert.ToString(user.IsAllowLeave)),
                        new Claim(nameof(user.IsAllowWFH), Convert.ToString(user.IsAllowWFH)),
                        new Claim(nameof(user.NotificationMinute), Convert.ToString(user.NotificationMinute))

                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
            }
        }

        public async Task RemoveAuthentication()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        #endregion

        public PartialViewResult CreateModelStateErrors()
        {
            return PartialView("_ValidationSummary", ModelState.Values.SelectMany(x => x.Errors));
        }

        public RedirectToActionResult AccessDenied()
        {
            return RedirectToAction("accessdenied", "error");
        }

        //public RedirectToRouteResult AccessDenied()
        //{
        //    return RedirectToAction("accessdenied", "error");
        //}

        //#region "Serialization"

        public ActionResult NewtonSoftJsonResult(object data)
        {

            return Json(data);
            //return new JsonNetResult
            //{
            //    Data = data,
            //    JsonRequestBehavior = JsonRequestBehavior.AllowGet
            //};
        }

        public FileResult DownloadExcelFile<T>(List<T> source, string reportName, int isSubsheet, List<ExportExcelColumn> excelColumn = null)
        {
            var filename = reportName.Trim().Replace(" ", "_") + "_" + DateTime.Now.ToString("hh_mm_ss_tt") + ".xls";
            var memoryStream = GeneralMethods.ToExportToExcel(source, reportName, isSubsheet, excelColumn);

            return File(memoryStream.ToArray(), "application/vnd.ms-excel", filename);
        }

        #region "DataTables Response"

        public DataTablesJsonResult DataTablesJsonResult(int total, IDataTablesRequest request, IEnumerable<object> data)
        {
            var response = DataTablesResponse.Create(request, total, total, data);
            return new DataTablesJsonResult(response, true);

            //return new DataTablesJsonResult(response, JsonRequestBehavior.AllowGet);
        }

        public DataTablesJsonResult DataTablesJsonResult(int total, IDataTablesRequest request, IEnumerable<object> data, IDictionary<string, object> additionalParameters)
        {
            var response = DataTablesResponse.Create(request, total, total, data, additionalParameters);
            Configuration.Options.EnableResponseAdditionalParameters();
            return new DataTablesJsonResult(response, true);
        }

        #endregion

        public void WriteLogFile(string description)
        {
            //string filePath = $"{Path.Combine("~/Upload/logs")}\\EMSLog_{DateTime.Today.ToFormatDateString("dd-MM-yyyy")}.txt";
            string filePath = $"{Path.Combine("wwwroot/Upload/logs")}\\EMSLog_{DateTime.Today.ToFormatDateString("dd-MM-yyyy")}.txt";
            try
            {
                using (StreamWriter objWrite = System.IO.File.Exists(filePath) ? System.IO.File.AppendText(filePath) : System.IO.File.CreateText(filePath))
                {
                    objWrite.WriteLine($"{description}\n");

                    objWrite.Flush();
                    objWrite.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void WriteLogFilePMS(string description)
        {
            //string filePath = $"{Path.Combine("~/Upload/logs")}\\EMSLog_{DateTime.Today.ToFormatDateString("dd-MM-yyyy")}.txt";
            string filePath = $"{Path.Combine("wwwroot/Upload/logs")}\\PMSLog_{DateTime.Today.ToFormatDateString("dd-MM-yyyy")}.txt";
            try
            {
                using (StreamWriter objWrite = System.IO.File.Exists(filePath) ? System.IO.File.AppendText(filePath) : System.IO.File.CreateText(filePath))
                {
                    objWrite.WriteLine($"{description}\n");

                    objWrite.Flush();
                    objWrite.Close();
                }
            }
            catch (Exception ex)
            { }
        }

    }
}