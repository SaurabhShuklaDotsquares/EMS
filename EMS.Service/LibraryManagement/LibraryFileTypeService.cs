using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMS.Data;
using EMS.Repo;

namespace EMS.Service.LibraryManagement
{
    public class LibraryFileTypeService : ILibraryFileTypeService
    {
        private IRepository<LibraryFileType> repoLibraryFileType;
        public LibraryFileTypeService(IRepository<LibraryFileType> _repoLibraryFileType)
        {
            repoLibraryFileType = _repoLibraryFileType;
        }
        public void Dispose()
        {
            if (repoLibraryFileType != null)
            {
                repoLibraryFileType.Dispose();
                repoLibraryFileType = null;
            }
        }

        public LibraryFileType GetLibraryFileTypeById(long LibraryFileTypeId)
        {
            return repoLibraryFileType.FindById(LibraryFileTypeId);
        }

        public List<LibraryFileType> GetLibraryFileTypes()
        {
            return repoLibraryFileType.Query().Filter(l=>l.IsActive==true).Get().ToList();
        }
        public LibraryFileType GetImageType()
        {
            return repoLibraryFileType.Query().Filter(l => l.Name.ToLower() == "image").Get().FirstOrDefault();
        }

        public LibraryFileType GetLibraryFileTypeByName(string name)
        {
            if(!string.IsNullOrWhiteSpace(name))
            {
                return repoLibraryFileType.Query().Filter(l => l.Name.ToLower() == name.ToLower()).Get().FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
    }
}
