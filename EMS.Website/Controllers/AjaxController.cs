using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Web.Controllers
{
    public class AjaxController : BaseController
    {
        [HttpPost]
        public byte isLoggedIn()
        {
            if (CurrentUser != null)
                return 1;
            else
                return 0;
        }
    }
}