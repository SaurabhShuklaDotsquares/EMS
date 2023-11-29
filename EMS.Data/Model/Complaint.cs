using EMS.Data.Model;
using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Complaint
    {
        public Complaint()
        {
            ComplaintUser = new HashSet<ComplaintUser>();
        }

        public int Id { get; set; }
        public byte ComplaintType { get; set; }
        public byte Priority { get; set; }
        public DateTime? TlComplainDate { get; set; }
        public string TlExplanation { get; set; }
        public string ClientComplain { get; set; }
        public DateTime? ClientComplainDate { get; set; }
        public string DeveloperExplanation { get; set; }
        public DateTime? DeveloperComplainDate { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public bool IsDelete { get; set; }
        public int AddedBy { get; set; }
        public string AreaofImprovement { get; set; }
        public string LessionLearned { get; set; }
        public int? ProjectId { get; set; }
        public virtual UserLogin AddedByNavigation { get; set; }
        public virtual ICollection<ComplaintUser> ComplaintUser { get; set; }
    }
}
