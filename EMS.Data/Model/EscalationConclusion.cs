using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EscalationConclusion
    {
        public int Id { get; set; }
        public int AddedByUid { get; set; }
        public string LessonLearnExplanation { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string Resolution { get; set; }
        public int EscalationId { get; set; }

        public virtual Escalation Escalation { get; set; }
        public virtual UserLogin AddedByU { get; set; }
    }
}
