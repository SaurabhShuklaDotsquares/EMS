using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class OrgDocumentRole
    {
        public int OrgDocumentId { get; set; }
        public int RoleId { get; set; }

        public virtual OrgDocument OrgDocument { get; set; }
        public virtual Role Role { get; set; }
    }
}
