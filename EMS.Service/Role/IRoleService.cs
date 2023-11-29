using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Dto;

namespace EMS.Service
{
    public interface IRoleService : IDisposable
    {
        Role GetRoleById(int RoleId);
        ICollection<Role> GetRoles(out int total, PagingService<Role> pagingService);
        List<FrontMenu> GetParentMenu();
        List<FrontMenu> GetFrontMenuByIds(List<int> ids);
        List<MenuAccess> GetChildMenu(int RoleId,int designationId);       
        RoleDto SaveList(RoleDto menuAccessList);
        RoleDto DeleteList(RoleDto menuAccessList);    
        List<Role> GetActiveRoles();
        List<Designation> GetDesignationList();
        List<RoleCategory> GetActiveRoleCategory();
        List<Role> GetRolesByRoleCategoyId(int RoleCateGoryId);
        List<Designation> GetDesignationByRoleId(int RoleId);
        List<Designation> GetDesignationByRoleIds(List<int> ids);
        List<Designation> GetDesignationByGroupId(int id);
        List<Designation> GetDesignationByParentGroupIds(int parentGroupId);
        Designation GetDesignationById(int designationId);
    }
}
