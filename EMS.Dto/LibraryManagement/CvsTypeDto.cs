using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EMS.Dto.LibraryManagement
{
    public class CvsTypeDto
    {
        public int CvsId { get; set; }
        [Display(Name="Display Order")]
        public int? DisplayOrder { get; set; }
        public string Name { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
}
