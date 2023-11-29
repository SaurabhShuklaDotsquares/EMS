using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Web.Models.Attendance
{
    public class EmployeeAttendanceModel
    {
        [JsonProperty(PropertyName = "Status")]
        public bool Status { get; set; }

        [JsonProperty(PropertyName = "Message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "emp_data")]
        public AttendanceResultModel EmployeeData { get; set; }

        [JsonProperty(PropertyName = "attendance_data")]
        public List<AttendanceModel> AttendanceData { get; set; }

        [JsonProperty(PropertyName = "leave_data")]
        public EmployeeAttendanceLeaveData LeaveData { get; set; }

        [JsonProperty(PropertyName = "month_data")]
        public EmployeeAttendanceMonthData MonthData { get; set; }
    }

    public class AttendanceResultModel
    {
        [JsonProperty(PropertyName = "HrmId")]
        public string HrmId { get; set; }

        [JsonProperty(PropertyName = "Year")]
        public string Year { get; set; }

        [JsonProperty(PropertyName = "Month")]
        public string Month { get; set; }
    }
    public class AttendanceModel
    {
        [JsonProperty(PropertyName = "emp_in")]
        public string InTime { get; set; }

        [JsonProperty(PropertyName = "emp_out")]
        public string OutTime { get; set; }

        [JsonProperty(PropertyName = "status_update")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }

        [JsonProperty(PropertyName = "day")]
        public string Day { get; set; }

        [JsonProperty(PropertyName = "emp_late")]
        public string EmployeeLate { get; set; }

        [JsonProperty(PropertyName = "left_early")]
        public string LeftEarly { get; set; }

        [JsonProperty(PropertyName = "hours_worked")]
        public string HoursWorked { get; set; }
    }

    public class EmployeeAttendanceLeaveData
    {
        [JsonProperty(PropertyName = "total_present")]
        public int TotalPresent { get; set; }

        [JsonProperty(PropertyName = "total_absent")]
        public int TotalAbsent { get; set; }

        [JsonProperty(PropertyName = "total_halfday")]
        public int TotalHalfday { get; set; }

        [JsonProperty(PropertyName = "total_sat")]
        public int TotalSat { get; set; }

        [JsonProperty(PropertyName = "total_sunday")]
        public int TotalSunday { get; set; }
    }

    public class EmployeeAttendanceMonthData
    {
        [JsonProperty(PropertyName = "total_month_sunday")]
        public int TotalSunday { get; set; }

        [JsonProperty(PropertyName = "total_month_saturday")]
        public int TotalSaturday { get; set; }

        [JsonProperty(PropertyName = "total_main_days")]
        public int TotalWorkingDays { get; set; }

        [JsonProperty(PropertyName = "total_month_days")]
        public int TotalDays { get; set; }
    }
}