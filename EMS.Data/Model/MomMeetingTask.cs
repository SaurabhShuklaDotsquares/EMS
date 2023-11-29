using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class MomMeetingTask
    {
        public MomMeetingTask()
        {
            MomMeetingTaskParticipant = new HashSet<MomMeetingTaskParticipant>();
            MomMeetingTaskTimeLine = new HashSet<MomMeetingTaskTimeLine>();
            TaskNavigation = new HashSet<Task>();
            MomMeetingTaskDocuments = new HashSet<MomMeetingTaskDocument>();
        }

        public int Id { get; set; }
        public int MomMeetingId { get; set; }
        public string Task { get; set; }
        public byte Status { get; set; }
        public string Remark { get; set; }
        public DateTime? TargetDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Decision { get; set; }

        public short? Priority { get; set; }

        public virtual MomMeeting MomMeeting { get; set; }
        public virtual ICollection<MomMeetingTaskParticipant> MomMeetingTaskParticipant { get; set; }
        public virtual ICollection<MomMeetingTaskTimeLine> MomMeetingTaskTimeLine { get; set; }
        public virtual ICollection<Task> TaskNavigation { get; set; }

        public virtual ICollection<MomMeetingTaskDocument> MomMeetingTaskDocuments { get; set; }
        public virtual PILog Pilog { get; set; }
    }
}
