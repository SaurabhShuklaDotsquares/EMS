using System;
using System.Collections.Generic;

namespace EMS.Data.Models
{
    public partial class Role
    {
        public Role()
        {
            Designation = new HashSet<Designation>();
            UserLogin = new HashSet<UserLogin>();
        }

        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string MenuAccess { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? AddDate { get; set; }
        public int? RoleCategoryId { get; set; }

        public virtual RoleCategory RoleCategory { get; set; }
        public virtual ICollection<Designation> Designation { get; set; }
        public virtual ICollection<UserLogin> UserLogin { get; set; }
    }
}
