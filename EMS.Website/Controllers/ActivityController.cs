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
using static EMS.Core.Enums;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class ActivityController : BaseController
    {
        #region Constructor and Fields

        private readonly ITechnologyService technologyService;
        private readonly IProjectService projectService;
        private readonly IDepartmentService departmentService;
        private readonly IUserLoginService userLoginService;
        private readonly IUserActivityService userActivityService;
        private readonly ITypeMasterService typeMasterService;
        private readonly IDomainTypeService domainTypeService;
        public ActivityController(ITechnologyService _technologyService, IProjectService _projectService, IDepartmentService _departmentService,
            IUserLoginService _userLoginService, IUserActivityService _userActivityService, ITypeMasterService _typeMasterService,
            IDomainTypeService _domainTypeService)
        {
            technologyService = _technologyService;
            projectService = _projectService;
            departmentService = _departmentService;
            userLoginService = _userLoginService;
            userActivityService = _userActivityService;
            typeMasterService = _typeMasterService;
            domainTypeService = _domainTypeService;
        }

        #endregion

        // GET: Activity
        [CustomActionAuthorization()]
        public ActionResult Index()
        {
            int pmuId = 0;
            var model = GetActivityModel(ref pmuId); 

            model.Technologies = technologyService.GetTechnologyList().OrderBy(T => T.Title).Select(x => new ActivityCheckboxDto { Id = x.TechId, Name = x.Title }).ToList();
            model.Department = departmentService.GetActiveDepartments().OrderBy(x => x.Name).Select(x => new ActivityCheckboxDto { Id = x.DeptId, Name = x.Name }).ToList();

            ViewBag.TeamSummary = BindCounting(model.DataGrid, pmuId);
            model.SpecTypeList = WebExtensions.GetList<Enums.TechnologySpecializationType>();
            model.DomainExpert = domainTypeService.GetDomainList().OrderBy(x => x.DomainName)
                                 .Select(x => new DomainExpertDto { DomainId = x.DomainId, DomainName = x.DomainName }).ToList();
            //try
            //{
            //    userActivityService.DeleteUserActivityOfLastDay();
            //}
            //catch { }

            return View(model);
        }

        [CustomActionAuthorization()]
        public ActionResult Detail()
        {
            int pmuId = Convert.ToInt32(TempData["filterPMId"]);
            var model = GetActivityModel(ref pmuId);

            ViewBag.TeamSummary = BindDetailCounting(model.DataGrid, pmuId);

            return View(model);
        }

        private ActivityDto GetActivityModel(ref int pmuId)
        {
            ActivityDto model = new ActivityDto();
            
            if (pmuId == 0)
            {
                pmuId = CurrentUser.RoleId == (int)Enums.UserRoles.PM ? CurrentUser.Uid : CurrentUser.PMUid;

                if (CurrentUser.RoleId == (int)Enums.UserRoles.Director || CurrentUser.RoleId == (int)Enums.UserRoles.HRBP)
                {
                    pmuId = 0;
                }
            }

            model.PmFilter = userLoginService.GetPMAndPMOHRUsers(true)
                .Where(x => x.DeptId != (int)Enums.ProjectDepartment.AccountDepartment && x.DeptId != (int)Enums.ProjectDepartment.Other)
                .OrderBy(G => G.Name).Select(x => new SelectListItem { Value = x.Uid.ToString(), Text = x.Name }).ToList();

            if (CurrentUser.RoleId != (int)Enums.UserRoles.HRBP)
            {
                int pmuid = pmuId;
                model.TeamLead = userLoginService.GetUsers1(CurrentUser.PMUid)
                            .OrderBy(T => T.Name)
                            .Where(P => P.PMUid == pmuid)
                            .Select(x => new SelectListItem { Value = x.Uid.ToString(), Text = x.Name })
                            .ToList();
            }

            model.DataGrid = userActivityService.GetActivitiesnew(DateTime.Now.Date, 0, pmuId).OrderBy(x => x.AvailabilityStatusOrder).ToList();
            //model.DataGrid = userActivityService.GetActivities(DateTime.Now.Date, 0, pmuId).OrderBy(x => x.AvailabilityStatusOrder).ToList();
            
            return model;
        }

        private List<TeamOccupancyDto> BindCountingOld(List<ActivityGrid> model, int pmId, List<int> departments = null)
        {            
            var runningProjects = projectService.RunningProjectByPM(pmId, departments);
            var teamSummary = model.Where(a => a.PmRole.HasValue && a.PmRole.Value != (int)UserRoles.Director)
                .GroupBy(a => a.PmUID)
                .Select(g =>
                {
                    var occupancy = new TeamOccupancyDto
                    {
                        TeamManagerName = g.FirstOrDefault().TeamManager,
                        TeamManagerId = g.Key ?? 0,
                        AdditionalSupport = g.Count(x => x.AvailabilityStatus == "Additional Support"),
                        Free = g.Count(x => x.AvailabilityStatus == "Free"),
                        Leave = g.Count(x => x.AvailabilityStatus == "Leave" || x.AvailabilityStatus == "Leave-[Half]"),
                        OverRun = g.Count(x => x.AvailabilityStatus == "Working-Overrun"),
                        Running = g.Count(x => x.AvailabilityStatus == "Running") - (g.Count(x => x.IsBucketProject) + g.Count(x => x.IsSEOProject)),
                        ActualRunning = runningProjects.FirstOrDefault(m => m.PMUid == (g.Key.HasValue ? g.Key.Value : 0))?.TotalDevelopers ?? 0,
                        BonusRunning = g.Sum(x => x.AssignedAddon),
                        UnassignedRunning = runningProjects.FirstOrDefault(m => m.PMUid == (g.Key.HasValue ? g.Key.Value : 0))?.UnassignedActualDevelopers ?? 0,

                        NotLogin = g.Count(x => x.AvailabilityStatus == "Not Logged-In" || x.AvailabilityStatus == "Not Logged-In [Leave Pending]"),
                        OnLeaveButRunning = g.Sum(x => x.OnLeaveButRunning),
                        DesignerInSupport = g.Count(x => x.AvailabilityStatus == "Support Team" && (x.RoleId == (int)UserRoles.UIUXDesigner|| x.RoleId == (int)UserRoles.UIUXDeveloper || x.RoleId == (int)UserRoles.UIUXFrontEndDeveloper || x.RoleId == (int)UserRoles.UIUXManagerial || x.RoleId == (int)UserRoles.UIUXMeanStackDeveloper)),
                        BAInSupport = g.Count(x => x.AvailabilityStatus == "Support Team" && (RoleValidator.BA_RoleIds.Contains(x.RoleId))),
                        QAInSupport = g.Count(x => x.AvailabilityStatus == "Support Team" && (x.RoleId == (int)UserRoles.QA|| x.RoleId == (int)UserRoles.QAManagerial|| x.RoleId == (int)UserRoles.QAPManagerial)),
                        TLInSupport = g.Count(x => x.AvailabilityStatus == "Support Team" && RoleValidator.TL_RoleIds.Contains(x.RoleId)),
                        BucketRunning = g.Count(x => x.IsBucketProject),
                        SEORunning = g.Sum(x => x.SEOProjectRunningDev),
                        NotLoginButRunning = g.Sum(x => x.NotLogInButRunning),

                        TotalEmployee = g.Count(),
                        TotalDotNetEmployee = g.Count(x => x.UserDepartmentID == (int)ProjectDepartment.DotNetDevelopment && x.RoleId != (int)UserRoles.ST),
                        TotalSEOEmployee = g.Count(x => x.UserDepartmentID == (int)ProjectDepartment.SEO && x.RoleId != (int)UserRoles.ST),
                        TotalPHPEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.PHPDevelopment && x.RoleId != (int)UserRoles.ST),
                        TotalMobileEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.MobileApplication && x.RoleId != (int)UserRoles.ST),
                        TotalHubspotEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.HubSpot && x.RoleId != (int)UserRoles.ST),
                        TotalBAEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.BusinessAnalyst && x.RoleId != (int)UserRoles.ST),
                        TotalTraineeEmployee = g.Count(x => x.RoleId == (int)UserRoles.ST),
                        TotalDesignerEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.WebDesigning && x.RoleId != (int)UserRoles.ST),
                        TotalQAEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.QualityAnalyst && x.RoleId != (int)UserRoles.ST),
                        SEOUserIds = string.Join(",", g.Where(x => x.SEOProjectRunningDev > 0).Select(x => x.UserID)),
                        UnassignedProjectIds = string.Join(",", runningProjects.FirstOrDefault(m => m.PMUid == (g.Key.HasValue ? g.Key.Value : 0))?.UnassignedProjectIds ?? new int[] { }),
                        BucketProjectIds = string.Join(",", g.Where(x => x.IsBucketProject).Select(x => x.ProjectID.Value)),
                        BonusRunningDevelopers = Newtonsoft.Json.JsonConvert.SerializeObject(g.Where(x => x.UserID.HasValue && x.AssignedAddon > 0)
                                                             .Select(x => new { DevId = x.UserID.Value, ProjectId = x.ProjectID, AssignedAddon = x.AssignedAddon })),
                    };

                    occupancy.TotalOtherEmployee = occupancy.TotalEmployee - (occupancy.TotalDotNetEmployee + occupancy.TotalSEOEmployee + occupancy.TotalPHPEmployee + occupancy.TotalMobileEmployee + occupancy.TotalBAEmployee + occupancy.TotalTraineeEmployee + occupancy.TotalQAEmployee + occupancy.TotalDesignerEmployee + occupancy.TotalHubspotEmployee);

                    return occupancy;
                })
                .Where(t => t.TotalEmployee > 1)
                .OrderBy(t => t.TeamManagerName)
                .ToList();

            return teamSummary;
        }
        private List<TeamOccupancyDto> BindCounting(List<ActivityGrid> model, int pmId, List<int> departments = null)
        {
            var runningProjects = projectService.RunningProjectByPM(pmId, departments);
            var teamSummary = model.Where(a => a.PmRole.HasValue && a.PmRole.Value != (int)UserRoles.Director)
                .GroupBy(a => a.PmUID)
                .Select(g =>
                {
                    var occupancy = new TeamOccupancyDto
                    {
                        TeamManagerName = g.FirstOrDefault().TeamManager,
                        TeamManagerId = g.Key ?? 0,
                        AdditionalSupport = g.Count(x => x.AvailabilityStatus == "Additional Support"),
                        Free = g.Count(x => x.AvailabilityStatus == "Free"),
                        Leave = g.Count(x => x.AvailabilityStatus == "Leave" || x.AvailabilityStatus == "Leave-[Half]"),
                        OverRun = g.Count(x => x.AvailabilityStatus == "Working-Overrun"),
                        Running = g.Count(x => x.AvailabilityStatus == "Running") - (g.Count(x => x.IsBucketProject) + g.Count(x => x.IsSEOProject)),
                        ActualRunning = runningProjects.FirstOrDefault(m => m.PMUid == (g.Key.HasValue ? g.Key.Value : 0))?.TotalDevelopers ?? 0,
                        BonusRunning = g.Sum(x => x.AssignedAddon),
                        UnassignedRunning = runningProjects.FirstOrDefault(m => m.PMUid == (g.Key.HasValue ? g.Key.Value : 0))?.UnassignedActualDevelopers ?? 0,

                        NotLogin = g.Count(x => x.AvailabilityStatus == "Not Logged-In" || x.AvailabilityStatus == "Not Logged-In [Leave Pending]"),
                        OnLeaveButRunning = g.Sum(x => x.OnLeaveButRunning),
                        DesignerInSupport = g.Count(x => x.AvailabilityStatus == "Support Team" && RoleValidator.AllUIUX_DesignationIds.Contains(x.DesignationId)),
                        BAInSupport = g.Count(x => x.AvailabilityStatus == "Support Team" && RoleValidator.AllSales_DesignationIds.Contains(x.DesignationId)),
                        QAInSupport = g.Count(x => x.AvailabilityStatus == "Support Team" && RoleValidator.AllQualityAnalyst_DesignationIds.Contains(x.DesignationId)),
                        TLInSupport = g.Count(x => x.AvailabilityStatus == "Support Team" && (RoleValidator.TL_Technical_DesignationIds.Contains(x.DesignationId)
                        || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(x.DesignationId)
                        || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(x.DesignationId)
                        || RoleValidator.TL_UIUX_DesignationIds.Contains(x.DesignationId)
                        || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(x.DesignationId)                       
                        || RoleValidator.TL_ITInfra_DesignationIds.Contains(x.DesignationId)) 
                        ),
                        BucketRunning = g.Count(x => x.IsBucketProject),
                        SEORunning = g.Sum(x => x.SEOProjectRunningDev),
                        NotLoginButRunning = g.Sum(x => x.NotLogInButRunning),

                        TotalEmployee = g.Count(),
                        TotalDotNetEmployee = g.Count(x => x.UserDepartmentID == (int)ProjectDepartment.DotNetDevelopment && !RoleValidator.Trainee_DesignationIds.Contains(x.DesignationId)),
                        TotalSEOEmployee = g.Count(x => x.UserDepartmentID == (int)ProjectDepartment.SEO && !RoleValidator.Trainee_DesignationIds.Contains(x.DesignationId)),
                        TotalPHPEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.PHPDevelopment && !RoleValidator.Trainee_DesignationIds.Contains(x.DesignationId)),
                        TotalMobileEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.MobileApplication && !RoleValidator.Trainee_DesignationIds.Contains(x.DesignationId)),
                        TotalHubspotEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.HubSpot && !RoleValidator.Trainee_DesignationIds.Contains(x.DesignationId)),
                        TotalBAEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.BusinessAnalyst && !RoleValidator.Trainee_DesignationIds.Contains(x.DesignationId)),
                        TotalTraineeEmployee = g.Count(x => RoleValidator.Trainee_DesignationIds.Contains(x.DesignationId)),
                        TotalDesignerEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.WebDesigning && !RoleValidator.Trainee_DesignationIds.Contains(x.DesignationId)),
                        TotalQAEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.QualityAnalyst && !RoleValidator.Trainee_DesignationIds.Contains(x.DesignationId)),
                        SEOUserIds = string.Join(",", g.Where(x => x.SEOProjectRunningDev > 0).Select(x => x.UserID)),
                        UnassignedProjectIds = string.Join(",", runningProjects.FirstOrDefault(m => m.PMUid == (g.Key.HasValue ? g.Key.Value : 0))?.UnassignedProjectIds ?? new int[] { }),
                        BucketProjectIds = string.Join(",", g.Where(x => x.IsBucketProject).Select(x => x.ProjectID.Value)),
                        BonusRunningDevelopers = Newtonsoft.Json.JsonConvert.SerializeObject(g.Where(x => x.UserID.HasValue && x.AssignedAddon > 0)
                                                             .Select(x => new { DevId = x.UserID.Value, ProjectId = x.ProjectID, AssignedAddon = x.AssignedAddon })),
                    };

                    occupancy.TotalOtherEmployee = occupancy.TotalEmployee - (occupancy.TotalDotNetEmployee + occupancy.TotalSEOEmployee + occupancy.TotalPHPEmployee + occupancy.TotalMobileEmployee + occupancy.TotalBAEmployee + occupancy.TotalTraineeEmployee + occupancy.TotalQAEmployee + occupancy.TotalDesignerEmployee + occupancy.TotalHubspotEmployee);

                    return occupancy;
                })
                .Where(t => t.TotalEmployee > 1)
                .OrderBy(t => t.TeamManagerName)
                .ToList();

            return teamSummary;
        }
        private List<TeamOccupancyDto> BindDetailCountingOld(List<ActivityGrid> model, int pmId, List<int> departments = null)
        {
            var runningProjects = projectService.RunningProjectByPM(pmId, departments);
            var teamSummary = model.Where(a => a.PmRole.HasValue && a.PmRole.Value != (int)UserRoles.Director)
                .GroupBy(a => a.PmUID)
                .Select(g =>
                {
                    var occupancy = new TeamOccupancyDto
                    {
                        TeamManagerName = g.FirstOrDefault().TeamManager,
                        TeamManagerId = g.Key ?? 0,
                        AdditionalSupport = g.Count(x => x.AvailabilityStatus == "Additional Support"),
                        Free = g.Count(x => x.AvailabilityStatus == "Free"),
                        Leave = g.Count(x => x.AvailabilityStatus == "Leave" || x.AvailabilityStatus == "Leave-[Half]"),
                        OverRun = g.Count(x => x.AvailabilityStatus == "Working-Overrun"),
                        Running = g.Count(x => x.AvailabilityStatus == "Running") - (g.Count(x => x.IsBucketProject) + g.Count(x => x.IsSEOProject)),
                        ActualRunning = runningProjects.FirstOrDefault(m => m.PMUid == (g.Key.HasValue ? g.Key.Value : 0))?.TotalDevelopers ?? 0,
                        BonusRunning = g.Sum(x => x.AssignedAddon),
                        UnassignedRunning = runningProjects.FirstOrDefault(m => m.PMUid == (g.Key.HasValue ? g.Key.Value : 0))?.UnassignedActualDevelopers ?? 0,

                        NotLogin = g.Count(x => x.AvailabilityStatus == "Not Logged-In" || x.AvailabilityStatus == "Not Logged-In [Leave Pending]"),
                        OnLeaveButRunning = g.Sum(x => x.OnLeaveButRunning),
                        DesignerInSupport = g.Count(x => x.AvailabilityStatus == "Support Team" && (x.RoleId == (int)UserRoles.UIUXDesigner || x.RoleId == (int)UserRoles.UIUXDeveloper || x.RoleId == (int)UserRoles.UIUXFrontEndDeveloper || x.RoleId == (int)UserRoles.UIUXManagerial || x.RoleId == (int)UserRoles.UIUXMeanStackDeveloper)),
                        BAInSupport = g.Count(x => x.AvailabilityStatus == "Support Team" && RoleValidator.BA_RoleIds.Contains(x.RoleId)),
                        QAInSupport = g.Count(x => x.AvailabilityStatus == "Support Team" && (x.RoleId == (int)UserRoles.QA || x.RoleId == (int)UserRoles.QAManagerial || x.RoleId == (int)UserRoles.QAPManagerial)),
                        TLInSupport = g.Count(x => x.AvailabilityStatus == "Support Team" && RoleValidator.TL_RoleIds.Contains(x.RoleId)),
                        BucketRunning = g.Count(x => x.IsBucketProject),
                        SEORunning = g.Sum(x => x.SEOProjectRunningDev),
                        NotLoginButRunning = g.Sum(x => x.NotLogInButRunning),

                        TotalEmployee = g.Count(),
                        TotalDotNetEmployee = g.Count(x => x.UserDepartmentID == (int)ProjectDepartment.DotNetDevelopment && x.RoleId != (int)UserRoles.ST),
                        TotalSEOEmployee = g.Count(x => x.UserDepartmentID == (int)ProjectDepartment.SEO && x.RoleId != (int)UserRoles.ST),
                        TotalPHPEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.PHPDevelopment && x.RoleId != (int)UserRoles.ST),
                        TotalMobileEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.MobileApplication && x.RoleId != (int)UserRoles.ST),
                        TotalBAEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.BusinessAnalyst && x.RoleId != (int)UserRoles.ST),
                        TotalTraineeEmployee = g.Count(x => x.RoleId == (int)UserRoles.ST),
                        TotalDesignerEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.WebDesigning && x.RoleId != (int)UserRoles.ST),
                        TotalQAEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.QualityAnalyst && x.RoleId != (int)UserRoles.ST),
                        TotalHubspotEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.HubSpot && x.RoleId != (int)UserRoles.ST),



                    };

                    occupancy.RunningStats = g.Where(x => x.AvailabilityStatus == "Running" && !x.IsBucketProject && !x.IsSEOProject && x.UserDepartmentID.HasValue)
                                                        .GroupBy(x => x.UserDepartmentID.Value)
                                                        .Select(x => new DepartmentOccupancyDto
                                                        {
                                                            DepartmentId = x.Key,
                                                            DepartmentName = ((Enums.ProjectDepartment)x.Key).GetEnumDisplayName(),
                                                            OccupancyCount = x.Count()
                                                        }).ToList();

                    occupancy.FreeStats = g.Where(x => x.AvailabilityStatus == "Free" && x.UserDepartmentID.HasValue)
                                                       .GroupBy(x => x.UserDepartmentID.Value)
                                                       .Select(x => new DepartmentOccupancyDto
                                                       {
                                                           DepartmentId = x.Key,
                                                           DepartmentName = ((Enums.ProjectDepartment)x.Key).GetEnumDisplayName(),
                                                           OccupancyCount = x.Count()
                                                       }).ToList();

                    occupancy.TotalOtherEmployee = occupancy.TotalEmployee - (occupancy.TotalDotNetEmployee + occupancy.TotalSEOEmployee + occupancy.TotalPHPEmployee + occupancy.TotalMobileEmployee + occupancy.TotalBAEmployee + occupancy.TotalTraineeEmployee + occupancy.TotalQAEmployee + occupancy.TotalDesignerEmployee + occupancy.TotalHubspotEmployee);

                    occupancy.TotalEmployeeStats.Add(new DepartmentOccupancyDto
                    {
                        DepartmentId = (int)Enums.ProjectDepartment.DotNetDevelopment,
                        DepartmentName = Enums.ProjectDepartment.DotNetDevelopment.GetEnumDisplayName(),
                        OccupancyCount = occupancy.TotalDotNetEmployee
                    });

                    occupancy.TotalEmployeeStats.Add(new DepartmentOccupancyDto
                    {
                        DepartmentId = (int)Enums.ProjectDepartment.SEO,
                        DepartmentName = Enums.ProjectDepartment.SEO.GetEnumDisplayName(),
                        OccupancyCount = occupancy.TotalSEOEmployee
                    });

                    occupancy.TotalEmployeeStats.Add(new DepartmentOccupancyDto
                    {
                        DepartmentId = (int)Enums.ProjectDepartment.PHPDevelopment,
                        DepartmentName = Enums.ProjectDepartment.PHPDevelopment.GetEnumDisplayName(),
                        OccupancyCount = occupancy.TotalPHPEmployee
                    });

                    occupancy.TotalEmployeeStats.Add(new DepartmentOccupancyDto
                    {
                        DepartmentId = (int)Enums.ProjectDepartment.MobileApplication,
                        DepartmentName = Enums.ProjectDepartment.MobileApplication.GetEnumDisplayName(),
                        OccupancyCount = occupancy.TotalMobileEmployee
                    });

                    occupancy.TotalEmployeeStats.Add(new DepartmentOccupancyDto
                    {
                        DepartmentId = (int)Enums.ProjectDepartment.BusinessAnalyst,
                        DepartmentName = Enums.ProjectDepartment.BusinessAnalyst.GetEnumDisplayName(),
                        OccupancyCount = occupancy.TotalBAEmployee
                    });

                    occupancy.TotalEmployeeStats.Add(new DepartmentOccupancyDto
                    {
                        DepartmentId = (int)Enums.ProjectDepartment.SoftwareTrainee,
                        DepartmentName = Enums.ProjectDepartment.SoftwareTrainee.GetEnumDisplayName(),
                        OccupancyCount = occupancy.TotalTraineeEmployee
                    });

                    occupancy.TotalEmployeeStats.Add(new DepartmentOccupancyDto
                    {
                        DepartmentId = (int)Enums.ProjectDepartment.WebDesigning,
                        DepartmentName = Enums.ProjectDepartment.WebDesigning.GetEnumDisplayName(),
                        OccupancyCount = occupancy.TotalDesignerEmployee
                    });

                    occupancy.TotalEmployeeStats.Add(new DepartmentOccupancyDto
                    {
                        DepartmentId = (int)Enums.ProjectDepartment.QualityAnalyst,
                        DepartmentName = Enums.ProjectDepartment.QualityAnalyst.GetEnumDisplayName(),
                        OccupancyCount = occupancy.TotalQAEmployee
                    });

                    occupancy.TotalEmployeeStats.Add(new DepartmentOccupancyDto
                    {
                        DepartmentId = (int)Enums.ProjectDepartment.Other,
                        DepartmentName = Enums.ProjectDepartment.Other.GetEnumDisplayName(),
                        OccupancyCount = occupancy.TotalOtherEmployee
                    });

                    occupancy.TotalEmployeeStats.Add(new DepartmentOccupancyDto
                    {
                        DepartmentId = (int)Enums.ProjectDepartment.HubSpot,
                        DepartmentName = Enums.ProjectDepartment.HubSpot.GetEnumDisplayName(),
                        OccupancyCount = occupancy.TotalHubspotEmployee
                    });


                    return occupancy;
                })
                .Where(t => t.TotalEmployee > 1)
                .OrderBy(t => t.TeamManagerName)
                .ToList();

            return teamSummary;
        }
        private List<TeamOccupancyDto> BindDetailCounting(List<ActivityGrid> model, int pmId, List<int> departments = null)
        {
            var runningProjects = projectService.RunningProjectByPM(pmId, departments);
            var teamSummary = model.Where(a => a.PmRole.HasValue && a.PmRole.Value != (int)UserRoles.Director)
                .GroupBy(a => a.PmUID)
                .Select(g =>
                {
                    var occupancy = new TeamOccupancyDto
                    {
                        TeamManagerName = g.FirstOrDefault().TeamManager,
                        TeamManagerId = g.Key ?? 0,
                        AdditionalSupport = g.Count(x => x.AvailabilityStatus == "Additional Support"),
                        Free = g.Count(x => x.AvailabilityStatus == "Free"),
                        Leave = g.Count(x => x.AvailabilityStatus == "Leave" || x.AvailabilityStatus == "Leave-[Half]"),
                        OverRun = g.Count(x => x.AvailabilityStatus == "Working-Overrun"),
                        Running = g.Count(x => x.AvailabilityStatus == "Running") - (g.Count(x => x.IsBucketProject) + g.Count(x => x.IsSEOProject)),
                        ActualRunning = runningProjects.FirstOrDefault(m => m.PMUid == (g.Key.HasValue ? g.Key.Value : 0))?.TotalDevelopers ?? 0,
                        BonusRunning = g.Sum(x => x.AssignedAddon),
                        UnassignedRunning = runningProjects.FirstOrDefault(m => m.PMUid == (g.Key.HasValue ? g.Key.Value : 0))?.UnassignedActualDevelopers ?? 0,

                        NotLogin = g.Count(x => x.AvailabilityStatus == "Not Logged-In" || x.AvailabilityStatus == "Not Logged-In [Leave Pending]"),
                        OnLeaveButRunning = g.Sum(x => x.OnLeaveButRunning),
                        DesignerInSupport = g.Count(x => x.AvailabilityStatus == "Support Team" && RoleValidator.AllUIUX_DesignationIds.Contains(x.DesignationId)),
                        BAInSupport = g.Count(x => x.AvailabilityStatus == "Support Team" && RoleValidator.AllSales_DesignationIds.Contains(x.DesignationId)),
                        QAInSupport = g.Count(x => x.AvailabilityStatus == "Support Team" && RoleValidator.AllQualityAnalyst_DesignationIds.Contains(x.DesignationId)),
                        TLInSupport = g.Count(x => x.AvailabilityStatus == "Support Team" && (RoleValidator.TL_Technical_DesignationIds.Contains(x.DesignationId)
                        || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(x.DesignationId)
                        || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(x.DesignationId)
                        || RoleValidator.TL_UIUX_DesignationIds.Contains(x.DesignationId)
                        || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(x.DesignationId)
                        || RoleValidator.TL_ITInfra_DesignationIds.Contains(x.DesignationId))),
                        BucketRunning = g.Count(x => x.IsBucketProject),
                        SEORunning = g.Sum(x => x.SEOProjectRunningDev),
                        NotLoginButRunning = g.Sum(x => x.NotLogInButRunning),

                        TotalEmployee = g.Count(),
                        TotalDotNetEmployee = g.Count(x => x.UserDepartmentID == (int)ProjectDepartment.DotNetDevelopment && !RoleValidator.Trainee_DesignationIds.Contains(x.DesignationId)),
                        TotalSEOEmployee = g.Count(x => x.UserDepartmentID == (int)ProjectDepartment.SEO && !RoleValidator.Trainee_DesignationIds.Contains(x.DesignationId)),
                        TotalPHPEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.PHPDevelopment && !RoleValidator.Trainee_DesignationIds.Contains(x.DesignationId)),
                        TotalMobileEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.MobileApplication && !RoleValidator.Trainee_DesignationIds.Contains(x.DesignationId)),
                        TotalBAEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.BusinessAnalyst && !RoleValidator.Trainee_DesignationIds.Contains(x.DesignationId)),
                        TotalTraineeEmployee = g.Count(x => RoleValidator.Trainee_DesignationIds.Contains(x.DesignationId)),
                        TotalDesignerEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.WebDesigning && !RoleValidator.Trainee_DesignationIds.Contains(x.DesignationId)),
                        TotalQAEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.QualityAnalyst && !RoleValidator.Trainee_DesignationIds.Contains(x.DesignationId)),
                        TotalHubspotEmployee = g.Count(x => x.UserDepartmentID == (int)Enums.ProjectDepartment.HubSpot && !RoleValidator.Trainee_DesignationIds.Contains(x.DesignationId)),



                    };

                    occupancy.RunningStats = g.Where(x => x.AvailabilityStatus == "Running" && !x.IsBucketProject && !x.IsSEOProject && x.UserDepartmentID.HasValue)
                                                        .GroupBy(x => x.UserDepartmentID.Value)
                                                        .Select(x => new DepartmentOccupancyDto
                                                        {
                                                            DepartmentId = x.Key,
                                                            DepartmentName = ((Enums.ProjectDepartment)x.Key).GetEnumDisplayName(),
                                                            OccupancyCount = x.Count()
                                                        }).ToList();

                    occupancy.FreeStats = g.Where(x => x.AvailabilityStatus == "Free" && x.UserDepartmentID.HasValue)
                                                       .GroupBy(x => x.UserDepartmentID.Value)
                                                       .Select(x => new DepartmentOccupancyDto
                                                       {
                                                           DepartmentId = x.Key,
                                                           DepartmentName = ((Enums.ProjectDepartment)x.Key).GetEnumDisplayName(),
                                                           OccupancyCount = x.Count()
                                                       }).ToList();

                    occupancy.TotalOtherEmployee = occupancy.TotalEmployee - (occupancy.TotalDotNetEmployee + occupancy.TotalSEOEmployee + occupancy.TotalPHPEmployee + occupancy.TotalMobileEmployee + occupancy.TotalBAEmployee + occupancy.TotalTraineeEmployee + occupancy.TotalQAEmployee + occupancy.TotalDesignerEmployee + occupancy.TotalHubspotEmployee);

                    occupancy.TotalEmployeeStats.Add(new DepartmentOccupancyDto
                    {
                        DepartmentId = (int)Enums.ProjectDepartment.DotNetDevelopment,
                        DepartmentName = Enums.ProjectDepartment.DotNetDevelopment.GetEnumDisplayName(),
                        OccupancyCount = occupancy.TotalDotNetEmployee
                    });

                    occupancy.TotalEmployeeStats.Add(new DepartmentOccupancyDto
                    {
                        DepartmentId = (int)Enums.ProjectDepartment.SEO,
                        DepartmentName = Enums.ProjectDepartment.SEO.GetEnumDisplayName(),
                        OccupancyCount = occupancy.TotalSEOEmployee
                    });

                    occupancy.TotalEmployeeStats.Add(new DepartmentOccupancyDto
                    {
                        DepartmentId = (int)Enums.ProjectDepartment.PHPDevelopment,
                        DepartmentName = Enums.ProjectDepartment.PHPDevelopment.GetEnumDisplayName(),
                        OccupancyCount = occupancy.TotalPHPEmployee
                    });

                    occupancy.TotalEmployeeStats.Add(new DepartmentOccupancyDto
                    {
                        DepartmentId = (int)Enums.ProjectDepartment.MobileApplication,
                        DepartmentName = Enums.ProjectDepartment.MobileApplication.GetEnumDisplayName(),
                        OccupancyCount = occupancy.TotalMobileEmployee
                    });

                    occupancy.TotalEmployeeStats.Add(new DepartmentOccupancyDto
                    {
                        DepartmentId = (int)Enums.ProjectDepartment.BusinessAnalyst,
                        DepartmentName = Enums.ProjectDepartment.BusinessAnalyst.GetEnumDisplayName(),
                        OccupancyCount = occupancy.TotalBAEmployee
                    });

                    occupancy.TotalEmployeeStats.Add(new DepartmentOccupancyDto
                    {
                        DepartmentId = (int)Enums.ProjectDepartment.SoftwareTrainee,
                        DepartmentName = Enums.ProjectDepartment.SoftwareTrainee.GetEnumDisplayName(),
                        OccupancyCount = occupancy.TotalTraineeEmployee
                    });

                    occupancy.TotalEmployeeStats.Add(new DepartmentOccupancyDto
                    {
                        DepartmentId = (int)Enums.ProjectDepartment.WebDesigning,
                        DepartmentName = Enums.ProjectDepartment.WebDesigning.GetEnumDisplayName(),
                        OccupancyCount = occupancy.TotalDesignerEmployee
                    });

                    occupancy.TotalEmployeeStats.Add(new DepartmentOccupancyDto
                    {
                        DepartmentId = (int)Enums.ProjectDepartment.QualityAnalyst,
                        DepartmentName = Enums.ProjectDepartment.QualityAnalyst.GetEnumDisplayName(),
                        OccupancyCount = occupancy.TotalQAEmployee
                    });

                    occupancy.TotalEmployeeStats.Add(new DepartmentOccupancyDto
                    {
                        DepartmentId = (int)Enums.ProjectDepartment.Other,
                        DepartmentName = Enums.ProjectDepartment.Other.GetEnumDisplayName(),
                        OccupancyCount = occupancy.TotalOtherEmployee
                    });

                    occupancy.TotalEmployeeStats.Add(new DepartmentOccupancyDto
                    {
                        DepartmentId = (int)Enums.ProjectDepartment.HubSpot,
                        DepartmentName = Enums.ProjectDepartment.HubSpot.GetEnumDisplayName(),
                        OccupancyCount = occupancy.TotalHubspotEmployee
                    });


                    return occupancy;
                })
                .Where(t => t.TotalEmployee > 1)
                .OrderBy(t => t.TeamManagerName)
                .ToList();

            return teamSummary;
        }

        public ActionResult GetFilter(ActivitySearch entity)
        {
            var activities = FilterRecord(entity);

            ViewBag.TeamSummary = BindCounting(activities, entity.pmId, entity.department != null ? entity.department.ToList() : new List<int>());
            return PartialView("_ActivityList", activities);
        }

        [HttpPost]
        public ActionResult TechActivityDetails(ActivitySearch model)
        {

            return PartialView("_TechActivityDetails", model);
        }


        [HttpGet]
        public ActionResult GetDetail(int id)
        {
            TempData["filterPMId"] = id;

            if (CurrentUser.RoleId == (int)Enums.UserRoles.Director)
            {
                return RedirectToAction("Detail");
            }
            else
            {
                return RedirectToAction("accessdenied", "error");
            }
        }

        [HttpGet]
        public ActionResult GetDetailByPM(int id)
        {
            if (CurrentUser.RoleId == (int)Enums.UserRoles.Director)
            {
                int pmuId = id;
                var activity = GetActivityModel(ref pmuId);

                var teamSummary = BindDetailCounting(activity.DataGrid, pmuId);

                return PartialView("_ActivityDetailList", teamSummary);
            }
            else
            {
                return RedirectToAction("accessdenied", "error");
            }
        }

        public List<UserLogin> FillRecursive(List<UserLogin> objAllUser, int parentId, int Pmid)
        {

            List<UserLogin> recursiveObjects = new List<UserLogin>();

            if (parentId != 0)
            {
                foreach (var item in objAllUser.Where(x => x.TLId.Equals(parentId)))
                {

                    recursiveObjects.Add(new UserLogin
                    {
                        Uid = item.Uid,
                        Gender = item.Gender,

                        UserName = item.UserName,
                        // Password = item.Password,
                        PasswordKey = item.PasswordKey,
                        //Type = item.Type,
                        Title = item.Title,
                        Name = item.Name,
                        JobTitle = item.JobTitle,
                        User_Tech = item.User_Tech,
                        DeptId = item.DeptId,
                        Department = item.Department,
                        RoleId = item.RoleId,
                        Role = item.Role,
                        TLId = item.TLId,
                        DOB = item.DOB,
                        JoinedDate = item.JoinedDate,
                        AddDate = item.AddDate,
                        IsActive = item.IsActive,
                        ModifyDate = item.ModifyDate,
                        IP = item.IP,
                        //MAC_Address = item.MAC_Address,
                        EmailOffice = item.EmailOffice,
                        EmailPersonal = item.EmailPersonal,
                        MobileNumber = item.MobileNumber,
                        PhoneNumber = item.PhoneNumber,
                        AlternativeNumber = item.AlternativeNumber,
                        Address = item.Address,
                        SkypeId = item.SkypeId,
                        MarraigeDate = item.MarraigeDate,
                        //isnew = item.isnew,
                        PMUid = item.PMUid,
                    });
                    // comment by hari 12-11-2013
                    recursiveObjects.AddRange(FillRecursive(objAllUser, item.Uid, item.Uid));
                }
            }
            else
            {
                if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
                {
                    objAllUser = objAllUser.Where(x => x.PMUid.Equals(CurrentUser.Uid) || x.Uid.Equals(CurrentUser.Uid) || x.RoleId.Equals((int)Enums.UserRoles.HRBP)).ToList();
                }
                else if (CurrentUser.RoleId == (int)Enums.UserRoles.HRBP)
                {
                    objAllUser = objAllUser.Where(x => (Pmid > 0 ? x.PMUid.Equals(Pmid) : true) && x.RoleId != (int)Enums.UserRoles.HRBP || x.Uid.Equals(Pmid)).ToList();
                }
                else
                {
                    objAllUser = objAllUser.Where(x => x.PMUid.Equals(CurrentUser.PMUid)).ToList();
                }

                foreach (var item in objAllUser)
                {
                    List<User_Tech> lsTech = userLoginService.GetUserInfoByID(item.Uid).User_Tech.ToList();
                    string technology = string.Empty;
                    string specialist = string.Empty;
                    if (lsTech.Any())
                    {
                        foreach (var tech in lsTech)
                        {
                            technology += tech.Technology.Title + ",";
                            if (tech.SpecTypeId == (byte)Enums.TechnologySpecializationType.Expert)
                            {
                                specialist += tech.Technology.Title + ",";
                            }
                        }
                    }

                    recursiveObjects.Add(new UserLogin
                    {
                        Uid = item.Uid,
                        Gender = item.Gender,
                        UserName = item.UserName,
                        // Password = item.Password,
                        PasswordKey = item.PasswordKey,
                        //Type = item.Type,
                        Title = item.Title,
                        Name = item.Name,
                        JobTitle = item.JobTitle,
                        User_Tech = item.User_Tech,
                        DeptId = item.DeptId,
                        Department = item.Department,
                        RoleId = item.RoleId,
                        Role = item.Role,
                        TLId = item.TLId,
                        DOB = item.DOB,
                        JoinedDate = item.JoinedDate,
                        AddDate = item.AddDate,
                        IsActive = item.IsActive,
                        ModifyDate = item.ModifyDate,
                        IP = item.IP,
                        EmailOffice = item.EmailOffice,
                        EmailPersonal = item.EmailPersonal,
                        MobileNumber = item.MobileNumber,
                        PhoneNumber = item.PhoneNumber,
                        AlternativeNumber = item.AlternativeNumber,
                        Address = item.Address,
                        SkypeId = item.SkypeId,
                        MarraigeDate = item.MarraigeDate,

                        PMUid = item.PMUid
                    });

                }
            }
            return recursiveObjects;
        }

        public List<int> FillRecursive1(List<UserLogin> objAllUser, int parentId, int Pmid)
        {

            List<int> recursiveObjects = new List<int>();

            if (parentId != 0)
            {
                foreach (var item in objAllUser.Where(x => x.TLId.Equals(parentId)))
                {
                    recursiveObjects.Add(item.Uid);
                    recursiveObjects.AddRange(FillRecursive1(objAllUser, item.Uid, item.Uid));
                }
            }
            else
            {
                if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
                {
                    objAllUser = objAllUser.Where(x => x.PMUid.Equals(CurrentUser.Uid) || x.Uid.Equals(CurrentUser.Uid) || x.RoleId.Equals((int)Enums.UserRoles.HRBP)).ToList();
                }
                else if (CurrentUser.RoleId == (int)Enums.UserRoles.HRBP)
                {
                    objAllUser = objAllUser.Where(x => (Pmid > 0 ? x.PMUid.Equals(Pmid) : true) && x.RoleId != (int)Enums.UserRoles.HRBP || x.Uid.Equals(Pmid)).ToList();
                }
                else
                {
                    objAllUser = objAllUser.Where(x => x.PMUid.Equals(CurrentUser.PMUid)).ToList();
                }

                foreach (var item in objAllUser)
                {
                    List<User_Tech> lsTech = userLoginService.GetUserInfoByID(item.Uid).User_Tech.ToList();
                    string technology = string.Empty;
                    string specialist = string.Empty;

                    if (lsTech.Any())
                    {
                        foreach (var tech in lsTech)
                        {
                            technology += tech.Technology.Title + ",";
                            if (tech.SpecTypeId == (byte)Enums.TechnologySpecializationType.Expert)
                            {
                                specialist += tech.Technology.Title + ",";
                            }
                        }
                    }

                    recursiveObjects.Add(item.Uid);
                }
            }
            return recursiveObjects;
        }

        public ActionResult BindTL(int pmId)
        {           
            var list = userLoginService.GetUsers1(CurrentUser.PMUid).OrderBy(T => T.Name).Where(P => P.PMUid == pmId).Select(x => new SelectListItem { Value = x.Uid.ToString(), Text = x.Name }).ToList();
            return NewtonSoftJsonResult(new { IsSucess = true, Data = list });
        }

        //public ActionResult DeleteUserActivityLog(string fromDateStr, string toDateStr)
        //{
        //    DateTime fromDate = fromDateStr.ToDateTime();
        //    DateTime toDate = toDateStr.ToDateTime();
        //    string message = string.Empty;
        //    bool isSucess = false;
        //    try
        //    {

        //        if (fromDate <= toDate)
        //        {
        //            List<UserActivity> useractivitydata = userActivityService.GetUserActivitiesdataBetweenTwoDate(fromDate.Date, toDate.Date);

        //            if (useractivitydata.Count > 0)
        //            {
        //                foreach (UserActivity ua in useractivitydata)
        //                {
        //                    userActivityService.Delete(ua);
        //                }
        //                message = "User activity has been deleted successfully.";
        //                isSucess = true;
        //            }
        //            else
        //            {
        //                message = " No Records found.";
        //                isSucess = true;
        //            }
        //        }
        //        else
        //        {
        //            message = "Check selected criteria.";
        //            isSucess = false;

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        message = "Check selected criteria.";
        //        isSucess = false;

        //    }

        //    return NewtonSoftJsonResult(new { IsSucess = isSucess, message = message });
        //}
        public ActionResult ExcelExport(ActivitySearch entity)
        {
            var objAct = FilterRecord(entity);
            List<ActivityExcepExpoert> model = objAct.Select(x => new ActivityExcepExpoert
            {
                AvailabilityStatus = x.AvailabilityStatus,
                Name = x.Name,
                ProjectName = !string.IsNullOrEmpty(x.ProjectName) ? x.ProjectName : "-",
                Technology_Expert = x.ExpertExcelTechSpecilization.Count > 0 ? x.ExpertExcelTechSpecilization.ToArray().ToCombinedString() : string.Empty,
                Technology_Intermediate = x.InterExcelTechSpecilization.Count > 0 ? x.InterExcelTechSpecilization.ToArray().ToCombinedString() : string.Empty,
                Technology_Beginner = x.BeignExcelTechSpecilization.Count > 0 ? x.BeignExcelTechSpecilization.ToArray().ToCombinedString() : string.Empty,
                Technology_Interested = x.InteresExcelTechSpecilization.Count > 0 ? x.InteresExcelTechSpecilization.ToArray().ToCombinedString() : string.Empty,
                OtherTechnology = !string.IsNullOrEmpty(x.OtherTechnology) ? x.OtherTechnology : string.Empty,
                DomainExpert = x.DomainExpertName.Length > 0 ? x.DomainExpertName.ToCombinedString() : string.Empty,
                Status = x.AvailabilityStatus,
                Department = x.UserDepartmentName,
                RunningProjects = x.RunningProjects > 0 ? x.RunningProjects.ToString() : string.Empty,
                RunningDevelopers = x.RunningDevelopers > 0 ? x.RunningDevelopers.ToString() : string.Empty,
            }).ToList();

            string filename = "activity" + DateTime.Now.Ticks.ToString() + ".xlsx";
            string[] columns = { "Name", "ProjectName", "Technology_Expert", "Technology_Intermediate", "Technology_Beginner", "Technology_Interested", "OtherTechnology", "DomainExpert", "Status", "AvailabilityStatus" };
            byte[] filecontent = ExportExcelHelper.ActivityExportToExcel(model, "Activity Report", true, columns);
            string fileName = filename;
            return File(filecontent, ExportExcelHelper.ExcelContentType, fileName);
        }
        //public ActionResult ExcelExport(ActivitySearch entity)
        //{
        //    var objAct = FilterRecord(entity);
        //    List<ActivityExcepExpoert> model = objAct.Select(x => new ActivityExcepExpoert
        //    {
        //        AvailabilityStatus = x.AvailabilityStatus,
        //        Name = x.Name,
        //        ProjectName = !string.IsNullOrEmpty(x.ProjectName) ? x.ProjectName : "-",
        //        Technology_Expert = x.ExpertExcelTechSpecilization.Count > 0 ? x.ExpertExcelTechSpecilization.ToArray().ToCombinedString() : string.Empty,
        //        Technology_Intermediate = x.InterExcelTechSpecilization.Count > 0 ? x.InterExcelTechSpecilization.ToArray().ToCombinedString() : string.Empty,
        //        Technology_Beginner = x.BeignExcelTechSpecilization.Count > 0 ? x.BeignExcelTechSpecilization.ToArray().ToCombinedString() : string.Empty,
        //        Technology_Interested = x.InteresExcelTechSpecilization.Count > 0 ? x.InteresExcelTechSpecilization.ToArray().ToCombinedString() : string.Empty,
        //        OtherTechnology = !string.IsNullOrEmpty(x.OtherTechnology) ? x.OtherTechnology : string.Empty,
        //        DomainExpert = x.DomainExpertName.Length > 0 ? x.DomainExpertName.ToCombinedString() : string.Empty,
        //        Status = x.Status,
        //        Department = x.UserDepartmentName,
        //        RunningProjects = x.RunningProjects > 0 ? x.RunningProjects.ToString() : string.Empty,
        //        RunningDevelopers = x.RunningDevelopers > 0 ? x.RunningDevelopers.ToString() : string.Empty,
        //    }).ToList();

        //    string filename = "activity" + DateTime.Now.Ticks.ToString() + ".xlsx";
        //    string[] columns = { "Name", "ProjectName", "Technology_Expert", "Technology_Intermediate", "Technology_Beginner", "Technology_Interested", "OtherTechnology", "DomainExpert", "Status", "AvailabilityStatus" };
        //    byte[] filecontent = ExportExcelHelper.ActivityExportToExcel(model, "Activity Report", true, columns);
        //    string fileName = filename;
        //    return File(filecontent, ExportExcelHelper.ExcelContentType, fileName);
        //}

        private List<ActivityGrid> FilterRecord(ActivitySearch entity)
        {

            int pmuid = PMUserId;
            if (entity.pmId != 0)
            {
                pmuid = entity.pmId;
            }

            DateTime activityDate = entity.activityDate.ToDateTime("dd/MM/yyyy") ?? DateTime.Now.Date;
            
            var objAct = userActivityService.GetActivitiesnew(activityDate, 0, pmuid).OrderBy(x => x.AvailabilityStatusOrder).ToList();

            if (entity.department != null && entity.department.Any())
            {
                objAct = objAct.FindAll(x => entity.department.Contains(x.UserDepartmentID ?? 0));
            }
            
            if (entity.employeesdept != null && entity.employeesdept.Any())
            {
                if(entity.employeesdept.Contains((int)ProjectDepartment.SoftwareTrainee))
                {
                    objAct = objAct.FindAll(x => RoleValidator.Trainee_DesignationIds.Contains(x.DesignationId));
                }
                else if (entity.employeesdept.Contains((int)ProjectDepartment.Other))
                {
                    objAct = objAct.FindAll(x => !RoleValidator.ActivityDepartment.Contains(x.UserDepartmentID ?? 0) && !RoleValidator.Trainee_DesignationIds.Contains(x.DesignationId));
                    //objAct = objAct.FindAll(x => x.RoleId == (int)Enums.UserRoles.OTH);
                }
                else
                {
                    objAct = objAct.FindAll(x => entity.employeesdept.Contains(x.UserDepartmentID ?? 0) && !RoleValidator.Trainee_DesignationIds.Contains(x.DesignationId));
                }
            }
            if (entity.specialist != null && entity.specialist.Any())
            {
                objAct = objAct.FindAll(x => x.Specialisties.Any() && entity.specialist.Any(t => x.Specialisties.Contains(t)));
            }
            if (entity.technologies != null && entity.technologies.Any())
            {
                objAct = objAct.FindAll(x => x.Technologies.Any() && entity.technologies.Any(t => x.Technologies.Contains(t)));
            }

            if (entity.domainexpert != null && entity.domainexpert.Any())
            {
                objAct = objAct.FindAll(x => x.DomainExperts.Any() && entity.domainexpert.Any(t => x.DomainExperts.ToArray().Contains(t)));
            }
            if (entity.specialist != null && entity.specialist.Any() && entity.technologies != null && entity.technologies.Any())
            {
                objAct = objAct.FindAll(x => x.TechnologiesWithSpecility.Any(t => entity.technologies.Contains(t.Key.ToString()) && entity.specialist.Contains(t.Value.ToString())));
            }
            if (!string.IsNullOrEmpty(entity.othertechnology))
            {
                objAct = objAct.FindAll(x => x.OtherTechnology.Replace(" ", "").ToLower().Contains(entity.othertechnology.Replace(" ", "").ToLower()));
            }

            if (entity.Avail != null && entity.Avail.Any())
            {
                List<string> availabilityStatus = new List<string>();
                foreach (var status in entity.Avail)
                {
                    switch (status)
                    {
                        case "Running":
                            availabilityStatus.Add("Running");
                            break;
                        case "Working-Overrun":
                            availabilityStatus.Add("Working-Overrun");
                            break;
                        case "Additional Support":
                            availabilityStatus.Add("Additional Support");
                            break;
                        case "Support Team":
                            availabilityStatus.Add("Support Team");
                            break;
                        case "Free":
                            availabilityStatus.Add("Free");
                            break;
                        case "Not Logged-In":
                            availabilityStatus.Add("Not Logged-In");
                            availabilityStatus.Add("Not Logged-In [Leave Pending]");
                            break;
                        case "Leave":
                            availabilityStatus.Add("Leave");
                            availabilityStatus.Add("Leave-[Half]");
                            break;
                    }
                }

                if (availabilityStatus.Any())
                {
                    objAct = objAct.FindAll(x => availabilityStatus.Contains(x.AvailabilityStatus));
                }
            }
            if(entity.noticePeriod==true)
            {
                objAct = objAct.FindAll(x => x.IsResigned == false).ToList();
            }
            string searchkey = (string.IsNullOrWhiteSpace(entity.search) ? "" : entity.search).ToLower();

            if (!string.IsNullOrWhiteSpace(searchkey))
            {
                objAct = objAct.FindAll(a => (!string.IsNullOrEmpty(a.Name) ? a.Name.ToLower().Contains(searchkey) : false) ||
                        (!string.IsNullOrEmpty(a.ProjectName) ? a.ProjectName.ToLower().Contains(searchkey) : false));
            }

            if (entity.leadId != 0)
            {
                var UIDs = FillRecursive(userLoginService.GetUsers(true), entity.leadId, 0).Select(U => Convert.ToString(U.Uid));
                objAct = UIDs != null ? objAct.FindAll(X => UIDs.Contains(Convert.ToString(X.UserID))) : objAct;
            }

            if (objAct != null)
            {
                objAct = objAct.OrderBy(x => x.AvailabilityStatusOrder).ToList();
            }

            return objAct;
        }

        [HttpPost]
        public ActionResult UnassignedProjects(int pmUid, int[] runningProjects)
        {
            if (pmUid > 0 && runningProjects != null && runningProjects.Length > 0)
            {
                var model = projectService.UnassignedActualDeveloperProjects(pmUid, runningProjects);

                return PartialView("_UnassignedProjectList", model);
            }

            ModelState.AddModelError("pmUid", "Invalid pm id or running project ids");
            return CreateModelStateErrors();
        }

        [HttpPost]
        public ActionResult BucketProjects(int pmUid, int[] runningProjects)
        {
            if (pmUid > 0 && runningProjects != null && runningProjects.Length > 0)
            {
                var model = projectService.BucketBasedProjects(pmUid, runningProjects);

                return PartialView("_BucketProjectList", model);
            }

            ModelState.AddModelError("pmUid", "Invalid pm id or running project ids");
            return CreateModelStateErrors();
        }

        [HttpPost]
        public ActionResult SEOProjects(int pmUid, int[] runningDevelopers)
        {
            if (pmUid > 0 && runningDevelopers != null && runningDevelopers.Length > 0)
            {
                var model = projectService.SEOProjects(pmUid, runningDevelopers);

                return PartialView("_SEOProjectList", model);
            }

            ModelState.AddModelError("pmUid", "Invalid pm id or running project ids");
            return CreateModelStateErrors();
        }

        [HttpPost]
        public ActionResult BonusProjects(int pmUid, List<AssignedDeveloperDto> runningDevelopers)
        {
            if (pmUid > 0 && runningDevelopers != null && runningDevelopers.Count > 0)
            {
                var model = projectService.BonusProjects(pmUid, runningDevelopers);

                return PartialView("_BonusProjectList", model);
            }

            ModelState.AddModelError("pmUid", "Invalid pm id or running project ids");
            return CreateModelStateErrors();
        }

    }
}