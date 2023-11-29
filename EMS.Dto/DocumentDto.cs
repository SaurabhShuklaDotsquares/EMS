using EMS.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EMS.Dto
{
    public class DocumentDto
    {
        public DocumentDto()
        {
           
        }

        public int Id { get; set; }
        [DisplayName("Title")]
        [Required]
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsActive { get; set; }
        [DisplayName("Is All")]
        public bool IsAll { get; set; }
        public int? UserId { get; set; }
        [DisplayName("Upload Document")]        
        public IFormFile Document { get; set; }
        [DisplayName("Department")]
        [Required]
        public int[] DepartmentId { get; set; }
        public string Department { get; set; }
        public virtual ICollection<DocumentDepartment> DocumentDepartment { get; set; }

    }
}
