using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectInvoiceComment
    {
        public int Id { get; set; }
        public int ProjectInvoiceId { get; set; }
        public string InvoiceComments { get; set; }
        public DateTime? ChaseDate { get; set; }
        public DateTime Created { get; set; }

        public virtual ProjectInvoice ProjectInvoice { get; set; }
    }
}
