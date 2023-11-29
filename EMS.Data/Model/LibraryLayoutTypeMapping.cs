using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LibraryLayoutTypeMapping
    {
        public long LibraryId { get; set; }
        public int LibraryLayoutTypeId { get; set; }

        public virtual Library Library { get; set; }
        public virtual LibraryLayoutType LibraryLayoutType { get; set; }
    }
}
