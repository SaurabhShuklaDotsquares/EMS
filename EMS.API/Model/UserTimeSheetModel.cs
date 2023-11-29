using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.API.Model
{
    public class UserTimeSheetModel
    {
        public int CRMId { get; set; }
        public string EmailOffice { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string Hours { get; set; }
        public string Type { get; set; }
        public decimal? EMSTimesheetId { get; set; }
        public int EMSProjectID { get; set; }
    }
}
