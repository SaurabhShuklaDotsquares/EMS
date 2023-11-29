using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EMS.Dto.CVEstimatePrice
{
    public class CVEstimatePriceDto
    {
        [Required(ErrorMessage = "Please select category")]
        public int RoleCateGoryId { get; set; }
        public int? TechnologyId { get; set; }

       
        public List<DropdownListDto> RoleList { get; set; }
        public List<DropdownListDto> roleCategoryList { get; set; }
        public List<DropdownListDto> technologyList { get; set; }
        public List<UserTechnologyDto> TechnologyList { get; set; }
        public List<SelectListItem> Technologies { get; set; }
        //public int[] Technologies { get; set; }
        public decimal EntryLevelPrice { get; set; }
        public decimal OneToTwoPrice { get; set; }
        public decimal ThreeToSixPrice { get; set; }
        public decimal SixToTenPrice { get; set; }
        public decimal TenPlusPrice { get; set; }

        public int Id { get; set; }
        public int? RoleId { get; set; }
        public int? ExpId { get; set; }
        public int? TechId { get; set; }
        public decimal? Price { get; set; }
        public bool IsActive { get; set; }
        public string USD { get; set; }
        public string AUD { get; set; }
        public string AED { get; set; }
    }
    public class CVEstimatePriceResponseDto
    {
        public string TechName { get; set; }
        public int? TechnologyId { get; set; }
        public int? RoleId { get; set; }
        public decimal EntryLevel { get; set; }
        public decimal onetoTwo { get; set; }
        public decimal threetoSix { get; set; }
        public decimal sixtoTen { get; set; }
        public decimal TenPlus { get; set; }
        
    }
}
