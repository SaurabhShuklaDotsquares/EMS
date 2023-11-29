using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class CvbuilderPreviousExperience
    {
        public long Id { get; set; }
        public long CvbuilderId { get; set; }
        public string OrganizationName { get; set; }
        public string Designation { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public virtual Cvbuilder Cvbuilder { get; set; }
    }
}
