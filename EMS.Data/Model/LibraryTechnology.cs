using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LibraryTechnology
    {
        public long LibraryId { get; set; }
        public int TechnologyId { get; set; }

        public virtual Library Library { get; set; }
        public virtual Technology Technology { get; set; }
    }
}
