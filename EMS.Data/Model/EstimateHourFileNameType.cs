using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class EstimateHourFileNameType
    {
        public EstimateHourFileNameType()
        {
            EstimateHour = new HashSet<EstimateHour>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public bool IsActive { get; set; }
        public string UploadFileName { get; set; }
        public virtual ICollection<EstimateHour> EstimateHour { get; set; }
    }
}
