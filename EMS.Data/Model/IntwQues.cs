using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class IntwQues
    {
        public IntwQues()
        {
            IntwQuesExp = new HashSet<IntwQuesExp>();
            IntwQusOptions = new HashSet<IntwQusOptions>();
            IntwUserQues = new HashSet<IntwUserQues>();
        }

        public decimal IntwQuesId { get; set; }
        public int? IntwTechnologyId { get; set; }
        public int? IntwQuestypeId { get; set; }
        public string Title { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? Modifydate { get; set; }

        public virtual IntwQuestype IntwQuestype { get; set; }
        public virtual IntwTechnology IntwTechnology { get; set; }
        public virtual ICollection<IntwQuesExp> IntwQuesExp { get; set; }
        public virtual ICollection<IntwQusOptions> IntwQusOptions { get; set; }
        public virtual ICollection<IntwUserQues> IntwUserQues { get; set; }
    }
}
