using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class RoleCategory
    {

        public RoleCategory()
        {
            Role = new HashSet<Role>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<Role> Role { get; set; }
    }
}
