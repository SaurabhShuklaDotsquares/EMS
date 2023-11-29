using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using static EMS.Core.Enums;

namespace EMS.Service {
    public class MinutesOfMeetingTaskService : IMinutesOfMeetingTaskService {
        private readonly IRepository<MomMeetingTask> repoMinutesOfMeetingTask;
        private readonly IRepository<UserLogin> repoUserLogin;
        private readonly IRepository<Task> repotask;
        private readonly IRepository<TaskAssignedTo> repotaskAssignedTo;
        private readonly IRepository<TaskComment> repotaskComment;
        private readonly IRepository<MomMeetingTaskParticipant> repoMinutesOfMeetingTaskParticipants;
        private readonly IRepository<MomMeetingTaskTimeLine> repoMinutesOfMeetingTaskComment;
        private readonly IMinutesOfMeetingTaskCommentService MinutesOfMeetingTaskCommentService;
        private readonly IMinutesOfMeetingService minutesOfMeetingService;
        private readonly IUserLoginService userLoginService;

        public MinutesOfMeetingTaskService(IRepository<Data.Task> repotask, IRepository<TaskAssignedTo> repotaskAssignedTo, IRepository<TaskComment> repotaskComment, IRepository<MomMeetingTask> repoMinutesOfMeeting, IRepository<UserLogin> repoUserLogin, IRepository<MomMeetingTaskParticipant> repoMinutesOfMeetingTaskParticipants, IRepository<MomMeetingTaskTimeLine> repoMinutesOfMeetingTaskComment, IMinutesOfMeetingTaskCommentService _MinutesOfMeetingTaskCommentService, IMinutesOfMeetingService _minutesOfMeetingService, IUserLoginService _userLoginService) {
            this.repotask = repotask;
            this.repotaskAssignedTo = repotaskAssignedTo;
            this.repotaskComment = repotaskComment;
            repoMinutesOfMeetingTask = repoMinutesOfMeeting;
            this.repoUserLogin = repoUserLogin;
            this.repoMinutesOfMeetingTaskParticipants = repoMinutesOfMeetingTaskParticipants;
            this.repoMinutesOfMeetingTaskComment = repoMinutesOfMeetingTaskComment;
            MinutesOfMeetingTaskCommentService = _MinutesOfMeetingTaskCommentService;
            minutesOfMeetingService = _minutesOfMeetingService;
            this.userLoginService = _userLoginService;
        }
        public List<MomMeetingTask> GetMinutesOfMeetingTaskByPaging(out int total, PagingService<MomMeetingTask> pagingService) {
            return repoMinutesOfMeetingTask.Query()
              .Filter(pagingService.Filter)
              .OrderBy(pagingService.Sort)
              .GetPage(pagingService.Start, pagingService.Length, out total)
              .ToList();
        }

        public List<MomMeetingTask> GetMinutesOfMeetingTaskForEmail(/*ICollection<MomMeetingTask> momMeetingTasks,DateTime meetingDate*/int MeetingMasterID, DateTime MOMCreatedDate) {
            return repoMinutesOfMeetingTask.Query().Filter(m => m.MomMeeting.MeetingMasterID == MeetingMasterID && m.MomMeeting.CreatedDate <= MOMCreatedDate && m.Status != (byte)MomMeetingStatus.Completed).Get().ToList();
        }

        public List<MomMeetingTask> GetMinutesOfMeetingTaskByMeetingMasterId(int MeetingMasterID, DateTime MOMCreatedDate) {
            return repoMinutesOfMeetingTask.Query().Filter(m => m.MomMeeting.MeetingMasterID == MeetingMasterID && m.MomMeeting.CreatedDate <= MOMCreatedDate).Get().ToList();
        }

        public List<MomMeetingTask> GetMinutesOfMeetingTaskList(int id) {
            return repoMinutesOfMeetingTask.Query().Filter(x => x.MomMeetingId == id).Get().ToList();
        }
        public MomMeetingTask GetMinutesOfMeetingTaskFindById(int id) {
            return repoMinutesOfMeetingTask.FindById(id);
        }
        public MomMeetingTask Save(MomMeetingTaskDto model) {
            MomMeetingTask entity = model.Id > 0 ? GetMinutesOfMeetingTaskFindById(model.Id) : new MomMeetingTask();
            Task entitytask = model.Id > 0 ? repotask.Query().Filter(x => x.MomMeetingTaskId == model.Id).Get().FirstOrDefault() : new Task();
            //MomMeetingTask resTask = GetMinutesOfMeetingTaskFindById(model.Id);

            if (entitytask == null) {
                entitytask = new Task();
            }


            List<int> ids = new List<int>();
            if (entity != null) {
                entity.Task = model.Task;
                entity.ModifiedDate = DateTime.Now;
                entity.TargetDate = model.TargetDates.ToDateTime("dd/MM/yyyy");
                entity.MomMeetingId = model.MomMeetingId;
                entity.Status = (byte)model.Status;
                entity.Remark = model.Remark;
                entity.Priority = (byte)model.Priority;

                entity.MomMeetingTaskParticipant.Clear();

                entitytask.TaskName = entity.Task;
                entitytask.LastUpdatedDate = entity.ModifiedDate;
                entitytask.TaskEndDate = entity.TargetDate;
                entitytask.MomMeetingTaskId = entity.Id;
                if (entity.Status == 1) {
                    entitytask.TaskStatusID = 1;
                }
                else if (entity.Status == 2) {
                    entitytask.TaskStatusID = 2;
                }
                else if (entity.Status == 3) {
                    entitytask.TaskStatusID = 1;
                }
                else if (entity.Status == 4) {
                    entitytask.TaskStatusID = 3;
                }
                else {
                    entitytask.TaskStatusID = 4;
                }

                entitytask.Remark = entity.Remark;
                entitytask.Priority = entity.Priority;
                entitytask.AddedUid = model.CommentByUId;

                foreach (var filename in model.MomMeetingTaskDocuments)
                {
                    entity.MomMeetingTaskDocuments.Add(new MomMeetingTaskDocument
                    {
                        AddedDate = DateTime.Now,
                        DocumentPath = filename.DocumentPath,
                    });
                }

                if (model.Paticipants != null && model.Paticipants.Any(p => p.ToString() == "0")) {
                    var momMeet = minutesOfMeetingService.GetMinutesOfMeetingFindById(model.MomMeetingId);
                    //var chairedParticipants = userLoginService.GetUserInfoByID(entity.MomMeeting.ChairedByUID);
                    //var authorParticipants = userLoginService.GetUserInfoByID(entity.MomMeeting.AuthorByUID);

                    if (momMeet.ParticipantType == (byte)MomMeetingParticipantType.Individual) {
                        
                        //model.Paticipants = momMeet.MomMeetingParticipant.Select(n => n.U.Uid).ToArray();
                        int[] momParticipants = momMeet.MomMeetingParticipant.Select(n => n.U.Uid).ToArray();
                        int[] modalParticipant = model.Paticipants.Where(p=>p!=0).ToArray();
                        int[] extraParticipants= modalParticipant.Except(momParticipants).ToArray();
                        model.Paticipants = new int[momParticipants.Length + extraParticipants.Length];
                        Array.Copy(momParticipants, model.Paticipants, momParticipants.Length);
                        Array.Copy(extraParticipants, 0, model.Paticipants, momParticipants.Length, extraParticipants.Length);
                    }
                    else
                    {
                        //model.Paticipants = userLoginService.GetUsersDetailsByDeptId(momMeet.MomMeetingDepartment.Select(d => d.DepartmentId).ToArray()).Select(n => n.Uid).ToArray();
                        UserLogin CurrentUser = repoUserLogin.FindById(entitytask.AddedUid);
                        if (CurrentUser == null || IsSpecialUser(CurrentUser))
                        {
                            model.Paticipants = userLoginService.GetUsersDetailsByDeptId(momMeet.MomMeetingDepartment.Select(d => d.DepartmentId).ToArray()).Select(n => n.Uid).ToArray();
                        }
                        else
                        {
                            model.Paticipants = userLoginService.GetUsersDetailsByDeptId(momMeet.MomMeetingDepartment.Select(d => d.DepartmentId).ToArray())
                                .Where(n => (n.PMUid == CurrentUser.Uid || n.Uid == CurrentUser.Uid || n.Uid == CurrentUser.PMUid || n.PMUid == CurrentUser.PMUid))
                                .Select(n => n.Uid).ToArray();
                        }
                        

                        
                    }
                    
                    int[] paticipants = model.Paticipants.ToArray();

                    //if (!model.Paticipants.Any(x => x.ToString() == authorParticipants.Uid.ToString())) {
                    //    Array.Resize(ref paticipants, paticipants.Length + 1);
                    //    paticipants[paticipants.Length - 1] = authorParticipants.Uid;
                    //}

                    //if (!model.Paticipants.Any(x => x.ToString() == chairedParticipants.Uid.ToString())) {
                    //    Array.Resize(ref paticipants, paticipants.Length + 1);
                    //    paticipants[paticipants.Length - 1] = chairedParticipants.Uid;
                    //}
                    model.Paticipants = paticipants;

                    Array.ForEach(model.Paticipants, x => entity.MomMeetingTaskParticipant.Add(new MomMeetingTaskParticipant { U = repoUserLogin.FindById(x) }));
                }
                else if (model.Paticipants != null && model.Paticipants.Any() && model.Paticipants.Any(p => p.ToString() != "0")) {
                    Array.ForEach(model.Paticipants, x => entity.MomMeetingTaskParticipant.Add(new MomMeetingTaskParticipant { U = repoUserLogin.FindById(x) }));
                }


                //entity.MomMeetingTaskTimeLine.Add(new MomMeetingTaskTimeLine
                //{
                //    MomMeetingId = model.MomMeetingId,
                //    Status = (byte)model.Status,
                //    CreatedDate = DateTime.Now,
                //    ModifiedDate = DateTime.Now,
                //    Comment = model.Remark,
                //    CommentByUid = model.CommentByUId,
                //});

                if (entity.Id == 0) {
                    entity.CreatedDate = DateTime.Now;

                    repoMinutesOfMeetingTask.InsertGraph(entity);
                }
                else {
                    repoMinutesOfMeetingTask.Update(entity);
                }

                entitytask.MomMeetingTaskId = entity.Id;
                //ids = entitytask.TaskAssignedToes.Select(x => x.AssignUid).ToList();

                //if (ids != null && ids.Any()) {
                //    repotask.ChangeEntityCollectionState(entitytask.TaskAssignedToes.Where(x => ids.Contains(x.AssignUid)).ToList(), ObjectState.Deleted);
                //}
                //// Add new  
                //if (model.Paticipants != null && model.Paticipants.Any()) {
                //    foreach (var item in model.Paticipants) {
                //        entitytask.TaskAssignedToes.Add(new TaskAssignedTo { AssignUid = Convert.ToInt32(item) });
                //    }
                //}

                

                if (entitytask.TaskID == 0) {
                    entitytask.CreatedDate = DateTime.Now;
                    // Add new  
                    if (model.Paticipants != null && model.Paticipants.Any()) {
                        foreach (var item in model.Paticipants) {
                            entitytask.TaskAssignedToes.Add(new TaskAssignedTo { AssignUid = Convert.ToInt32(item),TaskStatusId=(int)Enums.TaskStatusType.Pending });
                        }
                    }

                    repotask.InsertGraph(entitytask);
                }
                else {

                    var taskAssignedToesTobeUpdate = entitytask.TaskAssignedToes.Where(x => model.Paticipants.Contains(x.AssignUid)).ToList();
                    var taskAssignedToesTobeDeleted = entitytask.TaskAssignedToes.Where(x => !model.Paticipants.Contains(x.AssignUid)).ToList();
                    if (model.Status == Enums.MomMeetingStatus.Completed)
                    {
                        if (taskAssignedToesTobeUpdate != null && taskAssignedToesTobeUpdate.Count > 0)
                        {
                            foreach (var item in taskAssignedToesTobeUpdate) // Update
                            {
                                if (model.Status == MomMeetingStatus.Pending)
                                {
                                    item.TaskStatusId = 1;
                                }
                                else if (model.Status == MomMeetingStatus.Ongoing)
                                {
                                    item.TaskStatusId = 2;
                                }
                                else if (model.Status == MomMeetingStatus.Delayed)
                                {
                                    item.TaskStatusId = 1;
                                }
                                else if (model.Status == MomMeetingStatus.Completed)
                                {
                                    item.TaskStatusId = 3;
                                }
                                else
                                {
                                    item.TaskStatusId = 4;
                                }
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
                        var addtaskAssignedToes = model.Paticipants.Where(val => !existingtaskAssignedToes.Contains(val)).ToArray();
                        if (addtaskAssignedToes != null && addtaskAssignedToes.Length > 0) // Add
                        {
                            foreach (var item in addtaskAssignedToes)
                            {
                                entitytask.TaskAssignedToes.Add(new TaskAssignedTo
                                {
                                    AssignUid = item,
                                    TaskStatusId = (int)Enums.TaskStatusType.Pending
                                });
                            }

                        }
                    }

                    repotask.Update(entitytask);
                    //repotask.SaveChanges();
                }

                MomMeetingTaskTimeLine momMeeting = MinutesOfMeetingTaskCommentService.GetMinutesOfMeetingTaskCommentByTaskId(entity.Id).OrderBy(m => m.CreatedDate).FirstOrDefault() == null ? new MomMeetingTaskTimeLine() : MinutesOfMeetingTaskCommentService.GetMinutesOfMeetingTaskCommentByTaskId(entity.Id).OrderBy(m => m.CreatedDate).FirstOrDefault();

                if (momMeeting.Id != 0) {
                    momMeeting.MomMeetingId = model.MomMeetingId;
                    momMeeting.ModifiedDate = DateTime.Now;
                    momMeeting.Status = (byte)model.Status;
                    momMeeting.TargetDate = model.TargetDates.ToDateTime("dd/MM/yyyy");
                    momMeeting.Comment = entity.Remark;
                    momMeeting.CommentByUid = model.CommentByUId;
                    momMeeting.Priority = (byte)model.Priority;
                    repoMinutesOfMeetingTaskComment.Update(momMeeting);
                }
                else {
                    momMeeting.MomMeetingId = model.MomMeetingId;
                    momMeeting.MomMeetingTaskId = entitytask.MomMeetingTaskId.Value;
                    momMeeting.Status = (byte)model.Status;
                    momMeeting.CreatedDate = DateTime.Now;
                    momMeeting.ModifiedDate = DateTime.Now;
                    momMeeting.Comment = model.Remark;
                    momMeeting.Priority = (byte)model.Priority;
                    momMeeting.TargetDate = model.TargetDates.ToDateTime("dd/MM/yyyy");
                    momMeeting.CommentByUid = model.CommentByUId;
                    repoMinutesOfMeetingTaskComment.InsertGraph(momMeeting);
                }


            }
            return entity;
        }

        private bool IsSpecialUser(UserLogin CurrentUser)
        {
            return (CurrentUser.RoleId == (int)Enums.UserRoles.HRBP ||
                    CurrentUser.RoleId == (int)Enums.UserRoles.PMO ||
                    CurrentUser.RoleId == (int)Enums.UserRoles.UKPM ||
                    CurrentUser.RoleId == (int)Enums.UserRoles.NWCloudLeadRole ||
                    CurrentUser.RoleId == (int)Enums.UserRoles.NWLeadRole ||
                    CurrentUser.RoleId == (int)Enums.UserRoles.NWLANWAN ||
                     CurrentUser.RoleId == (int)Enums.UserRoles.NWProblemResolution ||
                      CurrentUser.RoleId == (int)Enums.UserRoles.NWHardwareSoftware ||
                    CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM ||
                    CurrentUser.RoleId == (int)Enums.UserRoles.Director);
        }

        public MomMeetingTask SaveTaskDecision(MomMeetingTaskDto model) {
            MomMeetingTask entity = new MomMeetingTask();
            if (model.Id != 0) {
                entity = repoMinutesOfMeetingTask.FindById(model.Id);
                entity.Decision = model.Decision;
                repoMinutesOfMeetingTask.Update(entity);
            }
            return entity;
        }

        public bool Delete(int Id) {
            var data = GetMinutesOfMeetingTaskFindById(Id);
            if (data != null) {
                try {
                    var dataTaskAssignedTo = new List<TaskAssignedTo>();
                    var datataskComment = new List<TaskComment>();
                    var dataMOMTaskParticipants = repoMinutesOfMeetingTaskParticipants.Query().Filter(x => x.MomMeetingTaskId == Id).Get().ToList();
                    var dataMOMTaskComment = repoMinutesOfMeetingTaskComment.Query().Filter(x => x.MomMeetingTaskId == Id).Get().ToList();

                    var dataTask = repotask.Query().Filter(x => x.MomMeetingTaskId == Id).Get().ToList();

                    if (dataTask != null && dataTask.Count() > 0) {
                        var taskid = repotask.Query().Filter(m => m.MomMeetingTaskId == Id).Get().FirstOrDefault().TaskID;
                        dataTaskAssignedTo = repotaskAssignedTo.Query().Filter(x => x.TaskId == taskid).Get().ToList();

                        datataskComment = repotaskComment.Query().Filter(x => x.TaskId == taskid).Get().ToList();
                    }

                    repoMinutesOfMeetingTaskParticipants.DeleteBulk(dataMOMTaskParticipants);
                    repoMinutesOfMeetingTaskComment.DeleteBulk(dataMOMTaskComment);

                    repotaskAssignedTo.DeleteBulk(dataTaskAssignedTo);
                    repotaskComment.DeleteBulk(datataskComment);

                    repotask.DeleteBulk(dataTask);


                    repoMinutesOfMeetingTask.Delete(Id);
                    return true;
                }
                catch (Exception) {
                    return false;
                }
            }
            else {
                return false;
            }
        }
        public void Dispose() {
            if (repoMinutesOfMeetingTask != null) {
                repoMinutesOfMeetingTask.Dispose();
            }
        }
    }
}
