using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EstimateRoleExp
    {
        public EstimateRoleExp()
        {
            EstimateRoleTechnoloyPrice = new HashSet<EstimateRoleTechnoloyPrice>();
            EstimatePriceCalculationDetail = new HashSet<EstimatePriceCalculationDetail>();
        }

        public int Id { get; set; }
        public int EstimateRoleId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual EstimateRole EstimateRole { get; set; }
        public virtual ICollection<EstimateRoleTechnoloyPrice> EstimateRoleTechnoloyPrice { get; set; }
        public virtual ICollection<EstimatePriceCalculationDetail> EstimatePriceCalculationDetail { get; set; }
    }
}
