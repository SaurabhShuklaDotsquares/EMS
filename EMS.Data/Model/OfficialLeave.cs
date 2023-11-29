using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class OfficialLeave
    {
        public int LeaveId { get; set; }
        public string Title { get; set; }
        public DateTime LeaveDate { get; set; }
        public byte CountryId { get; set; }
        public bool IsActive { get; set; }
        public string LeaveType { get; set; }
    }
}
