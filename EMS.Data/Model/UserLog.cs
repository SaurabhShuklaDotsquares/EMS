using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class UserLog
    {
        public int LogId { get; set; }
        public int? Uid { get; set; }
        public DateTime? Login { get; set; }
        public DateTime? Logout { get; set; }
        public string Ip { get; set; }

        public virtual UserLogin U { get; set; }
    }
}
