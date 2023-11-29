using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EstimateRole
    {
        public EstimateRole()
        {
            EstimatePriceCalculationDetail = new HashSet<EstimatePriceCalculationDetail>();
            EstimateRoleExp = new HashSet<EstimateRoleExp>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public bool? IsSdlc { get; set; }

        public virtual ICollection<EstimatePriceCalculationDetail> EstimatePriceCalculationDetail { get; set; }
        public virtual ICollection<EstimateRoleExp> EstimateRoleExp { get; set; }
    }
}
