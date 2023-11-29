using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Designation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RoleId { get; set; }
        public bool? IsActive { get; set; }
        public bool? AppendWithChild { get; set; }
        public string Experience { get; set; }
        public virtual Role Role { get; set; }
        public int? GroupId { get; set; }
        public int? DisplayOrder { get; set; }
        public int? ParentGroupId { get; set; }

        public virtual ICollection<UserLogin> UserLogin { get; set; }
    }
}
