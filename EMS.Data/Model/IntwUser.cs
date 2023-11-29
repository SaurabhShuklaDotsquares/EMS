using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class IntwUser
    {
        public IntwUser()
        {
            IntwUserAnswer = new HashSet<IntwUserAnswer>();
            IntwUserQues = new HashSet<IntwUserQues>();
            IntwUserSession = new HashSet<IntwUserSession>();
        }

        public int IntwUserId { get; set; }
        public int? IntwTechnologyId { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public int? TotalAttempt { get; set; }
        public string UserResume { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? Modifydate { get; set; }

        public virtual IntwTechnology IntwTechnology { get; set; }
        public virtual ICollection<IntwUserAnswer> IntwUserAnswer { get; set; }
        public virtual ICollection<IntwUserQues> IntwUserQues { get; set; }
        public virtual ICollection<IntwUserSession> IntwUserSession { get; set; }
    }
}
