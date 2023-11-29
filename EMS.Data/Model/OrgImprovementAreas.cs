using System;
using System.Collections.Generic;

namespace EMS.Data.Model
{
    public partial class OrgImprovementAreas
    {
        public OrgImprovementAreas()
        {
            OrgnaizationImprovement = new HashSet<OrgnaizationImprovement>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public DateTime AddedOn { get; set; }

        public virtual ICollection<OrgnaizationImprovement> OrgnaizationImprovement { get; set; }
    }
}
