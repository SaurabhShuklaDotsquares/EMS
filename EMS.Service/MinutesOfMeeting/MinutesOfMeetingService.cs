using EMS.Core;
using EMS.Data;
using EMS.Data.Model;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EMS.Service
{
    public class MinutesOfMeetingService : IMinutesOfMeetingService
    {
        private readonly IRepository<MomMeeting> repoMinutesOfMeeting;
        private readonly IRepository<Momdocument> momdocumentRepository;
        private readonly IRepository<UserLogin> repoUserLogin;
        private readonly IRepository<Department> repodepartment;
        private readonly IRepository<MomMeetingTaskDocument> repoMomMeetingTaskDocument;
        private readonly IRepository<MomMeetingDepartment> repoMomMeetingDepartment;
        private readonly IRepository<MomMeetingParticipant> repoMomMeetingParticipant;
        private readonly IRepository<MomMeetingTaskTimeLine> repoMomMeetingTaskTimeLine;

        private readonly IRepository<MomMeetingTask> repoMinutesOfMeetingTask;
        private readonly IRepository<MomMeetingTaskParticipant> repoMinutesOfMeetingTaskParticipants;
        private readonly IRepository<Task> repotask;
        private readonly IRepository<TaskAssignedTo> repotaskAssignedTo;
        private readonly IRepository<TaskComment> repotaskComment;
        private readonly IRepository<PILog> repoPILog;
        private readonly IRepository<GETMOMListByMeetingMasterID_Result> repoGETMOMListByMeetingMasterID;


        private readonly IMinutesOfMeetingTaskService repoMomMeetingTaskService;

        public MinutesOfMeetingService(IRepository<MomMeeting> repoMinutesOfMeeting, IRepository<Momdocument> momdocumentRepository, IRepository<UserLogin> repoUserLogin, IRepository<Department> repodepartment, IRepository<MomMeetingTaskDocument> repoMomMeetingTaskDocument,
            IRepository<MomMeetingDepartment> repoMomMeetingDepartment, IRepository<MomMeetingParticipant> repoMomMeetingParticipant,
           IRepository<MomMeetingTaskTimeLine> repoMomMeetingTaskTimeLine,
           IRepository<MomMeetingTask> repoMinutesOfMeetingTask,
           IRepository<MomMeetingTaskParticipant> repoMinutesOfMeetingTaskParticipants,
           IRepository<Task> repotask, IRepository<TaskAssignedTo> repotaskAssignedTo, IRepository<TaskComment> repotaskComment,
           IRepository<PILog> repoPILog, IRepository<GETMOMListByMeetingMasterID_Result> repoGETMOMListByMeetingMasterID)
        {
            this.repoMinutesOfMeeting = repoMinutesOfMeeting;
            this.repoUserLogin = repoUserLogin;
            this.repodepartment = repodepartment;
            this.momdocumentRepository = momdocumentRepository;
            this.repoMomMeetingTaskDocument = repoMomMeetingTaskDocument;
            this.repoMomMeetingDepartment = repoMomMeetingDepartment;
            this.repoMomMeetingParticipant = repoMomMeetingParticipant;
            this.repoMomMeetingTaskTimeLine = repoMomMeetingTaskTimeLine;
            this.repoMinutesOfMeetingTask = repoMinutesOfMeetingTask;
            this.repoMinutesOfMeetingTaskParticipants = repoMinutesOfMeetingTaskParticipants;
            this.repotask = repotask;
            this.repotaskAssignedTo = repotaskAssignedTo;
            this.repotaskComment = repotaskComment;
            this.repoPILog = repoPILog;
            this.repoGETMOMListByMeetingMasterID = repoGETMOMListByMeetingMasterID;
        }

        public List<MomMeeting> GetMinutesOfMeetingByPaging(out int total, PagingService<MomMeeting> pagingService)
        {
            return repoMinutesOfMeeting.Query()
                   .Filter(pagingService.Filter)
                   .OrderBy(pagingService.Sort)
                   .Include(x => x.UserLogin)
                   .GetPage(pagingService.Start, pagingService.Length, out total)
                   .ToList();
        }

        public MomMeeting GetMinutesOfMeetingFindById(int id)
        {
            return repoMinutesOfMeeting.FindById(id);
        }

        public MomMeeting GetMinutesOfMeetingFindTopicId(int id)
        {
            return repoMinutesOfMeeting.Query().GetQuerable().Where(x => x.MeetingMasterID == id).SingleOrDefault();
        }

        public List<MomMeeting> GetAllMinutesOfMeetingFindByTopicId(int id)
        {
            return repoMinutesOfMeeting.Query().GetQuerable().Where(x => x.MeetingMasterID == id).ToList();
        }

        public List<MomMeetingParticipant> GetMinutesofMeetingPreviousMeetingUsersByMeetingMasterId(int id, int authorId)
        {
            return repoMinutesOfMeeting.Query().GetQuerable().Where(x => x.MeetingMasterID == id && x.AuthorByUID == authorId).OrderByDescending(x => x.DateOfMeeting).OrderByDescending(x => x.Id).FirstOrDefault()?.MomMeetingParticipant.ToList();
            //&& x.ParticipantType == (byte)Core.Enums.MomMeetingParticipantType.Individual
        }

        public List<MomMeetingDepartment> GetMinutesofMeetingPreviousMeetingDepartmentsByMeetingMasterId(int id, int authorId)
        {
            return repoMinutesOfMeeting.Query().GetQuerable().Where(x => x.MeetingMasterID == id && x.AuthorByUID == authorId).OrderByDescending(x => x.DateOfMeeting).OrderByDescending(x => x.Id).FirstOrDefault()?.MomMeetingDepartment.ToList();
            //&& x.ParticipantType == (byte)Core.Enums.MomMeetingParticipantType.Group
        }

        public MomMeeting GetPreviousMinutesofMeetingByMeetingMasterId(int masterId)
        {
            return repoMinutesOfMeeting.Query().GetQuerable().Where(x => x.MeetingMasterID == masterId).OrderByDescending(x => x.DateOfMeeting).OrderByDescending(x => x.Id).FirstOrDefault();
        }

        public MomMeeting Save(MomMeetingDto model)
        {
            MomMeeting entity = model.Id > 0 ? GetMinutesOfMeetingFindById(model.Id) : new MomMeeting();

            if (entity != null)
            {
                if (entity.Id > 0)
                {
                    if (entity.AuthorByUID != model.AuthorByUID)
                    {
                        if (entity.ChairedByUID != model.AuthorByUID)
                        {
                            return null;
                        }
                    }
                    entity.AuthorByUID = entity.AuthorByUID;
                    entity.ChairedByUID = entity.ChairedByUID;
                }
                else
                {
                    entity.AuthorByUID = model.AuthorByUID;
                    entity.ChairedByUID = model.ChairedByUID;
                }
                entity.MeetingMasterID = model.MeetingMasterID;
                entity.MeetingTitle = model.MeetingTitle;
                entity.ModifiedDate = DateTime.Now;
                entity.DateOfMeeting = model.DateOfMeetings.ToDateTime("dd/MM/yyyy").Value;
                entity.MeetingTime = model.MeetingTime;
                entity.MeetingStartTime = TimeSpan.Parse(model.MeetingStartTime);
                entity.Agenda = model.Agenda;
                entity.Notes = model.Notes;
                entity.ParticipantType = (byte)model.ParticipantType;

                entity.VenueName = model.VenueName;

                foreach (var filename in model.MomDocuments)
                {
                    entity.Momdocuments.Add(new Momdocument
                    {
                        AddedDate = DateTime.Now,
                        DocumentPath = filename.DocumentPath,
                    });
                }

                if (model.ParticipantType == Core.Enums.MomMeetingParticipantType.Individual)
                {
                    if (model.Paticipants != null && model.Paticipants.Any())
                    {
                        entity.MomMeetingParticipant.Clear();
                        Array.ForEach(model.Paticipants, x => entity.MomMeetingParticipant.Add(new MomMeetingParticipant { U = repoUserLogin.FindById(x) }));
                        //Array.ForEach(model.Paticipants, x => entity.MomMeetingParticipant.Add(repoUserLogin.FindById(x)));
                    }

                    //var usersToRemove = entity.MomMeetingParticipant.Where(u => !model.Paticipants.Contains(u.Uid)).ToList();
                    //foreach (var user in usersToRemove)
                    //{ entity.MomMeetingParticipant.Remove(user); }
                    //var usersToInsert = model.Paticipants.Where(p => !entity.MomMeetingParticipant.Any(e => e.Uid == p)).ToList();
                    //usersToInsert.ForEach(x => entity.MomMeetingParticipant.Add(new MomMeetingParticipant { Uid = x }));
                    //repoMinutesOfMeeting.ChangeEntityCollectionState(entity.MomMeetingParticipant, ObjectState.Unchanged);
                }
                else
                {
                    if (model.Groups != null && model.Groups.Any())
                    {
                        entity.MomMeetingDepartment.Clear();
                        Array.ForEach(model.Groups, x => entity.MomMeetingDepartment.Add(new MomMeetingDepartment { Department = repodepartment.FindById(x) }));
                        //Array.ForEach(model.Paticipants, x => entity.MomMeetingParticipant.Add(repoUserLogin.FindById(x)));
                    }

                    //var departmentToRemove = entity.MomMeetingDepartment.Where(u => !model.Groups.Contains(u.DepartmentId)).ToList();
                    //foreach (var department in departmentToRemove)
                    //{ entity.MomMeetingDepartment.Remove(department); }
                    //var departmentToInsert = model.Groups.Where(p => !entity.MomMeetingDepartment.Any(e => e.DepartmentId == p)).ToList();
                    //departmentToInsert.ForEach(x => entity.MomMeetingDepartment.Add(new MomMeetingDepartment { DepartmentId = x }));
                    //repoMinutesOfMeeting.ChangeEntityCollectionState(entity.MomMeetingDepartment, ObjectState.Unchanged);
                }
                if (entity.Id == 0)
                {
                    entity.CreatedDate = DateTime.Now;
                    repoMinutesOfMeeting.InsertGraph(entity);
                }
                else
                {
                    repoMinutesOfMeeting.SaveChanges();
                }
            }
            return entity;
        }

        public bool Delete(int Id)
        {
            var data = GetMinutesOfMeetingFindById(Id);
            if (data != null)
            {
                repoMomMeetingDepartment.DeleteBulk(data.MomMeetingDepartment.ToList());
                repoMomMeetingParticipant.DeleteBulk(data.MomMeetingParticipant.ToList());
                repoMomMeetingTaskTimeLine.DeleteBulk(data.MomMeetingTaskTimeLines.ToList());
                momdocumentRepository.DeleteBulk(data.Momdocuments.ToList());

                MOMMeetingTaskDelete(data.MomMeetingTasks.ToList());


                repoMinutesOfMeeting.Delete(Id);
                return true;
            }
            return false;
        }

        public Momdocument GetDocument(int id)
        {
            return momdocumentRepository.FindById(id);
        }

        public bool DeleteDocument(int id)
        {
            var document = momdocumentRepository.FindById(id);
            if (document != null)
            {
                momdocumentRepository.Delete(document);
                return true;
            }
            return false;
        }

        public MomMeetingTaskDocument GetMomMeetingTaskDocument(int id)
        {
            return repoMomMeetingTaskDocument.FindById(id);
        }

        public bool DeleteMomMeetingTaskDocument(int id)
        {
            var document = repoMomMeetingTaskDocument.FindById(id);
            if (document != null)
            {
                repoMomMeetingTaskDocument.Delete(document);
                return true;
            }
            return false;
        }

        public List<GETMOMListByMeetingMasterID_Result> GETMOMListByMeetingMasterIDSP(int Id)
        {
            return repoGETMOMListByMeetingMasterID.GetDbContext().GETMOMListByMeetingMasterID(Id).Result.ToList();
        }


        public void Dispose()
        {
            if (repoMinutesOfMeeting != null)
            {
                repoMinutesOfMeeting.Dispose();
            }
        }

        public MomMeeting GetLatestMinutesofMeeting()
        {
            return repoMinutesOfMeeting.Query().Get().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
        }

        #region [Private Methods]
        public bool MOMMeetingTaskDelete(List<MomMeetingTask> momMeetingTask)
        {
            var dataTaskAssignedTo = new List<TaskAssignedTo>();
            var datataskComment = new List<TaskComment>();

            var dataMOMTaskParticipants = new List<MomMeetingTaskParticipant>();
            var dataMomMeetingTaskTimeLine = new List<MomMeetingTaskTimeLine>();
            var dataTask = new List<Task>();
            var dataPILog = new List<PILog>();
            var dataMomMeetingTaskDocument = new List<MomMeetingTaskDocument>();

            foreach (var data in momMeetingTask)
            {
                dataMOMTaskParticipants.AddRange(repoMinutesOfMeetingTaskParticipants.Query().Filter(x => x.MomMeetingTaskId == data.Id).Get().ToList());
                dataMomMeetingTaskTimeLine.AddRange(repoMomMeetingTaskTimeLine.Query().Filter(x => x.MomMeetingTaskId == data.Id).Get().ToList());

                dataTask.AddRange(repotask.Query().Filter(x => x.MomMeetingTaskId == data.Id).Get().ToList());

                if (dataTask != null && dataTask.Count() > 0)
                {
                    var taskid = repotask.Query().Filter(m => m.MomMeetingTaskId == data.Id).Get().FirstOrDefault().TaskID;
                    dataTaskAssignedTo.AddRange(repotaskAssignedTo.Query().Filter(x => x.TaskId == taskid).Get().ToList());

                    datataskComment.AddRange(repotaskComment.Query().Filter(x => x.TaskId == taskid).Get().ToList());
                }

                if (data.Pilog != null)
                {
                    dataPILog.Add(data.Pilog);
                }

                dataMomMeetingTaskDocument.AddRange(data.MomMeetingTaskDocuments);
            }

            repoMinutesOfMeetingTaskParticipants.DeleteBulk(dataMOMTaskParticipants);
            repoMomMeetingTaskTimeLine.DeleteBulk(dataMomMeetingTaskTimeLine);

            repotaskAssignedTo.DeleteBulk(dataTaskAssignedTo);
            repotaskComment.DeleteBulk(datataskComment);
            repoMomMeetingTaskDocument.DeleteBulk(dataMomMeetingTaskDocument);

            repotask.DeleteBulk(dataTask);
            repoPILog.DeleteBulk(dataPILog);
            repoMinutesOfMeetingTask.DeleteBulk(momMeetingTask);

            return true;
        }
        #endregion
    }
}