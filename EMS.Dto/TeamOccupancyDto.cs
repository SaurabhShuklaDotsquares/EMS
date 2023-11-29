using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Dto
{
    public class TeamOccupancyDto
    {
        public int TeamManagerId { get; set; }
        public string TeamManagerName { get; set; }
        public int TotalEmployee { get; set; }
        public int OnLeaveButRunning { get; set; }
        public int Running { get; set; }
        public int Free { get; set; }
        public int AdditionalSupport { get; set; }
        public int Leave { get; set; }
        public int OverRun { get; set; }
        public int TotalPaidDays { get; set; }
        public int NotLogin { get; set; }
        public int ActualRunning { get; set; }
        public int BonusRunning { get; set; }
        public int DesignerInSupport { get; set; }
        public int BAInSupport { get; set; }
        public int TLInSupport { get; set; }
        public int QAInSupport { get; set; }
        public int TotalSupport { get { return DesignerInSupport + BAInSupport + TLInSupport + QAInSupport; } }
        public int BucketRunning { get; set; }
        public string ActualRunningPercentage { get { return (Math.Round((double)(ActualRunning * 100) / TotalEmployee)).ToString(); } }
        public string SupportTeamPercentage { get { return (Math.Round((double)(TotalSupport * 100) / TotalEmployee)).ToString(); } }
        public string FreePercentage { get { return (Math.Round((double)(Free * 100) / TotalEmployee)).ToString(); } }
        public string AdditionalSupportPercentage { get { return (Math.Round((double)(AdditionalSupport * 100) / TotalEmployee)).ToString(); } }
        public string OverrunPercentage { get { return (Math.Round((double)(OverRun * 100) / TotalEmployee)).ToString(); } }

        public int UnassignedRunning { get; set; }
        public int SEORunning { get; set; }
        public int NotLoginButRunning { get; set; }

        public int TotalTraineeEmployee { get; set; }

        public int TotalDotNetEmployee { get; set; }
        public int TotalSEOEmployee { get; set; }
        public int TotalPHPEmployee { get; set; }
        public int TotalMobileEmployee { get; set; }
        public int TotalBAEmployee { get; set; }
        public int TotalDesignerEmployee { get; set; }
        public int TotalQAEmployee { get; set; }
        public int TotalOtherEmployee { get; set; }

        public int TotalHubspotEmployee { get; set; }

        public List<DepartmentOccupancyDto> RunningStats { get; set; }
        public List<DepartmentOccupancyDto> FreeStats { get; set; }

        public List<DepartmentOccupancyDto> TotalEmployeeStats { get; set; }
        
        public string BonusRunningDevelopers { get; set; }

        public string BucketProjectIds { get; set; }
        public string UnassignedProjectIds { get; set; }
        public string SEOUserIds { get; set; }

        public TeamOccupancyDto()
        {
            RunningStats = new List<DepartmentOccupancyDto>();
            FreeStats = new List<DepartmentOccupancyDto>();
            TotalEmployeeStats = new List<DepartmentOccupancyDto>();
        }
    }

    public class DepartmentOccupancyDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int OccupancyCount { get; set; }
    }
}
