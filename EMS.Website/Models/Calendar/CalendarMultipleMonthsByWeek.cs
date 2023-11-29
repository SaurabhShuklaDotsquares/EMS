using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Web.Models.Calendar
{
    public class CalendarMultipleMonthsByWeek
    {
        public CalendarMultipleMonthsByWeek()
        {
            CalendarMonths = new List<CalendarMonthWithWeek>();
        }
        public DateTime startDate { get; set; }
        public int PharmacyId { get; set; }
        public List<CalendarMonthWithWeek> CalendarMonths { get; set; }
    }
}