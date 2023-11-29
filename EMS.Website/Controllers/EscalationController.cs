using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.Models.Others;
using System;
using System.Collections.Generic;

//using System.Configuration;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using EMS.Website.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Text;
using EMS.Web.Modals;
//using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using EMS.Website.Code.LIBS;
using System.Data;
using EMS.Service.LibraryManagement;
using Microsoft.AspNetCore.Hosting;
using System.Globalization;


namespace EMS.Web.Controllers
{
    [CustomAuthorization(IsAshishTeam: true)]
    public class EscalationController : BaseController
    {
        #region "Variables and constructor"
        private readonly IEscalationService escalationService;
        private readonly IEscalationTypeService escalationTypeService;
        private readonly IEscalationRootCauseService escalationRootCauseService;
        private readonly IProjectService projectService;
        private readonly IUserLoginService userLoginService;
        private readonly IHostingEnvironment _hostingEnvironment;
        #endregion

        public EscalationController(IEscalationService _escalationService,
            IEscalationTypeService _escalationTypeService,
            IEscalationRootCauseService _escalationRootCauseService,
            IProjectService _projectService,
           IUserLoginService _userLoginService)
        {
            escalationService = _escalationService;
            escalationTypeService = _escalationTypeService;
            escalationRootCauseService = _escalationRootCauseService;
            projectService = _projectService;
            userLoginService = _userLoginService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult Index(IDataTablesRequest request)
        {
            try
            {
                var pagingServices = new PagingService<Escalation>(request.Start, request.Length);
                var filterExpr = PredicateBuilder.True<Escalation>();

                filterExpr = filterExpr.And(x => x.EsculationForUser.Any(y => y.Uid == CurrentUser.Uid) || x.EsculationFoundForUser.Any(y => y.Uid == CurrentUser.Uid) || x.AddedByUid == CurrentUser.Uid);

                if (!string.IsNullOrEmpty(request.Search.Value))
                {
                    var search = request.Search.Value.ToString().Trim().ToLower();
                    filterExpr = filterExpr.And(X => X.Project.Name.Trim().ToLower().Contains(search));
                    filterExpr = filterExpr.Or(X => X.ProjectId.ToString().Trim().ToLower().Contains(search));
                    filterExpr = filterExpr.Or(X => X.EscalationDescription.Trim().ToLower().Contains(search));
                    filterExpr = filterExpr.Or(X => X.RootCauseAnalysis.Trim().ToLower().Contains(search));
                    filterExpr = filterExpr.Or(X => X.EsculationForUser.Any(y => y.U.Name.Contains(search)));
                    filterExpr = filterExpr.Or(X => X.EsculationFoundForUser.Any(y => y.U.Name.Contains(search)));
                }
                pagingServices.Filter = filterExpr;
                pagingServices.Sort = (o) =>
                {
                    return o.OrderByDescending(c => c.Id);
                };

                int totalCount = 0;
                var response = escalationService.GetEscalations(out totalCount, pagingServices);
                var result = DataTablesJsonResult(totalCount, request, response.Select((d, index) =>
                {
                    var detail = new
                    {
                        rowIndex = (index + 1) + (request.Start),
                        id = d.Id,
                        type = d.EscalationTypeNavigation.Title,
                        project = d.Project.Name,
                        escalationDate = d.DateofEscalation.Date.ToFormatDateString("MMM, dd yyyy"),
                        createdDate = d.AddDate.ToFormatDateString("MMM, dd yyyy hh:mm tt"),
                        isActive = d.IsActive.ToString().ToUpper(),
                    };
                    return detail;
                }));
                return result;
            }
            catch (Exception ex)
            {

            }
            return Ok();
        }

        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult AddEdit(int? Id)
        {
            if (CurrentUser.RoleId != (int)Enums.UserRoles.PM)
            {
                return AccessDenied();
            }
            EscalationDto model = new EscalationDto();
            if (Id.HasValue && Id.Value > 0)
            {
                var entity = escalationService.GetEscalationById(Id.Value);
                model.Id = entity.Id;
                model.DateofEscalation = entity.DateofEscalation.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                model.EscalationType = entity.EscalationType.Value;
                model.SeverityLevel = entity.SeverityLevel;
                model.EscalationDetails = entity.EscalationDescription;
                model.ProjectId = entity.ProjectId;
                model.RootCauseAnalysisId = entity.RootCause.Value;
                model.RootCauseAnalysisDesctiption = entity.RootCauseAnalysis;
                model.Status = entity.Status;
                model.IsActive = entity.IsActive.HasValue ? entity.IsActive.Value : false;
                model.AddDate = entity.AddDate;
                model.ModifiedDate = entity.ModifyDate;
                model.AddedByUid = entity.AddedByUid;
                model.EscalationDocumentsList = entity.EscalationDocument.Select(y => new EscalationDocumentDto()
                {
                    Id = y.Id,
                    DocumentPath = y.DocumentPath,
                    EscalationId = y.EscalationId.Value
                }).ToList();

                var userList = userLoginService.GetUsersByPM(PMUserId).Select(y => new SelectListItem
                {
                    //Text = y.Name + ' ' + GetPMName(y.PMUid.HasValue? y.PMUid.Value:0),
                    Text = y.Name + ' ' + (y.Pmu!=null? '(' + y.Pmu.Name + ')' : ""),
                    Value = y.Uid.ToString(),
                    Selected = entity.EsculationForUser.Any(x => x.Uid == y.Uid)
                }).OrderBy(t => t.Text).ToList();

                var foundUserList = userLoginService.GetUsersByPM(PMUserId).Select(y => new SelectListItem
                {
                    Text = y.Name + ' ' + (y.Pmu != null ? '(' + y.Pmu.Name + ')' : ""),
                    Value = y.Uid.ToString(),
                    Selected = entity.EsculationFoundForUser.Any(x => x.Uid == y.Uid)
                }).OrderBy(t => t.Text).ToList();

                model.EscalationTypeList = escalationTypeService.GetEscalationTypeList().Select(x => new SelectListItem
                {
                    Text = x.Title,
                    Value = x.Id.ToString()
                }).OrderBy(t => t.Text).ToList();


                model.Project = projectService.GetProjectListByPmuid(PMUserId).Select(x => new SelectListItem
                {
                    Text = $"{x.Name} [{x.CRMProjectId}]",
                    Value = x.ProjectId.ToString()
                }).OrderBy(t => t.Text).ToList();

                model.EscalationRootCauseList = escalationRootCauseService.GetEscalationsRootCauseList().Select(x => new SelectListItem
                {
                    Text = x.Title,
                    Value = x.Id.ToString()
                }).OrderBy(t => t.Text).ToList();

                model.EscalationForUserList = userList;
                model.EscalationFoundForUserList = foundUserList;
            }
            else
            {

                var userList = userLoginService.GetUsersByPM(PMUserId).Select(y => new SelectListItem
                {
                    Text = y.Name + ' ' + (y.Pmu != null ? '(' + y.Pmu.Name + ')' : ""),
                    Value = y.Uid.ToString()
                }).OrderBy(t => t.Text).ToList();

                model.EscalationTypeList = escalationTypeService.GetEscalationTypeList().Select(x => new SelectListItem
                {
                    Text = x.Title,
                    Value = x.Id.ToString()
                }).OrderBy(t => t.Text).ToList();


                model.Project = projectService.GetProjectListByPmuid(PMUserId).Select(x => new SelectListItem
                {
                    Text = $"{x.Name} [{x.CRMProjectId}]",
                    Value = x.ProjectId.ToString()
                }).OrderBy(t => t.Text).ToList();

                model.EscalationRootCauseList = escalationRootCauseService.GetEscalationsRootCauseList().Select(x => new SelectListItem
                {
                    Text = x.Title,
                    Value = x.Id.ToString()
                }).OrderBy(t => t.Text).ToList();

                model.EscalationForUserList = userList;
                model.EscalationFoundForUserList = userList;
            }
            return View(model);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult AddEdit(EscalationDto model)
        {
            if (CurrentUser.RoleId != (int)Enums.UserRoles.PM)
            {
                return AccessDenied();
            }
            var entity = BindEscalationEntityData(model);
            dynamic param = null;
            if (model.SendEmail)
            {
                var users = model.EscalationFound.Union(model.ReceivedFor).ToArray();
                if (users != null && users.Length > 0)
                {
                    var userList = userLoginService.GetUsersByPM(PMUserId).Where(x => users.Any(y => y == x.Uid.ToString())).Select(y => new SelectListItem
                    {
                        Text = y.Name,
                        Value = y.EmailOffice.ToString(),
                    }).OrderBy(t => t.Text).ToList();

                    var project = projectService.GetProjectById(entity.ProjectId);

                    var EscalationType = escalationTypeService.GetEscalationTypeList().Where(x => x.Id == model.EscalationType).FirstOrDefault();

                    string v6 = ConclusionHtml(entity.EscalationConclusion);


                    var v0 = project.Name;
                    var v1 = entity.DateofEscalation.ToString("dd-MM-yyyy");
                    var v2 = model.SeverityLevel.ToString();
                    var v3 = String.Join(", ", userList.Select(x => x.Text).ToList());
                    var v4 = model.EscalationDetails;
                    var v5 = EscalationType != null ? EscalationType.Title : "";
                    var rsEmails = userList.Select(x => x.Value).ToList();
                    List<string> emailIdsTLAndPMPerson = new List<string>();

                    var PmEmailId = userLoginService.GetPmEmailId(CurrentUser.PMUid);
                    rsEmails.Add(PmEmailId);

                    FlexiMail objSendMail = new FlexiMail();
                    var ValueArray = new string[] { v0, v1, v2, v3, v4, v5, v6 };
                    objSendMail.ValueArray = ValueArray;
                    objSendMail.Subject = "Escalation Email";
                    objSendMail.MailBodyManualSupply = true;
                    objSendMail.MailBody = objSendMail.GetHtml("EscalationEmail.html");
                    objSendMail.From = SiteKey.From;
                    //objSendMail.To = allEmails;

                    if (rsEmails != null && rsEmails.Count > 0)
                    {
                        objSendMail.To = string.Join(",", rsEmails);

                        if (emailIdsTLAndPMPerson != null && emailIdsTLAndPMPerson.Count > 0)
                        {
                            objSendMail.CC = string.Join(",", emailIdsTLAndPMPerson);
                        }
                        param = new { values = ValueArray, emails = string.Join(",", rsEmails) };
                        // objSendMail.Send();
                    }
                }
            }
            escalationService.Save(entity);
            ShowSuccessMessage("Success", "Escalation saved successfully", false);
            return NewtonSoftJsonResult(new RequestOutcome<dynamic> { RedirectUrl = SiteKey.DomainName + "Escalation/index", Data = param });
        }

        public string ConclusionHtml(ICollection<EscalationConclusion> escalationConclusions)
        {
            string v6 = "";
            if (escalationConclusions != null && escalationConclusions.Count > 0)
            {
                v6 += "<tr><td>&nbsp;</td></tr><tr><td><b>Conclusions: </b></td></tr><tr><td>&nbsp;</td></tr>";
                foreach (var item in escalationConclusions.ToList())
                {
                    FlexiMail objSendMail1 = new FlexiMail();
                    var v0 = item.Resolution;
                    var v1 = item.LessonLearnExplanation;
                    var v2 = item.AddedByU.Name;
                    var v3 = item.AddDate.ToFormatDateString("MMM, dd yyyy hh:mm tt");
                    var ValueArray1 = new string[] { v0, v1, v2, v3 };
                    objSendMail1.ValueArray = ValueArray1;
                    v6 += objSendMail1.GetHtml("EscalationSingleConclusionEmail.html");
                    v6 += "<tr><td>&nbsp;</td></tr>";
                }
            }
            return v6;
        }

        public IActionResult SendEmail()
        {
            return PartialView("_SendEmail");
        }

        [HttpPost]
        public IActionResult SendEmail(string[] ValueArray, string to)
        {
            FlexiMail objSendMail = new FlexiMail();
            objSendMail.ValueArray = ValueArray;
            objSendMail.Subject = "Escalation Email";
            objSendMail.MailBodyManualSupply = true;
            objSendMail.MailBody = objSendMail.GetHtml("EscalationEmail.html");
            objSendMail.From = SiteKey.From;
            objSendMail.To = to;
            objSendMail.Send();
            return Ok();
        }

        private Escalation BindEscalationEntityData(EscalationDto vModel)
        {
            var _Escalation = escalationService.GetEscalationById(vModel.Id) ?? new Escalation();
            if (_Escalation != null)
            {
                _Escalation.Id = vModel.Id;
                _Escalation.DateofEscalation = DateTime.ParseExact(vModel.DateofEscalation, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                _Escalation.EscalationType = vModel.EscalationType;
                _Escalation.SeverityLevel = vModel.SeverityLevel;
                _Escalation.EscalationDescription = vModel.EscalationDetails;
                _Escalation.ProjectId = vModel.ProjectId;
                _Escalation.RootCause = vModel.RootCauseAnalysisId;
                _Escalation.RootCauseAnalysis = vModel.RootCauseAnalysisDesctiption;
                _Escalation.Status = vModel.Status;
                _Escalation.IsActive = true;
                _Escalation.AddDate = DateTime.Now;
                _Escalation.ModifyDate = DateTime.Now;
                if (_Escalation.Id == 0)
                {
                    _Escalation.AddedByUid = CurrentUser.Uid;
                }

                if (_Escalation.Id > 0)
                {
                    if (_Escalation.EsculationForUser != null && _Escalation.EsculationForUser.Any())
                    {
                        escalationService.EscalationUserDeleted(_Escalation);
                    }
                    if (_Escalation.EsculationFoundForUser != null && _Escalation.EsculationFoundForUser.Any())
                    {
                        escalationService.EscalationFoundUserDeleted(_Escalation);
                    }
                }
                _Escalation = BindEscalationImageEntityData(_Escalation, vModel);
            }
            return _Escalation;
        }

        //Bind Escalation Images
        private Escalation BindEscalationImageEntityData(Escalation _Escalation, EscalationDto vModel)
        {
            var files = HttpContext.Request.Form.Files;

            _Escalation.EscalationDocument = _Escalation.EscalationDocument ?? new List<EscalationDocument>();
            vModel.EscalationImages = vModel.EscalationImages ?? new List<IFormFile>();
            if (files.Count > 0)
            {
                var imageDirectory = Path.Combine(ContextProvider.HostEnvironment.WebRootPath, $"Images//escalation");
                if (!Directory.Exists(imageDirectory))
                    Directory.CreateDirectory(imageDirectory);
                foreach (var item in files)
                {
                    var ImageName = item.FileName.ToUnique();
                    using (var fileStream = new FileStream(Path.Combine(ContextProvider.HostEnvironment.WebRootPath, $"Images//escalation//{ImageName}"), FileMode.Create))
                    {
                        item.CopyTo(fileStream);
                    }
                    EscalationDocument escalationImage = new EscalationDocument();
                    escalationImage.EscalationId = _Escalation.Id;
                    escalationImage.DocumentPath = ImageName;
                    escalationImage.AddedDate = DateTime.Now;
                    _Escalation.EscalationDocument.Add(escalationImage);
                }
            }

            _Escalation = BindEscalationUserEntityData(_Escalation, vModel);
            return _Escalation;
        }

        private Escalation BindEscalationUserEntityData(Escalation _Escalation, EscalationDto vModel)
        {
            if (vModel.ReceivedFor == null)
            {
                vModel.ReceivedFor = new string[0];
            }
            foreach (var item in vModel.ReceivedFor)
            {
                EsculationForUser esculationForUser = new EsculationForUser();
                esculationForUser.Uid = Convert.ToInt32(item);
                esculationForUser.EsculationId = _Escalation.Id;
                _Escalation.EsculationForUser.Add(esculationForUser);
            }
            if (vModel.EscalationFound == null)
            {
                vModel.EscalationFound = new string[0];
            }
            foreach (var item in vModel.EscalationFound)
            {
                EsculationFoundForUser esculationFoundForUser = new EsculationFoundForUser();
                esculationFoundForUser.Uid = Convert.ToInt32(item);
                esculationFoundForUser.EsculationId = _Escalation.Id;
                _Escalation.EsculationFoundForUser.Add(esculationFoundForUser);
            }
            return _Escalation;

            //_Escalation.EsculationForUser = _Escalation.EsculationForUser ?? new List<EsculationForUser>();
            //vModel.EscalationUsers = vModel.EscalationForUserList.Where(t => vModel.ReceivedFor.Contains(t.Value)).Select(x => new EscalationUserDTO() { EscalationId = _Escalation.Id, UserId = Convert.ToInt32(x.Value) }).ToList();
            //List<int> previousEscalationUsers = _Escalation.EsculationForUser.Select(t => t.Uid).ToList();
            //vModel.DeletedEscalationUsers = _Escalation.EsculationForUser.Where(x => !vModel.EscalationUsers.Any(t => t.UserId == x.Uid)).Select(x => x.Uid).ToList();
            //foreach (var item in vModel.EscalationUsers)
            //{
            //    EsculationForUser esculationForUser = _Escalation.EsculationForUser.FirstOrDefault(t => t.EsculationId == item.EscalationId) ?? new EsculationForUser();
            //    esculationForUser.EsculationId = item.EscalationId;
            //    esculationForUser.Uid = item.UserId;

            //    if (!previousEscalationUsers.Contains(item.EscalationId))
            //    {
            //        _Escalation.EsculationForUser.Add(esculationForUser);
            //    }
            //}

        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult DeleteDocumentFile(int id)
        {
            try
            {
                var response = escalationService.DeleteDocumentFile(id);
                return NewtonSoftJsonResult(response);
            }
            catch (Exception)
            {
                return NewtonSoftJsonResult(false);
            }
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult GetEscalationConclusion(IDataTablesRequest request, int escalationId)
        {
            try
            {
                var pagingServices = new PagingService<EscalationConclusion>(request.Start, request.Length);
                var filterExpr = PredicateBuilder.True<EscalationConclusion>();

                filterExpr = filterExpr.And(x => x.EscalationId == escalationId);

                if (!string.IsNullOrEmpty(request.Search.Value))
                {
                    var search = request.Search.Value.ToString().Trim().ToLower();
                    filterExpr = filterExpr.And(X => X.AddedByU.Name.Trim().ToLower().Contains(search));
                }
                pagingServices.Filter = filterExpr;
                pagingServices.Sort = (o) =>
                {
                    return o.OrderByDescending(c => c.Id);
                };

                int totalCount = 0;
                var response = escalationService.GetEscalationConclusions(out totalCount, pagingServices);
                var result = DataTablesJsonResult(totalCount, request, response.Select((d, index) =>
                {
                    var detail = new
                    {
                        rowIndex = (index + 1) + (request.Start),
                        id = d.Id,
                        uid = d.AddedByUid,
                        name = d.AddedByU.Name,
                        username = d.AddedByU.UserName,
                        resolution = d.Resolution,
                        lesson = d.LessonLearnExplanation,
                        createdDate = d.AddDate.ToFormatDateString("MMM, dd yyyy hh:mm tt"),
                    };
                    return detail;
                }));
                return result;
            }
            catch (Exception ex)
            {

            }
            return Ok();
        }

        [HttpGet]
        public ActionResult AddEditConclusion(int id, int escalationId)
        {
            var model = new EscalationConclusionDTO();
            model.EscalationId = escalationId;
            if (id > 0)
            {
                var result = escalationService.GetEscalationConclusionById(id);
                model.Resolution = result.Resolution;
                model.LessonLearnExplanation = result.LessonLearnExplanation;
                model.AddedByUid = result.AddedByUid;
                model.AddDate = result.AddDate;
                model.ModifyDate = result.ModifyDate;
                model.Id = result.Id;
                model.EscalationId = result.EscalationId;
            }
            return PartialView("_AddEditConclusion", model);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult AddEditConclusion(EscalationConclusionDTO model)
        {
            var entity = new EscalationConclusion();
            entity.Id = model.Id;
            entity.AddedByUid = model.AddedByUid;
            entity.EscalationId = model.EscalationId;
            entity.Resolution = model.Resolution;
            entity.LessonLearnExplanation = model.LessonLearnExplanation;
            entity.AddedByUid = CurrentUser.Uid;
            escalationService.SaveEscalationConclusion(entity);
            var escalation = escalationService.GetEscalationById(model.EscalationId);
            dynamic param = null;

            if (escalation != null)
            {
                var users = escalation.EsculationFoundForUser.Select(x => x.Uid).ToArray().Union(escalation.EsculationForUser.Select(x => x.Uid).ToArray()).ToArray();
                if (users != null && users.Length > 0)
                {
                    var userList = userLoginService.GetUsersByPM(PMUserId).Where(x => users.Any(y => y == x.Uid)).Select(y => new SelectListItem
                    {
                        Text = y.Name,
                        Value = y.EmailOffice.ToString(),
                    }).OrderBy(t => t.Text).ToList();

                    var project = projectService.GetProjectById(escalation.ProjectId);

                    var EscalationType = escalationTypeService.GetEscalationTypeList().Where(x => x.Id == escalation.EscalationType).FirstOrDefault();

                    var v0 = project.Name;
                    var v1 = EscalationType != null ? EscalationType.Title : "";

                    var v2 = model.Resolution;
                    var v3 = model.LessonLearnExplanation;
                    var v4 = CurrentUser.Name;
                    var v5 = DateTime.Now.ToFormatDateString("MMM, dd yyyy hh:mm tt");
                    var rsEmails = userList.Select(x => x.Value).ToList();
                    List<string> emailIdsTLAndPMPerson = new List<string>();

                    var PmEmailId = userLoginService.GetPmEmailId(CurrentUser.PMUid);
                    rsEmails.Add(PmEmailId);

                    var ValueArray = new string[] { v0, v1, v2, v3, v4, v5 };
                    FlexiMail objSendMail = new FlexiMail();
                    objSendMail.ValueArray = ValueArray;
                    objSendMail.Subject = "Escalation Conclusion Email";
                    objSendMail.MailBodyManualSupply = true;
                    objSendMail.MailBody = objSendMail.GetHtml("EscalationConclusionEmail.html");
                    objSendMail.From = SiteKey.From;

                    if (rsEmails != null && rsEmails.Count > 0)
                    {
                        objSendMail.To = string.Join(",", rsEmails);
                        objSendMail.Send();
                    }
                }
            }
            ShowSuccessMessage("Success", "Escalation Conclusion saved successfully", false);
            return NewtonSoftJsonResult(new RequestOutcome<dynamic> { RedirectUrl = SiteKey.DomainName + "Escalation/AddEdit?Id=" + model.EscalationId });
        }
        private string GetPMName(int id)
        {
            var user = string.Empty;
            var userdata = userLoginService.GetUserInfoByID(id);
            if (userdata != null)
            {
                user = '(' + userdata.Name + ')';
            }
            else
            {
                user = "";
            }

            return user;
        }

        private string GetPMName(UserLogin userData)
        {
            var user = string.Empty;
            if(userData.Pmu != null)
            {
                user = '(' + userData.Pmu.Name + ')';
            }
            else
            {
                user = "";
            }

            return user;
        }     

    }
}
