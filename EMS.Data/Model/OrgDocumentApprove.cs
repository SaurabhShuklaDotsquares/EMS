using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class OrgDocumentApprove
    {
        public int OrgDocumentId { get; set; }
        public int ApproverUid { get; set; }
        public string Comments { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public virtual UserLogin UserLogin { get; set; }
        public virtual OrgDocument OrgDocument { get; set; }
    }
}
