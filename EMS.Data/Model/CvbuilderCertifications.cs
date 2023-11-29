using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class CvbuilderCertifications
    {
        public long Id { get; set; }
        public long CvbuilderId { get; set; }
        public string CertificationName { get; set; }
        public string CertificationNumber { get; set; }
        public string CertificationsImage { get; set; }

        public virtual Cvbuilder Cvbuilder { get; set; }
    }
}
