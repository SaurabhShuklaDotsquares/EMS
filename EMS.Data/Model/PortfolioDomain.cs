using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class PortfolioDomain
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public int DomainId { get; set; }

        public virtual DomainType Domain { get; set; }
        public virtual Portfolio Portfolio { get; set; }
    }
}
