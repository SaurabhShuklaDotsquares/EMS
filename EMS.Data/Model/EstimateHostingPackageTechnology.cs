using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EstimateHostingPackageTechnology
    {
        public int EstimateHostingPackageId { get; set; }
        public int EstimateTechnologyId { get; set; }

        public virtual EstimateHostingPackage EstimateHostingPackage { get; set; }
        public virtual EstimateTechnology EstimateTechnology { get; set; }
    }
}
