using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class CadidateExam
    {
        public int CexamId { get; set; }
        public int CandidateId { get; set; }
        public int ExamId { get; set; }
        public DateTime DateOfExam { get; set; }
        public bool IsComplete { get; set; }
        public string Ip { get; set; }

        public virtual Candidate Candidate { get; set; }
        public virtual Examination Exam { get; set; }
    }
}
