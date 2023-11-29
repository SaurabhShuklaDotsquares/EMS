using System;
using System.Collections.Generic;

namespace EMS.Data.Model
{
    public partial class EmployeeParticipationImprovement
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string OrganizationParticipationDescription { get; set; }
        public string ProcessImprovementDescription { get; set; }
        public string AssesmentYear { get; set; }
        public int? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }

        public virtual UserLogin AddedByNavigation { get; set; }
        public virtual UserLogin User { get; set; }
    }
}
