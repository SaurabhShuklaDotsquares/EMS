using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EmployeeActivity
    {
        public int ActivityId { get; set; }
        public int Uid { get; set; }
        public int EmployeeUid { get; set; }
        public int? AppraisalId { get; set; }
        public int? Type { get; set; }
        public int? TypeId { get; set; }
        public int? TypeAns { get; set; }
        public string Comments { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Ip { get; set; }
    }
}
