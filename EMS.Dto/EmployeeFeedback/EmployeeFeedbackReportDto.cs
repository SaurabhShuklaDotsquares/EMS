using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto.EmployeeFeedback
{
    public class EmployeeFeedbackReportDto
    {
        public string EmpNameCode { get; set; }
        public string ProjectManager { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string FeedbackReasons { get; set; }
        public string JoiningDate { get; set; }
        public string LeavingDate { get; set; }
        public string CreatedDate { get; set; }
    }
}
