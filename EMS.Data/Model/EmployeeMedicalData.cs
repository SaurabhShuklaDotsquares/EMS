using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EmployeeMedicalData
    {
        public EmployeeMedicalData()
        {
            EmployeeRelativeMedicalDatas = new HashSet<EmployeeRelativeMedicalData>();
        }

        public int Id { get; set; }
        public string EmployeeCode { get; set; }
        public byte Title { get; set; }
        public string Name { get; set; }
        public byte Gender { get; set; }
        public string Designation { get; set; }
        public DateTime Dob { get; set; }
        public DateTime AddedDate { get; set; }
        public bool IsActive { get; set; }
        public int UserId { get; set; }
        public bool? ShowRelative { get; set; }
        public decimal? PremiumTotal { get; set; }
        public decimal? PremiumPerMonth { get; set; }
        public int? TotalCoverage { get; set; }
        public int? Validity { get; set; }
        public virtual UserLogin UserLogin { get; set; }
        public virtual ICollection<EmployeeRelativeMedicalData> EmployeeRelativeMedicalDatas { get; set; }
    }
}
