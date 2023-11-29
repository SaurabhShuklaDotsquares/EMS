using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
    public class ComplaintDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int ProjectId { get; set; }
        public int ComplaintTypeId { get; set; }
        public int PriorityId { get; set; }

        [DisplayName("Complaint Received from Client")]
        public string ClientComplain { get; set; }

        [DisplayName("Client Complain Date")]
        public string ClientComplainDate { get; set; }

        [DisplayName("TL/PM Explanation")]
        public string TlExplanation { get; set; }

        [DisplayName("TL/PM Complain Date")]
        public string TlComplainDate { get; set; }

        [DisplayName("Developer's Explanation")]
        public string DeveloperExplanation { get; set; }

        [DisplayName("Developer's Explanation Date")]
        public string DeveloperComplainDate { get; set; }
        public int AddedBy { get; set; }
        public string AddedDate { get; set; }
        public string ModifyDate { get; set; }
        public bool IsDelete { get; set; }
        [DisplayName("Area of Improvement")]
        public string AreaofImprovement { get; set; }
        [DisplayName("Employee")]
        public List<SelectListItem> EmployeeList { get; set; }
        [DisplayName("Complaint Type")]
        public List<SelectListItem> ComplaintType { get; set; }
        public List<SelectListItem> Priority { get; set; }
        public List<SelectListItem> Project { get; set; }
        public int[] Employees { get; set; }
        [DisplayName("What Lesson Learned?")]
        public string LessionLearned { get; set; }
        [DisplayName("Send lesson learned email for all members of Roles PM,TL,BA and SD?")]
        public bool SendEmailLessionLearned { get; set; }
        [DisplayName("Send complaint email to Respective employee, PM and TL")]
        public bool SendEmailEmployee { get; set; }
        public ComplaintDto()
        {
            EmployeeList = new List<SelectListItem>();
            ComplaintType = new List<SelectListItem>();
            Priority = new List<SelectListItem>();
        }
    }
}
