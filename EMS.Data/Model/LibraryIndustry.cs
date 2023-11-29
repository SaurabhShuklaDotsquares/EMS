using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LibraryIndustry
    {
        public long LibraryId { get; set; }
        public int IndustryId { get; set; }

        public virtual DomainType Industry { get; set; }
        public virtual Library Library { get; set; }
    }
}
