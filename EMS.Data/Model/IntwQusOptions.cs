using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class IntwQusOptions
    {
        public IntwQusOptions()
        {
            IntwUserAnswer = new HashSet<IntwUserAnswer>();
        }

        public int IntwQusOptionsId { get; set; }
        public decimal? IntwQuesId { get; set; }
        public string OptionTitle { get; set; }
        public bool? Iscorrect { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual IntwQues IntwQues { get; set; }
        public virtual ICollection<IntwUserAnswer> IntwUserAnswer { get; set; }
    }
}
