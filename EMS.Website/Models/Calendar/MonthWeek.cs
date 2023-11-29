using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Web.Models.Calendar
{
    public class MonthWeek
    {
        public MonthWeek()
        {
            calendarDays = new List<CalendarDay>();
        }
        public List<CalendarDay> calendarDays { get; set; }
    }
}