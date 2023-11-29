using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class EmployeeWorkingHourReportDto
    {
        public string EmployeeName { get; set; }
        public string EmployeeEmail { get; set; }
        public string MobileNo { get; set; }
        public string ReportingTo { get; set; }
        public string ProjectName { get; set; }
        public string TaskName { get; set; }
        public string PlanHour { get; set; }
        public string ActualHours { get; set; }
        public string HTML { get; set; }
    }
}
