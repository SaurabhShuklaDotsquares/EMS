using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.API.Model {
    public class TimeSheetInputModel {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string CRMProjectId { get; set; }
    }
    public class TimeSheetModel {
        public int TimeSheetId { get; set; }
        public string CRMId { get; set; }
        public string ProjectName { get; set; }
        public string TimeSheetDate { get; set; }
        public string VirtualDeveloper { get; set; }
        public string VirtualDeveloperEmail { get; set; }
        public string ActualDeveloper { get; set; }
        public string ActualDeveloperEmail { get; set; }

        public string Description { get; set; }
        public string WorkHours { get; set; }

    }

    public class TimeSheetWorkHourRequestModel {
        public string CRMProjectId { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
    public class TimeSheetWorkHourResponseModel {
        public string TotalDevHours { get; set; }
        public string TotalOtherHours { get; set; }
    }

}
