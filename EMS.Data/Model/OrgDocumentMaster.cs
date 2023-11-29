using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class OrgDocumentMaster
    {
        public OrgDocumentMaster()
        {
            OrgDocuments = new HashSet<OrgDocument>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public byte DocType { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<OrgDocument> OrgDocuments { get; set; }
    }
}
