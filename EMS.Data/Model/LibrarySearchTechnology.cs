using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LibrarySearchTechnology
    {
        public Guid LibrarySearchId { get; set; }
        public int? TechnologyId { get; set; }

        public virtual LibrarySearch LibrarySearch { get; set; }
        public virtual Technology Technology { get; set; }
    }
}
