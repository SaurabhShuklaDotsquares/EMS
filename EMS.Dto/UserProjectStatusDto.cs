using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Dto
{
    public class UserProjectStatusDto
    {
        public string UserName { get; set; }
        public string UserStatus { get; set; }
        public string ProjectName { get; set; }
        public string ProjectId { get; set; }
        public string ProjectIdAdditional { get; set; }
        public int ActivityId { get; set; }
        public string FreeText { get; set; }

        public List<string> ManagingProjects { get; set; }
    }
}
