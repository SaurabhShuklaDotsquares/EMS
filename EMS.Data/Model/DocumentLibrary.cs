using System;
using System.Collections.Generic;

namespace EMS.Data.Model
{
    public partial class DocumentLibrary
    {
        public int Id { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentType { get; set; }
        public string Version { get; set; }
        public string FilePath { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
