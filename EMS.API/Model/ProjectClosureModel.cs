using EMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.API.Model
{
    public class ProjectClosureModel
    {
        public ProjectClosureModel()
        {
            Developers = new List<ProjectDeveloperModel>();
        }
        public int CRMId { get; set; }
        public string PMEmail { get; set; }
        public Enums.APICRMStatus Status { get; set; }
        public Enums.APICRMStatus? OldStatus { get; set; }

        public string Reason { get; set; }

        /// <summary>
        /// Date format must be like "dd/MM/yyyy" OR NULL to set Current Date
        /// </summary>
        public string DateOfClosing { get; set; }

        /// <summary>
        /// Date format must be like "dd/MM/yyyy"
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// Date format must be like "dd/MM/yyyy"
        /// </summary>
        public string EndDate { get; set; }

        public int? EstimateDays { get; set; }
        public int? InvoiceDays { get; set; }
        public double? BucketHours { get; set; }
        public string IsTimeMaterial { get; set; }
        public string ProjectURL { get; set; }

        public string ClientBadge { get; set; }

        public ProjectDeveloperModel BA { get; set; }

        public List<ProjectDeveloperModel> Developers { get; set; }
    }

    public class ProjectDeveloperModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
