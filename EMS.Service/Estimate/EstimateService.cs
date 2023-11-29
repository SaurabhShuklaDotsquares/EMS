using EMS.Core;
using EMS.Data;
using EMS.Repo;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMS.Service
{
    public class EstimateService : IEstimateService
    {
        private readonly IRepository<EstimateRole> repoEstimateRole;
        private readonly IRepository<EstimateRoleExp> repoEstimateRoleExp;
        private readonly IRepository<EstimateRoleTechnoloyPrice> repoEstimateRoleTechnoloyPrice;
        private readonly IRepository<EstimatePriceCalculation> repoEstimatePriceCalculation;
        private readonly IRepository<EstimateTechnology> repoEstimateTechnology;
        private readonly IRepository<EstimateCountry> repoEstimateCountry;
        private readonly IRepository<EstimateModel> repoEstimateModel;

        public EstimateService(IRepository<EstimateRole> _repoEstimateRole,
            IRepository<EstimateRoleExp> _repoEstimateRoleExp,
            IRepository<EstimateRoleTechnoloyPrice> _repoEstimateRoleTechnoloyPrice,
            IRepository<EstimatePriceCalculation> _repoEstimatePriceCalculation,
            IRepository<EstimateTechnology> _repoEstimateTechnology,
            IRepository<EstimateCountry> _repoEstimateCountry,
            IRepository<EstimateModel> _repoEstimateModel)
        {
            repoEstimateRole = _repoEstimateRole;
            repoEstimateRoleExp = _repoEstimateRoleExp;
            repoEstimateRoleTechnoloyPrice = _repoEstimateRoleTechnoloyPrice;
            repoEstimatePriceCalculation = _repoEstimatePriceCalculation;
            repoEstimateTechnology = _repoEstimateTechnology;
            repoEstimateCountry = _repoEstimateCountry;
            repoEstimateModel = _repoEstimateModel;
        }

        public void Dispose()
        {
            if (repoEstimateRole != null)
            {
                repoEstimateRole.Dispose();
            }
            if (repoEstimateRoleExp != null)
            {
                repoEstimateRoleExp.Dispose();
            }
            if (repoEstimateTechnology != null)
            {
                repoEstimateTechnology.Dispose();
            }
            if (repoEstimateCountry != null)
            {
                repoEstimateCountry.Dispose();
            }
            if (repoEstimateModel != null)
            {
                repoEstimateModel.Dispose();
            }
        }

        public List<SelectListItem> GetEstimateRoleDropdown()
        {
            return repoEstimateRole.Query().Filter(x => x.IsActive).Get().Select(item =>
            new SelectListItem
            {
                Value = item.Id.ToString(),
                Text = item.Name
            }).ToList();
        }


        public List<SelectListItem> GetEstimateRoleExpDropdown(int estimateRoleId)
        {
            return repoEstimateRoleExp.Query().Filter(x => x.IsActive && x.EstimateRoleId == estimateRoleId).Get().Select(item =>
             new SelectListItem
             {
                 Value = item.Id.ToString(),
                 Text = item.Name
             }).ToList();
        }

        public EstimateRoleTechnoloyPrice GetEstimateRoleTechnoloyPrice(int roleId, int estimateRoleExpId, int? technologyParentId)
        {
            return repoEstimateRoleTechnoloyPrice.Query()
                .Filter(x => x.EstimateRoleExp.EstimateRoleId == roleId && x.EstimateRoleExp.IsActive && x.EstimateRoleExpId == estimateRoleExpId && x.EstimateTechnologyId == technologyParentId)
                .Get().FirstOrDefault();

        }

        public EstimateRoleTechnoloyPrice GetEstimateRoleTechnoloyPrice(int roleId, int? technologyParentId)
        {
            return repoEstimateRoleTechnoloyPrice.Query()
                .Filter(x => x.EstimateRoleExp.EstimateRoleId == roleId && x.EstimateRoleExp.IsActive && x.EstimateTechnologyId == technologyParentId)
                .Get().FirstOrDefault();
        }

        public EstimatePriceCalculation Save(EstimatePriceCalculation entity)
        {
            if (entity.Id == 0)
            {
                entity.CreatedDate = DateTime.Now;
                entity.ModifiedDate = DateTime.Now;
                repoEstimatePriceCalculation.InsertGraph(entity);
            }
            else
            {
                entity.ModifiedDate = DateTime.Now;
                var estimateHourEntity = repoEstimatePriceCalculation.FindById(entity.Id);
                var estimateHourUpdated = repoEstimatePriceCalculation.Update(estimateHourEntity, entity);

                repoEstimatePriceCalculation.SaveChanges();
            }
            return entity;
        }
        public List<EstimatePriceCalculation> GetEstimatePriceCalculationByCRMUserId(string CRMLeadId, int RoleId, int PmUId)
        {
            if (RoleId == (int)Enums.UserRoles.PMO || RoleId == (int)Enums.UserRoles.UKPM
                       || RoleId == (int)Enums.UserRoles.UKBDM || RoleId == (int)Enums.UserRoles.Director
                       || RoleId == (int)Enums.UserRoles.PMOAU || RoleId == (int)Enums.UserRoles.AUPM)
            {
                return repoEstimatePriceCalculation.Query()
                      .Filter(x => x.CrmleadId == CRMLeadId)
                      .Get().ToList();
            }

            else
            {
                return repoEstimatePriceCalculation.Query()
                          .Filter(x => x.CrmleadId == CRMLeadId &&
                          (x.CreatedByUid == null || x.CreatedByU.PMUid == PmUId || x.CreatedByU.Uid == PmUId))
                          .Get().ToList();
            }
        }
        public EstimatePriceCalculation GetEstimatePriceCalculationByCRM(string CRMLeadId)
        {
            return repoEstimatePriceCalculation.Query()
                .Filter(x => x.CrmleadId == CRMLeadId)
                .Get().FirstOrDefault();
        }

        public EstimatePriceCalculation GetEstimatePriceCalculation(string CRMLeadId, int estimateModelId, string estimateName)
        {
            return repoEstimatePriceCalculation.Query()
                .Filter(x => x.CrmleadId == CRMLeadId && x.EstimateModelId == estimateModelId && x.EstimateName == estimateName)
                .Get().FirstOrDefault();
        }

        public void DeleteEstimatePriceCalculation(string CRMLeadId, int estimateModelId, string estimateName)
        {
            EstimatePriceCalculation estimatePriceCalculation = GetEstimatePriceCalculation(CRMLeadId, estimateModelId, estimateName);
            repoEstimatePriceCalculation.ChangeEntityCollectionState(estimatePriceCalculation.EstimatePriceCalculationDetail, ObjectState.Deleted);
            repoEstimatePriceCalculation.ChangeEntityState(estimatePriceCalculation, ObjectState.Unchanged);
            repoEstimatePriceCalculation.SaveChanges();
        }

        public bool IsEstimationPriceExists(string CRMLeadId, int estimateModelId, string estimateName)
        {
            bool isExist = repoEstimatePriceCalculation.Query().Filter(x => x.CrmleadId.ToLower().Equals(CRMLeadId.ToLower()) && x.EstimateModelId == estimateModelId && x.EstimateName == estimateName).Get().FirstOrDefault() != null;
            return isExist;
        }

        public EstimateRole GetEstimateRole(int roleId)
        {
            return repoEstimateRole.Query()
                .Filter(x => x.Id == roleId)
                .Get().FirstOrDefault();
        }
        public EstimateRoleExp GetEstimateRoleExp(int expRoleId)
        {
            return repoEstimateRoleExp.Query()
                .Filter(x => x.Id == expRoleId)
                .Get().FirstOrDefault();
        }

        public List<SelectListItem> GetEstimateTechnologyItemList()
        {
            return repoEstimateTechnology.Query().Filter(x => x.IsActive).Get().OrderBy(x => x.Title).Select(item => new SelectListItem
            {
                Text = item.Title,
                Value = item.Id.ToString()
            }).ToList();
        }

        public List<SelectListItem> GetCountrySelectList()
        {
            return repoEstimateCountry.Query().Filter(x => x.IsActive).Get().OrderBy(x => x.Name).Select(item => new SelectListItem
            {
                Text = item.Name,
                Value = item.Id.ToString()
            }).ToList();
        }

        public List<SelectListItem> GetEstimateModelSelectList()
        {
            return repoEstimateModel.Query().Filter(x => x.IsActive).Get().OrderBy(x => x.Name).Select(item => new SelectListItem
            {
                Text = item.Name,
                Value = item.Id.ToString()
            }).ToList();
        }

        //public List<EstimatePriceCalculation> GetEstimatePriceCalculations(string CRMLeadId)
        //{
        //    return repoEstimatePriceCalculation.Query()
        //        .Filter(x => x.CrmleadId == CRMLeadId)
        //        .Get().ToList();
        //}
        public List<EstimateRole> GetActiveRoleCategory()
        {
            return repoEstimateRole.Query().Filter(x => x.IsActive == true).Get().OrderBy(x => x.Name).ToList();
        }

        public List<EstimateTechnology> GetActiveTechnologyCategory()
        {
            return repoEstimateTechnology.Query().Filter(x => x.IsActive == true).Get().OrderBy(x => x.Title).ToList();
        }

        public EstimateTechnology GetTechnologiesById(int Id)
        {
            return repoEstimateTechnology.Query().Filter(x => x.Id == Id).Get().FirstOrDefault();
        }


    }
}
