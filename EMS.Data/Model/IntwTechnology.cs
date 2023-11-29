using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class IntwTechnology
    {
        public IntwTechnology()
        {
            IntwQues = new HashSet<IntwQues>();
            IntwUser = new HashSet<IntwUser>();
        }

        public int IntwTechnologyId { get; set; }
        public string TechnologyName { get; set; }
        public int? NoOfQues { get; set; }
        public int? NoOfMultipleQues { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<IntwQues> IntwQues { get; set; }
        public virtual ICollection<IntwUser> IntwUser { get; set; }
    }
}
