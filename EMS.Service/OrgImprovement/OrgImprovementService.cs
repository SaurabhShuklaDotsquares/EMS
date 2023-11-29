using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EMS.Service
{
    public class OrgImprovementService : IOrgImprovementService
    {
        #region "Constructor & members"
        private IRepository<OrgImprovement> repoOrgImprovement;
        public OrgImprovementService(IRepository<OrgImprovement> _repoImprovement)
        {
            this.repoOrgImprovement = _repoImprovement;
        }
        #endregion
        public List<OrgImprovement> GetImprovementByPaging(out int total, PagingService<OrgImprovement> pagingSerices)
        {
            return repoOrgImprovement.Query().Filter(pagingSerices.Filter).
                 OrderBy(pagingSerices.Sort).
                 GetPage(pagingSerices.Start, pagingSerices.Length, out total).
                 ToList();
        }
        public bool Save(OrgImprovementDto model)
        {
            OrgImprovement entity = model.Id > 0 ? GetImprovementById(model.Id) : new OrgImprovement();
            if (entity != null)
            {
                entity.Title = model.Title;
                entity.TypeId = (byte)model.TypeId;
                entity.ImprovementDate = (DateTime)model.ImprovementDate.ToDateTime("dd/MM/yyyy");
                entity.Description = model.Description;
                entity.ModifyDate = DateTime.Now;
                entity.EmployeeUid = model.EmployeeUid;
                if (entity.Id == 0)
                {
                    entity.AddedByUid = model.CurrentUserId;
                    entity.AddDate = DateTime.Now;
                    repoOrgImprovement.Insert(entity);
                }
                else
                {
                    repoOrgImprovement.SaveChanges();
                }
                return true;
            }
            return false;

        }

        public OrgImprovement GetImprovementById(int ID)
        {
            return repoOrgImprovement.FindById(ID);
        }
        public void Delete(int id)
        {
            var entity = GetImprovementById(id);
            if (entity != null)
            {
                repoOrgImprovement.Delete(entity);
            }
        }
        public void Dispose()
        {
            if (repoOrgImprovement != null)
            {
                repoOrgImprovement.Dispose();
                repoOrgImprovement = null;
            }

        }
        public List<OrgImprovement> GetImprovementsInDuration(int uid,DateTime? startDate,DateTime? endDate,Enums.ImprovementType typeId)
        {
            var expr = PredicateBuilder.True<OrgImprovement>();
            expr = expr.And(o => o.EmployeeUid == uid && o.TypeId == (byte)typeId);
            if (startDate.HasValue)
            {
                expr = expr.And(o => o.ImprovementDate >= startDate);
            }
            if (endDate.HasValue)
            {
                expr = expr.And(o => o.ImprovementDate <= endDate);
            }
            //return repoOrgImprovement.Query()
            //    .Filter(o => o.EmployeeUid == uid 
            //    && o.ImprovementDate >= startDate 
            //    && o.ImprovementDate <= endDate
            //    && o.TypeId==(byte)typeId).Get().ToList();
            return repoOrgImprovement.Query()
                .Filter(expr).Get().ToList();
        }
    }
}
