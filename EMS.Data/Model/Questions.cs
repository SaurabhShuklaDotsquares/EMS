using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Questions
    {
        public Questions()
        {
            EmployeeAnswers = new HashSet<EmployeeAnswers>();
        }

        public int Qid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Ip { get; set; }

        public virtual ICollection<EmployeeAnswers> EmployeeAnswers { get; set; }
    }
}
