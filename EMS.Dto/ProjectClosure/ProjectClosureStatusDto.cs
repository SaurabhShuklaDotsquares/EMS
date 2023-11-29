using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
    public class ProjectClosureStatusDto
    {
        public ProjectClosureStatusDto()
        {
            ChangeStatus = new List<SelectListItem>();
        }

        public int ProjectClosureId { get; set; }

        [DisplayName("Project Name")]
        public string ProjectName { get; set; }

        public string Reason { get; set; }

        [DisplayName("Current Status")]
        public string CurrentStatus { get; set; }
        
        [DisplayName("Change Status")]
        public int ChangeStatusId { get; set; }

        [DisplayName("Permanent Dead")]
        public bool IsPermanentDead { get; set; } = false;

        public string DeadResponseDate { get; set; }

        public int PMUid { get; set; }
        public bool FromReviewPage { get; set; }

        public List<SelectListItem> ChangeStatus { get; set; }
    }
}
