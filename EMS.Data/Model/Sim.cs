using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Sim
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Network { get; set; }
        public byte Status { get; set; }
        public bool IsActive { get; set; }
        public int CreateByUid { get; set; }
        public int ModifyByUid { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }

        public virtual UserLogin CreateByU { get; set; }
        public virtual UserLogin ModifyByU { get; set; }
    }
}
