using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
    public class ProjectClosureReviewDto
    {
        public ProjectClosureReviewDto()
        {
            PromisingPercentageList = new List<SelectListItem>();
        }

        public int ProjectClosureId { get; set; }

        [DisplayName("Project Name")]
        public string ProjectName { get; set; }

        [DisplayName("Review Comments")]
        public string Comments { get; set; }

        [DisplayName("May start again on")]
        public string NextStartDate { get; set; }

        [DisplayName("How much promising")]
        public byte? PromisingPercentageId { get; set; }

        [DisplayName("No. of Developers may start")]
        public int? DeveloperCount { get; set; }

        public int PMUid { get; set; }
        public int CurrentUserId { get; set; }
        public List<SelectListItem> PromisingPercentageList { get; set; }
    }

    public class ProjectClosureReviewSummaryDto
    {
        public int PMId { get; set; }
        public string PMName { get; set; }

        public int ProjectPromising { get; set; }
        public int ProjectLessPromising { get; set; }
        public int ProjectNotSure { get; set; }

        public int OccupancyIncreasePromising { get; set; }
        public int OccupancyIncreaseLessPromising { get; set; }
        public int OccupancyIncreaseNotSure { get; set; }
    }
}
