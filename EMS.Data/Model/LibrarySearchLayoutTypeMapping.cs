using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LibrarySearchLayoutTypeMapping
    {
        public Guid LibrarySearchId { get; set; }
        public int LayoutTypeId { get; set; }

        public virtual LibraryLayoutType LayoutType { get; set; }
        public virtual LibrarySearch LibrarySearch { get; set; }
    }
}
