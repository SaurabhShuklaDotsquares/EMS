using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class AwardProjectStakeHolder
    {
        public int Id { get; set; }
        public int? AwardProjectId { get; set; }
        public int? StakeHolderUid { get; set; }
        public int? StakeHolderRoleId { get; set; }
        public decimal? AssignedDuration { get; set; }

        public virtual AwardProjectDuration AwardProject { get; set; }
        public virtual EstimateRole StakeHolderRole { get; set; }
        public virtual UserLogin StakeHolderU { get; set; }
    }
}
