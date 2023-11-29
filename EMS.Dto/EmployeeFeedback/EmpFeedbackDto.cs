using EMS.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto.EmployeeFeedback
{
    public class EmpFeedbackDto
    {
        public int Id { get; set; }
        public int Uid { get; set; }
        public int PmUid { get; set; }
        public string Name { get; set; }
        public string EmailOffice { get; set; }
        public string Designation { get; set; }
        public string EmpCode { get; set; }
        public DateTime? LeavingDate { get; set; }
        public string comment { get; set; }
        public string ReviewLink { get; set; }
        public bool EmailSkypePassReset { get; set; }
        public bool LFProfile { get; set; }
        public string Suggestions { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifedDate { get; set; }
        public UserLogin Userinfo { get; set; }
        public string PMName { get; set; }
        public string Department { get; set; }
        public bool IsFeedbackSubmited { get; set; }
        public int[] selectedfeedbackreason { get; set; }
        public List<SelectListItem> LeavingReason { get; set; }
        public List<SelectListItem> Feedbackreason { get; set; }
        public List<SelectListItem> Feedbackrank { get; set; }
        public List<FeedbackRankStatusDto> FeedbackRankStatus { get; set; }
        public List<FeedbackrankDto> FeedbackrankDto { get; set; }

        public List<SelectListItem> PMUsers { get; set; }
        public List<SelectListItem> EMpList { get; set; }
        public int? SaveBy { get; set; }
    }
}
