using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.Website.Models
{
    public class GetLeadSummaryFilterViewModel
    {
        public string txtSearch { get; set; }       
        public string txtAssignedFrom { get; set; }
        public string txtAssignedTo { get; set; }
        public string drpOwner { get; set; }
        public string drpStatus { get; set; }
        public string drpType { get; set; }
        public string drpClient { get; set; }
        public string[] chkCountry { get; set; }         
        public string[] chkStatus { get; set; }
        public bool existClient { get; set; }
        public bool newClient { get; set; }
        public bool awaitResp { get; set; }
        public bool escalatedClient { get; set; }
        public bool existingConvert { get; set; }
        public bool newConverted { get; set; }
        public bool isPageLoad { get; set; }
        public bool isUpdateLeadSummaryTop { get; set; }

    }
}  