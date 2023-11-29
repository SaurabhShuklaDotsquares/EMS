using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Web.Models.Calendar
{
    public class CalendarMonth
    {
        public CalendarMonth()
        {
            LocumCalendarDays = new List<CalendarDay>();
        }
        public List<CalendarDay> LocumCalendarDays { get; set; }
        public string MonthName { get; set; }

    }
}