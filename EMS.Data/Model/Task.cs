using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Task
    {
        public Task()
        {
            TaskAssignedToes = new HashSet<TaskAssignedTo>();
            TaskComments = new HashSet<TaskComment>();
        }

        public decimal TaskID { get; set; }
        public string TaskName { get; set; }
        public short? Priority { get; set; }
        public string Remark { get; set; }
        public int TaskStatusID { get; set; }
        public int AddedUid { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public DateTime? TaskEndDate { get; set; }  
        public bool ReminderEmailSent { get; set; }
        public int? MomMeetingTaskId { get; set; }

        public virtual UserLogin UserLogin { get; set; }
        public virtual MomMeetingTask MomMeetingTask { get; set; }
        public virtual TaskStatu TaskStatu { get; set; }
        public virtual ICollection<TaskAssignedTo> TaskAssignedToes { get; set; }
        public virtual ICollection<TaskComment> TaskComments { get; set; }
    }
}
