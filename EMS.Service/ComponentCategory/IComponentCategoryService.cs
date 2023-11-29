using EMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service
{
   public interface IComponentCategoryService :IDisposable
    {
        List<ComponentCategory> GetComponentCategory();
    }
}
