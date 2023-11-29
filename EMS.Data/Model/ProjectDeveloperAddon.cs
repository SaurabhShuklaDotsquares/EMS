using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectDeveloperAddon
    {
        public int ProjectId { get; set; }
        public int Uid { get; set; }
        public Guid TransId { get; set; }
        public string Remark { get; set; }
        public int WorkStatus { get; set; }
        public int WorkRole { get; set; }
        public DateTime AddDate { get; set; }
        public string Ip { get; set; }
        public bool IsCurrent { get; set; }

        public virtual Project Project { get; set; }
        public virtual UserLogin U { get; set; }
        public virtual TypeMaster WorkRoleNavigation { get; set; }
        public virtual TypeMaster WorkStatusNavigation { get; set; }
    }
}
