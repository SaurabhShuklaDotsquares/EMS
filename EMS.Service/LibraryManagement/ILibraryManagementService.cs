using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EMS.Data;

namespace EMS.Service
{
    public interface ILibraryManagementService : IDisposable 
    {
        List<Library> GetLibraryByPaging(out int total, PagingService<Library> pagingService);
        List<Library> GetLibraries(out int total, PagingService<Library> pagingService);
        int CountDesignLibraries(PagingService<Library> pagingService);
        List<Library> GetAllLibraries();
        List<TechnologyParent> GetUnModifiedLibraryTechnologyParents(int days);
        TechnologyParent GetLibraryDate(int parentId);
        Library GetLibraryById(long libraryId);
        Library GetLibraryByKeyId(Guid libraryId);
        Library GetFirstLibrary();
        Library Save(Library entity);
        void SaveCollection(List<Library> entityCollection);
        bool LibraryLayoutDeleted(Library library);
        bool LibraryIndustryDeleted(Library library);
        bool LibraryTechnologyDeleted(Library library);
        bool LibraryComponentDeleted(Library library);
        bool LibraryTemplateDeleted(Library library);
        bool LibraryFileDeleted(Library library);

        LibraryFile GetLibraryFileById(long id);
        LibraryComponentFile GetLibraryComponentFileById(long id);
        Library GetLibraryByGUID(Guid KeyId);
        LibraryDownloadPermission GetLibraryFileTypeDownloadPermission(LibraryFile libraryFile);
        bool UpdateFeatureValue(Guid keyId);
        bool DeleteLibraryByIds(string Id);
    }
}
