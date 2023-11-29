using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMS.Data;
using EMS.Repo;

namespace EMS.Service
{
    public class TechnologyParentMappingService : ITechnologyParentMappingService
    {
        private IRepository<TechnologyParentMapping> repoTechnologyParentMapping;
        public TechnologyParentMappingService(IRepository<TechnologyParentMapping> _repoTechnologyParentMapping)
        {
            repoTechnologyParentMapping = _repoTechnologyParentMapping;
        }
        public void Dispose()
        {
            if (repoTechnologyParentMapping != null)
            {
                repoTechnologyParentMapping.Dispose();
                repoTechnologyParentMapping = null;
            }
        }

        public List<TechnologyParentMapping> GetTechnologyByParentId(int parentId)
        {
            return repoTechnologyParentMapping.Query().Filter(x => x.TechnologyParentId == parentId).Get().ToList();
        }

        public List<TechnologyParentMapping> GetTechnologyParentList()
        {
            return repoTechnologyParentMapping.Query().Get().ToList();
        }
    }
}
