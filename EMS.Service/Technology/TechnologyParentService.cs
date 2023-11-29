using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMS.Data;
using EMS.Repo;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Service
{
    public class TechnologyParentService : ITechnologyParentService
    {
        #region "Fields"
        private IRepository<TechnologyParent> repoTechnologyParent;
        #endregion

        #region "Cosntructor"
        public TechnologyParentService(IRepository<TechnologyParent> _repoTechnologyParent)
        {
            this.repoTechnologyParent = _repoTechnologyParent;
        }
        #endregion
        public void Dispose()
        {
            if (repoTechnologyParent != null)
            {
                repoTechnologyParent.Dispose();
                repoTechnologyParent = null;
            }
        }

        public List<TechnologyParent> GetTechnologyParentList()
        {
            return repoTechnologyParent.Query().Filter(x => x.IsActive == true).Get().ToList();
        }

        public List<SelectListItem> GetTechnologyParentDropdown()
        {
            return repoTechnologyParent.Query().Filter(x => x.IsActive).Get().Select(item =>
            new SelectListItem
            {
                Value = item.Id.ToString(),
                Text = item.Title
            }).ToList();
        }
    }
}
