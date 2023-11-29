using EMS.Data;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMS.Service.LibraryManagement
{
    public class LibraryComponentTypeService : ILibraryComponentTypeService
    {
        private IRepository<LibraryComponentType> repoLibraryComponentType;
        public LibraryComponentTypeService(IRepository<LibraryComponentType> _repoLibraryComponentType)
        {
            repoLibraryComponentType = _repoLibraryComponentType;
        }
        public void Dispose()
        {
            if (repoLibraryComponentType != null)
            {
                repoLibraryComponentType.Dispose();
                repoLibraryComponentType = null;
            }
        }

        public List<LibraryComponentType> GetLibraryComponentTypes()
        {
            return repoLibraryComponentType.Query().Get().ToList();
        }

        public LibraryComponentType Save(LibraryComponentType libraryComponentType)
        {
            libraryComponentType = repoLibraryComponentType.InsertCallback(libraryComponentType);
            return libraryComponentType;
        }
        public List<LibraryComponentType> GetLibraryComponentTypesByIds(int[] ids)
        {
            return repoLibraryComponentType.Query().Filter(lc => ids.Contains(lc.Id)).Get().ToList();
        }
    }
}
