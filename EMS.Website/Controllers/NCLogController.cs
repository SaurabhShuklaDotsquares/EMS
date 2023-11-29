using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class NCLogController : BaseController
    {
        #region Fields and Constructor

        private readonly IProjectNCLogService projectNCLogService;
        private readonly IProjectService projectService;
        private readonly IUserLoginService userLoginService;

        public NCLogController(IProjectNCLogService _projectNCLogService, IProjectService _projectService, IUserLoginService _userLoginService)
        {
            projectNCLogService = _projectNCLogService;
            projectService = _projectService;
            userLoginService = _userLoginService;
        }

        #endregion

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Index()
        {
            var model = new ProjectNCLogIndexDto();
            var filterExpr = PredicateBuilder.True<ProjectNCLog>();

            if (CurrentUser.IsSPEG)
            {
                model.PMUserList = userLoginService.GetUserByRole((int)Enums.UserRoles.PM, true)
                                    .Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() })
                                    .ToList();
            }
            else
            {
                var currentUserId = CurrentUser.Uid;

                if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
                {
                    filterExpr = filterExpr.And(x => x.Project.PMUid == currentUserId || x.AuditeeUid == currentUserId || x.AuditorUid == currentUserId);
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
                    filterExpr = filterExpr.And(x => x.UserLogin.TLId == currentUserId || x.AuditeeUid == currentUserId || x.AuditorUid == currentUserId);
                }
                else
                {
                    filterExpr = filterExpr.And(x => x.AuditeeUid == currentUserId || x.AuditorUid == currentUserId);
                }
            }

            model.ProjectList = projectNCLogService.GetProjecsFromNCLog(filterExpr).Select(x => new SelectListItem
            {
                Text = $"{x.Name} [{x.CRMProjectId}]",
                Value = x.ProjectId.ToString()
            }).ToList();

            return View(model);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult Index(IDataTablesRequest request, int? pmId, int? projectId)
        {
            var pagingServices = new PagingService<ProjectNCLog>(request.Start, request.Length);
            var filterExpr = PredicateBuilder.True<ProjectNCLog>();
            var currentUserId = CurrentUser.Uid;

            if (!CurrentUser.IsSPEG)
            {
                if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
                {
                    filterExpr = filterExpr.And(x => x.Project.PMUid == currentUserId || x.AuditeeUid == currentUserId || x.AuditorUid == currentUserId);
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
                    filterExpr = filterExpr.And(x => x.UserLogin.TLId == currentUserId || x.AuditeeUid == currentUserId || x.AuditorUid == currentUserId);
                }
                else
                {
                    filterExpr = filterExpr.And(x => x.AuditeeUid == currentUserId || x.AuditorUid == currentUserId);
                }
            }

            if (pmId.HasValue && pmId.Value > 0)
            {
                filterExpr = filterExpr.And(x => x.Project.PMUid == pmId.Value);
            }

            if (projectId.HasValue && projectId.Value > 0)
            {
                filterExpr = filterExpr.And(x => x.ProjectId == projectId.Value);
            }

            pagingServices.Filter = filterExpr;

            pagingServices.Sort = (o) =>
            {
                return o.OrderBy(c => c.Status).OrderByDescending(c => c.AuditDate);
            };

            int totalCount = 0;
            var response = projectNCLogService.GetLogsByPaging(out totalCount, pagingServices);

            return DataTablesJsonResult(totalCount, request, response.Select((log, index) => new
            {
                rowIndex = (index + 1) + (request.Start),
                Id = log.Id,
                ProjectName = $"{log.Project.Name} [{log.Project.CRMProjectId}]",
                AuditDesc = log.AuditDesc,
                AuditCycle = $"{(Enums.ProjectAuditCycle)log.AuditCycle} [{(Enums.ProjectAuditType)log.AuditType}]",
                AuditDate = log.AuditDate.ToFormatDateString("dd MMM yyyy"),
                FollowUpDate = log.FollowUpDate.ToFormatDateString("dd MMM yyyy"),
                ClosedDate = log.ClosedDate.ToFormatDateString("dd MMM yyyy"),
                Status = ((Enums.ProjectAuditStatus)log.Status).ToString(),
                Auditor = log.UserLogin1.Name,
                Auditee = log.UserLogin.Name,
                DaysTaken = log.CompletedDate.HasValue ? log.CompletedDate.Value.Subtract(log.AuditDate).Days.ToString() : "",
                ActionAllowed= log.AuditeeUid == currentUserId && log.Status == (byte)Enums.ProjectAuditStatus.Open,
                EditAllowed = log.AuditorUid == currentUserId && log.Status == (byte)Enums.ProjectAuditStatus.Open
            }));
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult AddEdit(int? id)
        {
            try
            {
                var model = new ProjectNCLogDto();
                model.ProjectList = projectService.GetProjectList()
                                    .Select(p => new SelectListItem
                                    {
                                        Text = $"{p.Name} [{p.CRMProjectId}]",
                                        Value = p.ProjectId.ToString()
                                    }).ToList();

                model.AuditPAList = projectNCLogService.GetAuditPAs()
                                   .Select(p => new SelectListItem
                                   {
                                       Text = p.Name,
                                       Value = p.Id.ToString()
                                   }).ToList();

                model.AuditeeList = userLoginService.GetUsers(true)
                                   .Select(p => new SelectListItem
                                   {
                                       Text = p.Name,
                                       Value = p.Uid.ToString()
                                   }).ToList();

                model.AuditCycleList = Extensions.EnumToDictionaryWithDescription(typeof(Enums.ProjectAuditCycle))
                                               .Select(x => new SelectListItem { Text = x.Key, Value = x.Value.ToString() })
                                               .ToList();

                model.AuditStatusList = Extensions.EnumToDictionaryWithDescription(typeof(Enums.ProjectAuditStatus))
                                               .Select(x => new SelectListItem { Text = x.Key, Value = x.Value.ToString() })
                                               .ToList();

                model.AuditTypeList = Extensions.EnumToDictionaryWithDescription(typeof(Enums.ProjectAuditType))
                                               .Select(x => new SelectListItem { Text = x.Key, Value = x.Value.ToString() })
                                               .ToList();

                if (id.HasValue && id.Value > 0)
                {
                    var ncLogEntity = projectNCLogService.GetNCLogById(id.Value);

                    if (ncLogEntity != null)
                    {
                        if (ncLogEntity.AuditorUid == CurrentUser.Uid && ncLogEntity.Status == (byte)Enums.ProjectAuditStatus.Open)
                        {
                            model.AuditCycle = ncLogEntity.AuditCycle;
                            model.AuditDate = ncLogEntity.AuditDate.ToFormatDateString("dd/MM/yyyy");
                            model.AuditDesc = ncLogEntity.AuditDesc;
                            model.AuditeeUid = ncLogEntity.AuditeeUid;
                            model.AuditType = ncLogEntity.AuditType;
                            model.FollowUpDate = ncLogEntity.FollowUpDate.ToFormatDateString("dd/MM/yyyy");
                            model.Id = ncLogEntity.Id;
                            model.ProjectAuditPAId = ncLogEntity.ProjectAuditPAId;
                            model.ProjectId = ncLogEntity.ProjectId;
                        }
                        else
                        {
                            return MessagePartialView("Invalid Auditor access or audit has been Completed or Closed");
                        }
                    }
                    else
                    {
                        return MessagePartialView("Unable to find record");
                    }
                }

                return PartialView("_AddEdit", model);
            }
            catch (Exception ex)
            {
                return CustomErrorView(ex.Message);
            }
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult AddEdit(ProjectNCLogDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.CurrentUserId = CurrentUser.Uid;
                    model.Status = (byte)Enums.ProjectAuditStatus.Open;

                    var result = projectNCLogService.Save(model);

                    if (result != null && result.Id > 0)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = "NC Log saved successfully"
                        });
                    }
                    else
                    {
                        return MessagePartialView("Unable to save record");
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
        public ActionResult AuditeeComments(int id)
        {
            try
            {
                if (id > 0)
                {
                    var nclogEntity = projectNCLogService.GetNCLogById(id);

                    if (nclogEntity != null)
                    {
                        var model = new ProjectNCLogAuditeeDto
                        {
                            Id = nclogEntity.Id,
                            ProjectName = $"{nclogEntity.Project.Name} [{nclogEntity.Project.CRMProjectId}]",
                            AuditCycle = $"{(Enums.ProjectAuditCycle)nclogEntity.AuditCycle}",
                            AuditType = $"{(Enums.ProjectAuditType)nclogEntity.AuditType}",
                            AuditDesc = nclogEntity.AuditDesc.NewLineToHtmlBreak(),
                            AuditorName = nclogEntity.UserLogin1.Name,
                            ProjectAuditPA = nclogEntity.ProjectAuditPa.Name,
                            AuditAction = nclogEntity.AuditAction,
                            RootCause = nclogEntity.RootCause,
                            Status = nclogEntity.Status,
                            AuditDate = nclogEntity.AuditDate.ToFormatDateString("dd MMM yyyy"),
                            FollowUpDate = nclogEntity.FollowUpDate.ToFormatDateString("dd MMM yyyy"),
                            AuditeeName = nclogEntity.UserLogin.Name,
                            CompletedDate = nclogEntity.CompletedDate.ToFormatDateString("dd MMM yyyy"),
                            ClosedDate = nclogEntity.ClosedDate.ToFormatDateString("dd MMM yyyy"),
                            EditAllowed = nclogEntity.AuditeeUid == CurrentUser.Uid && nclogEntity.Status == (byte)Enums.ProjectAuditStatus.Open,
                            CloseAllowed = nclogEntity.AuditorUid == CurrentUser.Uid && nclogEntity.Status == (byte)Enums.ProjectAuditStatus.Completed
                        };
                        if (model.EditAllowed)
                        {
                            model.AuditStatusList.Add(new SelectListItem { Text = Enums.ProjectAuditStatus.Open.ToString(), Value = ((byte)Enums.ProjectAuditStatus.Open).ToString() });
                            model.AuditStatusList.Add(new SelectListItem { Text = Enums.ProjectAuditStatus.Completed.ToString(), Value = ((byte)Enums.ProjectAuditStatus.Completed).ToString() });
                        }

                        return PartialView("_AuditeeComments", model);
                    }
                    else
                    {
                        return MessagePartialView("Record not found");
                    }
                }
                else
                {
                    return MessagePartialView("Invalid log id");
                }
            }
            catch (Exception ex)
            {
                return CustomErrorView(ex.Message);
            }
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult AuditeeComments(ProjectNCLogAuditeeDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.CurrentUserId = CurrentUser.Uid;

                    var result = projectNCLogService.UpdateStatus(model);

                    if (result != null && result.Id > 0)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = result.Status == (byte)Enums.ProjectAuditStatus.Completed ? "NC Log marked as completed successfully" : "NC Log comments added successfully"
                        });
                    }
                    else
                    {
                        return MessagePartialView("Unable to update record");
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

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult AuditClose(ProjectNCLogDto model)
        {
            if (model.Id > 0 && model.ClosedDate.HasValue())
            {
                try
                {
                    model.CurrentUserId = CurrentUser.Uid;
                    var result = projectNCLogService.CloseAudit(model);

                    if (result != null && result.Id > 0)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = "NC Log marked as closed successfully"
                        });
                    }
                    else
                    {
                        return MessagePartialView("Unable to update record");
                    }
                }
                catch (Exception ex)
                {
                    return MessagePartialView(ex.Message);
                }
            }
            else
            {
                return MessagePartialView("Invalid Audit Id");
            }
        }

    }
}