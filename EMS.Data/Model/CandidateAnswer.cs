using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class CandidateAnswer
    {
        public int CanswerId { get; set; }
        public int CquestionId { get; set; }
        public string Canswer { get; set; }

        public virtual ExamQuestion Cquestion { get; set; }
    }
}
