using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class CvbuilderIndustry
    {
        public int Id { get; set; }
        public long? CvbuilderId { get; set; }
        public int? IndustryId { get; set; }

        public virtual Cvbuilder Cvbuilder { get; set; }
        public virtual DomainType Industry { get; set; }
    }
}
