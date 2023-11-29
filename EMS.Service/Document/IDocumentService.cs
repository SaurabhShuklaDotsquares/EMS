using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EMS.Service
{
    public interface IDocumentService : IDisposable
    {
        List<Document> GetDocumentByPaging(out int total, PagingService<Document> pagingService);
        Document Save(DocumentDto model);
        Document GetDocumentById(int id);
        bool Delete(Document entity);
        List<Document> GetDocumentByFilter(Expression<Func<Document, bool>> filter);
        Document ApprovedStatus(Document model);
    }
}
