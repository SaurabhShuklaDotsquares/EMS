using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using EMS.Core;
using EMS.Data.Model;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EMS.Service
{
    public class EventService : IEventService
    {
        #region Constructor and Fields

        private readonly IRepository<OfficialLeave> repoOfficialLeave;
        private readonly IRepository<UserLogin> repoUserLogin;
        public EventService(IRepository<OfficialLeave> _repoOfficialLeave, IRepository<UserLogin> repoUserLogin)
        {
            this.repoOfficialLeave = _repoOfficialLeave;
            this.repoUserLogin = repoUserLogin;
        }

        #endregion

        public bool Delete(OfficialLeave entity)
        {
            if (entity != null)
            {
                repoOfficialLeave.Delete(entity);
                return true;
            }
            else
            {
                return false;
            }
        }

        public OfficialLeave GetEventById(int id)
        {
            return repoOfficialLeave.FindById(id);
        }
        //public List<ComplaintUser> GetEventUserById(int id)
        //{
        //    return repoComplaintUser.Query().Filter(x => x.ComplaintId == id).Get().ToList();
        //}


        public List<OfficialLeave> GetEventByPaging(out int total, PagingService<OfficialLeave> pagingService)
        {
            return repoOfficialLeave.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetPage(pagingService.Start, pagingService.Length, out total)
                .ToList();
        }

        public OfficialLeave Save(EventDto model)
        {
            OfficialLeave entity = new OfficialLeave();
            
            entity.LeaveId = model.LeaveId;
            entity.Title = model.Title;
            entity.LeaveDate = (DateTime)model.LeaveDate.ToDateTime("dd/MM/yyyy");
            entity.CountryId = model.CountryId;
            entity.IsActive = model.IsActive;
            entity.LeaveType = model.LeaveType;
            
            if (entity.LeaveId > 0)
            {
                repoOfficialLeave.Update(entity);
            }
            else
            {
                repoOfficialLeave.Insert(entity);
            }

            return entity;
        }

        public OfficialLeave ApprovedStatus(OfficialLeave entity)
        {
            repoOfficialLeave.Update(entity);
            return entity;
        }
        public void Dispose()
        {
            if (repoOfficialLeave != null)
            {
                repoOfficialLeave.Dispose();
            }

            if (repoUserLogin != null)
            {
                repoUserLogin.Dispose();
            }
        }
       
        public List<OfficialLeave> GetEventByFilter(Expression<Func<OfficialLeave, bool>> filter)
        {
            return repoOfficialLeave.Query().Filter(filter).Get().ToList();
        }
    }
}
