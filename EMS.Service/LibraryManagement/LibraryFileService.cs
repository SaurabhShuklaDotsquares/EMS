using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMS.Data;
using EMS.Repo;

namespace EMS.Service.LibraryManagement
{
    public class LibraryFileService : ILibraryFileService
    {
        private IRepository<LibraryFile> repoLibraryFile;
        private IRepository<LibraryComponentFile> repoLibraryComponentFile;
        private IRepository<LibraryLayoutTypeMapping> repoLibraryLayoutTypeMapping;
        public LibraryFileService(IRepository<LibraryFile> _repoLibraryFile, IRepository<LibraryLayoutTypeMapping> _repoLibraryLayoutTypeMapping, IRepository<LibraryComponentFile> _repoLibraryComponentFile)
        {
            repoLibraryFile = _repoLibraryFile;
            this.repoLibraryLayoutTypeMapping = _repoLibraryLayoutTypeMapping;
            this.repoLibraryComponentFile = _repoLibraryComponentFile;
        }

        public bool DeleteLibraryFile(long libraryFileId)
        {
            var libraryFile = repoLibraryFile.FindById(libraryFileId);
            if (libraryFile != null)
            {
                LibraryLayoutTypeMapping layoutMapping = repoLibraryLayoutTypeMapping.Query().
                    Filter(x => x.LibraryLayoutTypeId == libraryFile.LibraryLayoutTypeId && x.LibraryId == libraryFile.LibraryId).Get().FirstOrDefault();
                if (layoutMapping != null)
                {
                    repoLibraryLayoutTypeMapping.Delete(layoutMapping);
                }

                repoLibraryFile.Delete(libraryFile);
            }


            return true;
        }
        public bool DeleteLibraryComponentFile(long libraryFileId)
        {
            var libraryFile = repoLibraryComponentFile.FindById(libraryFileId);
            if (libraryFile != null)
            {
                //LibraryLayoutTypeMapping layoutMapping = repoLibraryLayoutTypeMapping.Query().
                //    Filter(x => x.LibraryLayoutTypeId == libraryFile.LibraryLayoutTypeId && x.LibraryId == libraryFile.LibraryId).Get().FirstOrDefault();
                //if (layoutMapping != null)
                //{
                //    repoLibraryLayoutTypeMapping.Delete(layoutMapping);
                //}

                repoLibraryComponentFile.Delete(libraryFile);
            }


            return true;
        }

        public void Dispose()
        {
            if (repoLibraryFile != null)
            {
                repoLibraryFile.Dispose();
                repoLibraryFile = null;
            }
        }

        public LibraryFile GetLibraryFile(long libraryFileId)
        {
            return repoLibraryFile.Query().Get().Where(x => x.Id == libraryFileId).FirstOrDefault();
        }
        
        public LibraryComponentFile GetLibraryComponentFile(long libraryFileId)
        {
            return repoLibraryComponentFile.Query().Get().Where(x => x.Id == libraryFileId).FirstOrDefault();
        }

        public List<LibraryFile> GetLibraryFileOnLibraryId(long libraryId)
        {
            return repoLibraryFile.Query().Get().Where(x => x.LibraryId == libraryId && x.LibraryLayoutTypeId == null).ToList();
        }

        public List<LibraryFile> GetLibraryFiles()
        {
            return repoLibraryFile.Query().Get().ToList();
        }

        public void LibraryFileBulkDelete(List<LibraryFile> libraryFiles)
        {
            repoLibraryFile.DeleteBulk(libraryFiles);
        }
    }
}
