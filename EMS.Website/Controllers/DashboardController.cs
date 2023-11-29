using EMS.Data;
using EMS.Web.Code.Attributes;
using EMS.Web.LIBS;
using EMS.Web.Models.Others;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Http;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class DashboardController : BaseController
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorization()]
        [HttpGet]
        public ActionResult Signout()
        {
            RemoveAuthentication();           
            var cookieOptions = new CookieOptions { Expires = DateTime.Now.AddSeconds(1) };
            Response.Cookies.Append("UserSessionCookies", string.Empty, cookieOptions);
            EMS.Web.Code.LIBS.WebsiteSession.RemoveProjectStatusSession();
            //SiteSession.SessionUser = null; // for webforms
            // Response.Cookies["UserSessionCookies"].Expires = System.DateTime.Now.AddSeconds(1); // Clear cookies of SiteSession.SessionUser
            return RedirectToAction("Index", "Login");
        }
    }
}