using System;
using System.Collections.Generic;
using System.Text;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
namespace EMS.Service
{
    public class LibrarySearchService:ILibrarySearchService
    {
        #region "Fields"
        private IRepository<LibrarySearch> repoLibrarySearch;
        #endregion
        #region "Constructor"
        public LibrarySearchService(IRepository<LibrarySearch> _repoLibrarySearch)
        {
            repoLibrarySearch = _repoLibrarySearch;
        }
        public LibrarySearch LibrarySearchFindById(Guid Id)
        {
            return repoLibrarySearch.FindById(Id);
        }
        #endregion
        public LibrarySearch Save(LibrarySearchDto model)
        {
            LibrarySearch librarySearch = new LibrarySearch();

            if (librarySearch == null)
            {
                return null;
            }

            librarySearch.SearchDate = DateTime.Now;
            librarySearch.Ip = model.IP;
            librarySearch.IsFeatured = model.Featured;
            librarySearch.IsNda = model.IsNDA;
            librarySearch.IsReadyToUse = model.IsReadyToUse;
            librarySearch.Keyword = model.Keyword;
            librarySearch.IsAdvanceFiltered = model.IsAdvanceSearch;
            librarySearch.LibraryTypeId = (byte)model.LibraryType;
            librarySearch.DesignTypeId = model.DesignType !=null?(byte?)model.DesignType:null;
            librarySearch.KeyId = model.KeyId;
            if (model.Technologies!=null && model.Technologies.Length>0)
            {
                foreach(int techId in  model.Technologies)
                {
                    LibrarySearchTechnology librarySearchTechnology = new LibrarySearchTechnology();
                    librarySearchTechnology.TechnologyId = techId;
                    librarySearchTechnology.LibrarySearchId = model.SearchId;
                    librarySearch.LibrarySearchTechnology.Add(librarySearchTechnology);
                }
                
            }
            if (model.Domains != null && model.Domains.Length > 0)
            {
                foreach (int DomainId in model.Domains)
                {
                    LibrarySearchIndustry librarySearchIndustry = new LibrarySearchIndustry();
                    librarySearchIndustry.DomainId = DomainId;
                    librarySearchIndustry.LibrarySearchId = model.SearchId;
                    librarySearch.LibrarySearchIndustry.Add(librarySearchIndustry);
                }

            }
            if (model.Layouts != null && model.Layouts.Length > 0)
            {
                foreach (int layoutId in model.Layouts)
                {
                    LibrarySearchLayoutTypeMapping librarySearchLayoutTypeMapping = new LibrarySearchLayoutTypeMapping();

                    librarySearchLayoutTypeMapping.LibrarySearchId= model.SearchId;
                    librarySearchLayoutTypeMapping.LayoutTypeId = layoutId;
                    
                    librarySearch.LibrarySearchLayoutTypeMapping.Add(librarySearchLayoutTypeMapping);
                }

            }
            if (model.Components != null && model.Components.Length > 0)
            {
                foreach (int componentTypeId in model.Components)
                {
                    LibrarySearchComponent librarySearchComponent = new LibrarySearchComponent();
                    librarySearchComponent.LibrarySearchId = model.SearchId;
                    librarySearchComponent.ComponentTypeId = componentTypeId;
                    librarySearch.LibrarySearchComponent.Add(librarySearchComponent);
                }

            }

            repoLibrarySearch.InsertGraph(librarySearch);

            return LibrarySearchFindById(librarySearch.Id);
        }

        public void Dispose()
        {
            repoLibrarySearch.Dispose();
            repoLibrarySearch = null;
        }
    }
}
