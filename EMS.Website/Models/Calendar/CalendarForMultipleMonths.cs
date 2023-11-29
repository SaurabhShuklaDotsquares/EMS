using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Web.Models.Calendar
{
    public class CalendarForMultipleMonths
    {
        public CalendarForMultipleMonths()
        {
            CalendarMonths = new List<CalendarMonth>();
        }
        public DateTime startDate { get; set; }
        public List<CalendarMonth> CalendarMonths { get; set; }
    }
}