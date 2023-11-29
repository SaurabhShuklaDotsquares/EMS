using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EstimateDocumentIndustry
    {
        public int EstimateDocumentId { get; set; }
        public int IndustryId { get; set; }

        public virtual EstimateDocument EstimateDocument { get; set; }
        public virtual DomainType Industry { get; set; }
    }
}
