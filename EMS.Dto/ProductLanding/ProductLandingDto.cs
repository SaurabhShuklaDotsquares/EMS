using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
    public class ProductLandingDto
    {
        public ProductLandingDto()
        {
            Screenshots = new List<ProductLandingScreenshotDto>();
        }

        public int Id { get; set; }

        public string ProductName { get; set; }

        // [AllowHtml] ** no need ot AllowHTML in .net Core https://github.com/aspnet/Mvc/issues/324#issuecomment-50082022**
        public string HighlightText { get; set; }

        // [AllowHtml] ** no need ot AllowHTML in .net Core https://github.com/aspnet/Mvc/issues/324#issuecomment-50082022**
        public string AboutProduct { get; set; }

        // [AllowHtml] ** no need ot AllowHTML in .net Core https://github.com/aspnet/Mvc/issues/324#issuecomment-50082022**
        public string Features { get; set; }

        // [AllowHtml] ** no need ot AllowHTML in .net Core https://github.com/aspnet/Mvc/issues/324#issuecomment-50082022**
        public string Feature1 { get; set; }

        // [AllowHtml] ** no need ot AllowHTML in .net Core https://github.com/aspnet/Mvc/issues/324#issuecomment-50082022**
        public string Feature2 { get; set; }

        // [AllowHtml] ** no need ot AllowHTML in .net Core https://github.com/aspnet/Mvc/issues/324#issuecomment-50082022**
        public string Feature3 { get; set; }

        // [AllowHtml] ** no need ot AllowHTML in .net Core https://github.com/aspnet/Mvc/issues/324#issuecomment-50082022**
        public string Testimonials { get; set; }

        // [AllowHtml] ** no need ot AllowHTML in .net Core https://github.com/aspnet/Mvc/issues/324#issuecomment-50082022**
        public string ServiceDetail { get; set; }

        // [AllowHtml] ** no need ot AllowHTML in .net Core https://github.com/aspnet/Mvc/issues/324#issuecomment-50082022**
        public string TechnologyDetail { get; set; }

        public byte Status { get; set; }
        
        
        public int CurrentUserId { get; set; }
        public int PMUserId { get; set; }
        public bool IsDraft { get; set; }

        public List<ProductLandingScreenshotDto> Screenshots { get; set; }
    }

    public class ProductLandingScreenshotDto
    {
        public long Id { get; set; }
        public IFormFile Screenshot { get; set; }
        public string ScreenshotUrl { get; set; }
        public string Description { get; set; }
    }

    public class ProductLandingIndexDto
    {
        public bool IsMainTeamUser { get; set; }
        public List<SelectListItem> PMUserList { get; set; }

        public List<SelectListItem> ProductLandingPageStatusList { get; set; }

        public ProductLandingIndexDto()
        {
            PMUserList = new List<SelectListItem>();
            ProductLandingPageStatusList = new List<SelectListItem>();
        }
    }
}