using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Web.Models.Others
{
    [Serializable]
    public class Invoicejson
    {
        public string crm_id { get; set; }
        public string invoice_id { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string total_amount { get; set; }
        public string currency { get; set; }
        public string payment_status_id { get; set; }
        public string status { get; set; }
    }
    public class InvoiceWorkingHourjson
    {
        public string crm_id { get; set; }
        public string invoice_id { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string total_amount { get; set; }
        public string currency { get; set; }
        public string payment_status_id { get; set; }
        public string status { get; set; }
    }
}