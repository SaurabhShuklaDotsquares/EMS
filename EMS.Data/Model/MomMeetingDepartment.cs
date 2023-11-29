using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class MomMeetingDepartment
    {
        public int MomMeetingId { get; set; }
        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual MomMeeting MomMeeting { get; set; }
    }
}
