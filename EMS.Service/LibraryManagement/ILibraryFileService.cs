using EMS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service.LibraryManagement
{
    public interface ILibraryFileService : IDisposable
    {
        List<LibraryFile> GetLibraryFiles();
        bool DeleteLibraryFile(long libraryFileId);
        bool DeleteLibraryComponentFile(long libraryFileId);
        LibraryFile GetLibraryFile(long libraryFileId);
        LibraryComponentFile GetLibraryComponentFile(long libraryFileId);
        void LibraryFileBulkDelete(List<LibraryFile> libraryFiles);
        List<LibraryFile> GetLibraryFileOnLibraryId(long libraryId);
    }
}
