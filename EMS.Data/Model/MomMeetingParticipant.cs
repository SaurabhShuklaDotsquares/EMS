using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class MomMeetingParticipant
    {
        public int MomMeetingId { get; set; }
        public int Uid { get; set; }

        public virtual MomMeeting MomMeeting { get; set; }
        public virtual UserLogin U { get; set; }
    }
}
