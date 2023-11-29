using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class TaskComment
    {
        public TaskComment()
        {
            MomMeetingTaskTimeLine = new HashSet<MomMeetingTaskTimeLine>();
        }

        public decimal TaskCommentID { get; set; }
        public decimal TaskId { get; set; }
        public string Comment { get; set; }
        public int TaskStatusID { get; set; }
        public DateTime AddedDate { get; set; }
        public int AddedUid { get; set; }

        public virtual UserLogin UserLogin { get; set; }
        public virtual Task Task { get; set; }
        public virtual TaskStatu TaskStatus { get; set; }

        public virtual ICollection<MomMeetingTaskTimeLine> MomMeetingTaskTimeLine { get; set; }
    }
}
