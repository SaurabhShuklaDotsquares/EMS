using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class AlertMessageRead
    {
        public int AlertReadId { get; set; }
        public int AlertId { get; set; }
        public int Uid { get; set; }
        public DateTime? AddDate { get; set; }
        public bool? IsRead { get; set; }

        public virtual AlertMessage Alert { get; set; }
        public virtual UserLogin U { get; set; }
    }
}
