using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EstimateRoleTechnoloyPrice
    {
        public int Id { get; set; }
        public int EstimateRoleExpId { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int? EstimateTechnologyId { get; set; }
        public virtual EstimateRoleExp EstimateRoleExp { get; set; }
        public virtual EstimateTechnology EstimateTechnology { get; set; }
    }
}
