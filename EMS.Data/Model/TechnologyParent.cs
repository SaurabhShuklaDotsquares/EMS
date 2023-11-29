using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class TechnologyParent
    {
        public TechnologyParent()
        {
            TechnologyParentMapping = new HashSet<TechnologyParentMapping>();
            EstimateHourParentTechnology = new HashSet<EstimateHourParentTechnology>();
           
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime AddDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime ModifyDate { get; set; }
        public string Email { get; set; }
        public virtual ICollection<TechnologyParentMapping> TechnologyParentMapping { get; set; }
        public virtual ICollection<EstimateHourParentTechnology> EstimateHourParentTechnology { get; set; }
      
    }
}
