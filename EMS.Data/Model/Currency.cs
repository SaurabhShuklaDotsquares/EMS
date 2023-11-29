using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Currency
    {
        public Currency()
        {
            Expense = new HashSet<Expense>();
            ProjectInvoice = new HashSet<ProjectInvoice>();
            EstimateCountry = new HashSet<EstimateCountry>();
            EstimateHostingPackage = new HashSet<EstimateHostingPackage>();
        }

        public int Id { get; set; }
        public string CurrName { get; set; }
        public string CurrSign { get; set; }
        public int OrderBy { get; set; }
        public DateTime Modified { get; set; }
        public double? ExchangeRate { get; set; }

        public virtual ICollection<Expense> Expense { get; set; }
        public virtual ICollection<ProjectInvoice> ProjectInvoice { get; set; }

        public virtual ICollection<EstimateCountry> EstimateCountry { get; set; }
        public virtual ICollection<EstimateHostingPackage> EstimateHostingPackage { get; set; }
    }
}
