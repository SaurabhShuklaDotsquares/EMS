using EMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
    public class FeedbackDto
    {
        public FeedbackDto()
        {
            FeedbackrankDto = new List<Dto.FeedbackrankDto>();
        }
        public int Id { get; set; }
        public int Uid { get; set; }
        public string Name { get; set; }
        public string EmailOffice { get; set; }
        public string Designation { get; set; }
        public string EmpCode { get; set; }
        public string LeavingDate { get; set; }
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
        public List<EmployeeFeedbackRank> Feedbackrank { get; set; }
        public List<FeedbackRankStatusDto> FeedbackRankStatus { get; set; }
        public List<FeedbackrankDto> FeedbackrankDto { get; set; }
        public int? SaveBy { get; set; }
    }
    public class FeedbackRankStatusDto
    {
        public int EmployeeFeedbackRankId { get; set; }
        public byte EmployeeFeedbackStatus { get; set; }
    }
    public class FeedbackrankDto
    {
        public string Name { get; set; }
        public int EmployeeFeedbackRankId { get; set; }
        public byte? EmployeeFeedbackStatus { get; set; }
    }

    public class FeedbackIndexDto
    {
        public List<SelectListItem> PMList { get; set; }
        public List<SelectListItem> DepartmentList { get; set; }
        public List<SelectListItem> EmpFeedbackReasonList { get; set; }
        public string EmployeeCode { get; set; }
    }
}
