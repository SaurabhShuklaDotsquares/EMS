using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EstimateTechnology
    {
        public EstimateTechnology()
        {
            EstimateHostingPackageTechnology = new HashSet<EstimateHostingPackageTechnology>();
            EstimateRoleTechnoloyPrice = new HashSet<EstimateRoleTechnoloyPrice>();
            EstimatePriceCalculationDetail = new HashSet<EstimatePriceCalculationDetail>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime ModifyDate { get; set; }
        public virtual ICollection<EstimateHostingPackageTechnology> EstimateHostingPackageTechnology { get; set; }
        public virtual ICollection<EstimatePriceCalculationDetail> EstimatePriceCalculationDetail { get; set; }
        public virtual ICollection<EstimateRoleTechnoloyPrice> EstimateRoleTechnoloyPrice { get; set; }
    }
}
