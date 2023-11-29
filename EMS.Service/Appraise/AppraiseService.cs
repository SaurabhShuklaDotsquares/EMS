using System;
using System.Collections.Generic;
using System.Linq;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using EMS.Core;
using System.Linq.Expressions;
using EMS.Data.Model;

namespace EMS.Service
{
    public class AppraiseService : IAppraiseService
    {
        #region Constructor and Fileds

        private IRepository<EmployeeAppraise> repoEmployeeAppraise;

        public AppraiseService(IRepository<EmployeeAppraise> repoEmployeeAppraise)
        {
            this.repoEmployeeAppraise = repoEmployeeAppraise;
        }

        #endregion

        public List<EmployeeAppraise> GetData()
        {
            return repoEmployeeAppraise.Query().Get().ToList();
        }

        public void Dispose()
        {
            if (repoEmployeeAppraise != null)
            {
                repoEmployeeAppraise.Dispose();
                repoEmployeeAppraise = null;
            }

        }

        public List<EmployeeAppraise> GetAppraiseByPaging(out int total, PagingService<EmployeeAppraise> pagingSerices)
        {
            return repoEmployeeAppraise.Query().Filter(pagingSerices.Filter).
                 OrderBy(pagingSerices.Sort).
                 GetPage(pagingSerices.Start, pagingSerices.Length, out total).
                 ToList();
        }

        public EmployeeAppraise GetAppraiseData(int id)
        {
            return repoEmployeeAppraise.FindById(id);
        }

        public void Delete(int id)
        {
            var entity = GetAppraiseData(id);
            if (entity != null)
            {
                entity.IsDelete = true;
                repoEmployeeAppraise.SaveChanges();
            }
        }

        public EmployeeAppraise Save(AppraiseDto model)
        {
            EmployeeAppraise entity = null;
            if (model.Id > 0)
            {
                entity = GetAppraiseData(model.Id);

                if (entity == null)
                {
                    return null;
                }
            }
            else
            {
                entity = new EmployeeAppraise
                {
                    AddDate = DateTime.Now,
                    IsActive = true,
                    IsDelete = false
                };
            }

            entity.UserId = model.UId;
            entity.EmployeeId = model.EmployeeId;
            entity.ClientComment = model.ClientComment;
            entity.ClientDate = model.ClientDate.ToDateTime("dd/MM/yyyy");
            entity.TlComment = model.TlComment;
            entity.ModifyDate = DateTime.Now;
            entity.IP = model.IP;
            entity.AppraiseType = model.AppraiseId;
            entity.ProjectId = model.ProjectId;
            entity.Priority = (byte?)model.PriorityId;

            if (entity.Id > 0)
            {
                repoEmployeeAppraise.SaveChanges();
            }
            else
            {
                repoEmployeeAppraise.Insert(entity);
            }

            return entity;
        }

        public List<EmployeeAppraise> GetAppraises(Expression<Func<EmployeeAppraise, bool>> filter)
        {
            return repoEmployeeAppraise.Query().Filter(filter).Get().ToList();
        }

        public List<EmployeeAppraise> GetAppraiseUserIdById(int id)
        {
            return repoEmployeeAppraise.Query().Filter(x => x.EmployeeId == id).Get().ToList();
        }

        public List<EmployeeAppraise> GetAppraiseUserIdByPMId(int id)
        {
            return repoEmployeeAppraise.Query().Filter(x => x.UserId == id && x.IsDelete == false).Get().ToList();
        }
    }
}
