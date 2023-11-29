using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class IntwUserSession
    {
        public IntwUserSession()
        {
            IntwUserAnswer = new HashSet<IntwUserAnswer>();
            IntwUserQues = new HashSet<IntwUserQues>();
        }

        public int IntwUserSessionId { get; set; }
        public int? IntwUserId { get; set; }
        public int? IntwQuesExpId { get; set; }
        public int? TotalQues { get; set; }
        public int? TotalMarks { get; set; }
        public int? MarksObtained { get; set; }
        public int? TotalTime { get; set; }
        public bool? Result { get; set; }
        public string Status { get; set; }
        public DateTime? AddDate { get; set; }

        public virtual IntwQuesExp IntwQuesExp { get; set; }
        public virtual IntwUser IntwUser { get; set; }
        public virtual ICollection<IntwUserAnswer> IntwUserAnswer { get; set; }
        public virtual ICollection<IntwUserQues> IntwUserQues { get; set; }
    }
}
