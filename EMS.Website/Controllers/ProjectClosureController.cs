using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.Modals;
using EMS.Web.Models.Others;
using EMS.Website.Code.LIBS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NPOI.HSSF.Record.Formula.Functions;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Transactions;
using System.Web;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class ProjectClosureController : BaseController
    {
        #region Variable and Constructor
        private readonly IUserLoginService userLoginService;
        private readonly IProjectClosureService projectClosureService;
        private readonly IProjectClosureReviewService projectClosureReviewService;
        private readonly IProjectService projectService;
        private readonly ILeadServices leadService;
        private readonly ITechnologyParentService technologyParentService;
        private bool IfAshishTeamPMUId { get { return (CurrentUser.PMUid == SiteKey.AshishTeamPMUId || CurrentUser.Uid == SiteKey.AshishTeamPMUId) ? true : false; } }
        public ProjectClosureController(
            IUserLoginService _userLoginService,
            IProjectClosureService _projectClosureService,
            IProjectClosureReviewService _projectClosureReviewService,
            IProjectService _projectService, ILeadServices _leadService,
            ITechnologyParentService _technologyParentService)
        {
            userLoginService = _userLoginService;
            projectClosureService = _projectClosureService;
            projectClosureReviewService = _projectClosureReviewService;
            projectService = _projectService;
            leadService = _leadService;
            technologyParentService = _technologyParentService;
        }


        public bool IsUKAUUserIDToShowAshishTeamActivity
        {
            get { return (SiteKey.UKAUUserIDToShowAshishTeamActivity != null && SiteKey.UKAUUserIDToShowAshishTeamActivity.Split(',').ToList().Contains(CurrentUser.Uid.ToString())); }
        }

        #endregion

        #region Index Page

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Index()
        {            
            var model = new ProjectClosureIndexDto();

            if (CurrentUser.RoleId == (int)Enums.UserRoles.Director || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM)
            {
                model.IsDirector = true;
                var userList = userLoginService.GetUserByRole((int)Enums.UserRoles.PM, true);
                model.PMList = userList.Where(x => x.DeptId != (int)Enums.ProjectDepartment.AccountDepartment &&
                                                   x.DeptId != (int)Enums.ProjectDepartment.HRDepartment &&
                                                   x.DeptId != (int)Enums.ProjectDepartment.Other)
                                       .ToSelectList(x => x.Name, x => x.Uid);
            }

            else if (CurrentUser.RoleId == (int)Enums.UserRoles.UKPM)
            {

                var userList = userLoginService.GetAllDotsquaresDevelopers();
                model.BAList = userList.Where(x => RoleValidator.BA_RoleIds.Contains(x.RoleId.Value) || x.RoleId == (int)Enums.UserRoles.PM || x.RoleId == (int)Enums.UserRoles.UKPM)
                                      .ToSelectList(x => x.Name, x => x.Uid);

                model.TLList = userList.Where(x => RoleValidator.TL_Technical_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(x.DesignationId.Value)            
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(x.DesignationId.Value)
            // || RoleValidator.DV_RoleIds.Contains(x.RoleId.Value)
             || x.RoleId == (int)Enums.UserRoles.UKPM)
                                      .ToSelectList(x => x.Name, x => x.Uid);


            }

            else
            {
                var userList = userLoginService.GetUsersByPM(PMUserId);

                model.BAList = userList.Where(x => RoleValidator.BA_RoleIds.Contains(x.RoleId.Value) || x.RoleId == (int)Enums.UserRoles.PM || x.RoleId == (int)Enums.UserRoles.UKPM)
                                       .ToSelectList(x => x.Name, x => x.Uid);
                model.TLList = userList.Where(x =>
                RoleValidator.TL_Technical_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(x.DesignationId.Value)            
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(x.DesignationId.Value)
            //|| RoleValidator.DV_RoleIds.Contains(x.RoleId.Value)
                || x.RoleId == (int)Enums.UserRoles.UKPM)
                                       .ToSelectList(x => x.Name, x => x.Uid);
            }

            model.CRMStatus = WebExtensions.GetSelectList<Enums.CRMStatus>();

            model.ProjectStatus = Enums.CloserType.Pending;

            return View(model);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult Index(IDataTablesRequest request, ProjectClosureSearchFilter searchFilter)
        {
            var pagingServices = new PagingService<ProjectClosure>(request.Start, request.Length);

            var expr = GetClosureFilterExpersion(searchFilter);

            pagingServices.Filter = expr;

            pagingServices.Sort = (o) =>
            {
                return o.OrderBy(c => c.NextStartDate);
            };

            TempData.Put("ProjectClosureFilters", searchFilter);

            int totalCount = 0;
            var response = projectClosureService.GetProjectClosurePaging(out totalCount, pagingServices);

            bool isPMUser = CurrentUser.RoleId == (int)Enums.UserRoles.PM;
            int currentUserId = CurrentUser.Uid;

            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                r.Id,
                rowIndex = (index + 1) + (request.Start),
                ClientName = r.Project.Client?.Name ?? string.Empty,
                ProjectName = r.Project.Name,
                r.Project.CRMProjectId,
                PCountry = r.Country,
                EngagementDate = r.NextStartDate.ToFormatDateString("MMM d, yyyy"),
                BA = r.UserLogin?.Name,
                TL = r.UserLogin3?.Name,
                PM = r.UserLogin1?.Name,
                BaId = r.Uid_BA,
                TlId = r.Uid_TL,
                r.AddedBy,
                ClientQuality = r.ClientQuality ?? 0,
                r.Status,
                CloserType = ((Enums.CloserType)searchFilter.ProjectStatus).GetDescription().ToString(),
                CRMStatus = r.CRMStatus.HasValue ? ((Enums.CRMStatus?)r.CRMStatus.Value).ToString() : "",
                StartEndDate = StartEndDates(r.StartDate, r.EndDate),
                Invoice = (r.InvoiceDays ?? 0) > 0 ? DaysToWeeks(r.InvoiceDays.Value) : "-",
                BucketHours = r.BucketHours.HasValue && r.BucketHours > 0.0f ? $"{r.BucketHours} hours" : null,
                Estimate = (r.EstimateDays ?? 0) > 0 ? DaysToWeeks(r.EstimateDays.Value) : "-",
                CreatedDate = r.Created.ToFormatDateString("MMM d, yyyy hh:mm tt"),
                ModityDate = r.Modified.ToFormatDateString("MMM d, yyyy hh:mm tt"),
                AllowUpdateStatus = isPMUser || (r.AddedBy == currentUserId || r.Uid_BA == currentUserId || r.Uid_TL == currentUserId)
            }));
        }

        [HttpPost]
        public PartialViewResult BAConversion(ProjectClosureSearchFilter searchFilter)
        {
            var expr = GetClosureFilterExpersion(searchFilter, true);
            var projectClosures = projectClosureService.GetProjectClosure(expr);
            List<BAConversionSummaryDto> objClouserSummary = projectClosures.OrderBy(x => x.Modified)
                .GroupBy(x => x.Uid_BA ?? 0)
                .Select(g =>
                {
                    //var summary = new BAConversionSummaryDto
                    //{
                    //    BA_ID = g.Key,
                    //    BAName = g.FirstOrDefault().UserLogin?.Name,
                    //    Total = g.Select(p => p.ProjectID).Distinct().Count(),
                    //    Pending = g.GroupBy(p => p.ProjectID).Count(s => s.FirstOrDefault().Status == (int)Enums.CloserType.Pending),
                    //    Converted = g.GroupBy(p => p.ProjectID).Count(s => s.FirstOrDefault().Status == (int)Enums.CloserType.Converted),
                    //    Escalated = g.GroupBy(p => p.ProjectID).Count(s => s.FirstOrDefault().Status == (int)Enums.CloserType.DeadResponse),
                    //};

                    var summary = new BAConversionSummaryDto
                    {
                        BA_ID = g.Key,
                        BAName = g.FirstOrDefault().UserLogin?.Name,
                        Total = g.Select(p => p.ProjectID).Count(),
                        Pending = g.Select(p => p.Status == (int)Enums.CloserType.Pending).Where(s => s == true).Count(),
                        Converted = g.Select(p => p.Status == (int)Enums.CloserType.Converted).Where(s => s == true).Count(),
                        Escalated = g.Select(p => p.Status == (int)Enums.CloserType.DeadResponse).Where(s => s == true).Count()
                    };

                    var percentage = summary.Converted / (double)summary.Total * 100;
                    summary.Per = percentage > 0 ? string.Format("{0:#.##}%", percentage) : "-";
                    return summary;
                })
                .OrderBy(x => x.BAName)
                .ToList();

            return PartialView("_BAConversion", objClouserSummary);
        }

        #endregion

        #region Report Page

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Report()
        {            
            var model = new ProjectClosureIndexDto();

            if (CurrentUser.RoleId == (int)Enums.UserRoles.Director || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM)
            {
                model.IsDirector = true;
                var userList = userLoginService.GetUserByRole((int)Enums.UserRoles.PM, true);
                model.PMList = userList.Where(x => x.DeptId != (int)Enums.ProjectDepartment.AccountDepartment &&
                                                   x.DeptId != (int)Enums.ProjectDepartment.HRDepartment &&
                                                   x.DeptId != (int)Enums.ProjectDepartment.Other)
                                       .ToSelectList(x => x.Name, x => x.Uid);
            }

            else if (CurrentUser.RoleId == (int)Enums.UserRoles.UKPM)
            {

                var userList = userLoginService.GetAllDotsquaresDevelopers();
                model.BAList = userList.Where(x => RoleValidator.BA_RoleIds.Contains(x.RoleId.Value) || x.RoleId == (int)Enums.UserRoles.PM || x.RoleId == (int)Enums.UserRoles.UKPM)
                                      .ToSelectList(x => x.Name, x => x.Uid);

                model.TLList = userList.Where(x =>
                RoleValidator.TL_Technical_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(x.DesignationId.Value)            
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(x.DesignationId.Value)
                //|| RoleValidator.DV_RoleIds.Contains(x.RoleId.Value) 
                || x.RoleId == (int)Enums.UserRoles.UKPM)
                                      .ToSelectList(x => x.Name, x => x.Uid);


            }


            else
            {
                var userList = userLoginService.GetUsersByPM(PMUserId);

                model.BAList = userList.Where(x => RoleValidator.BA_RoleIds.Contains(x.RoleId.Value) || x.RoleId == (int)Enums.UserRoles.PM  || x.RoleId == (int)Enums.UserRoles.UKPM)
                                       .ToSelectList(x => x.Name, x => x.Uid);
                model.TLList = userList.Where(x =>
                RoleValidator.TL_Technical_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(x.DesignationId.Value)            
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(x.DesignationId.Value)
                //|| RoleValidator.DV_RoleIds.Contains(x.RoleId.Value) 
                || x.RoleId == (int)Enums.UserRoles.UKPM)
                                       .ToSelectList(x => x.Name, x => x.Uid);
            }

            ViewBag.Technologies = technologyParentService.GetTechnologyParentDropdown();

            model.CRMStatus = WebExtensions.GetSelectList<Enums.CRMStatus>();

            model.DateFrom = DateTime.Today.AddDays(-30).ToFormatDateString("dd/MM/yyyy");
            model.DateTo = DateTime.Today.ToFormatDateString("dd/MM/yyyy");

            return View(model);
        }
         
        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult Report(IDataTablesRequest request, ProjectClosureSearchFilter searchFilter, Enums.ProjectClosureFilterType? filterType)
        {
            try
            {
                var pagingServices = new PagingService<ProjectClosure>(request.Start, request.Length);
                searchFilter.chaseStatus = 3;
                var expr = GetClosureFilterExpersion(searchFilter, reportFilter: true);

                if (CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM)
                { 
                    // for UK BDM approval will be hide

                    expr = expr.And(x => x.CRMUpdated == true);

                }

                pagingServices.Filter = expr.And(x => x.DateofClosing.HasValue);

                //Sorting is giving in service call
                //pagingServices.Sort = (o) =>
                //{
                //    return o.OrderBy(c => c.CRMUpdated).ThenByDescending(c => c.DateofClosing.Value);
                //};

                TempData.Put("ProjectClosureReportFilters", searchFilter);

                int totalCount = 0;
                var response = projectClosureService.GetProjectClosureReportPaging(out totalCount, pagingServices, filterType);


                bool isPMUser = IsPM;
                bool isDirector = IsDirector;
                int currentUserId = CurrentUser.Uid;
                bool allowReview = CurrentUser.Uid == SiteKey.AshishTeamPMUId || IsDirector;

                return DataTablesJsonResult(totalCount, request, response.Select((r, index) =>  new
                {
                    r.Id,
                    rowIndex = (index + 1) + (request.Start),
                    ClientName = r.Project.Client?.Name ?? string.Empty,
                    ProjectName = r.Project.Name,
                    r.Project.CRMProjectId,
                    ProjectId = r.ProjectID,
                    PCountry = r.Country,
                    ClosingDate = r.DateofClosing.ToFormatDateString("MMM d, yyyy"),
                    StartEndDate = StartEndDates(r.StartDate, r.EndDate),
                    Invoice = (r.InvoiceDays ?? 0) >  0 ? DaysToWeeks(r.InvoiceDays.Value) : "-",
                    BucketHours = r.BucketHours.HasValue && r.BucketHours > 0.0f ? $"{r.BucketHours} hours" : null,
                    Estimate = (r.EstimateDays ?? 0) > 0 ? DaysToWeeks(r.EstimateDays.Value) : "-",
                    BA = r.UserLogin?.Name,
                    TL = r.UserLogin3?.Name,
                    PM = r.UserLogin1?.Name,
                    BaId = r.Uid_BA,
                    TlId = r.Uid_TL,
                    r.AddedBy,
                    ClientQuality = r.ClientQuality ?? 0,

                    CRMStatus = r.CRMStatus.HasValue ? ((Enums.CRMStatus?)r.CRMStatus.Value).ToString() : "",
                    CreatedDate = r.Created.ToFormatDateString("MMM d, yyyy hh:mm tt"),
                    ModityDate = r.Modified.ToFormatDateString("MMM d, yyyy hh:mm tt"),

                    PromisingPercentage = r.ProjectClosureReview != null ? ((Enums.ProjectClosureReviewPercentage)r.ProjectClosureReview.PromisingPercentageId).GetDescription() : "",
                    Developers = r.ProjectClosureReview?.DeveloperCount,
                    r.ProjectClosureReview?.Comments,

                    NextStartDate = r.ProjectClosureReview?.NextStartDate.ToFormatDateString("MMM d, yyyy"),
                    r.IsNewLeadGenerated,
                    //NewLeadId = (r.IsNewLeadGenerated == true ? leadService.GetProjectLeadByProjectClosureId(r.Id).LeadId : 0),
                    NewLeadId = (r.IsNewLeadGenerated == true ? leadService.GetProjectClosureLeadId(r.Id) : 0),

                    AllowUpdateStatus = r.AddedBy == currentUserId || r.Uid_BA == currentUserId || r.Uid_TL == currentUserId || IsUKAUUserIDToShowAshishTeamActivity,
                    IsNotPermanentDead = (r.Status == (int)Enums.CloserType.DeadResponse && r.DeadResponseDate != null),

                    HasConverted = r.Status == (int)Enums.CloserType.Converted,
                    HasDeadResponse = r.Status == (int)Enums.CloserType.DeadResponse,
                    DeadResponseDate = r.DeadResponseDate?.ToFormatDateString("MMM d, yyyy"),
                    IsPending = !r.CRMUpdated,
                    HasReview = r.ProjectClosureReview != null,
                    AllowPendingEdit = !IsDirector,
                    AllowEdit = isPMUser || (r.AddedBy == currentUserId || r.Uid_BA == currentUserId || r.Uid_TL == currentUserId),
                    AllowReview = allowReview && (r.ProjectClosureReview == null || r.ProjectClosureReview.CreateByUid == currentUserId),
                }));
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult getprojectSummary(IDataTablesRequest request, ProjectClosureSearchFilter searchFilter, Enums.ProjectClosureFilterType? filterType)
        {
            var pagingServices = new PagingService<ProjectClosure>(0, int.MaxValue);
            searchFilter.chaseStatus = 3;
            var expr = GetClosureFilterExpersion(searchFilter, reportFilter: true);

            if (CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM)
            { // for UK BDM approval will be hide
                expr = expr.And(x => x.CRMUpdated == true);
            }

            expr = expr.And(b => (b.Uid_BA ?? 0) > 0);

            pagingServices.Filter = expr.And(x => x.DateofClosing.HasValue);

            TempData.Put("ProjectClosureReportFilters", searchFilter);

            int totalCount = 0;
            var projectClosures = projectClosureService.GetProjectClosurePaging(out totalCount, pagingServices);
            //var projectClosures = projectClosureService.GetProjectClosureReportPaging(out totalCount, pagingServices, filterType);

            //Show the Closed, Completed, Hold and Converted
            //Closed (Completed + Hold), Completed and Hold
            var objClouserSummary = projectClosures.Where(b => b.Uid_BA != null && b.Uid_BA != SiteKey.AshishTeamPMUId)
                .GroupBy(x => (x.Uid_BA == SiteKey.AshishTeamPMUId ? x.Uid_TL : x.Uid_BA))
                //.GroupBy(x => x.Uid_BA ?? 0)
                .Select((g, index) => new
                {
                    index = (index + 1),
                    BA_ID = g.Key,
                    BAName = (g.FirstOrDefault().Uid_BA == SiteKey.AshishTeamPMUId ? g.FirstOrDefault().UserLogin3.Name : g.FirstOrDefault().UserLogin?.Name),
                    Total = g.Select(p => p.ProjectID).Count(),
                    Closed = g.Select(p => p.CRMStatus == (int)Enums.CRMStatus.Completed || p.CRMStatus == (int)Enums.CRMStatus.OnHold).Where(s => s == true).Count(), //Closed
                    Completed = g.Select(p => p.CRMStatus == (int)Enums.CRMStatus.Completed).Where(s => s == true).Count(), //Completed
                    Hold = g.Select(p => p.CRMStatus == (int)Enums.CRMStatus.OnHold).Where(s => s == true).Count(), //Hold
                    Converted = g.Select(p => p.Status == (int)Enums.CloserType.Converted).Where(s => s == true).Count(), //Converted
                })
                .OrderBy(x => x.BAName)
                .ToList();

            //Show the Closed, Completed, Hold and Converted
            //Closed (Completed + Hold), Completed and Hold
            var objClouserSummaryTL = projectClosures.Where(b => b.Uid_TL != null)
                .GroupBy(x => (x.Uid_TL))
                //.GroupBy(x => x.Uid_BA ?? 0)
                .Select((g, index) => new
                {
                    index = (index + 1),
                    BA_ID = g.Key,
                    BAName = g.FirstOrDefault().UserLogin3.Name,
                    Total = g.Select(p => p.ProjectID).Count(),
                    Closed = g.Select(p => p.CRMStatus == (int)Enums.CRMStatus.Completed || p.CRMStatus == (int)Enums.CRMStatus.OnHold).Where(s => s == true).Count(), //Closed
                    Completed = g.Select(p => p.CRMStatus == (int)Enums.CRMStatus.Completed).Where(s => s == true).Count(), //Completed
                    Hold = g.Select(p => p.CRMStatus == (int)Enums.CRMStatus.OnHold).Where(s => s == true).Count(), //Hold
                    Converted = g.Select(p => p.Status == (int)Enums.CloserType.Converted).Where(s => s == true).Count(), //Converted
                })
                .OrderBy(x => x.BAName)
                .ToList();

            //Show the Closed, Completed, Hold and Converted
            //Closed (Completed + Hold), Completed and Hold
             objClouserSummary.AddRange(objClouserSummaryTL);


            IDictionary<string, object> additionalparam = new Dictionary<string, object>();
            additionalparam.Add(new KeyValuePair<string, object>("userProjectSummary", objClouserSummary));

            return Json(additionalparam);
        }


        #region Project Closure Report For Country
        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult CountryReport()
        {
            var model = new ProjectClosureIndexDto();            
            //if (CurrentUser.RoleId == (int)Enums.UserRoles.Director || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM)
            //{
            //    model.IsDirector = true;
            //    var userList = userLoginService.GetUserByRole((int)Enums.UserRoles.PM, true);
            //    model.PMList = userList.Where(x => x.DeptId != (int)Enums.ProjectDepartment.AccountDepartment &&
            //                                       x.DeptId != (int)Enums.ProjectDepartment.HRDepartment &&
            //                                       x.DeptId != (int)Enums.ProjectDepartment.Other)
            //                           .ToSelectList(x => x.Name, x => x.Uid);
            //}
            //else
            //{
            //var userList = userLoginService.GetUsersByPM(PMUserId);
            var userList = userLoginService.GetUsersByPM(SiteKey.AshishTeamPMUId);

            model.BAList = userList.Where(x => RoleValidator.BA_RoleIds.Contains(x.RoleId.Value) || x.RoleId == (int)Enums.UserRoles.PM)
                                   .ToSelectList(x => x.Name, x => x.Uid);
            model.TLList = userList.Where(x =>
            RoleValidator.TL_Technical_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(x.DesignationId.Value)            
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(x.DesignationId.Value)
            //|| RoleValidator.DV_RoleIds.Contains(x.RoleId.Value)
            )
                                   .ToSelectList(x => x.Name, x => x.Uid);
            //}

            model.CRMStatus = WebExtensions.GetSelectList<Enums.CRMStatus>();

            model.DateFrom = DateTime.Today.AddDays(-30).ToFormatDateString("dd/MM/yyyy");
            model.DateTo = DateTime.Today.ToFormatDateString("dd/MM/yyyy");

            return View(model);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult CountryReport(IDataTablesRequest request, ProjectClosureSearchFilter searchFilter, Enums.ProjectClosureFilterType? filterType)
        {
            var pagingServices = new PagingService<ProjectClosure>(request.Start, request.Length);
            searchFilter.chaseStatus = 3;
            var expr = GetClosureFilterExpersion(searchFilter, isCountryClosureReport: true);
            if (searchFilter.PMUid.HasValue)
            {
                expr = expr.And(x => x.PMID == searchFilter.PMUid.Value);
            }
            else
            {
                expr = expr.And(x => x.PMID == SiteKey.AshishTeamPMUId);
            }

            if (CurrentUser.RoleId == (int)Enums.UserRoles.PMOAU || CurrentUser.RoleId == (int)Enums.UserRoles.AUPM)
            {
                expr = expr.And(x => x.Country == "AUS");
            }
            else if (CurrentUser.RoleId == (int)Enums.UserRoles.PMO || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM)
            {
                expr = expr.And(x => x.Country == "UK" || x.Country == "US");
            }

            expr = expr.And(x => x.CRMUpdated == true);
            pagingServices.Filter = expr.And(x => x.DateofClosing.HasValue);

            //Sorting is giving in service call
            //pagingServices.Sort = (o) =>
            //{
            //    return o.OrderBy(c => c.CRMUpdated).ThenByDescending(c => c.DateofClosing.Value);
            //};

            TempData.Put("ProjectClosureReportFilters", searchFilter);

            int totalCount = 0;
            var response = projectClosureService.GetProjectClosureReportPaging(out totalCount, pagingServices, filterType);


            bool isPMUser = IsPM;
            bool isDirector = IsDirector;
            int currentUserId = CurrentUser.Uid;
            bool allowReview = CurrentUser.Uid == SiteKey.AshishTeamPMUId || IsDirector;

            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                r.Id,
                rowIndex = (index + 1) + (request.Start),
                ClientName = r.Project.Client?.Name ?? string.Empty,
                ProjectName = r.Project.Name,
                r.Project.CRMProjectId,
                ProjectId = r.ProjectID,
                PCountry = r.Country,
                ClosingDate = r.DateofClosing.ToFormatDateString("MMM d, yyyy"),
                StartEndDate = StartEndDates(r.StartDate, r.EndDate),
                Invoice = (r.InvoiceDays ?? 0) > 0 ? DaysToWeeks(r.InvoiceDays.Value) : "-",
                BucketHours = r.BucketHours.HasValue && r.BucketHours > 0.0f ? $"{r.BucketHours} hours" : null,
                Estimate = (r.EstimateDays ?? 0) > 0 ? DaysToWeeks(r.EstimateDays.Value) : "-",
                BA = r.UserLogin?.Name,
                TL = r.UserLogin3?.Name,
                PM = r.UserLogin1?.Name,
                BaId = r.Uid_BA,
                TlId = r.Uid_TL,
                r.AddedBy,
                ClientQuality = r.ClientQuality ?? 0,

                CRMStatus = r.CRMStatus.HasValue ? ((Enums.CRMStatus?)r.CRMStatus.Value).ToString() : "",
                CreatedDate = r.Created.ToFormatDateString("MMM d, yyyy hh:mm tt"),
                ModityDate = r.Modified.ToFormatDateString("MMM d, yyyy hh:mm tt"),

                PromisingPercentage = r.ProjectClosureReview != null ? ((Enums.ProjectClosureReviewPercentage)r.ProjectClosureReview.PromisingPercentageId).GetDescription() : "",
                Developers = r.ProjectClosureReview?.DeveloperCount,
                r.ProjectClosureReview?.Comments,
                NextStartDate = r.ProjectClosureReview?.NextStartDate.ToFormatDateString("MMM d, yyyy"),

                HasConverted = r.Status == (int)Enums.CloserType.Converted,
                HasDeadResponse = r.Status == (int)Enums.CloserType.DeadResponse,
                IsPending = !r.CRMUpdated,
                HasReview = r.ProjectClosureReview != null,
                AllowPendingEdit = !IsDirector,
                AllowEdit = isPMUser || (r.AddedBy == currentUserId || r.Uid_BA == currentUserId || r.Uid_TL == currentUserId),
                AllowReview = allowReview && (r.ProjectClosureReview == null || r.ProjectClosureReview.CreateByUid == currentUserId)
            }));
        }



        #endregion



        [HttpPost]
        public PartialViewResult ProjectClosureSummary(ProjectClosureSearchFilter searchFilter)
        {
            var pagingServices = new PagingService<ProjectClosure>(0, int.MaxValue);
            searchFilter.chaseStatus = 3;
            var expr = GetClosureFilterExpersion(searchFilter, reportFilter: true);

            pagingServices.Filter = expr.And(x => x.DateofClosing.HasValue);

            pagingServices.Sort = (o) =>
            {
                return o.OrderBy(c => c.DateofClosing.Value);
            };

            int totalCount = 0;
            var projectClosures = projectClosureService.GetProjectClosurePaging(out totalCount, pagingServices);

            List<ProjectClosureSummaryDto> objClouserSummary = projectClosures.GroupBy(x => x.PMID ?? 0)
                .Select(g =>
                {
                    var summary = new ProjectClosureSummaryDto
                    {
                        PMId = g.Key,
                        PMName = g.FirstOrDefault().UserLogin1?.Name,
                        RecurringProjectClosed = g.Count(),
                        RecurringProjectRestarted = g.Count(c => c.Status == (int)Enums.CloserType.Converted)
                    };

                    var uniqueClosures = g.GroupBy(x => x.ProjectID.Value).Select(x => x.OrderByDescending(c => c.Id).FirstOrDefault());
                    summary.TotalProjectClosed = uniqueClosures.Count();
                    summary.ProjectRestarted = uniqueClosures.Count(x => x.Status == (int)Enums.CloserType.Converted);
                    summary.ProjectNotStarted = summary.TotalProjectClosed - summary.ProjectRestarted;

                    var reviewedClosures = g.Where(x => x.Status == (int)Enums.CloserType.Pending && x.ProjectClosureReview != null);
                    summary.ProjectPromising = reviewedClosures.Count(c => c.ProjectClosureReview != null && c.ProjectClosureReview.PromisingPercentageId == (byte)Enums.ProjectClosureReviewPercentage.HundredPercent);
                    summary.ProjectLessPromising = reviewedClosures.Count(c => c.ProjectClosureReview != null && c.ProjectClosureReview.PromisingPercentageId == (byte)Enums.ProjectClosureReviewPercentage.FiftyPercent);
                    summary.ProjectNotSure = summary.ProjectNotStarted - (summary.ProjectLessPromising + summary.ProjectPromising);

                    return summary;
                })
                .OrderBy(x => x.PMName)
                .ToList();

            return PartialView("_ProjectClosureSummary", objClouserSummary);
        }

        public string StartEndDates(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return $"{startDate.ToFormatDateString("MMM d, yyyy")} / {endDate.ToFormatDateString("MMM d, yyyy")}";
            }
            else if (startDate.HasValue)
            {
                return startDate.ToFormatDateString("MMM d, yyyy");
            }

            return "";
        }

        public string DaysToWeeks(int days)
        {
            int weeks = days / 5;
            int remDays = days % 5;
            string weekText = weeks > 1 ? "Weeks" : "Week";
            string dayText = remDays > 1 ? "Days" : "Day";

            if (weeks > 0)
            {
                if (remDays > 0)
                {
                    return $"{weeks} {weekText} and {remDays} {dayText}";
                }
                return $"{weeks} {weekText}";
            }
            else if (remDays > 0)
            {
                return $"{remDays} {dayText}";
            }

            return "";
        }

        #endregion

        #region Project Closure Review Page

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult ProjectionReport()
        {
            var model = new ProjectClosureIndexDto();            
            if (CurrentUser.RoleId == (int)Enums.UserRoles.Director)
            {
                model.IsDirector = true;
                var userList = userLoginService.GetUserByRole((int)Enums.UserRoles.PM, true);
                model.PMList = userList.Where(x => x.DeptId != (int)Enums.ProjectDepartment.AccountDepartment &&
                                                   x.DeptId != (int)Enums.ProjectDepartment.HRDepartment &&
                                                   x.DeptId != (int)Enums.ProjectDepartment.Other)
                                       .ToSelectList(x => x.Name, x => x.Uid);
            }
            else
            {
                var userList = userLoginService.GetUsersByPM(PMUserId);

                model.BAList = userList.Where(x => RoleValidator.BA_RoleIds.Contains(x.RoleId.Value) || x.RoleId == (int)Enums.UserRoles.PM)
                                       .ToSelectList(x => x.Name, x => x.Uid);
                model.TLList = userList.Where(x =>
                RoleValidator.TL_Technical_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(x.DesignationId.Value)            
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(x.DesignationId.Value)
                //|| RoleValidator.DV_RoleIds.Contains(x.RoleId.Value)
                )
                                       .ToSelectList(x => x.Name, x => x.Uid);
            }
            model.ReviewPercentageList = WebExtensions.GetSelectList<Enums.ProjectClosureReviewPercentage>();
            //model.projectionReportWeeksDto = ProjectionReportWeekWise();
            return View(model);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult ProjectionReport(IDataTablesRequest request, ProjectClosureSearchFilter searchFilter)
        {
            var pagingServices = new PagingService<ProjectClosureReview>(request.Start, request.Length);
            pagingServices.Filter = GetReviewFilterExpersion(searchFilter);

            pagingServices.Sort = (o) =>
            {
                return o.OrderBy(c => c.NextStartDate);
            };

            TempData.Put("ProjectionReportFilters", searchFilter);

            int totalCount = 0;
            var response = projectClosureReviewService.GetReviewPaging(out totalCount, pagingServices);

            bool isPMUser = IsPM;
            int currentUserId = CurrentUser.Uid;
            bool allowUpdateStatus = CurrentUser.Uid == SiteKey.AshishTeamPMUId || IsDirector;
            var currentDate = DateTime.Today;
            var nextWeekDate = DateTime.Today.AddDays(6);
            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                rowIndex = (index + 1) + request.Start,
                Id = r.ProjectClosureId,
                ClientName = r.ProjectClosure.Project.Client?.Name ?? string.Empty,
                ProjectName = r.ProjectClosure.Project.Name,
                r.ProjectClosure.Project.CRMProjectId,
                ProjectId = r.ProjectClosure.ProjectID,
                CRMStatus = r.ProjectClosure.CRMStatus.HasValue ? ((Enums.CRMStatus?)r.ProjectClosure.CRMStatus.Value).ToString() : "",
                PCountry = r.ProjectClosure.Country,
                PromisingPercentage = r.PromisingPercentageId.HasValue ? ((Enums.ProjectClosureReviewPercentage)r.PromisingPercentageId).GetDescription() : "",
                Developers = r.DeveloperCount,
                r.Comments,
                NextStartDate = r.NextStartDate.ToFormatDateString("ddd, dd MMM yyyy"),
                AddedBy = r.UserLogin?.Name,
                PM = r.ProjectClosure.UserLogin1?.Name,
                BA = r.ProjectClosure.UserLogin?.Name,
                TL = r.ProjectClosure.UserLogin3?.Name,
                BGStyle = r.NextStartDate.HasValue ? (r.NextStartDate < currentDate ? "dark" : (r.NextStartDate <= nextWeekDate ? "light" : "")) : "",
                CreatedDate = r.CreateDate.ToFormatDateString("MMM d, yyyy hh:mm tt"),
                AllowUpdateStatus = allowUpdateStatus
            }));
        }

        [HttpPost]
        public PartialViewResult ProjectionSummary(ProjectClosureSearchFilter searchFilter)
        {
            var pagingService = new PagingService<ProjectClosureReview>(1, int.MaxValue);
            int? pmUid = CurrentUser.RoleId == (int)Enums.UserRoles.Director ? (int?)null : PMUserId;
            pagingService.Filter = GetReviewFilterExpersion(searchFilter).And(x => !pmUid.HasValue || x.ProjectClosure.PMID == pmUid);

            pagingService.Sort = (o) =>
            {
                return o.OrderBy(c => c.NextStartDate);
            };

            var model = projectClosureReviewService.GetReviewSummary(pagingService);

            return PartialView("_ProjectClosureReviewSummary", model);
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult AddEditReview(int id)
        {
            if (id > 0)
            {
                var entity = projectClosureService.projectClosureFindById(id);
                if (entity != null && (entity.PMID == PMUserId || IsDirector))
                {
                    var model = new ProjectClosureReviewDto
                    {
                        ProjectClosureId = entity.Id,
                        ProjectName = $"{entity.Project.Name} [{entity.Project.CRMProjectId}]",
                        PromisingPercentageId = (byte)Enums.ProjectClosureReviewPercentage.NotApplicable,
                        PromisingPercentageList = WebExtensions.GetSelectList<Enums.ProjectClosureReviewPercentage>()
                    };

                    if (entity.ProjectClosureReview != null)
                    {
                        var review = entity.ProjectClosureReview;
                        model.Comments = review.Comments;
                        model.NextStartDate = review.NextStartDate.ToFormatDateString("dd/MM/yyyy");
                        model.PromisingPercentageId = review.PromisingPercentageId;
                        model.DeveloperCount = review.DeveloperCount;
                    }

                    return PartialView("_AddEditReview", model);
                }
            }
            return MessagePartialView("Invalid Closure Id");
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult AddEditReview(ProjectClosureReviewDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.CurrentUserId = CurrentUser.Uid;
                    model.PMUid = PMUserId;
                    model.PromisingPercentageId = model.PromisingPercentageId ?? (byte)Enums.ProjectClosureReviewPercentage.NotApplicable;
                    model.DeveloperCount = model.DeveloperCount ?? 0;

                    var result = projectClosureReviewService.Save(model);

                    if (result != null)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Review saved successfully", IsSuccess = true });
                    }
                    else
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Unable to save record" });
                    }
                }
                catch (Exception ex)
                {
                    return MessagePartialView((ex.InnerException ?? ex).Message);
                }
            }
            else
            {
                return CreateModelStateErrors();
            }
        }

        #endregion

        #region Filter Expressions

        private Expression<Func<ProjectClosure, bool>> GetClosureFilterExpersion(ProjectClosureSearchFilter searchFilter, bool isSummaryFilter = false, bool reportFilter = false, bool isCountryClosureReport = false)
        {
            var expr = PredicateBuilder.True<ProjectClosure>();

            if (!isCountryClosureReport)
            {
                if (reportFilter)
                {
                    if (CurrentUser.RoleId == (int)Enums.UserRoles.Director || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM)
                    {
                        if (searchFilter.PMUid.HasValue)
                        {
                            expr = expr.And(x => x.PMID == searchFilter.PMUid.Value);
                        }
                    }
                    else if (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO || CurrentUser.RoleId == (int)Enums.UserRoles.PMOAU)
                    {
                        expr = expr.And(x => x.PMID == PMUserId);
                    }
                    else if (CurrentUser.RoleId == (int)Enums.UserRoles.UKPM)
                    {
                        expr = expr.And(x => ((x.Uid_Dev == CurrentUser.Uid || x.Uid_BA == CurrentUser.Uid || x.Uid_TL == CurrentUser.Uid) ||
                                                 !x.Uid_BA.HasValue || !x.Uid_TL.HasValue) && (x.PMID == CurrentUser.Uid || x.PMID == PMUserId));
                    }
                    else
                    {
                        expr = expr.And(x => ((x.Uid_Dev == CurrentUser.Uid || x.Uid_BA == CurrentUser.Uid || x.Uid_TL == CurrentUser.Uid) ||
                                                 !x.Uid_BA.HasValue || !x.Uid_TL.HasValue) && x.PMID == PMUserId);
                    }
                }
                else
                {
                    expr = expr.And(x => x.CRMUpdated);

                    if (CurrentUser.RoleId == (int)Enums.UserRoles.Director)
                    {
                        if (searchFilter.PMUid.HasValue)
                        {
                            expr = expr.And(x => x.PMID == searchFilter.PMUid.Value);
                        }
                    }
                    else if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
                    {
                        expr = expr.And(x => x.PMID == PMUserId);
                    }
                    else
                    {
                        expr = expr.And(x => (x.Uid_Dev == CurrentUser.Uid || x.Uid_BA == CurrentUser.Uid || x.Uid_TL == CurrentUser.Uid || !x.Uid_TL.HasValue || !x.Uid_BA.HasValue) && x.PMID == PMUserId);
                    }
                }
            }

            if (searchFilter.Country > 0)
            {
                expr = expr.And(x => x.Country.Equals(searchFilter.Country.ToString()));
            }

            if (searchFilter.TechnologyId > 0)
            {
                expr = expr.And(x => x.Project.Project_Tech.Any(FinancialYear => FinancialYear.Technology.TechnologyParentMapping.Any(z => z.TechnologyParentId == searchFilter.TechnologyId)));
            }

            if (searchFilter.textSearch.HasValue())
            {
                searchFilter.textSearch = searchFilter.textSearch.Trim().ToLower();

                expr = expr.And(L =>
                (Convert.ToString(L.Project.ProjectId) == searchFilter.textSearch)
                || L.Project.CRMProjectId.ToString() == searchFilter.textSearch
                || L.Project.Name.ToLower().Contains(searchFilter.textSearch)
                || (L.Project.ClientId.HasValue ? L.Project.Client.Name.ToLower().Contains(searchFilter.textSearch) : false)
                );
            }

            DateTime? startDate = searchFilter.DateFrom.ToDateTime("dd/MM/yyyy");
            DateTime? endDate = searchFilter.DateTo.ToDateTime("dd/MM/yyyy");

            if (startDate.HasValue && endDate.HasValue)
            {
                if (searchFilter.chaseStatus.Value == 1)
                {
                    expr = expr.And(L => L.NextStartDate >= startDate && L.NextStartDate <= endDate.Value);
                }
                else if (searchFilter.chaseStatus.Value == 2)
                {
                    expr = expr.And(L => L.ProjectClosureDetails.Any(x => x.Created >= startDate.Value && x.Created <= endDate.Value));
                }
                else
                {
                    expr = expr.And(L => L.DateofClosing >= startDate.Value && L.DateofClosing <= endDate.Value);
                }
            }
            else if (startDate.HasValue)
            {
                if (searchFilter.chaseStatus.Value == 1)
                {
                    expr = expr.And(L => L.NextStartDate >= startDate);
                }
                else if (searchFilter.chaseStatus.Value == 2)
                {
                    expr = expr.And(L => L.ProjectClosureDetails.Any(x => x.Created >= startDate.Value));
                }
                else
                {
                    expr = expr.And(L => L.DateofClosing >= startDate.Value);
                }
            }
            else if (endDate.HasValue)
            {
                if (searchFilter.chaseStatus.Value == 1)
                {
                    expr = expr.And(L => L.NextStartDate <= endDate.Value);
                }
                else if (searchFilter.chaseStatus.Value == 2)
                {
                    expr = expr.And(L => L.ProjectClosureDetails.Any(x => x.Created <= endDate.Value));
                }
                else
                {
                    expr = expr.And(L => L.DateofClosing <= endDate.Value);
                }
            }

            if ((searchFilter.BA ?? 0) > 0)
            {
                expr = expr.And(l => l.Uid_BA == searchFilter.BA.Value);
            }
            if ((searchFilter.TL ?? 0) > 0)
            {
                expr = expr.And(l => l.Uid_TL == searchFilter.TL.Value);
            }
            if (!isSummaryFilter && searchFilter.CRMStatusId != null && searchFilter.CRMStatusId.Value > 0)
            {
                expr = expr.And(l => l.CRMStatus == searchFilter.CRMStatusId.Value);
            }

            if (!isSummaryFilter && searchFilter.ProjectStatus > 0)
            {
                int statusId = searchFilter.ProjectStatus;
                expr = expr.And(x => x.Status == statusId || x.Status == 0);
            }

            return expr;
        }

        private Expression<Func<ProjectClosureReview, bool>> GetReviewFilterExpersion(ProjectClosureSearchFilter searchFilter)
        {
            var expr = PredicateBuilder.True<ProjectClosureReview>().And(x => x.NextStartDate.HasValue);
            if (searchFilter.ProjectionData == Enums.ProjectionData.Converted)
            {
                expr = expr.And(x => x.ProjectClosure.Status == (int)Enums.CloserType.Converted);
            }
            else if (searchFilter.ProjectionData == Enums.ProjectionData.PendingAndConverted)
            {
                expr = expr.And(x => x.ProjectClosure.Status == (int)Enums.CloserType.Pending ||
                x.ProjectClosure.Status == (int)Enums.CloserType.Converted);
            }
            else
            {
                expr = expr.And(x => x.ProjectClosure.Status == (int)Enums.CloserType.Pending);
            }

            if (CurrentUser.RoleId == (int)Enums.UserRoles.Director || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM)
            {
                if (searchFilter.PMUid.HasValue)
                {
                    expr = expr.And(x => x.ProjectClosure.PMID == searchFilter.PMUid.Value);
                }
            }
            else if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
            {
                expr = expr.And(x => x.ProjectClosure.PMID == PMUserId);
            }
            else
            {
                int currentUserId = CurrentUser.Uid;
                expr = expr.And(x => (x.CreateByUid == currentUserId || x.ProjectClosure.Uid_Dev == currentUserId ||
                                      x.ProjectClosure.Uid_BA == currentUserId || x.ProjectClosure.Uid_TL == currentUserId) &&
                                        x.ProjectClosure.PMID == PMUserId);
            }

            if ((searchFilter.ReviewPercentageId ?? 0) > 0)
            {
                expr = expr.And(x => x.PromisingPercentageId == searchFilter.ReviewPercentageId.Value);
            }

            if (searchFilter.textSearch.HasValue())
            {
                searchFilter.textSearch = searchFilter.textSearch.Trim().ToLower();

                expr = expr.And(x => x.ProjectClosure.Project.ProjectId.ToString() == searchFilter.textSearch ||
                                     x.ProjectClosure.Project.CRMProjectId.ToString() == searchFilter.textSearch ||
                                     x.ProjectClosure.Project.Name.ToLower().Contains(searchFilter.textSearch) ||
                                     (x.ProjectClosure.Project.ClientId.HasValue ? x.ProjectClosure.Project.Client.Name.ToLower().Contains(searchFilter.textSearch) : false));
            }

            DateTime? startDate = searchFilter.DateFrom.ToDateTime("dd/MM/yyyy");
            DateTime? endDate = searchFilter.DateTo.ToDateTime("dd/MM/yyyy");

            if (startDate.HasValue && endDate.HasValue)
            {
                expr = expr.And(x => x.NextStartDate >= startDate && x.NextStartDate <= endDate.Value);
            }
            else if (startDate.HasValue)
            {
                expr = expr.And(x => x.NextStartDate >= startDate);
            }
            else if (endDate.HasValue)
            {
                expr = expr.And(x => x.NextStartDate <= endDate.Value);
            }

            if ((searchFilter.BA ?? 0) > 0)
            {
                expr = expr.And(x => x.ProjectClosure.Uid_BA == searchFilter.BA.Value);
            }
            if ((searchFilter.TL ?? 0) > 0)
            {
                expr = expr.And(x => x.ProjectClosure.Uid_TL == searchFilter.TL.Value);
            }

            return expr;
        }

        #endregion

        #region Add Project Closure

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Add(int? id, string referrer)
        {
            ProjectClosureDto model = new ProjectClosureDto();
            
            model.Uid_BA = RoleValidator.BA_RoleIds.Contains(CurrentUser.RoleId) ? CurrentUser.Uid : 0;
            model.Uid_TL = RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)            
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
                ? CurrentUser.Uid : 0;
            model.DateOfClosing = DateTime.Now.ToFormatDateString("dd/MM/yyyy");
            model.Referrer = referrer == "engagement" ? Url.Action("index") : Url.Action("report");
            model.Status = (int)Enums.CloserType.Pending;
            model.IsCovid19 = false;

            if (id > 0)
            {
                var entity = projectClosureService.projectClosureFindById(id.Value);
                if (entity != null && entity.PMID == PMUserId)
                {
                    model.ProjectID = entity.ProjectID.Value;
                    model.ClientQuality = (Enums.ClientQualtiy?)entity.ClientQuality ?? Enums.ClientQualtiy.Average;
                    model.Uid_BA = entity.Uid_BA ?? 0;
                    model.Uid_Dev = entity.Uid_Dev ?? 0;
                    model.Uid_TL = entity.Uid_TL ?? 0;
                    model.DateOfClosing = entity.DateofClosing.ToFormatDateString("dd/MM/yyyy");
                    model.NextStartDate = entity.NextStartDate.ToFormatDateString("dd/MM/yyyy");
                    model.Id = entity.Id;
                    model.PMUid = entity.PMID ?? 0;
                    model.Suggestion = entity.Suggestion;
                    model.Reason = entity.Reason;
                    model.CRMStatusId = entity.CRMStatus ?? 0;
                    model.OtherActualDeveloper = entity.OtherActualDeveloper;
                    model.Status = entity.Status;
                    model.ChangeStatusId = entity.Status;

                    if (entity.DeadResponseDate != null && model.ChangeStatusId == (int)Enums.CloserType.DeadResponse)
                    {
                        model.DeadResponseDate = entity.DeadResponseDate.ToFormatDateString("dd/MM/yyyy");
                    }
                    else if (model.ChangeStatusId == (int)Enums.CloserType.DeadResponse)
                    {
                        model.IsPermanentDead = true;
                    }

                    Enums.ClientCountry country;
                    if (Enum.TryParse(entity.Country, true, out country))
                    {
                        model.Country = country;
                    }
                    model.ProjectLiveUrl = entity.ProjectLiveUrl;
                    model.ProjectUrlAbsenseReason = entity.ProjectUrlAbsenseReason;
                    model.CRMUpdated = entity.CRMUpdated;
                    model.ClientBadge = entity.ClientBadge != null ? (Enums.ClientBadge)entity.ClientBadge : 0;
                    model.IsCovid19 = entity.IsCovid19 ?? false;
                }
            }

            Enums.CloserType[] enumClosure = new Enums.CloserType[0];
            model.ChangeStatus = WebExtensions.GetSelectList(enumClosure);
            List<Project> project = new List<Project>();
            List<UserLogin> userList = new List<UserLogin>();
            if (CurrentUser.RoleId == (int)Enums.UserRoles.UKPM)
            {
                project = projectClosureService.GetAllProjectsNamewise(CurrentUser.Uid);
                userList = userLoginService.GetAllDotsquaresDevelopers();
                model.ActualLeadDevelopers = userList.Where(x => !RoleValidator.BA_RoleIds.Contains(x.RoleId.Value) || x.RoleId != (int)Enums.UserRoles.PM || x.RoleId != (int)Enums.UserRoles.UKPM).Select(s => new SelectListItem { Text = s.Name + (s.Pmu != null ? " "+'(' + s.Pmu.Name + ')' : ""), Value = s.Uid.ToString() }).OrderBy(s=>s.Text).ToList();
            }
            else
            {
                project = projectClosureService.GetAllProjectsNamewise(PMUserId);
                userList = userLoginService.GetUsersByPM(PMUserId);
                model.ActualLeadDevelopers = userList.Where(x => !RoleValidator.BA_RoleIds.Contains(x.RoleId.Value) || x.RoleId != (int)Enums.UserRoles.PM || x.RoleId != (int)Enums.UserRoles.UKPM).Select(s => new SelectListItem { Text = s.Name, Value = s.Uid.ToString() }).OrderBy(s => s.Text).ToList();
            }
            model.ProjectList = project.Select(s => new SelectListItem { Text = s.Name + "[" + s.CRMProjectId + "]", Value = s.ProjectId.ToString() }).ToList();
            model.CRMStatus = Enum.GetValues(typeof(Enums.CRMStatus)).Cast<Enums.CRMStatus>().Select(v => new SelectListItem
            {
                Text = (v).GetDescription(),
                Value = ((int)v).ToString()
            }).ToList();

           
            //model.ActualLeadDevelopers = userList.Select(s => new SelectListItem { Text = s.Name, Value = s.Uid.ToString() }).ToList();
            model.BAList = userList.Where(x => RoleValidator.BA_RoleIds.Contains(x.RoleId.Value) || x.RoleId == (int)Enums.UserRoles.PM || x.RoleId == (int)Enums.UserRoles.UKPM)
                                   .Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() }).OrderBy(s => s.Text).ToList();
            model.TLList = userList.Where(x =>
            RoleValidator.TL_Technical_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(x.DesignationId.Value)            
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(x.DesignationId.Value)
            //|| RoleValidator.DV_RoleIds.Contains(x.RoleId.Value)
            || x.RoleId == (int)Enums.UserRoles.UKPM)
                                   .Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() }).OrderBy(s => s.Text).ToList();

            if (IfAshishTeamPMUId)
            {
                model.isAshishTeamMember = true;
            }
            else
            {
                model.isAshishTeamMember = false;
            }
            return View(model);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult Add(ProjectClosureDto model)
        {
            try
            {

                if (model.ChangeStatusId != (int)Enums.CloserType.DeadResponse || model.IsPermanentDead)
                {
                    ModelState.Remove("DeadResponseDate");
                    model.DeadResponseDate = "";
                }

                if (ModelState.IsValid)
                {
                    if (model.Id == 0)
                    {
                        ProjectClosure objCheckProject = projectClosureService.GetDataByProjectID(model.ProjectID);
                        if (objCheckProject != null)
                        {
                            return MessagePartialView($"Project : {objCheckProject.Project.Name} already added in this Process.");
                        }
                    }

                    model.AddedBy = CurrentUser.Uid;
                    model.PMUid = PMUserId;
                    var projectClosureEntity = new ProjectClosure();
                    if (CurrentUser.RoleId!= (int)Enums.UserRoles.UKPM)
                    {
                        projectClosureEntity = projectClosureService.Save(model);
                    }
                    else
                    {
                        projectClosureEntity = projectClosureService.SaveByUK(model, CurrentUser.Uid);
                    }
                       
                    if (projectClosureEntity != null && projectClosureEntity.Id>0)
                    {

                        List<ProjectClosureAbroadPm> projectClosureAbroadPms = new List<ProjectClosureAbroadPm>();
                        foreach (var abroadPM in model.AbroadPMList)
                        {
                            if (abroadPM.Selected)
                            {
                                projectClosureAbroadPms.Add(new ProjectClosureAbroadPm { ProjectClosureId = projectClosureEntity.Id, AbroadPmid = abroadPM.AutoId });
                            }
                        }
                        projectClosureService.SaveProjectClosureAbroadPMB(projectClosureAbroadPms);


                        if (model.Id > 0)
                        {
                            if (!projectClosureEntity.CRMUpdated)
                            {
                                var crmResponse = UpdateCRMStatus(projectClosureEntity.Project.CRMProjectId, Enums.ClosureApiRequestStatusType.Approved);

                                if (crmResponse != null && crmResponse.Status)
                                {
                                    //projectClosureService.UpdateCRMStatus(projectClosureEntity.Id);

                                    projectClosureService.UpdateCRMStatus(projectClosureEntity);

                                    if (SiteKey.IsPMSClosureReportLive)
                                    {
                                        PMSApiCallForAddClosureReport(projectClosureEntity);
                                    }

                                    SendClosureEmail(projectClosureEntity, model);
                                    // Send Project Closure Email to Ashish Team users only
                                    //if (SiteKey.AshishTeamPMUId == PMUserId)
                                    //{
                                    //
                                    //}
                                    ShowSuccessMessage("Success!", $"Record saved successfully.<br>{crmResponse.Message}", false);
                                }
                                else
                                {
                                    var errorMessage = crmResponse?.Errors != null ? string.Join(", ", crmResponse.Errors) : crmResponse?.Message ?? "Some error while calling CRM API";
                                    ShowInfoMessage("Success!", $"Record saved successfully but unable to update status on CRM. Error : {errorMessage}", false);
                                }

                                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = model.Referrer, IsSuccess = true });
                            }
                            else
                            {
                                ShowSuccessMessage("Success!", "Record saved successfully.", false);
                            }

                            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = model.Referrer, IsSuccess = true });
                        }
                        else
                        {
                            SendClosureEmail(projectClosureEntity, model);

                            ShowSuccessMessage("Success!", "Record saved successfully.", false);
                            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = model.Referrer, IsSuccess = true });
                        }
                    }
                    else
                    {
                        return MessagePartialView("Unable to save record");
                    }
                }
                else
                {
                    return CreateModelStateErrors();
                }
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }

        #endregion


        #region [ Abored PM ]
        public IActionResult GetAbortedPM(string countryName, int projectClosureId)
        {
            ProjectClosureDto model = new ProjectClosureDto();

            var projectClosurePM = projectClosureService.GetProjectClosureAbroadPMByProjectId(projectClosureId);
            var abortedPMs = projectClosureService.GetAllAbroadPM(countryName);
            var selectedAbroadPMs = projectClosurePM.Select(i => i.AbroadPmid).ToArray();

            model.AbroadPMList = abortedPMs.Select(x => new AbroadPMModel
            {
                AutoId = x.AutoID,
                GroupName = x.Country,
                Name = x.Name,
                Email = x.Email,
                Selected = (projectClosurePM.Count() > 0 ? selectedAbroadPMs.Contains(x.AutoID) : x.isDefaultForEmail)
            }).OrderBy(x => x.GroupName).ToList();

            return PartialView("_AbortedPMView", model);
        }
        #endregion 

        #region Update Project Status

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult UpdateProjectStatus(int id, bool fromReview = false)
        {
            if (id > 0)
            {
                var projectClosure = projectClosureService.projectClosureFindById(id);
                if (projectClosure != null && (IsDirector || projectClosure.PMID == PMUserId || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM))
                {
                    var model = new ProjectClosureStatusDto
                    {
                        FromReviewPage = fromReview,
                        ProjectClosureId = projectClosure.Id,
                        CurrentStatus = ((Enums.CloserType)projectClosure.Status).GetDescription(),
                        ProjectName = projectClosure.Project.Name
                    };

                    model.ChangeStatus = WebExtensions.GetSelectList(Enums.CloserType.Pending);
                    return PartialView("_ProjectClosureStatus", model);
                }
            }
            return MessagePartialView("Invalid Project Closure Id");
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult UpdateProjectStatus(ProjectClosureStatusDto model)
        {
            if (model.ChangeStatusId != (int)Enums.CloserType.DeadResponse || model.IsPermanentDead)
            {
                ModelState.Remove("DeadResponseDate");
                model.DeadResponseDate = "";
            }
            if (ModelState.IsValid)
            {
                try
                {
                    model.PMUid = PMUserId;
                    var projectClosure = projectClosureService.UpdateProjectStatus(model);

                    if (projectClosure != null)
                    {
                        if (!String.IsNullOrEmpty(model.DeadResponseDate))
                        {
                            projectClosure.DeadResponseDate = model.DeadResponseDate.ToDateTime("dd/MM/yyyy");
                        }
                        else
                        {
                            projectClosure.DeadResponseDate = null;
                        }

                        projectClosureService.Save(projectClosure);


                        FlexiMail objSendMail = new FlexiMail();
                        objSendMail.ValueArray = new string[]
                        {
                            ((Enums.CloserType)projectClosure.Status).GetDescription(),
                            projectClosure.Project.CRMProjectId.ToString(),
                            projectClosure.Project.Name,
                            projectClosure.Project.Client?.Name,
                            projectClosure.UserLogin?.Name,
                            projectClosure.UserLogin3?.Name,
                            model.Reason,
                            //projectClosure.Reason,
                            CurrentUser.Name.ToTitleCase(),
                            (string.IsNullOrEmpty(model.DeadResponseDate) ? "" : model.DeadResponseDate),
                            (string.IsNullOrEmpty(model.DeadResponseDate) ? "none" : "block")
                        };

                        if (!model.FromReviewPage || CurrentUser.Uid == SiteKey.AshishTeamPMUId || CurrentUser.PMUid == SiteKey.AshishTeamPMUId)
                        {
                            var ObjPreference = projectClosureService.GetDataByPmuid(PMUserId);
                            objSendMail.Subject = $"Project Update - {projectClosure.Project.Name} [{projectClosure.Project.CRMProjectId}]";
                            objSendMail.MailBodyManualSupply = true;
                            objSendMail.MailBody = objSendMail.GetHtml("ProjectClosureUpdateStatus.html");
                            if (ObjPreference != null && ObjPreference.ProjectClosureEmail.HasValue())
                            {
                                objSendMail.To = $"{ObjPreference.ProjectClosureEmail};{projectClosure.UserLogin?.EmailOffice };{projectClosure.UserLogin3?.EmailOffice}";
                            }
                            else
                            {
                                objSendMail.To = $"{projectClosure.UserLogin?.EmailOffice };{projectClosure.UserLogin3?.EmailOffice}";
                            }

                            objSendMail.From = SiteKey.From;
                            objSendMail.Send();
                        }

                        return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = true, Message = "Project Closure Status Updated successfully" });
                    }

                    return MessagePartialView("Unable to get Project Closure");
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

        #endregion

        #region Delete / Decline Project Closure

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Delete(int? id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Project Closure?",
                Size = Enums.ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Project Closure?" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult Delete(int id)
        {
            try
            {
                if (id > 0)
                {
                    var projectClosure = projectClosureService.projectClosureFindById(id);

                    if (projectClosure != null && projectClosure.PMID == CurrentUser.Uid || projectClosure.AddedBy == CurrentUser.Uid)
                    {
                        projectClosureService.Delete(id);
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record deleted successfully", IsSuccess = true });
                    }
                }

                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = "Unable to delete record" });
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult Decline(int id)
        {
            try
            {
                if (id > 0)
                {
                    var projectClosureEntity = projectClosureService.projectClosureFindById(id);

                    if (projectClosureEntity != null && !projectClosureEntity.CRMUpdated && projectClosureEntity.PMID == PMUserId && projectClosureEntity.Status != (int)Enums.CloserType.Converted)
                    {
                        var crmResponse = UpdateCRMStatus(projectClosureEntity.Project.CRMProjectId, Enums.ClosureApiRequestStatusType.Declined);

                        if (crmResponse != null && crmResponse.Status)
                        {
                            projectClosureService.Delete(id);
                            return NewtonSoftJsonResult(new RequestOutcome<string> { Message = $"Project closure declined successfully.<br>{crmResponse.Message}", IsSuccess = true });
                        }
                        else
                        {
                            var errorMessage = crmResponse?.Errors != null ? string.Join(", ", crmResponse.Errors) : crmResponse?.Message ?? "Some error while calling CRM API";
                            return NewtonSoftJsonResult(new RequestOutcome<string> { Message = $"Unable to revert CRM Status. Error : {errorMessage}" });
                        }
                    }
                }

                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = "Unable to decline closure" });
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = ex.Message });
            }
        }

        #endregion

        #region Chase Project Closure

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Chase(int id)
        {
            if (id > 0)
            {
                ProjectClousreDetailDto model = new ProjectClousreDetailDto();
                var entity = projectClosureService.projectClosureFindById(id);
                if (entity != null && entity.PMID == PMUserId)
                {
                    model.ProjectClousreId = id;
                    return PartialView("_ChaseProjectClosure", model);
                }
            }
            return MessagePartialView("Invaild Chase Id");
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult Chase(ProjectClousreDetailDto model)
        {
            if (!model.ExpectedNewWork)
            {
                ModelState.Remove("ConversionDate");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (!IsPM && !model.IsConfirmSubmit)
                    {
                        var prList = projectClosureService.GetProjectClosureOnDate(CurrentUser.Uid, model.NextStartDate.ToDateTime("dd/MM/yyyy") ?? DateTime.Now);
                        var plList = leadService.GetProjectLeadOnDate(CurrentUser.Uid, model.NextStartDate.ToDateTime("dd/MM/yyyy") ?? DateTime.Now);
                        if ((prList.Count() > 0 || plList.Count() > 0))
                        {
                            return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "You have already data for same date", Data = (prList.Count() + plList.Count()).ToString() });
                        }
                    }

                    model.AddedByUid = CurrentUser.Uid;
                    model.PMUid = PMUserId;

                    var result = projectClosureService.SaveDetail(model);

                    if (model.ExpectedNewWork)
                    {
                        var projectLead = CreateLead(model);

                        //return RedirectToAction("AddEditLead", "Estimate", new { id = projectLead.LeadId });
                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            RedirectUrl = Url.Action("AddEditLead", "Estimate", new { id = projectLead.LeadId })
                        });
                    }

                    if (result != null)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Chased Successfully", IsSuccess = true });
                    }
                    else
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Unable to save record" });
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

        private ProjectLead CreateLead(ProjectClousreDetailDto model)
        {
            ProjectLead projectLead = new ProjectLead
            {
                Status = (int)Enums.LeadStatus.ActionRequiredFrom_Team,
                Isdelivered = true,
                AddDate = DateTime.Now,
                StatusUpdateDate = DateTime.Now,
                PMID = PMUserId,
                LeadId = 0
            };
            try
            {

                using (TransactionScope trans = new TransactionScope())
                {
                    try
                    {
                        var entity = projectClosureService.projectClosureFindById(model.ProjectClousreId);

                        #region UK_PM(Lead From)
                        var abroadPMs = projectClosureService.GetAllAbroadPM(entity.Country);
                        AbroadPM abroadPM = abroadPMs.FirstOrDefault(ap => ap.isDefaultForEmail == true);
                        if (!String.IsNullOrEmpty(SiteKey.UKAUUserIDToShowAshishTeamActivity))
                        {
                            var abroadPM_AppSettings = abroadPMs.Where(x => !SiteKey.UKAUUserIDToShowAshishTeamActivity.Split(',').Contains(x.AutoID.ToString())).FirstOrDefault();
                            if (abroadPM_AppSettings != null)
                            {
                                abroadPM = abroadPM_AppSettings;
                            }
                        }

                        if (abroadPM != null)
                            projectLead.AbroadPMID = abroadPM.AutoID;

                        #endregion

                        projectLead.AssignedDate = DateTime.Now.ToString("dd/MM/yyyy").ToDateTime(false) ?? DateTime.Now;
                        projectLead.CommunicatorId = entity.Uid_BA ?? 0;
                        projectLead.IP = GeneralMethods.Getip();
                        projectLead.IsNewClient = false;
                        projectLead.Title = entity.Project.Name;
                        projectLead.TitleCheckSum = Guid.NewGuid().ToString().GetHashCode();
                        projectLead.OwnerId = CurrentUser.Uid;
                        projectLead.Technologies = "";
                        projectLead.Notes = model.Reason;
                        //projectLead.LeadType = leadService.GetLeadType("Lead").Where(x => x.TypeName == "Promising").FirstOrDefault().TypeId;
                        projectLead.LeadType = leadService.GetLeadType("Lead").Where(x => x.TypeName == "Almost Converted").FirstOrDefault().TypeId;

                        projectLead.ModifyDate = DateTime.Now;
                        projectLead.EstimateTimeinDay = 0;
                        projectLead.LeadCRMId = "0";
                        projectLead.NextChasedDate = entity.NextStartDate;
                        projectLead.ProjectClosureId = entity.Id;
                        projectLead.ConversionDate = model.ConversionDate.ToDateTime("dd/MM/yyyy");

                        leadService.SaveLead(projectLead);

                        if (entity.Uid_Dev != null && entity.Uid_Dev > 0)
                        {
                            List<LeadTechnician> technicianList = new List<LeadTechnician>();
                            technicianList.Add(new LeadTechnician { LeadId = projectLead.LeadId, TechnicianId = Convert.ToInt32(entity.Uid_Dev) });
                            leadService.SaveLeadTechnicians(technicianList);
                        }

                        ShowSuccessMessage("Success", "Lead has been Successfully added", false);

                        CreateLeadHistory(projectLead.LeadId, entity.Id, (int)Enums.LeadStatus.ChaseRequest);
                        entity.IsNewLeadGenerated = true;
                        entity.Status = (int)Enums.CloserType.DeadResponse;
                        projectClosureService.Save(entity);

                        trans.Complete();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return projectLead;
        }

        private void CreateLeadHistory(int projectLeadId, int ProjectClousreId, int leadStatusId)
        {
            try
            {
                var entity = projectClosureService.GetProjectClosureDetail(ProjectClousreId);

                foreach (var item in entity)
                {
                    LeadTransaction leadTransaction = new LeadTransaction()
                    {
                        LeadId = projectLeadId,
                        AddDate = item.Created,
                        AddedBy = item.AddedByUid ?? CurrentUser.Uid,
                        Doc = null,
                        StatusId = leadStatusId,
                        Notes = item.Reason,
                        ProjectClosureId = ProjectClousreId
                    };
                    leadService.SaveLeadTransaction(leadTransaction);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        #endregion

        #region View Project Closure

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Detail(int id)
        {
            if (id > 0)
            {
                try
                {
                    ViewProjectClosureDto model = new ViewProjectClosureDto();
                    var entity = projectClosureService.projectClosureFindById(id);
                    //if (entity != null && (CurrentUser.RoleId == (int)Enums.UserRoles.Director || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM || entity.PMID == PMUserId
                    //    || this.IsUKAUUserIDToShowAshishTeamActivity))
                    if (entity != null)
                    {
                        model.DateOfClosing = entity.DateofClosing.ToFormatDateString();
                        model.TL_Name = entity.UserLogin3?.Name;
                        model.BA_Name = entity.UserLogin?.Name;
                        model.DeveloperName = entity.UserLogin2?.Name;
                        model.VirtualDeveloperName = entity.OtherActualDeveloper;
                        model.NextEngementDate = entity.NextStartDate.ToFormatDateString();
                        model.Reason = entity.Reason;
                        model.ProjectName = $"{ entity.Project.Name }[{entity.Project.CRMProjectId}]";
                        model.LiveUrl = entity.ProjectLiveUrl.HasValue() ? entity.ProjectLiveUrl : entity.ProjectUrlAbsenseReason;
                        model.ClientQuality = entity.ClientQuality.HasValue ? ((Enums.ClientQualtiy)entity.ClientQuality.Value).GetDescription() : "";
                        model.ClientName = entity.Project.Client != null ? entity.Project.Client.Name : "";
                        model.CRMStatus = ((Enums.CRMStatus)entity.CRMStatus).GetDescription();
                        model.Suggestion = entity.Suggestion;
                        model.Technologies = string.Join(", ", entity.Project.Project_Tech.Select(x => x.Technology.Title).Distinct());

                        if (entity.ProjectClosureReview != null)
                        {
                            var review = entity.ProjectClosureReview;
                            model.PromisingPercentage = review.PromisingPercentageId.HasValue ? ((Enums.ProjectClosureReviewPercentage)review.PromisingPercentageId).GetDescription() : "";
                            model.DeveloperCount = review.DeveloperCount;
                            model.ReviewComments = review.Comments;
                            model.ProjectMayStartDate = review.NextStartDate.ToFormatDateString("ddd, dd MMM yyyy");
                        }

                        model.ChaseHistory = entity.ProjectClosureDetails.OrderByDescending(x => x.Id)
                            .Select(s => new ChaseHistoryDto
                            {
                                ChaseDate = s.Created.ToFormatDateString(),
                                AddedBy = s.UserLogin?.Name,
                                MessageText = s.Reason
                            }).ToList();

                        return PartialView("_ProjectClosureDetail", model);
                    }
                }
                catch (Exception ex)
                {
                    return MessagePartialView(ex.Message);
                }
            }

            return MessagePartialView("Invalid Project closure Id");
        }

        [HttpGet]
        [CustomAuthorization]
        public ActionResult History(int id, string view = "")
        {
            if (id > 0)
            {
                try
                {
                    var project = projectService.GetProjectById(id);

                    if (project == null)
                    {
                        return MessagePartialView("Project not found");
                    }

                    if (!IsDirector && project.PMUid != PMUserId && CurrentUser.RoleId != (int)Enums.UserRoles.PMOAU && CurrentUser.RoleId != (int)Enums.UserRoles.AUPM)
                    {
                        return MessagePartialView("Project not accessible");
                    }

                    var model = new ProjectClosureProjectDetailDto
                    {
                        ClientName = project.Client?.Name,
                        ProjectName = $"{project.Name} [{project.CRMProjectId}]",
                        ClosureHistory = project.ProjectClosures.OrderByDescending(x => x.DateofClosing)
                                                .Select(x => new ProjectClosureHistoryDto
                                                {
                                                    Id = x.Id,
                                                    ProjectId = x.ProjectID.Value,
                                                    DateOfClosing = x.DateofClosing.ToFormatDateString("MMM dd yyyy"),
                                                    CRMStatus = x.CRMStatus.HasValue ? ((Enums.CRMStatus?)x.CRMStatus.Value).ToString() : "",
                                                    StartEndDate = StartEndDates(x.StartDate, x.EndDate),
                                                    Invoice = (x.InvoiceDays ?? 0) > 0 ? DaysToWeeks(x.InvoiceDays.Value) : "-",
                                                    Estimate = (x.EstimateDays ?? 0) > 0 ? DaysToWeeks(x.EstimateDays.Value) : "-",
                                                    Reason = x.Reason,
                                                    Suggestion = x.Suggestion,
                                                    IsNewLeadGenerated = x.IsNewLeadGenerated,
                                                    //NewLeadId = (x.IsNewLeadGenerated == true ? leadService.GetProjectLeadByProjectClosureId(x.Id).LeadId : 0),
                                                    NewLeadId = (x.IsNewLeadGenerated == true ? leadService.GetProjectClosureLeadId(x.Id) : 0),
                                                    AssignedToBA = ((x.UserLogin != null ? x.UserLogin.Name : "--") + " / " + (x.UserLogin3 != null ? x.UserLogin3.Name : "--")),
                                                    //IsAllowToViewProjectClosureDetail = (CurrentUser.RoleId == (int)Enums.UserRoles.Director || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM || x.PMID == PMUserId || this.IsUKAUUserIDToShowAshishTeamActivity)
                                                }).ToList()

                    };

                    if (!string.IsNullOrEmpty(view))
                    {
                        return PartialView(view, model);
                    }

                    return PartialView("_ProjectClosureHistory", model);
                }
                catch (Exception ex)
                {
                    return MessagePartialView(ex.Message);
                }
            }

            return MessagePartialView("Invalid Project Id");
        }

        #endregion

        #region Download Excel

        public ActionResult DownloadExcel()
        {
            var searchFilter = TempData.Get<ProjectClosureSearchFilter>("ProjectClosureFilters");
            if (searchFilter != null)
            {
                var expr = GetClosureFilterExpersion(searchFilter);
                var pagingServices = new PagingService<ProjectClosure>(0, int.MaxValue);
                pagingServices.Filter = expr;
                pagingServices.Sort = (o) =>
                {
                    return o.OrderByDescending(c => c.Modified);
                };

                int totalCount = 0;
                var projectClosures = projectClosureService.GetProjectClosurePaging(out totalCount, pagingServices);

                if (projectClosures.Count > 0)
                {
                    var records = projectClosures.Select(r => new
                    {
                        CRMId = r.Project.CRMProjectId,
                        ProjectName = r.Project.Name,
                        ClientName = r.Project.Client?.Name ?? "",
                        Status = ((Enums.CloserType)r.Status).GetDescription(),
                        ProjectStatus = r.CRMStatus != null ? ((Enums.CRMStatus)r.CRMStatus.Value).GetDescription() : "",
                        Quality = r.ClientQuality != null ? ((Enums.ClientQualtiy)r.ClientQuality.Value).GetDescription() : "",
                        Technology = string.Join(",", r.Project.Project_Tech.Select(x => x.Technology.Title).Distinct()),
                        DateOfChasing = (r.ProjectClosureDetails != null && r.ProjectClosureDetails.Count > 1) ? (r.ProjectClosureDetails.LastOrDefault().Created).ToFormatDateString("dd/MM/yyyy") : "",
                        LatestConversation = (r.ProjectClosureDetails != null && r.ProjectClosureDetails.Count > 0) ? r.ProjectClosureDetails.Select(x => x.Reason).LastOrDefault() : " ",
                        NextDate = r.NextStartDate != null ? r.DateofClosing.ToFormatDateString("MMM d, yyyy") : "",
                        BAName = r.UserLogin?.Name ?? "",
                        PMName = r.UserLogin1?.Name,
                        StartEndDate = StartEndDates(r.StartDate, r.EndDate),
                        DateOfClosing = r.DateofClosing.ToFormatDateString("MMM d, yyyy"),
                        Estimate = (r.EstimateDays ?? 0) > 0 ? DaysToWeeks(r.EstimateDays.Value) : "-",
                        Invoice = (r.InvoiceDays ?? 0) > 0 ? DaysToWeeks(r.InvoiceDays.Value) : "-",
                        BucketHours = r.BucketHours.HasValue && r.BucketHours > 0.0f ? $"{r.BucketHours} hours" : "",
                        TLName = r.UserLogin3?.Name ?? " ",
                        ActualDeveloper = (r.UserLogin2?.Name ?? "") + "," + (r.OtherActualDeveloper ?? "").Trim().Trim(',') ?? "",
                        ChaseHistory = (r.ProjectClosureDetails != null && r.ProjectClosureDetails.Count > 0) ? "[Discussion on " + r.Created.ToFormatDateString("dd/MM/yyyy") + "] : " + string.Join("|", r.ProjectClosureDetails.Select(x => x.Reason)) : "",
                    }).ToList();

                    string filename = "ProjectEngagementReport_" + DateTime.Now.Ticks.ToString() + ".xlsx";
                    string[] columns = { "CRMId", "ProjectName", "ClientName", "Status", "ProjectStatus", "Quality", "Technology", "DateOfChasing", "LatestConversation", "NextDate", "PMName", "BAName", "StartEndDate", "DateOfClosing", "Estimate", "Invoice", "BucketHours", "TLName", "ActualDeveloper", "ChaseHistory" };
                    byte[] filecontent = ExportExcelHelper.ExportExcel(records, "Project Engagement Report", true, columns);

                    return File(filecontent, ExportExcelHelper.ExcelContentType, filename);
                }

                return Content("No record found");
            }

            return Content("Unable to get filters");
        }

        public ActionResult DownloadReportExcel()
        {
            var searchFilter = TempData.Get<ProjectClosureSearchFilter>("ProjectClosureReportFilters");
            if (searchFilter != null)
            {
                var pagingServices = new PagingService<ProjectClosure>(0, int.MaxValue);
                searchFilter.chaseStatus = 3;
                var expr = GetClosureFilterExpersion(searchFilter, reportFilter: true);

                pagingServices.Filter = expr.And(x => x.DateofClosing.HasValue);

                //Sorting is giving in service call
                //pagingServices.Sort = (o) =>
                //{
                //    return o.OrderBy(c => c.CRMUpdated).ThenByDescending(c => c.DateofClosing.Value);
                //};

                int totalCount = 0;
                var projectClosures = projectClosureService.GetProjectClosureReportPaging(out totalCount, pagingServices);

                if (projectClosures.Count > 0)
                {
                    var records = projectClosures.Select(r => new
                    {
                        CRMId = r.Project.CRMProjectId,
                        Client = r.Project.Client?.Name ?? " ",
                        Country = r.Country,
                        Project = r.Project.Name,
                        Status = r.CRMStatus.HasValue ? ((Enums.CRMStatus)r.CRMStatus.Value).GetDescription() : "",
                        ProjectStatus = ((Enums.CloserType)r.Status).GetDescription(),
                        Quality = r.ClientQuality.HasValue ? ((Enums.ClientQualtiy)r.ClientQuality.Value).GetDescription() : "",
                        StartEndDate = StartEndDates(r.StartDate, r.EndDate),
                        DateOfClosing = r.DateofClosing.ToFormatDateString("MMM d, yyyy"),
                        Estimate = (r.EstimateDays ?? 0) > 0 ? DaysToWeeks(r.EstimateDays.Value) : "-",
                        Invoice = (r.InvoiceDays ?? 0) > 0 ? DaysToWeeks(r.InvoiceDays.Value) : "-",
                        BucketHours = r.BucketHours.HasValue && r.BucketHours > 0.0f ? $"{r.BucketHours} hours" : "",
                        BA_Name = r.UserLogin?.Name,
                        TL_Name = r.UserLogin3?.Name,
                        TM_Name = r.UserLogin1?.Name,
                        Technology = string.Join(", ", r.Project.Project_Tech.Select(x => x.Technology.Title).Distinct()),
                        Reason = Common.StripHTMLTagsRegex(r.Reason),
                        Suggestion = Common.StripHTMLTagsRegex(r.Suggestion),
                    }).ToList();

                    string filename = "ProjectClosureReport_" + DateTime.Now.Ticks.ToString() + ".xlsx";
                    string[] columns = { "CRMId", "Client", "Country", "Project", "Status", "ProjectStatus", "Quality", "StartEndDate", "DateOfClosing", "Estimate", "Invoice", "BucketHours", "BA_Name", "TL_Name", "TM_Name", "Technology", "Reason", "Suggestion" };
                    byte[] filecontent = ExportExcelHelper.ExportExcel(records, "Project Closure Report", true, columns);

                    return File(filecontent, ExportExcelHelper.ExcelContentType, filename);
                }

                return Content("No record found");
            }

            return Content("Unable to get filters");
        }

        public ActionResult DownloadProjectionReport()
        {
            var searchFilter = TempData.Get<ProjectClosureSearchFilter>("ProjectionReportFilters");
            if (searchFilter != null)
            {
                var expr = GetClosureFilterExpersion(searchFilter);
                var pagingServices = new PagingService<ProjectClosure>(0, int.MaxValue);
                pagingServices.Filter = expr;
                pagingServices.Sort = (o) =>
                {
                    return o.OrderByDescending(c => c.Modified);
                };

                int totalCount = 0;
                var projectClosures = projectClosureService.GetProjectClosurePaging(out totalCount, pagingServices);

                if (projectClosures.Count > 0)
                {
                    var records = projectClosures.Select(r => new
                    {
                        CRMId = r.Project.CRMProjectId,
                        ProjectName = r.Project.Name,
                        ClientName = r.Project.Client?.Name ?? "",
                        Status = ((Enums.CloserType)r.Status).GetDescription(),
                        ProjectStatus = r.CRMStatus != null ? ((Enums.CRMStatus)r.CRMStatus.Value).GetDescription() : "",
                        Quality = r.ClientQuality != null ? ((Enums.ClientQualtiy)r.ClientQuality.Value).GetDescription() : "",
                        Technology = string.Join(",", r.Project.Project_Tech.Select(x => x.Technology.Title).Distinct()),
                        DateOfChasing = (r.ProjectClosureDetails != null && r.ProjectClosureDetails.Count > 1) ? (r.ProjectClosureDetails.LastOrDefault().Created).ToFormatDateString("dd/MM/yyyy") : "",
                        LatestConversation = (r.ProjectClosureDetails != null && r.ProjectClosureDetails.Count > 0) ? r.ProjectClosureDetails.Select(x => x.Reason).LastOrDefault() : " ",
                        NextDate = r.NextStartDate != null ? r.DateofClosing.ToFormatDateString("MMM d, yyyy") : "",
                        BAName = r.UserLogin?.Name ?? "",
                        PMName = r.UserLogin1?.Name,
                        StartEndDate = StartEndDates(r.StartDate, r.EndDate),
                        DateOfClosing = r.DateofClosing.ToFormatDateString("MMM d, yyyy"),
                        Estimate = (r.EstimateDays ?? 0) > 0 ? DaysToWeeks(r.EstimateDays.Value) : "-",
                        Invoice = (r.InvoiceDays ?? 0) > 0 ? DaysToWeeks(r.InvoiceDays.Value) : "-",
                        BucketHours = r.BucketHours.HasValue && r.BucketHours > 0.0f ? $"{r.BucketHours} hours" : "",
                        TLName = r.UserLogin3?.Name ?? " ",
                        ActualDeveloper = (r.UserLogin2?.Name ?? "") + "," + (r.OtherActualDeveloper ?? "").Trim().Trim(',') ?? "",
                        ChaseHistory = (r.ProjectClosureDetails != null && r.ProjectClosureDetails.Count > 0) ? "[Discussion on " + r.Created.ToFormatDateString("dd/MM/yyyy") + "] : " + string.Join("|", r.ProjectClosureDetails.Select(x => x.Reason)) : "",
                    }).ToList();

                    string filename = "ProjectEngagementReport_" + DateTime.Now.Ticks.ToString() + ".xlsx";
                    string[] columns = { "CRMId", "ProjectName", "ClientName", "Status", "ProjectStatus", "Quality", "Technology", "DateOfChasing", "LatestConversation", "NextDate", "PMName", "BAName", "StartEndDate", "DateOfClosing", "Estimate", "Invoice", "BucketHours", "TLName", "ActualDeveloper", "ChaseHistory" };
                    byte[] filecontent = ExportExcelHelper.ExportExcel(records, "Project Engagement Report", true, columns);

                    return File(filecontent, ExportExcelHelper.ExcelContentType, filename);
                }

                return Content("No record found");
            }

            return Content("Unable to get filters");
        }

        #endregion

        private ResponseModel<string> UpdateCRMStatus(int CRMId, Enums.ClosureApiRequestStatusType requestStatusType)
        {
            var crmResponse = new ResponseModel<string>();
            try
            {
                var request = WebRequest.CreateHttp(SiteKey.CRMApiProjectClosureUpdateUrl);
                StringBuilder postData = new StringBuilder();
                postData.Append($"crmid={CRMId}&");
                postData.Append($"status={requestStatusType.ToString().ToLower()}&");
                postData.Append($"user_email={CurrentUser.EmailOffice ?? ""}");

                WriteLogFile($"===== Update CRM Status {DateTime.Now} ======\n\nRequest Parameters : {postData.ToString()}");

                request.Headers.Add("userid", SiteKey.CRMApiUser);
                request.Headers.Add("password", SiteKey.CRMApiPassword);

                var data = Encoding.ASCII.GetBytes(postData.ToString());

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)(request.GetResponse());

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    if (responseData.HasValue())
                    {
                        WriteLogFile($"Response : {responseData}");

                        crmResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseModel<string>>(responseData);
                    }
                    else
                    {
                        WriteLogFile($"Response : No response from API");
                    }
                }
                else
                {
                    WriteLogFile($"Error Response : Code = {response.StatusCode} Description = {response.StatusDescription}");

                    crmResponse.Code = response.StatusCode;
                    crmResponse.Errors = new string[] { response.StatusDescription };
                }
            }
            catch (Exception ex)
            {
                WriteLogFile($"Exception Response : {(ex.InnerException ?? ex).Message}");

                crmResponse.Code = HttpStatusCode.InternalServerError;
                crmResponse.Errors = new string[] { (ex.InnerException ?? ex).Message };
            }

            WriteLogFile($"===== Update CRM Status End {DateTime.Now} ======");

            return crmResponse;
        }

        private void SendClosureEmail(ProjectClosure projectClosureEntity, ProjectClosureDto abordModel)
        {
            try
            {
                FlexiMail objSendMail = new FlexiMail();

                var entityProject = projectService.GetProjectById(projectClosureEntity.ProjectID.Value);

                objSendMail.ValueArray = new string[]
                {
                    CurrentUser.Name.ToTitleCase(),
                   entityProject.CRMProjectId.ToString(),
                    projectClosureEntity.DateofClosing.ToFormatDateString("MMM d, yyyy"),
                   entityProject.Client.Name,
                    projectClosureEntity.ClientQuality.HasValue? ((Enums.ClientQualtiy)projectClosureEntity.ClientQuality).GetDescription(): "",
                    $"{ entityProject.Name }[{ entityProject.CRMProjectId}]",
                    ((Enums.CRMStatus)projectClosureEntity.Status).GetDescription(),
                    projectClosureEntity.ProjectLiveUrl != null ? projectClosureEntity.ProjectLiveUrl : projectClosureEntity.ProjectUrlAbsenseReason,
                    string.Join(",",entityProject.Project_Tech.Select(x=>x.Technology.Title).Distinct().ToList()),
                    projectClosureEntity.UserLogin2?.Name,
                    projectClosureEntity.OtherActualDeveloper,
                    projectClosureEntity.UserLogin?.Name,
                    projectClosureEntity.UserLogin3?.Name,
                    projectClosureEntity.NextStartDate.ToFormatDateString("MMM d, yyyy"),
                    projectClosureEntity.Reason,
                    projectClosureEntity.Suggestion,
                    CurrentUser.EmailOffice.ToTitleCase(),
                    CurrentUser.MobileNumber.ToTitleCase(),
                };

                var ObjPreference = projectClosureService.GetDataByPmuid(PMUserId);
                string covid19 = abordModel.IsCovid19 ? "! Covid 19 Impact - " : string.Empty;
                objSendMail.Subject = $"{covid19}Project Closure Report - {entityProject.Name} [{entityProject.CRMProjectId}]";
                objSendMail.MailBodyManualSupply = true;

                objSendMail.MailBody = objSendMail.GetHtml("ProjectClosureReport.html");

                List<SelectListItem> ToEmailList = new List<SelectListItem>();
                List<SelectListItem> CCEmailList = new List<SelectListItem>();


                //if (abordModel.isAshishTeamMember) {
                var aboardEmails = abordModel.AbroadPMList.Where(x => x.Selected == true).Select(x => x.Email);
                foreach (var item in aboardEmails)
                {
                    ToEmailList.Add(new SelectListItem { Text = item });
                }
                var toAbordPMEmail = string.Join("; ", ToEmailList.Select(x => x.Text));
                objSendMail.To = toAbordPMEmail;

                if (ObjPreference != null && ObjPreference.ProjectClosureEmail.HasValue())
                {
                    if (!string.IsNullOrEmpty(ObjPreference.ProjectClosureEmail))
                    {
                        CCEmailList.Add(new SelectListItem { Text = ObjPreference.ProjectClosureEmail });

                    }
                    if (!string.IsNullOrEmpty(projectClosureEntity.UserLogin?.EmailOffice))
                    {
                        CCEmailList.Add(new SelectListItem { Text = projectClosureEntity.UserLogin?.EmailOffice });

                    }
                    if (!string.IsNullOrEmpty(projectClosureEntity.UserLogin3?.EmailOffice))
                    {
                        CCEmailList.Add(new SelectListItem { Text = projectClosureEntity.UserLogin3?.EmailOffice });
                    }
                }

                else
                {

                    if (!string.IsNullOrEmpty(projectClosureEntity.UserLogin?.EmailOffice))
                    {
                        CCEmailList.Add(new SelectListItem { Text = projectClosureEntity.UserLogin?.EmailOffice });

                    }
                    if (!string.IsNullOrEmpty(projectClosureEntity.UserLogin3?.EmailOffice))
                    {
                        CCEmailList.Add(new SelectListItem { Text = projectClosureEntity.UserLogin3?.EmailOffice });

                    }
                }

                if (projectClosureEntity.ClientBadge != null && ((projectClosureEntity.ClientBadge == (byte)Enums.ClientBadge.PLATINUM) || (projectClosureEntity.ClientBadge == (byte)Enums.ClientBadge.GOLD)))
                {
                    CCEmailList.Add(new SelectListItem { Text = SiteKey.ProjectClosureClientBadgeCCEmail });
                }

                var ccAbordPMEmail = string.Join("; ", CCEmailList.Select(x => x.Text));
                objSendMail.CC = ccAbordPMEmail;

                //}
                //else {

                //    if (ObjPreference != null && ObjPreference.ProjectClosureEmail.HasValue()) {
                //        if (!string.IsNullOrEmpty(ObjPreference.ProjectClosureEmail)) {
                //            ToEmailList.Add(new SelectListItem { Text = ObjPreference.ProjectClosureEmail });

                //        }
                //        if (!string.IsNullOrEmpty(projectClosureEntity.UserLogin?.EmailOffice)) {
                //            ToEmailList.Add(new SelectListItem { Text = projectClosureEntity.UserLogin?.EmailOffice });

                //        }
                //        if (!string.IsNullOrEmpty(projectClosureEntity.UserLogin3?.EmailOffice)) {
                //            ToEmailList.Add(new SelectListItem { Text = projectClosureEntity.UserLogin3?.EmailOffice });
                //        }
                //    }

                //    else {

                //        if (!string.IsNullOrEmpty(projectClosureEntity.UserLogin?.EmailOffice)) {
                //            ToEmailList.Add(new SelectListItem { Text = projectClosureEntity.UserLogin?.EmailOffice });

                //        }
                //        if (!string.IsNullOrEmpty(projectClosureEntity.UserLogin3?.EmailOffice)) {
                //            ToEmailList.Add(new SelectListItem { Text = projectClosureEntity.UserLogin3?.EmailOffice });

                //        }
                //    }

                //    var toEmails = string.Join("; ", ToEmailList.Select(x => x.Text));
                //    objSendMail.To = toEmails;
                //    if (projectClosureEntity.ClientBadge != null && ((projectClosureEntity.ClientBadge == (byte)Enums.ClientBadge.PLATINUM) || (projectClosureEntity.ClientBadge == (byte)Enums.ClientBadge.GOLD))) {
                //        objSendMail.CC = $"{SiteKey.ProjectClosureClientBadgeCCEmail}";
                //    }
                //}

                objSendMail.From = SiteKey.From;
                objSendMail.Send();
            }
            catch (Exception)
            {
            }
        }

        #region Call this method to update last Project Closure Status based on Project Status

        private void UpdateProjectClosuresStatusFromProject()
        {
            var pagingServices = new PagingService<Project>(0, int.MaxValue);

            pagingServices.Filter = PredicateBuilder.True<Project>().And(x => x.Status == "R" && !x.IsInHouse);

            pagingServices.Sort = (o) =>
            {
                return o.OrderBy(c => c.ProjectId);
            };

            int totalCount = 0;
            var projects = projectService.GetProjectsByPaging(out totalCount, pagingServices);

            foreach (var project in projects)
            {
                var closure = project.ProjectClosures.OrderByDescending(x => x.Id).FirstOrDefault();
                try
                {
                    if (closure != null && closure.Status != (int)Enums.CloserType.Converted)
                    {
                        closure.Status = (int)Enums.CloserType.Converted;
                        projectClosureService.Save(closure);
                    }
                }
                catch
                {
                }
            }
        }

        #endregion

        #region "Project Closure detail"

        public ViewProjectClosureDto GetProjectClosureDetail(int id, bool isAnonymousRequest = false)
        {
            ViewProjectClosureDto model = new ViewProjectClosureDto();
            var entity = projectClosureService.projectClosureFindById(id);
            //if (entity != null && (CurrentUser.RoleId == (int)Enums.UserRoles.Director || entity.PMID == PMUserId))
            if (entity != null && (CurrentUser.RoleId == (int)Enums.UserRoles.Director || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM || entity.PMID == PMUserId || isAnonymousRequest == true))
            {
                model.DateOfClosing = entity.DateofClosing.ToFormatDateString();
                model.TL_Name = entity.UserLogin3?.Name;
                model.BA_Name = entity.UserLogin?.Name;
                model.DeveloperName = entity.UserLogin2?.Name;
                model.VirtualDeveloperName = entity.OtherActualDeveloper;
                model.NextEngementDate = entity.NextStartDate.ToFormatDateString();
                model.Reason = entity.Reason;
                model.ProjectName = $"{ entity.Project.Name } [{entity.Project.CRMProjectId}]";
                model.LiveUrl = entity.ProjectLiveUrl.HasValue() ? entity.ProjectLiveUrl : entity.ProjectUrlAbsenseReason;
                model.ClientQuality = entity.ClientQuality.HasValue ? ((Enums.ClientQualtiy)entity.ClientQuality.Value).GetDescription() : "";
                model.ClientName = entity.Project.Client != null ? entity.Project.Client.Name : "";
                model.CRMStatus = ((Enums.CRMStatus)entity.CRMStatus).GetDescription();
                model.Suggestion = entity.Suggestion;
                model.Technologies = string.Join(", ", entity.Project.Project_Tech.Select(x => x.Technology.Title).Distinct());

                if (entity.ProjectClosureReview != null)
                {
                    var review = entity.ProjectClosureReview;
                    model.PromisingPercentage = review.PromisingPercentageId.HasValue ? ((Enums.ProjectClosureReviewPercentage)review.PromisingPercentageId).GetDescription() : "";
                    model.DeveloperCount = review.DeveloperCount;
                    model.ReviewComments = review.Comments;
                    model.ProjectMayStartDate = review.NextStartDate.ToFormatDateString("ddd, dd MMM yyyy");
                }
            }
            return model;
        }

        #endregion

        #region "Code to generate pdf"

        [HttpGet]
        [CustomAuthorization]
        public ActionResult DownloadPDF(int id)
        {
            ViewProjectClosureDto model = new ViewProjectClosureDto();
            if (id > 0)
            {
                model = GetProjectClosureDetail(id);
            }
            return new ViewAsPdf("ProjectClosurePDFDownload", model) { FileName = $"{model.ProjectName.ToSelfURL()}.pdf" };
        }

        #endregion "Code to generate pdf"

        #region [Calling for closure report API call]

        private string PMSApiCallForAddClosureReport(ProjectClosure projectClosure)
        {
            try
            {
                int CRMId = projectClosure.Project.CRMProjectId;
                string ClosureReportFileName = Convert.ToString(CRMId) + "-" + projectClosure.Project.Name.ToSelfURL() + ".pdf";
                string generatePDFFile = DownloadPDFFileInTemp(projectClosure.Id, ClosureReportFileName);
                if (System.IO.File.Exists(generatePDFFile))
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(generatePDFFile);
                    if (System.IO.File.Exists(generatePDFFile))
                    {
                        System.IO.File.Delete(generatePDFFile);
                    }

                    string MemberEmail = CurrentUser.EmailOffice;

                    string ClosureReportDocument = Convert.ToBase64String(bytes);

                    using (var client = new HttpClient())
                    {
                        //set up client
                        client.BaseAddress = new Uri(SiteKey.PMSApiAddClosureReportUrl);
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        client.DefaultRequestHeaders.Add("ApiKey", SiteKey.PMSApiKey);
                        client.DefaultRequestHeaders.Add("ApiPassword", SiteKey.PMSApiPassword);

                        var values = new Dictionary<string, string>();
                        values.Add("CRMId", Convert.ToString(CRMId));
                        values.Add("MemberEmail", MemberEmail);
                        values.Add("ClosureReportDocument", ClosureReportDocument);
                        values.Add("ClosureReportFileName", ClosureReportFileName);

                        var jsonString = JsonConvert.SerializeObject(values);
                        try
                        {
                            WriteLogFilePMS($"Request : {jsonString}");
                            HttpResponseMessage Res = client.PostAsync(SiteKey.PMSApiAddClosureReportUrl, new StringContent(jsonString, Encoding.UTF8, "application/json")).Result;
                            var jsonResult = Res.Content.ReadAsStringAsync();
                            WriteLogFilePMS($"Response : {jsonResult.Result}");
                        }
                        catch (Exception ex)
                        {
                            WriteLogFilePMS($"Exception Response : {(ex.InnerException ?? ex).Message}");
                        }
                    }
                }
                else
                {
                    WriteLogFilePMS($"Exception Response : File not created.");
                }
            }
            catch (Exception ex)
            {
                WriteLogFilePMS($"Exception Response : {(ex.InnerException ?? ex).Message}");
            }
            return "";
        }

        public string DownloadPDFFileInTemp(int id, string fileName)
        {
            string EMSReportPath = "\\ReportFiles\\";
            string headerUrl = string.Empty, footerUrl = string.Empty, generatePDFName = string.Empty;
            generatePDFName = ContextProvider.HostEnvironment.WebRootPath + EMSReportPath + fileName;

            if (System.IO.File.Exists(generatePDFName))
            {
                System.IO.File.Delete(generatePDFName);
            }

            if (id > 0)
            {
                generatePDFName = ContextProvider.HostEnvironment.WebRootPath + EMSReportPath + "/" + fileName;
                ViewProjectClosureDto model = new ViewProjectClosureDto();
                model = GetProjectClosureDetail(id);

                string strURL = string.Format("{0}projectclosure/DownloadPDFForAPI?id={1}", SiteKey.DomainName, id);
                HtmlToPdfbyContent(strURL, generatePDFName, null, null, "10mm", "Portrait");
            }
            return generatePDFName;
        }

        [NonAction]
        public bool HtmlToPdfbyContent(string URL, string fileName, string headerUrl = null, string footerUrl = null, params string[] metaInfo)
        {
            string WKHTMLDirectoryPath = "/Rotativa/";
            string WKHTMLFileName = "wkhtmltopdf.exe";
            string workingDir = ContextProvider.HostEnvironment.WebRootPath + WKHTMLDirectoryPath;
            var exePath = (workingDir + WKHTMLFileName);
            var p = new System.Diagnostics.Process
            {
                StartInfo =
                {
                    FileName = @"""" + exePath + @"""",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                }
            };

            string switches = Uri.EscapeUriString(URL);
            if (!string.IsNullOrEmpty(headerUrl))
            {
                switches += " -T 5mm -B 2mm -L 5mm -R 5mm --header-spacing 10 --header-html \"" + Uri.EscapeUriString(headerUrl) + "\"";
            }

            if (!string.IsNullOrEmpty(footerUrl))
            {
                switches += "--footer-center --footer-font-size 6 --footer-line --footer-html \"" + Uri.EscapeUriString(footerUrl) + "\"";
            }
            switches += " \"" + fileName + "\"";
            p.StartInfo.Arguments = switches;
            p.Start();
            p.WaitForExit();
            int returnCode = p.ExitCode;
            p.Close();
            p.Dispose();
            return (returnCode <= 2);
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public ActionResult DownloadPDFForAPI(int id)
        {
            ViewProjectClosureDto model = new ViewProjectClosureDto();
            if (id > 0)
            {
                model = GetProjectClosureDetail(id, true);
            }
            return View("ProjectClosurePDFDownload", model);
        }

        //[HttpPost]
        //[CustomActionAuthorization]
        //public IActionResult ProjectionReportWeekWise(ProjectClosureSearchFilter searchFilter)
        //{
        //    List<ProjectionWeek> weeks = new List<ProjectionWeek>();
        //    List<string> colors = new List<string>() { "#ff6347", "#90ee90", "#add8e6", "#fafad2", "#fbca95", "#b84dff" }; // light red,green,blue,yellow etc. colors hex code
        //    var currentDate = DateTime.Today;
        //    CultureInfo culture = new CultureInfo("en-GB");
        //    DateTime weekEndDate = new DateTime();
        //    for (int i = 1; i < 13; i++) // 12 weeks
        //    {
        //        ProjectionWeek week = new ProjectionWeek();
        //        var weekStartDate = Common.GetStartDateOfWeek(currentDate, culture);
        //        week.StartDate = weekStartDate> DateTime.Today ? weekStartDate: DateTime.Today;

        //        week.WeekNo = Common.GetWeekOfMonth(week.StartDate, culture); // gets start date for week
        //        week.Month = week.StartDate.Month;

        //        weekEndDate = Common.GetEndDateOfWeek(currentDate, culture); //  gets end date for week
        //        var monthEndDate = Common.LastDayOfMonth(week.StartDate);
        //        // sets end date for week to month end date if week end date falls in next month
        //        week.EndDate = weekEndDate > monthEndDate ? monthEndDate : weekEndDate; 
        //        week.StartDateEndDate = week.StartDate.ToString("dd MMM") + "-" + week.EndDate.ToString("dd MMM"); // date concatenation

        //        // gets developer count
        //        week.DeveloperCount = projectClosureReviewService.GetForcastOccupancyForWeek(GetProjectionReportFilterExperession(week, searchFilter));
        //        weeks.Add(week);

        //        if (weekEndDate > monthEndDate) // week falls to next month
        //        {
        //            ProjectionWeek nextWeek = new ProjectionWeek(); // new week 
        //            nextWeek.StartDate = monthEndDate.AddDays(1);
        //            nextWeek.EndDate = weekEndDate;
        //            nextWeek.Month = nextWeek.StartDate.Month;
        //            nextWeek.WeekNo = Common.GetWeekOfMonth(nextWeek.StartDate, culture);
        //            nextWeek.StartDateEndDate = nextWeek.StartDate.ToString("dd MMM") + "-" + nextWeek.EndDate.ToString("dd MMM");
        //            weeks.Add(nextWeek); // adds new week to weeeks
        //        }

        //        currentDate = currentDate.AddDays(7); // move by 7 days as each next weeks is 7 days from current week
        //    }
        //    ProjectionReportWeeksDto response = new ProjectionReportWeeksDto();
        //    // fills ProjectionReportWeeksDto 
        //    response.months = weeks.GroupBy(x => x.Month).Select((s,i) => new ProjectionMonth
        //    {
        //        Month = s.Key,
        //        MonthName = s.FirstOrDefault().StartDate.ToString("MMM yyyy"), // i.e. Nov 19
        //        Weeks = s.ToList(), // fills weeks collection 
        //        TotalMonthOccupancy=s.Sum(c=>c.DeveloperCount), //sets developers count for month
        //        Colspan =s.Count(), // sets colspan for month based on week count
        //        Color=colors[i] // sets color for month
        //    }).ToList();
        //    return new JsonResult(response);
        //}

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult ProjectionReportWeekWise(ProjectClosureSearchFilter searchFilter)
        {
            searchFilter.IsForcastOccupancy = true;
            return PrepareReport(DateTime.Today, searchFilter); //12 week from current week
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult ProjectionReportPast(ProjectClosureSearchFilter searchFilter)
        {
            return PrepareReport(DateTime.Today.AddDays(-84), searchFilter); // 12 weeks old
        }

        //public IActionResult ProjectionReportWeekWise()
        //{
        //    ProjectionReportWeeksDto projectionReportWeeksDto = new ProjectionReportWeeksDto();
        //    var currentDate = DateTime.Today;
        //    var currentMonth = DateTime.Today.Month;
        //    for (int i = 1; i < 13; i++)
        //    {
        //        //if (currentMonth != week.StartDate.Month)
        //        //{
        //        ProjectionWeek week = new ProjectionWeek();
        //        week.WeekNo = i;
        //        week.StartDate = Common.GetStartDateOfWeek(currentDate);
        //        week.Month = week.StartDate.Month;
        //        week.EndDate = Common.GetEndDateOfWeek(currentDate);
        //        week.StartDateEndDate = week.StartDate.ToString("dd MMM yy") + "-" + week.EndDate.ToString("dd MMM yy");
        //        week.DeveloperCount = projectClosureReviewService.GetForcastOccupancyForWeek(week);
        //        projectionReportWeeksDto.weeks.Add(week);
        //        currentDate = currentDate.AddDays(7);
        //        //}
        //    }
        //    projectionReportWeeksDto.weeks[0].StartDate = DateTime.Today; // First 
        //    //projectionReportWeeksDto.weeks[0].StartDate = DateTime.Today;
        //    projectionReportWeeksDto.weeks[0].StartDateEndDate = projectionReportWeeksDto.weeks[0].StartDate.ToString("dd MMM yy") + "-" +
        //        projectionReportWeeksDto.weeks[0].EndDate.ToString("dd MMM yy");

        //    return new JsonResult(projectionReportWeeksDto);
        //}

        #endregion
        /// <summary>
        /// Prepares filter expression
        /// </summary>
        /// <param name="projectionWeek">week for projection</param>
        /// <returns>filter</returns>
        [HttpPost]
        [CustomActionAuthorization]
        private Expression<Func<ProjectClosureReview, bool>> GetProjectionReportFilterExperession(ProjectionWeek projectionWeek,
            ProjectClosureSearchFilter searchFilter = null, bool isConvertedData = false)
        {            
            var expr = PredicateBuilder.True<ProjectClosureReview>()
            .And(x => x.NextStartDate.HasValue &&
        ((DateTime)x.NextStartDate).Date >= projectionWeek.StartDate && ((DateTime)x.NextStartDate).Date <= projectionWeek.EndDate);
            if (CurrentUser.RoleId == (int)Enums.UserRoles.Director || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM)
            {
                if (searchFilter != null && searchFilter.PMUid.HasValue)
                {
                    expr = expr.And(x => x.ProjectClosure.PMID == searchFilter.PMUid);
                }

            }
            else if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
            {
                expr = expr.And(x => x.ProjectClosure.PMID == CurrentUser.Uid);
            }
            else if (
                RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)            
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
                //|| RoleValidator.DV_RoleIds.Contains(CurrentUser.RoleId)
                || RoleValidator.BA_RoleIds.Contains(CurrentUser.RoleId))
            {
                expr = expr.And(x => x.ProjectClosure.PMID == CurrentUser.PMUid);
            }
            else
            {
                expr = expr.And(x => (x.CreateByUid == CurrentUser.Uid || x.ProjectClosure.Uid_Dev == CurrentUser.Uid ||
                                      x.ProjectClosure.Uid_BA == CurrentUser.Uid || x.ProjectClosure.Uid_TL == CurrentUser.Uid) &&
                                        x.ProjectClosure.PMID == PMUserId);
            }

            if (searchFilter.ProjectionData == Enums.ProjectionData.Converted || isConvertedData)
            {
                expr = expr.And(x => x.ProjectClosure.Status == (int)Enums.CloserType.Converted);
            }
            // searchFilter.IsForcastOccupancy for past occupancy
            else if (searchFilter.ProjectionData == Enums.ProjectionData.PendingAndConverted || !searchFilter.IsForcastOccupancy)
            {
                expr = expr.And(x => x.ProjectClosure.Status == (int)Enums.CloserType.Converted || x.ProjectClosure.Status == (int)Enums.CloserType.Pending);
            }
            else
            {
                expr = expr.And(x => x.ProjectClosure.Status == (int)Enums.CloserType.Pending);
            }



            //if (isConverted)
            //{
            //    expr = expr.And(x => x.ProjectClosure.Status == (int)Enums.CloserType.Converted);
            //}
            //else if(pastOccupancy)
            //{
            //    expr = expr.And(x => x.ProjectClosure.Status == (int)Enums.CloserType.Converted || x.ProjectClosure.Status == (int)Enums.CloserType.Pending);
            //}
            //else
            //{
            //    expr = expr.And(x => x.ProjectClosure.Status == (int)Enums.CloserType.Pending);
            //}

            return expr;
        }
        /// <summary>
        /// find running developers
        /// </summary>
        /// <returns>returns running developers</returns>
        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult RunningDevelopers(ProjectClosureSearchFilter searchFilter)
        {            
            var bucketSystem = Common.GetExcludedBucketModelIds();
            var expr = PredicateBuilder.True<ProjectDeveloper>()
            .And(x => x.Project.Status == "R" && x.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running &&
                                            !x.Project.IsInHouse && x.Project.Model.HasValue && !bucketSystem.Contains(x.Project.Model.Value));

            if (CurrentUser.RoleId == (int)Enums.UserRoles.Director || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM)
            {
                // All project would display to director and UKBDM
                if (searchFilter.PMUid.HasValue)
                {
                    expr = expr.And(x => x.Project.PMUid == searchFilter.PMUid.Value);
                }
            }
            else if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
            {
                expr = expr.And(x => x.Project.PMUid == CurrentUser.Uid);
            }
            else if (
                RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)            
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
                //|| RoleValidator.DV_RoleIds.Contains(CurrentUser.RoleId)
                || RoleValidator.BA_RoleIds.Contains(CurrentUser.RoleId))
            {
                expr = expr.And(x => x.Project.PMUid == CurrentUser.PMUid);
            }
            else
            {
                expr = expr.And(x => x.Project.PMUid == CurrentUser.PMUid);
            }
            int runningDevCount = projectClosureReviewService.GetRunningDevelopersCount(expr);
            return new JsonResult(new
            {
                RunningDevelopers = runningDevCount
            });
        }
        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult PrepareReport(DateTime date, ProjectClosureSearchFilter searchFilter)
        {
            List<ProjectionWeek> weeks = new List<ProjectionWeek>();
            CultureInfo culture = new CultureInfo("en-IN");
            var initialDate = date;
            for (int i = 1; i < 13; i++)
            {
                ProjectionWeek week = new ProjectionWeek();

                week.StartDate = Common.GetStartDateOfWeek(initialDate, culture);
                week.Month = week.StartDate.Month;
                //week.WeekNo = Common.GetWeekOfMonth(week.StartDate, culture);
                week.WeekNo = i;
                week.EndDate = Common.GetEndDateOfWeek(initialDate, culture);
                week.StartDateEndDate = week.StartDate.ToString("dd MMM") + "-" + week.EndDate.ToString("dd MMM");
                week.DeveloperCount = projectClosureReviewService.GetForcastOccupancyForWeek(GetProjectionReportFilterExperession(week, searchFilter));
                if (searchFilter.IsForcastOccupancy)
                {
                    // we don't need converted count in this condition
                }
                else // it's past occupancy so need conversion
                {
                    week.ConvertedCount = projectClosureReviewService
                        .GetConvertedClosureCount(GetProjectionReportFilterExperession(week, searchFilter, isConvertedData: true)); // needs converted count
                }

                weeks.Add(week);
                initialDate = initialDate.AddDays(7);
            }

            // First week record 
            weeks[0].StartDate = date < DateTime.Today ? weeks[0].StartDate : DateTime.Today;
            weeks[0].StartDateEndDate = weeks[0].StartDate.ToString("dd MMM") + "-" +
            weeks[0].EndDate.ToString("dd MMM");

            return new JsonResult(weeks);
        }

        #region "COVID 19 report page"
        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult ReportCOVID19()
        {
            var model = new ProjectClosureIndexDto();
                       
            if (CurrentUser.RoleId == (int)Enums.UserRoles.Director || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM
                || CurrentUser.RoleId == (int)Enums.UserRoles.PMO)
            {
                model.IsDirector = true;
                var userList = userLoginService.GetUserByRole((int)Enums.UserRoles.PM, true);
                model.PMList = userList.Where(x => x.DeptId != (int)Enums.ProjectDepartment.AccountDepartment &&
                                                   x.DeptId != (int)Enums.ProjectDepartment.HRDepartment &&
                                                   x.DeptId != (int)Enums.ProjectDepartment.Other)
                                       .ToSelectList(x => x.Name, x => x.Uid);
            }
            else
            {
                var userList = userLoginService.GetUsersByPM(PMUserId);

                model.BAList = userList.Where(x => RoleValidator.BA_RoleIds.Contains(x.RoleId.Value) || x.RoleId == (int)Enums.UserRoles.PM)
                                       .ToSelectList(x => x.Name, x => x.Uid);
                model.TLList = userList.Where(x =>
                RoleValidator.TL_Technical_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(x.DesignationId.Value)            
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(x.DesignationId.Value)
                //|| RoleValidator.DV_RoleIds.Contains(x.RoleId.Value)
                )
                                       .ToSelectList(x => x.Name, x => x.Uid);
            }

            model.CRMStatus = WebExtensions.GetSelectList<Enums.CRMStatus>();

            model.DateFrom = DateTime.Today.AddDays(-30).ToFormatDateString("dd/MM/yyyy");
            model.DateTo = DateTime.Today.ToFormatDateString("dd/MM/yyyy");

            return View(model);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult ReportCOVID19(IDataTablesRequest request, ProjectClosureSearchFilter searchFilter, Enums.ProjectClosureFilterType? filterType)
        {
            var pagingServices = new PagingService<ProjectClosure>(request.Start, request.Length);
            searchFilter.chaseStatus = 3;
            var expr = GetClosureCovid19FilterExpersion(searchFilter, reportFilter: true);

            if (CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM)
            { // for UK BDM approval will be hide

                expr = expr.And(x => x.CRMUpdated == true);

            }

            pagingServices.Filter = expr.And(x => x.DateofClosing.HasValue && x.IsCovid19 == true
            && (x.CRMStatus == (int)Enums.CRMStatus.OnHold || x.CRMStatus == (int)Enums.CRMStatus.NotInitiated));


            //Sorting is giving in service call
            //pagingServices.Sort = (o) =>
            //{
            //    return o.OrderBy(c => c.CRMUpdated).ThenByDescending(c => c.DateofClosing.Value);
            //};

            TempData.Put("ProjectClosureReportCovid19Filters", searchFilter);

            int totalCount = 0;
            var response = projectClosureService.GetProjectClosureReportPaging(out totalCount, pagingServices, filterType);


            bool isPMUser = IsPM;
            bool isDirector = IsDirector;
            int currentUserId = CurrentUser.Uid;
            bool allowReview = CurrentUser.Uid == SiteKey.AshishTeamPMUId || IsDirector;

            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                r.Id,
                rowIndex = (index + 1) + (request.Start),
                ClientName = r.Project.Client?.Name ?? string.Empty,
                ProjectName = r.Project.Name,
                r.Project.CRMProjectId,
                ProjectId = r.ProjectID,
                PCountry = r.Country,
                ClosingDate = r.DateofClosing.ToFormatDateString("MMM d, yyyy"),
                //StartEndDate = StartEndDates(r.StartDate, r.EndDate),
                //Invoice = (r.InvoiceDays ?? 0) > 0 ? DaysToWeeks(r.InvoiceDays.Value) : "-",
                //BucketHours = r.BucketHours.HasValue && r.BucketHours > 0.0f ? $"{r.BucketHours} hours" : null,
                //Estimate = (r.EstimateDays ?? 0) > 0 ? DaysToWeeks(r.EstimateDays.Value) : "-",
                BA = r.UserLogin?.Name,
                TL = r.UserLogin3?.Name,
                PM = r.UserLogin1?.Name,
                BaId = r.Uid_BA,
                TlId = r.Uid_TL,
                r.AddedBy,
                ClientQuality = r.ClientQuality ?? 0,

                CRMStatus = r.CRMStatus.HasValue ? ((Enums.CRMStatus?)r.CRMStatus.Value).ToString() : "",
                CreatedDate = r.Created.ToFormatDateString("MMM d, yyyy hh:mm tt"),
                ModityDate = r.Modified.ToFormatDateString("MMM d, yyyy hh:mm tt"),

                PromisingPercentage = r.ProjectClosureReview != null ? ((Enums.ProjectClosureReviewPercentage)r.ProjectClosureReview.PromisingPercentageId).GetDescription() : "",
                Developers = r.ProjectClosureReview?.DeveloperCount,
                r.ProjectClosureReview?.Comments,
                NextStartDate = r.ProjectClosureReview?.NextStartDate.ToFormatDateString("MMM d, yyyy"),

                //HasConverted = r.Status == (int)Enums.CloserType.Converted,
                //HasDeadResponse = r.Status == (int)Enums.CloserType.DeadResponse,
                //IsPending = !r.CRMUpdated,
                //HasReview = r.ProjectClosureReview != null,
                //AllowPendingEdit = !IsDirector,
                //AllowEdit = isPMUser || (r.AddedBy == currentUserId || r.Uid_BA == currentUserId || r.Uid_TL == currentUserId),
                //AllowReview = allowReview && (r.ProjectClosureReview == null || r.ProjectClosureReview.CreateByUid == currentUserId)
            }));
        }

        private Expression<Func<ProjectClosure, bool>> GetClosureCovid19FilterExpersion(ProjectClosureSearchFilter searchFilter, bool isSummaryFilter = false, bool reportFilter = false)
        {
            var expr = PredicateBuilder.True<ProjectClosure>();

            if (reportFilter)
            {
                if (CurrentUser.RoleId == (int)Enums.UserRoles.Director || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM
                    || CurrentUser.RoleId == (int)Enums.UserRoles.PMO
                    )
                {
                    if (searchFilter.PMUid.HasValue)
                    {
                        expr = expr.And(x => x.PMID == searchFilter.PMUid.Value);
                    }
                }
                else if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
                {
                    expr = expr.And(x => x.PMID == PMUserId);
                }
                else
                {
                    expr = expr.And(x => ((x.Uid_Dev == CurrentUser.Uid || x.Uid_BA == CurrentUser.Uid || x.Uid_TL == CurrentUser.Uid) ||
                                             !x.Uid_BA.HasValue || !x.Uid_TL.HasValue) && x.PMID == PMUserId);
                }
            }
            else
            {
                expr = expr.And(x => x.CRMUpdated);

                if (CurrentUser.RoleId == (int)Enums.UserRoles.Director)
                {
                    if (searchFilter.PMUid.HasValue)
                    {
                        expr = expr.And(x => x.PMID == searchFilter.PMUid.Value);
                    }
                }
                else if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
                {
                    expr = expr.And(x => x.PMID == PMUserId);
                }
                else
                {
                    expr = expr.And(x => (x.Uid_Dev == CurrentUser.Uid || x.Uid_BA == CurrentUser.Uid || x.Uid_TL == CurrentUser.Uid || !x.Uid_TL.HasValue || !x.Uid_BA.HasValue) && x.PMID == PMUserId);
                }
            }

            if (searchFilter.textSearch.HasValue())
            {
                searchFilter.textSearch = searchFilter.textSearch.Trim().ToLower();

                expr = expr.And(L =>
                (Convert.ToString(L.Project.ProjectId) == searchFilter.textSearch)
                || L.Project.CRMProjectId.ToString() == searchFilter.textSearch
                || L.Project.Name.ToLower().Contains(searchFilter.textSearch)
                || (L.Project.ClientId.HasValue ? L.Project.Client.Name.ToLower().Contains(searchFilter.textSearch) : false)
                );
            }

            DateTime? startDate = searchFilter.DateFrom.ToDateTime("dd/MM/yyyy");
            DateTime? endDate = searchFilter.DateTo.ToDateTime("dd/MM/yyyy");

            if (startDate.HasValue && endDate.HasValue)
            {
                if (searchFilter.chaseStatus.Value == 1)
                {
                    expr = expr.And(L => L.NextStartDate >= startDate && L.NextStartDate <= endDate.Value);
                }
                else if (searchFilter.chaseStatus.Value == 2)
                {
                    expr = expr.And(L => L.ProjectClosureDetails.Any(x => x.Created >= startDate.Value && x.Created <= endDate.Value));
                }
                else
                {
                    expr = expr.And(L => L.DateofClosing >= startDate.Value && L.DateofClosing <= endDate.Value);
                }
            }
            else if (startDate.HasValue)
            {
                if (searchFilter.chaseStatus.Value == 1)
                {
                    expr = expr.And(L => L.NextStartDate >= startDate);
                }
                else if (searchFilter.chaseStatus.Value == 2)
                {
                    expr = expr.And(L => L.ProjectClosureDetails.Any(x => x.Created >= startDate.Value));
                }
                else
                {
                    expr = expr.And(L => L.DateofClosing >= startDate.Value);
                }
            }
            else if (endDate.HasValue)
            {
                if (searchFilter.chaseStatus.Value == 1)
                {
                    expr = expr.And(L => L.NextStartDate <= endDate.Value);
                }
                else if (searchFilter.chaseStatus.Value == 2)
                {
                    expr = expr.And(L => L.ProjectClosureDetails.Any(x => x.Created <= endDate.Value));
                }
                else
                {
                    expr = expr.And(L => L.DateofClosing <= endDate.Value);
                }
            }

            if ((searchFilter.BA ?? 0) > 0)
            {
                expr = expr.And(l => l.Uid_BA == searchFilter.BA.Value);
            }
            if ((searchFilter.TL ?? 0) > 0)
            {
                expr = expr.And(l => l.Uid_TL == searchFilter.TL.Value);
            }
            if (!isSummaryFilter && searchFilter.CRMStatusId != null && searchFilter.CRMStatusId.Value > 0)
            {
                expr = expr.And(l => l.CRMStatus == searchFilter.CRMStatusId.Value);
            }

            //if (!isSummaryFilter && searchFilter.ProjectStatus > 0)
            //{
            //    int statusId = searchFilter.ProjectStatus;
            //    expr = expr.And(x => x.Status == statusId || x.Status == 0);
            //}

            return expr;
        }

        [HttpGet]
        //[CustomActionAuthorization]
        public ActionResult DownloadReportCovid19Excel()
        {
            var searchFilter = TempData.Get<ProjectClosureSearchFilter>("ProjectClosureReportCovid19Filters");
            if (searchFilter != null)
            {
                var pagingServices = new PagingService<ProjectClosure>(0, int.MaxValue);
                searchFilter.chaseStatus = 3;
                var expr = GetClosureFilterExpersion(searchFilter, reportFilter: true);
                if (CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM)
                {
                    expr = expr.And(x => x.CRMUpdated == true); // UKBDM only approved record is shown
                }

                pagingServices.Filter = expr.And(x => x.DateofClosing.HasValue && x.IsCovid19 == true
                && (x.CRMStatus == (int)Enums.CRMStatus.OnHold || x.CRMStatus == (int)Enums.CRMStatus.NotInitiated));

                //Sorting is giving in service call
                //pagingServices.Sort = (o) =>
                //{
                //    return o.OrderBy(c => c.CRMUpdated).ThenByDescending(c => c.DateofClosing.Value);
                //};

                int totalCount = 0;
                var projectClosures = projectClosureService.GetProjectClosureReportPaging(out totalCount, pagingServices);

                if (projectClosures.Count > 0)
                {
                    var records = projectClosures.Select(r => new
                    {
                        CRMId = r.Project.CRMProjectId,
                        Client = r.Project.Client?.Name ?? " ",
                        Project = r.Project.Name,
                        Status = r.CRMStatus.HasValue ? ((Enums.CRMStatus)r.CRMStatus.Value).GetDescription() : "",
                        ProjectStatus = ((Enums.CloserType)r.Status).GetDescription(),
                        Quality = r.ClientQuality.HasValue ? ((Enums.ClientQualtiy)r.ClientQuality.Value).GetDescription() : "",
                        StartEndDate = StartEndDates(r.StartDate, r.EndDate),
                        DateOfClosing = r.DateofClosing.ToFormatDateString("MMM d, yyyy"),
                        Estimate = (r.EstimateDays ?? 0) > 0 ? DaysToWeeks(r.EstimateDays.Value) : "-",
                        Invoice = (r.InvoiceDays ?? 0) > 0 ? DaysToWeeks(r.InvoiceDays.Value) : "-",
                        BucketHours = r.BucketHours.HasValue && r.BucketHours > 0.0f ? $"{r.BucketHours} hours" : "",
                        BA_Name = r.UserLogin?.Name,
                        TL_Name = r.UserLogin3?.Name,
                        TM_Name = r.UserLogin1?.Name,
                        Technology = string.Join(", ", r.Project.Project_Tech.Select(x => x.Technology.Title).Distinct()),
                        Reason = Common.StripHTMLTagsRegex(HttpUtility.HtmlDecode(r.Reason)),
                        Suggestion = Common.StripHTMLTagsRegex(HttpUtility.HtmlDecode(r.Suggestion))
                    }).ToList();

                    string filename = "ProjectClosureReport_" + DateTime.Now.Ticks.ToString() + ".xlsx";
                    string[] columns = { "CRMId", "Client", "Project", "Status", "ProjectStatus", "Quality", "DateOfClosing", "BucketHours", "BA_Name", "TL_Name", "TM_Name", "Technology", "Reason", "Suggestion" };
                    byte[] filecontent = ExportExcelHelper.ExportExcel(records, "COVID 19 - Project Closure Report", true, columns);

                    return File(filecontent, ExportExcelHelper.ExcelContentType, filename);
                }

                return Content("No record found");
            }

            return Content("Unable to get filters");
        }
        #endregion


    }
}