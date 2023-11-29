using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service
{
    public interface IEstimatePriceService : IDisposable
    {
         List<EstimateRoleExp> GetEstimateRoleExpList();
        List<EstimateRoleExp> GetEstimateRoleExpList(int estimateRoleId);
        List<EstimateRole> GetEstimateRoleList();
        List<TechnologyParent> GettechnologyParentList();
        EstimateRoleTechnoloyPrice GetEstimateRoleTechnoloyPrice(int Id);
        EstimateRoleTechnoloyPrice GetEstimateRoleTechnoloyPrice(int estimateRoleExpId, int? technologyParentId);
        void SaveEstimatePrice(List<EstimateRoleTechnoloyPrice> entity);
        EstimateRoleTechnoloyPrice GetExpPriceByExpIdAndTechId(int EstimateRoleExpID, int? TechnologyParentId);
        List<EstimateRoleTechnoloyPrice> GetEstimateRoleTechnoloyPriceByRoleId(int estimateRoleId);
        List<EstimateRoleTechnoloyPriceDto> GetEstimateRoleTechnoloyPriceDto(int roleId = 0);
    }
}
