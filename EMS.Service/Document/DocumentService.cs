using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using EMS.Core;
using EMS.Data.Model;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EMS.Service
{
    public class DocumentService : IDocumentService
    {
        #region Constructor and Fields

        private readonly IRepository<Document> repoDocument;
        private readonly IRepository<UserLogin> repoUserLogin;
        private readonly IRepository<DocumentDepartment> repoDocumentDepartment;
        public DocumentService(IRepository<Document> _repoDocument, IRepository<UserLogin> repoUserLogin, IRepository<DocumentDepartment> _repoDocumentDepartment)
        {
            this.repoDocument = _repoDocument;
            this.repoUserLogin = repoUserLogin;
            repoDocumentDepartment = _repoDocumentDepartment;
        }

        #endregion

        public bool Delete(Document entity)
        {
            if (entity != null)
            {
                repoDocument.Delete(entity);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Document GetDocumentById(int id)
        {
            return repoDocument.FindById(id);
        }
        
        public List<Document> GetDocumentByPaging(out int total, PagingService<Document> pagingService)
        {
            return repoDocument.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetPage(pagingService.Start, pagingService.Length, out total)
                .ToList();
        }

        public Document Save(DocumentDto model)
        {
            Document entity = new Document();
            entity.Id = model.Id;
            entity.DocumentName = model.DocumentName;
            entity.DocumentPath = model.DocumentPath;
            entity.CreatedDate = model.CreatedDate;
            entity.IsActive = true;            
            entity.UserId = model.UserId;
            if (model.DepartmentId != null && model.DepartmentId.Any())
            {
                if (model.DepartmentId[0] == 0)
                {
                    entity.IsAll = true;
                }
                else
                {
                    entity.DocumentDepartment.Clear();
                    Array.ForEach(model.DepartmentId, x => entity.DocumentDepartment.Add(new DocumentDepartment { DepartmentId = x }));
                }
            }
            if (entity.Id > 0)
            {
                var Dept = repoDocumentDepartment.Query().Get().Where(x => x.DocumentId == model.Id).ToList();
                foreach (var dpt in Dept)
                {
                    repoDocumentDepartment.Delete(dpt.Id);
                }
                
                repoDocument.Update(entity);
            }
            else
            {
                entity.CreatedDate = DateTime.Now;
                entity.IsActive = true;
                repoDocument.Insert(entity);
            }

            return entity;
        }

        public Document ApprovedStatus(Document entity)
        {
            repoDocument.Update(entity);
            return entity;
        }
        public void Dispose()
        {
            if (repoDocument != null)
            {
                repoDocument.Dispose();
            }

            if (repoUserLogin != null)
            {
                repoUserLogin.Dispose();
            }
        }

        public List<Document> GetDocumentByFilter(Expression<Func<Document, bool>> filter)
        {
            return repoDocument.Query().Filter(filter).Get().ToList();
        }
    }
}
