using System;
using System.Collections.Generic;

namespace EMS.Data.Model
{
    public partial class Certificate
    {
        public Certificate()
        {
            CertificationByEmployee = new HashSet<CertificationByEmployee>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string CertificateCode { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual ICollection<CertificationByEmployee> CertificationByEmployee { get; set; }
    }
}
