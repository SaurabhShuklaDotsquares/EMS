using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class CvbuilderTechnology
    {
        public int Id { get; set; }
        public long? CvbuilderId { get; set; }
        public int? TechnologyId { get; set; }

        public virtual Cvbuilder Cvbuilder { get; set; }
        public virtual Technology Technology { get; set; }
    }
}
