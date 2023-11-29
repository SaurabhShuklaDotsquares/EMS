using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EMS.Dto
{
    public class EventDto
    {
        public int LeaveId { get; set; }
        [DisplayName("Event Title")]
        [Required]
        public string Title { get; set; }
        [DisplayName("Event Date")]
        [Required]
        public string LeaveDate { get; set; }
        public byte CountryId { get; set; }
        public bool IsActive { get; set; }
        public string LeaveType { get; set; }
    }
}
