using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class DocumentDepartment
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int DepartmentId { get; set; }

        public virtual Document Document { get; set; }
    }
}
