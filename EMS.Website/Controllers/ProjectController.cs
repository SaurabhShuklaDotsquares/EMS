using System;
using System.Collections.Generic;
using System.Linq;
using EMS.Data;
using EMS.Service;
using EMS.Web.Code.Attributes;
using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Web.Code.LIBS;
using EMS.Dto;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using NUglify.Helpers;

namespace EMS.Web.Controllers
{
    //[CustomAuthorization()]
    public class ProjectController : BaseController
    {
        #region "Fields & Constructor"

        private readonly IUserLoginService userLoginService;
        private readonly IProjectService projectService;
        private readonly IDepartmentService departmentService;
        private readonly ITechnologyService technologyService;
        private readonly IVirtualDeveloperService virtualDeveloperService;
        private readonly IPreferenceService preferenceService;

        public ProjectController(IUserLoginService userLoginService, IVirtualDeveloperService virtualDeveloperService, IProjectService projectService, IDepartmentService departmentService, ITechnologyService technologyService, IPreferenceService preferenceService)
        {
            this.projectService = projectService;
            this.departmentService = departmentService;
            this.technologyService = technologyService;
            this.virtualDeveloperService = virtualDeveloperService;
            this.userLoginService = userLoginService;
            this.preferenceService = preferenceService;
        }

        #endregion

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Index()
        {
            ProjectIndexDto projectViewModel = new ProjectIndexDto();
            projectViewModel.IsSuperAdmin = CurrentUser.IsSuperAdmin ? true : false;
            projectViewModel.IsHR = CurrentUser.RoleId == (int)Enums.UserRoles.HRBP ? true : false;
            var StatusList = from Enums.ProjectStatus p in Enum.GetValues(typeof(Enums.ProjectStatus)) select new { Name = p.ToString(), Id = (char)p };
            projectViewModel.ProjectStatusList = StatusList.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString(),
            }).ToList();

            if (CurrentUser.RoleId == (int)Enums.UserRoles.HRBP)
            {
                projectViewModel.PMList = userLoginService.GetPMAndPMOUsers(true).Select(x => new SelectListItem
                {
                    Value = x.Uid.ToString(),
                    Text = x.Name,

                }).ToList();
            }
            return View(projectViewModel);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult Index(IDataTablesRequest request, string ddl_status, string txt_search, string ddlPMList)
        {

            var pagingServices = new PagingService<Project>(request.Start, request.Length);
            var expr = PredicateBuilder.True<Project>();


            if (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO)

            {
                CurrentUser.PMUid = CurrentUser.Uid;
            }
            if (CurrentUser.RoleId == (int)Enums.UserRoles.HRBP)
            {
                if (!string.IsNullOrEmpty(ddlPMList))
                {
                    int pmUID = Convert.ToInt32(ddlPMList);
                    expr = expr.And(e => e.PMUid == pmUID || e.ProjectOtherPm.Any(po => po.Pmuid == pmUID));

                }
            }
            else
            {
                expr = expr.And(e => e.PMUid == CurrentUser.PMUid || e.ProjectOtherPm.Any(po => po.Pmuid == CurrentUser.PMUid));
            }
            if (!string.IsNullOrEmpty(ddl_status))
            {
                expr = expr.And(e => e.Status.Equals(ddl_status));
            }
            if (!string.IsNullOrEmpty(txt_search))
            {
                bool isId = false;
                isId = txt_search.All(char.IsDigit);
                if (isId)
                {
                    int crmId = Convert.ToInt32(txt_search);
                    expr = expr.And(e => e.CRMProjectId.Equals(crmId));
                }
                else
                {
                    expr = expr.And(e => e.Name.Contains(txt_search));
                }

            }

            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "Name":
                            return o.OrderByColumn(item, c => c.Name);

                        default:
                            return o.OrderByColumn(item, c => c.ModifyDate);

                    }
                }
                return o.OrderByDescending(c => c.ModifyDate);
            };
            int totalCount = 0;
            var response = projectService.GetProjectsByPaging(out totalCount, pagingServices);
            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                r.ProjectId,
                rowIndex = (index + 1) + (request.Start),
                ClientId = r.ClientId == null ? "Client : N/A" : "Client : " + r.ClientId + "<br/><b>" + r.Client.Name + "</b>",
                Name = string.IsNullOrEmpty(r.Name) ? "N/A" : r.Name + "<br/>EMS Project ID : <b>" + r.ProjectId + "</b>&nbsp;(CRM ID : <b>" + r.CRMProjectId + "</b>)",
                Model = r.BucketModel.ModelName,
                r.ActualDevelopers,
                Status = string.Format("{0} ({1})", Common.GetProjectDisplayStatus(r.Status), r.ProjectDevelopers.Count(p => p.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running))
            }));
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult AddEditProject(int? id)
        {

            ProjectDto projectViewModel = new ProjectDto();

            projectViewModel.DepartmentList = departmentService.GetActiveDepartments().OrderBy(x => x.Name).Select(x => new DropdownListDto
            {
                Text = x.Name,
                Id = x.DeptId
            }).ToList();

            projectViewModel.TechnologyList = technologyService.GetTechnologyList().Where(x => x.IsActive == true).OrderBy(y => y.Title).
                      Select(z => new DropdownListDto
                      {
                          Id = z.TechId,
                          Text = z.Title
                      }).ToList();

            projectViewModel.ModelList = projectService.GetBucketModels().Select(x => new SelectListItem
            {
                Text = x.ModelName,
                Value = x.BucketId.ToString()
            }).ToList();
            var statusList = from Enums.ProjectStatus p in Enum.GetValues(typeof(Enums.ProjectStatus)) select new { Text = p.ToString(), Id = (char)p };
            projectViewModel.ProjectStatusList = statusList.Select(x => new SelectListItem
            {
                Text = x.Text,
                Value = x.Id.ToString()
            }).ToList();
            if (id != null)
            {
                Project project = projectService.GetProjectById(Convert.ToInt32(id));
                if (project != null)
                {
                    projectViewModel.ProjectId = project.ProjectId;
                    projectViewModel.ActualDevelopers = project.ActualDevelopers;
                    projectViewModel.Name = project.Name;
                    projectViewModel.CRMId = project.CRMProjectId;
                    projectViewModel.BillingTeam = project.BillingTeam;
                    projectViewModel.ClientId = project.ClientId;

                    projectViewModel.Model = Convert.ToInt32(project.Model);
                    projectViewModel.EstimatedDays = Convert.ToInt32(project.EstimateTime);
                    projectViewModel.Notes = project.Notes;
                    projectViewModel.ProjectDetailsDoc = project.ProjectDetailsDoc;
                    projectViewModel.StartDate = project.StartDate.HasValue ? project.StartDate.Value.ToString("dd/MM/yyyy") : null;
                    projectViewModel.Status = project.Status;
                    projectViewModel.IsInHouse = project.IsInHouse;

                    if (project.Project_Department.Count > 0)
                    {
                        projectViewModel.DepartmentList.ForEach(x =>
                        {
                            if (project.Project_Department.Any(y => y.Department.DeptId == x.Id))
                            {
                                x.Selected = true;
                            }
                        });
                    }

                    if (project.Project_Tech.Count > 0)
                    {
                        projectViewModel.TechnologyList.ForEach(x =>
                        {
                            if (project.Project_Tech.Any(y => y.Technology.TechId == x.Id))
                            {
                                x.Selected = true;
                            }
                        });
                    }
                }

            }
            return View(projectViewModel);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult AddEditProject(ProjectDto projectViewModel, IFormFile ProjectDetailsDoc)
        {
            if (CurrentUser != null && CurrentUser.Uid != 0 && ModelState.IsValid)
            {
                bool success = false;
                bool fileUploadError = false;
                bool isClientExist = true;
                Project project = new Project();
                if (projectViewModel.ProjectId != 0)
                {
                    project = projectService.GetProjectById(projectViewModel.ProjectId);
                }
                if (projectViewModel.ClientId != null)
                {
                    isClientExist = projectService.CheckClientId(Convert.ToInt32(projectViewModel.ClientId));
                }
                if (isClientExist)
                {
                    //File Uploader extension and size check
                    int size = 0;
                    string[] requiredExtension = null;
                    if (ProjectDetailsDoc != null)
                    {
                        if (SiteKey.Size != null && SiteKey.Extension != null)
                        {
                            size = Convert.ToInt32(SiteKey.Size);
                            requiredExtension = SiteKey.Extension.Split(',');
                        }
                        var fileExt = Path.GetExtension(ProjectDetailsDoc.FileName.ToLower());
                        if (!requiredExtension.Contains(fileExt))
                        {
                            fileUploadError = true;
                            ShowErrorMessage("Error", "Uploaded file format is not valid", false);
                        }
                        else if (ProjectDetailsDoc.Length > size)
                        {
                            fileUploadError = true;
                            ShowErrorMessage("Error", "Uploaded file size should not be grater than 200 MB", false);
                        }
                        else
                        {
                            string path = Path.Combine(ContextProvider.HostEnvironment.WebRootPath, "Content/ProjectDoc/", Path.GetFileName(ProjectDetailsDoc.FileName));//Server.MapPath("~/Content/ProjectDoc/");
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                ProjectDetailsDoc.CopyTo(stream);
                            }
                            //ProjectDetailsDoc.SaveAs(path + Path.GetFileName(ProjectDetailsDoc.FileName));
                            project.ProjectDetailsDoc = ProjectDetailsDoc.FileName;
                        }
                    }
                    if (!fileUploadError)
                    {
                        project.Name = projectViewModel.Name;
                        project.ActualDevelopers = projectViewModel.ActualDevelopers;
                        project.BillingTeam = projectViewModel.BillingTeam;
                        project.Model = projectViewModel.Model;
                        project.ClientId = projectViewModel.ClientId;
                        project.CRMProjectId = projectViewModel.CRMId;
                        if (projectViewModel.ProjectId == 0)
                            project.CRMProjectId = 0;
                        project.EstimateTime = projectViewModel.EstimatedDays;
                        project.IP = GeneralMethods.Getip();
                        project.Notes = projectViewModel.Notes;

                        project.PMUid = CurrentUser.RoleId == (int)Enums.UserRoles.PMO ? CurrentUser.Uid : CurrentUser.RoleId == (int)Enums.UserRoles.PM ? CurrentUser.Uid : CurrentUser.PMUid;
                        project.Status = projectViewModel.Status;
                        project.IsInHouse = projectViewModel.IsInHouse;
                        if (project.ProjectId == 0 && project.CRMProjectId == 0)
                            project.IsInHouse = true;
                        project.Uid = CurrentUser.Uid;
                        project.IsClosed = false;
                        project.StartDate = DateTime.Parse(projectViewModel.StartDate, CultureInfo.GetCultureInfo("en-gb"));
                        project.ModifyDate = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy"), CultureInfo.GetCultureInfo("en-gb"));

                        var isUserDeptDeleted = projectService.ProjectDeptDeleted(project);
                        if (isUserDeptDeleted)
                        {
                            if (project.Project_Department != null)
                            {
                                project.Project_Department.Clear();
                            }

                            if (projectViewModel.Department != null)
                            {
                                foreach (var item in projectViewModel.Department)
                                {

                                    project.Project_Department.Add(
                                        new Project_Department
                                        {
                                            ProjectID = projectViewModel.ProjectId,
                                            DeptID = Convert.ToInt32(item)
                                        });
                                }

                            }
                        }
                        var isUserTechDeleted = projectService.ProjectTechDeleted(project);
                        if (isUserTechDeleted)
                        {
                            if (project.Project_Tech != null)
                            {
                                project.Project_Tech.Clear();
                            }
                            if (projectViewModel.Technology != null)
                            {
                                foreach (var item in projectViewModel.Technology)
                                {
                                    project.Project_Tech.Add(new Project_Tech
                                    {
                                        ProjectID = projectViewModel.ProjectId,
                                        TechId = Convert.ToInt32(item)
                                    });

                                }
                            }
                        }


                        success = projectService.Save(project);
                        if (projectViewModel.ProjectId == 0)
                        {
                            if (success)
                            {
                                ShowSuccessMessage("Success", "Project has been added successfully", false);
                            }
                            else
                            {
                                ShowErrorMessage("Error", "Failed to add project..!! CRMID must be unique", false);
                            }
                        }
                        else
                        {
                            if (success)
                            {
                                ShowSuccessMessage("Success", "Project has been updated successfully", false);

                            }
                            else
                            {
                                ShowErrorMessage("Error", "Failed to update project..!! CRMID must be unique", false);

                            }
                        }
                    }
                }
                else
                {
                    ShowErrorMessage("Error", "Client id does not exist", false);
                }


            }
            if (projectViewModel.ProjectId == 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("AddEditProject", new { id = projectViewModel.ProjectId });

            }
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult AddEditProjectDeveloper(int projectId)
        {
            ViewBag.ProjectId = projectId;
            ViewBag.IsEditable = CurrentUser.IsSuperAdmin || CurrentUser.RoleId == (int)Enums.UserRoles.HRBP;

            return PartialView("_AddEditProjectDeveloper");
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult GetVirtualDevelopers(int projectId, int? PMUid)
        {
            int pmuid = 0;
            if (CurrentUser.RoleId == (int)Enums.UserRoles.HRBP)
            {
                pmuid = projectService.GetProjectPM(projectId) ?? 0;
            }
            else
            {
                pmuid = CurrentUser.RoleId == (int)Enums.UserRoles.PM ? CurrentUser.Uid : CurrentUser.RoleId == (int)Enums.UserRoles.PMO ? CurrentUser.Uid : CurrentUser.PMUid;
            }

            ProjectDeveloperDto projectDeveloper = new ProjectDeveloperDto();

            projectDeveloper.VirtualDeveloperList = virtualDeveloperService.GetVirtualDeveloperByPM(pmuid).Select(x => new SelectListItem
            {
                Text = x.VirtualDeveloper_Name,
                Value = x.VirtualDeveloper_ID.ToString()
            }).ToList();

            projectDeveloper.DeveloperList = userLoginService.GetUsers(true).Where(T => T.PMUid.Equals(pmuid)).OrderBy(P => P.Name).Select(x => new SelectListItem
            {
                Value = x.Uid.ToString(),
                Text = x.Name
            }).ToList();

            var projectDevStatus = from Enums.ProjectDevWorkStatus e in Enum.GetValues(typeof(Enums.ProjectDevWorkStatus)) select new { Id = (int)e, Name = e.ToString() };
            projectDeveloper.Status = projectDevStatus.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            projectDeveloper.projectDeveloperMappingList = projectService.GetProjectById(projectId).ProjectDevelopers.Select(x => new ProjectDeveloperMapping
            {
                Uid = Convert.ToInt32(x.Uid),
                Remark = x.Remark == null ? "" : x.Remark.ToString(),
                VirtualDeveloperID = Convert.ToInt32(x.VD_id),
                Status = Convert.ToInt32(x.WorkStatus),
            }).ToList();


            // Add Other Pm  Developer and virtual developer  
            foreach (var dev in projectDeveloper.projectDeveloperMappingList)
            {
                //Developer

                if (!projectDeveloper.DeveloperList.Any(x => x.Value.Contains(dev.Uid.ToString())))
                {
                    var developer = userLoginService.GetUsers(true).Where(T => T.Uid == dev.Uid).Select(x => new SelectListItem
                    {
                        Value = x.Uid.ToString(),
                        Text = x.Name
                    }).FirstOrDefault();

                    if (developer != null)
                    {

                        projectDeveloper.DeveloperList.Add(developer);
                    }
                }
                // virtual developer....
                if (!projectDeveloper.VirtualDeveloperList.Any(x => x.Value.Contains(dev.VirtualDeveloperID.ToString())))
                {
                    var Getvirtualdeveloper = virtualDeveloperService.GetVirtualDeveloperById(dev.VirtualDeveloperID);

                    if (Getvirtualdeveloper != null)
                    {
                        var virtualdeveloper = new SelectListItem
                        {
                            Text = Getvirtualdeveloper.VirtualDeveloper_Name,
                            Value = Getvirtualdeveloper.VirtualDeveloper_ID.ToString()
                        };

                        projectDeveloper.VirtualDeveloperList.Add(virtualdeveloper);
                    }
                }
            }



            if (projectDeveloper.projectDeveloperMappingList.Count == 0)
            {
                projectDeveloper.projectDeveloperMappingList.Add(new ProjectDeveloperMapping());
            }
            return Json(projectDeveloper);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult AddEditProjectDeveloper(string ProjectId, [FromBody] List<ProjectDeveloperMapping> projectDeveloperMappingList)
        {
            if (ProjectId != null && projectDeveloperMappingList != null)
            {

                int ProjectID = Convert.ToInt32(ProjectId);
                bool isProjectRunning = projectService.GetProjectStatus(ProjectID);
                if (isProjectRunning)
                {
                    List<ProjectDeveloper> ProjectDeveloperList = new List<ProjectDeveloper>();
                    foreach (var item in projectDeveloperMappingList)
                    {
                        ProjectDeveloper ProjectDeveloper = new ProjectDeveloper();

                        ProjectDeveloper.AddDate = DateTime.Now;
                        ProjectDeveloper.ProjectId = ProjectID;
                        ProjectDeveloper.IP = GeneralMethods.Getip();
                        ProjectDeveloper.Remark = item.Remark;
                        ProjectDeveloper.Uid = item.Uid == null ? null : item.Uid;
                        ProjectDeveloper.VD_id = Convert.ToInt32(item.VirtualDeveloperID);
                        ProjectDeveloper.WorkStatus = Convert.ToInt32(item.Status);
                        ProjectDeveloper.ModifyDate = DateTime.Now;
                        ProjectDeveloper.TransId = Guid.NewGuid();
                        ProjectDeveloperList.Add(ProjectDeveloper);

                    }
                    bool success = projectService.UpdateProjectDeveloper(ProjectDeveloperList, ProjectID);
                    if (success)
                    {
                        ShowSuccessMessage("Success", "Project developer(s) has been updated successfully", false);
                    }
                    else
                    {
                        ShowErrorMessage("Error", "Same developer cannot be assign more then once in a project", false);
                    }
                }
                else
                {
                    ShowErrorMessage("Error", "You cannot add/edit project developer(s) when project status is Completed/Hold", false);
                }
            }
            return Json(new { isSuccess = true, redirectUrl = Url.Action("Index", "Project") });

        }

        //[CustomActionAuthorization]
        //[HttpGet]
        //public ActionResult GetCRMProjectList(int? pmId)
        //{
        //    if (pmId == null)
        //    {
        //        pmId = CurrentUser.RoleId == (int)Enums.UserRoles.PM ? CurrentUser.Uid : CurrentUser.RoleId == (int)Enums.UserRoles.PMO ? CurrentUser.Uid : CurrentUser.PMUid;
        //        return RedirectToAction("Index", "ProjectInfo", new { id = EncryptDecrypt.Encrypt(pmId.ToString()) });
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "ProjectInfo", new { id = EncryptDecrypt.Encrypt(pmId.ToString()) });
        //    }
        //}

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult AdditionalSupport()
        {
            var model = new ProjectAdditionalSupportIndexDto();

            int? pmUserId = PMUserId;
            int? requestByUid = CurrentUser.Uid;

            if (CurrentUser.RoleId == (int)Enums.UserRoles.Director)
            {
                pmUserId = null;
                requestByUid = null;
                model.IsDirector = true;

                model.PMUserList = userLoginService.GetUserByRole((int)Enums.UserRoles.PM)
                                                   .Where(x => x.DeptId != (int)Enums.ProjectDepartment.AccountDepartment &&
                                                               x.DeptId != (int)Enums.ProjectDepartment.HRDepartment &&
                                                               x.DeptId != (int)Enums.ProjectDepartment.Other)
                                                   .Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() })
                                                   .ToList();
            }
            else if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
            {
                requestByUid = null;
            }

            model.ProjectList = projectService.GetAdditionalSupportProjectList(pmUserId, requestByUid)
                                              .Select(x => new SelectListItem { Text = $"{x.Name} [{x.CRMProjectId}]", Value = x.ProjectId.ToString() })
                                              .DistinctBy(x => x.Value).OrderBy(x => x.Text).ToList();



            model.UserListByPM = userLoginService.GetUsersByPM(Convert.ToInt32(pmUserId)).Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() }).ToList();
            model.StatusList = WebExtensions.GetSelectList<Enums.AddSupportRequestStatus>();

            return View(model);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult AdditionalSupport(IDataTablesRequest request, int? pmUid, int? projectId, Enums.AddSupportRequestStatus? status)
        {
            var pagingServices = new PagingService<ProjectAdditionalSupport>(request.Start, request.Length);
            var filterExpr = PredicateBuilder.True<ProjectAdditionalSupport>();
            var currentUserId = CurrentUser.Uid;
            var isPMUser = CurrentUser.RoleId == (int)Enums.UserRoles.PM;
            var isDirector = CurrentUser.RoleId == (int)Enums.UserRoles.Director;

            if (!isDirector)
            {
                if (isPMUser)
                {
                    filterExpr = filterExpr.And(x => x.RequestByUid == currentUserId || x.Project.PMUid == currentUserId);
                }
                else
                {
                    filterExpr = filterExpr.And(x => x.RequestByUid == currentUserId || x.TLId == currentUserId);
                }
            }

            else if (pmUid.HasValue && pmUid.Value > 0)
            {
                filterExpr = filterExpr.And(x => x.Project.PMUid == pmUid);
            }

            if (projectId.HasValue && projectId.Value > 0)
            {
                filterExpr = filterExpr.And(x => x.ProjectId == projectId.Value);
            }

            if (status.HasValue && Enum.IsDefined(typeof(Enums.AddSupportRequestStatus), status.Value))
            {
                filterExpr = filterExpr.And(x => x.Status == (byte)status.Value);
            }

            pagingServices.Filter = filterExpr;

            pagingServices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.CreateDate);
            };

            int totalCount = 0;
            var response = projectService.GetAdditionalSupportByPaging(out totalCount, pagingServices);

            return DataTablesJsonResult(totalCount, request, response.Select((x, index) => new
            {
                rowIndex = (index + 1) + (request.Start),
                Id = x.Id.ToUrlBase64String(),
                ProjectName = $"{x.Project.Name} [{x.Project.CRMProjectId}]",
                x.Description,
                AssignedUsers = string.Join(", ", x.ProjectAdditionalSupportUser.Select(u => u.AssignedU.Name)),

                StartEndDate = $"{x.StartDate.ToFormatDateString("dd MMM yyyy")} to {x.EndDate.ToFormatDateString("dd MMM yyyy")}",
                Status = ((Enums.AddSupportRequestStatus)x.Status).GetEnumDisplayName(),
                RequestedBy = x.UserLogin1?.Name,
                PMName = x.UserLogin2?.Name,
                CreateDate = x.CreateDate.ToFormatDateString("dd MMM yyyy, hh:mm tt"),
                UpdateDate = x.ApprovalDate.ToFormatDateString("dd MMM yyyy, hh:mm tt"),
                UpdateComment = x.ApprovalComment,
                UpdateAllowed = isPMUser && (x.Status == (byte)Enums.AddSupportRequestStatus.Pending || x.Status == (byte)Enums.AddSupportRequestStatus.Partial),
                EditAllowedPM = isPMUser && ((x.Status == (byte)Enums.AddSupportRequestStatus.Approved) || (x.Status == (byte)Enums.AddSupportRequestStatus.UnApproved)),
                EditAllowedForOtherUser = (((x.UserLogin2 != null ? x.UserLogin2.Uid == CurrentUser.Uid : false) || (x.UserLogin1 != null ? x.UserLogin1.Uid == CurrentUser.Uid : false)) && (x.Status == (byte)Enums.AddSupportRequestStatus.Pending || x.Status == (byte)Enums.AddSupportRequestStatus.Partial))


            }));
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult RequestAdditionalSupport(string id)
        {
            try
            {
                int? requestId = id.HasValue() ? id.Trim().UrlBaseToInt32() : null;

                ProjectAdditionalSupportDto model = null;

                if (requestId.HasValue && requestId.Value > 0)
                {
                    var supportEntity = projectService.GetAdditionalSupportRequest(requestId.Value);

                    if (supportEntity != null && supportEntity.Project.PMUid == PMUserId)
                    {

                        if ((CurrentUser.RoleId == (int)Enums.UserRoles.PM && (supportEntity.Status == (byte)Enums.AddSupportRequestStatus.Pending
                            || supportEntity.Status == (byte)Enums.AddSupportRequestStatus.Partial)) ||
                            ((supportEntity.RequestByUid == CurrentUser.Uid || supportEntity.TLId == CurrentUser.Uid) && (supportEntity.Status == (byte)Enums.AddSupportRequestStatus.Partial || supportEntity.Status == (byte)Enums.AddSupportRequestStatus.Pending)))
                        {
                            model = InitAdditionalSupportModel(PMUserId, supportEntity);
                        }
                        else if (CurrentUser.RoleId == (int)Enums.UserRoles.PM && ((supportEntity.Status == (byte)Enums.AddSupportRequestStatus.Approved) || (supportEntity.Status == (byte)Enums.AddSupportRequestStatus.UnApproved)))
                        {
                            model = InitAdditionalSupportModel(PMUserId, supportEntity);
                        }
                        else
                        {
                            return MessagePartialView("Request can't be processed");
                        }
                    }
                    else
                    {
                        return MessagePartialView("Request not found or Project is not assigned to current project manager");
                    }
                }
                else
                {
                    model = InitAdditionalSupportModel(PMUserId);
                }

                model.IsPMUser = IsPM;

                return PartialView("_RequestAdditionalSupport", model);

            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.Message);
            }
        }

        [HttpPost]
        [CustomAuthorization]
        public ActionResult RequestAdditionalSupport(ProjectAdditionalSupportDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Project project = projectService.GetProjectByProjectIdAndPmId(model.ProjectId, PMUserId);

                    if (project != null)//&& project.PMUid == PMUserId)
                    {
                        var startDate = model.StartDate.ToDateTime("dd/MM/yyyy").Value;
                        var endDate = model.EndDate.ToDateTime("dd/MM/yyyy").Value;

                        if (startDate > endDate)
                        {
                            return MessagePartialView("Invalid date selection", messageType: Enums.MessageType.Danger);
                        }

                        ProjectAdditionalSupport addSupportEntity;

                        if (model.Id == 0)
                        {
                            addSupportEntity = new ProjectAdditionalSupport
                            {
                                ProjectId = model.ProjectId,
                                Description = model.AddDescription ?? string.Empty,
                                StartDate = startDate,
                                EndDate = endDate,
                                RequestByUid = CurrentUser.Uid,
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Status = (byte)Enums.AddSupportRequestStatus.Pending,
                                TLId = model.TLid.HasValue && model.TLid.Value > 0 ? model.TLid : model.UserIdByPM.HasValue && model.UserIdByPM.Value > 0 ? model.UserIdByPM.Value : (int?)null,

                            };

                            if (model.FromProjectStatus)
                            {
                                //if (model.TLid.HasValue && model.TLid.Value > 0)
                                //{
                                //    addSupportEntity.TLId = model.TLid;
                                //}

                                //addSupportEntity.TLId = CurrentUser.Uid;


                                addSupportEntity.Status = !model.AddDescription.HasValue() ?
                                        (byte)Enums.AddSupportRequestStatus.Partial : addSupportEntity.Status;
                            }

                            var result = projectService.SaveAdditionalSupportRequest(addSupportEntity, model.AssignedUserIds);

                            if (result != null && result.Id > 0)
                            {
                                if (model.FromProjectStatus)
                                {
                                    string message = "";
                                    if (result.Status == (byte)Enums.AddSupportRequestStatus.Pending)
                                    {
                                        SendAdditionalSupportEmail(result, true);
                                        message = $"Additional Support request for Project : {project.Name}, has been sent to Team Manager";
                                    }
                                    else
                                    {
                                        SendAddAdditionalSupportEmailToTL(result);
                                        SendAdditionalSupportEmail(result, true);
                                        message = $"Additional Support request for Project : {project.Name}, has been sent to Project Manager.\n\nPlease ask Project Manager to review and process the request further.";
                                    }

                                    return NewtonSoftJsonResult(new { IsSuccess = true, Message = message, Data = new { ProjectId = project.ProjectId, ProjectName = project.Name } });
                                }
                                else
                                {
                                    SendAdditionalSupportEmail(result, true);
                                    return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = true, Message = "Additional support request sent successfully" });
                                }
                            }
                            else
                            {
                                return MessagePartialView("Unable to save record");
                            }
                        }
                        else
                        {
                            addSupportEntity = projectService.GetAdditionalSupportRequest(model.Id);

                            if (addSupportEntity != null && addSupportEntity.Project.PMUid == PMUserId)
                            {
                                addSupportEntity.TLId = model.UserIdByPM == 0 ? null : model.UserIdByPM;
                                if (CurrentUser.RoleId == (int)Enums.UserRoles.PM && (addSupportEntity.Status == (int)Enums.AddSupportRequestStatus.Pending
                                   || addSupportEntity.Status == (int)Enums.AddSupportRequestStatus.Partial))
                                {
                                    // Case for PM
                                    addSupportEntity.Status = model.Status;
                                    addSupportEntity.ApprovalComment = model.ApprovalComment;
                                    addSupportEntity.ModifyDate = DateTime.Now;
                                    addSupportEntity.ApprovalDate = DateTime.Now;
                                    addSupportEntity.ApproveByUid = CurrentUser.Uid;
                                    addSupportEntity.Description = model.AddDescription;
                                    addSupportEntity.TLId = model.UserIdByPM == 0 ? null : model.UserIdByPM;
                                    projectService.SaveAdditionalSupportRequest(addSupportEntity, model.AssignedUserIds);

                                    if (addSupportEntity.Status != (int)Enums.AddSupportRequestStatus.Pending)
                                    {
                                        SendAdditionalSupportEmail(addSupportEntity, false);
                                    }

                                    return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = true, Message = $"Additional Support for Project : {addSupportEntity.Project.Name}, marked as {((Enums.AddSupportRequestStatus)addSupportEntity.Status).GetEnumDisplayName()} successfully" });
                                }
                                else if ((addSupportEntity.TLId == CurrentUser.Uid || addSupportEntity.RequestByUid == CurrentUser.Uid) && (addSupportEntity.Status == (int)Enums.AddSupportRequestStatus.Partial || addSupportEntity.Status == (int)Enums.AddSupportRequestStatus.Pending))
                                {
                                    // Case for TL or Requested User
                                    addSupportEntity.Status = (int)Enums.AddSupportRequestStatus.Pending;
                                    addSupportEntity.Description = model.AddDescription;
                                    addSupportEntity.StartDate = startDate;
                                    addSupportEntity.EndDate = endDate;
                                    addSupportEntity.ModifyDate = DateTime.Now;
                                    addSupportEntity.TLId = model.UserIdByPM == 0 ? null : model.UserIdByPM;
                                    projectService.SaveAdditionalSupportRequest(addSupportEntity, model.AssignedUserIds);

                                    SendAdditionalSupportEmail(addSupportEntity, true);
                                    return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = true, Message = "Additional support request sent successfully" });
                                }
                                else if ((CurrentUser.RoleId == (int)Enums.UserRoles.PM) && ((addSupportEntity.Status == (int)Enums.AddSupportRequestStatus.Approved) || (addSupportEntity.Status == (int)Enums.AddSupportRequestStatus.UnApproved)))
                                {
                                    addSupportEntity.Status = model.Status == 0 ? addSupportEntity.Status : model.Status;
                                    addSupportEntity.ApprovalComment = model.ApprovalComment;
                                    addSupportEntity.ModifyDate = DateTime.Now;
                                    addSupportEntity.ApprovalDate = DateTime.Now;
                                    addSupportEntity.ApproveByUid = CurrentUser.Uid;
                                    addSupportEntity.Description = model.AddDescription;
                                    addSupportEntity.StartDate = startDate;
                                    addSupportEntity.EndDate = endDate;
                                    addSupportEntity.TLId = model.UserIdByPM == 0 ? null : model.UserIdByPM;
                                    projectService.SaveAdditionalSupportRequest(addSupportEntity, model.AssignedUserIds);
                                    return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = true, Message = $"Project has been updated successfully" });
                                }
                                else
                                {
                                    return MessagePartialView("Request already marked as " + ((Enums.AddSupportRequestStatus)addSupportEntity.Status).GetEnumDisplayName());
                                }
                            }
                        }

                        return MessagePartialView("Access denied");

                    }
                    else
                    {
                        return MessagePartialView("Project not found or Project is not assigned to current project manager");
                    }
                }
                else
                {
                    return MessagePartialView(string.Join("; ", ModelState.Values
                                               .SelectMany(x => x.Errors)
                                               .Select(x => x.ErrorMessage)));
                }
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.Message);
            }
        }

        [HttpGet]
        [CustomAuthorization]
        /// <summary>
        /// Show Add. Support popup when user select Project in Project Status Popup as Additional Support
        /// </summary>
        /// <param name="id">Project Id selected in Project Status Popup</param>
        public ActionResult AddAdditionalSupport(int id)
        {
            try
            {
                if (id > 0)
                {
                    ProjectAdditionalSupportDto model = new ProjectAdditionalSupportDto
                    {
                        FromProjectStatus = true
                    };

                    var project = projectService.GetProjectById(id);
                    if (project != null && project.PMUid == PMUserId && !project.IsInHouse)
                    {
                        model.ProjectId = id;
                        model.ProjectName = project.Name;
                        model.Status = (byte)Enums.AddSupportRequestStatus.Pending;
                        model.AssignedUserIds = new int[] { CurrentUser.Uid };
                        model.EndDate = model.StartDate = DateTime.Today.ToFormatDateString("dd/MM/yyyy");
                        model.TLList = userLoginService.GetUsersByRoles(RoleValidator.Multiple_RoleIds, PMUserId)
                                                  .OrderBy(x => x.Name)
                                                  .ToSelectList(x => x.Name, x => x.Uid);

                        //Here reason behind using ShowErrorMessage is just to show message in light red
                        ShowErrorMessage("", $"<p style='font-size: 16px;'>The Project(<strong>{project.Name}</strong>) is not running stage. You need to take approval from team manager before work on this project. We request you to mention the reason behind of additional support.</p>", true);

                        return PartialView("_AddAdditionalSupport", model);
                    }
                }

                return MessagePartialView("Invalid Project Id");

            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetAdditionalSupportRequest(string id)
        {
            try
            {
                if (id.HasValue())
                {
                    int requestId = 0, approverUid = 0, requestByUid = 0;

                    try
                    {
                        string decryptedValue = EncryptDecrypt.Decrypt(id.FromUrlBase64ToString());
                        if (decryptedValue.HasValue())
                        {
                            string[] parameters = decryptedValue.Split('|');

                            // must have <request id>|<approver user id>|<request by user id>

                            if (parameters != null && parameters.Length == 3)
                            {
                                requestId = Convert.ToInt32(parameters[0]);
                                approverUid = Convert.ToInt32(parameters[1]);   // Approver can be null or default 0
                                requestByUid = Convert.ToInt32(parameters[2]);
                            }
                        }
                    }
                    catch { }

                    if (requestId > 0 && requestByUid > 0)
                    {
                        var supportEntity = projectService.GetAdditionalSupportRequest(requestId);
                        if (supportEntity != null)
                        {
                            var pmUserId = supportEntity.UserLogin1.RoleId == (int)Enums.UserRoles.PM ? supportEntity.RequestByUid : supportEntity.UserLogin1.PMUid ?? 0;

                            var model = InitAdditionalSupportModel(pmUserId, supportEntity);

                            if (!model.AllowUpdate)
                            {
                                ShowInfoMessage("", $"Additional Support Request for Project : {model.ProjectName} has been marked as {(Enums.AddSupportRequestStatus)model.Status}");
                            }

                            model.ApproveByUid = approverUid > 0 ? approverUid : (int?)null;
                            model.RequestToken = id;

                            return View("UpdateAdditionalSupportRequest", model);
                        }
                        else
                        {
                            return MessagePartialView("Record not found");
                        }
                    }
                }

                return MessagePartialView("Invalid request");

            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult UpdateAdditionalSupportRequest(ProjectAdditionalSupportDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var startDate = model.StartDate.ToDateTime("dd/MM/yyyy").Value;
                    var endDate = model.EndDate.ToDateTime("dd/MM/yyyy").Value;

                    if (startDate > endDate)
                    {
                        return MessagePartialView("Invalid date selection", messageType: Enums.MessageType.Danger);
                    }

                    var addSupportEntity = projectService.GetAdditionalSupportRequest(model.Id);

                    if (addSupportEntity != null)
                    {
                        if (addSupportEntity.Status == (int)Enums.AddSupportRequestStatus.Pending ||
                            addSupportEntity.Status == (int)Enums.AddSupportRequestStatus.Partial)
                        {
                            addSupportEntity.Status = model.Status;
                            addSupportEntity.ApprovalComment = model.ApprovalComment;
                            addSupportEntity.ModifyDate = DateTime.Now;
                            addSupportEntity.ApprovalDate = DateTime.Now;
                            addSupportEntity.ApproveByUid = model.ApproveByUid;
                            addSupportEntity.Description = model.AddDescription;

                            projectService.SaveAdditionalSupportRequest(addSupportEntity, model.AssignedUserIds);

                            if (addSupportEntity.Status != (int)Enums.AddSupportRequestStatus.Pending)
                            {
                                SendAdditionalSupportEmail(addSupportEntity, false);
                            }
                            else
                            {
                                //return ShowWarningMessage("Record is still in pending status, Please choose Approved or Unapproved");
                                return MessagePartialView("Record is still in pending status, please choose Approved or Unapproved if you would like to allow or deny additional support.", messageType: Enums.MessageType.Warning);
                            }

                            return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = true, RedirectUrl = Url.Action("GetAdditionalSupportRequest", "Project", new { id = model.RequestToken }) });
                        }
                        else
                        {
                            return MessagePartialView("Request already marked as " + ((Enums.AddSupportRequestStatus)addSupportEntity.Status).GetEnumDisplayName());
                        }
                    }
                    else
                    {
                        return MessagePartialView("Record not found");
                    }
                }
                else
                {
                    return MessagePartialView(string.Join("; ", ModelState.Values
                                               .SelectMany(x => x.Errors)
                                               .Select(x => x.ErrorMessage)));
                }
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.Message);
            }
        }

        private ProjectAdditionalSupportDto InitAdditionalSupportModel(int pmUserId, ProjectAdditionalSupport supportEntity = null)
        {
            ProjectAdditionalSupportDto model = new ProjectAdditionalSupportDto();


            if (supportEntity != null)
            {
                model = new ProjectAdditionalSupportDto
                {
                    Id = supportEntity.Id,
                    ProjectId = supportEntity.ProjectId,
                    AddDescription = supportEntity.Description.NewLineToHtmlBreak(),
                    ApprovalComment = supportEntity.ApprovalComment.NewLineToHtmlBreak(),
                    StartDate = supportEntity.StartDate.ToFormatDateString("dd/MM/yyyy"),
                    EndDate = supportEntity.EndDate.ToFormatDateString("dd/MM/yyyy"),
                    CreateDate = supportEntity.CreateDate.ToFormatDateString("dd/MM/yyyy hh:mm tt"),
                    ApprovalDate = supportEntity.ApprovalDate.ToFormatDateString("dd/MM/yyyy hh:mm tt"),
                    RequestByUser = supportEntity.UserLogin1?.Name,
                    Status = supportEntity.Status,
                    //Status = supportEntity.Status == (byte)Enums.AddSupportRequestStatus.Pending ? (byte)Enums.AddSupportRequestStatus.Approved : supportEntity.Status,
                    UserIdByPM = supportEntity.TLId == null ? 0 : supportEntity.TLId.Value,
                    ProjectName = $"{supportEntity.Project.Name} [{supportEntity.Project.CRMProjectId}]",
                    AllowUpdate = (supportEntity.Status == (byte)Enums.AddSupportRequestStatus.Pending
                    || supportEntity.Status == (byte)Enums.AddSupportRequestStatus.Partial),
                    AssignedUserIds = supportEntity.ProjectAdditionalSupportUser.Select(x => x.AssignedUid).ToArray()
                };
            }
            else
            {
                model.UserIdByPM = CurrentUser.Uid;
                model.ProjectList = projectService.GetProjectListByPmuid(pmUserId).Select(x =>
                    new SelectListItem
                    {
                        Text = $"{x.Name} [{x.CRMProjectId}]",
                        Value = x.ProjectId.ToString()
                    }).ToList();
            }

            model.StatusList = WebExtensions.GetSelectList(Enums.AddSupportRequestStatus.Partial);
            model.UserList = userLoginService.GetUsersByPM(pmUserId).ToSelectList(x => x.Name, x => x.Uid);
            model.UserListByPM = userLoginService.GetUsersByPM(pmUserId).ToSelectList(x => x.Name, x => x.Uid);

            return model;
        }

        private void SendAdditionalSupportEmail(ProjectAdditionalSupport addSupport, bool isNewRequest)
        {
            try
            {
                if (addSupport != null)
                {
                    if (isNewRequest)
                    {
                        SendAdditionalSupportRequestEmail(addSupport);
                    }
                    else
                    {
                        SendAdditionalSupportStatusEmail(addSupport);
                    }
                }
            }
            catch
            {

            }
        }

        private void SendAdditionalSupportRequestEmail(ProjectAdditionalSupport addSupport)
        {
            try
            {
                string toEmail = preferenceService.GetDataByPmid(PMUserId)?.AdditionalSupportEmail;
                List<UserLogin> recipients = new List<UserLogin>();
                string[] emailList = null;
                if (toEmail.HasValue())
                {
                    emailList = toEmail.Split(',', ';').Where(x => x.HasValue()).Select(x => x.Trim().ToLower()).ToArray();

                    foreach (var item in emailList)
                    {
                        var user = userLoginService.GetLoginDeatilByEmail(toEmail);
                        if (user != null)
                        {
                            recipients.Add(user);
                        }
                    }
                }
                else
                {
                    return;
                }

                if (emailList != null && emailList.Length > 0)
                {
                    foreach (var recipientEmail in emailList)
                    {
                        var user = recipients.FirstOrDefault(x => x.EmailOffice.ToLower() == recipientEmail);

                        string fromemail = CurrentUser.EmailOffice;
                        string fromName = CurrentUser.Name;
                        string projectName = $"{addSupport.Project.Name} [{addSupport.Project.CRMProjectId}]";
                        string startDate = addSupport.StartDate.ToFormatDateString("dd MMM yyyy");
                        string endDate = addSupport.EndDate.ToFormatDateString("dd MMM yyyy");
                        string status = ((Enums.AddSupportRequestStatus)addSupport.Status).GetEnumDisplayName();
                        string token = EncryptDecrypt.Encrypt($"{addSupport.Id}|{user?.Uid ?? 0}|{addSupport.RequestByUid}").ToUrlBase64String();

                        FlexiMail flexiMail = new FlexiMail();

                        //   v0 = Project Name, v1 = Start Date, v2 = End Date
                        //   v3 = Reason/Description, v4 = Request from, v5 = Names of Assigned Developers
                        //   v6 = Link to Approve Add. Support Request

                        #region Request Email

                        flexiMail.From = fromemail;
                        flexiMail.To = recipientEmail;

                        flexiMail.Subject = $"Additional support request for Project : {projectName}";

                        flexiMail.ValueArray = new string[] {
                                                    projectName,
                                                    startDate,
                                                    endDate,
                                                    addSupport.Description.NewLineToHtmlBreak(),
                                                    fromName,
                                                  //  string.Join(", ", addSupport.UserLogins.Select(x=> x.Name)),
                                                  string.Join(", ", addSupport.ProjectAdditionalSupportUser.Select(x=> x.AssignedU.Name)),
                                                    $"{SiteKey.DomainName}project/getadditionalsupportrequest/{token}"
                                                   };
                        flexiMail.MailBodyManualSupply = true;

                        flexiMail.MailBody = flexiMail.GetHtml("AdditionalSupportRequestEmail.html");

                        flexiMail.Send();

                        #endregion
                    }
                }
            }
            catch
            {
            }
        }

        private void SendAdditionalSupportStatusEmail(ProjectAdditionalSupport addSupport)
        {
            try
            {
                string fromemail = addSupport.UserLogin?.EmailOffice ?? SiteKey.From;
                string fromName = addSupport.UserLogin?.Name;
                string toEmail = addSupport.UserLogin1.EmailOffice;
                string toName = addSupport.UserLogin1.Name;
                string projectName = $"{addSupport.Project.Name} [{addSupport.Project.CRMProjectId}]";
                string startDate = addSupport.StartDate.ToFormatDateString("dd MMM yyyy");
                string endDate = addSupport.EndDate.ToFormatDateString("dd MMM yyyy");
                string status = ((Enums.AddSupportRequestStatus)addSupport.Status).GetEnumDisplayName();

                FlexiMail flexiMail = new FlexiMail();

                //   v0 = Project Name, v1 = Start Date, v2 = End Date
                //   v3 = Comments/Description, v4 = from, v5 = status, v6 = to

                #region Request Email

                flexiMail.From = fromemail;
                flexiMail.To = toEmail;

                flexiMail.Subject = $"{status} : Additional support request for Project : {projectName}";

                flexiMail.ValueArray = new string[] {
                                                    projectName,
                                                    startDate,
                                                    endDate,
                                                    addSupport.ApprovalComment.NewLineToHtmlBreak(),
                                                    fromName,
                                                    status,
                                                    toName,
                                                   // string.Join(", ", addSupport.UserLogins.Select(x=> x.Name)),
                                                   string.Join(", ", addSupport.ProjectAdditionalSupportUser.Select(x=> x.AssignedU.Name)),
                                                   };

                flexiMail.MailBody = flexiMail.GetHtml("AdditionalSupportStatusEmail.html");
                flexiMail.MailBodyManualSupply = true;

                flexiMail.Send();

                #endregion
            }
            catch
            {

            }
        }

        private void SendAddAdditionalSupportEmailToTL(ProjectAdditionalSupport addSupport)
        {
            try
            {
                if (addSupport != null)
                {
                    string fromemail = addSupport.UserLogin?.EmailOffice ?? SiteKey.From;
                    string fromName = addSupport.UserLogin?.Name;
                    string toEmail = addSupport.UserLogin2.EmailOffice;
                    string toName = addSupport.UserLogin2.Name;
                    string projectName = $"{addSupport.Project.Name} [{addSupport.Project.CRMProjectId}]";
                    string startDate = addSupport.StartDate.ToFormatDateString("dd MMM yyyy");
                    string endDate = addSupport.EndDate.ToFormatDateString("dd MMM yyyy");
                    string status = ((Enums.AddSupportRequestStatus)addSupport.Status).GetEnumDisplayName();

                    FlexiMail flexiMail = new FlexiMail();

                    //   v0 = Project Name, v1 = Start Date, v2 = End Date
                    //   v3 = Comments/Description, v4 = from, v5 = status, v6 = to

                    #region Request Email

                    flexiMail.From = fromemail;
                    flexiMail.To = toEmail;

                    flexiMail.Subject = $"{status} : Additional support request for Project : {projectName}";

                    flexiMail.ValueArray = new string[] {
                                                    projectName,
                                                    startDate,
                                                    endDate,
                                                   // string.Join(", ", addSupport.UserLogins.Select(x=> x.Name)),
                                                   string.Join(", ", addSupport.ProjectAdditionalSupportUser.Select(x=> x.AssignedU.Name)),
                                                    addSupport.Description.NewLineToHtmlBreak(),
                                                    fromName
                                                   };

                    flexiMail.MailBody = flexiMail.GetHtml("AdditionalSupportPartialRequestEmail.html");
                    flexiMail.MailBodyManualSupply = true;

                    flexiMail.Send();

                    #endregion
                }
            }
            catch
            {

            }

        }
    }
}