using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace EMS.Dto
{
    public class FileNameDto
    {
        [DisplayName("File Name")]
        [Required]
        public string Name { get; set; }

        public IFormFile EstimateFiles { get; set; }
    }
}
