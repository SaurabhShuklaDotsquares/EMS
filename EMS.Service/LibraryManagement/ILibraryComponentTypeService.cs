using EMS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service.LibraryManagement
{
    public interface ILibraryComponentTypeService : IDisposable
    {
        List<LibraryComponentType> GetLibraryComponentTypes();
        LibraryComponentType Save(LibraryComponentType entity);
        List<LibraryComponentType> GetLibraryComponentTypesByIds(int[] ids);
    }
}
