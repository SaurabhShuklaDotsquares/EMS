using EMS.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service
{
    public interface ITechnologyParentService : IDisposable
    {
        List<TechnologyParent> GetTechnologyParentList();

        List<SelectListItem> GetTechnologyParentDropdown();
    }
}
