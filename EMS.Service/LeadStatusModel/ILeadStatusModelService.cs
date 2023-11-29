using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Dto;

namespace EMS.Service
{
    public interface ILeadStatusModelService
    {
        List<LeadStatu> GetLeadStatusModelList();
        LeadStatu GetLeadStatusModelById(int id);
        LeadStatu Save(LeadStatusModelDto model);
        bool IsLeadStatusModelExists(int statusId, string statusName);
        List<LeadStatu> GetLeadStatusModelByPaging(out int total, PagingService<LeadStatu> pagingService);
        void DeleteLeadStatusModelById(int id);
    }
}
