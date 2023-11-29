using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public class SalesKitType
    {
        public SalesKitType()
        {
            Library = new HashSet<Library>();
        }
        public int SalesKitId { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public string DisplayName { get; set; }
        public bool? IsActive { get; set; }
        public int? DisplayOrder { get; set; }

        public virtual ICollection<Library> Library { get; set; }
    }
}
