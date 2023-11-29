using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Web.Models
{
    public class LateHourReportViewModel
    {
        public string EmpName { get; set; }
        public string StartDate { get; set; }
        public string LeaveType { get; set; }
        public string Reason { get; set; }
    }
}