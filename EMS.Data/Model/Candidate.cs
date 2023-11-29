using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Candidate
    {
        public Candidate()
        {
            CadidateExam = new HashSet<CadidateExam>();
        }

        public int CandidateId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Contact { get; set; }
        public bool? IsActive { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string Ip { get; set; }

        public virtual ICollection<CadidateExam> CadidateExam { get; set; }
    }
}
