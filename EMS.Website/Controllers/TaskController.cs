using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.Models.Others;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Text;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class TaskController : BaseController
    {
        #region Fileds and Constructor

        private readonly IUserLoginService userLoginService;
        private readonly ITaskService taskService;
        private readonly IMinutesOfMeetingTaskCommentService minutesOfMeetingTaskCommentService;

        public TaskController(IUserLoginService userLoginService, ITaskService taskService, IMinutesOfMeetingTaskCommentService _minutesOfMeetingTaskCommentService)
        {
            this.userLoginService = userLoginService;
            this.taskService = taskService;
            this.minutesOfMeetingTaskCommentService = _minutesOfMeetingTaskCommentService;
        }

        #endregion

        #region Index Get Method

        [CustomActionAuthorization()]
        public ActionResult Index()
        {
            TaskToDoDto dto = new TaskToDoDto();

            var users = userLoginService.GetUsersByPM(PMUserId);
            // var selectedItems = users.Select(n => new SelectListItem { Text = n.Name, Value = n.Uid.ToString() }).ToList();
            dto.users = users.Select(n => new SelectListItem { Text = n.Name, Value = n.Uid.ToString() }).ToList();
            dto.TaskStatusId = (int)Enums.TaskStatusType.Pending;
            return View(dto);
        }

        [HttpPost]
        public IActionResult Index(IDataTablesRequest request, int? userId, bool assignedToMe, bool assignedByMe, bool otherTeamMembers, int? TaskStatusId)
        {
            var pagingServices = new PagingService<Task>(request.Start, request.Length);
            var expr = PredicateBuilder.True<Task>();
            //Task added by me or user having I as PM
            if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
            {
                if (assignedToMe && assignedByMe && otherTeamMembers)
                {
                    expr = expr.And(x => x.AddedUid == CurrentUser.Uid || x.UserLogin.PMUid == CurrentUser.Uid || x.TaskAssignedToes.Any(m => m.AssignUid == CurrentUser.Uid));
                }
                else if (assignedToMe && assignedByMe != true && otherTeamMembers)
                {
                    expr = expr.And(x => x.AddedUid == CurrentUser.Uid || x.UserLogin.PMUid == CurrentUser.Uid);
                }
                else if (assignedToMe && assignedByMe && otherTeamMembers != true)
                {
                    expr = expr.And(x => x.AddedUid == CurrentUser.Uid || x.TaskAssignedToes.Any(m => m.AssignUid == CurrentUser.Uid));
                }
                else if (assignedToMe && assignedByMe != true && otherTeamMembers != true)
                {
                    expr = expr.And(x => x.TaskAssignedToes.Any(m => m.AssignUid == CurrentUser.Uid));
                }
                else if (assignedToMe != true && assignedByMe != true && otherTeamMembers)
                {
                    expr = expr.And(x => x.UserLogin.PMUid == CurrentUser.Uid);
                }
                else if (assignedToMe != true && assignedByMe && otherTeamMembers != true)
                {
                    expr = expr.And(x => x.AddedUid == CurrentUser.Uid);
                }
                else if (assignedToMe != true && assignedByMe && otherTeamMembers)
                {
                    expr = expr.And(x => x.AddedUid == CurrentUser.Uid || x.UserLogin.PMUid == CurrentUser.Uid);
                }
                else
                {
                    expr = expr.And(x => x.AddedUid == CurrentUser.Uid || x.UserLogin.PMUid == CurrentUser.Uid || x.TaskAssignedToes.Any(m => m.AssignUid == CurrentUser.Uid));
                }
            }
            else
            {
                if ((assignedToMe && assignedByMe) || (!assignedToMe && !assignedByMe))
                {
                    expr = expr.And(x => x.TaskAssignedToes.Any(m => m.AssignUid == CurrentUser.Uid || x.AddedUid == CurrentUser.Uid));
                }
                else if (assignedToMe && assignedByMe != true)
                {
                    expr = expr.And(x => x.TaskAssignedToes.Any(m => m.AssignUid == CurrentUser.Uid));
                }
                else if (assignedToMe != true && assignedByMe) //Two true assignedToMe && otherTeamMembers
                {
                    expr = expr.And(x => x.TaskAssignedToes.Any(m => x.AddedUid == CurrentUser.Uid));
                }

                else
                {
                    expr = expr.And(x => x.TaskAssignedToes.Any(m => m.AssignUid == CurrentUser.Uid || x.AddedUid == CurrentUser.Uid));
                }

            }
            if (TaskStatusId != null)
            {
                expr = expr.And(x => x.TaskStatusID == TaskStatusId);
            }
            if (userId.HasValue && userId.Value > 0)
            {
                expr = expr.And(x => x.UserLogin.Uid == userId.Value || x.TaskAssignedToes.Any(m => m.AssignUid == userId.Value));

            }

            //expr = expr.And(x => x.TaskStatusID != (int)Enums.TaskStatusType.Closed);

            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "Name":
                            return o.OrderByColumn(item, c => c.TaskName);

                        default:
                            return o.OrderByColumn(item, c => c.TaskID);

                    }
                }
                return o.OrderByDescending(c => c.LastUpdatedDate);
            };
            int totalCount = 0;
            var response = taskService.GettaskByPaging(out totalCount, pagingServices);
            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                Id = r.TaskID,
                TaskId = "Task ID :" + r.TaskID,
                rowIndex = (index + 1) + (request.Start),
                TaskName = r.TaskName,
                UpdatedDate = r.LastUpdatedDate.ToFormatDateString("dd/MM/yyyy"),
                TaskEndDate = r.TaskEndDate.ToFormatDateString("dd/MM/yyyy"),
                Source = (r.MomMeetingTaskId != null && r.MomMeetingTaskId > 0 ? "MOM" : "TO-DO"),
                //Status = Extensions.GetDescription((Enums.TaskStatusType)r.TaskStatusID),
                //Status = taskService.GetStatus(r.TaskID, CurrentUser.Uid),
                Status = taskService.GetStatus(r.TaskID, ((userId.HasValue && userId.Value > 0) ? userId.Value : CurrentUser.Uid)),
                //momMeetingTaskId = (r.MomMeetingTaskId != null && r.MomMeetingTaskId > 0 ? r.MomMeetingTaskId:0),
                meetingId = (r.MomMeetingTaskId != null && r.MomMeetingTaskId > 0 ? r.MomMeetingTask.MomMeetingId : 0),
                MeetingStatus = (r.MomMeetingTaskId != null && r.MomMeetingTaskId > 0 ? Extensions.GetDescription((Enums.MomMeetingStatus)r.MomMeetingTask.Status) : ""),
                // Status = Extensions.GetDescription((Enums.TaskStatusType)r.TaskStatusID),
                AssignTo = string.Join(", ", r.TaskAssignedToes.Select(x => userLoginService.GetUserInfoByID(x.AssignUid).Name)),
                AssignBy = userLoginService.GetUserInfoByID(r.UserLogin.Uid).Name,
                Priority = r.Priority != null ? Extensions.GetDescription((Enums.Priority)r.Priority) : string.Empty,
                assignid = r.UserLogin.Uid,
                currentUserId = CurrentUser.Uid
            }));
        }

        #endregion

        #region Add Task

        [HttpGet]
        public ActionResult Add(int? id)
        {
            CreateTaskDto model = new CreateTaskDto();
            var users = userLoginService.GetUsersByPM(PMUserId);
            model.Users = users.Select(n => new SelectListItem { Text = n.Name, Value = n.Uid.ToString() }).ToList();
            if (id > 0)
            {
                var task = taskService.GetTaskById(id.Value);
                if (task != null)
                {
                    model.Priority = (Enums.Priority)task.Priority;
                    model.TaskName = task.TaskName;
                    model.TaskID = task.TaskID;
                    model.AddedUid = task.AddedUid;
                    model.Remark = task.Remark;
                    model.TaskStatusId = (Enums.TaskStatusType)task.TaskStatusID;
                    model.TaskEndDate = task.TaskEndDate.ToFormatDateString("dd/MM/yyyy");
                    model.Assign = task.TaskAssignedToes.Select(x => x.AssignUid).ToArray();
                }
            }
            return PartialView("_AddEditTask", model);
        }

        [HttpPost]
        public ActionResult Add(CreateTaskDto model)
        {
            if (ModelState.IsValid)
            {
                bool isExist = taskService.Save(model);
                if (isExist)
                {
                    if (model.TaskID == 0)
                    {
                        EmailSender(model.Assign, model.Remark, model.TaskName);
                    }
                    string Message = model.TaskID > 0 ? "Record updated successfully." : "Record saved successfully";
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = Message, IsSuccess = true });
                }
                else
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = "Add/Edit Not Allowed This User.", IsSuccess = true });
                }
            }
            return CreateModelStateErrors();
        }

        #endregion

        #region Delete Task

        [HttpGet]
        public ActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this task?",
                Size = Enums.ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Task?" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        public ActionResult delete(int id)
        {
            try
            {
                if (id > 0)
                {
                    var task = taskService.GetTaskById(id);
                    if (task != null && task.AddedUid == CurrentUser.Uid)
                    {
                        taskService.Delete(id);

                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record deleted successfully.", IsSuccess = true });
                    }
                }

                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = "Unable to delete task" });
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = ex.Message });
            }
        }

        #endregion

        #region Chase Task

        [HttpGet]
        public ActionResult Chase(int id)
        {
            Commentdto taskCommentdto = new Commentdto();
            var task = taskService.GetTaskById(id);
            if (task != null)
            {
                taskCommentdto.TaskID = Convert.ToInt32(task.TaskID);
                taskCommentdto.TaskStatusID = task.TaskStatusID;
                taskCommentdto.User = task.UserLogin?.Name;
            }
            return PartialView("_ChaseTask", taskCommentdto);
        }

        [HttpPost]
        public ActionResult Chase(Commentdto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var task = taskService.GetTaskById(model.TaskID);
                    var AssignUID = task.TaskAssignedToes.Select(x => x.AssignUid).ToArray();

                    EmailSender(AssignUID, model.Comment,task.TaskName);

                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Email Sent Successfully.", IsSuccess = true });
                }

                return CreateModelStateErrors();
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = ex.Message });
            }
        }

        #endregion

        #region Comment

        [HttpGet]
        public ActionResult Comment(int? id)
        {
            TaskCommentDto model = new TaskCommentDto();
            var task = taskService.GetTaskById(id.Value);
            if (task != null)
            {
                model.AssignTo = string.Join(", ", task.TaskAssignedToes.Select(x => x.UserLogin.Name));
                model.AssignBy = task.UserLogin?.Name;
                model.TaskID = Convert.ToInt32(task.TaskID);
                //model.Status = ((Enums.TaskStatusType)task.TaskStatusID).GetDescription();
                model.Status = taskService.GetStatus(task.TaskID, CurrentUser.Uid);
                model.TaskName = task.TaskName;
                model.CreatedDate = task.CreatedDate.ToFormatDateString("dd/MM/yyyy hh:mm tt");
                model.UpdateDate = task.LastUpdatedDate.ToFormatDateString("dd/MM/yyyy hh:mm tt");
                model.Comments = task.TaskComments.OrderByDescending(x => x.TaskCommentID)
                                                  .Select(x => new TaskCommentDto
                                                  {
                                                      Comment = x.Comment,
                                                      CommentBy = x.UserLogin?.Name,
                                                      AddedDate = x.AddedDate.ToFormatDateString("dd/MM/yyyy hh:mm tt")
                                                  }).ToList();
                //model.TaskStatusId = (Enums.TaskStatusType)task.TaskStatusID;
                //model.TaskStatusId = ((Enums.TaskStatusType)(assignToesStatus.HasValue ? assignToesStatus.Value : 1)); //setting status to TaskAssignedTo status else "Pending"
                model.TaskStatusId = taskService.GetTaskStatusId(task.TaskID, CurrentUser.Uid);
                model.Remark = task.Remark;
                model.Priority = Extensions.GetDescription((Enums.Priority)task.Priority);
                var enumList = Enum.GetValues(typeof(Enums.TaskStatusType)).Cast<Enums.TaskStatusType>();
                if (task.AddedUid == CurrentUser.Uid)
                {
                    model.TaskStatusList = enumList.Select(x => new SelectListItem()
                    {
                        Text = x.ToString(),
                        Value = Convert.ToInt32(x).ToString(),
                    })
                    .ToList();
                    model.ShowPostReplyButton = true;
                }
                else
                {
                    model.TaskStatusList = enumList.Where(x => x != Enums.TaskStatusType.Closed).Select(x => new SelectListItem()
                    {
                        Text = x.ToString(),
                        Value = Convert.ToInt32(x).ToString(),
                    })
                    .ToList();
                }
                model.IsUserInTaskAssignToList = task.TaskAssignedToes?.Any(ta => ta.AssignUid == CurrentUser.Uid) == true;
                if (model.TaskStatusId != Enums.TaskStatusType.Completed)
                {
                    model.ShowPostReplyButton = true;
                }
            }
            return PartialView("_CommentView", model);
        }

        [HttpPost]
        public ActionResult Comment(TaskCommentDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.AddedUid = CurrentUser.Uid;
                    model.CommentBy = CurrentUser.Name;
                    model.AddedDate = DateTime.Now.ToFormatDateString("dd/MM/yyyy hh:mm tt");
                    model.CurrentUserUid = CurrentUser.Uid;
                    if (model.TaskStatusId == Enums.TaskStatusType.Closed && model.TaskID > 0)
                    {
                        var task = taskService.GetTaskById(model.TaskID);
                        if (task != null && task.AddedUid != CurrentUser.Uid)
                        {
                            return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Invalid status selected" });
                        }
                    }
                    var taskEntity = taskService.SaveComment(model);
                    model.ShowPostReplyButton = true;
                    if (taskEntity != null)
                    {
                        if (taskEntity.TaskAssignedToes.Any() && taskEntity.TaskStatusID == (int)Enums.TaskStatusType.Completed)
                        {
                            EmailSender(taskEntity.TaskAssignedToes.Select(x => x.AssignUid).ToArray(), model.Comment,taskEntity.TaskName);
                        }
                        if (model.TaskStatusId == Enums.TaskStatusType.Completed && taskEntity.AddedUid != CurrentUser.Uid)
                        {
                            model.ShowPostReplyButton = false;
                        }
                        else if (model.TaskStatusId == Enums.TaskStatusType.Closed && taskEntity.AddedUid == CurrentUser.Uid)
                        {
                            model.ShowPostReplyButton = false;
                        }

                        if (taskEntity.MomMeetingTaskId != null)
                        {
                            MomMeetingTaskTimeLineDto timeLineDto = new MomMeetingTaskTimeLineDto();
                            timeLineDto.TaskCommentId = taskEntity.TaskComments.OrderByDescending(x => x.TaskCommentID).FirstOrDefault().TaskCommentID;
                            if (model.TaskStatusId == Enums.TaskStatusType.Closed || model.TaskStatusId == Enums.TaskStatusType.Completed)
                            {
                                timeLineDto.Status = Enums.MomMeetingStatus.Completed;

                            }
                            else if (model.TaskStatusId == Enums.TaskStatusType.Pending)
                            {
                                timeLineDto.Status = Enums.MomMeetingStatus.Pending;
                            }
                            else if (model.TaskStatusId == Enums.TaskStatusType.Process)
                            {
                                timeLineDto.Status = Enums.MomMeetingStatus.Ongoing;
                            }
                            //timeLineDto.Status = model.Status;
                            timeLineDto.Comment = model.Comment;
                            timeLineDto.CommentedByUid = CurrentUser.Uid;
                            timeLineDto.MomMeetingTaskId = taskEntity.MomMeetingTaskId ?? 0;

                            minutesOfMeetingTaskCommentService.AddMomMeetingTaskTimeLine(timeLineDto);
                        }
                        return NewtonSoftJsonResult(new RequestOutcome<TaskCommentDto> { Message = "Comment has been saved Successfully.", IsSuccess = true, Data = model });
                    }
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Unable to post comments" });
                }
                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Invalid parameters" });
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = ex.GetBaseException().Message });
            }
        }

        #endregion

        #region Email Sender

        private void EmailSender(int[] AssignIds, string text, string taskName)
        {
            try
            {
                if (AssignIds.Any())
                {
                    var selectedUsers = userLoginService.GetUserInfoByID(AssignIds);
                    string Ename = AssignIds.Length == 1 ? selectedUsers.FirstOrDefault()?.Name ?? "Hello Everyone" : "Hello Everyone";
                    string EmailTo = string.Join(";", selectedUsers.Select(t => t.EmailOffice).ToArray());
                    string User = CurrentUser.Name;
                    SendEmailTask(
                       EmailTo,
                       text.Trim(),
                       Ename,
                       User, taskName);
                }
            }
            catch
            {
            }
        }

        private void SendEmailTask(string EmailTo, string Comment, string Ename, string User, string taskName)
        {
            try
            {
                string Subject = "Urgent Task Comment :: " + DateTime.Now.ToFormatDateString("MMM, dd yyyy hh:mm tt");
                StringBuilder BodyContent = new StringBuilder();
                BodyContent.Append("<table style='width:100%;font-family:Arial; font-size:13px;' cellpadding='0' cellspacing='0'>");
                BodyContent.Append("<tr><td>Dear&nbsp;<b>" + Ename + "</b>,<br/></td></tr>");
                BodyContent.Append("<tr><td><br/></td></tr>");
                BodyContent.Append("<tr><td>Please accelerate below task and complete in given time period.</td></tr>");
                BodyContent.Append("<tr><td><br/><br/></td></tr>");
                BodyContent.Append("<tr><td><b>Description: </b> " + Comment + "</td></tr><br/>");                
                BodyContent.Append("<tr><td>For more details you can review To-Do ( <strong><a href='"+SiteKey.DomainName+"/task'>" +taskName + "</a></strong> ) in EMS.</td></tr>");
                BodyContent.Append("<tr><td><br/><br/></td></tr>");
                BodyContent.Append("<tr><td>Thanks & Regards<br/>" + User + "</td></tr>");
                BodyContent.Append("<tr><td><span style='font-weight:bold; font-family:Arial; font-size:11px; font-style:italic; color:gray;'>[Note : Please do not reply to this automated message; replies to this message are undeliverable. Contact your senior for any query.]</span></td></tr>");
                BodyContent.Append("</table>");

                FlexiMail objMail = new FlexiMail
                {
                    From = SiteKey.From,
                    To = EmailTo,
                    CC = "",
                    BCC = "",
                    Subject = Subject,
                    MailBodyManualSupply = true,
                    MailBody = BodyContent.ToString()
                };

                objMail.Send();
            }
            catch
            {

            }
        }

        #endregion
    }
}