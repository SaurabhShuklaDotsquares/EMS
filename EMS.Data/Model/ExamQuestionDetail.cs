using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ExamQuestionDetail
    {
        public int Eqid { get; set; }
        public int ExamId { get; set; }
        public int QuestionId { get; set; }

        public virtual Examination Exam { get; set; }
        public virtual ExamQuestion Question { get; set; }
    }
}
