using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EMS.Dto
{
    public class LessonLearntDto
    {
        public LessonLearntDto()
        {
            ProjectList = new List<SelectListItem>();
        }

        public int Id { get; set; }
        [DisplayName("Project Name")]
        public int? ProjectId { get; set; }
        [DisplayName("What Lesson Learned ?")]
        public string WhatLearnt { get; set; }
        public int CreatedById { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        [DisplayName("Email Sent?")]
        public bool IsSendEmail { get; set; }

        public List<SelectListItem> ProjectList { get; set; }
    }
}
