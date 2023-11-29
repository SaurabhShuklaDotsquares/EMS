using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EstimateHour
    {
        public EstimateHour()
        {
            EstimateHourParentTechnology = new HashSet<EstimateHourParentTechnology>();
            EstimateHourTechnology = new HashSet<EstimateHourTechnology>();
        }

        public int Id { get; set; }
        public int DomainId { get; set; }
        public int? TechnologyParentId { get; set; }
        public string RequirementName { get; set; }
        public decimal EstimatedHour { get; set; }
        public bool IsFreeBie { get; set; }
        public int? Crmid { get; set; }
        public int Bauid { get; set; }
        public int Tluid { get; set; }
        public byte ComplexityLevel { get; set; }
        public string RequirementDesc { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime? ConversionDate { get; set; }
        public int? EstimateHourFileNameTypeId { get; set; }

        public virtual EstimateHourFileNameType EstimateHourFileNameType { get; set; }

        public virtual UserLogin Bau { get; set; }
        public virtual DomainType Domain { get; set; }
       // public virtual TechnologyParent TechnologyParent { get; set; }
        public virtual UserLogin Tlu { get; set; }
        public virtual ICollection<EstimateHourParentTechnology> EstimateHourParentTechnology { get; set; }
        public virtual ICollection<EstimateHourTechnology> EstimateHourTechnology { get; set; }
    }
}
