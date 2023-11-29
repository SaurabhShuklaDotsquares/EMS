using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LibrarySearch
    {
        public LibrarySearch()
        {
            LibrarySearchIndustry = new HashSet<LibrarySearchIndustry>();
            LibrarySearchTechnology = new HashSet<LibrarySearchTechnology>();
            LibrarySearchLayoutTypeMapping = new HashSet<LibrarySearchLayoutTypeMapping>();
            LibrarySearchComponent = new HashSet<LibrarySearchComponent>();
        }

        public Guid Id { get; set; }
        public string Keyword { get; set; }
        public DateTime SearchDate { get; set; }
        public string Ip { get; set; }
        public bool? IsNda { get; set; }
        public bool? IsFeatured { get; set; }
        public bool? IsReadyToUse { get; set; }
        public bool IsAdvanceFiltered { get; set; }
        public byte LibraryTypeId { get; set; }
        public Guid? KeyId { get; set; }
        public byte? DesignTypeId { get; set; }

        public virtual Library Key { get; set; }

        public virtual ICollection<LibrarySearchIndustry> LibrarySearchIndustry { get; set; }
        public virtual ICollection<LibrarySearchTechnology> LibrarySearchTechnology { get; set; }
        public virtual ICollection<LibrarySearchLayoutTypeMapping> LibrarySearchLayoutTypeMapping { get; set; }
        public virtual ICollection<LibrarySearchComponent> LibrarySearchComponent { get; set; }
    }
}
