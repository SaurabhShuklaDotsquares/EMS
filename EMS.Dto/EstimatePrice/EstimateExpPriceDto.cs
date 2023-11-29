using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
   public class EstimateExpPriceDto
    {
        public int EstimateRoleExpID { get; set; }
        public string EstimateRoleExp { get; set; }
        public decimal Price { get; set; }
    }
}
