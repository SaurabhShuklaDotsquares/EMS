using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class LessonLearnedController : BaseController
    {
        #region Fields and Constructor

        private readonly IProjectLessonLearnedService projectLessonLearnedService;
        private readonly IProjectService projectService;
        private readonly IUserLoginService userLoginService;

        public LessonLearnedController(IProjectLessonLearnedService _projectLessonLearnedService, IProjectService _projectService, IUserLoginService _userLoginService)
        {
            projectLessonLearnedService = _projectLessonLearnedService;
            projectService = _projectService;
            userLoginService = _userLoginService;
        }

        #endregion

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Index()
        {
            var model = new ProjectLessonLearnedIndexDto();

            if (CurrentUser.IsSPEG)
            {
                model.PMUserList = userLoginService.GetUserByRole((int)Enums.UserRoles.PM, true)
                                    .Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() })
                                    .ToList();
            }

            return View(model);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult Index(IDataTablesRequest request, int? pmId)
        {
            var pagingServices = new PagingService<ProjectLesson>(request.Start, request.Length);
            var filterExpr = PredicateBuilder.True<ProjectLesson>();
            var currentUserId = CurrentUser.Uid;

            if (!CurrentUser.IsSPEG)
            {
                if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
                {
                    filterExpr = filterExpr.And(x => x.UserLogin.PMUid == currentUserId || x.CreateByUid == currentUserId);
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
                    filterExpr = filterExpr.And(x => x.UserLogin.TLId == currentUserId || x.CreateByUid == currentUserId);
                }
                else
                {
                    filterExpr = filterExpr.And(x => x.CreateByUid == currentUserId);
                }
            }

            if (pmId.HasValue && pmId.Value > 0)
            {
                filterExpr = filterExpr.And(x => x.Project.PMUid == pmId.Value);
            }

            pagingServices.Filter = filterExpr;

            pagingServices.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        default:
                            return o.OrderByColumn(item, c => c.CreateDate);
                    }
                }
                return o.OrderByDescending(c => c.ModifyDate);
            };

            int totalCount = 0;
            var response = projectLessonLearnedService.GetLessonsByPaging(out totalCount, pagingServices);

            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                rowIndex = (index + 1) + (request.Start),
                Id = r.Id,
                CreateBy = r.UserLogin.Name,
                ProjectName = $"{r.Project.Name} [{r.Project.CRMProjectId}]",
                CreateDate = r.CreateDate.ToFormatDateString("dd MMM yyyy, hh:mm tt")
            }));
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult AddEdit(int? id)
        {
            try
            {
                if (id.HasValue && id.Value > 0)
                {
                    return CustomErrorView("Edit lesson is pending", messageType: Enums.MessageType.Info);
                }

                var model = new ProjectLessonDto();
                model.ProjectList = projectService.GetProjectListByPmuid(CurrentUser.RoleId == (int)Enums.UserRoles.PM ? CurrentUser.Uid : CurrentUser.PMUid)
                                    .Select(p => new SelectListItem
                                    {
                                        Text = $"{p.Name} [{p.CRMProjectId}]",
                                        Value = p.ProjectId.ToString()
                                    }).ToList();

                var topics = projectLessonLearnedService.GetLessonTopics();

                model.LearnedLessons = projectLessonLearnedService.GetLessonTopics()
                                    .Select(c => new ProjectLessonLearnedDto
                                    {
                                        ProjectLessonTopicId = c.Id,
                                        ProjectLessonTopicName = c.Name,
                                        TopicGroup = c.TopicGroup,
                                        TopicGroupName = ((Enums.ProjectLessonTopicGroup)c.TopicGroup).GetDescription(),
                                    }).OrderBy(x => x.ProjectLessonTopicId).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                return CustomErrorView(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult AddEdit(ProjectLessonDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.CurrentUserId = CurrentUser.Uid;

                    model.LearnedLessons = model.LearnedLessons.FindAll(x => x.ProjectLessonTopicId > 0 &&
                         (!string.IsNullOrWhiteSpace(x.WhatLearned) ||
                         !string.IsNullOrWhiteSpace(x.WhatWentBad) ||
                         !string.IsNullOrWhiteSpace(x.WhatWentGood) ||
                         !string.IsNullOrWhiteSpace(x.WhatImpacted)));

                    if (model.LearnedLessons.Any())
                    {
                        var result = projectLessonLearnedService.Save(model);

                        if (result != null && result.Id > 0)
                        {
                            ShowSuccessMessage("Success", "Record saved successfully", false);
                            return NewtonSoftJsonResult(new RequestOutcome<string>
                            {
                                IsSuccess = true,
                                RedirectUrl = Url.Action("Index")
                            });
                        }
                        else
                        {
                            return MessagePartialView("Unable to save record");
                        }
                    }
                    else
                    {
                        return MessagePartialView("No lesson found to save. Please add any one");
                    }
                }
                catch (Exception ex)
                {
                    return MessagePartialView(ex.Message);
                }
            }
            else
            {
                return CreateModelStateErrors();
            }
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Detail(int id)
        {
            try
            {
                if (id > 0)
                {
                    var lessonEntity = projectLessonLearnedService.GetLessonById(id);
                    
                    if (lessonEntity != null)
                    {
                        if (!CurrentUser.IsSPEG && lessonEntity.CreateByUid != CurrentUser.Uid)
                        {
                            if ((CurrentUser.RoleId == (int)Enums.UserRoles.PM && lessonEntity.UserLogin.PMUid == CurrentUser.Uid) ||
                                (RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_Sales_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_HR_DesignationIds.Contains(CurrentUser.DesignationId)
                                && lessonEntity.UserLogin.TLId == CurrentUser.Uid))
                            {
                                // Do nothing
                            }
                            else
                            {
                                return AccessDenied();
                            }
                        }

                        var model = new ProjectLessonDto
                        {
                            ProjectName = $"{lessonEntity.Project.Name} [{lessonEntity.Project.CRMProjectId}]",
                            CreateByName = lessonEntity.UserLogin.Name,
                            CreateDate = lessonEntity.CreateDate.ToFormatDateString("dd MMM yyyy, hh:mm tt"),
                        };

                        foreach (var item in lessonEntity.ProjectLessonLearneds)
                        {
                            model.LearnedLessons.Add(new ProjectLessonLearnedDto
                            {
                                ProjectLessonId = item.ProjectLessonId,
                                ProjectLessonTopicId = item.ProjectLessonTopicId,
                                ProjectLessonTopicName = item.ProjectLessonTopic.Name,
                                TopicGroup = item.ProjectLessonTopic.TopicGroup,
                                TopicGroupName = ((Enums.ProjectLessonTopicGroup)item.ProjectLessonTopic.TopicGroup).GetDescription(),
                                WhatLearned = item.WhatLearned,
                                WhatWentBad = item.WhatWentBad,
                                WhatWentGood = item.WhatWentGood,
                                WhatImpacted = item.WhatImpacted
                            });
                        }

                        model.LearnedLessons = model.LearnedLessons.OrderBy(o => o.ProjectLessonTopicId).ToList();

                        return View(model);
                    }
                    else
                    {
                        return CustomErrorView("Record not found");
                    }
                }
                else
                {
                    return CustomErrorView("Invalid lesson id");
                }
            }
            catch (Exception ex)
            {
                return CustomErrorView(ex.Message);
            }
        }

    }
}