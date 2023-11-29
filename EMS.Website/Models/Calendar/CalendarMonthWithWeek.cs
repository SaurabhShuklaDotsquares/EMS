using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Web.Models.Calendar
{
    public class CalendarMonthWithWeek
    {
        public CalendarMonthWithWeek()
        {
            calendarWeeks = new List<MonthWeek>();
        }
        public List<MonthWeek> calendarWeeks { get; set; }
        public string MonthName { get; set; }

        public bool DisablePrevious { get; set; }
    }
}