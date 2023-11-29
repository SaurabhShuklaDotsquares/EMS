using System;
using System.Collections.Generic;
using System.Text;
using EMS.Data;
using EMS.Data.Model;
using EMS.Dto;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EMS.Service
{
    public interface IEventService : IDisposable
    {
        List<OfficialLeave> GetEventByPaging(out int total, PagingService<OfficialLeave> pagingService);
        OfficialLeave Save(EventDto model);
        OfficialLeave GetEventById(int id);        
        bool Delete(OfficialLeave entity);
        List<OfficialLeave> GetEventByFilter(Expression<Func<OfficialLeave, bool>> filter);
        OfficialLeave ApprovedStatus(OfficialLeave model);
    }
}
