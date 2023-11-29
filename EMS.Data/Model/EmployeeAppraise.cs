using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EmployeeAppraise
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? EmployeeId { get; set; }
        public string ClientComment { get; set; }
        public DateTime? ClientDate { get; set; }
        public string TlComment { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string IP { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public int? AppraiseType { get; set; }
        public int? ProjectId { get; set; }
        public byte? Priority { get; set; }
        public virtual UserLogin UserLogin1 { get; set; }
        public virtual UserLogin UserLogin { get; set; }
    }
}
