using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class PortfolioTech
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public int TechId { get; set; }

        public virtual Portfolio Portfolio { get; set; }
        public virtual Technology Tech { get; set; }
    }
}
