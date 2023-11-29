using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Web.Models.Others
{
    public class FinalInvoiceList
    {
        public int crm_id { get; set; }
        public string invoice_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public decimal total_amount { get; set; }

        public string invoice_amount { get; set; }
        public string currency { get; set; }
        public int payment_status_id { get; set; }

        public string invoice_payment_status_id { get; set; }
        public int status { get; set; }
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string ClientName { get; set; }
        public int? ProjectInvoiceID { get; set; }
        public int? BA_ID { get; set; }
        public int? TL_ID { get; set; }
        public int CurrencyID { get; set; }
        public string CurrencySign { get; set; }
        public string payment_status { get; set; }
    }
}