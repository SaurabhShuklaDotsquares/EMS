using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace EMS.Dto
{
    public class LeadStatusModelDto
    {
        public LeadStatusModelDto()
        {
            LeadStatusModelList = new List<SelectListItem>();
        }
        public int StatusId { get; set; }
        [DisplayName("Status")]
        public string StatusName { get; set; }
        [DisplayName("Parent")]
        public int ? ParentId { get; set; }
        // [AllowHtml] ** no need ot AllowHTML in .net Core https://github.com/aspnet/Mvc/issues/324#issuecomment-50082022**
        [DisplayName("Mail Content")]
        public string MailContent { get; set; }
        public string AddDate { get; set; }
        public string ModifyDate { get; set; }
        public string IP { get; set; }
        [DisplayName("From Email")]
        public string FromEmail { get; set; }
        [DisplayName("To Email")]
        public string To { get; set; }
        [DisplayName("CC Email")]
        public string CC { get; set; }
        [DisplayName("BCC Email")]
        public string BCC { get; set; }
        public List<SelectListItem> LeadStatusModelList { get; set; }

    }
}
