using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class DomainExperts
    {
        public int DomainId { get; set; }
        public int Uid { get; set; }

        public virtual UserLogin U { get; set; }
    }
}
