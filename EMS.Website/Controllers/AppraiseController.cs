using DataTables.AspNet.Core;
using DataTables.AspNet.AspNetCore;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.Models.Others;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using static EMS.Core.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http.Features;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;

namespace EMS.Web.Controllers
{
    public class AppraiseController : BaseController
    {
        #region Constructor and Member

        private readonly IAppraiseService appraiseService;
        private readonly IUserLoginService userLoginService;
        private IProjectService projectService;
        public AppraiseController(IAppraiseService appraiseService, IUserLoginService userLoginService, IProjectService _projectService)
        {
            this.appraiseService = appraiseService;
            this.userLoginService = userLoginService;
            this.projectService = _projectService;
        }

        #endregion

        #region Appraise Index

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Index(AppraiseDto model)
        {
            int currentUserId = CurrentUser.Uid;
            if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
            {
                //var users = userLoginService.GetUsersByPM(PMUserId).Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() }).ToList();
                //users.Insert(0, new SelectListItem() { Text = "Select Employee", Value = "0" });
                //ViewBag.EmployeeList = users;

                var currentUserEmployeeAppraiseId = appraiseService.GetAppraiseUserIdByPMId(currentUserId)?.Select(x => x.EmployeeId).Distinct().ToList();
                var employeeList = new List<SelectListItem>();
                if (currentUserEmployeeAppraiseId.Any())
                {

                    List<int?> employeeIds = currentUserEmployeeAppraiseId;
                    employeeList = userLoginService.GetProjectListByCurrentUser(employeeIds).Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.Uid.ToString() }).ToList();
                    
                }
                employeeList.Insert(0, new SelectListItem() { Text = "Employee", Value = "0" });
                ViewBag.EmployeeList = employeeList;

                
                var currentUserProjectAppraiseId = appraiseService.GetAppraiseUserIdByPMId(currentUserId)?.Select(x => x.ProjectId).Distinct().ToList();
                var projectList = new List<SelectListItem>();
                if (currentUserProjectAppraiseId.Any())
                {

                    List<int?> projIds = currentUserProjectAppraiseId;
                    projectList = projectService.GetProjectListByCurrentUser(projIds).Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.ProjectId.ToString() }).ToList();

                }
                projectList.Insert(0, new SelectListItem() { Text = "Project", Value = "0" });
                ViewBag.ProjectList = projectList;

                model.Priority = WebExtensions.GetSelectList<Enums.AppraisePriority>();

            }
            else
            {
                var currentUserProjectAppraiseId = appraiseService.GetAppraiseUserIdById(currentUserId)?.Select(x => x.ProjectId).Distinct().ToList();
                var projectList = new List<SelectListItem>();
                if (currentUserProjectAppraiseId.Any())
                {

                    List<int?> projIds = currentUserProjectAppraiseId;
                    projectList = projectService.GetProjectListByCurrentUser(projIds).Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.ProjectId.ToString() }).ToList();

                }
                projectList.Insert(0, new SelectListItem() { Text = "Project", Value = "0" });
                ViewBag.ProjectList = projectList;
            }  

            
            return View(model);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult Index(IDataTablesRequest request, int ProjectId, int EmployeeId, int InternalTypeId, int CLientTypeId, int PriorityId)
        {

            var pagingServices = new PagingService<EmployeeAppraise>(request.Start, request.Length);
            var expr = PredicateBuilder.True<EmployeeAppraise>();

            if (ProjectId > 0)
            {
                expr = expr.And(e => e.ProjectId == ProjectId);
            }
            if (EmployeeId > 0)
            {
                expr = expr.And(e => e.EmployeeId == EmployeeId);
            }
            if (InternalTypeId > 0)
            {
                expr = expr.And(e => e.AppraiseType == InternalTypeId);
            }
            if (CLientTypeId > 0)
            {
                expr = expr.And(e => e.AppraiseType == CLientTypeId);
            }
            if (PriorityId > 0)
            {
                expr = expr.And(e => e.Priority == PriorityId);
            }

            expr = expr.And(e => e.IsActive == true && e.IsDelete == false);

            bool isPMUser = CurrentUser.RoleId == (int)Enums.UserRoles.PM;
            int currentUserId = CurrentUser.Uid;


            if (isPMUser)
            {
                expr = expr.And(x => x.UserId == currentUserId || x.UserLogin.PMUid == currentUserId);
            }
            else if (RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_Sales_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_HR_DesignationIds.Contains(CurrentUser.DesignationId))
            {
                expr = expr.And(x => x.UserId == currentUserId || x.UserLogin.TLId == currentUserId || x.EmployeeId == currentUserId);
            }
            //else if (RoleValidator.TL_RoleIds.Contains(CurrentUser.RoleId) || RoleValidator.DV_RoleIds.Contains(CurrentUser.RoleId))
            //{
            //    expr = expr.And(x => x.UserId == currentUserId || x.UserLogin.TLId == currentUserId);
            //}
            else
            {
                expr = expr.And(x => x.UserId == currentUserId);
            }

            if (request.Search.Value.HasValue())
            {
                string searchValue = request.Search.Value.Trim().ToLower();
                expr = expr.And(x => x.UserLogin1 != null && x.UserLogin1.Name.ToLower().Contains(searchValue));
            }

            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.ModifyDate);
            };

            int totalCount = 0;

            var response = appraiseService.GetAppraiseByPaging(out totalCount, pagingServices);

            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                rowIndex = (index + 1) + (request.Start),
                Id = r.Id,
                Name = r.UserLogin1?.Name,
                Type = ((Enums.AppraiseType)r.AppraiseType).GetDescription(),
                AddedDate = r.AddDate.ToFormatDateString("MMM dd, yyyy"),
                AddedBy = r.UserLogin?.Name,
                AllowEditDelete = isPMUser || r.UserId == currentUserId,
                project = r.ProjectId.HasValue?projectService.GetProjectById(r.ProjectId.Value).Name:"",
                priority = r.Priority.HasValue? Extensions.GetDescription((Enums.AppraisePrioritys)r.Priority):"",
            }));
        }

        #endregion

        #region ADD/EDIT Appraise

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Add(int? id)
        {
            if (CurrentUser.RoleId == (int)Enums.UserRoles.HRBP || CurrentUser.RoleId == (int)Enums.UserRoles.PM)
            {
                try
                {

                    AppraiseDto model = new AppraiseDto();
                    model.AppraiseListItem = WebExtensions.GetSelectList<Enums.AppraiseType>();
                    var users = userLoginService.GetUsersByPM(PMUserId);
                    model.AppraiseId = (int)AppraiseType.Internal;
                    model.Employee = users.Select(n => new SelectListItem { Text = n.Name, Value = n.Uid.ToString() }).ToList();
                    model.Priority = WebExtensions.GetSelectList<Enums.AppraisePrioritys>();
                    int pmUId = CurrentUser.RoleId == (int)Enums.UserRoles.PM ? CurrentUser.Uid : CurrentUser.RoleId == (int)Enums.UserRoles.PMO ? CurrentUser.Uid : CurrentUser.PMUid;
                    var projectList = projectService.GetProjectListByPmuid(pmUId, CurrentUser.Uid).Select(x => new SelectListItem { Text = x.CRMProjectId > 0 ? x.Name.ToString() + "[" + x.CRMProjectId + "]" : x.Name.ToString(), Value = x.ProjectId.ToString() }).ToList();
                    projectList.Insert(0, new SelectListItem() { Text = "--Project--", Value = "0" });
                    ViewBag.ProjectList = projectList;
                    model.PriorityId = (int)Enums.AppraisePrioritys.Low;

                    if (id.HasValue && id.Value > 0)
                    {
                        var AppraiseData = appraiseService.GetAppraiseData(id.Value);
                        if (AppraiseData != null)
                        {
                            //if (AppraiseData.UserId == CurrentUser.Uid)
                            {
                                model.Id = id.Value;
                                model.ClientComment = AppraiseData.ClientComment;
                                model.TlComment = AppraiseData.TlComment;
                                model.AppraiseId = AppraiseData.AppraiseType.HasValue ? AppraiseData.AppraiseType.Value : 0;
                                model.EmployeeId = AppraiseData.EmployeeId.HasValue ? AppraiseData.EmployeeId.Value : 0;
                                model.ClientDate = AppraiseData.ClientDate.HasValue ? AppraiseData.ClientDate.Value.ToFormatDateString("dd/MM/yyyy") : null;
                                model.ProjectId = AppraiseData.ProjectId.Value;
                                model.PriorityId = AppraiseData.Priority.Value;
                            }
                            //else
                            //{
                            //    return MessagePartialView("Invalid access.");
                            //}
                        }
                        else
                        {
                            return MessagePartialView("Unable to find record");
                        }
                    }
                    return PartialView("_AddEditAppraise", model);
                }
                catch (Exception ex)
                {
                    return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
                }
            }
            else
            {
                return AccessDenied();
            }
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult Add(AppraiseDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.UId = CurrentUser.Uid;
                    model.IP = ContextProvider.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();// Request.UserHostAddress;
                    var result = appraiseService.Save(model);

                    if (result != null && result.Id > 0)
                    {
                        if (model.SendEmail)
                        {
                            var status = SendEmailToRespectivePersions(model);

                        }
                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = "Appraise saved successfully"
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
        private string SendEmailToRespectivePersions(AppraiseDto model)
        {
            try
            {
                string Employee = "";
                List<string> allEmployeeEmails = new List<string>();
                List<string> rsEmails = new List<string>();
                List<string> tlEmails = null;
                UserLogin pmInfo = null;
                string allEmails = null;
                List<string> emailIds = new List<string>();
                List<string> emailIdsTLAndPMPerson = new List<string>();

                if (model.EmployeeId != 0)
                {
                    var u = userLoginService.GetUserInfoByID(model.EmployeeId);
                    if (u != null)
                    {
                        rsEmails.Add(u.EmailOffice);
                    }

                    if (u != null && u.TLId > 0)
                    {
                        var tl = userLoginService.GetUserInfoByID(u.TLId ?? 0);
                        if (tl != null && !String.IsNullOrEmpty(tl.EmailOffice) && !emailIdsTLAndPMPerson.Contains(tl.EmailOffice))
                        {
                            emailIdsTLAndPMPerson.Add(tl.EmailOffice);
                        }
                    }

                    if (u != null && u.PMUid > 0)
                    {
                        var pmuser = userLoginService.GetUserInfoByID(u.PMUid ?? 0);
                        if (pmuser != null && !String.IsNullOrEmpty(pmuser.EmailOffice) && !emailIdsTLAndPMPerson.Contains(pmuser.EmailOffice))
                        {
                            emailIdsTLAndPMPerson.Add(pmuser.EmailOffice);
                        }
                    }

                    Employee = u.Name.ToString();
                }
                //tlEmails = userLoginService.GetUsersEmailByRole(CurrentUser.PMUid, (byte)Enums.UserRoles.TL);
                //pmInfo = userLoginService.GetUsersById(CurrentUser.PMUid);

                //if (tlEmails != null)
                //{
                //    emailIds = allEmployeeEmails.Concat(tlEmails).ToList();
                //}
                //if (rsEmails != null)
                //{
                //    if (emailIds == null && emailIds.Count == 0)
                //    {
                //        emailIds = allEmployeeEmails.Concat(rsEmails).ToList();
                //    }
                //    else
                //    {
                //        emailIds = allEmployeeEmails.Concat(rsEmails).Concat(emailIds).ToList();
                //    }
                //}
                //if (pmInfo != null)
                //{
                //    emailIds.Add(pmInfo.EmailOffice);
                //}

                //if (emailIds != null && emailIds.Count > 0)
                //{
                //    allEmails = string.Join(",", emailIds);
                //}

                string AppraiseType = ((Enums.AppraiseType)model.AppraiseId).ToString();
                var v0 = "";
                if (AppraiseType == "Internal")
                {
                    v0 = "none";
                }
                else
                {
                    v0 = "block";
                }
                var v1 = Employee;
                var v2 = model.TlComment;
                var v3 = model.ClientDate;
                var v4 = model.ClientComment;
                var v5 = CurrentUser.Name;


                var ValueArray = new string[] { v0, v1, v2, v3, v4, v5, };

                FlexiMail objSendMail = new FlexiMail();
                objSendMail.ValueArray = ValueArray;
                objSendMail.Subject = "Appraise Email - " + Employee;
                objSendMail.MailBodyManualSupply = true;
                objSendMail.MailBody = objSendMail.GetHtml("AppraiseEmail.html");
                objSendMail.From = SiteKey.From;
                //objSendMail.To = allEmails;

                if (rsEmails != null && rsEmails.Count > 0)
                {
                    objSendMail.To = string.Join(",", rsEmails);

                    if (emailIdsTLAndPMPerson != null && emailIdsTLAndPMPerson.Count > 0)
                    {
                        objSendMail.CC = string.Join(",", emailIdsTLAndPMPerson);
                    }

                    // objSendMail.Send();
                    return "Success";
                }

                //if (!string.IsNullOrEmpty(allEmails))
                //{
                //    objSendMail.Send();
                //    return "Success";
                //}
                return "No email found to send.";
            }
            catch (Exception ex)
            {
                return (ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }
        #endregion

        #region Delete Appraise

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
                    var AppraiseData = appraiseService.GetAppraiseData(id);
                    if (AppraiseData != null && AppraiseData.UserId == CurrentUser.Uid)
                    {
                        appraiseService.Delete(id);
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record deleted successfully.", IsSuccess = true });
                    }
                }

                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = "Unable to delete this appraise" });
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = ex.Message });
            }
        }

        #endregion
    }
}