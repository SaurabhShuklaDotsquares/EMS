using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Data
{
    public class CvsType
    {
        public CvsType()
        {
            Library = new HashSet<Library>();
        }
        public int CvsId { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public int? DisplayOrder { get; set; }

        public virtual ICollection<Library> Library { get; set; }

    }
}
