using EMS.Data;
using EMS.Dto;
using EMS.Dto.CVEstimatePrice;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service.CVEstimatePrice
{
    public interface ICVEstimatePriceService
    {
        CvBuilderEstimatePrice GetPricebyId(int id);
        List<CvBuilderEstimatePrice> GetExperincePricesbyTechnology(int RoleId, int TechId);
        List<CvBuilderEstimatePrice> GetExpericnePricesbyRole(int RoleId);
        List<CvBuilderEstimatePrice> GetCvBuilderEstimatePrice(out int total,PagingService<CvBuilderEstimatePrice> pagingService);
        CvBuilderEstimatePrice GetPriceListById(int Id);
        bool Save(CvBuilderEstimatePrice cvBuilderEstimatePrice);
        bool SaveCollection(List<CvBuilderEstimatePrice> entityCollection);
        List<CvBuilderEstimatePrice> GetRecordsById(int RoleId, int TechnologyId);
        List<CvBuilderEstimatePrice> GetRecordsByRoleId(int RoleId);
        void Delete(CvBuilderEstimatePrice entity);
        List<CvBuilderEstimatePrice> GetExperincePricesbyTechnologyId(int RoleId, int TechnologyId);
        void DeleteCollection(List<CvBuilderEstimatePrice> entityCollection);
        //List<ExperienceType> GetExperienceList();
        List<CVEstimatePriceResponseDto> GetCvBuilderEstimatePriceSP(int RowStart, int RowEnd, string SearchResult, int ColumnOrder, string OrderDirection);
    }
}
