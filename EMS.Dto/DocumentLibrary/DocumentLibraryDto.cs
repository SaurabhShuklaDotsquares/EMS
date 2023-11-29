using EMS.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EMS.Dto
{
    public class DocumentLibraryDto
    {
        public int Id { get; set; }
        [DisplayName("Document Title")]
        [Required]
        public string DocumentTitle { get; set; }
        [DisplayName("Document Type")]
        [Required]
        public string DocumentType { get; set; }
        [Required]
        public string Version { get; set; }
        [DisplayName("Upload File")]
        public string FilePath { get; set; }
        public bool? IsActive { get; set; }
        public IFormFile Files { get; set; }
    }
    public class DocumentLibraryFilter
    {
        public string Status { get; set; }
    }
    public class DocumentLibraryIndexDto
    {
        public string IsApprover { get; set; }
        public List<SelectListItem> StatusList { get; set; }

    }
}
