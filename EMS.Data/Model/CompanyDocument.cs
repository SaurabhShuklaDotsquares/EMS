using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class CompanyDocument
    {
        public int Id { get; set; }
        public string Heading { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public DateTime Modified { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
    }
}
