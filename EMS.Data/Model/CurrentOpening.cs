using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class CurrentOpening
    {
        public CurrentOpening()
        {
            JobReferences = new HashSet<JobReference>();
        }

        public int Id { get; set; }
        public int? DepartmentId { get; set; }
        public string Post { get; set; }
        public string Technology { get; set; }
        public string Min_Experience { get; set; }
        public string Small_Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string Ip { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<JobReference> JobReferences { get; set; }
    }
}
