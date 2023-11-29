using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class UserActivity
    {
        public UserActivity()
        {
            UserActivityManageProject = new HashSet<UserActivityManageProject>();
        }

        public int ActivityID { get; set; }
        public int? Uid { get; set; }
        public DateTime? Date { get; set; }
        public int? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateModify { get; set; }
        public int SubProjectId { get; set; }

        public virtual Project Project { get; set; }
        public virtual UserLogin UserLogin { get; set; }

        public virtual ICollection<UserActivityManageProject> UserActivityManageProject { get; set; }
    }
}
