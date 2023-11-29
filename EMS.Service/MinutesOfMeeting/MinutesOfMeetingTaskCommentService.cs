using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using static EMS.Core.Enums;

namespace EMS.Service {
    public class MinutesOfMeetingTaskCommentService : IMinutesOfMeetingTaskCommentService {
        private readonly IRepository<MomMeetingTaskTimeLine> repoMinutesOfMeetingTaskComment;
        private readonly IRepository<UserLogin> repoUserLogin;
        private readonly IRepository<MomMeetingTask> repoMinutesOfMeeting;
        private readonly IRepository<Data.Task> repotask;
        private readonly IRepository<Data.TaskComment> repotaskComment;
        private readonly IMinutesOfMeetingService minutesOfMeetingService;
        private readonly IUserLoginService userLoginService;

        public MinutesOfMeetingTaskCommentService(IRepository<Data.Task> repotask, IRepository<Data.TaskComment> repotaskComment, IRepository<MomMeetingTaskTimeLine> _repoMinutesOfMeetingTaskComment, IRepository<UserLogin> repoUserLogin, IRepository<MomMeetingTask> _repoMinutesOfMeeting, IMinutesOfMeetingService _minutesOfMeetingService,
            IUserLoginService _userLoginService) {
            this.repotask = repotask;
            this.repotaskComment = repotaskComment;
            repoMinutesOfMeetingTaskComment = _repoMinutesOfMeetingTaskComment;
            this.repoUserLogin = repoUserLogin;
            repoMinutesOfMeeting = _repoMinutesOfMeeting;
            minutesOfMeetingService = _minutesOfMeetingService;
            userLoginService = _userLoginService;
        }
        public List<MomMeetingTaskTimeLine> GetMinutesOfMeetingByPaging(out int total, PagingService<MomMeetingTaskTimeLine> pagingService) {
            return repoMinutesOfMeetingTaskComment.Query()
                   .Filter(pagingService.Filter)
                   .OrderBy(pagingService.Sort)
                   .GetPage(pagingService.Start, pagingService.Length, out total)
                   .ToList();
        }
        public MomMeetingTask GetMinutesOfMeetingTaskTimeLineWithMomTaskFindById(int id) {
            return repoMinutesOfMeeting.FindById(id);
        }
        public MomMeetingTaskTimeLine GetMinutesOfMeetingTaskTimeLineFindById(int id) {

            return repoMinutesOfMeetingTaskComment.FindById(id);
        }


        public List<MomMeetingTaskTimeLine> GetMinutesOfMeetingTaskCommentByTaskId(int id) {
            return repoMinutesOfMeetingTaskComment.Query().Filter(x => x.MomMeetingTaskId == id).Get().ToList();
        }


        public bool AddMomMeetingTaskTimeLine(MomMeetingTaskTimeLineDto model)
        {
            MomMeetingTaskTimeLine momMeeting = GetMinutesOfMeetingTaskCommentByTaskId(model.MomMeetingTaskId).OrderBy(m=> m.CreatedDate).FirstOrDefault();
            MomMeetingTaskTimeLine newMomMeeting = new MomMeetingTaskTimeLine();
            if (momMeeting != null)
            {
                /*insert into meeting Comments*/
                newMomMeeting.MomMeetingTaskId = momMeeting.MomMeetingTaskId;
                newMomMeeting.MomMeetingId = momMeeting.MomMeetingId;
                newMomMeeting.Status = (byte)model.Status;
                newMomMeeting.Comment = model.Comment;
                newMomMeeting.CreatedDate = DateTime.Now;
                newMomMeeting.ModifiedDate = DateTime.Now;
                newMomMeeting.CommentByUid = model.CommentedByUid;
                newMomMeeting.TaskCommentId = model.TaskCommentId;
                newMomMeeting.TargetDate = momMeeting.TargetDate;
                newMomMeeting.Priority = momMeeting.Priority;

                repoMinutesOfMeetingTaskComment.ChangeEntityState<MomMeetingTaskTimeLine>(newMomMeeting, ObjectState.Added);
                repoMinutesOfMeetingTaskComment.SaveChanges();

                return true;
            }
            return false;
        }

        public MomMeetingTaskTimeLine Save(MomMeetingTaskCommentsAddDto model) {
            List<int> ids = new List<int>();
            MomMeetingTask entity = model.MomMeetingTaskId > 0 ? GetMinutesOfMeetingTaskTimeLineWithMomTaskFindById(model.MomMeetingTaskId) : new MomMeetingTask();
            Data.Task entitytask = new Data.Task();

            Data.TaskComment entitytaskComment = new Data.TaskComment();


            //MomMeetingTaskTimeLine momMeeting = GetMinutesOfMeetingTaskCommentByTaskId(model.MomMeetingTaskId).OrderBy(m=> m.CreatedDate).FirstOrDefault();

            MomMeetingTaskTimeLine momMeeting = new MomMeetingTaskTimeLine();
            if (entity.Id != 0) {
                /*insert into meeting Comments*/
                //entity.MomMeetingId = model.MomMeetingId;

                momMeeting.MomMeetingId = model.MomMeetingId;
                momMeeting.CreatedDate = DateTime.Now;
                momMeeting.ModifiedDate = DateTime.Now;
                momMeeting.Status = (byte)model.Status;
                momMeeting.TargetDate = model.TargetDates.ToDateTime("dd/MM/yyyy");
                momMeeting.Priority = (byte)model.Priority;
                momMeeting.Comment = model.Comment;
                momMeeting.CommentByUid = model.CommentedByUid;
                momMeeting.MomMeetingTaskId = model.MomMeetingTaskId;
                entity.MomMeetingTaskTimeLine.Add(momMeeting);

                //entity.Remark = model.Comment;
                entity.Decision = model.Decision;
                entity.Status = (byte)model.Status;
                entity.TargetDate = model.TargetDates.ToDateTime("dd/MM/yyyy");
                entity.Priority = (byte)model.Priority;
                entity.MomMeetingTaskParticipant.Clear();

                //if (model.Paticipants != null && model.Paticipants.Any())
                //{
                //    //Array.ForEach(model.Paticipants, x => entity.MomMeetingTaskParticipant.Add(repoUserLogin.FindById(x)));
                //    Array.ForEach(model.Paticipants, x => entity.MomMeetingTaskParticipant.Add(new MomMeetingTaskParticipant { U = repoUserLogin.FindById(x) }));
                //}

                if (model.Paticipants != null && model.Paticipants.Any(p => p.ToString() == "0")) {
                    var momMeet = minutesOfMeetingService.GetMinutesOfMeetingFindById(model.MomMeetingId);
                    if (momMeet.ParticipantType == (byte)MomMeetingParticipantType.Individual) {
                        //model.Paticipants = momMeet.MomMeetingParticipant.Select(n => n.U.Uid).ToArray();
                        int[] momParticipants = momMeet.MomMeetingParticipant.Select(n => n.U.Uid).ToArray();
                        int[] modalParticipant = model.Paticipants.Where(p => p != 0).ToArray();
                        int[] extraParticipants = modalParticipant.Except(momParticipants).ToArray();
                        model.Paticipants = new int[momParticipants.Length + extraParticipants.Length];
                        Array.Copy(momParticipants, model.Paticipants, momParticipants.Length);
                        Array.Copy(extraParticipants, 0, model.Paticipants, momParticipants.Length, extraParticipants.Length);
                    }
                    else {
                        model.Paticipants = userLoginService.GetUsersDetailsByDeptId(momMeet.MomMeetingDepartment.Select(d => d.DepartmentId).ToArray()).Select(n => n.Uid).ToArray();
                    }

                    //model.Paticipants.

                    Array.ForEach(model.Paticipants, x => entity.MomMeetingTaskParticipant.Add(new MomMeetingTaskParticipant { U = repoUserLogin.FindById(x) }));
                }
                else if (model.Paticipants != null && model.Paticipants.Any() && model.Paticipants.Any(p => p.ToString() != "0")) {
                    Array.ForEach(model.Paticipants, x => entity.MomMeetingTaskParticipant.Add(new MomMeetingTaskParticipant { U = repoUserLogin.FindById(x) }));
                }

                repoMinutesOfMeeting.SaveChanges();


                #region To Do
                #region Add Task in task table
                /*Add Task in task table*/

                if (repotask.Query().Filter(x => x.MomMeetingTaskId == model.MomMeetingTaskId).Get().FirstOrDefault() != null) {
                    entitytask = model.MomMeetingTaskId > 0 ? repotask.Query().Filter(x => x.MomMeetingTaskId == model.MomMeetingTaskId).Get().FirstOrDefault() : new Data.Task();

                    var taskAssignedToesTobeUpdate = entitytask.TaskAssignedToes.Where(x => model.Paticipants.Contains(x.AssignUid)).ToList();
                    
                    if (model.Status == Enums.MomMeetingStatus.Completed)
                    {
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

                        if (taskAssignedToesTobeUpdate != null && taskAssignedToesTobeUpdate.Count > 0)
                        {
                            foreach (var item in taskAssignedToesTobeUpdate) // Update
                            {
                                    item.TaskStatusId = (int)Enums.TaskStatusType.Completed; //
                            }
                            repotask.ChangeEntityCollectionState(taskAssignedToesTobeUpdate, ObjectState.Modified);
                        }

                    }
                    var taskAssignedToesTobeDeleted = entitytask.TaskAssignedToes.Where(x => !model.Paticipants.Contains(x.AssignUid)).ToList();
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


                    entitytask.Priority = entity.Priority;
                    entitytask.TaskEndDate = entity.TargetDate;

                    entitytask.MomMeetingTaskId = entity.Id;
                    // entitytask.TaskAssignedToes.Clear();
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
                    repotask.Update(entitytask);
                }
                else {
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
                    entitytask.AddedUid = model.CommentedByUid;


                    ids = entitytask.TaskAssignedToes.Select(x => x.AssignUid).ToList();

                    if (ids != null && ids.Any()) {
                        repotask.ChangeEntityCollectionState(entitytask.TaskAssignedToes.Where(x => ids.Contains(x.AssignUid)).ToList(), ObjectState.Deleted);
                    }
                    // Add new  
                    if (model.Paticipants != null && model.Paticipants.Any()) {
                        foreach (var item in model.Paticipants) {
                            entitytask.TaskAssignedToes.Add(new TaskAssignedTo { AssignUid = Convert.ToInt32(item) });
                        }
                    }

                    if (entitytask.TaskID == 0) {
                        entitytask.CreatedDate = DateTime.Now;

                        repotask.InsertGraph(entitytask);
                    }
                    else {
                        repotask.Update(entitytask);
                        //repotask.SaveChanges();
                    }
                }



                /*Add Task in task table*/
                #endregion

                #region Add comment in taskComment table

                /*Add comment in taskComment table*/

                
                entitytaskComment.TaskId = entitytask.TaskID;
                entitytaskComment.Comment = model.Comment;
                entitytaskComment.AddedDate = DateTime.Now;
                entitytaskComment.AddedUid = model.CommentedByUid;
                entitytaskComment.TaskStatusID = (byte)model.Status;


                repotaskComment.InsertGraph(entitytaskComment);



                /*Add comment in taskComment table*/
                #endregion 
                #endregion


            }
            return momMeeting;
        }
        public bool Delete(int Id) {
            var data = GetMinutesOfMeetingTaskTimeLineFindById(Id);
            if (data != null) {
                repoMinutesOfMeeting.Delete(Id);
                return true;
            }
            return false;
        }
        public void Dispose() {
            if (repoMinutesOfMeeting != null) {
                repoMinutesOfMeeting.Dispose();
            }
        }
    }
}
