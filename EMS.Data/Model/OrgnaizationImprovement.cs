using System;
using System.Collections.Generic;

namespace EMS.Data.Model
{
    public partial class OrgnaizationImprovement
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? OrganizationImprovementTypeId { get; set; }
        public string Description { get; set; }
        public int? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public string AssesmentYear { get; set; }

        public virtual UserLogin AddedByNavigation { get; set; }
        public virtual OrgImprovementAreas OrganizationImprovementType { get; set; }
        public virtual UserLogin User { get; set; }
    }
}
