using EMS.Core;
using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;

namespace EMS.Service
{
    public interface IProductLandingService : IDisposable
    {
        ProductLanding GetProductLandingById(int id);

        ProductLanding Save(ProductLandingDto model);
        
        void AddScreenshot(int id, string screenshotUrl, string description);

        List<ProductLanding> GetProductLandingByPaging(out int total, PagingService<ProductLanding> pagingService);

    }
}



