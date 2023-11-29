using EMS.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service
{
    public interface IEstimateService : IDisposable
    {
        List<SelectListItem> GetEstimateRoleDropdown();
        List<EstimateRole> GetActiveRoleCategory();
        List<EstimateTechnology> GetActiveTechnologyCategory();
        List<SelectListItem> GetEstimateRoleExpDropdown(int estimateRoleId);
        EstimateRoleTechnoloyPrice GetEstimateRoleTechnoloyPrice(int roleId, int estimateRoleExpId, int? technologyParentId);
        EstimateRoleTechnoloyPrice GetEstimateRoleTechnoloyPrice(int roleId, int? technologyParentId);
        EstimatePriceCalculation Save(EstimatePriceCalculation entity);
        bool IsEstimationPriceExists(string CRMLeadId, int estimatemodelid, string estimateName);
        EstimatePriceCalculation GetEstimatePriceCalculationByCRM(string CRMLeadId);
        EstimatePriceCalculation GetEstimatePriceCalculation(string CRMLeadId, int estimateModelId, string estimateName);
        void DeleteEstimatePriceCalculation(string CRMLeadId, int estimateModelId, string estimateName);
        EstimateRole GetEstimateRole(int roleId);
        EstimateRoleExp GetEstimateRoleExp(int expRoleId);
        List<EstimatePriceCalculation> GetEstimatePriceCalculationByCRMUserId(string CRMLeadId, int RoleId, int PmUId);

        List<SelectListItem> GetEstimateTechnologyItemList();
        List<SelectListItem> GetCountrySelectList();
        List<SelectListItem> GetEstimateModelSelectList();
        EstimateTechnology GetTechnologiesById(int Id);
    }
}
