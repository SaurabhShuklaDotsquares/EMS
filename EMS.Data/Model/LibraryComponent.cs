using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LibraryComponent
    {
        public long LibraryId { get; set; }
        public int LibraryComponentTypeId { get; set; }
        public virtual Library Library { get; set; }
        public virtual LibraryComponentType LibraryComponentType { get; set; }
    }
}
