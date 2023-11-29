using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Dto
{
    public class CurrentOpeningDto
    {
        public int Id { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string Post { get; set; }
        public string Technology { get; set; }
        public string Min_Experience { get; set; }
        public string Small_Description { get; set; }
        public bool IsActive { get; set; }
    }
}
