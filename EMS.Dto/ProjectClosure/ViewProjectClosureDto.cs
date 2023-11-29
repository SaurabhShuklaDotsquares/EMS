using System.Collections.Generic;

namespace EMS.Dto
{
    public class ViewProjectClosureDto
    {
        public ViewProjectClosureDto()
        {
            ChaseHistory = new List<ChaseHistoryDto>();
        }
        public string BA_Name { get; set; }
        public string TL_Name { get; set; }
        public string DeveloperName { get; set; }
        public string VirtualDeveloperName { get; set; }
        public string LiveUrl { get; set; }
        public string ProjectName { get; set; }
        public string ClientName { get; set; }
        public string ClientQuality { get; set; }
        public string CRMStatus { get; set; }        
        public string ProjectStatus { get; set; }
        public string Reason { get; set; }
        public string Suggestion { get; set; }
        public string DateOfClosing { get; set; }
        public string NextEngementDate { get; set; }
        public string Technologies { get; set; }

        public string PromisingPercentage { get; set; }
        public int DeveloperCount { get; set; }
        public string ReviewComments { get; set; }
        public string ProjectMayStartDate { get; set; }

        public List<ChaseHistoryDto> ChaseHistory { get; set; }
    }

    public class ChaseHistoryDto
    {
        public string MessageText { get; set; }
        public string AddedBy { get; set; }
        public string ChaseDate { get; set; }
    }

    public class ProjectClosureProjectDetailDto
    {

        public ProjectClosureProjectDetailDto()
        {
            ClosureHistory = new List<ProjectClosureHistoryDto>();
        }

        public string ProjectName { get; set; }
        public string ClientName { get; set; }

        public List<ProjectClosureHistoryDto> ClosureHistory { get; set; }
    }

    public class ProjectClosureHistoryDto
    {
        public string DateOfClosing { get; set; }
        public string Invoice { get; set; }
        public string Estimate { get; set; }
        public string StartEndDate { get; set; }
        public string Reason { get; set; }
        public string CRMStatus { get; set; }
        public string Suggestion { get; set; }
        public int Id { get; set; }
        public int ProjectId { get; set; }

        public string AssignedToBA { get; set;}
        public bool? IsNewLeadGenerated { get; set; }
        public int NewLeadId { get; set; }

        public bool IsAllowToViewProjectClosureDetail { get; set; }
    }


}
