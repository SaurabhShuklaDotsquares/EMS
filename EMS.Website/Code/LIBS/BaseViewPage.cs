﻿using Microsoft.AspNetCore.Mvc.Razor;

namespace EMS.Web.Code.LIBS
{
    public abstract class BaseViewPage<TModel> : RazorPage<TModel>
    {
        protected CustomPrincipal CurrentUser => new CustomPrincipal(ContextProvider.HttpContext.User);

        protected object getHtmlAttributes(bool readonl, string cssClass)
        {
            if (readonl)
            {
                return new { @class = cssClass, @readonly = true };
            }
            return new { @class = cssClass };
        }
    }
}