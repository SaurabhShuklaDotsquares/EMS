using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EMS.Core.Enums;

namespace EMS.Dto
{
    public class AppraiseDto
    {
        public AppraiseDto()
        {
            Priority = new List<SelectListItem>();
        }
        public int Id { get; set; }
        public int UId { get; set; }
        public int EmployeeId { get; set; }
        public int ProjectId { get; set; }

        [DisplayName("Appraise Received from Client")]
        public string ClientComment { get; set; }

        [DisplayName("Client Appraise Date")]
        public string ClientDate { get; set; }
        public string AddedDate { get; set; }
        public string ModifyDate { get; set; }
        public List<SelectListItem> Employee { get; set; }
        public List<SelectListItem> Project { get; set; }
        public AppraiseType AppraiseType { get; set; }

        [DisplayName("TL/PM Comment")]
        public string TlComment { get; set; }
        public string IP { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int AppraiseId { get; set; }
        public List<SelectListItem> AppraiseListItem { get; set; }
        [DisplayName("Send appraise email to respective Employee, PM and TL")]
        public bool SendEmail { get; set; }
        public int PriorityId { get; set; }
        public List<SelectListItem> Priority { get; set; }
    }
}
