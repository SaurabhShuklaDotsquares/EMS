using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class TaskStatusDto
    {
        public int TaskStatusId { get; set; }
        public string TaskStatus { get; set; }
    }

    public class TaskToDoDto
    {
        public TaskStatusDto TaskStatus { get; set; }
        public List<SelectListItem> users { get; set; }

        public int TaskStatusId { get; set; }
    }
}
