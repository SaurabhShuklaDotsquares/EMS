using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.API.Model
{
    public class LibraryManagementDtoModel
    {
        public List<int> LibraryFileIds { get; set; }
        public List<LibraryDtoAPI> libraries { get; set; } = new List<LibraryDtoAPI>();
        public int totalRecords { get; set; }
        public string LibraryType { get; set; }
        public LibraryManagementSearchFilterDtoAPI libraryManagementSearchFilterDto { get; set; }
    }

    public class LibraryDtoAPI
    {
        public long Id { get; set; }
        public Guid KeyId { get; set; }
        public byte LibraryTypeId { get; set; }
        public string BannerImage { get; set; }
        public bool? IsFeatured { get; set; }
        public string Title { get; set; }
        public string LiveURL { get; set; }
        public string Description { get; set; }
        public string CreatedDate { get; set; }
        public string Version { get; set; }
        public string LibraryTechnologiesComma { get; set; }
        public string LibraryIndustriesComma { get; set; }
        public decimal? IntegrationHours { get; set; }
        public bool? IsNDA { get; set; }
        public List<LibraryFileDtoAPI> LibraryFileDtos { get; set; } = new List<LibraryFileDtoAPI>();
        public int? CRMId { get; set; }
        public List<string> LibraryImagesList { get; set; } = new List<string>();
    }

    public class LibraryFileDtoAPI
    {
        public long Id { get; set; }
        public int? LibraryLayoutTypeId { get; set; }
        public string FilePath { get; set; }
        public LibraryLayoutTypeAPI LibraryLayoutType { get; set; }
    }

    public class LibraryLayoutTypeAPI
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class LibraryManagementSearchFilterDtoAPI
    {
        public Guid? KeyId { get; set; }
    }
}
