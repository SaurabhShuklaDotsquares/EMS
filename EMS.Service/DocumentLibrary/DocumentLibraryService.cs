using EMS.Core;
using EMS.Data;
using EMS.Data.Model;
using EMS.Dto;
using EMS.Repo;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EMS.Service
{
    public class DocumentLibraryService : IDocumentLibraryService
    {
        private IRepository<DocumentLibrary> _repoDocumentLibrary;

        public DocumentLibraryService(IRepository<DocumentLibrary> repoDocumentLibrary)
        {
            _repoDocumentLibrary = repoDocumentLibrary;
        }

        public DocumentLibrary Save(DocumentLibraryDto model)
        {
            DocumentLibrary entity = model.Id > 0 ? _repoDocumentLibrary.FindById(model.Id) : new DocumentLibrary();

            entity.Id = model.Id;
            entity.DocumentTitle = model.DocumentTitle;
            entity.DocumentType = model.DocumentType;
            entity.Version = model.Version;
            entity.FilePath = model.FilePath;
            
            if (model.Id > 0)
            {
                entity.UpdatedDate = DateTime.Now;
                _repoDocumentLibrary.SaveChanges();
            }
            else
            {
                entity.IsActive = true;
                entity.CreatedDate = DateTime.Now;
                _repoDocumentLibrary.Insert(entity);
            }
            var data = GetFindById(entity.Id);
            return entity;
        }
        private string UploadedCertificationFile(IFormFile ProfileImage, string ProfilePicture, out string selectedFileName, string filepath)
        {
            string uniqueFileName = selectedFileName = null;
            if (ProfileImage != null)
            {
                var extension = System.IO.Path.GetExtension(ProfileImage.FileName);
                if (extension.ToLower() == ".jpeg" || extension.ToLower() == ".jpg" || extension.ToLower() == ".png")
                {
                    string uploadsFolder = Path.Combine(filepath, $"Images//CertificationImage");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    //prev. file
                    string prevpath = Path.Combine(uploadsFolder, ProfilePicture ?? "");
                    if (System.IO.File.Exists(prevpath))
                    {
                        System.IO.File.Delete(prevpath);
                    }
                    //new file
                    selectedFileName = ProfileImage.FileName;
                    uniqueFileName = ProfileImage.FileName.ToUnique();
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        ProfileImage.CopyTo(fileStream);
                    }
                }
            }
            return uniqueFileName;
        }
        public DocumentLibrary GetFindById(int Id)
        {
            return _repoDocumentLibrary.FindById(Id);
        }

        public List<DocumentLibrary> GetRecourdByPaging(out int total, PagingService<DocumentLibrary> pagingService)
        {
            return _repoDocumentLibrary.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetPage(pagingService.Start, pagingService.Length, out total)
                .ToList();
        }

        public void UpdateActiveDeactiveStatus(DocumentLibrary entity)
        {
            _repoDocumentLibrary.Update(entity);
        }
        public void Delete(int id)
        {
            if (id > 0)
            {
                _repoDocumentLibrary.Delete(id);
            }
        }
        public void Dispose()
        {
            if (_repoDocumentLibrary != null)
            {
                _repoDocumentLibrary.Dispose();
                _repoDocumentLibrary = null;
            }
        }


    }
}
