using System;
using System.Collections.Generic;

namespace EMS.Data.Model

{
    public partial class UserNoc
    {
        public int Id { get; set; }
        public int Uid { get; set; }
        public int Nocid { get; set; }
        public bool? Value { get; set; }

        public virtual NocMaster Noc { get; set; }
        public virtual UserLogin U { get; set; }
    }
}
