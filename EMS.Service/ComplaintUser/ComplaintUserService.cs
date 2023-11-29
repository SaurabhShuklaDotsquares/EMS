using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using EMS.Dto.ComplaintUser;
using EMS.Repo;

namespace EMS.Service
{
    public class ComplaintUserService : IComplaintUserService
    {
        //private readonly IRepository<ComplaintUser> repoComplaintUser;
        //public ComplaintUserService(IRepository<ComplaintUser> _repoComplaintUser)
        //{
        //    this.repoComplaintUser = _repoComplaintUser;
        //}

        //public ComplaintUser Save(ComplaintUserDto model)
        //{
        //    ComplaintUser entity = new ComplaintUser();
        //    if (model != null)
        //    {
        //        entity.UserLoginId = model.UserLoginId;
        //        entity.ComplaintId = model.ComplaintId;

        //        repoComplaintUser.Insert(entity);
        //        repoComplaintUser.SaveChanges();
        //    }

        //    return entity;
        //}

        //public void Dispose()
        //{
        //    if (repoComplaintUser != null)
        //    {
        //        repoComplaintUser.Dispose();
        //    }
        //}


    }
}
