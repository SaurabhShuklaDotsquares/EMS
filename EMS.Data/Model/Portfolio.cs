using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Portfolio
    {
        public Portfolio()
        {
            PortfolioDomain = new HashSet<PortfolioDomain>();
            PortfolioTech = new HashSet<PortfolioTech>();
        }

        public int PortfolioId { get; set; }
        public string WebsiteName { get; set; }
        public string WebsiteUrl { get; set; }
        public string ClientName { get; set; }
        public int? CrmprojectId { get; set; }
        public bool IsScratch { get; set; }
        public bool IsNda { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Ip { get; set; }
        public int? Uid { get; set; }
        public int DeveloperId { get; set; }

        public virtual UserLogin Developer { get; set; }
        public virtual ICollection<PortfolioDomain> PortfolioDomain { get; set; }
        public virtual ICollection<PortfolioTech> PortfolioTech { get; set; }
    }
}
