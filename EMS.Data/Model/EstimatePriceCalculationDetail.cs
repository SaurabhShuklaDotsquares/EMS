using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EstimatePriceCalculationDetail
    {

        public int Id { get; set; }
        public int PriceCalculationId { get; set; }
        public int EstimateRoleId { get; set; }
        public int EstimateRoleExpId { get; set; }
        public int NoOfResources { get; set; }
        public int EstimateHours { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int? EstimateTechnologyId { get; set; }

        public virtual EstimateRole EstimateRole { get; set; }
        public virtual EstimateRoleExp EstimateRoleExp { get; set; }
        public virtual EstimateTechnology EstimateTechnology { get; set; }
        public virtual EstimatePriceCalculation PriceCalculation { get; set; }
    }
}
