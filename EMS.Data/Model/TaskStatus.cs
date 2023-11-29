using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EMS.Data
{
    public partial class TaskStatu
    {
        public TaskStatu()
        {
            Task = new HashSet<Task>();
            TaskComment = new HashSet<TaskComment>();
            TaskAssignedTo = new HashSet<TaskAssignedTo>();
        }
        [Key]
        public int TaskStatusId { get; set; }
        public string TaskStatus { get; set; }

        public virtual ICollection<Task> Task { get; set; }
        public virtual ICollection<TaskComment> TaskComment { get; set; }
        public virtual ICollection<TaskAssignedTo> TaskAssignedTo { get; set; }
    }
}
