using System;
using System.Collections.Generic;

namespace EMS.Data.model
{
    public partial class ProjectHandoverDetails
    {
        public long Id { get; set; }
        public int Uid { get; set; }
        public int ProjectId { get; set; }
        public string FilePath { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual Project Project { get; set; }
        public virtual UserLogin U { get; set; }
    }
}
