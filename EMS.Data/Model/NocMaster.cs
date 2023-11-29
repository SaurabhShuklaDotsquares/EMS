using System;
using System.Collections.Generic;

namespace EMS.Data.Model
{
    public partial class NocMaster
    {
        public NocMaster()
        {
            UserNoc = new HashSet<UserNoc>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool? IsClear { get; set; }
        public virtual ICollection<UserNoc> UserNoc { get; set; }
    }
}
