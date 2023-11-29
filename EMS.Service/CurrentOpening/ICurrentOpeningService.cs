using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
namespace EMS.Service
{
   public interface ICurrentOpeningService : IDisposable
    {
        CurrentOpening GetCurrentOpeningById(int? id);
        List<CurrentOpening> GetCurrentOpenings();
        List<CurrentOpening> GetCurrentOpeningByPage(out int total, PagingService<CurrentOpening> pagingService);
        void Save(CurrentOpening entity);
        void Delete(CurrentOpening entity);
    }
}
