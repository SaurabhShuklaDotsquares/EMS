using EMS.Data;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMS.Service.LibraryManagement
{
    public class LibraryTemplateTypeService : ILibraryTemplateTypeService
    {
        private IRepository<LibraryTemplateType> repoLibraryTemplateType;

        public LibraryTemplateTypeService(IRepository<LibraryTemplateType> repoLibraryTemplateType)
        {
            this.repoLibraryTemplateType = repoLibraryTemplateType;
        }

        public void Dispose()
        {
            if (repoLibraryTemplateType != null)
            {
                repoLibraryTemplateType.Dispose();
                repoLibraryTemplateType = null;
            }
        }

        public List<LibraryTemplateType> GetLibraryTemplateTypes()
        {
            return repoLibraryTemplateType.Query().Get().ToList();
        }

        public List<LibraryTemplateType> GetLibraryTemplateTypesByIds(int[] ids)
        {
            return repoLibraryTemplateType.Query().Filter(lc => ids.Contains(lc.Id)).Get().ToList();
        }

        public LibraryTemplateType Save(LibraryTemplateType libraryTemplateType)
        {
            libraryTemplateType = repoLibraryTemplateType.InsertCallback(libraryTemplateType);
            return libraryTemplateType;
        }
    }
}
