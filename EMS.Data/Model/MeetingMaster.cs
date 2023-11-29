using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class MeetingMaster
    {
        public MeetingMaster()
        {
            MomMeetings = new HashSet<MomMeeting>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int CreatedByUID { get; set; }

        public virtual UserLogin CreatedByU { get; set; }
        public virtual ICollection<MomMeeting> MomMeetings { get; set; }
    }
}
