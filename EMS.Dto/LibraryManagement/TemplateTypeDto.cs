using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EMS.Dto
{
    public class TemplateTypeDto
    {
        [DisplayName("Template Name")]
        [Required]
        public string Name { get; set; }
    }
}
