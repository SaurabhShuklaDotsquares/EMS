using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace EMS.Dto
{
    public class WorkingHourDto
    {
        public string Month { get; set; }
        public string DeveloperHours { get; set; }
        public string TLHours { get; set; }
        public string DesignerHours { get; set; }
        public string BAHours { get; set; }
        public List<SelectListItem> MonthsList { get; set; }
        public WorkingHourDto()
        {
            MonthsList = new List<SelectListItem>();
        }
    }
}

