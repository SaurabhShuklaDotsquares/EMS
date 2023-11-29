using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto.EmployeeFeedback
{
    public class EmpInfoDto
    {
        public int Uid { get; set; }
        public string Name { get; set; }
        public string EmpCode { get; set; }
        public string EmailOffice { get; set; }
        public string PMName { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public bool IsfeedSubmitted { get; set; }
    }
}
