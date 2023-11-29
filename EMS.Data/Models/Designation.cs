using System;
using System.Collections.Generic;

namespace EMS.Data.Models
{
    public partial class Designation
    {
        public Designation()
        {
            UserLogin = new HashSet<UserLogin>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int RoleId { get; set; }
        public bool? IsActive { get; set; }
        public string Experience { get; set; }

        public virtual ICollection<UserLogin> UserLogin { get; set; }
    }
}
