using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Web
{
    public class LeadSummaryViewModel
    {
        public int TotalClients { get;set;}
        public int NewClients { get; set; }
        public int ExistingClient { get; set; }
        public int TotalConvertedClients { get; set; }
        public int ConvertedNewClients { get; set; }
        public int ConvertedExistingClients { get; set; }

        public int EscalatedLeads { get; set; }
        public int AwaitingResponseLeads { get; set; }
        public string TotalConversion { get; set; }
        public string TotalNewLeadConversion { get; set; }

    }
}

