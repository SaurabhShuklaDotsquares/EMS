using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class OnNoticeIndexDto
    {
        public List<SelectListItem> PMList { get; set; }
        public List<SelectListItem> DepartmentList { get; set; }

    }
}
