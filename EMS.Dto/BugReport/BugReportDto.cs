using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace EMS.Dto
{
    public class BugReportDto
    {
        public int Id { get; set; }
        [DisplayName("Section Name")]
        public string SectionName { get; set; }
        [DisplayName("Description")]
        public string SectionDescription { get; set; }
        public string ImageName { get; set; }
        public string AddedDate { get; set; }
        public string ModifyDate { get; set; }
        public string IP { get; set; }
        public int AddedBy { get; set; }
        public bool IsClosed { get; set; }
        [DisplayName("Page Path")]
        public string PagePath { get; set; }
        public string Remark { get; set; }
        public bool IsApproved { get; set; }
        public bool IsApprover { get; set; }
        public IFormFile Attachment { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public BugReportDto()
        {
            StatusList = new List<SelectListItem>();
        }
    }
}
