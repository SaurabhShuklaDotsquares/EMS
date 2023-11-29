using System;
using System.Collections.Generic;
using System.Linq;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using EMS.Core;
using EMS.Data.Model;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;

namespace EMS.Service
{
    public class ComplaintService : IComplaintService
    {
        #region Constructor and Fields

        private readonly IRepository<Complaint> repoComplaint;
        private readonly IRepository<UserLogin> repoUserLogin;
        private readonly IRepository<ComplaintUser> repoComplaintUser;
        public ComplaintService(IRepository<Complaint> repoComplaint, IRepository<UserLogin> repoUserLogin, IRepository<ComplaintUser> repoComplaintUser)
        {
            this.repoComplaint = repoComplaint;
            this.repoUserLogin = repoUserLogin;
            this.repoComplaintUser = repoComplaintUser;
        }

        #endregion

        public bool Delete(int id)
        {
            var entity = GetComplaintById(id);
            if (entity != null)
            {
                entity.IsDelete = true;
                repoComplaint.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public Complaint GetComplaintById(int id)
        {
            return repoComplaint.FindById(id);
        }
        public List<ComplaintUser> GetComplaintUserById(int id)
        {
            return repoComplaintUser.Query().Filter(x => x.ComplaintId == id).Get().ToList();
        }
        public List<ComplaintUser> GetComplaintUserIdById(int id)
        {
            return repoComplaintUser.Query().Filter(x => x.UserLoginId == id).Get().ToList();
        }

        public List<Complaint> GetComplaintUserIdByPMId(int id)
        {
            return repoComplaint.Query().Filter(x => x.AddedBy == id && x.IsDelete == false).Get().ToList();
        }

        public List<Complaint> GetComplaintByPaging(out int total, PagingService<Complaint> pagingService)
        {
            return repoComplaint.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetPage(pagingService.Start, pagingService.Length, out total)
                .ToList();
        }

        public Complaint Save(ComplaintDto model)
        {
            Complaint entity = null;
            if (model.Id > 0)
            {
                entity = GetComplaintById(model.Id);

                if (entity == null)
                {
                    return null;
                }

                repoComplaint.ChangeEntityCollectionState(entity.ComplaintUser, ObjectState.Deleted);
                entity.ComplaintUser.Clear();
            }
            else
            {
                entity = new Complaint
                {
                    AddedBy = model.AddedBy,
                    AddedDate = DateTime.Now,
                    IsDelete = false,
                };

                //for (int i = 0; i < model.Employees.Count(); i++)
                //{
                //    ComplaintUser complaintUser = new ComplaintUser
                //    {
                //        UserLoginId = model.Employees[i]

                //    };
                //    entity.ComplaintUser.Add(complaintUser);
                //}

            }

            //entity.EmployeeId = model.EmployeeId;
            entity.ComplaintType = (byte)model.ComplaintTypeId;
            entity.Priority = (byte)model.PriorityId;
            entity.TlComplainDate = model.TlComplainDate.ToDateTime("dd/MM/yyyy");
            entity.TlExplanation = model.TlExplanation;
            entity.ClientComplain = model.ClientComplain;
            entity.ClientComplainDate = model.ClientComplainDate.ToDateTime("dd/MM/yyyy");
            entity.DeveloperExplanation = model.DeveloperExplanation;
            entity.DeveloperComplainDate = model.DeveloperComplainDate.ToDateTime("dd/MM/yyyy");
            entity.AreaofImprovement = model.AreaofImprovement;
            entity.LessionLearned = model.LessionLearned;
            entity.ModifyDate = DateTime.Now;
            entity.ProjectId = model.ProjectId;
            for (int i = 0; i < model.Employees.Count(); i++)
            {
                ComplaintUser complaintUser = new ComplaintUser
                {
                    UserLoginId = model.Employees[i]

                };
                entity.ComplaintUser.Add(complaintUser);
            }
            if (entity.Id > 0)
            {
                repoComplaint.SaveChanges();
            }
            else
            {
                repoComplaint.Insert(entity);
            }


            return entity;
        }

        public void Dispose()
        {
            if (repoComplaint != null)
            {
                repoComplaint.Dispose();
            }

            if (repoUserLogin != null)
            {
                repoUserLogin.Dispose();
            }
        }
        ///// <summary>
        ///// returns complaints for user in date range
        ///// </summary>
        ///// <param name="uid">uid</param>
        ///// <param name="startDate">startDate</param>
        ///// <param name="endDate">endDate</param>
        ///// <returns>Complaints list</returns>
        //public List<Complaint> GetComplaints(int uid,DateTime startDate,DateTime endDate)
        //{
        //    return repoComplaint.Query().Filter(c => c.ComplaintUser.Any(cu => cu.UserLoginId == uid) &&
        //    c.AddedDate.Date >=
        //    startDate && c.AddedDate.Date <= endDate).Get().ToList();
        //}

        public List<Complaint> GetComplaintsByFilter(Expression<Func<Complaint, bool>> filter)
        {
            return repoComplaint.Query().Filter(filter).Get().ToList();
        }

      
        public List<Complaint> GetProjectByComplainIds(List<int> currentUserProjectComplaintId)
        {
            return repoComplaint.Query().Filter(x => currentUserProjectComplaintId.Contains(x.Id)).Get().ToList();
        }

        public List<ComplaintUser> GetEmployeeByIds(List<int> currentUserProjectComplaintId)
        {
            return repoComplaintUser.Query().Filter(x => currentUserProjectComplaintId.Contains(x.ComplaintId)).Get().ToList();
        }

    }
}
