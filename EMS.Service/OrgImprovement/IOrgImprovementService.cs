using EMS.Core;
using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;

namespace EMS.Service
{
    public interface IOrgImprovementService
    {
        List<OrgImprovement> GetImprovementByPaging(out int total, PagingService<OrgImprovement> pagingSerices);
        bool Save(OrgImprovementDto model);

        OrgImprovement GetImprovementById(int ID);
        void Delete(int id);
        List<OrgImprovement> GetImprovementsInDuration(int uid, DateTime? startDate, DateTime? endDate, Enums.ImprovementType typeId);
    }
}
