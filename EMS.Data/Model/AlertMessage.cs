using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class AlertMessage
    {
        public AlertMessage()
        {
            AlertMessageRead = new HashSet<AlertMessageRead>();
        }

        public int AlertId { get; set; }
        public int Uid { get; set; }
        public string Description { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual UserLogin U { get; set; }
        public virtual ICollection<AlertMessageRead> AlertMessageRead { get; set; }
    }
}
