using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class VaccinationStatus
    {
        public int Id { get; set; }
        public int? Uid { get; set; }
        public int? VaccinationTypeId { get; set; }
        public DateTime? VaccinationDose1Date { get; set; }
        public DateTime? VaccinationDose2Date { get; set; }
        public string UpdatedCertificate { get; set; }
        public DateTime? AddedDate { get; set; }
        public bool? IsActive { get; set; }
        public byte? VaccinatedTypeId { get; set; }
        public virtual UserLogin U { get; set; }
        public virtual Vaccine VaccinationType { get; set; }
        public string DeclarationName { get; set; }
    }
}
