using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Forecasting
    {

        public Forecasting()
        {
            ForecastingDepartment = new HashSet<ForecastingDepartment>();
        }
        public int Id { get; set; }
        public string ProjectDescription { get; set; }
        public int? LeadId { get; set; }
        public int? ProjectId { get; set; }
        public string Country { get; set; }
        public int AddedPersonUId { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? TentiveDate { get; set; }
        public int ChasingType { get; set; }
        public string Groups { get; set; }
        public int? NoOfDeveloper { get; set; }
        public int? Status { get; set; }
        public int? ClientId { get; set; }
        public int? ReviewedUid { get; set; }
        public bool? IsHold { get; set; }
        public string HoldReason { get; set; }

        public virtual UserLogin UserLogin { get; set; }
        public virtual Client Client { get; set; }
        public virtual ProjectLead ProjectLead { get; set; }
        public virtual Project Project { get; set; }
        public virtual UserLogin ReviewedU { get; set; }
        public virtual ICollection<ForecastingDepartment> ForecastingDepartment { get; set; }
    }
}
