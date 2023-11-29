using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectAdditionalSupport
    {       
        public ProjectAdditionalSupport()
        {
            ProjectAdditionalSupportUser = new HashSet<ProjectAdditionalSupportUser>();
        }

        public int Id { get; set; }
        public int ProjectId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public int RequestByUid { get; set; }
        public int? ApproveByUid { get; set; }
        public string ApprovalComment { get; set; }
        public byte Status { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public int? TLId { get; set; }
        public virtual UserLogin UserLogin { get; set; }
        public virtual Project Project { get; set; }
        public virtual UserLogin UserLogin1 { get; set; }
        public virtual UserLogin UserLogin2 { get; set; }
        public virtual ICollection<ProjectAdditionalSupportUser> ProjectAdditionalSupportUser { get; set; }        
    }
}
