using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EstimateHourTechnology
    {
        public int EstimateHourId { get; set; }
        public int TechnologyId { get; set; }
        public virtual EstimateHour EstimateHour { get; set; }
    }
}
