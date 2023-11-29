using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class MomMeetingTaskTimeLine
    {
        public int Id { get; set; }
        public int MomMeetingTaskId { get; set; }
        public int MomMeetingId { get; set; }
        public byte Status { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int CommentByUid { get; set; }
        public DateTime? TargetDate { get; set; }
        public short? Priority { get; set; }
        public virtual UserLogin CommentByU { get; set; }
        public virtual MomMeeting MomMeeting { get; set; }
        public virtual MomMeetingTask MomMeetingTask { get; set; }
        public decimal? TaskCommentId { get; set; }

        public virtual TaskComment TaskComment { get; set; }
    }
}
