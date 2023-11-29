using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class User_Tech
    {
        public int Id { get; set; }
        public int TechId { get; set; }
        public int Uid { get; set; }
        public byte? SpecTypeId { get; set; }

        public virtual Technology Technology { get; set; }
        public virtual UserLogin UserLogin { get; set; }
    }
}
