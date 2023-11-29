using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Appraisal
    {
        public Appraisal()
        {
            AppraisalExtras = new HashSet<AppraisalExtras>();
            EmployeeAnswers = new HashSet<EmployeeAnswers>();
            FullLeave = new HashSet<FullLeave>();
            HalfLeave = new HashSet<HalfLeave>();
        }

        public int AppraisalId { get; set; }
        public int Uid { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public bool? IsCommit { get; set; }
        public bool? IsCommitTl { get; set; }
        public DateTime? AddDate { get; set; }
        public string Ip { get; set; }

        public virtual UserLogin U { get; set; }
        public virtual ICollection<AppraisalExtras> AppraisalExtras { get; set; }
        public virtual ICollection<EmployeeAnswers> EmployeeAnswers { get; set; }
        public virtual ICollection<FullLeave> FullLeave { get; set; }
        public virtual ICollection<HalfLeave> HalfLeave { get; set; }
    }
}
