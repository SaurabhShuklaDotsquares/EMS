using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Examination
    {
        public Examination()
        {
            CadidateExam = new HashSet<CadidateExam>();
            ExamQuestionDetail = new HashSet<ExamQuestionDetail>();
        }

        public int ExamId { get; set; }
        public string ExamCode { get; set; }
        public string MaxTime { get; set; }
        public bool? IsActive { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string Ip { get; set; }

        public virtual ICollection<CadidateExam> CadidateExam { get; set; }
        public virtual ICollection<ExamQuestionDetail> ExamQuestionDetail { get; set; }
    }
}
