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
using Microsoft.AspNetCore.Mvc.Rendering;
using EMS.Dto.ComplaintUser;
using EMS.Data.Model;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace EMS.Web.Controllers
{
    public class ComplaintController : BaseController
    {
        #region Constructor and Member

        private readonly IComplaintService complaintService;
        private readonly IUserLoginService userLoginService;
        private readonly IComplaintUserService complaintUserService;
        private readonly ILessonLearntService lessonLearntService;
        private IProjectService projectService;

        public ComplaintController(IComplaintService complaintService, IUserLoginService userLoginService, IComplaintUserService complaintUserService, ILessonLearntService _lessonLearntService, IProjectService _projectService)
        {
            this.complaintService = complaintService;
            this.userLoginService = userLoginService;
            this.complaintUserService = complaintUserService;
            this.lessonLearntService = _lessonLearntService;
            this.projectService = _projectService;
        }

        #endregion

        #region Complaint Index 

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Index(ComplaintDto model)
        {
            int currentUserId = CurrentUser.Uid;
            if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
            {
                //var users = userLoginService.GetUsersByPM(PMUserId).Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() }).ToList();
                //users.Insert(0, new SelectListItem() { Text = "Select Employee", Value = "0" });
                //ViewBag.EmployeeList = users;

                var currentUserEmployeeComplaintId = complaintService.GetComplaintUserIdByPMId(currentUserId)?.Select(x => x.Id).Distinct().ToList();
                var employeeList = new List<SelectListItem>();
                if (currentUserEmployeeComplaintId.Any())
                {

                    List<int> EmployeeIds = complaintService.GetEmployeeByIds(currentUserEmployeeComplaintId)?.Select(x => x.UserLoginId).Distinct().ToList();
                    //employeeList = projectService.GetProjectListByCurrentUser(EmployeeIds).Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.ProjectId.ToString() }).ToList();
                    employeeList = userLoginService.GetProjectListByCurrentUserId(EmployeeIds).Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.Uid.ToString() }).ToList();

                }

                employeeList.Insert(0, new SelectListItem() { Text = "Employee", Value = "0" });
                ViewBag.EmployeeList = employeeList;

                var currentUserProjectComplaintId = complaintService.GetComplaintUserIdByPMId(currentUserId)?.Select(x => x.ProjectId).Distinct().ToList();
                var projectList = new List<SelectListItem>();
                if (currentUserProjectComplaintId.Any())
                {

                    List<int?> projIds = currentUserProjectComplaintId;
                    projectList = projectService.GetProjectListByCurrentUser(projIds).Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.ProjectId.ToString() }).ToList();

                }
                projectList.Insert(0, new SelectListItem() { Text = "Project", Value = "0" });
                ViewBag.ProjectList = projectList;

                model.Priority = WebExtensions.GetSelectList<Enums.ComplainPrioritys>();
            }
            else
            {
                var currentUserProjectComplaintId = complaintService.GetComplaintUserIdById(currentUserId)?.Select(x => x.ComplaintId).Distinct().ToList() ?? new List<int>();
                var projectList = new List<SelectListItem>();
                if (currentUserProjectComplaintId.Any())
                {

                    List<int?> projIds = complaintService.GetProjectByComplainIds(currentUserProjectComplaintId)?.Select(x => x.ProjectId).Distinct().ToList();
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
            var pagingServices = new PagingService<Complaint>(request.Start, request.Length);
            var expr = PredicateBuilder.True<Complaint>();

            if (ProjectId > 0)
            {
                expr = expr.And(e => e.ProjectId == ProjectId);
            }
            if (EmployeeId > 0)
            {
                expr = expr.And(x => x.ComplaintUser.Any(a => a.UserLogin.Uid == EmployeeId));
            }
            if (InternalTypeId > 0)
            {
                expr = expr.And(e => e.ComplaintType == InternalTypeId);
            }
            if (CLientTypeId > 0)
            {
                expr = expr.And(e => e.ComplaintType == CLientTypeId);
            }
            if (PriorityId > 0)
            {
                expr = expr.And(e => e.Priority == PriorityId);
            }

            expr = expr.And(e => e.IsDelete == false);

            if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
            {
                expr = expr.And(x => x.AddedBy == CurrentUser.Uid || x.AddedByNavigation.PMUid == PMUserId);
            }
            else
            {
                //expr = expr.And(x => x.AddedBy == CurrentUser.Uid);
                expr = expr.And(x => x.ComplaintUser.FirstOrDefault().UserLoginId == CurrentUser.Uid);
            }

            if (request.Search.Value.HasValue())
            {
                string searchValue = request.Search.Value.Trim().ToLower();
                // expr = expr.And(x =>  x.AddedByNavigation.Name != null && x.AddedByNavigation.Name.ToLower().Contains(searchValue));
                expr = expr.And(x => x.ComplaintUser.Any(a => a.UserLogin.Name.ToLower().Contains(searchValue)));
            }

            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.ModifyDate);
            };

            int totalCount = 0;
            var isPMUser = IsPM;
            var response = complaintService.GetComplaintByPaging(out totalCount, pagingServices);

            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                rowIndex = (index + 1) + (request.Start),
                Name = GetUserNames(r.Id),
                Id = r.Id,
                ComplaintType = Extensions.GetDescription((Enums.ComplainType)r.ComplaintType),
                PriorityType = Extensions.GetDescription((Enums.ComplainPriority)r.Priority),
                ClientComplaint = (r.ComplaintType == (int)Enums.ComplainType.Internal ? r.TlExplanation : r.ClientComplain),
                AddedBy = r.AddedByNavigation?.Name,
                AddedDate = r.AddedDate.ToFormatDateString("MMM dd, yyyy"),
                AreaofImprovement = r.AreaofImprovement,
                AllowDelete = isPMUser,
                project = r.ProjectId.HasValue ? projectService.GetProjectById(r.ProjectId.Value).Name : ""
            }));
        }

        #endregion

        #region [ Get User Name ]
        public string GetUserNames(int id)
        {
            string usersArray = string.Empty;
            var complaintUsers = complaintService.GetComplaintUserById(id);
            if (complaintUsers.Count > 0)
            {
                List<int> UserIdList = new List<int>();
                int[] AssignUserIds;
                foreach (var item in complaintUsers)
                {
                    UserIdList.Add(item.UserLoginId);
                }

                AssignUserIds = UserIdList.ToArray();
                var selectedUsers = userLoginService.GetUserInfoByID(AssignUserIds);

                usersArray = string.Join(", ", selectedUsers.Select(t => t.Name).ToArray());

            }

            return usersArray;
        }

        #endregion



        #region ADD/EDIT Complaint

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Add(int? id)
        {
            if (CurrentUser.RoleId == (int)Enums.UserRoles.HRBP || CurrentUser.RoleId == (int)Enums.UserRoles.PM)
            {
                ComplaintDto model = new ComplaintDto();
                var users = userLoginService.GetUsersByPM(PMUserId);
                model.EmployeeList = users.Select(n => new SelectListItem { Text = n.Name, Value = n.Uid.ToString() }).ToList();
                model.ComplaintType = WebExtensions.GetSelectList<Enums.ComplainType>();
                model.Priority = WebExtensions.GetSelectList<Enums.ComplainPriority>();
                model.ComplaintTypeId = (int)Enums.ComplainType.Internal;
                model.PriorityId = (int)Enums.ComplainPriority.Low;

                int pmUId = CurrentUser.RoleId == (int)Enums.UserRoles.PM ? CurrentUser.Uid : CurrentUser.RoleId == (int)Enums.UserRoles.PMO ? CurrentUser.Uid : CurrentUser.PMUid;
                var projectList = projectService.GetProjectListByPmuid(pmUId, CurrentUser.Uid).Select(x => new SelectListItem { Text = x.CRMProjectId > 0 ? x.Name.ToString() + "[" + x.CRMProjectId + "]" : x.Name.ToString(), Value = x.ProjectId.ToString() }).ToList();
                projectList.Insert(0, new SelectListItem() { Text = "--Project--", Value = "0" });
                ViewBag.ProjectList = projectList;

                if (id.HasValue && id.Value > 0)
                {
                    var entity = complaintService.GetComplaintById(id.Value);
                    if (entity != null)
                    {

                        if (entity.AddedBy == CurrentUser.Uid || (IsPM && entity.AddedByNavigation.PMUid == PMUserId))
                        {


                            var complaintUsers = complaintService.GetComplaintUserById(id.Value);
                            if (complaintUsers.Count > 0)
                            {
                                List<int> UserIdList = new List<int>();

                                foreach (var item in complaintUsers)
                                {
                                    UserIdList.Add(item.UserLoginId);
                                }

                                model.Employees = UserIdList.ToArray();
                            }

                            model.Id = entity.Id;
                            // model.EmployeeId = entity.EmployeeId;

                            model.ComplaintTypeId = entity.ComplaintType;
                            model.PriorityId = entity.Priority;
                            model.TlComplainDate = entity.TlComplainDate.ToFormatDateString("dd/MM/yyyy");
                            model.TlExplanation = entity.TlExplanation;
                            model.DeveloperComplainDate = entity.DeveloperComplainDate.ToFormatDateString("dd/MM/yyyy");
                            model.DeveloperExplanation = entity.DeveloperExplanation;
                            model.ClientComplain = entity.ClientComplain;
                            model.ClientComplainDate = entity.ClientComplainDate.ToFormatDateString("dd/MM/yyyy");
                            model.AreaofImprovement = entity.AreaofImprovement;
                            model.LessionLearned = entity.LessionLearned;
                            model.ProjectId = entity.ProjectId.Value;
                        }
                        else
                        {
                            return CustomErrorView("Invalid access");
                        }
                    }
                    else
                    {
                        return CustomErrorView("Unable to find record");
                    }
                }

                return View("AddEditComplaint", model);
            }
            else
            {
                return AccessDenied();
            }
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult Add(ComplaintDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.AddedBy = CurrentUser.Uid;
                    var result = complaintService.Save(model);

                    if (result != null && model.LessionLearned != null && !String.IsNullOrEmpty(model.LessionLearned.Trim()) && (CurrentUser.PMUid == SiteKey.AshishTeamPMUId || CurrentUser.Uid == SiteKey.AshishTeamPMUId))
                    {
                        LessonLearntDto lessonLearntDto = new LessonLearntDto();
                        lessonLearntDto.CreatedById = CurrentUser.Uid;
                        lessonLearntDto.WhatLearnt = model.LessionLearned;

                        var resultLessonLearnt = lessonLearntService.Save(lessonLearntDto);
                        if (resultLessonLearnt != null)
                        {
                            if (model.SendEmailLessionLearned)
                            {
                                SendLessonLearntEmailToUsers(resultLessonLearnt.Id);
                            }
                        }
                    }

                    if (result != null && model.SendEmailEmployee)
                    {
                        var status = SendEmailToRespectivePersions(model);
                    }

                    ShowSuccessMessage("Success", "Complaint saved successfully", false);

                    return NewtonSoftJsonResult(new RequestOutcome<string>
                    {
                        IsSuccess = true,
                        Message = "Complaint saved successfully",
                        RedirectUrl = Url.Action("index")
                    });
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

        #region [Private Methods]

        private string SendEmailToRespectivePersions(ComplaintDto model)
        {
            try
            {
                string Employees = "";
                //List<string> rsEmails = new List<string>();
                List<string> emailIdsRespectivePerson = new List<string>();
                List<string> emailIdsTLAndPMPerson = new List<string>();
                //UserLogin pmInfo = null;
                if (model.Employees != null && model.Employees.Length > 0)
                {
                    UserLogin u = null;
                    foreach (var item in model.Employees)
                    {
                        u = userLoginService.GetUserInfoByID(item);
                        if (u != null && !String.IsNullOrEmpty(u.EmailOffice) && !emailIdsRespectivePerson.Contains(u.EmailOffice))
                        {
                            emailIdsRespectivePerson.Add(u.EmailOffice);
                        }

                        if (u != null && u.TLId > 0)
                        {
                            var tl = userLoginService.GetUserInfoByID(u.TLId ?? 0);
                            if (tl != null && !String.IsNullOrEmpty(tl.EmailOffice) && !emailIdsTLAndPMPerson.Contains(tl.EmailOffice))
                            {
                                emailIdsTLAndPMPerson.Add(tl.EmailOffice);
                            }
                        }

                        Employees += u.Name.ToString() + ", ";
                    }

                    if (u != null && u.PMUid > 0)
                    {
                        var pmuser = userLoginService.GetUserInfoByID(u.PMUid ?? 0);
                        if (pmuser != null && !String.IsNullOrEmpty(pmuser.EmailOffice) && !emailIdsTLAndPMPerson.Contains(pmuser.EmailOffice))
                        {
                            emailIdsTLAndPMPerson.Add(pmuser.EmailOffice);
                        }
                    }
                }
                Employees = Employees.Remove(Employees.Length - 2, 2);

                string ComplainType = ((Enums.ComplainType)model.ComplaintTypeId).ToString();
                string priority = ((Enums.ComplainPriority)model.PriorityId).ToString();

                var v0 = "";
                if (ComplainType == "Internal")
                {
                    v0 = "none";
                }
                else
                {
                    v0 = "block";
                }
                var v1 = priority;
                var v2 = Employees;
                var v3 = model.TlComplainDate;
                var v4 = model.TlExplanation;
                var v5 = model.AreaofImprovement;
                var v6 = model.DeveloperComplainDate;
                var v7 = model.DeveloperExplanation;
                var v8 = model.ClientComplainDate;
                var v9 = model.ClientComplain;
                var v10 = CurrentUser.Name;
                var v11 = "block";
                if (string.IsNullOrEmpty(model.AreaofImprovement))
                {
                    v11 = "none";
                }
                var v12 = "block";
                if (string.IsNullOrEmpty(model.DeveloperExplanation))
                {
                    v12 = "none";
                }
                var ValueArray = new string[] { v0, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12 };
                FlexiMail objSendMail = new FlexiMail();
                objSendMail.Subject = "Complaint Email";
                objSendMail.ValueArray = ValueArray;
                objSendMail.MailBodyManualSupply = true;
                objSendMail.MailBody = objSendMail.GetHtml("ComplaintEmail.html");
                objSendMail.From = SiteKey.From;


                if (emailIdsRespectivePerson != null && emailIdsRespectivePerson.Count > 0)
                {
                    objSendMail.To = string.Join(",", emailIdsRespectivePerson);

                    if (emailIdsTLAndPMPerson != null && emailIdsTLAndPMPerson.Count > 0)
                    {
                        objSendMail.CC = string.Join(",", emailIdsTLAndPMPerson);
                    }

                    objSendMail.Send();
                    return "Success";
                }
                return "No email found to send.";
            }
            catch (Exception ex)
            {
                return (ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }

        private string SendLessonLearntEmailToUsers(int id)
        {
            try
            {
                int pmUid = CurrentUser.PMUid == 0 ? CurrentUser.Uid : CurrentUser.PMUid;

                var lesson = lessonLearntService.GetById(id);
                var allusers = userLoginService.GetRoleByPM(pmUid, RoleValidator.roleIds);
                var emailIds = allusers.Where(x => x.RoleId != (int)Enums.UserRoles.PM && !String.IsNullOrEmpty(x.EmailOffice)).Select(x => x.EmailOffice).Distinct().ToList();
                var PMEmail = allusers.FirstOrDefault(x => x.RoleId == (int)Enums.UserRoles.PM && !String.IsNullOrEmpty(x.EmailOffice)).EmailOffice;
                if (emailIds.Any())
                {
                    FlexiMail objSendMail = new FlexiMail();
                    objSendMail.ValueArray = new string[]
                   {
                       $"{(lesson.Project !=null ? $" in <b>{lesson.Project?.Name}</b> project" : "")}",
                       $"{lesson.WhatLearnt}",
                       $"{lesson.CreatedBy.Name.ToLower()}"
                   };
                    objSendMail.Subject = $"Lesson Learned";
                    objSendMail.MailBodyManualSupply = true;
                    objSendMail.MailBody = objSendMail.GetHtml("LessonLearnt.html");
                    objSendMail.To = PMEmail;
                    objSendMail.CC = string.Join(";", emailIds);
                    objSendMail.From = lesson.CreatedBy.EmailOffice;
                    objSendMail.Send();
                }
                return "Success";
            }
            catch (Exception ex)
            {
                return (ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }
        #endregion

        #region Delete Complaint

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Complaint?",
                Size = Enums.ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Complaint?" },
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
                    var Complaint = complaintService.GetComplaintById(id);
                    if (Complaint != null && (Complaint.AddedBy == CurrentUser.Uid ||
                            (CurrentUser.RoleId == (int)Enums.UserRoles.PM && Complaint.AddedByNavigation.PMUid == PMUserId)))
                    {
                        complaintService.Delete(id);
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record deleted successfully.", IsSuccess = true });
                    }
                }

                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = "Unable to delete this complaint" });
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = ex.Message });
            }
        }

        #endregion
    }
}