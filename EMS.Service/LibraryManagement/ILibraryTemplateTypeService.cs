using EMS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service
{
    public interface ILibraryTemplateTypeService : IDisposable
    {
        List<LibraryTemplateType> GetLibraryTemplateTypes();
        LibraryTemplateType Save(LibraryTemplateType entity);
        List<LibraryTemplateType> GetLibraryTemplateTypesByIds(int[] ids);
    }
}
