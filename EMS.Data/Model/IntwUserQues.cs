using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class IntwUserQues
    {
        public IntwUserQues()
        {
            IntwUserAnswer = new HashSet<IntwUserAnswer>();
        }

        public decimal IntwUserQuesId { get; set; }
        public decimal? IntwQuesid { get; set; }
        public int? IntwUserid { get; set; }
        public int? IntwUserSessionid { get; set; }
        public DateTime? Adddate { get; set; }

        public virtual IntwQues IntwQues { get; set; }
        public virtual IntwUser IntwUser { get; set; }
        public virtual IntwUserSession IntwUserSession { get; set; }
        public virtual ICollection<IntwUserAnswer> IntwUserAnswer { get; set; }
    }
}
