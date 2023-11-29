using EMS.Data;
using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class AwardProjectDuration
    {
        public AwardProjectDuration()
        {
            AwardProjectStakeHolder = new HashSet<AwardProjectStakeHolder>();
        }

        public int Id { get; set; }
        public short? AwardProjectType { get; set; }
        public string AwardProjectCrmid { get; set; }
        public int? AwardProjectLeadId { get; set; }
        public decimal? TotalDuration { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<AwardProjectStakeHolder> AwardProjectStakeHolder { get; set; }
    }
}
