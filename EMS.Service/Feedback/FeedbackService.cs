using EMS.Data;
using EMS.Data.Model;
using EMS.Dto;
using EMS.Dto.EmployeeFeedback;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service
{
    public class FeedbackService : IFeedbackService
    {
        IRepository<EmployeeFeedback> repoemployeefeedback;
        IRepository<EmployeeFeedbackReason> repoemployeefeedbackreason;
        IRepository<EmployeeFeedbackRank> repoemployeefeedbackrank;
        IRepository<UserExitProcess> repouserexitprocess;
        IRepository<UserNoc> repousernoc;
        IRepository<NocMaster> reponocmaster;
        public FeedbackService(IRepository<EmployeeFeedback> repoemployeefeedback, IRepository<EmployeeFeedbackReason> repoemployeefeedbackreason,
            IRepository<EmployeeFeedbackRank> repoemployeefeedbackrank, IRepository<UserExitProcess> repouserexitprocess,
            IRepository<UserNoc> repousernoc, IRepository<NocMaster> reponocmaster)
        {
            this.repoemployeefeedback = repoemployeefeedback;
            this.repoemployeefeedbackreason = repoemployeefeedbackreason;
            this.repoemployeefeedbackrank = repoemployeefeedbackrank;
            this.repouserexitprocess = repouserexitprocess;
            this.repousernoc = repousernoc;
            this.reponocmaster = reponocmaster;
        }
        public List<EmployeeFeedbackReason> getemployeefeedbackreason()
        {
            return repoemployeefeedbackreason.Query().Get().ToList();
        }
        public List<EmployeeFeedbackRank> getemployeefeedbackrank()
        {
            return repoemployeefeedbackrank.Query().Get().ToList();
        }
        public bool Save(FeedbackDto model)
        {
            EmployeeFeedback entity = new EmployeeFeedback();
            entity.CreatedDate = DateTime.Now;
            entity.ModifyDate = DateTime.Now;
            entity.Suggestion = model.Suggestions;
            entity.EmpUid = model.Uid;
            //entity.LeavingDate = Convert.ToDateTime(model.LeavingDate);
            entity.Comment = model.comment;
            entity.Lfprofile = model.LFProfile;
            entity.ReviewLink = model.ReviewLink;
            entity.SaveBy = model.SaveBy;
            foreach (var item in model.selectedfeedbackreason)
            {
                /*Changed By Tabassum */
                //entity.EmployeeFeedbackReasons.Add(repoemployeefeedbackreason.FindById(item));
                entity.EmployeeFeedbackReasonMapping.Add(new EmployeeFeedbackReasonMapping { EmployeeFeedbackReason = repoemployeefeedbackreason.FindById(item) });
                /*End*/
            }
            foreach (var item in model.FeedbackrankDto)
            {
                entity.EmployeeFeedbackRankStatus.Add(new EmployeeFeedbackRankStatus() { EmployeeFeedbackRankId = item.EmployeeFeedbackRankId, FeedBackStatus = item.EmployeeFeedbackStatus.Value });
            }
            try
            {
                repoemployeefeedback.InsertGraph(entity);
            }
            catch (Exception ex)
            {
            }

            return true;
        }

        public List<EmployeeFeedback> GetfeedbackByPaging(out int total, PagingService<EmployeeFeedback> pagingService)
        {
            return repoemployeefeedback.Query().Filter(pagingService.Filter).
                OrderBy(pagingService.Sort).
                GetPage(pagingService.Start, pagingService.Length, out total).
                ToList();
        }
        public EmployeeFeedback findByData(int id)
        {
            return repoemployeefeedback.FindById(id);
        }
        public bool FeedbackExists(int uid)
        {
            var feedback = repoemployeefeedback.Query().Filter(f => f.EmpUid == uid).Get().FirstOrDefault();
            return feedback != null && feedback.Id > 0 ? true : false;
        }

        //public List<EmployeeFeedbackReasonMapping> getemployeefeedbackreasonmapping()
        //{
        //    return repoemployeefeedbackrank.Query().Get().ToList();
        //}

        public List<EmployeeFeedback> employeefeedback()
        {
            var list = repoemployeefeedback.Query().Get().ToList();

            return list;
        }

        public List<UserExitProcess> userexitprocesslist()
        {
            var list = repouserexitprocess.Query().Get().ToList();

            return list;
        }

        public UserExitProcess findByUid(int id)
        {
            return repouserexitprocess.Query().Get().Where(u => u.EmpUid == id).FirstOrDefault();
        }

        public UserExitProcess saveprocess(ExitProcessDto model)
        {
            UserExitProcess entity = new UserExitProcess();

            entity.EmpUid = model.Uid;
            entity.IsFeedbackEmailSent = model.IsFeedbackEmailSent;
            entity.IsFeedbackReceived = model.IsFeedbackReceived;
            entity.IsVoluntaryExit = model.IsVoluntaryExit;
            entity.VoluntaryComment = model.VoluntaryComment;
            entity.IsEligibleForRehire = model.IsEligibleForRehire;
            entity.RehireComment = model.RehireComment;

            repouserexitprocess.InsertGraph(entity);
            return entity;
        }

        public EmployeeFeedback employeeFeedbackbyUid(int id)
        {
            return repoemployeefeedback.Query().Get().Where(u => u.EmpUid == id).FirstOrDefault();
        }

        public List<EmployeeFeedback> employeeFeedbackList()
        {
            return repoemployeefeedback.Query().Get().ToList();
        }

        public List<UserNoc> usernocList(int uid)
        {
            var list = repousernoc.Query().Get().Where(n => n.Uid == uid).ToList();

            return list;
        }

        public List<NocMaster> NocMasterList()
        {
            var list = reponocmaster.Query().Get().ToList();

            return list;
        }
        public UserExitProcess updateExitprocess(ExitProcessDto model)
        {

            UserExitProcess entity = new UserExitProcess();

            entity = findById(model.Id);

            if (entity == null)
            {
                entity = new UserExitProcess();

                entity.EmpUid = model.Uid;
                entity.IsFeedbackEmailSent = Convert.ToBoolean(model.IsFeedbackEmailSent);
                entity.IsFeedbackReceived = Convert.ToBoolean(model.IsFeedbackReceived);
                entity.IsVoluntaryExit = Convert.ToBoolean(model.IsVoluntaryExit);
                entity.VoluntaryComment = model.VoluntaryComment;
                entity.IsEligibleForRehire = Convert.ToBoolean(model.IsEligibleForRehire);
                entity.RehireComment = model.RehireComment;
                entity.IsIdCardSubmitted = Convert.ToBoolean(model.IsIdCardSubmitted);
                entity.ReleaseDocPrepared = Convert.ToBoolean(model.ReleaseDocPrepared);
                entity.IsExitFormalitiesCompleted = Convert.ToBoolean(model.IsExitFormalitiesCompleted);
                repouserexitprocess.Insert(entity);
            }
            else
            {
                //entity.Uid = model.Uid;
                entity.IsFeedbackEmailSent = Convert.ToBoolean(model.IsFeedbackEmailSent);
                entity.IsFeedbackReceived = Convert.ToBoolean(model.IsFeedbackReceived);
                entity.IsVoluntaryExit = Convert.ToBoolean(model.IsVoluntaryExit);
                entity.VoluntaryComment = model.VoluntaryComment;
                entity.IsEligibleForRehire = Convert.ToBoolean(model.IsEligibleForRehire);
                entity.RehireComment = model.RehireComment;
                entity.IsIdCardSubmitted = Convert.ToBoolean(model.IsIdCardSubmitted);
                entity.ReleaseDocPrepared = Convert.ToBoolean(model.ReleaseDocPrepared);
                entity.IsExitFormalitiesCompleted = Convert.ToBoolean(model.IsExitFormalitiesCompleted);
                repouserexitprocess.Update(entity);
            }
            return entity;
        }

        public EmployeeFeedback updateEmpFeedback(FeedbackDto model)
        {
            EmployeeFeedback entity = null;

            if (model.Uid > 0)
            {
                entity = employeeFeedbackbyUid(model.Uid);

                if (entity != null)
                {
                    entity.Lfprofile = model.LFProfile;
                    entity.EmailSkypePassReset = model.EmailSkypePassReset;

                    repoemployeefeedback.Update(entity);
                }
            }
            return entity;
        }

        public UserNoc userNocData(int Uid, int NocId)
        {
            return repousernoc.Query().Get().Where(u => u.Uid == Uid && u.Nocid == NocId).FirstOrDefault();
        }

        public UserNoc saveNoc(NocDetailsDto model)
        {
            UserNoc entity = null;

            entity = userNocData(model.Uid, model.NocId);

            if (entity == null)
            {
                entity = new UserNoc();

                entity.Uid = model.Uid;
                entity.Nocid = model.NocId;
                entity.Value = model.Value;
                repousernoc.Insert(entity);
            }
            else
            {
                entity.Value = model.Value;
                repousernoc.Update(entity);
            }

            return entity;
        }

        public UserExitProcess updateFeedbackStatus(int uid)
        {
            UserExitProcess entity = null;

            if (uid > 0)
            {
                entity = findByUid(uid);

                if (entity != null)
                {
                    entity.IsFeedbackReceived = true;
                    repouserexitprocess.Update(entity);
                }
            }
            return entity;
        }
        public UserExitProcess findById(int id)
        {
            return repouserexitprocess.Query().Get().Where(u => u.Id == id).FirstOrDefault();
        }

        public bool Savefeedback(EmpFeedbackDto model)
        {


            EmployeeFeedback entity = new EmployeeFeedback();
            entity.CreatedDate = DateTime.Now;
            entity.ModifyDate = DateTime.Now;
            entity.LeavingDate = model.LeavingDate;
            //model.LeavingDate;

            entity.Suggestion = model.Suggestions;
            entity.EmpUid = model.Uid;
            entity.Comment = model.comment;
            entity.Lfprofile = model.LFProfile;
            entity.ReviewLink = model.ReviewLink;
            entity.EmpPmuid = model.PmUid;
            entity.SaveBy = model.SaveBy;
            entity.EmailSkypePassReset = model.EmailSkypePassReset;

            if (model.selectedfeedbackreason != null)
            {
                foreach (var item in model.selectedfeedbackreason)
                {
                    entity.EmployeeFeedbackReasonMapping.Add(new EmployeeFeedbackReasonMapping { EmployeeFeedbackReason = repoemployeefeedbackreason.FindById(item) });
                }
            }
            
            foreach (var item in model.FeedbackrankDto)
            {
                entity.EmployeeFeedbackRankStatus.Add(new EmployeeFeedbackRankStatus() { EmployeeFeedbackRankId = item.EmployeeFeedbackRankId, FeedBackStatus = item.EmployeeFeedbackStatus.Value });
            }

            repoemployeefeedback.InsertGraph(entity);

            return true;
        }
    }
}
