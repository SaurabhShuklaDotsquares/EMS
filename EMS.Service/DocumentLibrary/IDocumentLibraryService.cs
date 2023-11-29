using EMS.Data;
using EMS.Data.Model;
using EMS.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service
{
    public interface IDocumentLibraryService
    {
        DocumentLibrary Save(DocumentLibraryDto model);
        DocumentLibrary GetFindById(int Id);        
        List<DocumentLibrary> GetRecourdByPaging(out int total, PagingService<DocumentLibrary> pagingService);
        void UpdateActiveDeactiveStatus(DocumentLibrary entity);
        void Delete(int id);
    }
}
