using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class OrgDocumentDepartment
    {
        public int OrgDocumentId { get; set; }
        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual OrgDocument OrgDocument { get; set; }
    }
}
