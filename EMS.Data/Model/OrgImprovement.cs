using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class OrgImprovement
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public byte TypeId { get; set; }
        public DateTime ImprovementDate { get; set; }
        public string Description { get; set; }
        public DateTime AddDate { get; set; }
        public int AddedByUid { get; set; }
        public DateTime ModifyDate { get; set; }
        public int EmployeeUid { get; set; }

        public virtual UserLogin AddedByU { get; set; }
        public virtual UserLogin EmployeeU { get; set; }
    }
}
