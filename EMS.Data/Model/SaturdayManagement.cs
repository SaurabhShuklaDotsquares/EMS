using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class SaturdayManagement
    {
        public int Id { get; set; }
        public int? Uid { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public DateTime? SaturdayDt { get; set; }
        public bool Ispresent { get; set; }
        public DateTime? AddedOn { get; set; }
        public DateTime? UpdatedDt { get; set; }
        public DateTime? LastProcessDt { get; set; }

        public virtual UserLogin U { get; set; }
    }
}
