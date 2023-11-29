using EMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using static EMS.Core.Enums;

namespace EMS.Dto
{
    public class TaskCommentDto
    {
        public TaskCommentDto()
        {
            TaskStatusList = new List<SelectListItem>();
        }
        public int TaskCommentID { get; set; }
        public int TaskID { get; set; }
        public string Comment { get; set; }
        public string AddedDate { get; set; }
        public int AddedUid { get; set; }
        public string AssignTo { get; set; }
        public string AssignToList { get; set; }
        public string AssignBy { get; set; }
        public string Priority { get; set; }
        public string TaskName { get; set; }
        public string UpdateDate { get; set; }
        public string CreatedDate { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public string CommentBy { get; set; }
        public bool ShowPostReplyButton { get; set; }
        public int CurrentUserUid { get; set; }
        public TaskStatusType TaskStatusId { get; set; }
        public string CommentFor { get; set; }
        public bool IsUserInTaskAssignToList { get; set; }
        public List<TaskCommentDto> Comments { get; set; }
        public List<SelectListItem> TaskStatusList { get; set; }
    }

    public class Commentdto
    {
        public string Comment { get; set; }
        public int AddedUid { get; set; }
        public int TaskID { get; set; }
        public int TaskStatusID { get; set; }
        public DateTime AddedDate { get; set; }
        public string User { get; set; }
    }
}
