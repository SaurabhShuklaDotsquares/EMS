using EMS.Core;
using EMS.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using static EMS.Core.Enums;

namespace EMS.API.Model
{
    public class LibraryManagementSearchFilterDtoModel
    {
        public string SearchText { get; set; }
        private LibraryType? _LibraryType;
        public LibraryType? LibraryType
        {
            get => _LibraryType == 0 ? null : _LibraryType;
            set => _LibraryType = value;            
        }
        public DesignType? DesignType { get; set; }

        public int[] Technologies { get; set; }
        public int[] Domains { get; set; }
        public bool? IsAdvanceSearch { get; set; }
        public bool? IsNDA { get; set; }
        public bool? Featured { get; set; }
        public bool? IsReadyToUse { get; set; }

        public Guid? KeyId { get; set; }
        public int[] Layouts { get; set; }
        public int[] Components { get; set; }
        public int[] Templates { get; set; }
        public bool LibrarySearchExist { get; set; }
        private int? _PageStart;
        public int? PageStart
        {
            get => _PageStart == null ? 0 : _PageStart;
            set => _PageStart = value;            
        }
        private int? _DataLength;
        public int? DataLength
        {
            get => _DataLength == null || _DataLength == 0 ? 200 : _DataLength;
            set => _DataLength = value;            
        }
        public string DesignTypeLabel { get; set; }
        //used to get Id so it can be used while removing items from filter
        public string DesignTypeFilterId { get; set; }
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
        public string CRMId { get; set; }
    }
}
