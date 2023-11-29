using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Data.Model
{
    public partial class ComplaintUser
    {
        public int UserLoginId { get; set; }
        public int ComplaintId { get; set; }

        public virtual Complaint Complaint { get; set; }
        public virtual UserLogin UserLogin { get; set; }
    }
}
