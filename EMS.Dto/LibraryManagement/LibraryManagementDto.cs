using System;
using System.Collections.Generic;
using System.Text;
using EMS.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using static EMS.Core.Enums;

namespace EMS.Dto
{
    public class LibraryManagementDto
    {
        public LibraryManagementDto()
        {
            libraries = new List<LibraryDto>();
            //Industries = new List<DomainTypeDto>();
            //Layouts = new List<LibraryLayoutTypeDto>();
            libraryManagementSelectedItemsDtos = new List<LibrarySearchSelectedItemsDto>();
        }
        public List<int> LibraryFileIds { get; set; }
        public List<LibraryDto> libraries { get; set; }
        public int totalRecords { get; set; }
        public string LibraryType { get; set; }
        //public List<DomainTypeDto> Industries { get; set; }
        //public List<LibraryLayoutTypeDto> Layouts { get; set; }

        //public List<TechnologyDto> technologies { get; set; }

        public LibraryManagementSearchFilterDto libraryManagementSearchFilterDto { get; set; }
        public List<LibrarySearchSelectedItemsDto> libraryManagementSelectedItemsDtos { get; set; }

    }

    public class LibraryManagementSearchFilterDto
    {
        public string SearchText { get; set; }
        public LibraryType LibraryType { get; set; }
        public DesignType? DesignType { get; set; }

        public int? CvsTypeId { get; set; }
        public int? SalesKitTypeId { get; set; }

        public int[] Technologies { get; set; }
        public int[] Domains { get; set; }
        public bool IsAdvanceSearch { get; set; }
        public bool? IsNDA { get; set; }
        public bool? Featured { get; set; }
        public bool? IsReadyToUse { get; set; }

        public Guid? KeyId { get; set; }
        public int[] Layouts { get; set; }
        public int[] Components { get; set; }
        public int[] Templates { get; set; }
        public bool LibrarySearchExist { get; set; }
        public int PageStart { get; set; }
        public int DataLength { get; set; }

        public string DesignTypeLabel { get; set; }
        public string SalesKitTypeLabel { get; set; }
        public string CvsTypeLabel { get; set; }

        //used to get Id so it can be used while removing items from filter
        public string DesignTypeFilterId { get; set; }
        public string SalesKitTypeFilterId { get; set; }
        public string CvsTypeFilterId { get; set; }


        public string LayoutLabel { get; set; }
        public string LayoutFilterId { get; set; }
        public string TechnologyLabel { get; set; }
        public string TechnologyFilterId { get; set; }
        public string ComponentLabel { get; set; }
        public string ComponentFilterId { get; set; }
        public string TemplateLabel { get; set; }
        public string TemplateFilterId { get; set; }
        public string IndustryLabel { get; set; }
        public string IndustryFilterId { get; set; }
        public string NDAStatusLabel { get; set; }
        public string NDAStatusFilterId { get; set; }
        public string FeaturedStatusLabel { get; set; }
        public string FeaturedStatusFilterId { get; set; }
        public string IsReadyToUseStatusLabel { get; set; }
        public string IsReadyToUseStatusFilterId { get; set; }
        public List<SelectListItem> ComponentsSelected { get; set; }
        public List<SelectListItem> LayoutsSelected { get; set; }
        public string LibraryTypeLabel { get; set; }
        //used to get Id so it can be used while removing items from filter
        public string LibraryTypeFilterId { get; set; }

    }
    public class LibrarySearchSelectedItemsDto
    {
        public LibrarySearchSelectedItemsDto()
        {
            SelectListItems = new List<SelectListItem>();
        }
        public string FilterId { get; set; }
        public string Label { get; set; }
        public int Order { get; set; }

        public List<SelectListItem> SelectListItems { get; set; }
        public string SelectRadioItems { get; set; }
    }
}
