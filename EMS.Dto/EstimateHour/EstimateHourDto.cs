using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using EMS.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using EMS.Core;
namespace EMS.Dto
{
    public class EstimateHourDto
    {
        public EstimateHourDto()
        {
            BAUsers = new List<SelectListItem>();
            TLUsers = new List<SelectListItem>();

            Technologies = new List<SelectListItem>();
            TechnologyParents = new List<SelectListItem>();

            Domains = new List<SelectListItem>();
        }

        public int Id { get; set; }
        public int DomainId { get; set; }
        public int? TechnologyParentId { get; set; }

        [DisplayName("Requirement Name")]
        public string RequirementName { get; set; }

        [DisplayName("Estimated Hour")]
        public decimal EstimatedHour { get; set; }

        [DisplayName("Is Free Bie")]
        public bool IsFreeBie { get; set; }
        public string OpenBy { get; set; }
        [DisplayName("CRM ID")]
        public int? Crmid { get; set; }

        [DisplayName("File Name")]
        public int? EstimateHourFileNameTypeId { get; set; }
        public int Bauid { get; set; }
        public int Tluid { get; set; }
        [DisplayName("Complexity Level")]
        public byte ComplexityLevel { get; set; }
        public Enums.ComplexityLevel ComplaxityEnum  { get; set; }

        [DisplayName("Requirement Description")]
        public string RequirementDesc { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        [DisplayName("Date Of Conversion")]
        public string ConversionDate { get; set; }

        public List<SelectListItem> FileNames { get; set; }

        [DisplayName("Industry")]
        public List<SelectListItem> Domains { get; set; }


        [DisplayName("BA")]
        public List<SelectListItem> BAUsers { get; set; }
        
        [DisplayName("TL")]
        public List<SelectListItem> TLUsers { get; set; }

        [DisplayName("Technology")]
        public List<SelectListItem> Technologies { get; set; }
        
        [DisplayName("Technology Category")]
        public List<SelectListItem> TechnologyParents { get; set; }

        public string[] Technology { get; set; }
        public string[] TechnologyParent { get; set; }


    }

    public class EstimateHourIndexDto
    {
        public EstimateHourIndexDto()
        {
            BAList = new List<SelectListItem>();
            TLList = new List<SelectListItem>();
            TechnologyList = new List<SelectListItem>();
            IndustryList = new List<SelectListItem>();
            FileNameList = new List<SelectListItem>();
        }

        public int Uid_BA { get; set; }
        public int Uid_TL { get; set; }

        public int TechnologyId { get; set; }
        public int IndustryId { get; set; }
        public string FileName { get; set; }
        
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public List<SelectListItem> FileNameList { get; set; }
        public List<SelectListItem> TechnologyList { get; set; }
        public List<SelectListItem> IndustryList { get; set; }
        public List<SelectListItem> BAList { get; set; }
        public List<SelectListItem> TLList { get; set; }
        //public ProjectionReportWeeksDto projectionReportWeeksDto { get; set; }
    }
}
