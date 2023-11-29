using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Web.Models
{
    public class InvoiceFilterViewModel
    {       
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int BAId { get; set; }
        public int TLId { get; set; }
        public string Name { get; set; }
    }
}