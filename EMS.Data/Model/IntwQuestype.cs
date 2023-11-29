using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class IntwQuestype
    {
        public IntwQuestype()
        {
            IntwQues = new HashSet<IntwQues>();
        }

        public int IntwQuestypeId { get; set; }
        public string TypeName { get; set; }
        public decimal? Marks { get; set; }

        public virtual ICollection<IntwQues> IntwQues { get; set; }
    }
}
