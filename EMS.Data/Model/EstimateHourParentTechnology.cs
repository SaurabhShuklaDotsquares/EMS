using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EstimateHourParentTechnology
    {
        public int EstimateHourId { get; set; }
        public int TechnologyParentId { get; set; }

        public virtual EstimateHour EstimateHour { get; set; }
        public virtual TechnologyParent TechnologyParent { get; set; }
    }
}
