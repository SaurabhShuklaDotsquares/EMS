using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class MomMeetingTaskParticipant
    {
        public int MomMeetingTaskId { get; set; }
        public int Uid { get; set; }

        public virtual MomMeetingTask MomMeetingTask { get; set; }
        public virtual UserLogin U { get; set; }
    }
}
