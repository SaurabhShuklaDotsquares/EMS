using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Dto;

namespace EMS.Service
{
  public interface ITechnologyService : IDisposable
    {

        List<Technology> GetTechnologyList();
        Technology GetTechnologyById(int id);
        List<Technology> GetTechnologiesByIds(int[] ids);

        void UpdateStatus(int id);
        Technology Save(TechnologyDto model);
        List<Technology> GetTechnologyByPaging(out int total, PagingService<Technology> pagingService);
        bool IsTechnologyExists(int techId, string title);
        Technology GetTechnologyByName(string techname);
        List<KeyValuePair<int, string>> GetLibraryTechnologyList();



    }
}
