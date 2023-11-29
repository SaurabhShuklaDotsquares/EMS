using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class BucketModel
    {
        public BucketModel()
        {
            Project = new HashSet<Project>();
        }

        public int BucketId { get; set; }
        public string ModelName { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public bool? IsActive { get; set; }
        public string IP { get; set; }
        public string ModelCode { get; set; }

        public virtual ICollection<Project> Project { get; set; }
    }
}
