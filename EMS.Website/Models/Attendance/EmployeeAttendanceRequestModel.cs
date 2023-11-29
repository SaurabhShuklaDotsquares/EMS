using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Web.Models.Attendance
{
    public class EmployeeAttendanceRequestModel
    {
        public string ActionType { get { return "getattendance"; } }
        public string EmsId { get; set; }
        public int HrmId { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string EmailId { get; set; }
        public string Page { get; set; }
    }
}