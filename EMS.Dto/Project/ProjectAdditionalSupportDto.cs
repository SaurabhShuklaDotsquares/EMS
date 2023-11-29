using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
    public class ProjectAdditionalSupportIndexDto
    {
        public bool IsDirector { get; set; }

        public List<SelectListItem> ProjectList { get; set; }

        public List<SelectListItem> StatusList { get; set; }

        public List<SelectListItem> PMUserList { get; set; }

        public List<SelectListItem> UserListByPM { get; set; }

        public ProjectAdditionalSupportIndexDto()
        {
            ProjectList = new List<SelectListItem>();
            StatusList = new List<SelectListItem>();
            PMUserList = new List<SelectListItem>();
            UserListByPM = new List<SelectListItem>();
        }
    }

    public class ProjectAdditionalSupportDto
    {
        public int Id { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; }

        [DisplayName("Supporting Developers")]
        public int[] AssignedUserIds { get; set; }

        [DisplayName("Project")]
        public string ProjectName { get; set; }

        [DisplayName("Start Date")]
        public string StartDate { get; set; }

        [DisplayName("End Date")]
        public string EndDate { get; set; }

        [DisplayName("Request Date")]
        public string CreateDate { get; set; }

        public string ApprovalDate { get; set; }

        [DisplayName("Approver Comments")]
        public string ApprovalComment { get; set; }

        [DisplayName("Requested By")]
        public string RequestByUser { get; set; }

       
        public string Description { get; set; }

      //  [Required]
        [Display(Name = "Description")]
        public string AddDescription { get; set; }
        public byte Status { get; set; }

        public string StatusText { get; set; }

        public bool AllowUpdate { get; set; }
        public int? ApproveByUid { get; set; }
        public string RequestToken { get; set; }
        public bool FromProjectStatus { get; set; }
        public int ? TLid { get; set; }

        public bool IsPMUser { get; set; }

        public int? UserIdByPM { get; set; }

        public string Period { get; set; }

        public List<SelectListItem> ProjectList { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public List<SelectListItem> UserList { get; set; }
        public List<SelectListItem> TLList { get; set; }

        public List<SelectListItem> UserListByPM { get; set; }

        public ProjectAdditionalSupportDto()
        {
            StatusList = new List<SelectListItem>();
            ProjectList = new List<SelectListItem>();
            UserList = new List<SelectListItem>();
            TLList = new List<SelectListItem>();
            UserListByPM = new List<SelectListItem>();
        }
    }
}
