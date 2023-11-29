using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectInvoice
    {
        public ProjectInvoice()
        {
            ProjectInvoiceComments = new HashSet<ProjectInvoiceComment>();
        }

        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceStartDate { get; set; }
        public DateTime InvoiceEndDate { get; set; }
        public decimal InvoiceAmount { get; set; }
        public int InvoiceStatus { get; set; }
        public int? Uid_BA { get; set; }
        public int? Uid_TL { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public int CurrencyID { get; set; }
        public int? PMID { get; set; }
        public string Comment { get; set; }
        public string Country { get; set; }
        public int? CountryId { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual Project Project { get; set; }
        public virtual UserLogin UserLogin  { get; set; }
        public virtual UserLogin UserLogin1 { get; set; }
        public virtual ICollection<ProjectInvoiceComment> ProjectInvoiceComments { get; set; }
    }
}
