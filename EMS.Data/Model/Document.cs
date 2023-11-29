using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Document
    {
        public Document()
        {
            DocumentDepartment = new HashSet<DocumentDepartment>();
        }

        public int Id { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsActive { get; set; }
        public int? UserId { get; set; }
        public bool IsAll { get; set; }

        public virtual ICollection<DocumentDepartment> DocumentDepartment { get; set; }
    }
}
