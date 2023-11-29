using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.Website.Models
{
    public class LeaveHrm
    {
        [JsonProperty(PropertyName = "emailid")]
        public string emailid { get; set; }
        [JsonProperty(PropertyName = "startDate")]
        public string startDate { get; set; }
        [JsonProperty(PropertyName = "endDate")]
        public string endDate { get; set; }
        [JsonProperty(PropertyName = "leaveType")]
        public string leaveType { get; set; }
        [JsonProperty(PropertyName = "ActionType")]
        public string ActionType { get; set; }
        [JsonProperty(PropertyName = "HrmRequest")]
        public string HrmRequest { get; set; }
        [JsonProperty(PropertyName = "HrmResponse")]
        public string HrmResponse { get; set; }
        [JsonProperty(PropertyName = "Status")]
        public bool Status { get; set; }
    }
    public class LeaveHrmRequest
    {
        [JsonProperty(PropertyName = "emailId")]
        public string emailId { get; set; }
        [JsonProperty(PropertyName = "startDate")]
        public string startDate { get; set; }
        [JsonProperty(PropertyName = "endDate")]
        public string endDate { get; set; }
        [JsonProperty(PropertyName = "leaveType")]
        public string leaveType { get; set; }
        [JsonProperty(PropertyName = "actionType")]
        public string actionType { get; set; }
        public string emsLeaveId { get; set; }
        public string isHalf { get; set; }

        public string isWfh { get; set; }
        // [JsonProperty(PropertyName = "HrmRequest")]
        //public string HrmRequest { get; set; }
        //[JsonProperty(PropertyName = "HrmResponse")]
        //public string HrmResponse { get; set; }
        //[JsonProperty(PropertyName = "Status")]
        //public bool Status { get; set; }
    }
}
