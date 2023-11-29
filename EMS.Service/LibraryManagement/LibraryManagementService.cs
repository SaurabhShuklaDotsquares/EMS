using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EMS.Data;
using EMS.Repo;

namespace EMS.Service
{
    public class LibraryManagementService : ILibraryManagementService
    {
        #region "Fields"
        private IRepository<Library> repoLibrary;
        private IRepository<TechnologyParent> repoTechnologyParent;
        private IRepository<LibraryFile> repoLibraryFile;
        private IRepository<LibraryComponentFile> repoLibraryComponentFile;
        private IRepository<LibraryDownloadPermission> repoLibraryDownloadPermission;
        #endregion

        #region "Constructor"
        public LibraryManagementService(IRepository<Library> _repoLibrary,
            IRepository<LibraryFile> _repoLibraryFile,
            IRepository<LibraryDownloadPermission> _repoLibraryDownloadPermission,
            IRepository<TechnologyParent> repoTechnologyParent, IRepository<LibraryComponentFile> repoLibraryComponentFile)
        {
            repoLibrary = _repoLibrary;
            repoLibraryFile = _repoLibraryFile;
            repoLibraryDownloadPermission = _repoLibraryDownloadPermission;
            this.repoTechnologyParent = repoTechnologyParent;
            this.repoLibraryComponentFile = repoLibraryComponentFile;
        }
        #endregion

        public List<TechnologyParent> GetUnModifiedLibraryTechnologyParents(int days)
        {
            return repoTechnologyParent.Query()
                    .Filter(x => x.TechnologyParentMapping.Any(t =>
                             t.Technology.LibraryTechnology.Any(r => r.Library.AddDate > DateTime.Now.AddDays(-days))
                            )).Get()
                    .ToList();
        }

        public TechnologyParent GetLibraryDate(int parentId)
        {
            return repoTechnologyParent.Query()
                    .Filter(x => x.TechnologyParentMapping.Any(t =>
                             t.TechnologyParentId == parentId
                            )).Get()
                    .FirstOrDefault();
        }

        public Library GetFirstLibrary()
        {
            return repoLibrary.Query().Get().OrderBy(x => x.AddDate).FirstOrDefault();
        }

        public List<Library> GetLibraries(out int total, PagingService<Library> pagingService)
        {
            //return repoLibrary.Query()
            //        .AsTracking()
            //        .Filter(pagingService.Filter)
            //        .OrderBy(pagingService.Sort)
            //        .GetPage(pagingService.Start, pagingService.Length, out total)
            //        .ToList();

            //List<Library> libraries = new List<Library>();
            //var TempLibraries=repoLibrary.Query()
            //        .Filter(pagingService.Filter)
            //        .OrderBy(pagingService.Sort)
            //        .GetPage(pagingService.Start, pagingService.Length, out total);
            //foreach(var library in TempLibraries)
            //{
            //    libraries.Add(library);
            //}
            //return libraries;

            return repoLibrary.Query()
            .Filter(pagingService.Filter)
            .OrderBy(pagingService.Sort)
            .GetPage(pagingService.Start, pagingService.Length, out total).ToList();
        }
        public int CountDesignLibraries(PagingService<Library> pagingService)
        {
            return repoLibrary.Query()
                    .AsTracking()
                    .Filter(pagingService.Filter)
                    .Get()
                    .Sum(x => x.LibraryFile.Where(t => t.LibraryLayoutTypeId.HasValue).Count());
        }
        public Library GetLibraryById(long libraryId)
        {
            return repoLibrary.FindById(libraryId);
        }
        public Library GetLibraryByGUID(Guid KeyId)
        {
            return repoLibrary.Query().Filter(l => l.KeyId == KeyId).Get().FirstOrDefault();
        }
        public void Dispose()
        {
            if (repoLibrary != null)
            {
                repoLibrary.Dispose();
                repoLibrary = null;
            }
        }

        public Library Save(Library entity)
        {
            if (entity.Id == 0)
            {
                return repoLibrary.InsertCallback(entity);
            }
            else
            {
                var libraryEntity = repoLibrary.FindById(entity.Id);

                libraryEntity.Title = entity.Title;
                libraryEntity.LibraryTypeId = entity.LibraryTypeId;
                libraryEntity.CRMUserId = entity.CRMUserId;
                libraryEntity.SearchKeyword = entity.SearchKeyword;
                libraryEntity.Description = entity.Description;
                libraryEntity.IsNda = entity.IsNda;
                libraryEntity.IsFeatured = entity.IsFeatured;
                libraryEntity.IsInternal = entity.IsInternal;
                libraryEntity.IsLive = entity.IsLive;
                libraryEntity.LiveUrl = entity.LiveUrl;
                libraryEntity.ModifyDate = entity.ModifyDate;
                libraryEntity.AddedBy = entity.AddedBy;
                libraryEntity.ModifyByUid = entity.ModifyByUid;
                libraryEntity.BannerImage = entity.BannerImage;
                libraryEntity.DesignTypeId = entity.DesignTypeId;
                libraryEntity.OtherIndustry = entity.OtherIndustry;
                libraryEntity.OtherTechnologyParent = entity.OtherTechnologyParent;
                libraryEntity.OtherTechnology = entity.OtherTechnology;
                libraryEntity.IsActive = entity.IsActive;
                libraryEntity.AuthorUid = entity.AuthorUid;
                libraryEntity.UidBa = entity.UidBa;
                libraryEntity.UidTl = entity.UidTl;
                libraryEntity.IsGoodToShow = entity.IsGoodToShow;
                libraryEntity.Version = entity.Version;
                libraryEntity.IsReadyToUse = entity.IsReadyToUse;
                libraryEntity.IntegrationHours = entity.IntegrationHours;
                libraryEntity.ReDevelopmentHours = entity.ReDevelopmentHours;
                libraryEntity.EstimatedHours = entity.EstimatedHours;
                libraryEntity.SalesKitId = entity.SalesKitId;
                libraryEntity.CvsId = entity.CvsId;

                if (entity.LibraryLayoutTypeMapping != null && entity.LibraryLayoutTypeMapping.Any())
                {
                    entity.LibraryLayoutTypeMapping.ToList().ForEach(a =>
                    {
                        libraryEntity.LibraryLayoutTypeMapping.Add(a);
                    });
                }
                if (entity.LibraryIndustry != null && entity.LibraryIndustry.Any())
                {
                    entity.LibraryIndustry.ToList().ForEach(a =>
                    {
                        libraryEntity.LibraryIndustry.Add(a);
                    });
                }
                if (entity.LibraryTechnology != null && entity.LibraryTechnology.Any())
                {
                    entity.LibraryTechnology.ToList().ForEach(a =>
                    {
                        libraryEntity.LibraryTechnology.Add(a);
                    });
                }
                if (entity.LibraryComponent != null && entity.LibraryComponent.Any())
                {
                    entity.LibraryComponent.ToList().ForEach(a =>
                    {
                        libraryEntity.LibraryComponent.Add(a);
                    });
                }
                if (entity.LibraryTemplate != null && entity.LibraryTemplate.Any())
                {
                    entity.LibraryTemplate.ToList().ForEach(a =>
                    {
                        libraryEntity.LibraryTemplate.Add(a);
                    });
                }
                if (entity.LibraryFile != null && entity.LibraryFile.Any())
                {
                    entity.LibraryFile.ToList().ForEach(a =>
                    {
                        libraryEntity.LibraryFile.Add(a);
                    });
                } 
                if (entity.LibraryComponentFile != null && entity.LibraryComponentFile.Any())
                {
                    entity.LibraryComponentFile.ToList().ForEach(a =>
                    {
                        libraryEntity.LibraryComponentFile.Add(a);
                    });
                }
                repoLibrary.SaveChanges();
                return entity;
            }
        }

        public void SaveCollection(List<Library> entityCollection)
        {
            repoLibrary.InsertCollection(entityCollection);
        }

        public bool LibraryLayoutDeleted(Library library)
        {
            repoLibrary.ChangeEntityCollectionState(library.LibraryLayoutTypeMapping, ObjectState.Deleted);
            library.LibraryLayoutTypeMapping.Clear();
            repoLibrary.SaveChanges();
            return true;
        }

        public bool LibraryIndustryDeleted(Library library)
        {
            repoLibrary.ChangeEntityCollectionState(library.LibraryIndustry, ObjectState.Deleted);
            library.LibraryIndustry.Clear();
            repoLibrary.SaveChanges();
            return true;
        }
        public bool LibraryTechnologyDeleted(Library library)
        {
            repoLibrary.ChangeEntityCollectionState(library.LibraryTechnology, ObjectState.Deleted);
            library.LibraryTechnology.Clear();
            repoLibrary.SaveChanges();
            return true;
        }

        public bool LibraryComponentDeleted(Library library)
        {
            repoLibrary.ChangeEntityCollectionState(library.LibraryComponent, ObjectState.Deleted);
            library.LibraryComponent.Clear();
            repoLibrary.SaveChanges();
            return true;
        }

        public bool LibraryTemplateDeleted(Library library)
        {
            repoLibrary.ChangeEntityCollectionState(library.LibraryTemplate, ObjectState.Deleted);
            library.LibraryTemplate.Clear();
            repoLibrary.SaveChanges();
            return true;
        }

        public bool LibraryFileDeleted(Library library)
        {
            repoLibrary.ChangeEntityCollectionState(library.LibraryFile, ObjectState.Deleted);
            library.LibraryFile.Clear();
            repoLibrary.SaveChanges();
            return true;
        }

        public LibraryFile GetLibraryFileById(long id)
        {
            return repoLibraryFile.FindById(id);
        }
        public LibraryComponentFile GetLibraryComponentFileById(long id)
        {
            return repoLibraryComponentFile.FindById(id);
        }

        public LibraryDownloadPermission GetLibraryFileTypeDownloadPermission(LibraryFile libraryFile)
        {
            if (libraryFile != null)
            {
                return repoLibraryDownloadPermission.Query().Filter(d => d.LibraryFileType.Name == libraryFile.LibraryFileType.Name).Get().FirstOrDefault();
            }
            return null;
        }
        public Library GetLibraryByKeyId(Guid libraryId)
        {
            return repoLibrary.Query().Get().Where(x => x.KeyId == libraryId).FirstOrDefault();
        }

        public List<Library> GetLibraryByPaging(out int total, PagingService<Library> pagingService)
        {
            return repoLibrary.Query()
                    .AsTracking()
                    .Filter(pagingService.Filter)
                    .OrderBy(pagingService.Sort)
                    .GetPage(pagingService.Start, pagingService.Length, out total)
                    .ToList();
        }
        public List<Library> GetAllLibraries()
        {
            return repoLibrary.Query().Get().ToList();
        }

        public bool UpdateFeatureValue(Guid keyId)
        {
            if (keyId != Guid.Empty)
            {
                //var library=repoLibrary.FindById(libraryId);
                var library = GetLibraryByGUID(keyId);
                library.IsFeatured = !library.IsFeatured;
                library.ModifyDate = DateTime.Now;
                repoLibrary.SaveChanges();
                return true;
            }
            return false;
        }

        public bool DeleteLibraryByIds(string Id)
        {
            if (Id != null)
            {
                string[] ids = Id.Split(",").Where(x => !string.IsNullOrEmpty(x)).ToArray();

                foreach (var item in ids)
                {
                    var library = GetLibraryByGUID(Guid.Parse(item));
                    library.IsDeleted = true;
                    library.ModifyDate = DateTime.Now;
                    repoLibrary.SaveChanges();
                }
                return true;
            }




            return false;
        }
    }
}
