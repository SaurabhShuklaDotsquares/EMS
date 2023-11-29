using EMS.Data;
using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ComplaintAppraiseArea
    {
        public ComplaintAppraiseArea()
        {
            AppraiseUserArea = new HashSet<AppraiseUserArea>();
            ComplainUserArea = new HashSet<ComplainUserArea>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<AppraiseUserArea> AppraiseUserArea { get; set; }
        public virtual ICollection<ComplainUserArea> ComplainUserArea { get; set; }
    }
}
