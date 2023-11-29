using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProcessImprovement
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Description { get; set; }
        public int? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }

        public virtual UserLogin AddedByNavigation { get; set; }
        public virtual UserLogin User { get; set; }
    }
}
