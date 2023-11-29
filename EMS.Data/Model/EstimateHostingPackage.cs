using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EstimateHostingPackage
    {
        public EstimateHostingPackage()
        {
            EstimateHostingPackageTechnology = new HashSet<EstimateHostingPackageTechnology>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string PackageDetail { get; set; }
        public int? CountryId { get; set; }
        public decimal HostingCost { get; set; }
        public byte HostingCostType { get; set; }
        public decimal SetupCost { get; set; }
        public byte SetupCostType { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int? CurrencyId { get; set; }

        public virtual EstimateCountry Country { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual ICollection<EstimateHostingPackageTechnology> EstimateHostingPackageTechnology { get; set; }
    }
}
