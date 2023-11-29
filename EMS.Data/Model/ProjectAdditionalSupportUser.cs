using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectAdditionalSupportUser
    {
        public int AssignedUid { get; set; }
        public int ProjectAdditionalSupportId { get; set; }

        public virtual ProjectAdditionalSupport ProjectAdditionalSupport { get; set; }
        public virtual UserLogin AssignedU { get; set; }
    }
}
