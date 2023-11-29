using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class InvestmentTypeAmountMap
    {
        public int Id { get; set; }
        public int InvestmentTypeId { get; set; }
        public int InvestmentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ModifyDate { get; set; }

        public virtual Investment Investment { get; set; }
        public virtual InvestmentType InvestmentType { get; set; }
    }
}
