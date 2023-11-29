using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LibrarySearchComponent
    {
        public Guid LibrarySearchId { get; set; }
        public int ComponentTypeId { get; set; }

        public virtual LibraryComponentType ComponentType { get; set; }
        public virtual LibrarySearch LibrarySearch { get; set; }
    }
}
