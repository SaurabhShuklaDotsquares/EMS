using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LibraryLayoutType
    {
        public LibraryLayoutType()
        {
            LibraryLayoutTypeMapping = new HashSet<LibraryLayoutTypeMapping>();
            LibrarySearchLayoutTypeMapping = new HashSet<LibrarySearchLayoutTypeMapping>();
            LibraryFile = new HashSet<LibraryFile>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<LibraryLayoutTypeMapping> LibraryLayoutTypeMapping { get; set; }
        public virtual ICollection<LibrarySearchLayoutTypeMapping> LibrarySearchLayoutTypeMapping { get; set; }
        public virtual ICollection<LibraryFile> LibraryFile { get; set; }
    }
}
