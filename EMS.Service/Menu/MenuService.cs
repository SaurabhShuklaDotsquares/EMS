using System.Collections.Generic;
using System.Linq;
using EMS.Data;
using EMS.Repo;
using System;
using EMS.Core;

namespace EMS.Service
{
    public class MenuService : IMenuService
    {
        #region "Fields"
        private IRepository<FrontMenu> repoFrontMenu;
        private IRepository<MenuAccess> repoMenuAccess;
        #endregion

        #region "Cosntructor"
        public MenuService(IRepository<FrontMenu> _repoFrontMenu, IRepository<MenuAccess> _repoMenuAccess)
        {
            this.repoFrontMenu = _repoFrontMenu;
            this.repoMenuAccess = _repoMenuAccess;

        }
        #endregion
        public FrontMenu GetMenuDeatils(int menuId)
        {
            return repoFrontMenu.Query().Filter(x => x.MenuId == menuId).Get().FirstOrDefault();
        }

        public List<FrontMenu> GetMenus(bool IsActive = true)
        {
            return repoFrontMenu.Query()
                                .Filter(m => (m.IsActive == true || m.IsActive == null) && m.MenuName != "Home")
                                .OrderBy(o => o.OrderBy(x => x.MenuOrder)
                                              .ThenBy(T => T.ParentId)
                                              .ThenBy(S => S.MenuName))
                                .Get()
                                .ToList();
        }

        public List<FrontMenu> GetMenusByUID(int UID, bool IsActive = true)
        {
            //return repoFrontMenu.Query()
            //                    .Filter(x => (x.IsActive == true || x.IsActive == null) && 
            //                                  x.MenuName != "Home" && 
            //                                  ((x.ParentId == 0) || x.MenuAccesses.Where(u => u.Role.UserLogins.FirstOrDefault(l => l.Uid == UID) != null).Any()))
            //                    .OrderBy(o => o.OrderBy(x => x.MenuOrder)
            //                                  .ThenBy(S => S.MenuName))
            //                    .Get()
            //                    .ToList();


            return repoFrontMenu.Query()
                                .Filter(x => (x.IsActive == true || x.IsActive == null) &&
                                              x.MenuName != "Home" &&
                                              ((x.ParentId == 0) || x.MenuAccesses.Where(u => u.Designation.UserLogin.FirstOrDefault(l => l.Uid == UID) != null).Any()))
                                .OrderBy(o => o.OrderBy(x => x.MenuOrder)
                                              .ThenBy(S => S.MenuName))
                                .Get()
                                .ToList();



        }

        public bool CheckCurrentMenu(string url, int DesignationId, int userId)
        {
            var parentIds = repoFrontMenu.Query().Filter(M => M.MenuAccesses.Any(a => a.DesignationId == DesignationId)).Get()
                            .Select(x => x.ParentId);

            var pages = repoFrontMenu.Query().Filter(M => M.MenuAccesses.Any(a => a.DesignationId == DesignationId) ||
             (M.ParentId == 0)).Get().Where(M => parentIds.Contains(M.ParentId) || parentIds.Contains(M.MenuId))
                 .Select(P => P.PageName.ToLower() + (!String.IsNullOrEmpty(P.ChildPages) ? ("," + P.ChildPages) : ""))
                .ToList();


            var userPage = repoFrontMenu.Query().Get().Where(x => x.UserIds != null && x.UserIds.Split(",").Contains(userId.ToString())).Select(P => P.PageName.ToLower() + (!String.IsNullOrEmpty(P.ChildPages) ? ("," + P.ChildPages) : "")).ToList();
            if (userPage.Count > 0)
            {
                foreach (var page in userPage)
                {
                    var isExists = pages.Where(x => x.ToLower() == page.ToLower()).ToList();
                    if (isExists.Count == 0)
                    {
                        pages.Add(page);
                    }                    
                }
            }
            List<string> pageList = new List<string>();
            if (pages.Count > 0)
            {
                foreach (var page in pages)
                {
                    string[] str = page.Trim().Split(',');
                    if (str.Length > 1)
                    {
                        foreach (string value in str)
                        {
                            pageList.Add(value);
                        }
                    }
                    else
                    {
                        pageList.Add(page);
                    }
                }
            }
            return pageList.Any(P => url.ToLower().EndsWith(P));
        }

        #region "Dispose"
        public void Dispose()
        {
            if (repoFrontMenu != null)
            {
                repoFrontMenu.Dispose();
                repoFrontMenu = null;
            }
            if (repoMenuAccess != null)
            {
                repoMenuAccess.Dispose();
                repoMenuAccess = null;
            }
        }
        #endregion

    }
}
