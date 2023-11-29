using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectDeveloper
    {
        public int ProjectId { get; set; }
        public int? Uid { get; set; }
        public Guid TransId { get; set; }
        public string Remark { get; set; }
        public int WorkStatus { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string IP { get; set; }
        public int? VD_id { get; set; }
        public int ProjectDeveloperId { get; set; }

        public virtual Project Project { get; set; }
        public virtual UserLogin UserLogin { get; set; }
        public virtual VirtualDeveloper VirtualDeveloper { get; set; }
        public virtual TypeMaster WorkStatusNavigation { get; set; }
    }
}
