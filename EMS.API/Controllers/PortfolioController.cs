using EMS.API.LIBS;
using EMS.API.Model;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Service.LibraryManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : BaseApiController
    {
        #region Reference Variables
        private readonly int PageDataLength = int.MaxValue;
        private static int pageNo = 0;
        string[] libraryTypesIds = { "0", "1", "3" };
        private static LibraryManagementSearchFilterDtoModel _libraryManagementSearchFilter = new LibraryManagementSearchFilterDtoModel();
        private readonly ILibraryManagementService libraryManagementService;
        private readonly ILibraryLayoutService libraryLayoutService;
        private readonly ILibraryComponentTypeService libraryComponentTypeService;
        private readonly ILibraryTemplateTypeService libraryTemplateTypeService;
        private readonly IDomainTypeService domainTypeService;
        private readonly ITechnologyService technologyService;
        #endregion

        #region Constructor
        public PortfolioController(IApiKeyService _apiKeyService, ILibraryManagementService _libraryManagementService, ILibraryLayoutService _libraryLayoutService, ILibraryComponentTypeService _libraryComponentTypeService, ILibraryTemplateTypeService _libraryTemplateTypeService, IDomainTypeService _domainTypeService, ITechnologyService _technologyService) : base(_apiKeyService)
        {
            libraryManagementService = _libraryManagementService;
            libraryLayoutService = _libraryLayoutService;
            libraryComponentTypeService = _libraryComponentTypeService;
            libraryTemplateTypeService = _libraryTemplateTypeService;
            domainTypeService = _domainTypeService;
            technologyService = _technologyService;
        }
        #endregion

        /// <summary>
        /// Provide Any Filter Input = { "SearchText":"test","IsNDA":true,"Featured":true, "LibraryType": 1,"Domains":[1,2,5],"Technologies":[39,13,4,16,5] }
        /// </summary>
        /// <param name="libraryManagementSearchFilter"></param>
        /// <returns>LibraryManagementDtoModel</returns>
        [Route("~/Portfolio/LibrarySearch")]
        [HttpPost]
        public ResponseModel<LibraryManagementDtoModel> SearchLibrary(LibraryManagementSearchFilterDtoModel libraryManagementSearchFilter)
        {
            //if (!HasDownloadPermission())
            //{
            //    return AccessDenied();
            //}
            ResponseModel<LibraryManagementDtoModel> response = new ResponseModel<LibraryManagementDtoModel>();
            try
            {
                pageNo = 0;
                int totalCount = 0;
                _libraryManagementSearchFilter = libraryManagementSearchFilter;
                libraryManagementSearchFilter.DataLength = PageDataLength;
                libraryManagementSearchFilter.PageStart = pageNo;
                var expr = GetLibraryFilterExpression(libraryManagementSearchFilter);
                var libraries = libraryManagementService.GetLibraries(out totalCount, expr);
                LibraryManagementDto libraryManagementDto = new LibraryManagementDto();
                FillSelectedItemsList(libraryManagementDto, libraryManagementSearchFilter);
                pageNo++;
                if (libraries != null && libraries.Count > 0)
                {
                    foreach (var library in libraries)
                    {
                        string banner = null;
                        //if (library.LibraryLayoutTypeMapping != null && library.LibraryLayoutTypeMapping.Count > 0)
                        //{
                        //    var layoutBanner = library.LibraryFile?.Where(x => x.LibraryLayoutType?.Name.ToLower() == libraryManagementSearchFilter.SearchText.ToLower()).FirstOrDefault();
                        //    if (layoutBanner != null)
                        //    {
                        //        banner = layoutBanner.FilePath;
                        //    }
                        //}
                        LibraryDto libraryDto = new LibraryDto();
                        libraryDto.Id = library.Id;
                        libraryDto.IsNDA = library.IsNda;
                        libraryDto.LibraryTypeId = library.LibraryTypeId;
                        libraryDto.KeyId = library.KeyId;
                        libraryDto.Title = library.Title.Trim().ToTitleCase();
                        libraryDto.Description = !string.IsNullOrWhiteSpace(library.Description) ? library.Description.Trim().ToTitleCase() : null;
                        libraryDto.BannerImage = (string.IsNullOrEmpty(banner) ? library.BannerImage : banner);
                        libraryDto.IntegrationHours = library.IntegrationHours;
                        libraryDto.IsReadyToUse = library.IsReadyToUse;
                        libraryDto.Version = library.Version ?? "1.0";
                        libraryDto.CreatedDate = (library.LibraryCreatedDate.HasValue) ? library.LibraryCreatedDate.Value.ToString() : null;
                        libraryDto.IsFeatured = library.IsFeatured;
                        libraryDto.AddedBy = library.AddedBy;
                        libraryDto.LiveURL = library.LiveUrl;
                        if (library.LibraryTechnology.Count > 0)
                        {
                            libraryDto.LibraryTechnologiesComma = String.Join(", ", library.LibraryTechnology.Select(x => x.Technology.Title).ToArray());
                        }

                        if (library.LibraryIndustry.Count > 0)
                        {
                            libraryDto.LibraryIndustriesComma = String.Join(", ", library.LibraryIndustry.Select(x => x.Industry.DomainName).ToArray());
                        }
                        libraryDto.LibraryFileTypes = GetLibraryFileType(library.LibraryFile);

                        if (libraryManagementSearchFilter.LibraryType == Enums.LibraryType.Component
                            || libraryManagementSearchFilter.LibraryType == Enums.LibraryType.Design
                            || libraryManagementSearchFilter.LibraryType == Enums.LibraryType.Document
                            || libraryManagementSearchFilter.LibraryType == Enums.LibraryType.Template
                            || libraryManagementSearchFilter.LibraryType == Enums.LibraryType.Select)
                        {
                            if (library.LibraryFile != null)
                            {
                                foreach (var libraryFile in library.LibraryFile)
                                {
                                    LibraryFileDto libraryFileDto = new LibraryFileDto();
                                    libraryFileDto.FileName = Path.GetFileName(libraryFile.FilePath);
                                    libraryFileDto.Id = libraryFile.Id;
                                    libraryFileDto.LibraryId = libraryFile.LibraryId;
                                    //libraryFileDto.FileImage = Path.GetExtension(libraryFile.FilePath).ToLower() == ".zip" ? "zip-link" : "rar-link";
                                    libraryFileDto.FileImage = libraryFile.LibraryFileType.Icon;
                                    libraryFileDto.FilePath = libraryFile.FilePath;
                                    libraryFileDto.FileTypeName = libraryFile.LibraryFileType.Name;
                                    libraryFileDto.PSDFilePath = libraryFile.PsdfilePath;
                                    libraryFileDto.LibraryLayoutTypeId = libraryFile.LibraryLayoutTypeId;
                                    libraryFileDto.LibraryLayoutType = libraryFile.LibraryLayoutType;
                                    libraryDto.LibraryFileDtos.Add(libraryFileDto);

                                    if (library.LibraryTypeId == Convert.ToInt32(Enums.LibraryType.Design))
                                    {
                                        if (libraryFile.LibraryLayoutType != null && _libraryManagementSearchFilter.SearchText != null && libraryFile.LibraryLayoutType.Name.ToLower().Contains(_libraryManagementSearchFilter.SearchText.ToLower()))
                                        {
                                            libraryDto.BannerImage = libraryFile.FilePath;
                                        }
                                    }
                                }
                            }
                        }
                        libraryManagementDto.libraries.Add(libraryDto);
                    }
                }
                libraryManagementDto.totalRecords = totalCount;
                libraryManagementDto.LibraryType = libraryManagementSearchFilter.LibraryType.ToString();
                libraryManagementDto.LibraryFileIds = (libraryManagementSearchFilter.Layouts != null)
                    ? libraryManagementSearchFilter.Layouts.OfType<int>().ToList() : null;

                //new output model
                LibraryManagementDtoModel libraryManagementDtoModel = new LibraryManagementDtoModel();
                libraryManagementDtoModel.totalRecords = totalCount;
                libraryManagementDtoModel.LibraryType = libraryManagementSearchFilter.LibraryType.ToString();
                libraryManagementDtoModel.LibraryFileIds = (libraryManagementSearchFilter.Layouts != null)
                    ? libraryManagementSearchFilter.Layouts.OfType<int>().ToList() : null;
                if (libraryManagementDto.libraryManagementSearchFilterDto != null)
                {
                    libraryManagementDtoModel.libraryManagementSearchFilterDto = new LibraryManagementSearchFilterDtoAPI { KeyId = libraryManagementDto.libraryManagementSearchFilterDto.KeyId };
                }
                libraryManagementDto.libraries.ForEach(x =>
                {
                    LibraryDtoAPI libraryDtoAPI = new LibraryDtoAPI
                    {
                        LibraryTypeId = x.LibraryTypeId,
                        BannerImage = x.BannerImage,
                        IsFeatured = x.IsFeatured,
                        Title = x.Title,
                        LiveURL = x.LiveURL,
                        Description = x.Description,
                        CreatedDate = x.CreatedDate,
                        Version = x.Version,
                        LibraryTechnologiesComma = x.LibraryTechnologiesComma,
                        IntegrationHours = x.IntegrationHours,
                        KeyId = x.KeyId,
                        IsNDA = x.IsNDA,
                        Id = x.Id,
                        LibraryIndustriesComma = x.LibraryIndustriesComma
                    };
                    x.LibraryFileDtos.ForEach(y =>
                    {
                        LibraryFileDtoAPI libraryFileDtoAPI = new LibraryFileDtoAPI
                        {
                            FilePath = y.FilePath,
                            Id = y.Id,
                            LibraryLayoutTypeId = y.LibraryLayoutTypeId
                        };
                        if (y.LibraryLayoutType != null)
                        {
                            LibraryLayoutTypeAPI libraryLayoutTypeAPI = new LibraryLayoutTypeAPI
                            {
                                Id = y.LibraryLayoutType == null ? 0 : y.LibraryLayoutType.Id,
                                Name = y.LibraryLayoutType?.Name
                            };
                            libraryFileDtoAPI.LibraryLayoutType = libraryLayoutTypeAPI;
                        }
                        libraryDtoAPI.LibraryFileDtos.Add(libraryFileDtoAPI);
                    });
                    libraryManagementDtoModel.libraries.Add(libraryDtoAPI);
                });

                response.Status = true;
                response.Message = "success";
                response.Code = HttpStatusCode.OK;
                response.Data = libraryManagementDtoModel;

                //test
                //var settings = new Newtonsoft.Json.JsonSerializerSettings();
                //settings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                //string j = Newtonsoft.Json.JsonConvert.SerializeObject(libraryManagementDto, settings);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "failed";
                response.Code = HttpStatusCode.ExpectationFailed;
                response.Errors = new string[] { (ex.InnerException ?? ex).Message };
            }
            return response;
        }
        private PagingService<Library> GetLibraryFilterExpression(LibraryManagementSearchFilterDtoModel libraryManagementSearchFilter)
        {
            var pagingServices = new PagingService<Library>(Convert.ToInt32(libraryManagementSearchFilter.PageStart),Convert.ToInt32(libraryManagementSearchFilter.DataLength));
            var expr = PredicateBuilder.True<Library>();
            expr = expr.And(x => x.IsDeleted != true && x.IsActive == true);
            if (libraryManagementSearchFilter.KeyId != null && libraryManagementSearchFilter.KeyId != Guid.Empty)
            {
                expr = expr.And(l => l.KeyId == libraryManagementSearchFilter.KeyId);
            }
            else
            {
                expr = expr.And(l => l.LibraryFile != null);
                if (libraryManagementSearchFilter.LibraryType.HasValue)
                {
                    expr = expr.And(l => (l.LibraryTypeId == (byte)libraryManagementSearchFilter.LibraryType));
                }
                else
                {
                    expr = expr.And(x => libraryTypesIds.Contains(x.LibraryTypeId.ToString()));
                }

                //expr = expr.And(l => l.IsActive == true);
                if (libraryManagementSearchFilter.DesignType.HasValue)
                {
                    expr = expr.And(l => (l.DesignTypeId == (byte)libraryManagementSearchFilter.DesignType));
                }
               

                //expr = expr.And(l => (l.LibraryTypeId == (byte)libraryManagementSearchFilter.LibraryType));

                if (!string.IsNullOrWhiteSpace(libraryManagementSearchFilter.SearchText))
                {
                    string searchText = libraryManagementSearchFilter.SearchText.Trim();
                    expr = expr.And(l => (l.Title.Trim().Contains(searchText) ||
                    l.LibraryComponent.Any(lc => lc.LibraryComponentType.Name.Trim().Contains(searchText)) ||
                    l.LibraryTemplate.Any(lc => lc.LibraryTemplateType.Name.Trim().Contains(searchText)) ||
                    l.LibraryTechnology.Any(lt => lt.Technology.Title.Trim().Contains(searchText)) ||
                    l.LibraryIndustry.Any(li => li.Industry.DomainName.Trim().Contains(searchText)) ||
                    l.LibraryLayoutTypeMapping.Any(ll => ll.LibraryLayoutType.Name.Trim().Contains(searchText)) ||
                    l.LibraryFile.Any(f => f.LibraryLayoutType.Name.Contains(searchText)) ||
                    l.SearchKeyword.Trim().Contains(searchText) ||
                    l.Description.Trim().Contains(searchText) ||
                    l.OtherIndustry.Trim().Contains(searchText) ||
                    l.OtherTechnology.Trim().Contains(searchText) ||
                    l.OtherTechnologyParent.Trim().Contains(searchText)
                    ));
                }

                if (libraryManagementSearchFilter.Technologies != null && libraryManagementSearchFilter.Technologies.Length > 0)
                {
                    expr = expr.And(l => l.LibraryTechnology.Any(lt => libraryManagementSearchFilter.Technologies.Contains(lt.TechnologyId)));
                }
                if (libraryManagementSearchFilter.Domains != null && libraryManagementSearchFilter.Domains.Length > 0)
                {
                    expr = expr.And(l => l.LibraryIndustry.Any(lt => libraryManagementSearchFilter.Domains.Contains(lt.IndustryId)));
                }
                if (libraryManagementSearchFilter.Layouts != null && libraryManagementSearchFilter.Layouts.Length > 0)
                {
                    expr = expr.And(l => l.LibraryLayoutTypeMapping.Any(lt => libraryManagementSearchFilter.Layouts.Contains(lt.LibraryLayoutTypeId)));
                }
                if (libraryManagementSearchFilter.Components != null && libraryManagementSearchFilter.Components.Length > 0)
                {
                    expr = expr.And(l => l.LibraryComponent.Any(lc => libraryManagementSearchFilter.Components.Contains(lc.LibraryComponentTypeId)));
                }
                if (libraryManagementSearchFilter.Templates != null && libraryManagementSearchFilter.Templates.Length > 0)
                {
                    expr = expr.And(l => l.LibraryTemplate.Any(lc => libraryManagementSearchFilter.Templates.Contains(lc.LibraryTemplateTypeId)));
                }
                if (libraryManagementSearchFilter.IsNDA != null)
                {
                    expr = expr.And(l => l.IsNda == libraryManagementSearchFilter.IsNDA);
                }
                if (libraryManagementSearchFilter.IsReadyToUse != null)
                {
                    expr = expr.And(l => l.IsReadyToUse == libraryManagementSearchFilter.IsReadyToUse);
                }
                if (libraryManagementSearchFilter.Featured != null)
                {
                    expr = expr.And(l => l.IsFeatured == libraryManagementSearchFilter.Featured);
                }
                if (libraryManagementSearchFilter.LibraryType == Enums.LibraryType.Design)
                {
                    if (libraryManagementSearchFilter.Layouts != null && libraryManagementSearchFilter.Layouts.Length > 0)
                    {
                        expr = expr.And(l => l.LibraryFile.Any(x => libraryManagementSearchFilter.Layouts.Contains(Convert.ToInt32(x.LibraryLayoutTypeId))));
                    }
                    else
                    {
                        expr = expr.And(l => l.LibraryFile.Any(x => x.LibraryLayoutTypeId != null));
                    }
                }
            }
            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.ModifyDate).OrderByDescending(c => c.IsFeatured);
            };
            return pagingServices;
        }
        private void FillSelectedItemsList(LibraryManagementDto libraryManagementDto,
            LibraryManagementSearchFilterDtoModel searchFilter)
        {
            LibrarySearchSelectedItemsDto librarySearchSelectedItemsDto = new LibrarySearchSelectedItemsDto();
            if (searchFilter.LibraryType.HasValue)
            {
                librarySearchSelectedItemsDto = new LibrarySearchSelectedItemsDto();
                librarySearchSelectedItemsDto.FilterId = searchFilter.LibraryTypeFilterId;
                librarySearchSelectedItemsDto.Label = searchFilter.LibraryTypeLabel;
                librarySearchSelectedItemsDto.Order = 1;
                librarySearchSelectedItemsDto.SelectListItems = WebExtensions.GetSelectList<Enums.LibraryType>()
                    .Where(x => x.Value == ((byte)searchFilter.LibraryType).ToString()).ToList();
                libraryManagementDto.libraryManagementSelectedItemsDtos.Add(librarySearchSelectedItemsDto);
            }
            if (searchFilter.DesignType != null)
            {
                librarySearchSelectedItemsDto = new LibrarySearchSelectedItemsDto();
                librarySearchSelectedItemsDto.FilterId = searchFilter.DesignTypeFilterId;
                librarySearchSelectedItemsDto.Label = searchFilter.DesignTypeLabel;
                librarySearchSelectedItemsDto.Order = 2;
                if (searchFilter.DesignType != null)
                {
                    librarySearchSelectedItemsDto.SelectListItems = new List<SelectListItem>(){
                        new SelectListItem() { Text= searchFilter.DesignType.ToString(), Value = ((byte)searchFilter.DesignType).ToString() }
                    };
                    libraryManagementDto.libraryManagementSelectedItemsDtos.Add(librarySearchSelectedItemsDto);
                }
            }
            if (searchFilter.Layouts != null && searchFilter.Layouts.Length > 0)
            {
                librarySearchSelectedItemsDto = new LibrarySearchSelectedItemsDto();
                librarySearchSelectedItemsDto.FilterId = searchFilter.LayoutFilterId;
                librarySearchSelectedItemsDto.Label = searchFilter.LayoutLabel;
                librarySearchSelectedItemsDto.Order = 3;
                librarySearchSelectedItemsDto.SelectListItems = libraryLayoutService.GetLibraryLayoutsByIds(searchFilter.Layouts).OrderBy(l => l.Name).
                    Select(d => new SelectListItem { Text = d.Name, Value = d.Id.ToString() }).ToList();
                libraryManagementDto.libraryManagementSelectedItemsDtos.Add(librarySearchSelectedItemsDto);
            }
            if (searchFilter.Components != null && searchFilter.Components.Length > 0)
            {
                librarySearchSelectedItemsDto = new LibrarySearchSelectedItemsDto();
                librarySearchSelectedItemsDto.FilterId = searchFilter.ComponentFilterId;
                librarySearchSelectedItemsDto.Label = searchFilter.ComponentLabel;
                librarySearchSelectedItemsDto.Order = 4;
                librarySearchSelectedItemsDto.SelectListItems = libraryComponentTypeService.GetLibraryComponentTypesByIds(searchFilter.Components).
                OrderBy(c => c.Name).Select(d => new SelectListItem { Text = d.Name, Value = d.Id.ToString() }).ToList();
                libraryManagementDto.libraryManagementSelectedItemsDtos.Add(librarySearchSelectedItemsDto);
            }
            if (searchFilter.Templates != null && searchFilter.Templates.Length > 0)
            {
                librarySearchSelectedItemsDto = new LibrarySearchSelectedItemsDto();
                librarySearchSelectedItemsDto.FilterId = searchFilter.TemplateFilterId;
                librarySearchSelectedItemsDto.Label = searchFilter.TemplateLabel;
                librarySearchSelectedItemsDto.Order = 4;
                librarySearchSelectedItemsDto.SelectListItems = libraryTemplateTypeService.GetLibraryTemplateTypesByIds(searchFilter.Templates).
                OrderBy(c => c.Name).Select(d => new SelectListItem { Text = d.Name, Value = d.Id.ToString() }).ToList();
                libraryManagementDto.libraryManagementSelectedItemsDtos.Add(librarySearchSelectedItemsDto);
            }
            if (searchFilter.Domains != null && searchFilter.Domains.Length > 0)
            {
                librarySearchSelectedItemsDto = new LibrarySearchSelectedItemsDto();
                librarySearchSelectedItemsDto.FilterId = searchFilter.IndustryFilterId;
                librarySearchSelectedItemsDto.Label = searchFilter.IndustryLabel;
                librarySearchSelectedItemsDto.Order = 5;
                librarySearchSelectedItemsDto.SelectListItems = domainTypeService.GetDomainsByIds(searchFilter.Domains).
                OrderBy(i => i.DomainName).
                    Select(d => new SelectListItem { Text = d.DomainName, Value = d.DomainId.ToString() }).ToList();
                libraryManagementDto.libraryManagementSelectedItemsDtos.Add(librarySearchSelectedItemsDto);
            }
            if (searchFilter.Technologies != null && searchFilter.Technologies.Length > 0)
            {
                librarySearchSelectedItemsDto = new LibrarySearchSelectedItemsDto();
                librarySearchSelectedItemsDto.FilterId = searchFilter.TechnologyFilterId;
                librarySearchSelectedItemsDto.Label = searchFilter.TechnologyLabel;
                librarySearchSelectedItemsDto.Order = 6;
                librarySearchSelectedItemsDto.SelectListItems = technologyService.GetTechnologiesByIds(searchFilter.Technologies).OrderBy(t => t.Title).
                Select(d => new SelectListItem { Text = d.Title, Value = d.TechId.ToString() }).ToList();
                libraryManagementDto.libraryManagementSelectedItemsDtos.Add(librarySearchSelectedItemsDto);
            }
            if (searchFilter.IsNDA != null)
            {
                librarySearchSelectedItemsDto = new LibrarySearchSelectedItemsDto();
                librarySearchSelectedItemsDto.FilterId = searchFilter.NDAStatusFilterId;
                librarySearchSelectedItemsDto.Label = searchFilter.NDAStatusLabel;
                librarySearchSelectedItemsDto.Order = 7;
                if ((bool)searchFilter.IsNDA)
                {
                    librarySearchSelectedItemsDto.SelectListItems = new List<SelectListItem>(){
                            new SelectListItem() { Text= "Yes", Value = "1" }
                        };
                }
                else
                {
                    librarySearchSelectedItemsDto.SelectListItems = new List<SelectListItem>(){
                        new SelectListItem() { Text= "No", Value = "0" }
                    };
                }

                libraryManagementDto.libraryManagementSelectedItemsDtos.Add(librarySearchSelectedItemsDto);
            }
            if (searchFilter.Featured != null)
            {
                librarySearchSelectedItemsDto = new LibrarySearchSelectedItemsDto();
                librarySearchSelectedItemsDto.FilterId = searchFilter.FeaturedStatusFilterId;
                librarySearchSelectedItemsDto.Label = searchFilter.FeaturedStatusLabel;
                librarySearchSelectedItemsDto.Order = 8;
                //librarySearchSelectedItemsDto.SelectRadioItems = searchFilter.Featured;
                if ((bool)searchFilter.Featured)
                {
                    librarySearchSelectedItemsDto.SelectListItems = new List<SelectListItem>(){
                        new SelectListItem() { Text= "Yes", Value = "1" }
                    };
                }
                else
                {
                    librarySearchSelectedItemsDto.SelectListItems = new List<SelectListItem>(){
                        new SelectListItem() { Text= "No", Value = "0" }
                    };
                }
                libraryManagementDto.libraryManagementSelectedItemsDtos.Add(librarySearchSelectedItemsDto);
            }
            if (searchFilter.IsReadyToUse != null)
            {
                librarySearchSelectedItemsDto = new LibrarySearchSelectedItemsDto();
                librarySearchSelectedItemsDto.FilterId = searchFilter.IsReadyToUseStatusFilterId;
                librarySearchSelectedItemsDto.Label = searchFilter.IsReadyToUseStatusLabel;
                librarySearchSelectedItemsDto.Order = 9;
                if ((bool)searchFilter.IsReadyToUse)
                {
                    librarySearchSelectedItemsDto.SelectListItems = new List<SelectListItem>(){
                        new SelectListItem() { Text= "Yes", Value = "1" }
                    };
                }
                else
                {
                    librarySearchSelectedItemsDto.SelectListItems = new List<SelectListItem>(){
                        new SelectListItem() { Text= "No", Value = "0" }
                    };
                }
                libraryManagementDto.libraryManagementSelectedItemsDtos.Add(librarySearchSelectedItemsDto);
            }
        }
        private List<string> GetLibraryFileType(ICollection<LibraryFile> libraryFiles)
        {
            List<string> libraryFilesList = null;
            if (libraryFiles != null)
            {
                libraryFilesList = libraryFiles.GroupBy(lf => Path.GetExtension(lf.FilePath.ToLower())).
                            Select(g => g.Key).ToList();
                List<string> libraryWithoutExtension = new List<string>();
                foreach (var libraryFile in libraryFilesList)
                {
                    if (!string.IsNullOrWhiteSpace(libraryFile))
                        libraryWithoutExtension.Add(string.Format("{0}{1}", libraryFile.Remove(0, 1), ".png"));
                }
                libraryFilesList = libraryWithoutExtension;
            }
            return libraryFilesList;
        }

        [Route("~/Portfolio/LibraryIndustriesAndTechnologies")]
        [HttpGet]
        public ResponseModel<LibraryManagementIndexDtoModel> FillLibraryFilterControlsDefault()
        {
            ResponseModel<LibraryManagementIndexDtoModel> response = new ResponseModel<LibraryManagementIndexDtoModel>();
            LibraryManagementIndexDto libraryManagementIndexDto = new LibraryManagementIndexDto();
            try
            {

                libraryManagementIndexDto.Technologies = technologyService.GetLibraryTechnologyList().
                    Select(d => new SelectListItem { Text = d.Value, Value = d.Key.ToString() }).OrderBy(a => a.Text).ToList();

                libraryManagementIndexDto.Industries = domainTypeService.GetLibraryDomainList().
                    Select(d => new SelectListItem { Text = d.Value, Value = d.Key.ToString() }).OrderBy(a=>a.Text).ToList();


                libraryManagementIndexDto.Featured = "";
                libraryManagementIndexDto.IsNda = "";
                libraryManagementIndexDto.IsReadyToUse = "";
                libraryManagementIndexDto.LibraryTypes = WebExtensions.GetSelectList<Enums.LibraryType>();
                libraryManagementIndexDto.LibraryTypes[0].Selected = true;
                libraryManagementIndexDto.DesignTypes = WebExtensions.GetSelectList<Enums.DesignType>();

                //libraryManagementIndexDto.Layouts = libraryLayoutService.GetLibraryLayouts().Where(x => x.LibraryLayoutTypeMapping.Count() > 0).
                //    OrderBy(l => l.Name).Select(d => new SelectListItem { Text = d.Name, Value = d.Id.ToString() }).ToList();
                //libraryManagementIndexDto.Components = libraryComponentTypeService.GetLibraryComponentTypes().
                //    OrderBy(c => c.Name).Select(d => new SelectListItem { Text = d.Name, Value = d.Id.ToString() }).ToList();
                //libraryManagementIndexDto.Templates = libraryTemplateTypeService.GetLibraryTemplateTypes().
                //    OrderBy(c => c.Name).Select(d => new SelectListItem { Text = d.Name, Value = d.Id.ToString() }).ToList();

                //new output model
                LibraryManagementIndexDtoModel libraryManagementIndexDtoModel = new LibraryManagementIndexDtoModel();
                libraryManagementIndexDtoModel.Industries = libraryManagementIndexDto.Industries;
                libraryManagementIndexDtoModel.Technologies = libraryManagementIndexDto.Technologies;
                libraryManagementIndexDtoModel.LibraryTypes = libraryManagementIndexDto.LibraryTypes.Where(x => libraryTypesIds.Contains(x.Value)).ToList();

                response.Code = HttpStatusCode.OK;
                response.Status = true;
                response.Message = "success";
                response.Data = libraryManagementIndexDtoModel;
            }
            catch (Exception ex)
            {
                response.Code = HttpStatusCode.ExpectationFailed;
                response.Status = false;
                response.Message = "failed";
                response.Errors = new string[] { (ex.InnerException ?? ex).Message };
            }
            return response;
        }
    }
}
