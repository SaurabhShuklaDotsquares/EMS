using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ExamQuestionAnswerDetail
    {
        public int Qaid { get; set; }
        public int? QuestId { get; set; }
        public string Answer { get; set; }
        public bool? IsCorrect { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string Ip { get; set; }

        public virtual ExamQuestion Quest { get; set; }
    }
}
