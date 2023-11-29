using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service
{
    public interface ICVBuilderService
    {
        void Save(CVBuilderDto model, string filepath);
        void SaveOld(CVBuilder_Dto model);
        Cvbuilder cvBuilderFindById(long Id);
        Cvbuilder cvBuilderFindByUserId(int userId);
        List<Cvbuilder> GetCVBuilderByPaging(out int total, PagingService<Cvbuilder> pagingService);
        void UpdateApprovedStatus(Cvbuilder entity);
        CvBuilderEstimatePrice GetEstimateRoleTechnoloyPrice(int roleId, int estimateRoleExpId, long? technologyParentId);
        List<CVSpResponse> GetCVBuilderDatasp(out int total, CVSearchRequest SearchFilter, string action);
    }
}
