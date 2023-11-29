using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace EMS.Dto
{
    public class DepartmentDto
    {
        public int? DeptId { get; set; }

        [DisplayName("Department Name*")]
        public string Name { get; set; }

        [DisplayName("Department Code*")]
        public string Deptcode { get; set; }

        [DisplayName("Active")]
        public bool IsActive { get; set; }
    }
}
