using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class SubProject
    {
        public int SubProjectId { get; set; }
        public int ProjectId { get; set; }
        public string SubProjectName { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual Project Project { get; set; }
    }
}
