using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class WfhtypesMaster
    {
        public WfhtypesMaster()
        {
            Wfhactivity = new HashSet<Wfhactivity>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<Wfhactivity> Wfhactivity { get; set; }
    }
}
