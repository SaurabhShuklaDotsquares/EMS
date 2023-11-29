using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Dto
{
    public class OfficialLeaveDto
    {
        public int LeaveId { get; set; }
        public string Title { get; set; }
        public System.DateTime LeaveDate { get; set; }
        public byte CountryId { get; set; }
        public bool IsActive { get; set; }
        public string LeaveType { get; set; }
    }
}
