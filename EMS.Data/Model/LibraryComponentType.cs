using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LibraryComponentType
    {
        public LibraryComponentType()
        {
            LibraryComponent = new HashSet<LibraryComponent>();
            LibrarySearchComponent = new HashSet<LibrarySearchComponent>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<LibraryComponent> LibraryComponent { get; set; }
        public virtual ICollection<LibrarySearchComponent> LibrarySearchComponent { get; set; }
    }
}
