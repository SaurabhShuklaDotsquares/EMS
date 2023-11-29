using EMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service
{
   public interface IComponentService : IDisposable
    {
        bool Save(Component entity);
        List<Component> GetComponentByPaging(out int total, PagingService<Component> pagingServices);
        Component GetComponentById(int id);

        List<Component> GetList();
        void Delete(Component entity);
    }
}
