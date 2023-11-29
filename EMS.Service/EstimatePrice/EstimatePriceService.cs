using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMS.Service
{
    public class EstimatePriceService : IEstimatePriceService
    {
        #region "Fields"
        private IRepository<EstimateRoleTechnoloyPrice> repoEstimatePrice;
        private IRepository<EstimateRoleExp> repoEstimateRoleExp;
        private IRepository<EstimateRole> repoEstimateRole;
        private IRepository<TechnologyParent> repoTechnologyParent;

        #endregion
        #region "Cosntructor"
        public EstimatePriceService(IRepository<EstimateRoleTechnoloyPrice> _repoEstimatePrice, IRepository<EstimateRoleExp> _repoEstimateRoleExp, IRepository<EstimateRole> _repoEstimateRole, IRepository<TechnologyParent> _repoTechnologyParent)
        {
            this.repoEstimatePrice = _repoEstimatePrice;
            this.repoEstimateRoleExp = _repoEstimateRoleExp;
            this.repoEstimateRole = _repoEstimateRole;
            this.repoTechnologyParent = _repoTechnologyParent;
        }
        #endregion

        public List<EstimateRoleExp> GetEstimateRoleExpList()
        {
            return repoEstimateRoleExp.Query().Filter(R => R.IsActive).Get().ToList();
        }
        public List<EstimateRoleExp> GetEstimateRoleExpList(int estimateRoleId)
        {
            return repoEstimateRoleExp.Query().Filter(x => x.IsActive && x.EstimateRoleId == estimateRoleId).Get().ToList();
        }
        public List<EstimateRole> GetEstimateRoleList()
        {
            return repoEstimateRole.Query().Filter(R => R.IsActive).Get().ToList();
        }
        public List<TechnologyParent> GettechnologyParentList()
        {
            return repoTechnologyParent.Query().Filter(R => R.IsActive == true).Get().OrderBy(R => R.Title).ToList();
        }

        public EstimateRoleTechnoloyPrice GetEstimateRoleTechnoloyPrice(int Id)
        {
            return repoEstimatePrice.FindById(Id);
        }

        public EstimateRoleTechnoloyPrice GetEstimateRoleTechnoloyPrice(int estimateRoleExpId, int? technologyParentId)
        {
            return repoEstimatePrice.Query().Filter(x => x.EstimateRoleExpId == estimateRoleExpId && x.EstimateTechnologyId == technologyParentId).Get().FirstOrDefault();
        }

        public List<EstimateRoleTechnoloyPrice> GetEstimateRoleTechnoloyPriceByRoleId(int estimateRoleId)
        {
            return repoEstimatePrice.Query().Filter(x => x.EstimateRoleExp.IsActive && x.EstimateRoleExp.EstimateRoleId == estimateRoleId).Get().ToList();
        }
        public List<EstimateRoleTechnoloyPriceDto> GetEstimateRoleTechnoloyPriceDto(int roleId = 0)
        {
            return repoEstimatePrice.Query().Filter(x => (roleId == 0 || x.EstimateRoleExp.EstimateRoleId == roleId)).Get().Select(item => new EstimateRoleTechnoloyPriceDto
            {
                Id = item.Id,
                EstimateRoleExpId = item.EstimateRoleExpId,
                EstimateRoleExpName = item.EstimateRoleExp.Name,
                EstimateTechnologyId = item.EstimateTechnologyId,
                EstimateTechnologyName = item.EstimateTechnology?.Title,
                Price = item.Price,
                EstimateRoleId = item.EstimateRoleExp.EstimateRoleId,
                EstimateRoleName = item.EstimateRoleExp.EstimateRole.Name
            }).ToList();
        }

        public void SaveEstimatePrice(List<EstimateRoleTechnoloyPrice> entityList)
        {
            foreach (var item in entityList)
            {
                var entity = GetEstimateRoleTechnoloyPrice(item.EstimateRoleExpId, item.EstimateTechnologyId);
                if (entity != null)
                {
                    entity.ModifiedDate = DateTime.Now;
                    entity.Price = item.Price;
                    entity.EstimateTechnologyId = item.EstimateTechnologyId;
                    repoEstimatePrice.Update(entity);
                }
                else
                {
                    item.CreatedDate = DateTime.Now;
                    item.ModifiedDate = DateTime.Now;
                    repoEstimatePrice.InsertCallback(item);
                }
            }
        }
        public EstimateRoleTechnoloyPrice GetExpPriceByExpIdAndTechId(int EstimateRoleExpID, int? TechnologyParentId)
        {
            return repoEstimatePrice.Query().Filter(x => (x.EstimateRoleExpId == EstimateRoleExpID && x.EstimateTechnologyId == TechnologyParentId)).Get().FirstOrDefault();
        }

        #region "Dispose"
        public void Dispose()
        {
            if (repoEstimatePrice != null)
            {
                repoEstimatePrice.Dispose();
                repoEstimatePrice = null;
            }
            if (repoEstimateRole != null)
            {
                repoEstimateRole.Dispose();
                repoEstimateRole = null;
            }
            if (repoEstimatePrice != null)
            {
                repoEstimatePrice.Dispose();
                repoEstimatePrice = null;
            }
            if (repoTechnologyParent != null)
            {
                repoTechnologyParent.Dispose();
                repoTechnologyParent = null;
            }
        }
        #endregion
    }
}
