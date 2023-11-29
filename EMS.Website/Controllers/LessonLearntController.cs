using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.Models.Others;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class LessonLearntController : BaseController
    {
        #region [Constructor and Member]
        private readonly IProjectService projectService;
        private readonly ILessonLearntService lessonLearntService;
        private readonly IUserLoginService userLoginService;

        public LessonLearntController(IProjectService _projectService,
            ILessonLearntService _lessonLearntService, IUserLoginService _userLoginService)
        {
            this.projectService = _projectService;
            this.lessonLearntService = _lessonLearntService;
            this.userLoginService = _userLoginService;
        }
        #endregion

        #region [Index]
        //[CustomAuthorization(IsAshishTeam: true)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        //[CustomAuthorization(IsAshishTeam: true)]
        public IActionResult Index(IDataTablesRequest request, string SearchName)
        {
            var pagingServices = new PagingService<LessonLearnt>(request.Start, request.Length);
            var expr = PredicateBuilder.True<LessonLearnt>();

            if (request.Search.Value.HasValue())
            {
                string searchValue = request.Search.Value.Trim().ToLower();
                expr = expr.And(x => x.Project.Name.Contains(searchValue) || x.CreatedBy.Name.Contains(searchValue) || x.WhatLearnt.Contains(searchValue));
            }
            //if (!string.IsNullOrEmpty(SearchName))
            //{
            //    string searchValue = SearchName;
            //    expr = expr.And(x => x.Project.Name.Contains(searchValue) || x.CreatedBy.Name.Contains(searchValue) || x.WhatLearnt.Contains(searchValue));
            //}

            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.Created);
            };

            int totalCount = 0;
            var response = lessonLearntService.GetByPaging(out totalCount, pagingServices);

            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                rowIndex = (index + 1) + (request.Start),
                Id = r.Id,
                ProjectName = r.Project?.Name,
                r.WhatLearnt,
                CreatedByName = r.CreatedBy.Name,
                CreatedDate = r.Created.ToFormatDateString("dd-MMM-yyyy HH:mm tt"),
                r.Modified,
                AllowEdit = (CurrentUser.RoleId == (int)Enums.UserRoles.PM || r.CreatedById == CurrentUser.Uid),
                AllowDelete = (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
            }));
        }
        #endregion

        #region [Add Edit Get and Post]
        [HttpGet]
       // [CustomAuthorization(IsAshishTeam: true)]
        public ActionResult AddEdit(int? id)
        {
            try
            {
                var model = new LessonLearntDto();
                if (id.HasValue)
                {
                    var result = lessonLearntService.GetDtoById(id ?? 0);
                    if ((CurrentUser.RoleId == (int)Enums.UserRoles.PM || result.CreatedById == CurrentUser.Uid))
                    {
                        model = result;
                    }
                }
                model.ProjectList = projectService.GetProjectListByPmuid(CurrentUser.RoleId == (int)Enums.UserRoles.PM ? CurrentUser.Uid : CurrentUser.PMUid)
                                    .Select(p => new SelectListItem
                                    {
                                        Text = $"{p.Name} [{p.CRMProjectId}]",
                                        Value = p.ProjectId.ToString()
                                    }).ToList();

                return PartialView(model);
            }
            catch (Exception ex)
            {
                return CustomErrorView(ex.Message);
            }
        }

        [HttpPost]
       // [CustomAuthorization(IsAshishTeam: true)]
        public ActionResult AddEdit(LessonLearntDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.CreatedById = CurrentUser.Uid;
                    var result = lessonLearntService.Save(model);
                    if (result != null)
                    {
                        if (model.IsSendEmail)
                        {
                            SendEmailToUsers(result.Id);
                        }
                        //ShowSuccessMessage("Success", "Lesson Learnt saved successfully", false);
                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = "Lesson learned record saved successfully"
                        });
                    }
                    else
                    {
                        //ShowErrorMessage("Error", "Lesson Learnt not save", false);
                        //return View(model);
                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = false,
                            Message = "Lesson learned record not saved"
                        });
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

        #region Delete Lesson Learnt
        [HttpGet]
       // [CustomAuthorization(IsAshishTeam: true)]
        public ActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete?",
                Size = Enums.ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Record ?" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
       // [CustomAuthorization(IsAshishTeam: true)]
        public ActionResult delete(int id)
        {
            try
            {
                if (id > 0)
                {
                    var result = lessonLearntService.DeleteById(id);
                    if (result)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record deleted successfully.", IsSuccess = true });
                    }
                    else
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = "Unable to delete task" });
                    }
                }

                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = "Unable to delete task" });
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = ex.Message });
            }
        }
        #endregion

        #region [Private Methods]
        private void SendEmailToUsers(int id)
        {
            try
            {
                int pmUid = CurrentUser.PMUid == 0 ? CurrentUser.Uid : CurrentUser.PMUid;
                var lesson = lessonLearntService.GetById(id);
                
                var allusers = userLoginService.GetRoleByPM(pmUid, RoleValidator.roleIds);
                var emailIds = allusers.Where(x => x.RoleId != (int)Enums.UserRoles.PM).Select(x => x.EmailOffice).Distinct().ToList();
                var PMEmail = allusers.FirstOrDefault(x => x.RoleId == (int)Enums.UserRoles.PM).EmailOffice;
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
            }
            catch
            {

            }
        }
        #endregion
    }
}