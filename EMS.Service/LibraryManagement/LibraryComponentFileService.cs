using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMS.Data;
using EMS.Repo;

namespace EMS.Service.LibraryManagement
{
    public class LibraryComponentFileService : ILibraryComponentFileService
    {
        private IRepository<LibraryComponentFile> repoLibraryComponentFile;
        public LibraryComponentFileService(IRepository<LibraryComponentFile> _repoLibraryComponentFile)
        {
            repoLibraryComponentFile = _repoLibraryComponentFile;
        }

        //public bool DeleteLibraryFile(long libraryFileId)
        //{
        //    var libraryFile = repoLibraryFile.FindById(libraryFileId);
        //    if (libraryFile != null)
        //    {
        //        LibraryLayoutTypeMapping layoutMapping = repoLibraryLayoutTypeMapping.Query().
        //            Filter(x => x.LibraryLayoutTypeId == libraryFile.LibraryLayoutTypeId && x.LibraryId == libraryFile.LibraryId).Get().FirstOrDefault();
        //        if (layoutMapping != null)
        //        {
        //            repoLibraryLayoutTypeMapping.Delete(layoutMapping);
        //        }

        //        repoLibraryFile.Delete(libraryFile);
        //    }


        //    return true;
        //}

        public void Dispose()
        {
            if (repoLibraryComponentFile != null)
            {
                repoLibraryComponentFile.Dispose();
                repoLibraryComponentFile = null;
            }
        }

        public LibraryComponentFile GetLibraryFile(long libraryFileId)
        {
            return repoLibraryComponentFile.Query().Get().Where(x => x.Id == libraryFileId).FirstOrDefault();
        }

        public List<LibraryComponentFile> GetLibraryFileOnLibraryId(long libraryId)
        {
            return repoLibraryComponentFile.Query().Get().Where(x => x.LibraryId == libraryId).ToList();
        }

        public List<LibraryComponentFile> GetLibraryFiles()
        {
            return repoLibraryComponentFile.Query().Get().ToList();
        }

        public void LibraryFileBulkDelete(List<LibraryComponentFile> libraryFiles)
        {
            repoLibraryComponentFile.DeleteBulk(libraryFiles);
        }
    }
}
