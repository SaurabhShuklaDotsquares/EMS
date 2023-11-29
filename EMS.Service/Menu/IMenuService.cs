using System;
using System.Collections.Generic;
using EMS.Data;

namespace EMS.Service
{
    public interface IMenuService : IDisposable
    {
        FrontMenu GetMenuDeatils(int menuId);
        List<FrontMenu> GetMenus(bool IsActive = true);
        List<FrontMenu> GetMenusByUID(int UID, bool IsActive = true);
        bool CheckCurrentMenu(string url, int DesignationId, int userId);
    }
}
