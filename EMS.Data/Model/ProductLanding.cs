using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProductLanding
    {
        public ProductLanding()
        {
            ProductLandingScreenshots = new HashSet<ProductLandingScreenshot>();
        }

        public int Id { get; set; }
        public string ProductName { get; set; }
        public string HighlightText { get; set; }
        public string AboutProduct { get; set; }
        public string Features { get; set; }
        public string Feature1 { get; set; }
        public string Feature2 { get; set; }
        public string Feature3 { get; set; }
        public string ServiceDetail { get; set; }
        public string TechnologyDetail { get; set; }
        public string Testimonials { get; set; }
        public byte Status { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateByUid { get; set; }
        public DateTime ModifyDate { get; set; }
        public int ModifyByUid { get; set; }

        public virtual UserLogin UserLogin { get; set; }
        public virtual UserLogin UserLogin1 { get; set; }
        public virtual ICollection<ProductLandingScreenshot> ProductLandingScreenshots { get; set; }
    }
}
