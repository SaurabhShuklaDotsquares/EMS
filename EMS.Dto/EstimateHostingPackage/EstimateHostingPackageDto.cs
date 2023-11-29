using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EMS.Dto
{
    public class EstimateHostingPackageDto
    {
        public int Id { get; set; }
        [Display(Name = "Package Name")]
        public string Name { get; set; }
        [Display(Name = "Package Detail")]
        public string PackageDetail { get; set; }
        [Display(Name = "Country")]
        public int? CountryId { get; set; }
        [Display(Name = "Hosting Cost")]
        public decimal HostingCost { get; set; }
        [Display(Name = "Hosting Cost Type")]
        public byte HostingCostType { get; set; }
        [Display(Name = "Setup Cost")]
        public decimal SetupCost { get; set; }
        [Display(Name = "Setup Cost Type")]
        public byte SetupCostType { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Currency")]
        public int? CurrencyId { get; set; }

        [Display(Name = "Estimate Technology")]
        [Required]
        public List<int> EstimateTechnologyIds { get; set; }

        public string CurrSign { get; set; }
        public string HostingCostTypeName { get; set; }
        public string SetupCostTypeName { get; set; }
        public string CountryName { get; set; }
        public string TechnologyName { get; set; }
    }
}
