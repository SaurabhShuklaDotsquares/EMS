using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
   public class EstimatePriceDto
    {
        public int Id { get; set; }
        public int EstimateRoleExpId { get; set; }
        public int EstimateRoleId { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        
        public List<SelectListItem> EstimateRoleExpList { get; set; }
        public List<SelectListItem> TecnologyParentList { get; set; }
        public List<SelectListItem> EstimateRoleList { get; set; }
        public List<EstimateExpPriceDto> EstimateExpPriceDtoList { get; set; }
        public int? EstimateTechnologyId { get; set; }
    }

}
