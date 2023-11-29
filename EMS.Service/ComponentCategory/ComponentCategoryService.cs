using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Repo;

namespace EMS.Service
{
    public class ComponentCategoryService : IComponentCategoryService
    {
        #region "Fileds"
        private IRepository<ComponentCategory> repoComponentCategory;
        #endregion

        #region "Constructor"
        public ComponentCategoryService(IRepository<ComponentCategory> repoComponentCategory)
        {
            this.repoComponentCategory = repoComponentCategory;
        }
        #endregion
       

        public List<ComponentCategory> GetComponentCategory()
        {
            return repoComponentCategory.Query().Filter(x =>x.IsActive==true).Get().OrderByDescending(x => x.Modified).ToList();
        }

        #region Dispose
        public void Dispose()
        {
            if (repoComponentCategory != null)
            {
                repoComponentCategory.Dispose();
                repoComponentCategory = null;
            }
        }
        #endregion
    }
}
