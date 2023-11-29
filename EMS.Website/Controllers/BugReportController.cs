using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using static EMS.Core.Enums;
using Microsoft.AspNetCore.Http.Features;

namespace EMS.Web.Controllers
{
    [CustomAuthorization]
    public class BugReportController : BaseController
    {
        #region Fields and  Member

        private readonly IBugReportService bugReportService;
        private readonly IPreferenceService preferenceService;
        public BugReportController(IBugReportService bugReportService, IPreferenceService preferenceService)
        {
            this.bugReportService = bugReportService;
            this.preferenceService = preferenceService;
        }

        #endregion

        #region Index

        [HttpGet]
        public ActionResult Index()
        {
            BugReportDto model = new BugReportDto();
            model.IsApprover = CurrentUser.Uid == SiteKey.AshishTeamPMUId;
           model.StatusList = WebExtensions.GetSelectList<BugReportStatus>();
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(IDataTablesRequest request, int? StatusFilterId)
        {
            var pagingServices = new PagingService<ReportBug>(request.Start, request.Length);
            var expr = PredicateBuilder.True<ReportBug>();

            if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
            {
                expr = expr.And(x => x.UserId == CurrentUser.Uid || x.UserLogin.PMUid == PMUserId);
            }
            else
            {
                expr = expr.And(x => x.UserId == CurrentUser.Uid);
            }

            if (StatusFilterId.HasValue && StatusFilterId.Value > 0)
            {
                expr = expr.And(x => x.Status == StatusFilterId.Value);
            }

            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.ModifyDate);
            };

            int totalCount = 0;

            var response = bugReportService.GetBugReportByPaging(out totalCount, pagingServices);
            bool isAshishUser = CurrentUser.Uid == SiteKey.AshishTeamPMUId;

            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                rowIndex = (index + 1) + (request.Start),
                Id = r.ReportId,
                Module = r.SectionName,
                Attachment = r.ImageName.HasValue() ? r.ImageName : null,
                Description = r.SectionDescription,
                AddedDate = r.AddDate.ToFormatDateString("MMM dd, yyyy"),
                Status = ((BugReportStatus)r.Status).GetDescription(),
                AddedBy = r.UserLogin?.Name,
                Comment = r.Remark,
                IsAllowed = isAshishUser
            }));
        }

        #endregion

        #region ADD Bug

        [HttpGet]
        public ActionResult Add(int? id)
        {
            try
            {
                BugReportDto model = new BugReportDto();
                return View(model);
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Add(BugReportDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.IP = ContextProvider.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();//Request.UserHostAddress;
                    model.AddedBy = CurrentUser.Uid;
                    try
                    {
                        if (model.Attachment != null && model.Attachment.Length > 0)
                        {
                            string fileName = Guid.NewGuid().ToString() + "_" + model.Attachment.FileName.Replace("&", string.Empty).Replace("--", "-");                           
                            string folderPath = "Upload/BugReport";
                            string FilePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/"+ folderPath, fileName);                           
                            using (var stream = new FileStream(FilePath, FileMode.Create))
                            {
                                model.Attachment.CopyTo(stream);
                            }
                            model.ImageName = folderPath+"/"+ fileName;
                        }
                    }
                    catch { }

                    var result = bugReportService.Save(model);

                    if (result != null && result.ReportId > 0)
                    {
                        SendBugEmail(result);
                        SendBugAcknowledgement(result);
                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = "Bug/Suggestion saved successfully"
                        });
                    }
                    else
                    {
                        return MessagePartialView("Unable to save record");
                    }
                }
                catch (Exception ex)
                {
                    return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
                }
            }
            else
            {
                return CreateModelStateErrors();
            }
        }

        #endregion

        #region Update Status

        [HttpGet]
        public ActionResult Status(int id)
        {
            if (id > 0)
            {
                BugStatusDto model = new BugStatusDto();
                var data = bugReportService.GetBugReportById(id);
                if (data != null)
                {
                    model.Id = id;
                    model.SectionName = data.SectionName;
                    model.StatusId = data.Status;
                    model.SectionDescription = data.SectionDescription;
                    model.StatusList = WebExtensions.GetSelectList<Enums.BugReportStatus>();
                    return PartialView("_UpdateStatus", model);
                }
            }
            return MessagePartialView("Unable to Update");
        }

        [HttpPost]
        public ActionResult Status(BugStatusDto model)
        {
            try
            {
                if (model.Id > 0)
                {
                    var result = bugReportService.UpdateStatus(model);
                    if (result != null)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record Updated successfully.", IsSuccess = true });
                    }
                    else
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Unable to Update.", IsSuccess = false });
                    }
                }
                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = "Unable to Update" });
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = ex.Message });
            }
        }

        #endregion

        #region Send Email

        private void SendBugEmail(ReportBug bugEntity)
        {
            try
            {
                var preference = preferenceService.GetDataByPmid(PMUserId);
                if (preference != null && (preference.EmailDeveloper.HasValue() || preference.EmailPM.HasValue()))
                {
                    FlexiMail mailer = new FlexiMail();
                    mailer.ValueArray = new string[]
                    {
                        CurrentUser.Name.ToTitleCase(),
                        bugEntity.SectionName,
                        bugEntity.SectionDescription,
                        bugEntity.PagePath
                    };
                    mailer.Subject = $"Report Bug/Suggestion - {CurrentUser.Name.ToTitleCase()} ";
                    mailer.MailBodyManualSupply = true;
                    mailer.MailBody = mailer.GetHtml("BugReport.html");

                    if (preference.EmailDeveloper.HasValue() && preference.EmailPM.HasValue())
                    {
                        mailer.To = $"{preference.EmailDeveloper};";
                        mailer.CC = $"{preference.EmailPM};";
                    }
                    else if (preference.EmailDeveloper.HasValue())
                    {
                        mailer.To = $"{preference.EmailDeveloper};";
                    }
                    else
                    {
                        mailer.To = $"{preference.EmailPM};";
                    }

                    if (bugEntity.ImageName.HasValue())
                    {
                        var fileBytes = System.IO.File.ReadAllBytes(Path.Combine(ContextProvider.HostEnvironment.WebRootPath, bugEntity.ImageName) );
                        if (fileBytes != null && fileBytes.Length > 0)
                        {
                            var attachment = new System.Net.Mail.Attachment(new MemoryStream(fileBytes), $"BugReport_{DateTime.Now.Ticks}");
                            mailer.MailAttachments = new System.Net.Mail.Attachment[] { attachment };
                        }
                    }
                  //  mailer.From = LIBS.SiteKey.From;
                    mailer.Send();
                }

            }
            catch
            {

            }
        }
        private void SendBugAcknowledgement(ReportBug bugEntity)
        {
            try
            {
                if (bugEntity.UserLogin.EmailOffice.HasValue())
                {
                    FlexiMail mailer = new FlexiMail();
                    mailer.ValueArray = new string[]
                    {
                        CurrentUser.Name.ToTitleCase()
                    };
                    mailer.Subject = $"Thank you for reporting bug/suggestion - {CurrentUser.Name.ToTitleCase()} ";
                    mailer.MailBodyManualSupply = true;
                    mailer.MailBody = mailer.GetHtml("BugReportACK.html");
                    mailer.To = bugEntity.UserLogin.EmailOffice;
                    //mailer.From = LIBS.SiteKey.From;
                    mailer.Send();
                }
            }
            catch { }
        }

        #endregion

    }
}