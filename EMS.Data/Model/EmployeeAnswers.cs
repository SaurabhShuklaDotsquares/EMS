using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EmployeeAnswers
    {
        public int Id { get; set; }
        public int AppraisalId { get; set; }
        public int UserId { get; set; }
        public int? Qid { get; set; }
        public string Comments { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Ip { get; set; }

        public virtual Appraisal Appraisal { get; set; }
        public virtual Questions Q { get; set; }
    }
}
