using EMS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class TLEmployeeDto
    {
        public int Uid { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeEmail { get; set; }
        public string MobileNumber { get; set; }
        public int? TLId { get; set; }
        public string TLEmail { get; set; }
        public string TLName { get; set; }
        public string DepartmentName { get; set; }
        public string LeaveActivity { get; set; }
        
    }
}
