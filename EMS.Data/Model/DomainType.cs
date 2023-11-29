using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class DomainType
    {
        public DomainType()
        {
            PortfolioDomain = new HashSet<PortfolioDomain>();
            LibraryIndustry = new HashSet<LibraryIndustry>();
            LibrarySearchIndustry = new HashSet<LibrarySearchIndustry>();
            ProjectLeadIndustry = new HashSet<ProjectLeadIndustry>();
            EstimateDocumentIndustry = new HashSet<EstimateDocumentIndustry>();
            EstimateHour = new HashSet<EstimateHour>();
            CvbuilderIndustry = new HashSet<CvbuilderIndustry>();
        }

        public int DomainId { get; set; }
        public string DomainName { get; set; }
        public DateTime AddDate { get; set; }
        public string Ip { get; set; }
        public string Alias { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<PortfolioDomain> PortfolioDomain { get; set; }
        public virtual ICollection<LibraryIndustry> LibraryIndustry { get; set; }
        public virtual ICollection<LibrarySearchIndustry> LibrarySearchIndustry { get; set; }
        public virtual ICollection<ProjectLeadIndustry> ProjectLeadIndustry { get; set; }
        public virtual ICollection<EstimateDocumentIndustry> EstimateDocumentIndustry { get; set; }
        public virtual ICollection<EstimateHour> EstimateHour { get; set; }
        public virtual ICollection<CvbuilderIndustry> CvbuilderIndustry { get; set; }
    }
}
