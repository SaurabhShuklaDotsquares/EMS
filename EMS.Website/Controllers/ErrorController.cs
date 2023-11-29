using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Web.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult accessdenied()
        {
            return View();
        }
        public ActionResult error404()
        {
            return View();
        }
    }
}