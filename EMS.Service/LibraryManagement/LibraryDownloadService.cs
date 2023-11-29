using EMS.Data;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EMS.Dto;

namespace EMS.Service
{
    public class LibraryDownloadService : ILibraryDownloadService
    {
        private IRepository<LibraryDownloadPermission> repoLibraryDownload;
        private IRepository<LibraryDownloadHistory> repoLibraryDownloadHistory;
        public LibraryDownloadService(IRepository<LibraryDownloadPermission> _repoLibraryDownload, IRepository<LibraryDownloadHistory> _repoLibraryDownloadHistory)
        {
            repoLibraryDownload = _repoLibraryDownload;
            repoLibraryDownloadHistory = _repoLibraryDownloadHistory;
        }
        public void Dispose()
        {
            if (repoLibraryDownload != null)
            {
                repoLibraryDownload.Dispose();
                repoLibraryDownload = null;
            }
        }

        public LibraryDownloadPermission GetLibraryDownloadById(long libraryDownloadId)
        {
            return repoLibraryDownload.FindById(libraryDownloadId);
        }

        public List<LibraryDownloadPermission> GetLibraryDownloadPermissions(int? roleId, int? userId)
        {
            return repoLibraryDownload.Query().Filter(x => x.RoleId == roleId &&
                    x.UserLoginId == userId).Get().ToList();
        }

        public LibraryDownloadPermission GetLibraryDownloadByRoleId(int roleId, int fileTypeId)
        {
            return repoLibraryDownload.Query().Filter(x => x.RoleId == roleId && x.LibraryFileTypeId == fileTypeId)
                .Get().FirstOrDefault();
        }
        public LibraryDownloadPermission GetLibraryDownloadByOnlyUid(int? userId, int? roleId)
        {
            LibraryDownloadPermission libDownloadPermission = null;
            if (userId != null && userId > 0)
            {
                libDownloadPermission = repoLibraryDownload.Query().Filter(x => x.UserLoginId == userId && x.MaximumDownloadInDay > 0)
                .Get().OrderByDescending(x => x.ModifyDate).FirstOrDefault();

                if (libDownloadPermission == null)
                {
                    libDownloadPermission = repoLibraryDownload.Query().Filter(x => x.RoleId == roleId && x.MaximumDownloadInDay > 0 &&
                     x.UserLoginId == null).Get().OrderByDescending(x => x.ModifyDate).FirstOrDefault();
                }
            }
            return libDownloadPermission;

            //return repoLibraryDownload.Query().Filter(x => x.UserLoginId==uid && x.MaximumDownloadInDay>0)
            //    .Get().OrderByDescending(x=>x.ModifyDate). FirstOrDefault();
        }
        public LibraryDownloadPermission GetLibraryDownloadByUserId(int? roleId, int fileTypeId, int? userId)
        {
            LibraryDownloadPermission libDownloadPermission = null;
            if (userId != null && userId > 0)
            {
                libDownloadPermission = repoLibraryDownload.Query().Filter(x => x.RoleId == null && x.LibraryFileTypeId == fileTypeId &&
                     x.UserLoginId == userId).Get().FirstOrDefault();

                if (libDownloadPermission == null)
                {
                    libDownloadPermission = repoLibraryDownload.Query().Filter(x => x.RoleId == roleId && x.LibraryFileTypeId == fileTypeId &&
                     x.UserLoginId == null).Get().FirstOrDefault();
                }
            }
            return libDownloadPermission;
        }

        /// <summary>
        /// Download permission count
        /// </summary>
        /// <param name="uid">Uid</param>
        /// <param name="roleId">RoleId</param>
        /// <param name="libraryFileTypeId">LibraryFileTypeId</param>
        /// <returns>gets download permission count</returns>
        public int? DownloadPermissionCount(int uid, int roleId, int libraryFileTypeId)
        {
            //Checks permission first by user
            var libraryDownloadPermission = GetLibraryDownloadByUserId(roleId, libraryFileTypeId, uid);
            if (libraryDownloadPermission != null)
            {
                return (int?)libraryDownloadPermission.MaximumDownloadInDay;
            }
            //checks permission by role if by user doesn't exist
            //libraryDownloadPermission = GetLibraryDownloadByRoleId(roleId, libraryFileTypeId);
            //if (libraryDownloadPermission != null)
            //{
            //    return (int?)libraryDownloadPermission.MaximumDownloadInDay;
            //}
            return null; // Download permission doesn't exist;
        }

        public int? DownloadPermissionCountForMonth(int uid, int roleId, int libraryFileTypeId)
        {
            //Checks permission first by user
            var libraryDownloadPermission = GetLibraryDownloadByUserId(roleId, libraryFileTypeId, uid);
            if (libraryDownloadPermission != null)
            {
                return (int?)libraryDownloadPermission.MaximumDownloadInMonth;
            }
            return null; // Download permission doesn't exist;
        }

        /// <summary>
        /// Gets download count
        /// </summary>
        /// <param name="uid">userLogin Id</param>
        /// <param name="roleId">role Id</param>
        /// <param name="libraryFileTypeId">libraryFileTypeId</param>
        /// <returns>Gets today's download count</returns>
        public int getDownLoadCount(int uid, int roleId, int libraryFileTypeId)
        {
            return repoLibraryDownloadHistory.Query().Filter(ldh => ldh.LibraryFileTypeId == libraryFileTypeId &&
            ldh.DownloadBy == uid && ldh.DownloadOn.Date == DateTime.Today.Date).Get().Count();

        }

        public int getDownLoadCountInMonth(int uid, int libraryFileTypeId)
        {
            return repoLibraryDownloadHistory.Query().Filter(ldh => ldh.LibraryFileTypeId == libraryFileTypeId &&
            ldh.DownloadBy == uid && ldh.DownloadOn.Date <= DateTime.Today.Date && ldh.DownloadOn.Date >= DateTime.Today.AddMonths(-1).Date).Get().Count();

        }

        public void Save(LibraryDownloadPermission libraryDownload)
        {
            if (libraryDownload.Id == 0)
                repoLibraryDownload.InsertGraph(libraryDownload);
            else
                repoLibraryDownload.SaveChanges();
        }
        public void DeleteLibraryDownloadPermission(LibraryDownloadPermission libraryDownload)
        {
            if (libraryDownload.Id != 0)
                repoLibraryDownload.Delete(libraryDownload);
        }

        public List<LibraryDownloadPermission> GetLibraryDownloadPermissionByPaging(out int total, PagingService<LibraryDownloadPermission> pagingService)
        {
            return repoLibraryDownload.Query()
            .AsTracking()
            .Filter(pagingService.Filter)
            .OrderBy(pagingService.Sort)
            .GetPage(pagingService.Start, pagingService.Length, out total)
            .ToList();
        }


        public List<LibraryDownloadPermission> GetLibraryDownloadPermissionByPagingGroupBy(out int total, PagingService<LibraryDownloadPermission> pagingService)
        {
            var records = repoLibraryDownload.Query()//.Include(q => q.UserLogin)
                .Filter(pagingService.Filter)
                .GetQuerable()
                .GroupBy(x => new { x.RoleId, x.UserLoginId })
                .Select(g => g.OrderByDescending(x => x.Role.RoleName).FirstOrDefault());

            total = records.Count();

            return records.OrderBy(o => o.Role.RoleName)
                            .Skip((pagingService.Start - 1) * pagingService.Length)
                            .Take(pagingService.Length)
                            .ToList();
        }

        //public List<object> GetLibraryDownloadPermissionByPagingGroupBy(out int total, PagingService<LibraryDownloadPermission> pagingService)
        //{
        //    var records = repoLibraryDownload.Query()//.Include(q => q.UserLogin)
        //        .Filter(pagingService.Filter)
        //        .GetQuerable()
        //        .GroupBy(x => new { x.RoleId, x.UserLoginId })
        //        .Select(g => g.OrderByDescending(x => x.Role.RoleName).ToList())
        //        .Skip((pagingService.Start - 1) * pagingService.Length)
        //        .Take(pagingService.Length).ToList();

        //    total = records.Count();

        //    return records.Select(x => new LibraryDownloadPermission
        //    {
        //        RoleId =
        //    }).ToList();

        //    //return records.Skip((pagingService.Start - 1) * pagingService.Length)
        //    //                .Take(pagingService.Length)
        //    //                .ToList();
        //}
    }
}
