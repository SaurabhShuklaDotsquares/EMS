using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class CvbuilderCoreCompetencies
    {
        public long Id { get; set; }
        public long CvbuilderId { get; set; }
        public string Title { get; set; }
        public int? LevelId { get; set; }

        public virtual Cvbuilder Cvbuilder { get; set; }
    }
}
