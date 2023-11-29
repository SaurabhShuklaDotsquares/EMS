using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class TaskAssignedToBackUpForStatusTest
    {
        public decimal AssignedToTaskId { get; set; }
        public decimal TaskId { get; set; }
        public int AssignUid { get; set; }
        public int? TaskStatusId { get; set; }

        public virtual UserLogin AssignU { get; set; }
        public virtual Task Task { get; set; }
        public virtual TaskStatu TaskStatus { get; set; }
    }
}
