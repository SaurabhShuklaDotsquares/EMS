using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace EMS.Dto
{
    public class DeveloperDto
    {
        public int UserId { get; set; }
        public string DeveloperName { get; set; }
        public string ProjectName { get; set; }
        public string ModelName { get; set; }
        public string GroupName { get; set; }
        public string Status { get; set; }
        public int ProjectId { get; set; }
        public string UsersListSameProject { get; set; }
        public string DepartmentName { get; set; }
    }
}
