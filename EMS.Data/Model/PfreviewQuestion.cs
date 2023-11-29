using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class PfreviewQuestion
    {
        public PfreviewQuestion()
        {
            PfreviewResult = new HashSet<PfreviewResult>();
        }

        public int Id { get; set; }
        public string ReviewCategory { get; set; }
        public string ReviewQuestion { get; set; }
        public byte SkillType { get; set; }
        public byte RoleType { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual ICollection<PfreviewResult> PfreviewResult { get; set; }
    }
}
