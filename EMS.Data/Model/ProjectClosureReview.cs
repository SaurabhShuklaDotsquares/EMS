using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectClosureReview
    {
        public int ProjectClosureId { get; set; }
        public string Comments { get; set; }
        public DateTime? NextStartDate { get; set; }
        public byte? PromisingPercentageId { get; set; }
        public int DeveloperCount { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateByUid { get; set; }
        public DateTime ModifyDate { get; set; }
        public int ModifyByUid { get; set; }

        public virtual UserLogin UserLogin { get; set; }
        public virtual UserLogin UserLogin1 { get; set; }
        public virtual ProjectClosure ProjectClosure { get; set; }
    }
}
