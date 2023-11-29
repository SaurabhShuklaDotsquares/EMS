using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EMS.Dto
{
    public class EstimateFormDto
    {
        [DisplayName("Role")]
        //[Required]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        [DisplayName("Technology")]
        public int TechnologyId { get; set; }
        public string TechnologyName { get; set; }
        [DisplayName("Experience")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Experience is required")]
        public int ExperienceId { get; set; }
        public string ExperienceName { get; set; }
        [DisplayName("No. of resources")]
        //[Required]
        public int NoOfResources { get; set; }
        [Required(ErrorMessage = "Please enter numbers only")]
        //[Range(typeof(decimal), "0.1", "999999",ErrorMessage = "Please enter numbers only")]
        //[RegularExpression(@"/^(?!0(\.0*)?$)\d+(\.?\d{0,2})?$/", ErrorMessage = "Please enter numbers only")]
        [DisplayName("Estimate Hour")]
        public decimal EstimateHour { get; set; }
        public decimal Price { get; set; }
        public decimal MinPrice { get; set; }
        public bool? IsSdlc { get; set; }
        public List<SelectListItem> EstimateRoleExpList { get; set; }
        public List<SelectListItem> EstimateRole { get; set; }
        public List<SelectListItem> TechnologyParent { get; set; }

    }
}
