using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class CvBuilderEstimatePrice
    {
        public int Id { get; set; }
        public int? RoleId { get; set; }
        public int? ExpId { get; set; }
        public int? TechId { get; set; }
        public decimal? Price { get; set; }
        public bool? IsActive { get; set; }
    }
}
