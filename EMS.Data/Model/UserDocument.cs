using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class UserDocument
    {
        public int Id { get; set; }
        public string DocumentPath { get; set; }
        public DateTime AddedDate { get; set; }
        public int? Uid { get; set; }

        public virtual UserLogin U { get; set; }
    }
}
