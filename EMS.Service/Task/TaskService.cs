using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using EMS.Service;
using System.Globalization;
using EMS.Core;

namespace EMS.Service
{
    public class TaskService : ITaskService
    {
        #region global Fields and Constructor
        private IRepository<Data.Task> repotask;
        private IRepository<TaskStatu> repotaskstatus;
        private IRepository<TaskComment> repotaskcomment;
        public TaskService(IRepository<Data.Task> repotask, IRepository<TaskStatu> repotaskstatus, IRepository<TaskComment> repotaskcomment)
        {
            this.repotask = repotask;
            this.repotaskstatus = repotaskstatus;
            this.repotaskcomment = repotaskcomment;
        }
        #endregion

        #region Get Task List
        public List<TaskStatu> GetTask()
        {
            return repotaskstatus.Query().Get().ToList();
        }

        public List<Task> GetTaskList()
        {
            return repotask.Query().Get().ToList();
        }

        public List<Task> GettaskByPaging(out int total, PagingService<Task> pagingService)
        {
            return repotask.Query().Filter(pagingService.Filter).
                OrderBy(pagingService.Sort).
                GetPage(pagingService.Start, pagingService.Length, out total).
                ToList();
        }
        #endregion

        public Task GetTaskByMomMeetingTaskId(int momMeetingTaskId)
        {
            return repotask.Query().Filter(x => x.MomMeetingTaskId == momMeetingTaskId).Get().FirstOrDefault();
        }


        #region Save Task
        public bool Save(CreateTaskDto model)
        {
            Task task = new Task();
            try
            {
                if (model.TaskID > 0)
                {
                    List<int> ids = new List<int>();
                    task = repotask.FindById(model.TaskID);
                    if (task != null)
                    {
                        if (task.AddedUid == model.AddedUid)
                        {
                            var endDate = model.TaskEndDate.ToDateTime("dd/MM/yyyy").Value;
                            task.Priority = (byte)model.Priority;
                            task.TaskStatusID = (int)model.TaskStatusId;
                            task.TaskName = model.TaskName;
                            task.Remark = model.Remark;
                            task.LastUpdatedDate = DateTime.Now;
                            if (task.TaskEndDate != endDate)
                            {
                                task.TaskEndDate = endDate;
                                task.ReminderEmailSent = false;
                            }
                            //ids = task.TaskAssignedToes.Select(x => x.AssignUid).ToList();
                            //if (ids != null && ids.Any())
                            //{
                            //    repotask.ChangeEntityCollectionState(task.TaskAssignedToes.Where(x => ids.Contains(x.AssignUid)).ToList(), ObjectState.Deleted);
                            //}
                            // Add new 
                            if (model.Assign != null)
                            {
                                var taskAssignedToesTobeUpdate = task.TaskAssignedToes.Where(x => model.Assign.Contains(x.AssignUid)).ToList();
                                var taskAssignedToesTobeDeleted = task.TaskAssignedToes.Where(x => !model.Assign.Contains(x.AssignUid)).ToList();

                                if ( model.TaskStatusId== Enums.TaskStatusType.Completed || model.TaskStatusId == Enums.TaskStatusType.Closed)
                                {
                                    if (taskAssignedToesTobeUpdate != null && taskAssignedToesTobeUpdate.Count > 0)
                                    {
                                        foreach (var item in taskAssignedToesTobeUpdate) // Update
                                        {
                                            item.TaskStatusId = (int)model.TaskStatusId;
                                        }
                                        repotask.ChangeEntityCollectionState(taskAssignedToesTobeUpdate, ObjectState.Modified);
                                    }


                                }
                                if (taskAssignedToesTobeDeleted != null && taskAssignedToesTobeDeleted.Count > 0) //delete
                                {
                                    repotask.ChangeEntityCollectionState(taskAssignedToesTobeDeleted, ObjectState.Deleted);
                                }
                                var existingtaskAssignedToes = taskAssignedToesTobeUpdate?.Select(x => x.AssignUid).ToArray();
                                if (existingtaskAssignedToes != null)
                                {
                                    var addtaskAssignedToes = model.Assign.Where(val => !existingtaskAssignedToes.Contains(val)).ToArray();
                                    if (addtaskAssignedToes != null && addtaskAssignedToes.Length > 0) // Add
                                    {
                                        foreach (var item in addtaskAssignedToes)
                                        {
                                            task.TaskAssignedToes.Add(new TaskAssignedTo
                                            {
                                                AssignUid = Convert.ToInt32(item),
                                                TaskStatusId = (int)model.TaskStatusId
                                        });
                                        }

                                    }
                                }
                            }

                            repotask.Update(task);
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    task.TaskName = model.TaskName;
                    task.Priority = (byte)model.Priority;
                    task.Remark = model.Remark;
                    task.TaskStatusID = (int)Enums.TaskStatusType.Pending;
                    task.AddedUid = model.AddedUid;
                    task.CreatedDate = DateTime.Now;
                    task.LastUpdatedDate = DateTime.Now;
                    task.TaskEndDate = model.TaskEndDate.ToDateTime("dd/MM/yyyy");
                    foreach (var item in model.Assign)
                    {
                        task.TaskAssignedToes.Add(new TaskAssignedTo
                        {
                            AssignUid = Convert.ToInt32(item),
                            TaskStatusId = (int)Enums.TaskStatusType.Pending
                        });
                    }
                    repotask.InsertGraph(task);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task GetTaskById(int id)
        {
            return repotask.FindById(Convert.ToDecimal(id));
        }

        public void Delete(int id)
        {
            Data.Task task = repotask.FindById(Convert.ToDecimal(id));
            if (task != null)
            {

                repotask.ChangeEntityCollectionState(task.TaskAssignedToes, ObjectState.Deleted);
                repotask.ChangeEntityCollectionState(task.TaskComments, ObjectState.Deleted);
                repotask.Delete(Convert.ToDecimal(id));
            }
        }

        public Data.Task SaveComment(TaskCommentDto model)
        {
            //try
            //{
            if (model.TaskID > 0)
            {
                Task task = repotask.FindById(Convert.ToDecimal(model.TaskID));
                if (task != null)
                {
                    if (model.TaskStatusId == Enums.TaskStatusType.Completed || model.TaskStatusId == Enums.TaskStatusType.Closed) // Task would be updated only when comment status is set to "Completed" or "closed"
                    {
                        if (model.CommentFor == "Self")
                        {
                            var completedCount = task.TaskAssignedToes?.Where(ta => ta.TaskStatusId == (int)Enums.TaskStatusType.Completed && ta.AssignUid != model.CurrentUserUid)?.Count();
                            
                            if (task.TaskAssignedToes?.Count == 1) // only one TaskAssignTo
                            {
                                task.TaskStatusID = (int)model.TaskStatusId;
                            }
                            else if(model.TaskStatusId == Enums.TaskStatusType.Completed)
                            {
                                if (completedCount != null && completedCount + 1 == task.TaskAssignedToes?.Count) // Completed and TaskAssignedTo count are equal 
                                {
                                    task.TaskStatusID = (int)model.TaskStatusId;
                                }
                            }
                            else //closed
                            {
                                var closedCount = task.TaskAssignedToes?.Where(ta => ta.TaskStatusId == (int)Enums.TaskStatusType.Closed && ta.AssignUid != model.CurrentUserUid)?.Count();
                                if (closedCount != null && closedCount + 1 == task.TaskAssignedToes?.Count) // Completed and TaskAssignedTo count are equal 
                                {
                                    task.TaskStatusID = (int)model.TaskStatusId;
                                }
                            }
                            
                        }
                        else
                        {
                            task.TaskAssignedToes?.ToList().ForEach(ta => ta.TaskStatusId = (int)model.TaskStatusId); // Updates all TaskAssignedToes
                            task.TaskStatusID = (int)model.TaskStatusId; // Updates master Task
                        }


                    }

                    TaskAssignedTo taskAssignedTo = task.TaskAssignedToes.Where(ta => ta.AssignUid == model.CurrentUserUid)?.FirstOrDefault();
                    if (taskAssignedTo != null)
                    {
                        taskAssignedTo.TaskStatusId = (int)model.TaskStatusId;
                    }
                    task.TaskComments.Add(new TaskComment()
                    {
                        Comment = model.Comment,
                        AddedDate = DateTime.Now,
                        AddedUid = model.AddedUid,
                        TaskStatusID = (int)model.TaskStatusId
                    });

                    repotask.SaveChanges();
                    return task;
                }
            }
            //}
            //catch
            //{

            //}
            return null;
        }
        #endregion
        public string GetStatus(decimal taskId, int CurrentUserId)
        {
            var task = repotask.FindById(taskId);
            var taskAssignTo = task?.TaskAssignedToes.Where(ta => ta.AssignUid == CurrentUserId).FirstOrDefault();
            // If user is there in list then return task status from assignTo else from task(in case Assign by or PM user not there in assignTo list)
            if (taskAssignTo != null)
            {
                return taskAssignTo.TaskStatusId != null ? Extensions.GetDescription((Enums.TaskStatusType)taskAssignTo.TaskStatusId) : string.Empty;
            }
            else
            {
                return Extensions.GetDescription((Enums.TaskStatusType)task.TaskStatusID);
            }
        }

        public Enums.TaskStatusType GetTaskStatusId(decimal taskId, int CurrentUserId)
        {
            var task = repotask.FindById(taskId);
            var taskAssignTo = task?.TaskAssignedToes?.Where(ta => ta.AssignUid == CurrentUserId).FirstOrDefault();
            // If user is there in list then return task status from assignTo else from task(in case Assign by or PM user not there in assignTo list)
            if (taskAssignTo != null)
            {
                return taskAssignTo.TaskStatusId != null ? (Enums.TaskStatusType)taskAssignTo.TaskStatusId : Enums.TaskStatusType.Pending;
            }
            else
            {
                return (Enums.TaskStatusType)task.TaskStatusID;
            }

        }

        public List<Task> GetTasksInDuration(int uid,DateTime? startDate, DateTime? endDate)
        {
            var expr = PredicateBuilder.True<Task>();
            expr = expr.And(t => t.TaskAssignedToes.Any(ta => ta.AssignUid == uid && ta.TaskStatusId == (int)Enums.TaskStatusType.Pending));

            if (startDate.HasValue)
            {
                expr = expr.And(l => l.TaskEndDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                expr = expr.And(l => l.TaskEndDate <= endDate.Value);
            }

            //return repotask.Query().Filter(t => t.TaskAssignedToes.Any(ta => ta.AssignUid == uid && ta.TaskStatusId == (int)Enums.TaskStatusType.Pending)
            //&& t.TaskEndDate >= startDate && t.TaskEndDate <= endDate).Get().ToList();
            return repotask.Query().Filter(expr).Get().ToList();
        }
    }
}
