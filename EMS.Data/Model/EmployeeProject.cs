using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EmployeeProject
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? AppraisalId { get; set; }
        public int? Year { get; set; }
        public DateTime? PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }
        public int? ProjectId { get; set; }
        public int? Role { get; set; }
        public string Comments { get; set; }
        public string Tlcomments { get; set; }
        public string Tlstatus { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Ip { get; set; }

        public virtual EmployeeProject IdNavigation { get; set; }
        public virtual Project Project { get; set; }
        public virtual Role RoleNavigation { get; set; }
        public virtual EmployeeProject InverseIdNavigation { get; set; }
    }
}
