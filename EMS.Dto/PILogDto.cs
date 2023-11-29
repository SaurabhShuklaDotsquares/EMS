using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
    public class PILogRequestDto
    {
        public PILogRequestDto()
        {
            ProcessList = new List<SelectListItem>();
        }

        public int Id { get; set; }

        [DisplayName("Process")]
        public List<SelectListItem> ProcessList { get; set; }

        public int ProcessId { get; set; }

        [DisplayName("Process Name")]
        public string ProcessName { get; set; }

        [DisplayName("Potential Area")]
        public string PotentialArea { get; set; }
        
        public int CurrentUserId { get; set; }

        public System.DateTime? CreatedDateFromMoM { get; set; }

        public byte? StatusFromMoM { get; set; }

        public int? MomMeetingTaskId { get; set; }

        public System.DateTime? EstimatedScheduleFromMoM { get; set; }
        public string RemarkFromMOM { get; set; }

    }

    public class PILogApprovalDto
    {
        public PILogApprovalDto()
        {
            StatusList = new List<SelectListItem>();
        }

        public int Id { get; set; }

        [DisplayName("Process Name")]
        public string ProcessName { get; set; }

        [DisplayName("Potential Area")]
        public string PotentialArea { get; set; }
        
        public string Remarks { get; set; }

        [DisplayName("Cancel Reason")]
        public string CancelReason { get; set; }

        [DisplayName("Estimated Schedule")]
        public string EstimatedSchedule { get; set; }

        public string CreateDate { get; set; }

        [DisplayName("Suggested By")]
        public string CreateBy { get; set; }

        public string ModifyDate { get; set; }

        public string ModifyBy { get; set; }

        public byte Status { get; set; }

        public int CurrentUserId { get; set; }

        public bool ApprovalAllowed { get; set; }

        public bool RollOutAllowed { get; set; }
        
        public List<SelectListItem> StatusList { get; set; }
    }

    public class PILogIndexDto
    {
        public List<SelectListItem> PMUserList { get; set; }

        public List<SelectListItem> PILogStatusList { get; set; }

        public PILogIndexDto()
        {
            PMUserList = new List<SelectListItem>();
            PILogStatusList = new List<SelectListItem>();
        }
    }
}
