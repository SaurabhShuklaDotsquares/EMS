using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LeaveTypesMaster
    {
        public LeaveTypesMaster()
        {
            LeaveActivity = new HashSet<LeaveActivity>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<LeaveActivity> LeaveActivity { get; set; }
    }
}
