using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class BloodGroup
    {
        public BloodGroup()
        {
            UserLogin = new HashSet<UserLogin>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<UserLogin> UserLogin { get; set; }
    }
}
