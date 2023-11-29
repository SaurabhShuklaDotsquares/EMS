using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LibraryTemplate
    {
        public long LibraryId { get; set; }
        public int LibraryTemplateTypeId { get; set; }
        public virtual Library Library { get; set; }
        public virtual LibraryTemplateType LibraryTemplateType { get; set; }
    }
}
