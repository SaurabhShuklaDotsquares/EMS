using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LibraryTemplateType
    {
        public LibraryTemplateType()
        {
            LibraryTemplate = new HashSet<LibraryTemplate>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<LibraryTemplate> LibraryTemplate { get; set; }
    }
}
