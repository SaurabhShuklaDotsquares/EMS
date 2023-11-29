using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Web.Models
{
    public class LeaveReportViewModel
    {
        public string LeaveType { get; set; }
        public string EmpName { get; set; }
        public string LeaveCatagory { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string ApplyDate { get; set; }
        
    }
}