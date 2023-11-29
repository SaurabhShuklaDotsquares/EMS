using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EmpLateActivity
    {
        public int ElactId { get; set; }
        public int? Uid { get; set; }
        public DateTime ElactDate { get; set; }
        public string ElactTime { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateModify { get; set; }
        public string Ip { get; set; }

        public virtual UserLogin U { get; set; }
    }
}
