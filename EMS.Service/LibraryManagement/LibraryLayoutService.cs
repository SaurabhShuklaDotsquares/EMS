using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EMS.Data;
using EMS.Repo;
using EMS.Service;

namespace EMS.Service.LibraryManagement
{
    public class LibraryLayoutService : ILibraryLayoutService
    {
        public IRepository<LibraryLayoutType> repoLayout { get; set; }
        public LibraryLayoutService(IRepository<LibraryLayoutType> _repoLayout)
        {
            repoLayout = _repoLayout;
        }
        public void Dispose()
        {
            if (repoLayout != null)
            {
                repoLayout.Dispose();
                repoLayout = null;
            }
        }

        public List<LibraryLayoutType> GetLibraryLayouts()
        {
            return repoLayout.Query().Filter(l=>l.IsActive==true).Get().ToList();
        }

        public LibraryLayoutType Save(LibraryLayoutType layoutType)
        {
            layoutType = repoLayout.InsertCallback(layoutType);
            return layoutType;
        }
        public List<LibraryLayoutType> GetLibraryLayoutsByIds(int[] ids)
        {
            return repoLayout.Query().Filter(l => l.IsActive == true && ids.Contains(l.Id)).Get().ToList();
        }
    }
}
