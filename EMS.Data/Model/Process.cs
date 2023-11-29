using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Process
    {
        public Process()
        {
            Pilog = new HashSet<PILog>();
        }

        public int Id { get; set; }
        public string ProcessName { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<PILog> Pilog { get; set; }
    }
}
