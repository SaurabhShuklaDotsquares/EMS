using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EmployeeComplaint
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? EmployeeId { get; set; }
        public string ClientComment { get; set; }
        public DateTime? ClientDate { get; set; }
        public string EmpComment { get; set; }
        public DateTime? EmpDate { get; set; }
        public string TlComment { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Ip { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? Tldate { get; set; }
        public int? ComplainType { get; set; }
        public string Priority { get; set; }

        public virtual UserLogin Employee { get; set; }
        public virtual UserLogin User { get; set; }
    }
}
