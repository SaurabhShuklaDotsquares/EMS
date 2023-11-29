using EMS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service.LibraryManagement
{
    public interface ILibraryComponentFileService : IDisposable
    {
        List<LibraryComponentFile> GetLibraryFiles();
        //bool DeleteLibraryFile(long libraryFileId);
        LibraryComponentFile GetLibraryFile(long libraryFileId);
        void LibraryFileBulkDelete(List<LibraryComponentFile> libraryFiles);
        List<LibraryComponentFile> GetLibraryFileOnLibraryId(long libraryId);
    }
}
