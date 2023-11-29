using EMS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service
{
    public interface IEstimateHourService
    {
        List<EstimateHour> GetEstimateHourList();
        List<EstimateHour> GetEstimateHourByPaging(out int total, PagingService<EstimateHour> pagingSerices);
        EstimateHour Save(EstimateHour model);
        EstimateHour GetEstimateHourById(int id);
        List<EstimateHourFileNameType> GetEstimateHourFileNameTypes();
        EstimateHourFileNameType SaveEstimateHourFileNameType(EstimateHourFileNameType entity);
        EstimateHourFileNameType GetEstimateHourFileNameTypeById(int id);
    }
}
