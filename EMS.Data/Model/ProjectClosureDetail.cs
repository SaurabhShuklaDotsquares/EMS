using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectClosureDetail
    {
        public int Id { get; set; }
        public int? ProjectClosureId { get; set; }
        public DateTime? NextStartDate { get; set; }
        public string Reason { get; set; }
        public DateTime Created { get; set; }
        public int? AddedByUid { get; set; }

        public virtual UserLogin UserLogin { get; set; }
        public virtual ProjectClosure ProjectClosure { get; set; }
    }
}
