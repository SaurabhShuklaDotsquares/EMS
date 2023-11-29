using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ComplainUserArea
    {
        public int Id { get; set; }
        public int ComplaintId { get; set; }
        public int ComplaintAppraiseId { get; set; }

        public virtual Complaint Complaint { get; set; }
        public virtual ComplaintAppraiseArea ComplaintAppraise { get; set; }
    }
}
