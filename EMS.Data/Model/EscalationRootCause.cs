using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EscalationRootCause
    {
        public EscalationRootCause()
        {
            Escalation = new HashSet<Escalation>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<Escalation> Escalation { get; set; }
    }
}
