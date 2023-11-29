using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class CalculationResultDto
    {
        public string Heading { get; set; }
        public string Pound { get; set; }
        public string USD { get; set; }
        public string AUD { get; set; }
        public string AED { get; set; }
        public string MinPound { get; set; }
        public string MinUSD { get; set; }
        public string MinAUD { get; set; }
        public bool IsBasePrice { get; set; }
        public bool? IsCollapsePrice { get; set; }
        public decimal Percent { get; set; }
    }
}
