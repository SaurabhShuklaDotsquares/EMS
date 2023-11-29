using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.Models;
using EMS.Web.Models.Others;
using EMS.Website.Code.LIBS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
namespace EMS.Web.Controllers
{

    [CustomAuthorization]
    public class InvoiceController : BaseController {

        #region Constructor and Member

        private readonly IProjectInvoiceService projectInvoiceService;
        private readonly IUserLoginService userLoginService;
        private readonly IProjectService projectService;
        private readonly ICurrencyService currencyService;
        private readonly IPreferenceService prefrenceService;

        public InvoiceController(
            IProjectInvoiceService _projectInvoiceService,
            IUserLoginService _userLoginService,
            IProjectService _projectService,
            ICurrencyService _currencyService,
            IPreferenceService _prefrenceService) {
            projectInvoiceService = _projectInvoiceService;
            userLoginService = _userLoginService;
            projectService = _projectService;
            currencyService = _currencyService;
            prefrenceService = _prefrenceService;
        }

        #endregion

        #region  Invoice Index

        [HttpGet]
        [CustomActionAuthorization()]
        public ActionResult Index() {
            InvoiceFilterDto model = new InvoiceFilterDto();
            var userlist = userLoginService.GetUsersByPM(PMUserId);

            model.BAList = userlist.Where(x => RoleValidator.BA_RoleIds.Contains(x.RoleId.Value) || x.RoleId == (int)Enums.UserRoles.PM)
                .Select(x => new SelectListItem {
                    Text = x.Name,
                    Value = x.Uid.ToString()
                }).ToList();

            //model.TLList = userlist.Where(x => RoleValidator.TL_RoleIds.Contains(x.RoleId.Value) || RoleValidator.DV_RoleIds.Contains(x.RoleId.Value))
            model.TLList = userlist.Where(x => RoleValidator.TL_Technical_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(x.DesignationId.Value))
                .Select(x => new SelectListItem {
                    Text = x.Name,
                    Value = x.Uid.ToString()
                }).ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(IDataTablesRequest request, InvoiceFilterViewModel filter) {
            var pagingServices = GetPagingAndFilters(request, filter);
            int currentUserId = CurrentUser.Uid;
            bool isPMUser = CurrentUser.RoleId == (int)Enums.UserRoles.PM;
            TempData.Put("ProjectInvoiceFilter", filter);
            int totalCount = 0;

            var response = projectInvoiceService.GetInvoicesByPaging(out totalCount, pagingServices);

            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new {
                Id = r.Id,
                ProjectId = r.ProjectId,
                CRMProjectId = r.Project?.CRMProjectId,
                ClientName = r.Project?.Client?.Name,
                ProjectName = r.Project?.Name,
                LastActivity = r.Modified.ToFormatDateString("MMM d, yyyy  hh:mm tt"),
                InvoiceNumber = r.InvoiceNumber,
                InvoiceStartDate = r.InvoiceStartDate.ToFormatDateString("MMM d, yyyy"),
                InvoiceEndDate = r.InvoiceEndDate.ToFormatDateString("MMM d, yyyy"),
                BAName = r.UserLogin?.Name,
                TLName = r.UserLogin1?.Name,
                InvoiceStatus = ((Enums.ProjectInvoiceStatus)r.InvoiceStatus).GetDescription(),
                CssClass = GetInvoiceCssClass(r),
                ShowDelete = isPMUser && r.PMID == currentUserId,
                ShowChaseIcon = (r.Uid_BA == currentUserId || r.Uid_TL == currentUserId)
            }));
        }

        #endregion

        #region Add/Edit Invoice

        [HttpGet]
        [CustomActionAuthorization()]
        public ActionResult AddEditInvoice(int? id) {
            InvoiceDto model = new InvoiceDto();

            if (id.HasValue && id.Value > 0) {
                ProjectInvoice projectInvoice = projectInvoiceService.GetInvoiceByID(id.Value);
                if (projectInvoice != null && (projectInvoice.Uid_BA == CurrentUser.Uid || projectInvoice.Uid_TL == CurrentUser.Uid || projectInvoice.PMID == CurrentUser.Uid)) {
                    model.Id = projectInvoice.Id;
                    model.ProjectId = projectInvoice.ProjectId;
                    model.CurrencyID = projectInvoice.CurrencyID;
                    model.CountryId = projectInvoice.CountryId ?? 0;
                    model.InvoiceStatusId = projectInvoice.InvoiceStatus;
                    model.InvoiceNumber = projectInvoice.InvoiceNumber;
                    model.InvoiceAmount = projectInvoice.InvoiceAmount.ToString();
                    model.StartDate = projectInvoice.InvoiceStartDate.ToFormatDateString("dd/MM/yyyy");
                    model.EndDate = projectInvoice.InvoiceEndDate.ToFormatDateString("dd/MM/yyyy");
                    model.Comment = projectInvoice.ProjectInvoiceComments.FirstOrDefault()?.InvoiceComments;
                    model.Uid_BA = projectInvoice.Uid_BA;
                    model.Uid_TL = projectInvoice.Uid_TL;
                }
                else {
                    return MessagePartialView("Record not found or not accessible to current user");
                }
            }

            model.ProjectList = projectService.GetProjectListByPmuid(PMUserId, CurrentUser.Uid)
                .Select(x => new SelectListItem {
                    Text = x.Name,
                    Value = x.ProjectId.ToString()
                }).ToList();

            model.CurrencyList = currencyService.GetCurrency().Select(x => new SelectListItem {
                Text = x.CurrSign,
                Value = x.Id.ToString()
            }).ToList();

            var users = userLoginService.GetUsersByPM(PMUserId);

            //model.TLList = users.Where(x => RoleValidator.TL_RoleIds.Contains(x.RoleId.Value) || RoleValidator.DV_RoleIds.Contains(x.RoleId.Value))
            model.TLList = users.Where(x => RoleValidator.TL_Technical_DesignationIds.Contains(x.DesignationId.Value) 
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(x.DesignationId.Value) 
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(x.DesignationId.Value) 
            || RoleValidator.TL_UIUX_DesignationIds.Contains(x.DesignationId.Value) 
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(x.DesignationId.Value))
                .Select(x => new SelectListItem {
                    Text = x.Name,
                    Value = x.Uid.ToString()
                }).ToList();

            model.BAList = users.Where(x => RoleValidator.BA_RoleIds.Contains(x.RoleId.Value) || x.RoleId == (int)Enums.UserRoles.PM)
                .Select(x => new SelectListItem {
                    Text = x.Name,
                    Value = x.Uid.ToString()
                }).ToList();

            model.InvoiceStatus = WebExtensions.GetSelectList<Enums.ProjectInvoiceStatus>();
            model.ClientCountry = WebExtensions.GetSelectList<Enums.ClientCountry>();

            return PartialView("_AddEditInvoice", model);
        }

        [HttpPost]
        [CustomActionAuthorization()]
        public ActionResult AddEditInvoice(InvoiceDto model) {
            if (ModelState.IsValid) {
                try {
                    ProjectInvoice projectInvoice = null;
                    if (model.Id > 0) {
                        projectInvoice = projectInvoiceService.GetInvoiceByID(model.Id);

                        if (projectInvoice == null || (projectInvoice.Uid_BA != CurrentUser.Uid && projectInvoice.Uid_TL != CurrentUser.Uid && projectInvoice.PMID != CurrentUser.Uid)) {
                            return MessagePartialView("Record not found or not accessible to current user");
                        }
                    }
                    else {
                        projectInvoice = new ProjectInvoice {
                            Created = DateTime.Now,
                            PMID = PMUserId
                        };
                    }

                    projectInvoice.ProjectId = model.ProjectId;

                    projectInvoice.CountryId = model.CountryId;
                    projectInvoice.InvoiceStatus = model.InvoiceStatusId;
                    projectInvoice.InvoiceNumber = model.InvoiceNumber;
                    if (model.Id == 0) {
                        projectInvoice.InvoiceAmount = Convert.ToDecimal(model.InvoiceAmount);
                        projectInvoice.CurrencyID = model.CurrencyID;
                    }
                    projectInvoice.InvoiceStartDate = model.StartDate.ToDateTime("dd/MM/yyyy").Value;
                    projectInvoice.InvoiceEndDate = model.EndDate.ToDateTime("dd/MM/yyyy").Value;
                    projectInvoice.Uid_BA = model.Uid_BA;
                    projectInvoice.Uid_TL = model.Uid_TL;
                    projectInvoice.Comment = model.Comment;
                    projectInvoice.Modified = DateTime.Now;

                    projectInvoiceService.Save(projectInvoice);

                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = string.Format("Invoice {0} Successfully.", model.Id > 0 ? "Updated" : "Successfully"), IsSuccess = true });

                }
                catch (Exception ex) {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = ex.Message });
                }
            }
            return CreateModelStateErrors();
        }

        #endregion

        #region Chase Invoice

        [HttpGet]
        [CustomActionAuthorization()]
        public ActionResult ChaseInvoice(int id) {
            if (id > 0) {
                InvoiceChaseDto invoiceChaseDto = new InvoiceChaseDto { InvoiceId = id };
                return PartialView("_ChaseInvoice", invoiceChaseDto);
            }
            return MessagePartialView("Invalid Invoice Id");
        }

        [HttpPost]
        [CustomActionAuthorization()]
        public ActionResult ChaseInvoice(InvoiceChaseDto model) {
            try {
                if (ModelState.IsValid) {
                    model.CurrentUserId = CurrentUser.Uid;
                    var invoice = projectInvoiceService.SaveComment(model);
                    if (invoice != null) {
                        try {
                            FlexiMail objSendMail = new FlexiMail();
                            objSendMail.ValueArray = new string[]
                            {
                            CurrentUser.Name.ToTitleCase(),
                            invoice.Project?.CRMProjectId.ToString(),
                            invoice.Project?.Client?.Name ?? string.Empty,
                            invoice.Project?.Name,
                            ((Enums.ProjectInvoiceStatus)invoice.InvoiceStatus).GetDescription(),
                            invoice.InvoiceNumber,
                            $"{invoice.Currency.CurrSign}{invoice.InvoiceAmount.ToString()}",
                            $"{invoice.InvoiceStartDate.ToFormatDateString("MMM d, yyyy")}-{invoice.InvoiceEndDate.ToFormatDateString("MMM d, yyyy")}",
                            invoice.UserLogin?.Name,
                            invoice.UserLogin1?.Name,
                            model.ChaseDate.ToDateTime("dd/MM/yyyy").ToFormatDateString("MMM d, yyyy"),
                            model.Comment
                            };

                            var ObjPreference = prefrenceService.GetDataByPmid(PMUserId);
                            objSendMail.Subject = $"Chase Invoice - {invoice.Project.Name} [{invoice.Project.CRMProjectId}]";
                            objSendMail.MailBodyManualSupply = true;
                            objSendMail.MailBody = objSendMail.GetHtml("InvoiceChaseEmail.html");

                            if (ObjPreference != null && ObjPreference.ProjectClosureEmail.HasValue()) {
                                objSendMail.To = $"{ObjPreference.ProjectClosureEmail};{invoice.UserLogin.EmailOffice };{invoice.UserLogin1.EmailOffice}";
                            }
                            else {
                                objSendMail.To = $"{invoice.UserLogin.EmailOffice };{invoice.UserLogin1.EmailOffice}";
                            }

                            objSendMail.From = SiteKey.From;
                            objSendMail.Send();
                        }
                        catch {
                        }

                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Invoice Chased Successfully." });
                    }
                    else {
                        return MessagePartialView("Unable to save chnages");
                    }
                }
                else {
                    return CreateModelStateErrors();
                }
            }
            catch (Exception ex) {
                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = ex.Message });
            }
        }

        #endregion

        #region View Invoice

        [HttpGet]
        //[CustomActionAuthorization()]
        [CustomAuthorization()]
        public ActionResult ViewInvoice(int id) {
            if (id > 0) {
                InvoiceDto model = new InvoiceDto();
                ProjectInvoice projectInvoice = projectInvoiceService.GetInvoiceByID(id);
                if (projectInvoice != null && (projectInvoice.Uid_BA == CurrentUser.Uid || projectInvoice.Uid_TL == CurrentUser.Uid || projectInvoice.PMID == CurrentUser.Uid)
                    || (SiteKey.UKAUUserIDToShowAshishTeamActivity != null && SiteKey.UKAUUserIDToShowAshishTeamActivity.Split(',').ToList().Contains(CurrentUser.Uid.ToString()))
                    ) {
                    model.Id = projectInvoice.Id;
                    model.ProjectName = $"{projectInvoice?.Project?.Name} [{projectInvoice.Project?.CRMProjectId}]";
                    model.CountryId = projectInvoice.CountryId ?? 0;
                    model.ClientName = projectInvoice.Project?.Client?.Name;
                    model.Status = ((Enums.ProjectInvoiceStatus)projectInvoice.InvoiceStatus).ToString();
                    model.InvoiceNumber = projectInvoice.InvoiceNumber;
                    model.Amount = CurrentUser.RoleId == (int)Enums.UserRoles.PM ? $"{projectInvoice?.Currency?.CurrSign} {projectInvoice.InvoiceAmount}" : "";
                    model.StartDate = projectInvoice.InvoiceStartDate.ToFormatDateString("MMM d, yyyy");
                    model.EndDate = projectInvoice.InvoiceEndDate.ToFormatDateString("MMM d, yyyy");
                    model.BAName = projectInvoice.UserLogin?.Name ?? string.Empty;
                    model.TLName = projectInvoice.UserLogin1?.Name ?? string.Empty;
                    model.Created = projectInvoice.Created.ToFormatDateString("MMM d, yyyy");
                    model.Technologies = string.Join(", ", projectInvoice.Project.Project_Tech.Select(x => x.Technology.Title).Distinct());
                    model.ProjectInvoiceComments = projectInvoice.ProjectInvoiceComments.OrderByDescending(x => x.ChaseDate)
                                                                 .Select(x => new InvoiceComment {
                                                                     CommentDate = x.ChaseDate.ToFormatDateString("MMM d, yyyy"),
                                                                     Comment = x.InvoiceComments
                                                                 }).ToList();

                    return PartialView("_ViewDetails", model);
                }
            }

            return MessagePartialView("Record not found or not accessible to current user");
        }

        #endregion

        #region Delete Invoice 

        [HttpGet]
        [CustomActionAuthorization()]
        public ActionResult CancelInvoice() {
            return PartialView("_ModalDelete", new Modal {
                Message = "Are you sure you want to delete this Invoice?",
                Size = Enums.ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Invoice" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        [CustomActionAuthorization()]
        public ActionResult CancelInvoice(int id) {
            try {
                if (id > 0) {
                    ProjectInvoice projectInvoice = projectInvoiceService.GetInvoiceByID(id);
                    if (projectInvoice != null && projectInvoice.PMID == CurrentUser.Uid) {
                        projectInvoice.Modified = DateTime.Now;
                        projectInvoice.InvoiceStatus = (int)Enums.ProjectInvoiceStatus.Cancelled;
                        projectInvoiceService.UpdateStatus(projectInvoice);
                        ShowSuccessMessage("Success", "Invoice Deleted Successfully!!", false);
                    }
                }
                else {
                    ShowErrorMessage("Error", "Record not found or not accessible to current user", false);
                }
            }
            catch (Exception ex) {
                ShowErrorMessage("Error", ex.Message, false);
            }

            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("Index", "Invoice") });
        }

        #endregion

        #region Invoice Status

        [HttpGet]
        [CustomActionAuthorization()]
        public ActionResult InvoiceStatus(int invoiceId) {
            try {
                if (invoiceId > 0) {
                    ProjectInvoice projectInvoice = projectInvoiceService.GetInvoiceByID(invoiceId);
                    if (projectInvoice != null && (projectInvoice.Uid_BA == CurrentUser.Uid || projectInvoice.Uid_TL == CurrentUser.Uid || projectInvoice.PMID == CurrentUser.Uid)) {
                        InvoiceStatusDto model = new InvoiceStatusDto();
                        model.InvoiceStatus = WebExtensions.GetSelectList<Enums.ProjectInvoiceStatus>();
                        model.InvoiceId = projectInvoice.Id;
                        model.ProjectName = projectInvoice.Project?.Name;
                        model.CurrentStatus = ((Enums.ProjectInvoiceStatus)projectInvoice.InvoiceStatus).GetDescription();

                        return PartialView("_UpdateStatus", model);
                    }
                }

                return MessagePartialView("Record not found or not accessible to current user");
            }
            catch (Exception ex) {
                return MessagePartialView(ex.Message);
            }
        }

        [HttpPost]
        [CustomActionAuthorization()]
        public ActionResult InvoiceStatus(InvoiceStatusDto model) {
            try {
                if (ModelState.IsValid) {
                    ProjectInvoice projectInvoice = projectInvoiceService.GetInvoiceByID(model.InvoiceId);
                    if (projectInvoice != null && (projectInvoice.Uid_BA == CurrentUser.Uid || projectInvoice.Uid_TL == CurrentUser.Uid || projectInvoice.PMID == CurrentUser.Uid)) {
                        projectInvoice.InvoiceStatus = model.InvoiceStatusId;
                        projectInvoice.Modified = DateTime.Now;
                        if (model.InvoiceStatusId == 2) {
                            projectInvoice.InvoiceAmount = projectInvoice.InvoiceAmount - Convert.ToDecimal(model.RemainingAmount);
                        }
                        var result = projectInvoiceService.UpdateStatus(projectInvoice);

                        try {
                            FlexiMail objSendMail = new FlexiMail();
                            objSendMail.ValueArray = new string[]
                            {
                            CurrentUser.UserName.ToTitleCase(),
                            result.Project.CRMProjectId.ToString(),
                            result.Project?.Client?.Name??string.Empty,
                            result.Project?.Name,
                            ((Enums.ProjectInvoiceStatus)result.InvoiceStatus).GetDescription(),
                            result.InvoiceNumber,
                            $"{result.Currency.CurrSign}{result.InvoiceAmount.ToString()}",
                            $"{result.InvoiceStartDate.ToFormatDateString("MMM d, yyyy")}-{result.InvoiceEndDate.ToFormatDateString("MMM d, yyyy")}",
                            result.UserLogin?.Name,
                            result.UserLogin1?.Name
                            };

                            var ObjPreference = prefrenceService.GetDataByPmid(PMUserId);
                            objSendMail.Subject = $"Update Invoice Status- {result.Project.Name} [{result.Project.CRMProjectId}]";
                            objSendMail.MailBodyManualSupply = true;
                            objSendMail.MailBody = objSendMail.GetHtml("InvoiceUpdateStatus.html");

                            if (ObjPreference != null && ObjPreference.ProjectClosureEmail.HasValue()) {
                                objSendMail.To = $"{ObjPreference.ProjectClosureEmail};{result.UserLogin.EmailOffice };{result.UserLogin1.EmailOffice}";
                            }
                            else {
                                objSendMail.To = $"{result.UserLogin.EmailOffice };{result.UserLogin1.EmailOffice}";
                            }

                            objSendMail.From = SiteKey.From;
                            objSendMail.Send();
                        }
                        catch {

                        }

                        return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = true, Message = "Invoice Status Update Sucessfully." });
                    }
                    else {
                        return MessagePartialView("Record not found or not accessible to current user");
                    }
                }
                else {
                    return CreateModelStateErrors();
                }
            }
            catch (Exception ex) {
                return MessagePartialView(ex.InnerException?.Message ?? ex.Message);
            }
        }

        #endregion

        #region Export To Excel

        [HttpGet]
        public IActionResult ExportToExcel() {

            var filters = TempData.Get<InvoiceFilterViewModel>("ProjectInvoiceFilter");
            if (filters != null) {
                var pagingServices = GetPagingAndFilters(null, filters);
                var result = projectInvoiceService.GetInvoiceList(pagingServices);

                var reportname = $"ClientInvoiceReport_{DateTime.Now.ToFormatDateString("ddMMyyyy")}";
                var filename = reportname.Trim().Replace(" ", "_") + "_" + DateTime.Now.ToString("hh_mm_ss_tt") + ".xls";
                var memoryStream = ExportToProjectInvoice(result, reportname);

                return File(memoryStream, "application/vnd.ms-excel", filename);
            }
            return Json(new { Status = false, Message = "" });
        }

        #endregion

        #region Project Invoice List

        [HttpGet]
        [CustomActionAuthorization()]
        public ActionResult ProjectInvoiceList() {
            InvoiceFilterDto model = new InvoiceFilterDto();
            var userlist = userLoginService.GetUsersByPM(PMUserId);

            model.BAList = userlist.Where(x => RoleValidator.BA_RoleIds.Contains(x.RoleId.Value) || x.RoleId == (int)Enums.UserRoles.PM)
                .Select(x => new SelectListItem {
                    Text = x.Name,
                    Value = x.Uid.ToString()
                }).ToList();

            //model.TLList = userlist.Where(x => RoleValidator.TL_RoleIds.Contains(x.RoleId.Value)|| RoleValidator.DV_RoleIds.Contains(x.RoleId.Value))                        
            model.TLList = userlist.Where(x => RoleValidator.TL_Technical_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(x.DesignationId.Value))
                .Select(x => new SelectListItem {
                    Text = x.Name,
                    Value = x.Uid.ToString()
                }).ToList();

            return View(model);
        }

        [HttpPost]
        [CustomActionAuthorization()]
        public IActionResult ProjectInvoiceList(IDataTablesRequest request) {
            try {
                                int CrmUserid = 0; string ApiPass = ""; string InvoiceDate = ""; int totalCount = 0;

                UserLogin objUser = userLoginService.GetUserInfoByID(PMUserId);

                if (objUser != null) {
                    CrmUserid = objUser.CRMUserId != null ? objUser.CRMUserId.Value : 0;
                    ApiPass = objUser.ApiPassword.HasValue() ? objUser.ApiPassword : "";
                    InvoiceDate = DateTime.Now.AddDays(Convert.ToInt32(SiteKey.InvoiceDays)).ToFormatDateString("yyyy-MM-dd");
                }

                var httpWebRequest = WebRequest.CreateHttp(SiteKey.CrmInvoices);
                StringBuilder postData = new StringBuilder();
                postData.Append($"userid={CrmUserid}&");
                postData.Append($"apipass={ApiPass}&");
                postData.Append($"type={"invoices"}&");
                postData.Append($"date={InvoiceDate}");

                httpWebRequest.Headers.Add("userid", SiteKey.CRMApiUser);
                httpWebRequest.Headers.Add("password", SiteKey.CRMApiPassword);

                var data = Encoding.ASCII.GetBytes(postData.ToString());

                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.ContentLength = data.Length;

                using (var stream = httpWebRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)(httpWebRequest.GetResponse());

                string responseData = "";
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    if (responseData.HasValue())
                    {
                            //string jsonString = new WebClient().DownloadString(SiteKey.CrmInvoiceList.ToString().Replace("@USERID", CrmUserid.ToString()).Replace("@APIPASSWORD", ApiPass).Replace("@INVOICEDATE", InvoiceDate));

                            JObject obj = JObject.Parse(responseData);
                            var token = (JArray)obj.SelectToken("Data.invoiceInfo.response");
                            string json = null;

                            foreach (var item in token)
                            {
                                json = JsonConvert.SerializeObject(item.SelectToken("result"));
                            }
                            List<FinalInvoiceList> objFinalInvoiceList = new List<FinalInvoiceList>();
                            if (json != null && json != "null")
                            {
                                //JavaScriptSerializer jss = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
                                //List<Invoicejson> InvoiceList = jss.Deserialize<List<Invoicejson>>(json).ToList();
                                List<Invoicejson> InvoiceList = JsonConvert.DeserializeObject<List<Invoicejson>>(json).ToList();
                                if (InvoiceList != null && InvoiceList.Count > 0)
                                {
                                    var invoices = projectInvoiceService.GetDataByInvoiceNumbers(PMUserId, InvoiceList.Where(x => x.invoice_id.HasValue()).Select(x => x.invoice_id));
                                    foreach (Invoicejson objNewInvoice in InvoiceList)
                                    {
                                        ProjectInvoice invoiceObj = invoices.FirstOrDefault(x => x.InvoiceNumber == objNewInvoice.invoice_id);
                                   
                                        int PaymentStatusID = objNewInvoice.status == "0" ? (int)Enums.ProjectInvoiceStatus.Cancelled : objNewInvoice.payment_status_id == "1" ?
                                            (int)Enums.ProjectInvoiceStatus.Paid : objNewInvoice.payment_status_id == "3" ?
                                            (int)Enums.ProjectInvoiceStatus.PartialPayment : objNewInvoice.payment_status_id == "5" ?
                                            (int)Enums.ProjectInvoiceStatus.WaitingApproval : objNewInvoice.payment_status_id == "4" ?
                                            (int)Enums.ProjectInvoiceStatus.BadDebt : (int)Enums.ProjectInvoiceStatus.Pending;

                                        if (invoiceObj != null)
                                        {
                                            if (invoiceObj.InvoiceAmount != Convert.ToDecimal(objNewInvoice.total_amount) || invoiceObj.InvoiceStatus != PaymentStatusID)
                                            {
                                                try
                                                {
                                                    FinalInvoiceList objFinalInvoice = new FinalInvoiceList();
                                                    objFinalInvoice.crm_id = Convert.ToInt32(objNewInvoice.crm_id);
                                                    objFinalInvoice.invoice_id = objNewInvoice.invoice_id;
                                                    objFinalInvoice.start_date = Convert.ToDateTime(objNewInvoice.start_date);
                                                    objFinalInvoice.end_date = Convert.ToDateTime(objNewInvoice.end_date);
                                                    objFinalInvoice.total_amount = Convert.ToDecimal(objNewInvoice.total_amount);
                                                    objFinalInvoice.currency = objNewInvoice.currency;
                                                    objFinalInvoice.payment_status_id = PaymentStatusID;
                                                    objFinalInvoice.status = Convert.ToInt32(objNewInvoice.status);
                                                    objFinalInvoice.ProjectID = invoiceObj.Project.ProjectId;
                                                    objFinalInvoice.ProjectName = invoiceObj.Project.Name;
                                                    //objFinalInvoice.ClientName = invoiceObj.ClientName;
                                                    objFinalInvoice.ProjectInvoiceID = invoiceObj.Id;
                                                    objFinalInvoice.BA_ID = invoiceObj.Uid_BA;
                                                    objFinalInvoice.TL_ID = invoiceObj.Uid_TL;
                                                    objFinalInvoice.CurrencyID = invoiceObj.CurrencyID;
                                                    objFinalInvoice.CurrencySign = invoiceObj.Currency.CurrSign;
                                                    objFinalInvoice.payment_status = ((Enums.ProjectInvoiceStatus)PaymentStatusID).ToString();
                                                    objFinalInvoiceList.Add(objFinalInvoice);
                                                }
                                                catch { }

                                            }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                Project objProject = projectService.GetProjectByCRMId(Convert.ToInt32(objNewInvoice.crm_id));
                                                Currency objCurrency = currencyService.GetCurrencyByName(objNewInvoice.currency);
                                                if (objProject != null && objCurrency != null)
                                                {
                                                    FinalInvoiceList objFinalInvoice = new FinalInvoiceList();
                                                    objFinalInvoice.crm_id = Convert.ToInt32(objNewInvoice.crm_id);
                                                    objFinalInvoice.invoice_id = objNewInvoice.invoice_id;
                                                    objFinalInvoice.start_date = Convert.ToDateTime(objNewInvoice.start_date);
                                                    objFinalInvoice.end_date = Convert.ToDateTime(objNewInvoice.end_date);
                                                    objFinalInvoice.total_amount = Convert.ToDecimal(objNewInvoice.total_amount);
                                                    objFinalInvoice.currency = objNewInvoice.currency;
                                                    objFinalInvoice.payment_status_id = PaymentStatusID;
                                                    objFinalInvoice.status = Convert.ToInt32(objNewInvoice.status);
                                                    objFinalInvoice.ProjectID = objProject.ProjectId;
                                                    objFinalInvoice.ProjectName = objProject.Name;
                                                    objFinalInvoice.ClientName = objProject.Client?.Name ?? "";
                                                    objFinalInvoice.ProjectInvoiceID = 0;
                                                    objFinalInvoice.CurrencyID = objCurrency.Id;
                                                    objFinalInvoice.CurrencySign = objCurrency.CurrSign;
                                                    objFinalInvoice.payment_status = ((Enums.ProjectInvoiceStatus)PaymentStatusID).ToString();
                                                    objFinalInvoiceList.Add(objFinalInvoice);
                                                }
                                            }
                                            catch { }
                                        }
                                    }
                                }
                            }
                            return DataTablesJsonResult(totalCount, request, objFinalInvoiceList.Select((r, index) => new {
                                rowIndex = index + 1,
                                Id = r.invoice_id,
                                ProjectInvoiceId = r.ProjectInvoiceID,
                                ProjectId = r.ProjectID,
                                ClientName = r.ClientName,
                                StartDate = r.start_date,
                                EndDate = r.end_date,
                                CurrencyID = r.CurrencyID,
                                CurrencySign = r.CurrencySign,
                                ProjectName = $" {r.ProjectName} <br/> CRM ID:[{r.crm_id}] <br/> Client Name : {r.ClientName}",
                                InvoiceNumber = r.invoice_id,
                                InvoiceDate = $"{r.start_date.ToFormatDateString("MMM d, yyyy")} / {r.end_date.ToFormatDateString("MMM d, yyyy")}",
                                InvoiceAmount = r.total_amount,
                                InvoiceStatus = r.payment_status,
                                BAId = r.BA_ID,
                                TLId = r.TL_ID,
                                Action = r.ProjectInvoiceID == 0 ? "Add" : "Update"
                            }));
                        
                    }
                    else
                    {
                        WriteLogFile($"(InvoiceController/ProjectInvoiceList) Response : No response from API");
                        return NewtonSoftJsonResult(
                            new RequestOutcome<string> { ErrorMessage = "No response from API", IsSuccess = false, Data = "" });
                    }
                }
                else
                {
                    WriteLogFile($"(InvoiceController/ProjectInvoiceList) Error Response : Code = {response.StatusCode} Description = {response.StatusDescription}");
                    return NewtonSoftJsonResult(
                            new RequestOutcome<string> { ErrorMessage = "Error on page", IsSuccess = false, Data = "" });
                }


                

            }
            catch(Exception ex) {
                WriteLogFile($"(InvoiceController/ProjectInvoiceList) Exception : {ex.InnerException?.InnerException?.Message ?? ex.Message})");
                return null;
            }
        }

        [HttpPost]
        [CustomActionAuthorization()]
        public ActionResult UpdateCrmInvoice(List<FinalInvoiceList> model) {
            try {
                if (model != null) {
                    Guid TransId = Guid.NewGuid();
                    int pmuid = PMUserId;
                    model = model.FindAll(x => x.BA_ID.HasValue && x.BA_ID.Value > 0 && x.TL_ID.HasValue && x.TL_ID.Value > 0);

                    foreach (FinalInvoiceList item in model) {
                        try {
                            FinalInvoiceList finalInvoiceList = new FinalInvoiceList();

                            finalInvoiceList.ProjectInvoiceID = Convert.ToInt32(item.ProjectInvoiceID);
                            finalInvoiceList.ProjectID = Convert.ToInt32(item.ProjectID);
                            finalInvoiceList.ClientName = Convert.ToString(item.ClientName);
                            finalInvoiceList.invoice_id = Convert.ToString(item.invoice_id);
                            finalInvoiceList.start_date = Convert.ToDateTime(item.start_date);
                            finalInvoiceList.end_date = Convert.ToDateTime(item.end_date);
                            finalInvoiceList.total_amount = Convert.ToDecimal(item.invoice_amount);
                            finalInvoiceList.payment_status_id = (int)((Enums.ProjectInvoiceStatus)Enum.Parse(typeof(Enums.ProjectInvoiceStatus), item.invoice_payment_status_id));
                            finalInvoiceList.CurrencyID = Convert.ToInt32(item.CurrencyID);
                            finalInvoiceList.BA_ID = Convert.ToInt32(item.BA_ID);
                            finalInvoiceList.TL_ID = Convert.ToInt32(item.TL_ID);

                            if (item.ProjectInvoiceID.Value == 0) {
                                ProjectInvoice invoiceObj = projectInvoiceService.CheckValidProject(Convert.ToInt32(item.ProjectID), item.invoice_id);
                                if (invoiceObj != null) {
                                    finalInvoiceList.ProjectInvoiceID = Convert.ToInt32(invoiceObj.InvoiceNumber.ToString());
                                }
                            }
                            if (finalInvoiceList.ProjectInvoiceID == 0) {
                                ProjectInvoice objProjectInvoice = new ProjectInvoice {
                                    ProjectId = Convert.ToInt32(finalInvoiceList.ProjectID),
                                    //ClientName = hdClientName.Value,
                                    InvoiceStatus = (int)((Enums.ProjectInvoiceStatus)Enum.Parse(typeof(Enums.ProjectInvoiceStatus), item.invoice_payment_status_id)),
                                    InvoiceNumber = finalInvoiceList.invoice_id,
                                    InvoiceStartDate = finalInvoiceList.start_date,
                                    InvoiceEndDate = finalInvoiceList.end_date,
                                    Uid_BA = finalInvoiceList.BA_ID != 0 ? (int?)Convert.ToInt32(finalInvoiceList.BA_ID) : null,
                                    Uid_TL = finalInvoiceList.TL_ID != 0 ? (int?)Convert.ToInt32(finalInvoiceList.TL_ID) : null,
                                    InvoiceAmount = Convert.ToDecimal(item.invoice_amount),
                                    Created = DateTime.Now,
                                    Modified = DateTime.Now,
                                    CurrencyID = finalInvoiceList.CurrencyID,
                                    PMID = pmuid
                                };
                                projectInvoiceService.Save(objProjectInvoice);
                            }
                            else {
                                ProjectInvoice invoiceObj = projectInvoiceService.GetInvoiceByID(finalInvoiceList.ProjectInvoiceID.Value);
                                invoiceObj.InvoiceAmount = Convert.ToDecimal(item.invoice_amount);
                                invoiceObj.InvoiceStatus = (int)((Enums.ProjectInvoiceStatus)Enum.Parse(typeof(Enums.ProjectInvoiceStatus), item.invoice_payment_status_id));
                                invoiceObj.Uid_BA = finalInvoiceList.BA_ID != 0 ? (int?)Convert.ToInt32(finalInvoiceList.BA_ID) : null;
                                invoiceObj.Uid_TL = finalInvoiceList.TL_ID != 0 ? (int?)Convert.ToInt32(finalInvoiceList.TL_ID) : null;
                                invoiceObj.Modified = DateTime.Now;
                                projectInvoiceService.Save(invoiceObj);
                            }
                        }
                        catch { }
                    }
                    return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = true, RedirectUrl = Url.Action("ProjectInvoiceList"), Message = "Invoice Updated Successfully" });
                }
                else {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "No invoice found to update" });
                }
            }
            catch (Exception ex) {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = ex.InnerException?.Message ?? ex.Message, RedirectUrl = Url.Action("ProjectInvoiceList") });
            }
        }

        #endregion

        #region  ExtraMethods  

        private PagingService<ProjectInvoice> GetPagingAndFilters(IDataTablesRequest request, InvoiceFilterViewModel filter) {
            var pagingServices = new PagingService<ProjectInvoice>(request?.Start ?? 0, request?.Length ?? int.MaxValue);

            var expr = PredicateBuilder.True<ProjectInvoice>().And(x =>
                        x.InvoiceStatus != (int)Enums.ProjectInvoiceStatus.Paid &&
                        x.InvoiceStatus != (int)Enums.ProjectInvoiceStatus.Cancelled);

            int currentUserId = CurrentUser.Uid;
            bool isPMUser = CurrentUser.RoleId == (int)Enums.UserRoles.PM;

            if (isPMUser) {
                expr = expr.And(x => x.PMID == currentUserId);
            }
            else {
                expr = expr.And(x => (x.Uid_BA == currentUserId || x.Uid_TL == currentUserId));
            }

            if (filter.BAId > 0) {
                expr = expr.And(x => x.Uid_BA == filter.BAId);
            }

            if (filter.TLId > 0) {
                expr = expr.And(e => e.Uid_TL == filter.TLId);
            }

            if (filter.Name.HasValue()) {
                filter.Name = filter.Name.Trim().ToLower();
                expr = expr.And(e => e.Project.Name.ToLower().Contains(filter.Name) || e.InvoiceNumber.Trim() == filter.Name);
            }

            if (filter.FromDate.HasValue()) {
                var startDate = filter.FromDate.ToDateTime();
                expr = expr.And(e => e.InvoiceStartDate >= startDate);
            }

            if (filter.ToDate.HasValue()) {
                var endDate = filter.ToDate.ToDateTime();
                expr = expr.And(e => e.InvoiceEndDate <= endDate);
            }

            pagingServices.Filter = expr;
            pagingServices.Sort = o => o.OrderByDescending(c => c.Modified);

            return pagingServices;
        }

        private string GetInvoiceCssClass(ProjectInvoice projectInvoice) {
            string colorclass = "lightyellow";
            if (projectInvoice != null) {
                if (DateTime.Now.Date > projectInvoice.InvoiceStartDate.Date && DateTime.Now.Date.Subtract(projectInvoice.InvoiceStartDate.Date).TotalDays >= 5) {
                    int DaysCnt = 0;
                    DateTime checkDate = DateTime.Now.Date;
                    DateTime startDate = projectInvoice.InvoiceStartDate.Date;
                    while (checkDate > startDate && DaysCnt < 5) {
                        bool r = (checkDate.DayOfWeek == DayOfWeek.Sunday || checkDate.DayOfWeek == DayOfWeek.Saturday);
                        checkDate = checkDate.AddDays(-1);
                        if (!r) {
                            DaysCnt += 1;
                        }
                    }

                    if (DaysCnt >= 5) {
                        if (projectInvoice.ProjectInvoiceComments.Any(x => x.ChaseDate.HasValue && x.ChaseDate.Value.Date >= DateTime.Now.Date)) {
                            colorclass = "lightred";
                        }
                        else {
                            colorclass = "darkred";
                        }
                    }
                }

            }
            return colorclass;
        }

        #endregion

        private MemoryStream ExportToProjectInvoice(List<ProjectInvoice> source, string reportname) {
            MemoryStream response = new MemoryStream();
            if (source.Any()) {
                var workbook = new HSSFWorkbook();
                var headerLabelCellStyle = workbook.CreateCellStyle();
                headerLabelCellStyle.Alignment = HorizontalAlignment.LEFT;
                headerLabelCellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                headerLabelCellStyle.BorderBottom = CellBorderType.THIN;
                headerLabelCellStyle.WrapText = true;

                var headerLabelFont = workbook.CreateFont();
                headerLabelFont.Boldweight = (short)FontBoldWeight.BOLD;
                headerLabelFont.Color = HSSFColor.BLACK.index;
                headerLabelCellStyle.SetFont(headerLabelFont);

                var headerCellStyle = workbook.CreateCellStyle();
                headerCellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                headerLabelFont.FontHeightInPoints = 11;
                headerCellStyle.SetFont(headerLabelFont);


                var InfoLabelCellStyle = workbook.CreateCellStyle();

                InfoLabelCellStyle.Alignment = HorizontalAlignment.CENTER;
                InfoLabelCellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                //InfoLabelCellStyle.BorderBottom = CellBorderType.THIN;
                InfoLabelCellStyle.WrapText = false;
                InfoLabelCellStyle.FillForegroundColor = HSSFColor.YELLOW.index;
                InfoLabelCellStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
                var infoLabelFont = workbook.CreateFont();
                infoLabelFont.Boldweight = (short)FontBoldWeight.BOLD;
                infoLabelFont.Color = HSSFColor.BLACK.index;
                infoLabelFont.FontHeightInPoints = 14;
                InfoLabelCellStyle.SetFont(infoLabelFont);

                var sheet = workbook.CreateSheet(reportname);
                var attendeeLabelCellStyle = workbook.CreateCellStyle();

                attendeeLabelCellStyle.Alignment = HorizontalAlignment.LEFT;
                attendeeLabelCellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                attendeeLabelCellStyle.BorderBottom = CellBorderType.THIN;
                attendeeLabelCellStyle.WrapText = true;
                attendeeLabelCellStyle.FillForegroundColor = HSSFColor.GREY_50_PERCENT.index;
                attendeeLabelCellStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
                var attendeeLabelFont = workbook.CreateFont();
                attendeeLabelFont.Boldweight = (short)FontBoldWeight.BOLD;
                attendeeLabelFont.Color = HSSFColor.WHITE.index;
                attendeeLabelCellStyle.SetFont(attendeeLabelFont);

                sheet.CreateRow(0);
                var headerInfo = sheet.CreateRow(1);
                headerInfo.CreateCell(3, CellType.STRING).SetCellValue("Unpaid Invoice Report as on " + DateTime.Now.ToString("dd MMM yyyy, dddd"));
                headerInfo.GetCell(3).CellStyle = InfoLabelCellStyle;
                NPOI.SS.Util.CellRangeAddress cra = new NPOI.SS.Util.CellRangeAddress(1, 1, 3, 7);
                sheet.AddMergedRegion(cra);
                sheet.CreateRow(2);
                //Create a header row
                var headerRow = sheet.CreateRow(3);
                int counter = 0;
                //Set the column names in the header row 
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Sno");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("CRMID");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Project Name");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Client Name");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Invoice Number");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Status");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Invoice Amount");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Start Date");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("End Date");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Next Chase Date");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Last Conversation");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("BA Name");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("TL Name");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Comment History");
                counter = 0;

                for (int i = 0; i < headerRow.LastCellNum; i++) {
                    headerRow.GetCell(i).CellStyle = attendeeLabelCellStyle;
                    headerRow.Height = 600;
                }

                //(Optional) freeze the header row so it is not scrolled
                sheet.CreateFreezePane(0, 4, 0, 4);

                int rowNumber = 4;



                //cellStyle.setFillForegroundColor(myColor);

                var rowRedCellStyleattendeeothercolumn = workbook.CreateCellStyle();
                rowRedCellStyleattendeeothercolumn.VerticalAlignment = VerticalAlignment.CENTER;
                rowRedCellStyleattendeeothercolumn.BorderTop = CellBorderType.THIN;
                rowRedCellStyleattendeeothercolumn.BorderBottom = CellBorderType.THIN;
                rowRedCellStyleattendeeothercolumn.BorderLeft = CellBorderType.THIN;
                rowRedCellStyleattendeeothercolumn.BorderRight = CellBorderType.THIN;
                rowRedCellStyleattendeeothercolumn.FillForegroundColor = HSSFColor.ROSE.index;
                rowRedCellStyleattendeeothercolumn.FillPattern = FillPatternType.SOLID_FOREGROUND;
                var otherattendeeLabelFont = workbook.CreateFont();
                otherattendeeLabelFont.FontHeightInPoints = 9;
                rowRedCellStyleattendeeothercolumn.SetFont(otherattendeeLabelFont);


                var rowDarkRedCellStyleattendeeothercolumn = workbook.CreateCellStyle();
                rowDarkRedCellStyleattendeeothercolumn.VerticalAlignment = VerticalAlignment.CENTER;
                rowDarkRedCellStyleattendeeothercolumn.BorderTop = CellBorderType.THIN;
                rowDarkRedCellStyleattendeeothercolumn.BorderBottom = CellBorderType.THIN;
                rowDarkRedCellStyleattendeeothercolumn.BorderLeft = CellBorderType.THIN;
                rowDarkRedCellStyleattendeeothercolumn.BorderRight = CellBorderType.THIN;
                rowDarkRedCellStyleattendeeothercolumn.FillForegroundColor = HSSFColor.RED.index;
                rowDarkRedCellStyleattendeeothercolumn.FillPattern = FillPatternType.SOLID_FOREGROUND;
                rowDarkRedCellStyleattendeeothercolumn.SetFont(otherattendeeLabelFont);

                var rowLightYelloCellStyleattendeeothercolumn = workbook.CreateCellStyle();
                rowLightYelloCellStyleattendeeothercolumn.VerticalAlignment = VerticalAlignment.CENTER;
                rowLightYelloCellStyleattendeeothercolumn.BorderTop = CellBorderType.THIN;
                rowLightYelloCellStyleattendeeothercolumn.BorderBottom = CellBorderType.THIN;
                rowLightYelloCellStyleattendeeothercolumn.BorderLeft = CellBorderType.THIN;
                rowLightYelloCellStyleattendeeothercolumn.BorderRight = CellBorderType.THIN;
                rowLightYelloCellStyleattendeeothercolumn.FillForegroundColor = HSSFColor.LIGHT_YELLOW.index;
                rowLightYelloCellStyleattendeeothercolumn.FillPattern = FillPatternType.SOLID_FOREGROUND;
                rowLightYelloCellStyleattendeeothercolumn.SetFont(otherattendeeLabelFont);


                for (int i = 0; i < source.Count(); i++) {
                    var row = sheet.CreateRow(rowNumber++);


                    string techno = "";
                    var objtech = source[i].Project.Project_Tech.Select(x => new { TName = x.Technology.Title }).Distinct().ToList();
                    if (objtech != null && objtech.Count > 0) {
                        foreach (var ddl in objtech.ToList()) {
                            techno += ddl.TName + ",";
                        }
                    }

                    string reasons = "";
                    if (source[i].ProjectInvoiceComments != null && source[i].ProjectInvoiceComments.Count() > 0) {
                        foreach (ProjectInvoiceComment obj in source[i].ProjectInvoiceComments.ToList()) {
                            reasons += "[Discussion on " + obj.Created.ToString("dd/MM/yyyy") + "] : " + obj.InvoiceComments + " | ";

                        }
                    }
                    row.CreateCell(counter++).SetCellValue(Convert.ToString(i + 1));
                    row.CreateCell(counter++).SetCellValue(Convert.ToString(source[i].Project.CRMProjectId));
                    row.CreateCell(counter++).SetCellValue(Convert.ToString(source[i].Project.Name));
                    row.CreateCell(counter++).SetCellValue(Convert.ToString(source[i].Project.ClientId.HasValue ? source[i].Project.Client.Name : ""));
                    row.CreateCell(counter++).SetCellValue(Convert.ToString(source[i].InvoiceNumber));
                    row.CreateCell(counter++).SetCellValue(((Enums.ProjectInvoiceStatus)source[i].InvoiceStatus).ToString().Replace("PartialPayment", "Partial Payment"));
                    row.CreateCell(counter++).SetCellValue(Convert.ToString(source[i].Currency.CurrSign) + Convert.ToString(source[i].InvoiceAmount));
                    row.CreateCell(counter++).SetCellValue(Convert.ToDateTime(source[i].InvoiceStartDate).ToString("dd/MM/yyyy"));
                    row.CreateCell(counter++).SetCellValue(Convert.ToDateTime(source[i].InvoiceEndDate).ToString("dd/MM/yyyy"));
                    row.CreateCell(counter++).SetCellValue((source[i].ProjectInvoiceComments.Count(x => x.ChaseDate != null) > 0 ?
                        Convert.ToDateTime(source[i].ProjectInvoiceComments.Where(x => x.ChaseDate != null).Max(x => x.ChaseDate)).ToString("dd/MM/yyyy") : ""));
                    row.CreateCell(counter++).SetCellValue((source[i].ProjectInvoiceComments.Count() > 0 ?
                         Convert.ToString(source[i].ProjectInvoiceComments.LastOrDefault().InvoiceComments) : ""));
                    row.CreateCell(counter++).SetCellValue(source[i].UserLogin != null ? Convert.ToString(source[i].UserLogin.Name) : "N/A");
                    row.CreateCell(counter++).SetCellValue(source[i].UserLogin1 != null ? Convert.ToString(source[i].UserLogin1.Name) : "N/A");
                    row.CreateCell(counter++).SetCellValue(reasons.Trim().Trim('|'));
                    counter = 0;
                    row.Height = 350;

                    int style = 1;

                    if (DateTime.Now.Date > source[i].InvoiceStartDate.Date && DateTime.Now.Date.Subtract(source[i].InvoiceStartDate.Date).TotalDays >= 5) {
                        int DaysCnt = 0;
                        DateTime checkDate = DateTime.Now.Date;
                        DateTime startDate = source[i].InvoiceStartDate.Date;
                        while (checkDate > startDate && DaysCnt < 5) {
                            bool r = (checkDate.DayOfWeek == DayOfWeek.Sunday || checkDate.DayOfWeek == DayOfWeek.Saturday);
                            checkDate = checkDate.AddDays(-1);
                            if (!r) {
                                DaysCnt += 1;
                            }
                        }

                        if (DaysCnt >= 5) {
                            if (source[i].ProjectInvoiceComments.Count(x => x.ChaseDate.HasValue && x.ChaseDate.Value.Date >= DateTime.Now.Date) > 0) {
                                style = 2;
                            }
                            else {
                                style = 3;
                            }
                        }

                    }

                    for (int j = 0; j < headerRow.LastCellNum; j++) {
                        row.GetCell(j).CellStyle = (style == 1 ? rowLightYelloCellStyleattendeeothercolumn : style == 2 ? rowRedCellStyleattendeeothercolumn : rowDarkRedCellStyleattendeeothercolumn);
                    }
                }
                counter = 0;
                sheet.SetColumnWidth(counter++, 2000);
                sheet.SetColumnWidth(counter++, 3000);
                sheet.SetColumnWidth(counter++, 7000);
                sheet.SetColumnWidth(counter++, 6000);
                sheet.SetColumnWidth(counter++, 4000);
                sheet.SetColumnWidth(counter++, 4000);
                sheet.SetColumnWidth(counter++, 4000);
                sheet.SetColumnWidth(counter++, 4000);
                sheet.SetColumnWidth(counter++, 4000);
                sheet.SetColumnWidth(counter++, 4000);
                sheet.SetColumnWidth(counter++, 6000);
                sheet.SetColumnWidth(counter++, 6000);
                sheet.SetColumnWidth(counter++, 6000);
                sheet.SetColumnWidth(counter++, 8000);
                workbook.Write(response);
            }
            else {
                var workbook = new HSSFWorkbook();
                var headerLabelCellStyle = workbook.CreateCellStyle();
                headerLabelCellStyle.Alignment = HorizontalAlignment.LEFT;
                headerLabelCellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                headerLabelCellStyle.BorderBottom = CellBorderType.THIN;
                headerLabelCellStyle.WrapText = true;

                var headerLabelFont = workbook.CreateFont();
                headerLabelFont.Boldweight = (short)FontBoldWeight.BOLD;
                headerLabelFont.Color = HSSFColor.BLACK.index;
                headerLabelCellStyle.SetFont(headerLabelFont);

                var headerCellStyle = workbook.CreateCellStyle();
                headerCellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                headerLabelFont.FontHeightInPoints = 11;
                headerCellStyle.SetFont(headerLabelFont);


                var infoLabelCellStyle = workbook.CreateCellStyle();

                infoLabelCellStyle.Alignment = HorizontalAlignment.CENTER;
                infoLabelCellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                //InfoLabelCellStyle.BorderBottom = CellBorderType.THIN;
                infoLabelCellStyle.WrapText = false;
                infoLabelCellStyle.FillForegroundColor = HSSFColor.YELLOW.index;
                infoLabelCellStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
                var infoLabelFont = workbook.CreateFont();
                infoLabelFont.Boldweight = (short)FontBoldWeight.BOLD;
                infoLabelFont.Color = HSSFColor.BLACK.index;
                infoLabelFont.FontHeightInPoints = 14;
                infoLabelCellStyle.SetFont(infoLabelFont);

                var sheet = workbook.CreateSheet(reportname);
                var attendeeLabelCellStyle = workbook.CreateCellStyle();

                attendeeLabelCellStyle.Alignment = HorizontalAlignment.LEFT;
                attendeeLabelCellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                attendeeLabelCellStyle.BorderBottom = CellBorderType.THIN;
                attendeeLabelCellStyle.WrapText = true;
                attendeeLabelCellStyle.FillForegroundColor = HSSFColor.GREY_50_PERCENT.index;
                attendeeLabelCellStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
                var attendeeLabelFont = workbook.CreateFont();
                attendeeLabelFont.Boldweight = (short)FontBoldWeight.BOLD;
                attendeeLabelFont.Color = HSSFColor.WHITE.index;
                attendeeLabelCellStyle.SetFont(attendeeLabelFont);

                sheet.CreateRow(0);
                var headerInfo = sheet.CreateRow(1);
                headerInfo.CreateCell(3, CellType.STRING).SetCellValue("Unpaid Invoice Report as on " + DateTime.Now.ToString("dd MMM yyyy, dddd"));
                headerInfo.GetCell(3).CellStyle = infoLabelCellStyle;
                NPOI.SS.Util.CellRangeAddress cra = new NPOI.SS.Util.CellRangeAddress(1, 1, 3, 7);
                sheet.AddMergedRegion(cra);
                sheet.CreateRow(2);
                //Create a header row
                var headerRow = sheet.CreateRow(3);
                int counter = 0;
                //Set the column names in the header row 
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Sno");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("CRMID");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Project Name");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Client Name");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Invoice Number");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Status");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Invoice Amount");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Start Date");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("End Date");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Next Chase Date");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Last Conversation");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("BA Name");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("TL Name");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Comment History");
                counter = 0;

                for (int i = 0; i < headerRow.LastCellNum; i++) {
                    headerRow.GetCell(i).CellStyle = attendeeLabelCellStyle;
                    headerRow.Height = 600;
                }

                sheet.CreateFreezePane(0, 4, 0, 4);

                var rowRedCellStyleattendeeothercolumn = workbook.CreateCellStyle();
                rowRedCellStyleattendeeothercolumn.VerticalAlignment = VerticalAlignment.CENTER;
                rowRedCellStyleattendeeothercolumn.BorderTop = CellBorderType.THIN;
                rowRedCellStyleattendeeothercolumn.BorderBottom = CellBorderType.THIN;
                rowRedCellStyleattendeeothercolumn.BorderLeft = CellBorderType.THIN;
                rowRedCellStyleattendeeothercolumn.BorderRight = CellBorderType.THIN;
                rowRedCellStyleattendeeothercolumn.FillForegroundColor = HSSFColor.ROSE.index;
                rowRedCellStyleattendeeothercolumn.FillPattern = FillPatternType.SOLID_FOREGROUND;
                var otherattendeeLabelFont = workbook.CreateFont();
                otherattendeeLabelFont.FontHeightInPoints = 9;
                rowRedCellStyleattendeeothercolumn.SetFont(otherattendeeLabelFont);

                var rowDarkRedCellStyleattendeeothercolumn = workbook.CreateCellStyle();
                rowDarkRedCellStyleattendeeothercolumn.VerticalAlignment = VerticalAlignment.CENTER;
                rowDarkRedCellStyleattendeeothercolumn.BorderTop = CellBorderType.THIN;
                rowDarkRedCellStyleattendeeothercolumn.BorderBottom = CellBorderType.THIN;
                rowDarkRedCellStyleattendeeothercolumn.BorderLeft = CellBorderType.THIN;
                rowDarkRedCellStyleattendeeothercolumn.BorderRight = CellBorderType.THIN;
                rowDarkRedCellStyleattendeeothercolumn.FillForegroundColor = HSSFColor.RED.index;
                rowDarkRedCellStyleattendeeothercolumn.FillPattern = FillPatternType.SOLID_FOREGROUND;
                rowDarkRedCellStyleattendeeothercolumn.SetFont(otherattendeeLabelFont);

                var rowLightYelloCellStyleattendeeothercolumn = workbook.CreateCellStyle();
                rowLightYelloCellStyleattendeeothercolumn.VerticalAlignment = VerticalAlignment.CENTER;
                rowLightYelloCellStyleattendeeothercolumn.BorderTop = CellBorderType.THIN;
                rowLightYelloCellStyleattendeeothercolumn.BorderBottom = CellBorderType.THIN;
                rowLightYelloCellStyleattendeeothercolumn.BorderLeft = CellBorderType.THIN;
                rowLightYelloCellStyleattendeeothercolumn.BorderRight = CellBorderType.THIN;
                rowLightYelloCellStyleattendeeothercolumn.FillForegroundColor = HSSFColor.LIGHT_YELLOW.index;
                rowLightYelloCellStyleattendeeothercolumn.FillPattern = FillPatternType.SOLID_FOREGROUND;
                rowLightYelloCellStyleattendeeothercolumn.SetFont(otherattendeeLabelFont);
                sheet.SetColumnWidth(counter++, 2000);
                sheet.SetColumnWidth(counter++, 3000);
                sheet.SetColumnWidth(counter++, 7000);
                sheet.SetColumnWidth(counter++, 6000);
                sheet.SetColumnWidth(counter++, 4000);
                sheet.SetColumnWidth(counter++, 4000);
                sheet.SetColumnWidth(counter++, 4000);
                sheet.SetColumnWidth(counter++, 4000);
                sheet.SetColumnWidth(counter++, 4000);
                sheet.SetColumnWidth(counter++, 4000);
                sheet.SetColumnWidth(counter++, 6000);
                sheet.SetColumnWidth(counter++, 6000);
                sheet.SetColumnWidth(counter++, 6000);
                sheet.SetColumnWidth(counter++, 8000);

                workbook.Write(response);
            }
            response.Position = 0;
            return response;
        }
    }
}