using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProductLandingScreenshot
    {
        public long Id { get; set; }
        public int ProductLandingId { get; set; }
        public string ScreenshotUrl { get; set; }
        public string Description { get; set; }

        public virtual ProductLanding ProductLanding { get; set; }
    }
}
