using EMS.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EMS.Service.LibraryManagement
{
    public interface ILibraryLayoutService : IDisposable
    {
        List<LibraryLayoutType> GetLibraryLayouts();
        LibraryLayoutType Save(LibraryLayoutType layoutType);
        List<LibraryLayoutType> GetLibraryLayoutsByIds(int[] ids);
    }
}
