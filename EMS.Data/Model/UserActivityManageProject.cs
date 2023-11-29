using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class UserActivityManageProject
    {

        public int Id { get; set; }
        public int ActivityId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectStatus { get; set; }
        public int ProjectRunningDevloper { get; set; }
        public DateTime Created { get; set; }

        public virtual UserActivity Activity { get; set; }
        public virtual Project Project { get; set; }
    }
}
