using EMS.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class EmployeeWorkingHoursSummaryDto
    {
        public List<SelectListItem> WorkingHourTypes { get; set; }
        public List<SelectListItem> Departments { get; set; }
        public List<SelectListItem> Employees { get; set; }
        public string DepartmentId { get; set; }
        public string Uid { get; set; }
        public Enums.WorkingHourSummaryType WorkingHourTypeId { get; set; }

        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
}
