using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.Web.Code
{
    public static class ContextProviderExtensions
    {
        public static void StatusCodeResult(this AuthorizationFilterContext filterContext, int statusCodes)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new StatusCodeResult(statusCodes);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "error",
                    action = "accessdenied"
                }));
            }
        }

        public static bool IsAjaxRequest(this HttpRequest httpRequest)
        {
            return httpRequest.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}