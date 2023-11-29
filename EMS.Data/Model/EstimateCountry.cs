using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EstimateCountry
    {
        public EstimateCountry()
        {
            EstimateHostingPackage = new HashSet<EstimateHostingPackage>();
            EstimatePriceCalculation = new HashSet<EstimatePriceCalculation>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int? CurrencyId { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual ICollection<EstimateHostingPackage> EstimateHostingPackage { get; set; }
        public virtual ICollection<EstimatePriceCalculation> EstimatePriceCalculation { get; set; }
    }
}
