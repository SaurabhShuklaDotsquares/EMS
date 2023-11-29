using EMS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto.EmployeeFeedback
{
    public class EmployeeFeedbackFilterDto
    {
        public int Id { get; set; }
        public int EmpUid { get; set; }
        public int DeptId { get; set; }
        public string Name { get; set; }
        public string EmpCode { get; set; }
        public string PMName { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Reasons { get; set; }
        public string JoiningDate { get; set; }
        public string LeavingDate { get; set; }
        public string CreatedDate { get; set; }
        public string Comment { get; set; }
        public int? EmpPmuid { get; set; }
        public string ReviewLink { get; set; }
        public bool Lfprofile { get; set; }
        public string Suggestion { get; set; }
        public bool EmailSkypePassReset { get; set; }
        public bool? isVoluntryExit { get; set; }
    }
}
