using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class IntwQuesExp
    {
        public IntwQuesExp()
        {
            IntwUserSession = new HashSet<IntwUserSession>();
        }

        public int IntwQuesExpId { get; set; }
        public decimal? IntwQuesId { get; set; }
        public int? IntwExperienceId { get; set; }
        public bool? Isactive { get; set; }

        public virtual IntwExperience IntwExperience { get; set; }
        public virtual IntwQues IntwQues { get; set; }
        public virtual ICollection<IntwUserSession> IntwUserSession { get; set; }
    }
}
