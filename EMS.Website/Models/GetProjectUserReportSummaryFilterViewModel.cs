using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.Website.Models
{
    public class GetProjectUserReportSummaryFilterViewModel
    {
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
        public int porjectID { get; set; }
        public int? virtualDeveloperID { get; set; }
        public int userID { get; set; }
    }
}
