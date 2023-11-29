using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;


namespace EMS.Service
{
    public class MinutesOfMeetingMasterService : IMinutesOfMeetingMasterService
    {
        private readonly IRepository<MeetingMaster> repoMinutesOfMeeting;
        private readonly IRepository<UserLogin> repoUserLogin;
        public MinutesOfMeetingMasterService(IRepository<MeetingMaster> repoMinutesOfMeeting, IRepository<UserLogin> repoUserLogin)
        {
            this.repoMinutesOfMeeting = repoMinutesOfMeeting;
            this.repoUserLogin = repoUserLogin;
        }
        public List<MeetingMaster> GetMinutesOfMeetingByPaging(out int total, PagingService<MeetingMaster> pagingService)
        {
            return repoMinutesOfMeeting.Query()
                   .Filter(pagingService.Filter)
                   .OrderBy(pagingService.Sort)                  
                   .GetPage(pagingService.Start, pagingService.Length, out total)
                  .ToList();
        }
        public MeetingMaster GetMinutesOfMeetingFindById(int id)
        {
            return repoMinutesOfMeeting.FindById(id);
        }
        public MeetingMaster Save(MeetingMasterDto model)
        {
            MeetingMaster entity = model.Id > 0 ? GetMinutesOfMeetingFindById(model.Id) : new MeetingMaster();

            if (entity != null)
            {
                if (entity.Id > 0 && entity.CreatedByUID != model.CreateByUid)
                {
                    return null;
                }
                entity.Title = model.Title;
                entity.ModifiedDate = DateTime.Now;
                if (entity.Id == 0)
                {
                    entity.CreatedDate = DateTime.Now;
                    entity.CreatedByUID = model.CreateByUid;
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
                repoMinutesOfMeeting.Delete(Id);
                return true;
            }
            return false;
        }
        public void Dispose()
        {
            if (repoMinutesOfMeeting != null)
            {
                repoMinutesOfMeeting.Dispose();
            }
        }
    }
}
