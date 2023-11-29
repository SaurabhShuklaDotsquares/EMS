using EMS.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EMS.Dto.LibraryManagement
{
    public class SalesKitTypeDto
    {
        public int SalesKitId { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        [Display(Name = "Display Order")]
        public int? DisplayOrder { get; set; }
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }
        [Display(Name="Is Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Is Child Category")]
        public bool IsChild { get; set; }
        public List<SelectListItem> ParentSalesKit { get; set; }
    }
}
