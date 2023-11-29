using EMS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service
{
    public interface ILibraryDownloadService : IDisposable
    {
        List<LibraryDownloadPermission> GetLibraryDownloadPermissions(int? roleId, int? userId);
        LibraryDownloadPermission GetLibraryDownloadById(long libraryDownloadId);
        LibraryDownloadPermission GetLibraryDownloadByRoleId(int roleId, int fileTypeId);
        LibraryDownloadPermission GetLibraryDownloadByOnlyUid(int? uid, int? RoleId);
        LibraryDownloadPermission GetLibraryDownloadByUserId(int? roleId, int fileTypeId, int? userId);
        void Save(LibraryDownloadPermission libraryDownload);
        List<LibraryDownloadPermission> GetLibraryDownloadPermissionByPaging(out int total, PagingService<LibraryDownloadPermission> pagingService);
        List<LibraryDownloadPermission> GetLibraryDownloadPermissionByPagingGroupBy(out int total, PagingService<LibraryDownloadPermission> pagingService);
        int? DownloadPermissionCount(int uid, int roleId, int libraryFileTypeId);
        int? DownloadPermissionCountForMonth(int uid, int roleId, int libraryFileTypeId);
        int getDownLoadCount(int uid, int roleId, int libraryFileTypeId);
        int getDownLoadCountInMonth(int uid, int libraryFileTypeId);
        void DeleteLibraryDownloadPermission(LibraryDownloadPermission libraryDownload);
    }
}
