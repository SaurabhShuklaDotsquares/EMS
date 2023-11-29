using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class EstimateRoleTechnoloyPriceDto
    {
        public int Id { get; set; }
        public int EstimateRoleExpId { get; set; }
        public string EstimateRoleExpName { get; set; }
        public decimal Price { get; set; }
        public int EstimateRoleId { get; set; }
        public string EstimateRoleName { get; set; }
        public int? EstimateTechnologyId { get; set; }
        public string EstimateTechnologyName { get; set; }
    }
}
