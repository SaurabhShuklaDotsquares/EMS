using System;
using System.Collections.Generic;

namespace EMS.Data.Model
{
    public partial class CertificationByEmployee
    {
        public int Id { get; set; }
        public int AddedBy { get; set; }
        public int CertificateId { get; set; }
        public bool IsCertificateUploaded { get; set; }
        public string CertificateFile { get; set; }
        public bool? IsInvoiceUploaded { get; set; }
        public bool? Tcaccepted { get; set; }
        public DateTime? DateOfCommitment { get; set; }
        public DateTime DateOfExam { get; set; }
        public decimal InvoiceAmount { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public byte? CertificationStatus { get; set; }

        public virtual UserLogin AddedByNavigation { get; set; }
        public virtual Certificate Certificate { get; set; }
    }
}
