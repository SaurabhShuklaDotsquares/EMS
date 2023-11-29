using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace EMS.Website.Models
{
    public class WfhHRM
    {
        [JsonProperty(PropertyName = "emailid")]
        public string emailid { get; set; }

        [JsonProperty(PropertyName = "startDate")]
        public string startDate { get; set; }

        [JsonProperty(PropertyName = "endDate")]
        public string endDate { get; set; }

        [JsonProperty(PropertyName = "WFHCategory")]
        public string WFHCategory { get; set; }

        [JsonProperty(PropertyName = "ActionType")]
        public string ActionType { get; set; }

        [JsonProperty(PropertyName = "HrmRequest")]
        public string HrmRequest { get; set; }

        [JsonProperty(PropertyName = "HrmResponse")]
        public string HrmResponse { get; set; }

        [JsonProperty(PropertyName = "Status")]
        public bool Status { get; set; }
    }
    public class WfhHrmRequest
    {
        [JsonProperty(PropertyName = "emailId")]
        public string emailId { get; set; }

        [JsonProperty(PropertyName = "startDate")]
        public string startDate { get; set; }

        [JsonProperty(PropertyName = "endDate")]
        public string endDate { get; set; }

        [JsonProperty(PropertyName = "WFHCategory")]
        public string WFHCategory { get; set; }

        [JsonProperty(PropertyName = "actionType")]
        public string actionType { get; set; }
        public string emsWFHId { get; set; }
        // [JsonProperty(PropertyName = "HrmRequest")]
        //public string HrmRequest { get; set; }
        //[JsonProperty(PropertyName = "HrmResponse")]
        //public string HrmResponse { get; set; }
        //[JsonProperty(PropertyName = "Status")]
        //public bool Status { get; set; }
    }
}
