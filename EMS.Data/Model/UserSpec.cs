using System;
using System.Collections.Generic;

namespace EMS.Data.Model
{
    public partial class UserSpec
    {
        public int Id { get; set; }
        public int TechId { get; set; }
        public int Uid { get; set; }

        public virtual Technology Tech { get; set; }
        public virtual UserLogin U { get; set; }
    }
}
