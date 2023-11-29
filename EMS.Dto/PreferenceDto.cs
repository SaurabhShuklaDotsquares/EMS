using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using EMS.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EMS.Dto
{
    public class PreferenceDto
    {
        public int? PriorLeaveDay { get; set; }
        public int? ActivityRefreshTime { get; set; }
        public int? ELActTimeLimit { get; set; }
        public int? TimeSheetDay { get; set; }
        public string EmailFrom { get; set; }
        public string EmailPM { get; set; }
        public string EmailHR { get; set; }
        public string EmailDeveloper { get; set; }
        public string TimeSheetEmail { get; set; }
        public string ProjectClosureEmail { get; set; }
        public string DeveloperDocLink { get; set; }
        public string SeniorDeveloperDocLink { get; set; }
        public IFormFile DeveloperFile { get; set; }
        public IFormFile SenDeveloperFile { get; set; }
        public bool IsAllowLeave { get; set; }
        public bool IsAllowWFH { get; set; }
        


    }

}
