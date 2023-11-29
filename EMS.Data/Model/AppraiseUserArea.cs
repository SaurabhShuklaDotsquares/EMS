using EMS.Data;
using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class AppraiseUserArea
    {
        public int Id { get; set; }
        public int AppraiseId { get; set; }
        public int ComplaintAppraiseId { get; set; }

        public virtual EmployeeAppraise Appraise { get; set; }
        public virtual ComplaintAppraiseArea ComplaintAppraise { get; set; }
    }
}
