using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Vaccine
    {
        public Vaccine()
        {
            VaccinationStatus = new HashSet<VaccinationStatus>();
        }
        public int Id { get; set; }
        public string VaccineName { get; set; }
        public bool IsActive { get; set; }
        public DateTime AddedOn { get; set; }
        public virtual ICollection<VaccinationStatus> VaccinationStatus { get; set; }
    }
}
