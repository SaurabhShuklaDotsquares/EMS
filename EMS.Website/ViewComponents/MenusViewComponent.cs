using EMS.Core;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.LIBS;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EMS.Core.Enums;

namespace EMS.Website.ViewComponents
{
    public class MenusViewComponent : ViewComponent
    {
        private readonly IMenuService menuService;
        private readonly IProjectService projectService;
        private readonly IProjectClosureService projectClosureService;

        public MenusViewComponent(IMenuService menuService, IProjectService projectService, IProjectClosureService projectClosureService)
        {
            this.menuService = menuService;
            this.projectService = projectService;
            this.projectClosureService = projectClosureService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = new CustomPrincipal(HttpContext.User);
            var pmuserid = 0;
            if (user.IsAuthenticated)
            {
                pmuserid = user.RoleId == (int)UserRoles.PM || user.RoleId == (int)UserRoles.PMO ? user.Uid : user.PMUid;
            }

            var menus = user.IsSuperAdmin ? menuService.GetMenus() : menuService.GetMenusByUID(user.Uid);

            if (!user.IsSuperAdmin)
            {
                if (!(user.PMUid == Convert.ToInt32(SiteKey.AshishTeamPMUId) || (user.RoleId == (int)Enums.UserRoles.PMO)))
                {
                    menus.RemoveAll(x => x.MenuName == "Estimates" || x.PageName.ToLower().Contains("lead") || x.MenuName == "Estimate Documents"
                    || x.MenuName == "Forecast Occupancy" || x.PageName.ToLower().Contains("librarymanagement") || x.MenuName.ToLower().Contains("working hour variance details") || x.MenuName.ToLower().Contains("working hour variance summary")
                     || x.MenuName.ToLower().Contains("lessons learnt ashish team") || x.MenuName.ToLower().Contains("estimate calculator (library)"));
                }

                if (user.RoleId == (int)UserRoles.PM)
                {
                    menus.RemoveAll(x => x.PageName.ToLower().Contains("librarydownloadpermission"));
                }

                // Employee Activity menu enabled for Ashish team members according to role permission but in case of other PMs team only TL and PM can access it
                if (!(user.PMUid == Convert.ToInt32(SiteKey.AshishTeamPMUId) || (user.RoleId == (int)Enums.UserRoles.PM)
                    || RoleValidator.TL_Technical_DesignationIds.Contains(user.DesignationId)
                    || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(user.DesignationId)
                    || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(user.DesignationId)
                    || RoleValidator.TL_UIUX_DesignationIds.Contains(user.DesignationId)
                    || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(user.DesignationId)
                    || RoleValidator.TL_ITInfra_DesignationIds.Contains(user.DesignationId)
                    || RoleValidator.TL_Sales_DesignationIds.Contains(user.DesignationId)
                    || (user.RoleId == (int)Enums.UserRoles.Director) || (user.RoleId == (int)Enums.UserRoles.HRBP)))
                {
                    menus.RemoveAll(x => x.MenuName == "Activity");
                }

                if (user.PMUid != Convert.ToInt32(SiteKey.AshishTeamPMUId))
                {
                    menus.RemoveAll(x => x.MenuName == "Team Status Report");
                }
                if (user.PMUid != Convert.ToInt32(SiteKey.AshishTeamPMUId))
                {
                    menus.RemoveAll(x => x.MenuName == "SME");
                }
                if (user.RoleId == (int)Enums.UserRoles.PMO)
                {
                    menus.RemoveAll(x => x.MenuName == "Estimates" || x.MenuName == "Employee Activity" || x.MenuName == "Lead Clients" || x.MenuName == "Lead Report" || x.MenuName == "Lead Status" || x.MenuName == "Forecast Occupancy");
                }

                if (user.IsSPEG == false && user.RoleId != (int)Enums.UserRoles.HRBP)
                {
                    menus.RemoveAll(x => x.MenuName == "CI Document Upload");
                }

                if (user.RoleId == (int)Enums.UserRoles.PM && user.DeptId != (int)Enums.ProjectDepartment.AccountDepartment)
                {
                    menus.RemoveAll(x => x.MenuName == "Employees for PF Account");
                }


                menus.RemoveAll(x => x.MenuName == "Manage Roles");
                var menuList = menuService.GetMenus().Where(x => x.UserIds != null).ToList();
                if (menuList.Count > 0)
                {
                    var userAccessMenuList = menuList.Where(x => x.UserIds.Split(",").Contains(user.Uid.ToString())).ToList();
                    if (userAccessMenuList.Count > 0)
                    {
                        foreach (var m in userAccessMenuList)
                        {
                            var isExists = menus.Where(x => x.MenuId == m.MenuId).ToList();
                            if (isExists.Count == 0)
                            {
                                menus.Add(m);
                            }
                        }
                    }
                }
            }

            var menusList = new List<FrontMenuItemDto>();

            foreach (var menu in menus)
            {
                var menuItem = new FrontMenuItemDto
                {
                    MenuId = menu.MenuId,
                    ParentId = menu.ParentId,
                    MenuDisplayName = menu.MenuDisplayName,
                    NotificationFor = menu.NotificationFor,
                    PageName = menu.PageName,
                };

                if (menu.NotificationFor.HasValue && menu.NotificationFor.Value == (byte)Enums.NotificationFor.AdditionalSupport &&
                    (RoleValidator.TL_Technical_DesignationIds.Contains(user.DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(user.DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(user.DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(user.DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(user.DesignationId)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(user.DesignationId)
                    || user.RoleId == (int)Enums.UserRoles.PM ||
                     RoleValidator.BA_RoleIds.Contains(user.RoleId)
                     //|| RoleValidator.DV_RoleIds.Contains(user.RoleId)
                     ))
                {
                    menuItem.NotificationCount = projectService.GetPendingAdditionalSupportCount(pmuserid, user.RoleId == (int)Enums.UserRoles.PM ? 0 : user.Uid);
                }
                else if (menu.NotificationFor.HasValue && menu.NotificationFor.Value == (byte)Enums.NotificationFor.ProjectClosure &&
                    (RoleValidator.TL_Technical_DesignationIds.Contains(user.DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(user.DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(user.DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(user.DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(user.DesignationId)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(user.DesignationId)
                    || user.RoleId == (int)Enums.UserRoles.PM || RoleValidator.BA_RoleIds.Contains(user.RoleId)
                    //|| RoleValidator.DV_RoleIds.Contains(user.RoleId)
                    ))
                {
                    menuItem.NotificationCount = projectClosureService.GetUnapprovedClosureCount(pmuserid, user.RoleId == (int)Enums.UserRoles.PM ? 0 : user.Uid);
                }

                menusList.Add(menuItem);
            }

            return View("~/Views/Shared/_Menu.cshtml", menusList);
        }
    }
}