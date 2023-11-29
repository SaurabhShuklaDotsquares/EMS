using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class IntwExperience
    {
        public IntwExperience()
        {
            IntwQuesExp = new HashSet<IntwQuesExp>();
        }

        public int IntwExperienceId { get; set; }
        public string Experience { get; set; }

        public virtual ICollection<IntwQuesExp> IntwQuesExp { get; set; }
    }
}
