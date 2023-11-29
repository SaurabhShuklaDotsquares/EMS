using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EscalationDocument
    {
        public int Id { get; set; }
        public string DocumentPath { get; set; }
        public DateTime AddedDate { get; set; }
        public int? EscalationId { get; set; }

        public virtual Escalation Escalation { get; set; }
    }
}
