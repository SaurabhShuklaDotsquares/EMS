using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EMS.Data
{
    public partial class VirtualDeveloper
    {
        public VirtualDeveloper()
        {
            ProjectDeveloper = new HashSet<ProjectDeveloper>();
            UserTimeSheet = new HashSet<UserTimeSheet>();
        }
        [Key]
        public int VirtualDeveloper_ID { get; set; }
        public string VirtualDeveloper_Name { get; set; }
        public string SkypeId { get; set; }
        public bool? isactive { get; set; }
        public string emailid { get; set; }
        public bool? Ismain { get; set; }
        public int? PMUid { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime CreationDate { get; set; }
        public int? VirtualDeveloperCategoryId { get; set; }

        public virtual VirtualDeveloperCategory VirtualDeveloperCategory { get; set; }
        public virtual ICollection<ProjectDeveloper> ProjectDeveloper { get; set; }
        public virtual ICollection<UserTimeSheet> UserTimeSheet { get; set; }
    }
}
