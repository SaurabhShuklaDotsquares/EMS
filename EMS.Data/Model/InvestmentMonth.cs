using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class InvestmentMonth
    {
        public int Id { get; set; }
        public int InvestmentId { get; set; }
        public int InvMonth { get; set; }
        public int InvYear { get; set; }
        public decimal MonthlyRent { get; set; }
        public DateTime ModifyDate { get; set; }

        public virtual Investment Investment { get; set; }
    }
}
