using System.Web;
using EMS.Web.Code.LIBS;
using EMS.Service;
using System;
using EMS.Core;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace EMS.Web.Code.Attributes
{
    public class CustomAuthorization : Attribute, IAuthorizationFilter
    {
        #region "Fields"
        private bool isAshishTeam;
        private byte[] roleTypes;

        #endregion

        public CustomAuthorization()
        {
        }

        public CustomAuthorization(params byte[] roleTypes)
        {
            this.roleTypes = roleTypes;
        }

        public CustomAuthorization(bool IsAshishTeam = false, params byte[] roleTypes) : this(roleTypes)
        {
            isAshishTeam = IsAshishTeam;
        }

        protected virtual CustomPrincipal CurrentUser
        {
            get { return new CustomPrincipal(ContextProvider.HttpContext.User); }
        }

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var controllerInfo = filterContext.ActionDescriptor as ControllerActionDescriptor;
            if (filterContext != null)
            {
                if (!CurrentUser.IsAuthenticated)
                {
                    filterContext.Result = new RedirectResult("~/Login");
                }
                else if (CurrentUser.IsAuthenticated && CurrentUser.RoleId > 0)
                {
                    if (isAshishTeam && CurrentUser.Uid != SiteKey.AshishTeamPMUId && CurrentUser.PMUid != SiteKey.AshishTeamPMUId)
                    {
                        filterContext.StatusCodeResult(StatusCodes.Status403Forbidden);
                    }
                }
            }
        }
    }

    public class CustomActionAuthorization : Attribute, IAuthorizationFilter
    {
        #region "Fields"

        private byte[] roleTypes;
        private bool isSPEGUser;
        private bool isAshishTeam;

        #endregion

        public CustomActionAuthorization()
        {

        }

        public CustomActionAuthorization(params byte[] roleTypes)
        {
            this.roleTypes = roleTypes;
        }

        public CustomActionAuthorization(bool IsSPEGUser = false, bool IsAshishTeam = false, params byte[] roleTypes) : this(roleTypes)
        {
            isSPEGUser = IsSPEGUser;
            isAshishTeam = IsAshishTeam;
        }

        protected virtual CustomPrincipal CurrentUser
        {
            get { return new CustomPrincipal(ContextProvider.HttpContext.User); }
        }

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (CurrentUser.IsAuthenticated && CurrentUser.RoleId > 0)
            {
                string url = $"{ContextProvider.AbsoluteUri}";
                string controllerName = filterContext.RouteData.Values["controller"].ToString();
                string actionName = filterContext.RouteData.Values["action"].ToString();
                int index = url.IndexOf(controllerName);
                string path = (index < 0) ? url : url.Remove(index, url.Length - index);
                url = path + controllerName + "/" + actionName;

                if (!url.ToLower().Contains("accessdenied") && !url.ToLower().Contains("signout") && !url.ToLower().Contains("reportbug.aspx") && !CurrentUser.IsSuperAdmin && !filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    var menuService = (IMenuService)filterContext.HttpContext.RequestServices.GetService(typeof(IMenuService));
                    bool isValid = menuService.CheckCurrentMenu(url, CurrentUser.DesignationId, CurrentUser.Uid);
                    if (!isValid)
                    {
                        filterContext.StatusCodeResult(StatusCodes.Status403Forbidden);
                        return;
                    }
                }

                if (isSPEGUser && CurrentUser.IsSPEG == false && CurrentUser.RoleId != (int)Enums.UserRoles.HRBP)
                {
                    filterContext.StatusCodeResult(StatusCodes.Status403Forbidden);
                    return;
                }

                if (isAshishTeam && CurrentUser.Uid != SiteKey.AshishTeamPMUId && CurrentUser.PMUid != SiteKey.AshishTeamPMUId)
                {
                    filterContext.StatusCodeResult(StatusCodes.Status403Forbidden);
                    return;
                }

                // Employee Activity menu enabled for Ashish team members according to role permission but in case of other PMs team only TL and PM can access it
                if (!(CurrentUser.PMUid == Convert.ToInt32(SiteKey.AshishTeamPMUId) || (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
                    || RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_Sales_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_HR_DesignationIds.Contains(CurrentUser.DesignationId)
                    || (CurrentUser.RoleId == (int)Enums.UserRoles.Director) || (CurrentUser.RoleId == (int)Enums.UserRoles.HRBP)))
                {
                    if (url.ToLower().Contains("activity/index"))
                    {
                        filterContext.StatusCodeResult(StatusCodes.Status403Forbidden);
                        return;
                    }
                }
            }
            else
            {
                filterContext.Result = new RedirectResult("~/Login/index/?returnurl=" + ContextProvider.AbsoluteUri);
            }
        }
    }
}