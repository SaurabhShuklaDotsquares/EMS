using EMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service
{
    public interface IVirtualDeveloperService
    {
        List<VirtualDeveloper> GetDefaultVDeveloper();
        List<VirtualDeveloper> GetVirtualDeveloperByPM(int pmID);
        List<VirtualDeveloper> GetVirtualDeveloperByPaging(out int total, PagingService<VirtualDeveloper> pagingServices);
        bool Save(VirtualDeveloper virtualDeveloper);
        VirtualDeveloper GetVirtualDeveloperById(int id);

        List<VirtualDeveloper> GetAllVirtualDevelopers();

        VirtualDeveloper GetVirtualDeveloperByName(string vdName, bool isMain);

        //new added
        List<VirtualDeveloper> GetVirtualDeveloperByPMUid(int PMUid);
        List<VirtualDeveloperCategory> GetVirtualDeveloperCategoryByPMUid(int PMUid);
        List<VirtualDeveloper> GetVirtualDeveloperByCategory(int catId);
    }
}
