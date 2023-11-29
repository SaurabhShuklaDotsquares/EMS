using EMS.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EMS.Dto
{
    public class ReminderDto
    {
        public ReminderDto()
        {
            PaticipantList = new List<SelectListItem>();
            GroupList = new List<SelectListItem>();

        }
        public int Id { get; set; }
        public string Title { get; set; }
        [DisplayName("Reminder Date")]
        public string ReminderDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        [DisplayName("Status")]
        public bool IsActive { get; set; }
        public int? Status { get; set; }
        public int? ReminderType { get; set; }
        public bool? ActionTaken { get; set; }
        public int? ActionTakenBy { get; set; }
        public DateTime? ActionTakenOn { get; set; }
        [DisplayName("Exclude")]
        public bool IsExcludeMe { get; set; }
        public int[] Paticipants { get; set; }
        public int[] Groups { get; set; }
        public List<SelectListItem> PaticipantList { get; set; }
        public List<SelectListItem> GroupList { get; set; }
        public int ActiveStatusId { get; set; }
        public List<SelectListItem> ActiveStatus { get; set; }
        public int EmployeeId { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
}
