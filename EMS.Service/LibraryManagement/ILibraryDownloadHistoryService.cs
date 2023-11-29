using System;
using System.Collections.Generic;
using System.Text;
using EMS.Data;

namespace EMS.Service
{
    public interface ILibraryDownloadHistoryService : IDisposable
    {
        List<Library> GetLibrary();
        bool Save(LibraryDownloadHistory entity);
        bool DownloadExhausted(int uid, int roleId, int libraryFileTypeId);
        bool DeleteDownloadHistory(List<LibraryDownloadHistory> libraryDownloadHistories);
        List<LibraryDownloadHistory> GetDownloadHistory(long LibraryFileId);
        List<LibraryDownloadHistory> GetLibraryDownloadHistoryByPaging(out int total, PagingService<LibraryDownloadHistory> pagingService);
    }
}
