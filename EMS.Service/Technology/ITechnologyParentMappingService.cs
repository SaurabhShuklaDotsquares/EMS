using EMS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service
{
    public interface ITechnologyParentMappingService : IDisposable
    {
           List<TechnologyParentMapping> GetTechnologyByParentId(int parentId);
           List<TechnologyParentMapping> GetTechnologyParentList();
    }
}
