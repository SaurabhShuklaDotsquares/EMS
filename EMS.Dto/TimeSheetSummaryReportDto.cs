using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EMS.Dto
{
    public class TimeSheetSummaryReportDto
    {
        public string SRNo { get; set; }
        public string CrmId { get; set; }
        public string ProjectName { get; set; }
        public string StartDateEndDate { get; set; }
        public string User { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string HandoverPersonName { get; set; }
        public string HandoverPersonSignature { get; set; }
        public string TLSignature { get; set; }
        public string Comment { get; set; }
        
    }
  

}
