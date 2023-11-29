using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service
{
    public interface IEstimateHostingPackageService : IDisposable
    {
        List<EstimateHostingPackage> GetByPaging(out int total, PagingService<EstimateHostingPackage> pagingSerices);
        EstimateHostingPackageDto GetById(int id);
        bool Save(EstimateHostingPackageDto model);

        List<EstimateHostingPackage> GetByTechnologies(List<int> technologies, int? countryId = null);
        List<EstimateCountry> GetByCountryByTechnologies(List<int> technologies);
    }
}
