using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Data.Model
{
    public partial class PreferencesWFH
    {
        public int PreferenceId { get; set; }
        public int? pmid { get; set; }
        public int? PriorWFHDay { get; set; }
        public int? ActivityRefreshTime { get; set; }
        public string EmailFrom { get; set; }
        public string EmailPM { get; set; }
        public string EmailHR { get; set; }
        public bool? IsActive { get; set; }
        public DateTime AddDate { get; set; }
        public string EmailDeveloper { get; set; }
        public string InductionDoc { get; set; }
        public int? ELActTimeLimit { get; set; }
        public int? TimeSheetDay { get; set; }
        public string TimeSheetEmail { get; set; }
        public string ProjectClosureEmail { get; set; }
        public string AdditionalSupportEmail { get; set; }
        public int? TimesheetModifyDay { get; set; }
    }
}
