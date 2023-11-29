using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Web.Models.Calendar
{
    public class CalendarDay
    {

        public DateTime Date { get; set; }
        public string Day { get; set; }
        public int? LeaveType { get; set; }
        public int LeaveId { get; set; }



        public List<KeyValuePair<string, int>> EmployeeWithLeaveId { get; set; }

        /// <summary>
        /// CalenderDayList : 
        /// 1 string    :   Employee Name
        /// 2 int       :   LeaveId
        /// 3 int       :   LeaveType (Normal[15] / Urgent[16])
        /// 4 int       :   LeaveStatus (Pending[6] / Approved[7] / Cancelled[8] / UnApproved[9])
        /// </summary>
        public List<Tuple<string, int, int, int>> CalenderDayList { get; set; }
        /// <summary>
        /// created 19/12/2018 for adjust leave hour
        /// 1 string    :   Employee Name
        /// 2.int       :   LateHoureId
        /// 3.string    :   LateHour
        /// 4.string    :   EarlyLeave
        /// </summary>
        public List<Tuple<string, int, string, string,string>> CalenderDayAdjustHourList { get; set; }
    }
}