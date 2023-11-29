using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ElanceJobDetails
    {
        public ElanceJobDetails()
        {
            ElanceAssignedJob = new HashSet<ElanceAssignedJob>();
        }

        public decimal ElanceJobId { get; set; }
        public decimal JobId { get; set; }
        public string JobName { get; set; }
        public string Description { get; set; }
        public string Budget { get; set; }
        public int? NumProposals { get; set; }
        public DateTime? PostedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? ClientUserId { get; set; }
        public string ClientName { get; set; }
        public string ClientCountry { get; set; }
        public string ClientCountryCode { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public string Status { get; set; }
        public string JobUrl { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool? IsAwarded { get; set; }
        public bool? IsFirstResponse { get; set; }
        public bool? IsAwardedTii { get; set; }

        public virtual ICollection<ElanceAssignedJob> ElanceAssignedJob { get; set; }
    }
}
