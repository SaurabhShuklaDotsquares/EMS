using System;
using System.Collections.Generic;
using System.Text;
using EMS.Data;
using EMS.Dto;

namespace EMS.Service
{
    public interface ILibrarySearchService: IDisposable
    {
        LibrarySearch LibrarySearchFindById(Guid Id);
        LibrarySearch Save(LibrarySearchDto model);

    }
}
