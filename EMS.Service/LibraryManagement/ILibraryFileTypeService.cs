using EMS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service.LibraryManagement
{
    public interface ILibraryFileTypeService : IDisposable
    {
        List<LibraryFileType> GetLibraryFileTypes();
        LibraryFileType GetLibraryFileTypeById(long libraryFileTypeId);
        LibraryFileType GetImageType();
        LibraryFileType GetLibraryFileTypeByName(string name);
    }
}
