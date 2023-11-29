using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EstimatePriceCalculation
    {
        public EstimatePriceCalculation()
        {
            EstimatePriceCalculationDetail = new HashSet<EstimatePriceCalculationDetail>();
        }

        public int Id { get; set; }
        public string CrmleadId { get; set; }
        public decimal TotalCost { get; set; }
        public double? ExchangeRateUsd { get; set; }
        public double? ExchangeRateAud { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int? CountryId { get; set; }
        public int? CreatedByUid { get; set; }
        public int? EstimateModelId { get; set; }
        public int? ModifiedByUid { get; set; }
        public string EstimateName { get; set; }

        public virtual EstimateCountry Country { get; set; }
        public virtual EstimateModel EstimateModel { get; set; }
        public virtual UserLogin CreatedByU { get; set; }
        public virtual UserLogin ModifiedByU { get; set; }
        public virtual ICollection<EstimatePriceCalculationDetail> EstimatePriceCalculationDetail { get; set; }
    }
}
