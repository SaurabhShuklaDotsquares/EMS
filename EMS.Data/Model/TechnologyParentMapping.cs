using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class TechnologyParentMapping
    {
        public int TechnologyParentId { get; set; }
        public int TechnologyId { get; set; }

        public virtual Technology Technology { get; set; }
        public virtual TechnologyParent TechnologyParent { get; set; }
    }
    
}
