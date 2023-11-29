using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class IntwUserAnswer
    {
        public int IntwUserAnswerId { get; set; }
        public decimal? IntwUserQuesId { get; set; }
        public int? IntwUserId { get; set; }
        public int? IntwQusOptionsId { get; set; }
        public int? IntwUserSessionId { get; set; }
        public DateTime? AddDate { get; set; }

        public virtual IntwQusOptions IntwQusOptions { get; set; }
        public virtual IntwUser IntwUser { get; set; }
        public virtual IntwUserQues IntwUserQues { get; set; }
        public virtual IntwUserSession IntwUserSession { get; set; }
    }
}
