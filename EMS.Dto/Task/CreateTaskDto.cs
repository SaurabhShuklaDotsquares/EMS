using EMS.Data;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using static EMS.Core.Enums;

namespace EMS.Dto
{
    public class CreateTaskDto
    {
        public CreateTaskDto()
        {
            TaskAssigntoes = new List<TaskAssignedTo>();
            Users = new List<SelectListItem>();
        }
        public decimal TaskID { get; set; }       
        public int AddedUid { get; set; }
        [DisplayName("Name of Task")]
        public string TaskName { get; set; }
        [DisplayName("Priority Type")]       
        public Priority Priority { get; set; }
        [DisplayName("Assigned To")]
        public int[] Assign { get; set; }
        [DisplayName("Remark")]
        public string Remark { get; set; }
        [DisplayName("Task End Date")]
        public string TaskEndDate { get; set; }
        [DisplayName("Status")]
        public TaskStatusType TaskStatusId { get; set; }         
        public List<TaskAssignedTo> TaskAssigntoes { get; set; }
        public List<SelectListItem> Users { get; set; }
        public TaskStatusType TaskStatus { get; set; }    
    }    
}
