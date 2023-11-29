using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Project_Tech
    {
        public int Id { get; set; }
        public int ProjectID { get; set; }
        public int TechId { get; set; }

        public virtual Project Project { get; set; }
        public virtual Technology Technology { get; set; }
    }
}
