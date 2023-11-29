using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectAuditPA
    {
        public ProjectAuditPA()
        {
            ProjectNclog = new HashSet<ProjectNCLog>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }

        public virtual ICollection<ProjectNCLog> ProjectNclog { get; set; }
    }
}
