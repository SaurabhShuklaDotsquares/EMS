using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ExamQuestion
    {
        public ExamQuestion()
        {
            CandidateAnswer = new HashSet<CandidateAnswer>();
            ExamQuestionAnswerDetail = new HashSet<ExamQuestionAnswerDetail>();
            ExamQuestionDetail = new HashSet<ExamQuestionDetail>();
        }

        public int QuestionId { get; set; }
        public string Question { get; set; }
        public int? TechnologyId { get; set; }
        public int QuestionType { get; set; }
        public int QuestionLevel { get; set; }
        public bool? IsActive { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string Ip { get; set; }

        public virtual TypeMaster QuestionLevelNavigation { get; set; }
        public virtual TypeMaster QuestionTypeNavigation { get; set; }
        public virtual Technology Technology { get; set; }
        public virtual ICollection<CandidateAnswer> CandidateAnswer { get; set; }
        public virtual ICollection<ExamQuestionAnswerDetail> ExamQuestionAnswerDetail { get; set; }
        public virtual ICollection<ExamQuestionDetail> ExamQuestionDetail { get; set; }
    }
}
