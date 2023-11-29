using EMS.Dto;
using EMS.Service;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using EMS.Data;
using System.Collections.Generic;
using DataTables.AspNet.Core;
using EMS.Web.Modals;
using EMS.Core;
using EMS.Web.Controllers;
using EMS.Web.Code.Attributes;
using System;
using EMS.Web.Code.LIBS;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace EMS.Website.Controllers
{
    [CustomAuthorization]
    public class ProjectClientFeedbackController : BaseController
    {
        #region "Fields"
        private readonly IProjectClientFeedbackService projectClientFeedbackService;
        private readonly IProjectService projectService;
        #endregion
        #region "Constructor"
        public ProjectClientFeedbackController(IProjectClientFeedbackService _projectClientFeedbackService, 
            IProjectService _projectService)
        {
            projectClientFeedbackService = _projectClientFeedbackService;
            projectService = _projectService;
        }
        #endregion

        #region "Index"
        [HttpGet]
        public IActionResult Index()
        {
            //List<Project>
            ProjectClientFeedbackDto model = new ProjectClientFeedbackDto();
            //For HR need to display all project so adding condition with director
            int? pmId=
            CurrentUser.RoleId == (int)Enums.UserRoles.Director || CurrentUser.RoleId == (int)Enums.UserRoles.HRBP ? (int?)null : (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM ||
                CurrentUser.RoleId == (int)Enums.UserRoles.PMO) ? CurrentUser.Uid : CurrentUser.PMUid;

            var projects = projectClientFeedbackService.GetProjectHavingClientFeedback(pmId);

            model.ProjectList = projects != null && projects.Count > 0 ? projects.Select(p => new SelectListItem { Text = p.Name, Value = p.ProjectId.ToString() }
            ).ToList() : new List<SelectListItem>();
            return View(model);
        }
        [HttpPost]
        public IActionResult Index(IDataTablesRequest request, ProjectClientFeedbackFilter searchFilter)
        {
            //For HR need to display all project so adding condition with director
            int? pmId =
            CurrentUser.RoleId == (int)Enums.UserRoles.Director || CurrentUser.RoleId == (int)Enums.UserRoles.HRBP ? (int?)null : (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM ||
                CurrentUser.RoleId == (int)Enums.UserRoles.PMO) ? CurrentUser.Uid : CurrentUser.PMUid;

            var pagingServices = new PagingService<ProjectClientFeedback>(request.Start, request.Length);
            var expr = PredicateBuilder.True<ProjectClientFeedback>();
            expr = expr.And(cf=> !pmId.HasValue || cf.Project.PMUid== pmId);

            if (searchFilter.projectId > 0)
            {
                expr = expr.And(e => e.ProjectId == searchFilter.projectId);
            }
            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "ProjectName":
                            return o.OrderByColumn(item, c => c.Project.Name);
                        case "clientName":
                            return o.OrderByColumn(item, c => c.ClientName);
                        case "CRMProjectId":
                            return o.OrderByColumn(item, c => c.Project.CRMProjectId);

                        default:
                            return o.OrderByColumn(item, c => c.ModifyDate);

                    }
                }
                return o.OrderByDescending(c => c.ModifyDate);
            };
            int totalCount = 0;
            var response = projectClientFeedbackService.GetProjectClientFeedbacksByPaging(out totalCount, pagingServices);
            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                r.Id,
                rowIndex = (index + 1) + (request.Start),
                ProjectName=$"{r.Project.Name} [{r.Project.CRMProjectId}]" ,
                Status= !string.IsNullOrWhiteSpace(r.Status) ? ((Enums.FeedbackStatus)Enum.Parse(typeof(Enums.FeedbackStatus), r.Status)).ToString() : "" ,
                ClientName =r.ClientName+(!string.IsNullOrWhiteSpace(r.CompanyName)? $"<br/>{r.CompanyName}" :""),
                Comment=(r.Comment!=null && string.IsNullOrEmpty(r.Comment.Trim()) != true) ? r.Comment:r.MeetRequirements!=null? r.MeetRequirements :"",
                CommentDate= r.CommentDate!=null? ((DateTime)r.CommentDate).ToString("dd MMM yyyy"):"",
                ValuesAbout=r.ValueAboutDotsquares,
                IsEdit=r.AddedBy==CurrentUser.Uid
            }));

        }
        #endregion
        [HttpGet]
        public ActionResult Detail(int id)
        {
            if (id > 0)
            {
                try
                {
                    ProjectClientFeedbackDetailDto model = new ProjectClientFeedbackDetailDto();
                    var projectClientFeedback = projectClientFeedbackService.ProjectClientFeedbackById(id);

                    if (projectClientFeedback != null && projectClientFeedback.Id > 0)
                    {
                        model.ProjectName = projectClientFeedback.Project.Name;
                        model.Comment = projectClientFeedback.Comment;
                        model.WebsiteName = projectClientFeedback.WebsiteName;
                        model.WebUrl = projectClientFeedback.WebUrl;
                        model.CompanyName = projectClientFeedback.CompanyName;
                        model.ClientName = projectClientFeedback.ClientName;
                        model.BusinessDescription = projectClientFeedback.BusinessDescription;
                        model.ProjectScope = projectClientFeedback.ProjectScope;
                        model.MeetRequirements = projectClientFeedback.MeetRequirements;
                        model.ValueAboutDotsquares = projectClientFeedback.ValueAboutDotsquares;
                        model.Commentdate = projectClientFeedback.CommentDate!=null ?((DateTime)projectClientFeedback.CommentDate).ToString("dd MMM yyyy"):"";
                        model.Status = !string.IsNullOrWhiteSpace(projectClientFeedback.Status) ? ((Enums.FeedbackStatus)Enum.Parse(typeof(Enums.FeedbackStatus), projectClientFeedback.Status)).ToString() : "";
                        model.ClientFeedbackDocuments = projectClientFeedback.ProjectClientFeedbackDocument.
                        Select(s => new ProjectClientFeedbackDocumentDto() { Id = s.Id, DocumentPath = s.DocumentPath }).ToList();

                    }

                    return PartialView("_ProjectClientFeedbackDetail", model);
                }

                catch (Exception ex)
                {
                    return MessagePartialView(ex.Message);
                }
            }
           
            return MessagePartialView("Invalid Project Client Feedback Id.");
        }

        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult AddEditFeedback(int? id)
        {
            ProjectClientFeedbackDetailDto projectClientFeedbackDto = new ProjectClientFeedbackDetailDto();
                
            if (id.HasValue)
            {
                var feedback=projectClientFeedbackService.ProjectClientFeedbackById(id.Value);
                if(feedback!=null)
                {
                    projectClientFeedbackDto.ProjectId = feedback.ProjectId;
                    projectClientFeedbackDto.Comment = feedback.Comment;
                    projectClientFeedbackDto.WebsiteName = feedback.WebsiteName;
                    projectClientFeedbackDto.WebUrl = feedback.WebUrl;
                    projectClientFeedbackDto.CompanyName = feedback.CompanyName;
                    projectClientFeedbackDto.ClientName = feedback.ClientName;
                    projectClientFeedbackDto.BusinessDescription = feedback.BusinessDescription;
                    projectClientFeedbackDto.ProjectScope = feedback.ProjectScope;
                    projectClientFeedbackDto.MeetRequirements = feedback.MeetRequirements;
                    projectClientFeedbackDto.ValueAboutDotsquares = feedback.ValueAboutDotsquares;
                    projectClientFeedbackDto.Status = feedback.Status;
                    projectClientFeedbackDto.Commentdate = feedback.CommentDate!=null? ((DateTime)feedback.CommentDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) :null;
                    projectClientFeedbackDto.ClientFeedbackDocuments= feedback.ProjectClientFeedbackDocument.
                        Select(s => new ProjectClientFeedbackDocumentDto() { Id = s.Id, DocumentPath = s.DocumentPath }).ToList();
                }
                else
                {
                   return MessagePartialView("Unable to find record");
                }
            }
            var projects = projectService.GetProjectListByPmuid(CurrentUser.PMUid);
            if (projects != null)
            {
                projectClientFeedbackDto.ProjectList = projects.ToSelectList(x => $"{x.Name} [{x.CRMProjectId}]", x => x.ProjectId);
            }
            //projectClientFeedbackDto.Statuses = WebExtensions.GetSelectList<Enums.FeedbackStatus>();
            //it's been done manually to ordering items
            projectClientFeedbackDto.Statuses = new List<Enums.FeedbackStatus>() {
            Enums.FeedbackStatus.Excellent,Enums.FeedbackStatus.Good,Enums.FeedbackStatus.ClientNotSatisfied,
            Enums.FeedbackStatus.Worst,Enums.FeedbackStatus.Other}.Select(v => new SelectListItem
            {
                Text = v.GetEnumDisplayName(),
                Value = Convert.ToInt32(v).ToString(),
                Selected= v== Enums.FeedbackStatus.Other
            }).ToList();
            return PartialView("_AddEditFeedback", projectClientFeedbackDto);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult AddEditFeedback( ProjectClientFeedbackDetailDto model, IFormFileCollection docs)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    ProjectClientFeedback projectClientFeedback = new ProjectClientFeedback();
                    if (model.Id > 0)
                    {
                        projectClientFeedback = projectClientFeedbackService.ProjectClientFeedbackById(model.Id);
                        if (projectClientFeedback != null)
                        {
                            projectClientFeedback.Id = model.Id;
                        }
                        else
                        {
                            return NewtonSoftJsonResult(new RequestOutcome<string>
                            {
                                IsSuccess = false,
                                Message = "Invalid Id"
                            });
                        }

                    }
                    else
                    {
                        projectClientFeedback.AddDate = DateTime.Now;
                        projectClientFeedback.AddedBy = CurrentUser.Uid;
                    }
                    projectClientFeedback.ModifyDate = DateTime.Now;
                    projectClientFeedback.ProjectId = model.ProjectId;
                    projectClientFeedback.Comment = model.Comment;
                    projectClientFeedback.WebsiteName = model.WebsiteName;
                    projectClientFeedback.WebUrl = model.WebUrl;
                    projectClientFeedback.CompanyName = model.CompanyName;
                    projectClientFeedback.ClientName = model.ClientName;
                    projectClientFeedback.BusinessDescription = model.BusinessDescription;
                    projectClientFeedback.ProjectScope = model.ProjectScope;
                    projectClientFeedback.MeetRequirements = model.MeetRequirements;
                    projectClientFeedback.ValueAboutDotsquares = model.ValueAboutDotsquares;
                    projectClientFeedback.Status = model.Status;
                    projectClientFeedback.CrmfeedbackId = null;
                    projectClientFeedback.CommentDate = model.Commentdate.ToDateTime();

                    #region "File upload"
                    foreach (var item in docs)
                    {
                        var fileExt = Path.GetExtension(item.FileName.ToLower());
                        if (fileExt.ToLower()==".exe")
                        {
                            return MessagePartialView($"The file(s) which you are trying to upload does not support({fileExt}).");
                        }
                    }

                    foreach (var item in docs)
                    {
                        string FileName = GeneralMethods.SaveFile(item, "Upload/ClientFeedbackDocument/", "");
                        //model.ClientFeedbackDocuments.Add(new ProjectClientFeedbackDocumentDto() { DocumentPath = FileName });
                        projectClientFeedback.ProjectClientFeedbackDocument.
                            Add(new ProjectClientFeedbackDocument { DocumentPath=FileName,AddDate=DateTime.Now });
                    }
                    #endregion

                    var result =projectClientFeedbackService.Save(projectClientFeedback);

                    if (result != null && result.Id > 0)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = "Client feedback saved successfully"
                        });
                    }
                    else
                    {
                        return MessagePartialView("Unable to save record");
                    }
                }
                catch(Exception ex)
                {
                    return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
                }
            }
            else
            {
                return CreateModelStateErrors();
            }
        }

        [HttpPost]
        [CustomActionAuthorization]
        public JsonResult DeleteDocument(int id)
        {
            var status = false;
            var document = projectClientFeedbackService.GetDocumentById(id);
            if (document != null)
            {

                if(document.Feedback!=null)
                {
                    if(document.Feedback.AddedBy!=null && document.Feedback.AddedBy==CurrentUser.Uid)
                    {
                        //do nothing able to delete record
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You are  not authorised to delete this" });
                    }
                }
                else
                {
                    return Json(new { isSuccess=false,message="You are  not authorised to delete this"});
                }
                string filePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/Upload/ClientFeedbackDocument/", document.DocumentPath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);

                }
                status = projectClientFeedbackService.DeleteDocument(id);
            }
            if(status)
            {
                return Json(new { isSuccess = true});
            }
            else
            {
                return Json(new { isSuccess = false, message = "Unable to delete" });
            }
        }

    }
}