using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using EMS.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
    public class LibraryDto
    {
        public LibraryDto()
        {
            SalesKitTypes = new List<SelectListItem>();
            CvsTypes = new List<SelectListItem>();

            LibraryFileTypes = new List<string>();
            LibraryComponentFileTypes = new List<string>();
            LibraryTypes = new List<SelectListItem>();
            LayoutTypes = new List<SelectListItem>();
            DesignTypes = new List<SelectListItem>();
            Industries = new List<SelectListItem>();
            Technologies = new List<SelectListItem>();
            TechnologyParents = new List<SelectListItem>();
            LibraryComponentTypes = new List<SelectListItem>();
            LibraryFileList = new List<LibraryFile>();
            LibraryComponentFileList = new List<LibraryComponentFile>();
            ImageFiles = new List<string>();
            PSDFiles = new List<string>();
            LibraryFileDtos = new List<LibraryFileDto>();
            LibraryComponentFileDtos = new List<LibraryComponentFileDto>();
            DesignFiles = new List<DesignFile>();
            LibraryTemplateTypes = new List<SelectListItem>();
        }

        [DisplayName("Sales Kit Type")]
        public List<SelectListItem> SalesKitTypes { get; set; }
        [DisplayName("CVs Type")]
        public List<SelectListItem> CvsTypes { get; set; }

        public int? SalesKitId { get; set; }
        public int? CvsId { get; set; }

        public List<string> LibraryFileTypes { get; set; }
        public List<string> LibraryComponentFileTypes { get; set; }
        public List<LibraryFile> LibraryFileList { get; set; }
        public List<LibraryComponentFile> LibraryComponentFileList { get; set; }
        [DisplayName("Type")]
        public List<SelectListItem> LibraryTypes { get; set; }
        [DisplayName("Design Type")]
        public List<SelectListItem> DesignTypes { get; set; }
        [DisplayName("Layout Type")]
        public List<SelectListItem> LayoutTypes { get; set; }
        [DisplayName("Layout Type")]
        public List<SelectListItem> DesignLayoutTypes { get; set; }
        [DisplayName("Industry")]
        public List<SelectListItem> Industries { get; set; }
        [DisplayName("Technology")]
        public List<SelectListItem> Technologies { get; set; }
        [DisplayName("Technology Category")]
        public List<SelectListItem> TechnologyParents { get; set; }
        [DisplayName("Component Type")]
        public List<SelectListItem> LibraryComponentTypes { get; set; }
        [DisplayName("Template Type")]
        public List<SelectListItem> LibraryTemplateTypes { get; set; }
        //Holds psd
        public List<string> PSDFiles { get; set; }
        //Holds image
        public List<string> ImageFiles { get; set; }
        //for holding Non image files
        public List<LibraryFileDto> LibraryFileDtos { get; set; }
        public List<LibraryComponentFileDto> LibraryComponentFileDtos { get; set; }
        //public List<string> OtherThanImageFiles { get; set; }
        [DisplayName("Author")]
        public List<SelectListItem> Users { get; set; }
        [DisplayName("BA")]
        public List<SelectListItem> BAUsers { get; set; }
        [DisplayName("TL")]
        public List<SelectListItem> TLUsers { get; set; }

        public long Id { get; set; }
        public Guid KeyId { get; set; }

        [DisplayName("CRM ID")]
        public int? CRMUserId { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }
        public string BannerImage { get; set; }
        public string IP { get; set; }
        public string ModifyDate { get; set; }
        public string AddDate { get; set; }
        public int AddedBy { get; set; }
        [DisplayName("Search Keywords")]
        public string keywords { get; set; }
        public string CreatedDate { get; set; }
        public int LibraryFileTypeId { get; set; }
        [Display(Name = "Type")]
        public byte LibraryTypeId { get; set; }
        [Required]
        [Display(Name = "Layout Type")]
        public int LayoutTypeId { get; set; }
        public int IndustryId { get; set; }
        public byte? DesignTypeId { get; set; }
        public int TechnologyId { get; set; }
        public string[] LibraryFileType { get; set; }
        public string[] LayoutType { get; set; }
        public string[] Industry { get; set; }
        public string[] Technology { get; set; }
        public string[] TechnologyParent { get; set; }
        public string[] LibraryComponent { get; set; }
        public string[] LibraryTemplate { get; set; }
        [DisplayName("Is Featured?")]
        [Display(Name = "Is Featured")]
        [Required]
        public bool? IsFeatured { get; set; }
        [DisplayName("Is Live?")]
        [Display(Name = "Is Live")]
        public bool? IsLive { get; set; }
        [DisplayName("NDA Signed")]
        [Display(Name = "NDA Signed")]
        public bool? IsNDA { get; set; }
        [DisplayName("Team")]
        [Display(Name = "Team")]
        public bool? Team { get; set; }
        [Display(Name = "Live URL")]
        public string LiveURL { get; set; }
        [DisplayName("Upload File")]
        public string Image { get; set; }
        [DisplayName("Upload Description Document")]
        public string UploadDescription { get; set; }
        [DisplayName("Upload PSD")]
        public IFormFile PSD { get; set; }
        public List<IFormFile> LibraryFiles { get; set; }
        public int? CoverImage { get; set; }
        public List<DesignFile> DesignFiles { get; set; }
        public string OtherIndustry { get; set; }
        public string OtherTechnologyParent { get; set; }
        public string OtherTechnology { get; set; }
        public int? AuthorUid { get; set; }
        public string Author { get; set; }
        public int? UidBA { get; set; }
        public int? UidTL { get; set; }
        [DisplayName("Good as Portfolio")]
        public bool? IsGoodToShow { get; set; }
        public string BAName { get; set; }
        public string TLName { get; set; }
        public string EMSLibraryId { get; set; }
        public string LibraryTechnologiesComma { get; set; }
        public string LibraryIndustriesComma { get; set; }
        public string Version { get; set; }
        public bool IsActive { get; set; }
        public bool IsReadyToUse { get; set; }
        [DisplayName("Integration Hours")]
        public decimal? IntegrationHours { get; set; }
        [DisplayName("Re-Development Hours")]
        public decimal? ReDevelopmentHours { get; set; }
        [DisplayName("Estimated Hours")]
        public decimal? EstimatedHours { get; set; }
        public IFormFile ComponentFiles { get; set; }
        public List<string> LibraryFilesList { get; set; }
    }

    public class DesignFile
    {
        public int DesignLayoutType { get; set; }
        public bool DesignUnitType { get; set; }
        public FormFileWrapper Image { get; set; }
        public FormFileWrapper PSD { get; set; }
    }
    public class FormFileWrapper
    {
        public IFormFile File { get; set; }
    }
}
