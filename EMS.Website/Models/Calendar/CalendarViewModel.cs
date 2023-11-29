using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Web.Models.Calendar
{
    public class PharmacyCalendarViewModel
    {
        public PharmacyCalendarViewModel()
        {
            CalendarViews = new List<CalendarViewModel>();
            HeaderCalendarDates = new List<EmployeeCalendarDate>();
        }
        public List<CalendarViewModel> CalendarViews { get; set; }
        public string MonthName { get; set; }

        public List<EmployeeCalendarDate> HeaderCalendarDates { get; set; }
    }

    public class CalendarViewModel
    {
        public CalendarViewModel()
        {
            EmployeeCalendarDates = new List<EmployeeCalendarDate>();
            CalendarEmployees = new List<CalendarEmployee>();
        }
        public string Month { get; set; }
        public string TeamName { get; set; }
        public List<EmployeeCalendarDate> EmployeeCalendarDates { get; set; }        
        public List<CalendarEmployee> CalendarEmployees { get; set; }
        public bool LoadBaseCss { get; set; }
    }

    public class CalendarDatesModel
    {
        public bool Leave { get; set; }
        public bool IsSunday { get; set; }
        public bool IsPlanned { get; set; }
        public bool IsHalfDay { get; set; }
        public TimeSpan StartTime { get; set; }
        //public bool StartHalfDay { get; set; }
        //public bool EndHalfDay { get; set; }
        //public bool StartHalfDayType { get; set; }
        //public bool EndHalfDayType { get; set; }
        public bool IsScheduleCover { get; set; }       
        public string CalendarDate { get; set; }
        public byte LeaveStatus{ get; set; }
        public bool IsLocum { get; set; }
        public bool IsPublicHoliday { get; set; }
        public bool IsDayOff { get; set; }
        public TimeSpan OverTimeStart { get; set; }
        public TimeSpan OverTimeEnd { get; set; }
        public bool IsOverTime { get; set; }
        public byte Status { get; set; }
    }

    public class CalendarEmployee
    {
        public CalendarEmployee()
        {
            EmployeeCalendarDates = new List<EmployeeCalendarDate>();
        }
        public string Name { get; set; }
        public bool IsSelf { get; set; }
        public string JobTitle { get; set; }
        public List<EmployeeCalendarDate> EmployeeCalendarDates { get; set; }
    }

    public class EmployeeCalendarDate
    {
        public string Day { get; set; }
        public int DateDay { get; set; }
        public string LeaveCssClass { get; set; }
        public string HeaderCssClass { get; set; }
        public string LeaveInfoToolTip { get; set; }
    }
}
