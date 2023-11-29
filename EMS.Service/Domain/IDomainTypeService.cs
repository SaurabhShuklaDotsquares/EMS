using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Dto;

namespace EMS.Service
{
  public interface IDomainTypeService : IDisposable
    {
        List<DomainType> GetDomainList();
        DomainType GetDomainById(int id);
        List<DomainType> GetDomainsByIds(int[] ids);

        void UpdateStatus(int id);

        List<DomainType> GetDomainsByPaging(out int total, PagingService<DomainType> pagingService);

        bool IsTechnologyExists(int domainId, string title);

        DomainType GetDomainByName(string domainName);
        DomainType Save(DomainTypeDto model);
        List<KeyValuePair<int, string>> GetLibraryDomainList();
    }
}
