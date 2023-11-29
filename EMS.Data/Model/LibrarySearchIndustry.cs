using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LibrarySearchIndustry
    {
        public Guid LibrarySearchId { get; set; }
        public int DomainId { get; set; }

        public virtual DomainType Domain { get; set; }
        public virtual LibrarySearch LibrarySearch { get; set; }
    }
}
