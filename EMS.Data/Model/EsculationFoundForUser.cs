using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EsculationFoundForUser
    {
        public int EsculationId { get; set; }
        public int Uid { get; set; }

        public virtual Escalation Esculation { get; set; }
        public virtual UserLogin U { get; set; }
    }
}
