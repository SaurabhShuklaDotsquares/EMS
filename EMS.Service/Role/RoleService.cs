using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Repo;
using EMS.Core;
using System.Globalization;
using EMS.Dto;
using System.Security.Cryptography.X509Certificates;

namespace EMS.Service
{
    public class RoleService : IRoleService
    {
        #region "Fields"
        private IRepository<Role> repoUserRole;
        private IRepository<Designation> repoUserDesi;
        private IRepository<RoleCategory> repoUserRoleCate;
        private IRepository<FrontMenu> repoFrontMenu;
        private IRepository<MenuAccess> repoMenuAccess;
        #endregion

        #region "Cosntructor"
        public RoleService(IRepository<Role> _repoUserRole, IRepository<FrontMenu> _repoFrontMenu, IRepository<MenuAccess> _repoMenuAccess, IRepository<Designation> _repoUserDesi, IRepository<RoleCategory> _repoUserRoleCate)
        {
            this.repoUserRole = _repoUserRole;
            this.repoFrontMenu = _repoFrontMenu;
            this.repoMenuAccess = _repoMenuAccess;
            this.repoUserDesi = _repoUserDesi;
            this.repoUserRoleCate = _repoUserRoleCate;
        }
        #endregion

        public Role GetRoleById(int RoleId)
        {
            //return repoUserRole.FindById(RoleId);          
            return repoUserRole.Query()
                .Include(x => x.Designation)
                .Include(x => x.RoleCategory)               
                .Filter(x => x.RoleId == RoleId)
                .Get()
                .FirstOrDefault();
        }

        public ICollection<Role> GetRoles(out int total, PagingService<Role> pagingService)
        {
            return repoUserRole.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetPage(pagingService.Start, pagingService.Length, out total)
                .ToList();
        }

        public List<FrontMenu> GetParentMenu()
        {
            return repoFrontMenu.Query().Filter(x => x.ParentId == 0 && (x.IsActive == true || x.IsActive == null) && x.MenuName != "Home").Get().OrderBy(x => x.MenuDisplayName).ToList();
        }

        public List<Role> GetActiveRoles()
        {
            return repoUserRole.Query().Filter(x =>x.IsActive == true).Get().OrderBy(x => x.RoleName).ToList();
        }

        public List<MenuAccess> GetChildMenu(int RoleId,int designationId)
        {
            //return repoMenuAccess.Query().Filter(x => x.RoleId == RoleId && x.DesignationId ==designationId &&  x.FrontMenu.MenuId == x.MenuId).Get().OrderBy(x => x.FrontMenu.MenuDisplayName).ToList();

            return repoMenuAccess.Query().Filter(x => x.DesignationId == designationId  && x.FrontMenu.MenuId == x.MenuId).Get().OrderBy(x => x.FrontMenu.MenuDisplayName).ToList();

        }
        public List<FrontMenu> GetFrontMenuByIds(List<int> ids)
        {
            return repoFrontMenu.Query().Filter(x => x.IsActive.HasValue && x.IsActive.Value && x.ParentId.HasValue && ids.Contains(x.ParentId.Value)).Get().OrderBy(x => x.MenuDisplayName).ToList();
        }

        public RoleDto DeleteList(RoleDto menuAccessList)
        {
            //List<MenuAccess> entities = repoMenuAccess.Query().Filter(x => menuAccessList.childmenu.Contains(x.MenuId) && x.RoleId == menuAccessList.RoleId).Get().ToList();               
            //repoMenuAccess.DeleteBulk(entities);
            //return menuAccessList;
            List<MenuAccess> entities = repoMenuAccess.Query()
                .Filter(x=>x.DesignationId== menuAccessList.DesignationId && x.RoleId== menuAccessList.RoleId)
                .Get()
                .ToList();

            repoMenuAccess.DeleteBulk(entities);
            return menuAccessList;
        }
        public RoleDto SaveList(RoleDto menuAccessList)
        {
            List<MenuAccess> entities = menuAccessList.childmenu.Select(x => new MenuAccess { MenuId=x,RoleId=menuAccessList.RoleId,DesignationId= menuAccessList.DesignationId,RoleCategoryId= menuAccessList.RoleCateGoryId}).ToList();
            repoMenuAccess.SaveBulk(entities);
            return menuAccessList;
        }


        public List<Designation> GetDesignationList()
        {
            return repoUserDesi.Query().Filter(x => x.IsActive == true).Get().OrderBy(x => x.Name).ToList();
        }

        public List<RoleCategory> GetActiveRoleCategory()
        {
            return repoUserRoleCate.Query().Filter(x => x.IsActive == true).Get().OrderBy(x => x.Name).ToList();
        }

        public List<Role> GetRolesByRoleCategoyId(int RoleCateGoryId)
        {
            return repoUserRole.Query().Filter(x => x.RoleCategoryId == RoleCateGoryId && x.IsActive == true).Get().OrderBy(x => x.RoleName).ToList();
        }


        public List<Designation> GetDesignationByRoleId(int RoleId)
        {
            return repoUserDesi.Query().Filter(x => x.RoleId == RoleId && x.IsActive == true).Get().OrderBy(x => x.Name).ToList();
        }
        public List<Designation> GetDesignationByRoleIds(List<int> ids)
        {
            return repoUserDesi.Query().Filter(x => ids.Contains(x.RoleId) && x.IsActive == true).Get().OrderBy(x => x.GroupId).OrderBy(x=>x.DisplayOrder).ToList();
        }

        public List<Designation> GetDesignationByGroupId(int id)
        {
            return repoUserDesi.Query().Filter(x => x.IsActive == true && x.GroupId==id).Get().OrderBy(x => x.DisplayOrder).ToList();
        }

        public List<Designation> GetDesignationByParentGroupIds(int parentGroupId)
        {
            return repoUserDesi.Query().Filter(x => x.IsActive == true && x.GroupId==parentGroupId && x.AppendWithChild==true).Get().OrderBy(x => x.DisplayOrder).ToList();
        }

        public Designation GetDesignationById(int designationId)
        {
            return repoUserDesi.Query().Filter(x => x.Id == designationId && x.IsActive == true).Get().FirstOrDefault();
        }





        #region "Dispose"
        public void Dispose()
        {
            if (repoUserRole != null)
            {
                repoUserRole.Dispose();
                repoUserRole = null;
            }
        }

    
       
        #endregion

    }
}
