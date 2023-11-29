using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EmployeeRelativeMedicalData
    {
        public int Id { get; set; }
        public int EmployeeMedicalId { get; set; }
        public byte Relation { get; set; }
        public byte Title { get; set; }
        public string Name { get; set; }
        public byte Gender { get; set; }
        public DateTime Dob { get; set; }
        public DateTime AddedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual EmployeeMedicalData EmployeeMedical { get; set; }
    }
}
