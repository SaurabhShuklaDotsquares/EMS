using EMS.Data;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace EMS.Service
{
    public class LibraryDownloadHistoryService : ILibraryDownloadHistoryService
    {
        #region "fields  and constructor"
        private IRepository<LibraryDownloadHistory> repoLibraryDownloadHistory;
        private IRepository<LibraryDownloadPermission> repoLibraryDownloadPermission;
        private IRepository<Library> repoLibrary;

        public List<Library> GetLibrary()
        {
            var libraryList = repoLibrary.Query().Get().ToList();

            return libraryList;

        }
        public LibraryDownloadHistoryService(IRepository<LibraryDownloadHistory> _repoLibraryDownloadHistory,
            IRepository<LibraryDownloadPermission> _repoLibraryDownloadPermission, IRepository<Library> _repoLibrary
            )
        {
            repoLibraryDownloadHistory = _repoLibraryDownloadHistory;
            repoLibraryDownloadPermission = _repoLibraryDownloadPermission;
            repoLibrary = _repoLibrary;
        }
        #endregion
        public bool Save(LibraryDownloadHistory entity)
        {
            repoLibraryDownloadHistory.Insert(entity); // it's a history so don't require edit functionality
            return true;
        }
        public bool DownloadExhausted(int uid, int roleId, int libraryFileTypeId)
        {
            var libraryDownloadPermission = GetLibraryDownloadPermission(uid, roleId, libraryFileTypeId);
            return true;
        }

        public LibraryDownloadPermission GetLibraryDownloadPermission(int uid, int roleId, int libraryFileTypeId)
        {
            var libraryDownloadPermission = repoLibraryDownloadPermission.Query().
                Filter(dp => dp.UserLoginId == uid && dp.LibraryFileTypeId == libraryFileTypeId).Get().FirstOrDefault();
            if (libraryDownloadPermission != null)
            {
                return libraryDownloadPermission; // Library download permission for user
            }

            //Library download permission for role
            return libraryDownloadPermission = repoLibraryDownloadPermission.Query().
                Filter(dp => dp.RoleId == roleId && dp.LibraryFileTypeId == libraryFileTypeId).Get().FirstOrDefault();

        }

        public void Dispose()
        {
            if (repoLibraryDownloadHistory != null)
            {
                repoLibraryDownloadHistory.Dispose();
                repoLibraryDownloadHistory = null;
            }
        }

        public bool DeleteDownloadHistory(List<LibraryDownloadHistory> libraryDownloadHistories)
        {
            repoLibraryDownloadHistory.DeleteBulk(libraryDownloadHistories);
            return true;
        }

        public List<LibraryDownloadHistory> GetDownloadHistory(long LibraryFileId)
        {
            return repoLibraryDownloadHistory.Query().Get().Where(x => x.LibraryFileId == Convert.ToInt32(LibraryFileId)).ToList();
        }

        public List<LibraryDownloadHistory> GetLibraryDownloadHistoryByPaging(out int total, PagingService<LibraryDownloadHistory> pagingService)
        {
            return repoLibraryDownloadHistory.Query()
            //.AsTracking()
            .Filter(pagingService.Filter)
            .OrderBy(pagingService.Sort)
            .GetPage(pagingService.Start, pagingService.Length, out total)
            .ToList();
        }
    }
}
