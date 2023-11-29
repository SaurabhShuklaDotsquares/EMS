using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class FinancialYear
    {
        public FinancialYear()
        {
            Investment = new HashSet<Investment>();
            InvestmentType = new HashSet<InvestmentType>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int FromMonth { get; set; }
        public int FromYear { get; set; }
        public int ToYear { get; set; }
        public int ToMonth { get; set; }

        public virtual ICollection<Investment> Investment { get; set; }
        public virtual ICollection<InvestmentType> InvestmentType { get; set; }
    }
}
