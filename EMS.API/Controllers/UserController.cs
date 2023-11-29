using EMS.API.LIBS;
using EMS.API.Model;
using EMS.Core;
using EMS.Data;
using EMS.Service.SARAL;
using EMS.Service.SARALDT;
using EMS.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
//using static EMS.Core.MD5Encryption;
using static EMS.Core.Encryption;
using System.Text.RegularExpressions;
using static EMS.Core.Enums;
using EMS.Dto;
using Microsoft.AspNetCore.Mvc.Rendering;
using EMS.Service.LibraryManagement;
using System.Text;
using EMS.Dto.SARAL;
using System.Data;
using Newtonsoft.Json;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

namespace EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseApiController
    {
        #region Reference Variables

        private readonly IUserLoginService userLoginService;
        private readonly IDepartmentService departmentService;
        private readonly ITechnologyService technologyService;
        private readonly IProjectService projectService;
        private readonly ITimesheetService timesheetService;
        private readonly IVirtualDeveloperService virtualDeveloperService;
        private readonly IComplaintService complaintService;
        private IHostingEnvironment _env;
        private readonly ILibraryManagementService libraryManagementService;

        private readonly IDomainTypeService domainTypeService;

        private readonly ILibrarySearchService librarySearchService;

        private readonly ILibraryDownloadService libraryDownloadService;
        private readonly ILibraryDownloadHistoryService libraryDownloadHistoryService;

        private readonly ITechnologyParentService technologyParentService;
        private readonly ITechnologyParentMappingService technologyParentMappingService;
        private readonly ILibraryTemplateTypeService libraryTemplateTypeService;
        private readonly ILibraryLayoutService libraryLayoutService;
        private readonly ILibraryComponentTypeService libraryComponentTypeService;
        private readonly IUserActivityService userActivityService;
        private readonly ILeaveService leaveService;
        private readonly IWFHService wFHService;
        private readonly ILevDetailsService levDetailsService;
        private readonly ILevDetailsDTService levDetailsDTService;
        private readonly IRoleService _roleService;
        string[] libraryTypesIds = { "0", "1", "3" };//, "2", "4", "5", "6" 

        #endregion

        #region Constructor

        public UserController(IApiKeyService _apiKeyService,
            IUserLoginService _userLoginService,
            IUserActivityService _userActivityService,
            IDepartmentService _departmentService,
            ITechnologyService _technologyService,
            IProjectService _projectService,
            ITimesheetService _timesheetService,
            IComplaintService _complaintService,
            IVirtualDeveloperService _virtualDeveloperService, IHostingEnvironment hostingEnvironment, ITechnologyParentMappingService _technologyParentMappingService, ITechnologyParentService _technologyParentService, ILibraryManagementService _libraryManagementService, IDomainTypeService _domainTypeService,
            ILibrarySearchService _librarySearchService,
            ILibraryDownloadService _libraryDownloadService,
            ILibraryDownloadHistoryService _libraryDownloadHistoryService, ILibraryTemplateTypeService _libraryTemplateTypeService, ILibraryLayoutService _libraryLayoutService
            , ILibraryComponentTypeService _libraryComponentTypeService, IWFHService _wFHService, ILeaveService _leaveService, ILevDetailsService _levDetailsService, ILevDetailsDTService _levDetailsDTService, IRoleService roleService) : base(_apiKeyService)
        {
            userLoginService = _userLoginService;
            departmentService = _departmentService;
            technologyService = _technologyService;
            projectService = _projectService;
            timesheetService = _timesheetService;
            virtualDeveloperService = _virtualDeveloperService;
            complaintService = _complaintService;
            _env = hostingEnvironment;
            technologyParentMappingService = _technologyParentMappingService;
            technologyParentService = _technologyParentService;

            libraryManagementService = _libraryManagementService;

            domainTypeService = _domainTypeService;
            librarySearchService = _librarySearchService;


            libraryDownloadService = _libraryDownloadService;
            libraryDownloadHistoryService = _libraryDownloadHistoryService;
            libraryTemplateTypeService = _libraryTemplateTypeService;
            libraryLayoutService = _libraryLayoutService;
            libraryComponentTypeService = _libraryComponentTypeService;
            userActivityService = _userActivityService;
            leaveService = _leaveService;
            levDetailsService = _levDetailsService;
            levDetailsDTService = _levDetailsDTService;
            _roleService = roleService;
        }

        #endregion

        [Route("~/User/hello")]
        [HttpGet]
        public ResponseModel<string> hello()
        {
            return new ResponseModel<string>
            {
                Status = true,
                Message = "True"
            };
        }


        /// <summary>
        /// Provide Any Filter Input = { "SearchText":"test","IsNDA":true,"Featured":true, "LibraryType": 1,"Domains":[1,2,5],"Technologies":[39,13,4,16,5],"PageStart":0,"DataLength":12 }
        /// </summary>
        /// <param name="libraryManagementSearchFilter"></param>
        /// <returns>LibraryManagementDtoModel</returns>
        [Route("~/User/librarysearch")]        
        [HttpPost]
        public ResponseModel<LibraryManagementDtoModel> SearchLibrary(LibraryManagementSearchFilterDtoModel libraryManagementSearchFilter)
        {
            ResponseModel<LibraryManagementDtoModel> response = new ResponseModel<LibraryManagementDtoModel>();
            try
            {
                int totalCount = 0;
                libraryManagementSearchFilter.PageStart = libraryManagementSearchFilter.PageStart * libraryManagementSearchFilter.DataLength;
                var expr = GetLibraryFilterExpression(libraryManagementSearchFilter);
                var libraries = libraryManagementService.GetLibraries(out totalCount, expr);
                LibraryManagementDto libraryManagementDto = new LibraryManagementDto();
                FillSelectedItemsList(libraryManagementDto, libraryManagementSearchFilter);

                if (libraries != null && libraries.Count > 0)
                {
                    foreach (var library in libraries)
                    {
                        string banner = null;
                        LibraryDto libraryDto = new LibraryDto();
                        libraryDto.Id = library.Id;
                        libraryDto.IsNDA = library.IsNda;
                        libraryDto.CRMUserId = library.CRMUserId != null ? library.CRMUserId : 0;
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
                                        if (libraryFile.LibraryLayoutType != null && libraryManagementSearchFilter.SearchText != null && libraryFile.LibraryLayoutType.Name.ToLower().Contains(libraryManagementSearchFilter.SearchText.ToLower()))
                                        {
                                            libraryDto.BannerImage = libraryFile.FilePath;
                                        }
                                    }
                                }
                            }
                        }
                        if (library.LibraryFile != null && library.LibraryFile.Count > 0)
                        {
                            libraryDto.LibraryFilesList = new List<string>();
                            foreach (var lfile in library.LibraryFile)
                            {
                                string filePath = SiteKeys.DomainName + lfile.FilePath;
                                libraryDto.LibraryFilesList.Add(filePath);
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
                        CRMId = x.CRMUserId,
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
                                Id = y.LibraryLayoutType.Id,
                                Name = y.LibraryLayoutType.Name
                            };
                            libraryFileDtoAPI.LibraryLayoutType = libraryLayoutTypeAPI;
                        }
                        libraryDtoAPI.LibraryFileDtos.Add(libraryFileDtoAPI);
                    });
                    libraryDtoAPI.LibraryImagesList = new List<string>();
                    if (x.LibraryFilesList != null)
                    {
                        x.LibraryFilesList.ForEach(y =>
                        {
                            libraryDtoAPI.LibraryImagesList.Add(y);
                        });
                    }
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
            var pagingServices = new PagingService<Library>(Convert.ToInt32(libraryManagementSearchFilter.PageStart), Convert.ToInt32(libraryManagementSearchFilter.DataLength));
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

                if (!string.IsNullOrEmpty(libraryManagementSearchFilter.CRMId))
                {
                    expr = expr.And(l => (l.CRMUserId == Convert.ToInt32(libraryManagementSearchFilter.CRMId)));
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
                return o.OrderByDescending(c => c.LibraryCreatedDate).OrderByDescending(c => c.IsFeatured);
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

        [Route("~/User/LibraryIndustriesAndTechnologies")]
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
                    Select(d => new SelectListItem { Text = d.Value, Value = d.Key.ToString() }).OrderBy(a => a.Text).ToList();


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
            //ResponseModel<LibraryManagementIndexDtoModel> response = new ResponseModel<LibraryManagementIndexDtoModel>();
            //LibraryManagementIndexDto libraryManagementIndexDto = new LibraryManagementIndexDto();
            //try
            //{
            //    libraryManagementIndexDto.Technologies = technologyService.GetTechnologyList().OrderBy(t => t.Title).
            //        Select(d => new SelectListItem { Text = d.Title, Value = d.TechId.ToString() }).ToList();

            //    libraryManagementIndexDto.Industries = domainTypeService.GetDomainList().OrderBy(i => i.DomainName).
            //    Select(d => new SelectListItem { Text = d.DomainName, Value = d.DomainId.ToString() }).ToList();
            //    libraryManagementIndexDto.Featured = "";
            //    libraryManagementIndexDto.IsNda = "";
            //    libraryManagementIndexDto.IsReadyToUse = "";
            //    libraryManagementIndexDto.LibraryTypes = WebExtensions.GetSelectList<Enums.LibraryType>();
            //    libraryManagementIndexDto.LibraryTypes[0].Selected = true;
            //    libraryManagementIndexDto.DesignTypes = WebExtensions.GetSelectList<Enums.DesignType>();
            //    libraryManagementIndexDto.Layouts = libraryLayoutService.GetLibraryLayouts().Where(x => x.LibraryLayoutTypeMapping.Count() > 0).
            //        OrderBy(l => l.Name).Select(d => new SelectListItem { Text = d.Name, Value = d.Id.ToString() }).ToList();
            //    libraryManagementIndexDto.Components = libraryComponentTypeService.GetLibraryComponentTypes().
            //        OrderBy(c => c.Name).Select(d => new SelectListItem { Text = d.Name, Value = d.Id.ToString() }).ToList();
            //    libraryManagementIndexDto.Templates = libraryTemplateTypeService.GetLibraryTemplateTypes().
            //        OrderBy(c => c.Name).Select(d => new SelectListItem { Text = d.Name, Value = d.Id.ToString() }).ToList();

            //    //new output model
            //    LibraryManagementIndexDtoModel libraryManagementIndexDtoModel = new LibraryManagementIndexDtoModel();
            //    libraryManagementIndexDtoModel.Industries = libraryManagementIndexDto.Industries;
            //    libraryManagementIndexDtoModel.Technologies = libraryManagementIndexDto.Technologies;
            //    libraryManagementIndexDtoModel.LibraryTypes = libraryManagementIndexDto.LibraryTypes.Where(x => libraryTypesIds.Contains(x.Value)).ToList();

            //    response.Code = HttpStatusCode.OK;
            //    response.Status = true;
            //    response.Message = "success";
            //    response.Data = libraryManagementIndexDtoModel;
            //}
            //catch (Exception ex)
            //{
            //    response.Code = HttpStatusCode.ExpectationFailed;
            //    response.Status = false;
            //    response.Message = "failed";
            //    response.Errors = new string[] { (ex.InnerException ?? ex).Message };
            //}
            //return response;
        }

        [Route("~/User/DeactivateUser")]
        [HttpPost]
        //{"HrmId":"1553","EmpCode":"DS2651","IsActive":"1","EmpStatus":"on_notice",
        //"ResignationDate":"10/01/2020","RelievingDate":"01/03/2020"}
        public ResponseModel<string> DeactivateUser(DeactivateUserModel model)
        {
            ResponseModel<string> response = new ResponseModel<string>();

            try
            {
                // Request Authentication
                response = AuthorizeRequest();

                if (response.Status)
                {
                    if (model != null)
                    {
                        var user = userLoginService.GetUser(model.HrmId, model.empCode);
                        if (user == null)
                        {
                            response.Status = false;
                            response.Code = HttpStatusCode.NotFound;
                            response.Errors = new string[] { "User with given HRMId and EmpCode not found" };
                        }
                        else
                        {
                            user.IsActive = model.Active;
                            user.ModifyDate = DateTime.Now;

                            if (!string.IsNullOrWhiteSpace(model.EmpStatus))
                            {
                                user.IsResigned = (model.EmpStatus.Trim().ToLower() == "on_notice" || model.EmpStatus.Trim().ToLower() == "relieve" || model.EmpStatus.Trim().ToLower() == "abscond" || model.EmpStatus.Trim().ToLower() == "terminated");
                                DateTime? resignationDate = model.ResignationDate.ToDateTime("dd/MM/yyyy");
                                DateTime? relievingDate = model.RelievingDate.ToDateTime("dd/MM/yyyy");
                                if (user.IsResigned && resignationDate == null)
                                {
                                    response.Status = false;
                                    response.Code = HttpStatusCode.BadRequest;
                                    response.Errors = new string[] { "Valid resignation date required" };
                                    return response;
                                }
                                else if (user.IsResigned && relievingDate == null)
                                {
                                    response.Status = false;
                                    response.Code = HttpStatusCode.BadRequest;
                                    response.Errors = new string[] { "Valid relieving date required" };
                                    return response;
                                }

                                if (model.EmpStatus.Trim().ToLower() == "abscond")
                                {
                                    user.isAbscond = true;
                                    user.Terminated = false;
                                }
                                else if (model.EmpStatus.Trim().ToLower() == "terminated")
                                {
                                    user.isAbscond = false;
                                    user.Terminated = true;
                                }
                                else
                                {
                                    user.isAbscond = false;
                                    user.Terminated = false;
                                }

                                user.ResignationDate = resignationDate;
                                user.RelievingDate = relievingDate;
                            }
                            userLoginService.UpdateStatus(user);

                            response.Status = true;
                            response.UserId = user.Uid;

                            response.Message = string.Format("User has been {0} successfully", model.Active ? "Activated" : "Deactivated");
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Code = HttpStatusCode.BadRequest;
                        response.Errors = new string[] { "Request parameters are not in correct format!" };
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Code = HttpStatusCode.InternalServerError;
                response.Errors = new string[] { (ex.InnerException ?? ex).Message };
            }

            return response;
        }

        //{"EmailOffice":"Testxx@dotsquares.com","HRMId":"1777","EmpCode":"DS6666","AttendenceId":"",
        //"Title":"Mr.","Name":"Testx","Gender":"M","DOB":"10/10/2000",
        //"PMEmailId":"devendra.sunaria@dotsquares.com","TLEmailId":"devendra.sunaria@dotsquares.com",
        //"JobTitle":"Developer","MobileNumber":"6666666666","DepartmentCode":"DN","Role":"1",
        //"ActionType":"add","JoinedDate":"10/01/2020"}

        //{"EmailOffice":"Testxx@dotsquares.com","HRMId":"1777","EmpCode":"DS6666","AttendenceId":"",
        //"Title":"Mr.","Name":"Testx","Gender":"M","DOB":"10/10/2000",
        //"PMEmailId":"devendra.sunaria@dotsquares.com","TLEmailId":"devendra.sunaria@dotsquares.com",
        //"JobTitle":"Developer","MobileNumber":"6666666666","DepartmentCode":"DN","Role":"1",
        //"ActionType":"update","JoinedDate":"10/01/2020","RelievingDate":"01/03/2020"}

        [Route("~/User/SaveProfile")]
        [HttpPost]
        public ResponseModel<string> SaveProfile(UserLoginModel model)
        {
            ResponseModel<string> response = new ResponseModel<string>();
            var webRoot = _env.WebRootPath;
            var logFilePath = Path.Combine(webRoot, "UserProfileLogFile", $"UserLog-{DateTime.Now.Date.ToString("dd-MMM-yyyy")}.txt");
            try
            {
                // Request Authentication
                response = AuthorizeRequest();

                if (response.Status)
                {
                    if (model != null)
                    {
                        #region User Master Info
                        #region [API Request Log]

                        LogPrint.WriteIntoFile(logFilePath, $"\n---------------------------{DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt")}---------------------------");
                        var options = new JsonSerializerSettings
                        {
                            Formatting = Formatting.Indented,
                            NullValueHandling = NullValueHandling.Include
                        };
                        var jsonString = JsonConvert.SerializeObject(model, options);
                        LogPrint.WriteIntoFile(logFilePath, $"Request Body:\n{jsonString}");
                        LogPrint.WriteIntoFile(logFilePath, string.Empty);
                        #endregion
                        if (model.ActionType == "update")
                        {
                            var isExists = userLoginService.GetLoginDeatilByEmail(model.EmailOffice);
                            if (isExists == null)
                            {
                                model.ActionType = "add";
                            }
                        }
                        model.ActionType = !string.IsNullOrWhiteSpace(model.ActionType) ? model.ActionType.ToLower().Trim() : string.Empty;
                        string pmID = string.Empty;
                        string tlID = string.Empty;
                        string officeid = string.Empty;
                        int? deptId = null;
                        int? roleId = null;

                        int? BloodGroupID = null;
                        //int? HRMiD = null;
                        int modelAttendanceId = 0;
                        int.TryParse(model.AttendenceId, out modelAttendanceId);

                        if (string.IsNullOrWhiteSpace(model.EmailOffice))
                        {
                            response.Message = "Email Office required. ";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }

                        if (!string.IsNullOrWhiteSpace(model.Role))
                        {
                            foreach (Enums.UserRoles urole in Enum.GetValues(typeof(Enums.UserRoles)))
                            {
                                if (urole.ToString().ToLower().Trim() == model.Role.ToLower().Trim())
                                {
                                    roleId = (int)urole;
                                    break;
                                }
                            }
                        }

                        if (!roleId.HasValue)
                        {
                            roleId = (int)Enums.UserRoles.OTH;
                        }
                        if (model.DesignationId > 0)
                        {
                            roleId = _roleService.GetDesignationById(model.DesignationId).RoleId;
                        }
                        model.Gender = (model.Gender ?? "").Trim().ToUpper();

                        UserLogin userLogin = userLoginService.GetLoginDeatilByEmail(model.PMEmailId);

                        if (userLogin != null)
                        {
                            pmID = Convert.ToString(userLogin.Uid);
                        }

                        if (model.ActionType == "add")
                        {
                            if (!string.IsNullOrWhiteSpace(model.TLEmailId))
                            {
                                userLogin = userLoginService.GetLoginDeatilByEmail(model.TLEmailId);
                            }
                            if (userLogin != null)
                            {
                                tlID = Convert.ToString(userLogin.Uid);
                            }
                        }

                        userLogin = userLoginService.GetLoginDeatilByEmail(model.EmailOffice);
                        if (userLogin != null)
                        {
                            officeid = userLogin.EmailOffice;
                        }

                        Department dept = departmentService.GetDepartmentByCode(model.DepartmentCode);

                        deptId = dept?.DeptId ?? (int)Enums.ProjectDepartment.Other;

                        if (!string.IsNullOrWhiteSpace(model.BloodGroup))
                        {
                            foreach (Enums.BloodGroups bloodGroup in Enum.GetValues(typeof(Enums.BloodGroups)))
                            {
                                if (bloodGroup.GetEnumDisplayName().ToLower().Trim() == model.BloodGroup.ToLower().Trim())
                                {
                                    BloodGroupID = (int)bloodGroup;
                                    break;
                                }
                            }
                        }


                        #endregion

                        #region Check Validations

                        if (string.IsNullOrWhiteSpace(model.ActionType))
                        {
                            response.Message = "Action type (add/update) required.";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (model.ActionType == "add" && !string.IsNullOrWhiteSpace(officeid))
                        {
                            response.Message = "Office Mail id already exists, please try another one.";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (model.ActionType == "add" && (userLoginService.CheckHRMId(model.HRMId)))
                        {
                            response.Message = "This HRM Id already exists for other user, please try another one.";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (model.ActionType == "add" && (modelAttendanceId > 0 && userLoginService.CheckAttendanceId(modelAttendanceId)))
                        {
                            response.Message = "This Attendance Id already exists for other user, please try another one.";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (model.ActionType == "add" && (userLoginService.CheckEmpCode(model.EmpCode)))
                        {
                            response.Message = "This employee code already exists for other user, please try another one.";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (model.ActionType == "update" && string.IsNullOrWhiteSpace(officeid) && officeid != model.EmailOffice?.Trim())
                        {
                            response.Message = "User unable to update because user doesn't exist!";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (model.HRMId <= 0)
                        {
                            response.Message = "HRM Id required.";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (modelAttendanceId < 0)
                        {
                            response.Message = "Attendance Id required.";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (string.IsNullOrWhiteSpace(model.EmpCode))
                        {
                            response.Message = "Employee code required.";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (model.ActionType == "update" && userLogin.HRMId != null && userLogin.HRMId != model.HRMId)
                        {
                            response.Message = "User unable to update because HRM Id doesn't match to this user !";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (model.ActionType == "update" && userLogin.AttendenceId != null && userLogin.AttendenceId > 0 && modelAttendanceId > 0 && userLogin.AttendenceId != modelAttendanceId)
                        {
                            response.Message = "User unable to update because Attendance Id doesn't match to this user !";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (model.ActionType == "update" && userLogin.EmpCode != null && userLogin.EmpCode != model.EmpCode?.Trim())
                        {
                            response.Message = "User unable to update because Employee Code doesn't  match to this user!";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (string.IsNullOrWhiteSpace(model.Title))
                        {
                            response.Message = "Title required.";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (string.IsNullOrWhiteSpace(model.Name))
                        {
                            response.Message = "Full Name required. ";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (string.IsNullOrWhiteSpace(model.Gender))
                        {
                            response.Message = "Gender required.";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (model.Gender != "M" && model.Gender != "F")
                        {
                            response.Message = "Gender should be required in M/F format.";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (string.IsNullOrWhiteSpace(model.DOB))
                        {
                            response.Message = "DOB Date required.";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (string.IsNullOrWhiteSpace(pmID))
                        {
                            response.Message = "Project Manager email required.";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (string.IsNullOrWhiteSpace(tlID) && model.ActionType == "add")
                        {
                            response.Message = "Team Leader email required. ";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (string.IsNullOrWhiteSpace(model.JobTitle))
                        {
                            response.Message = "JobTitle required. ";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (deptId == null && model.ActionType == "update")
                        {
                            response.Message = "Correct department code required. ";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (roleId == null && model.ActionType == "update")
                        {
                            response.Message = "Correct Role code required. ";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (string.IsNullOrWhiteSpace(model.MobileNumber))
                        {
                            response.Message = "Mobile Number required.";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (string.IsNullOrWhiteSpace(model.JoinedDate))
                        {
                            response.Message = "Joined Date required.";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }
                        else if (model.DesignationId == 0)
                        {
                            response.Message = "Designation Id required.";
                            response.Status = false;
                            LogPrintEndingMessage(logFilePath, $"Error Message: {response.Message}");
                            return response;
                        }

                        #endregion

                        #region  Save user data

                        else
                        {
                            try
                            {
                                #region  Map user login info

                                UserLogin user = new UserLogin();

                                user.Title = model.Title;
                                user.Name = model.Name;
                                user.Gender = model.Gender;
                                user.DOB = model.DOB.ToDateTime("dd/MM/yyyy");
                                user.JobTitle = !string.IsNullOrWhiteSpace(model.JobTitle) ? model.JobTitle : " ";
                                user.DeptId = deptId;
                                user.RoleId = roleId;
                                user.PMUid = Convert.ToInt16(pmID);

                                user.JoinedDate = model.JoinedDate.ToDateTime("dd/MM/yyyy");

                                // user.Password = "dots@123";
                                // used to save password in Encrypted Format
                                //using (MD5 md5Hash = MD5.Create())
                                //{
                                //    var pass = "dots@123";
                                //    string hash = GetMd5Hash(md5Hash, pass);
                                //    user.Password = pass;
                                //    user.PasswordKey = hash;
                                //}


                                //*********** GetRiJndael Encryption**************Start/// 
                                var keybytes = GetRiJndael_KeyBytes(SiteKeys.Encryption_Key);
                                string strPasswordKey = "dots@123";
                                byte[] encrypted = EncryptStringToBytes(strPasswordKey, keybytes, keybytes);
                                // user.Password = pass;
                                user.PasswordKey = Convert.ToBase64String(encrypted);
                                //*********** GetRiJndael Encryption**************End///

                                user.AddDate = DateTime.Now;
                                user.ModifyDate = DateTime.Now;
                                user.IsActive = true;
                                user.EmailOffice = model.EmailOffice.Trim();
                                user.EmailPersonal = model.EmailPersonal?.Trim();

                                user.MobileNumber = model.MobileNumber.Trim();
                                user.PhoneNumber = model.PhoneNumber?.Trim();
                                user.AlternativeNumber = model.AlternativeNumber?.Trim();
                                user.Address = model.Address?.Trim();
                                user.MarraigeDate = model.MarriageDate.ToDateTime("dd/MM/yyyy");
                                user.AadharNumber = model.AadhaarNumber?.Trim();
                                user.PassportNumber = model.PassportNumber?.ToUpper();
                                user.PanNumber = model.PanNumber?.ToUpper();
                                user.BloodGroupId = BloodGroupID;
                                user.EmpCode = model.EmpCode.ToUpper();
                                user.HRMId = model.HRMId;
                                user.DesignationId = model.DesignationId;
                                if (modelAttendanceId > 0)
                                {
                                    user.AttendenceId = modelAttendanceId;
                                }

                                if (model.ActionType == "add")
                                {
                                    user.UserName = (model.EmailOffice.Split('@')[0]).Trim().ToLower();
                                    user.TLId = Convert.ToInt16(tlID);
                                }
                                else if (model.ActionType == "update")
                                {
                                    user.Uid = userLogin.Uid;
                                    user.UserName = userLogin.UserName;
                                    // user.Password = userLogin.Password;

                                    user.PasswordKey = userLogin.PasswordKey;

                                    user.IsActive = userLogin.IsActive;
                                    user.IsResigned = userLogin.IsResigned;
                                    user.ResignationDate = userLogin.ResignationDate;
                                    DateTime? relievingDate = model.RelievingDate.ToDateTime("dd/MM/yyyy");
                                    user.RelievingDate = relievingDate != null ? relievingDate : userLogin.RelievingDate;
                                    user.IsSPEG = userLogin.IsSPEG;
                                    user.AddDate = string.IsNullOrEmpty(userLogin.AddDate?.ToString()) ? user.AddDate : userLogin.AddDate;
                                    user.HRMId = userLogin.HRMId;
                                    user.DesignationId = model.DesignationId;
                                    if (modelAttendanceId > 0)
                                    {
                                        user.AttendenceId = modelAttendanceId;
                                    }
                                    else
                                    {
                                        user.AttendenceId = userLogin.AttendenceId;
                                    }
                                    user.EmpCode = userLogin.EmpCode;
                                    user.TLId = userLogin.TLId;
                                    user.User_Tech = userLogin.User_Tech.Select(u => new User_Tech { Uid = u.Uid, TechId = u.TechId, SpecTypeId = u.SpecTypeId }).ToList();
                                }

                                List<Technology> technologyList = technologyService.GetTechnologyList();

                                var userTechs = new List<User_Tech>();

                                model.TechDetails.ForEach(t =>
                                {
                                    var technology = technologyList.FirstOrDefault(x => x.Title.ToLower().Trim() == t.Technology.ToLower().Trim());

                                    if (technology != null)
                                    {
                                        userTechs.Add(new User_Tech
                                        {
                                            TechId = technology.TechId,
                                            SpecTypeId = t.Specialist ? (byte)Enums.TechnologySpecializationType.Expert : (byte?)null,
                                            Uid = user.Uid
                                        });
                                    }
                                });

                                if (userTechs.Any())
                                {
                                    foreach (var tech in userTechs)
                                    {
                                        var usrTech = user.User_Tech.FirstOrDefault(x => x.TechId == tech.TechId);

                                        if (usrTech == null)
                                        {
                                            user.User_Tech.Add(new User_Tech
                                            {
                                                Uid = user.Uid,
                                                TechId = tech.TechId,
                                                SpecTypeId = tech.SpecTypeId
                                            });
                                        }
                                    }
                                }

                                #endregion

                                #region Save Profile

                                userLoginService.Save(user);

                                if (model.ActionType == "add")
                                {
                                    SendWelcomeMail(user);
                                    response.Message = "User has been added successfully.";
                                }
                                else if (model.ActionType == "update")
                                {
                                    response.Message = "User has been updated successfully.";
                                }
                                if (SiteKeys.IsSaralLive)
                                {
                                    SARAL_SPCall(user, logFilePath);
                                }
                                response.Status = true;
                                response.UserId = user.Uid;
                                LogPrintEndingMessage(logFilePath, $"Data {(model.ActionType == "add" ? "added" : "updated")} successfully for uid:{user.Uid} on {DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt")}");
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                response.Errors = new string[] { (ex.InnerException ?? ex).Message };
                                response.Status = false;
                                LogPrintEndingMessage(logFilePath, $"Error Message: {Convert.ToString(response.Errors)}");
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        response.Status = false;
                        response.Code = HttpStatusCode.BadRequest;
                        response.Errors = new string[] { "Request parameters are not in correct format!" };
                        LogPrintEndingMessage(logFilePath, $"Error Message: {Convert.ToString(response.Errors)} and response code :{Convert.ToString(response.Code)}");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Code = HttpStatusCode.InternalServerError;
                response.Errors = new string[] { (ex.InnerException ?? ex).Message };
                LogPrintEndingMessage(logFilePath, $"Error Message: {Convert.ToString(response.Errors)} and response code :{Convert.ToString(response.Code)}");
            }

            return response;
        }
        private void SARAL_SPCall(UserLogin UserModelDB, string fileLogPath)
        {

            LogPrint.WriteIntoFile(fileLogPath, $"---------------------------SARAL Desigantion Update SP---------------------------");
            DateTime currentDate = DateTime.Now;
            string DesignationName = string.Empty;
            if (UserModelDB.DesignationId > 0)
            {
                DesignationName = _roleService.GetDesignationById(UserModelDB.DesignationId.Value).Name;
            }
            if (UserModelDB.AttendenceId != null)
            {
                if (UserModelDB.IsFromDbdt != null && UserModelDB.IsFromDbdt.Value == true)
                {
                    try
                    {
                        LogPrint.WriteIntoFile(fileLogPath, $"---------------------------SARAL DT---------------------------");
                        LogPrint.WriteIntoFile(fileLogPath, $"Request: {"AttendenceId=" + UserModelDB.AttendenceId.ToString() + "Name=" + UserModelDB.Name + "Designation" + DesignationName + "Email=" + UserModelDB.EmailOffice + "DateFrom=" + new DateTime(currentDate.Year, currentDate.Month, 1) + "DBNAME=DT"}");
                        //levDetailsDTService.SetDTUserDesignation(UserModelDB.AttendenceId.ToString(), UserModelDB.Name, DesignationName, UserModelDB.EmailOffice, new DateTime(currentDate.Year, currentDate.Month, 1), "DT");
                        levDetailsService.SetUserDesignation(UserModelDB.AttendenceId.ToString(), UserModelDB.Name, DesignationName, UserModelDB.EmailOffice, new DateTime(currentDate.Year, currentDate.Month, 1), "DT");
                        LogPrint.WriteIntoFile(fileLogPath, $"SARAL Data created on:{DateTime.Now}");
                        LogPrint.WriteIntoFile(fileLogPath, "");
                    }
                    catch (Exception ex)
                    {
                        LogPrint.WriteIntoFile(fileLogPath, $"SARAL Data created on:{DateTime.Now}");
                        LogPrint.WriteIntoFile(fileLogPath, $"Error Message:{ex.Message}");
                        LogPrint.WriteIntoFile(fileLogPath, "");
                    }
                }
                else
                {
                    try
                    {
                        LogPrint.WriteIntoFile(fileLogPath, $"---------------------------SARAL DTPL---------------------------");
                        LogPrint.WriteIntoFile(fileLogPath, $"Request: {"AttendenceId=" + UserModelDB.AttendenceId.ToString() + ", Name=" + UserModelDB.Name + ", Designation" + DesignationName + ", Email=" + UserModelDB.EmailOffice + ", DateFrom=" + new DateTime(currentDate.Year, currentDate.Month, 1) + ", DBNAME=DTPL"}");
                        levDetailsService.SetUserDesignation(UserModelDB.AttendenceId.ToString(), UserModelDB.Name, DesignationName, UserModelDB.EmailOffice, new DateTime(currentDate.Year, currentDate.Month, 1), "DTPL");
                        LogPrint.WriteIntoFile(fileLogPath, $"SARAL Data created on:{DateTime.Now}");
                        LogPrint.WriteIntoFile(fileLogPath, "");
                    }
                    catch (Exception ex)
                    {
                        LogPrint.WriteIntoFile(fileLogPath, $"SARAL Data created on:{DateTime.Now}");
                        LogPrint.WriteIntoFile(fileLogPath, $"Error Message:{ex.Message}");
                        LogPrint.WriteIntoFile(fileLogPath, "");
                    }
                }
            }
            else
            {
                LogPrint.WriteIntoFile(fileLogPath, $"{"AttendenceId=Not Assigned Yet,  Name=" + UserModelDB.Name + ", Email=" + UserModelDB.EmailOffice + ", Designation=" + DesignationName}");
                LogPrint.WriteIntoFile(fileLogPath, $"SARAL Update failed on :{DateTime.Now}");
                LogPrint.WriteIntoFile(fileLogPath, "");
            }
        }
        [Route("~/User/SaveTimeSheet")]
        [HttpPost]
        //{"EmailOffice":"devendra.sunaria@dotsquares.com","CRMId":"0","EMSProjectID":"79",
        //"Hours":"5:00","Description":"Test","Date":"08/04/2020","Type":"Additional Free","EMSTimesheetId":"0"}
        public ResponseModel<string> SaveTimeSheet(UserTimeSheetModel model)
        {
            ResponseModel<string> response = new ResponseModel<string>();

            try
            {
                // Request Authentication
                response = AuthorizeRequest();

                if (response.Status)
                {
                    if (model != null)
                    {
                        #region Validation & save

                        int prjtId = 0;
                        int uId = 0;
                        int vdId = 0;
                        TimeSpan wHours;
                        DateTime addDate;

                        #region check required input fields

                        if (string.IsNullOrWhiteSpace(model.EmailOffice))
                        {
                            response.Message = "Email Office required. ";
                            response.Status = false;
                            return response;
                        }

                        if (model.CRMId <= 0 && model.EMSProjectID <= 0)
                        {
                            response.Message = "CRM Id or EMS Project ID required. ";
                            response.Status = false;
                            return response;
                        }

                        if (string.IsNullOrWhiteSpace(model.Hours))
                        {
                            response.Message = "Work hours required. ";
                            response.Status = false;
                            return response;
                        }

                        if (string.IsNullOrWhiteSpace(model.Description))
                        {
                            response.Message = "Description required. ";
                            response.Status = false;
                            return response;
                        }

                        if (string.IsNullOrWhiteSpace(model.Date))
                        {
                            response.Message = "Add date is required. ";
                            response.Status = false;
                            return response;
                        }




                        try
                        {
                            addDate = model.Date.ToDateTime("dd/MM/yyyy").Value;
                        }
                        catch
                        {
                            response.Message = "Add date should be in correct format (dd/MM/yyyy). ";
                            response.Status = false;
                            return response;
                        }

                        try
                        {
                            TimeSpan.TryParse(model.Hours, out wHours);
                        }
                        catch
                        {
                            response.Message = "Work hours should be in correct format (hh:mm). ";
                            response.Status = false;
                            return response;
                        }

                        if (wHours == TimeSpan.Parse("00:00:00"))
                        {
                            response.Message = "Work hours should be in correct format (hh:mm). ";
                            response.Status = false;
                            return response;
                        }

                        #endregion

                        #region get reference data and validate



                        //Project prj = projectService.GetProjectByCRMId(model.CRMId);
                        Project prj = model.CRMId > 0
                            ? projectService.GetProjectByCRMId(model.CRMId) : projectService.GetProjectById(model.EMSProjectID);
                        UserLogin usr = userLoginService.GetLoginDeatilByEmail(model.EmailOffice);
                        VirtualDeveloper virDev = virtualDeveloperService.GetVirtualDeveloperByName(model.Type, false);

                        if (prj != null)
                        {
                            prjtId = prj.ProjectId;
                        }
                        else
                        {
                            response.Message = "CRM ID or EMS Project ID doesn't exist!";
                            response.Status = false;
                            return response;
                        }

                        if (usr != null)
                        {
                            uId = usr.Uid;
                        }
                        else
                        {
                            response.Message = "Email office is not exist!";
                            response.Status = false;
                            return response;
                        }

                        if (virDev != null)
                        {
                            vdId = virDev.VirtualDeveloper_ID;
                        }
                        else
                        {
                            response.Message = "Work type is not exist!";
                            response.Status = false;
                            return response;
                        }

                        #endregion

                        #region save details

                        try
                        {
                            #region Data Mapping
                            UserTimeSheet obj = new UserTimeSheet();

                            if ((model.EMSTimesheetId ?? 0) > 0)
                            {
                                obj = timesheetService.GetTimesheetByTimesheetId((model.EMSTimesheetId ?? 0));
                                if (obj == null)
                                {
                                    response.Message = "EMS timesheet id not found.";
                                    response.Status = false;
                                    return response;
                                }
                            }
                            else
                            {
                                obj.InsertedDate = DateTime.Now.Date;
                            }


                            obj.ProjectID = prjtId;
                            obj.UID = uId;
                            obj.Description = model.Description;
                            obj.AddDate = addDate;
                            obj.ModifyDate = DateTime.Now.Date;
                            obj.WorkHours = wHours;
                            obj.VirtualDeveloper_id = vdId;
                            obj.IsReviewed = false;
                            obj.IsFillByPMS = true;
                            obj.UserTimeSheetID = model.EMSTimesheetId ?? 0;
                            #endregion

                            #region Data save

                            timesheetService.Save(obj);

                            response.Message = "TimeSheet data has been saved successfully.";
                            response.UserId = uId;
                            response.Status = true;
                            response.EMSTimesheetId = (int)obj.UserTimeSheetID;

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            response.Errors = new string[] { (ex.InnerException ?? ex).Message };
                            response.Status = false;
                            return response;
                        }
                        #endregion

                        #endregion
                    }
                    else
                    {
                        response.Status = false;
                        response.Code = HttpStatusCode.BadRequest;
                        response.Errors = new string[] { "Request parameters is not in correct format." };
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Code = HttpStatusCode.InternalServerError;
                response.Errors = new string[] { (ex.InnerException ?? ex).Message };
            }

            return response;
        }

        #region [ GET TOTAL WORKING HOURS OF... ]

        private static Regex CreateValidEmailRegex()
        {
            string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            return new Regex(validEmailPattern, RegexOptions.IgnoreCase);
        }

        internal static bool EmailIsValid(string emailAddress)
        {
            Regex ValidEmailRegex = CreateValidEmailRegex();
            bool isValid = ValidEmailRegex.IsMatch(emailAddress);

            return isValid;
        }


        private ResponseModel<List<TimeSheetWorkHoursModel>> FormatError(ResponseModel<List<TimeSheetWorkHoursModel>> response, string errorType, string errorMessage)
        {
            response.Status = false;
            response.Code = HttpStatusCode.InternalServerError;
            response.Errors = new string[] { !string.IsNullOrEmpty(errorType) ? (errorType + " : " + errorMessage) : string.Empty + errorMessage };
            return response;
        }

        private ResponseStatusModel<UserLeaveAppriaseModel> UserFormatError(ResponseStatusModel<UserLeaveAppriaseModel> response, string errorType, string errorMessage)
        {
            response.Status = false;
            response.Code = HttpStatusCode.InternalServerError;
            response.Errors = new string[] { !string.IsNullOrEmpty(errorType) ? (errorType + " : " + errorMessage) : string.Empty + errorMessage };
            return response;
        }
        private ResponseStatusModel<List<LeaveModel>> UserLeaveFormatError(ResponseStatusModel<List<LeaveModel>> response, string errorType, string errorMessage)
        {
            response.Status = false;
            response.Code = HttpStatusCode.InternalServerError;
            response.Errors = new string[] { !string.IsNullOrEmpty(errorType) ? (errorType + " : " + errorMessage) : string.Empty + errorMessage };
            return response;
        }
        private ResponseStatusModel<List<WorkingDaysModel>> UserLeaveFormatError(ResponseStatusModel<List<WorkingDaysModel>> response, string errorType, string errorMessage)
        {
            response.Status = false;
            response.Code = HttpStatusCode.InternalServerError;
            response.Errors = new string[] { !string.IsNullOrEmpty(errorType) ? (errorType + " : " + errorMessage) : string.Empty + errorMessage };
            return response;
        }
        private ResponseStatusModel<ReviewModel> UserReviewFormatError(ResponseStatusModel<ReviewModel> response, string errorType, string errorMessage)
        {
            response.Status = false;
            response.Code = HttpStatusCode.InternalServerError;
            response.Errors = new string[] { !string.IsNullOrEmpty(errorType) ? (errorType + " : " + errorMessage) : string.Empty + errorMessage };
            return response;
        }

        [Route("~/User/CalculateWorkingHours")]
        [HttpPost]
        public IActionResult GetTotalHours(SearchWorkHoursModel model)
        {
            ResponseModel<string> authResponse = new ResponseModel<string>();
            ResponseModel<List<TimeSheetWorkHoursModel>> response = new ResponseModel<List<TimeSheetWorkHoursModel>>();
            try
            {
                authResponse = AuthorizeRequest();
                if (authResponse.Status)
                {
                    //if (string.IsNullOrWhiteSpace(model.month) || string.IsNullOrWhiteSpace(model.year)) {
                    //    response.Status = false;
                    //    response.Code = HttpStatusCode.NotAcceptable;
                    //    response.Errors = new string[] { "Invalid Input Parameters." };
                    //}
                    //else {
                    int intMonth = 0; int intYear = 0;
                    string errorType = string.Empty;
                    if (!string.IsNullOrEmpty(model.month))
                    {
                        try
                        {
                            intMonth = Convert.ToInt32(model.month);
                        }
                        catch (Exception ex)
                        {
                            response = FormatError(response, "month", (ex.InnerException ?? ex).Message);
                            return Ok(response);
                        }
                    }

                    if (!string.IsNullOrEmpty(model.year))
                    {
                        try
                        {
                            intYear = Convert.ToInt32(model.year);
                        }
                        catch (Exception ex)
                        {
                            response = FormatError(response, "year", (ex.InnerException ?? ex).Message);
                            return Ok(response);
                        }
                    }
                    if (!string.IsNullOrEmpty(model.email))
                    {
                        var validEmail = EmailIsValid(model.email);

                        if (!validEmail)
                        {
                            response = FormatError(response, "email", "email address is not valid.");
                            return Ok(response);
                        }

                        var emailExists = timesheetService.EmailIdExists(model.email);
                        if (!emailExists)
                        {
                            response = FormatError(response, string.Empty, "email id does not exists.");
                            return Ok(response);
                        }
                    }

                    List<TimeSheetWorkHoursModel> rTimeHours = new List<TimeSheetWorkHoursModel>();
                    IEnumerable<UserTimeSheet> userTimeSheets = timesheetService.GetAllTimeSheetHours(intMonth, intYear, model.email);

                    //if (!string.IsNullOrEmpty(model.email) && string.IsNullOrEmpty(model.month) && string.IsNullOrEmpty(model.year)) {
                    rTimeHours = userTimeSheets.GroupBy(x => x.UserLogin1.EmailOffice).Select(z => new TimeSheetWorkHoursModel
                    {
                        MemberEmail = z.Key,
                        MonthData = z.GroupBy(e => new { e.AddDate.Date.Month, e.AddDate.Date.Year }).Select(y => new MonthData
                        {
                            Month = y.Key.Month,
                            Year = y.Key.Year,
                            Hours = $"{Convert.ToInt32(Math.Floor(new TimeSpan(0, (int)y.Sum(x => x.WorkHours.TotalMinutes), 0).TotalHours))}:{new TimeSpan(0, (int)y.Sum(x => x.WorkHours.TotalMinutes), 0).Minutes}"

                        }).ToList()

                    }).ToList();
                    //}
                    //else {
                    //     rTimeHours = userTimeSheets.GroupBy(x => x.UserLogin1.EmailOffice).Select(y => new TimeSheetWorkHoursModel {
                    //        MemberEmail = y.Select(x => x.UserLogin1.EmailOffice).FirstOrDefault(),
                    //        MonthData = new List<MonthData>(){ new MonthData {
                    //                Month=y.Select(x => x.AddDate.Date.Month).FirstOrDefault(),
                    //                Year=y.Select(x => x.AddDate.Date.Year).FirstOrDefault(),
                    //                Hours=$"{Convert.ToInt32(Math.Floor(new TimeSpan(0,(int)y.Sum(x=>x.WorkHours.TotalMinutes),0).TotalHours))}:{new TimeSpan(0,(int)y.Sum(x=>x.WorkHours.TotalMinutes),0).Minutes}"

                    //   } }

                    //    }).ToList();
                    //}
                    if (rTimeHours != null)
                    {
                        response.Data = rTimeHours;
                        response.Status = true;
                        response.Code = HttpStatusCode.Found;
                    }
                    //}
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Code = HttpStatusCode.InternalServerError;
                response.Errors = new string[] { (ex.InnerException ?? ex).Message };
            }

            return Ok(response);
        }

        #endregion [ GET TOTAL WORKING HOURS OF.... ]

        private void SendWelcomeMail(UserLogin obj)
        {
            try
            {
                var keybytes = GetRiJndael_KeyBytes(SiteKeys.Encryption_Key);    //Add pass =passwordkey
                byte[] TempEncrypted = Encoding.UTF8.GetBytes(obj.PasswordKey);
                byte[] TempmyRijndaelKey2 = Encoding.UTF8.GetBytes(Convert.ToBase64String(keybytes));
                byte[] encrypted = Convert.FromBase64String(Encoding.ASCII.GetString(TempEncrypted));
                byte[] myRijndaelKey = Convert.FromBase64String(Encoding.ASCII.GetString(TempmyRijndaelKey2));
                obj.PasswordKey = DecryptStringFromBytes(encrypted, myRijndaelKey, myRijndaelKey);
                FlexiMail objSendMail = new FlexiMail();

                var v0 = "";  //user first name
                var v1 = "";  //portal link
                var v2 = "";  //user-id
                var v3 = "";  //password

                v0 = obj.Name.Split(' ')[0];
                v1 = SiteKeys.DomainName;
                v2 = obj.EmailOffice;
                v3 = obj.PasswordKey;

                var ValueArray = new string[] { v0, v1, v2, v3 };
                objSendMail.ValueArray = ValueArray;

                var webRoot = _env.WebRootPath;
                var filePath = Path.Combine(webRoot, "EmailTemplate", $"WelcomeEmail.html");

                var templatedata = objSendMail.GetHtml(filePath);

                objSendMail.From = SiteKeys.From; //"info@dotsquares.com";
                objSendMail.To = obj.EmailOffice;  //bhanwar.mali@dotsquares.com
                objSendMail.CC = SiteKeys.HREmailId;
                objSendMail.BCC = "devendra.sunaria@dotsquares.com";
                objSendMail.Subject = obj.Name + ", Welcome to Dotsquares";
                objSendMail.MailBodyManualSupply = true;
                objSendMail.MailBody = templatedata;
                objSendMail.Send();
            }
            catch (Exception ex)
            {

            }
        }

        [Route("~/User/LeaveAppraise")]
        [HttpPost]
        public IActionResult GetUserLeaveAppraise(SearchWorkHoursModel model)
        {
            ResponseStatusModel<string> authResponse = new ResponseStatusModel<string>();
            ResponseStatusModel<UserLeaveAppriaseModel> responseMsg = new ResponseStatusModel<UserLeaveAppriaseModel>();
            try
            {
                authResponse = AuthorizeRequestStatus();
                if (authResponse.Status)
                {
                    if (string.IsNullOrWhiteSpace(model.email) && string.IsNullOrWhiteSpace(model.year))
                    {
                        responseMsg.Status = false;
                        responseMsg.Code = HttpStatusCode.NotAcceptable;
                        responseMsg.Errors = new string[] { "Invalid Input Parameters." };
                    }

                    int intYear = 0;
                    string errorType = string.Empty;

                    if (!string.IsNullOrEmpty(model.email))
                    {
                        var validEmail = EmailIsValid(model.email);

                        if (!validEmail)
                        {
                            responseMsg = UserFormatError(responseMsg, "email", "email address is not valid.");
                            return Ok(responseMsg);
                        }
                    }
                    else
                    {
                        responseMsg.Status = false;
                        responseMsg.Message = "User email address required";
                        responseMsg.Code = HttpStatusCode.NotFound;
                        responseMsg.Errors = new string[] { "Please enter user email address" };
                        return Ok(responseMsg);
                    }

                    if (!string.IsNullOrEmpty(model.year))
                    {
                        try
                        {
                            intYear = Convert.ToInt32(model.year);
                        }
                        catch (Exception ex)
                        {
                            responseMsg = UserFormatError(responseMsg, "year", (ex.InnerException ?? ex).Message);
                            return Ok(responseMsg);
                        }
                    }
                    else
                    {
                        intYear = DateTime.Now.Year;
                    }

                    UserLeaveAppriaseModel responseUserData = new UserLeaveAppriaseModel();


                    responseUserData.year = intYear == 0 ? "Not available" : intYear.ToString();
                    responseUserData.email = model.email.ToLower();
                    var userEntity = userLoginService.GetLoginDeatilByEmail(model.email.ToLower());
                    decimal countTotalLeaves = 0;
                    decimal countTotalLeavesInYear = 0;
                    if (userEntity != null)
                    {
                        var userLeaves = userEntity.LeaveActivities1;
                        if (userLeaves != null)
                        {
                            var FullDay = userLeaves.Where(a => a.IsHalf == false && a.Status != 8).ToList();
                            var HalfDay = userLeaves.Count(a => a.IsHalf == true && a.Status != 8);

                            foreach (var item in FullDay)
                            {
                                DateTime dtStart = item.StartDate;
                                DateTime dtEnd = item.EndDate;
                                var DtDifference = (dtEnd - dtStart).TotalDays;

                                countTotalLeaves = countTotalLeaves + (DtDifference > 0 ? Convert.ToInt32(DtDifference) : 1);

                            }
                            countTotalLeaves = countTotalLeaves + (HalfDay > 0 ? Convert.ToDecimal(HalfDay / 2.0) : 0);

                            var FullDayYear = userLeaves.Where(a => a.IsHalf == false && a.Status != 8 && a.StartDate.Year == intYear).ToList();
                            var HalfDayYear = userLeaves.Count(a => a.IsHalf == true && a.Status != 8 && a.StartDate.Year == intYear);

                            foreach (var item in FullDayYear)
                            {
                                DateTime dtStart = item.StartDate;
                                DateTime dtEnd = item.EndDate;
                                var DtDifference = (dtEnd - dtStart).TotalDays;

                                countTotalLeavesInYear = countTotalLeavesInYear + (DtDifference > 0 ? Convert.ToInt32(DtDifference) : 1);

                            }
                            countTotalLeavesInYear = countTotalLeavesInYear + (HalfDayYear > 0 ? Convert.ToDecimal(HalfDayYear / 2.0) : 0);
                        }

                        responseUserData.totalleavestaken = countTotalLeaves;// userLeaves != null ? userLeaves.Count() : 0;
                        responseUserData.totalleavesinyear = countTotalLeavesInYear;// userLeaves != null ? (intYear > 0 ? userLeaves.Count(a => a.StartDate.Year == intYear && a.Status!=8 && ) : userLeaves.Count()): 0;
                        responseUserData.totalcomplaininyear = userEntity.ComplaintUser != null ? intYear > 0 ? userEntity.ComplaintUser.Count(a => a.Complaint.AddedDate.Year == intYear) : userEntity.ComplaintUser.Count() : 0;
                        responseUserData.totalappreciationinyear = userEntity.EmployeeAppraiseEmployee != null ? intYear > 0 ? userEntity.EmployeeAppraiseEmployee.Count(a => a.AddDate.Value.Year == intYear) : userEntity.EmployeeAppraiseEmployee.Count() : 0;

                        if (responseUserData != null)
                        {
                            responseMsg.Data = responseUserData;
                            responseMsg.Status = true;
                            responseMsg.Code = HttpStatusCode.OK;
                        }
                    }
                    else
                    {
                        responseMsg.Status = false;
                        responseMsg.Code = HttpStatusCode.NotFound;
                        responseMsg.Errors = new string[] { "User not found in system, please check email is valid" };
                    }

                }
                else
                {
                    responseMsg.Status = false;
                    responseMsg.Message = "Invalid authentication found";
                    responseMsg.Code = HttpStatusCode.NetworkAuthenticationRequired;
                    responseMsg.Errors = new string[] { "Invalid authentication found" };
                }
            }
            catch (Exception ex)
            {
                responseMsg.Status = false;
                responseMsg.Code = HttpStatusCode.InternalServerError;
                responseMsg.Errors = new string[] { (ex.InnerException ?? ex).Message };
            }

            return Ok(responseMsg);
        }

        [Route("~/User/Leaves")]
        [HttpPost]
        public IActionResult GetUserLeaves(SearchLeaveModel model)
        {
            ResponseStatusModel<string> authResponse = new ResponseStatusModel<string>();
            ResponseStatusModel<List<LeaveModel>> responseMsg = new ResponseStatusModel<List<LeaveModel>>();
            try
            {
                DateTime? dateFrom = null;
                DateTime? dateTo = null;

                authResponse = AuthorizeRequestStatus();
                if (authResponse.Status)
                {
                    if (string.IsNullOrWhiteSpace(model.memberid) && string.IsNullOrWhiteSpace(model.fromdate) && string.IsNullOrWhiteSpace(model.todate))
                    {
                        responseMsg.Status = false;
                        responseMsg.Code = HttpStatusCode.NotAcceptable;
                        responseMsg.Errors = new string[] { "Invalid Input Parameters." };
                    }

                    string errorType = string.Empty;

                    if (!string.IsNullOrEmpty(model.memberid))
                    {
                        var validEmail = EmailIsValid(model.memberid);

                        if (!validEmail)
                        {
                            responseMsg.Status = false;
                            responseMsg = UserLeaveFormatError(responseMsg, "email", "email address is not valid.");
                            return Ok(responseMsg);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(model.fromdate))
                    {
                        try
                        {
                            dateFrom = model.fromdate.ToDateTime();
                        }
                        catch (Exception ex)
                        {
                            responseMsg.Status = false;
                            responseMsg.Message = "From date format is not correct";
                            return Ok(responseMsg);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(model.todate))
                    {
                        try
                        {
                            dateTo = model.todate.ToDateTime();
                        }
                        catch (Exception ex)
                        {
                            responseMsg.Status = false;
                            responseMsg.Message = "To date format is not correct";
                            return Ok(responseMsg);
                        }
                    }


                    else
                    {
                        responseMsg.Status = false;
                        responseMsg.Message = "User email address required";
                        responseMsg.Code = HttpStatusCode.NotFound;
                        responseMsg.Errors = new string[] { "Please enter user email address" };
                        return Ok(responseMsg);
                    }

                    List<LeaveModel> responseUserData = new List<LeaveModel>();

                    var userEntity = userLoginService.GetLoginDeatilByEmail(model.memberid.ToLower());

                    if (userEntity != null)
                    {
                        var userLeaves = userEntity.LeaveActivities1?.Where(a => a.Status != (int)LeaveStatus.Cancelled && (dateFrom != null ? a.StartDate >= dateFrom : true) && (dateTo != null ? a.StartDate <= dateTo : true)).ToList(); ;
                        if (userLeaves != null)
                        {
                            foreach (var item in userLeaves)
                            {
                                DateTime dtStart = item.StartDate;
                                DateTime dtEnd = item.EndDate;

                                var DtDifference = (dtEnd - dtStart).TotalDays;

                                int countTotalLeaves = (DtDifference > 0 ? Convert.ToInt32(DtDifference) : 1);

                                LeaveModel userLeave = new LeaveModel();
                                userLeave.Date = item.StartDate.ToString("dd-MM-yyyy");
                                userLeave.Status = item.Status > 0 ? ((LeaveStatus)item.Status).ToString() : string.Empty;
                                userLeave.LeaveType = item.IsHalf == true ? "HalfDay" : "FullDay";
                                userLeave.NoOfDay = item.IsHalf == true ? 0.5 : countTotalLeaves;

                                responseUserData.Add(userLeave);

                                //send per day data with leave status means break date range
                                //for (int i = 0; i < countTotalLeaves; i++)
                                //{
                                //    LeaveModel userLeave = new LeaveModel();
                                //    userLeave.Date = item.StartDate.AddDays(i).ToString("dd-MM-yyyy");
                                //    userLeave.Status = item.Status > 0 ? ((LeaveStatus)item.Status).ToString() : string.Empty;
                                //    userLeave.LeaveType = item.IsHalf == true ? "HalfDay" : "FullDay";
                                //    responseUserData.Add(userLeave);
                                //}
                            }
                        }

                        if (responseUserData != null)
                        {
                            responseMsg.Data = responseUserData;
                            responseMsg.Message = "success";
                            responseMsg.Status = true;
                            responseMsg.Code = HttpStatusCode.OK;
                        }
                    }
                    else
                    {
                        responseMsg.Status = false;
                        responseMsg.Code = HttpStatusCode.NotFound;
                        responseMsg.Errors = new string[] { "User not found in system, please check email is valid" };
                    }

                }
                else
                {
                    responseMsg.Status = false;
                    responseMsg.Message = "Invalid authentication found";
                    responseMsg.Code = HttpStatusCode.NetworkAuthenticationRequired;
                    responseMsg.Errors = new string[] { "Invalid authentication found" };
                }
            }
            catch (Exception ex)
            {
                responseMsg.Status = false;
                responseMsg.Code = HttpStatusCode.InternalServerError;
                responseMsg.Errors = new string[] { (ex.InnerException ?? ex).Message };
            }

            return Ok(responseMsg);
        }

        [Route("~/User/LeavesCount")]
        [HttpPost]
        public IActionResult GetUserLeavesAndWorkingDaysCount(SearchLeaveModel model)
        {
            ResponseStatusModel<string> authResponse = new ResponseStatusModel<string>();
            ResponseStatusModel<TimeSheet> responseMsg = new ResponseStatusModel<TimeSheet>();
            try
            {
                DateTime? dateFrom = null;
                DateTime? dateTo = null;

                authResponse = AuthorizeRequestStatus();
                if (authResponse.Status)
                {
                    if (string.IsNullOrWhiteSpace(model.memberid) && string.IsNullOrWhiteSpace(model.fromdate) && string.IsNullOrWhiteSpace(model.todate))
                    {
                        responseMsg.Status = false;
                        responseMsg.Code = HttpStatusCode.NotAcceptable;
                        responseMsg.Errors = new string[] { "Invalid Input Parameters." };
                    }

                    string errorType = string.Empty;

                    if (!string.IsNullOrEmpty(model.memberid))
                    {
                        var validEmail = EmailIsValid(model.memberid);

                        if (!validEmail)
                        {
                            responseMsg.Status = false;
                            responseMsg.Errors = new string[] { "email address is not valid." };
                            return Ok(responseMsg);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(model.fromdate))
                    {
                        try
                        {
                            dateFrom = model.fromdate.ToDateTime();
                        }
                        catch (Exception ex)
                        {
                            responseMsg.Status = false;
                            responseMsg.Message = "From date format is not correct";
                            return Ok(responseMsg);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(model.todate))
                    {
                        try
                        {
                            dateTo = model.todate.ToDateTime();
                        }
                        catch (Exception ex)
                        {
                            responseMsg.Status = false;
                            responseMsg.Message = "To date format is not correct";
                            return Ok(responseMsg);
                        }
                    }

                    else
                    {
                        responseMsg.Status = false;
                        responseMsg.Message = "User email address required";
                        responseMsg.Code = HttpStatusCode.NotFound;
                        responseMsg.Errors = new string[] { "Please enter user email address" };
                        return Ok(responseMsg);
                    }

                    var userEntity = userLoginService.GetLoginDeatilByEmail(model.memberid.ToLower());

                    if (userEntity != null)
                    {
                        var userLeaves = userEntity.LeaveActivities1?.Where(a => a.Status == (int)LeaveStatus.Approved && (dateFrom != null ? a.StartDate >= dateFrom : true) && (dateTo != null ? a.StartDate <= dateTo : true)).ToList();

                        if (userLeaves != null)
                        {
                            //// ---------- To calculate user total working days of the given date range excluding Half-Days -----------------
                            var userActivityDays = userActivityService.GetUserActivityByUidDateRange(userEntity.Uid, dateFrom, dateTo);

                            var fullDayLeave = userLeaves.Where(x => x.IsHalf != true).Count();
                            var halfDayLeave = userLeaves.Where(x => x.IsHalf == true).Count();
                            double totalWorkingDays = 0;

                            if (userActivityDays > 0)
                            {
                                totalWorkingDays = Convert.ToDouble(userActivityDays - Convert.ToDouble(halfDayLeave > 0 ? halfDayLeave * 0.5 : 0));
                            }

                            else
                            {
                                double daycount = GetWorkingDays(dateFrom, dateTo);
                                var monthsBetween = ((dateTo.Value.Year - dateFrom.Value.Year) * 12) + dateTo.Value.Month - dateFrom.Value.Month;
                                var totalDays = (dateTo.Value.Date.AddDays(1) - dateFrom.Value.Date).TotalDays;
                                totalWorkingDays = Convert.ToDouble(daycount + (monthsBetween != 0 ? monthsBetween * 2 : 2) - (fullDayLeave > 0 ? fullDayLeave : 0) - Convert.ToDouble(halfDayLeave > 0 ? halfDayLeave * 0.5 : 0));
                            }

                            WorkingDaysModel workingModel = new WorkingDaysModel()
                            {
                                workingDays = totalWorkingDays.ToString(),
                                fullDayLeaves = fullDayLeave.ToString(),
                                halfDayLeaves = halfDayLeave.ToString()
                            };

                            responseMsg.Data = new TimeSheet() { timeSheet = workingModel };
                            responseMsg.Message = "success";
                            responseMsg.Status = true;
                            responseMsg.Code = HttpStatusCode.OK;

                            return Ok(responseMsg);
                        }
                    }
                    else
                    {
                        responseMsg.Status = false;
                        responseMsg.Code = HttpStatusCode.NotFound;
                        responseMsg.Errors = new string[] { "User not found in system, please check email is valid" };
                    }

                }
                else
                {
                    responseMsg.Status = false;
                    responseMsg.Message = "Invalid authentication found";
                    responseMsg.Code = HttpStatusCode.NetworkAuthenticationRequired;
                    responseMsg.Errors = new string[] { "Invalid authentication found" };
                }
            }
            catch (Exception ex)
            {
                responseMsg.Status = false;
                responseMsg.Code = HttpStatusCode.InternalServerError;
                responseMsg.Errors = new string[] { (ex.InnerException ?? ex).Message };
            }

            return Ok(responseMsg);
        }

        [Route("~/User/Review")]
        [HttpPost]
        public IActionResult GetUserReview(SearchLeaveModel model)
        {
            ResponseStatusModel<string> authResponse = new ResponseStatusModel<string>();
            ResponseStatusModel<ReviewModel> responseMsg = new ResponseStatusModel<ReviewModel>();
            try
            {
                DateTime? dateFrom = null;
                DateTime? dateTo = null;

                authResponse = AuthorizeRequestStatus();
                if (authResponse.Status)
                {
                    if (string.IsNullOrWhiteSpace(model.memberid) && string.IsNullOrWhiteSpace(model.fromdate) && string.IsNullOrWhiteSpace(model.todate))
                    {
                        responseMsg.Status = false;
                        responseMsg.Code = HttpStatusCode.NotAcceptable;
                        responseMsg.Errors = new string[] { "Invalid Input Parameters." };
                    }

                    string errorType = string.Empty;

                    if (!string.IsNullOrEmpty(model.memberid))
                    {
                        var validEmail = EmailIsValid(model.memberid);

                        if (!validEmail)
                        {
                            responseMsg.Status = false;
                            responseMsg = UserReviewFormatError(responseMsg, "email", "email address is not valid.");
                            return Ok(responseMsg);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(model.fromdate))
                    {
                        try
                        {
                            dateFrom = model.fromdate.ToDateTime();
                        }
                        catch (Exception ex)
                        {
                            responseMsg.Status = false;
                            responseMsg.Message = "From date format is not correct";
                            return Ok(responseMsg);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(model.todate))
                    {
                        try
                        {
                            dateTo = model.todate.ToDateTime();
                        }
                        catch (Exception ex)
                        {
                            responseMsg.Status = false;
                            responseMsg.Message = "To date format is not correct";
                            return Ok(responseMsg);
                        }
                    }


                    else
                    {
                        responseMsg.Status = false;
                        responseMsg.Message = "User email address required";
                        responseMsg.Code = HttpStatusCode.NotFound;
                        responseMsg.Errors = new string[] { "Please enter user email address" };
                        return Ok(responseMsg);
                    }

                    ReviewModel responseUserData = new ReviewModel();

                    var userEntity = userLoginService.GetLoginDeatilByEmail(model.memberid.ToLower());

                    if (userEntity != null)
                    {
                        //var userComplaints = userEntity.EmployeeComplaintUser?.Where(a => a.IsDelete == false && a.IsActive == true && (dateFrom != null ? a.AddDate.Value.Date >= dateFrom : true)
                        //                                             && (dateTo != null ? a.AddDate.Value.Date <= dateTo : true))?.ToList() ?? new List<EmployeeComplaint>();

                        var expr = PredicateBuilder.True<Complaint>();
                        expr = expr.And(e => e.ComplaintUser.Any(cu => cu.UserLoginId == userEntity.Uid));
                        expr.And(e => e.IsDelete == false);

                        //if (dateFrom.HasValue)
                        //{
                        //    expr = expr.And(e => e.AddedDate >= dateFrom.Value);
                        //}
                        //else if (dateTo.HasValue)
                        //{
                        //    expr = expr.And(e => e.AddedDate <= dateTo.Value);
                        //}

                        if (dateFrom.HasValue)
                        {
                            expr = expr.And(e => e.AddedDate >= dateFrom.Value || e.ClientComplainDate.Value >= dateFrom.Value);
                        }
                        else if (dateTo.HasValue)
                        {
                            expr = expr.And(e => e.AddedDate <= dateTo.Value || e.ClientComplainDate.Value <= dateTo.Value);
                        }

                        var userComplaints = complaintService.GetComplaintsByFilter(expr);

                        if (userComplaints != null && userComplaints.Any())
                        {
                            foreach (var item in userComplaints)
                            {
                                ComplainTypeModel complainType = new ComplainTypeModel();
                                complainType.Type = item.ComplaintType > 0 ? ((ComplainType)item.ComplaintType).ToString() : string.Empty;
                                complainType.EmsId = item.Id.ToString();
                                //complainType.Project = string.Empty;
                                complainType.Severioty = item.Priority > 0 ? ((ComplainPriority)item.Priority).ToString() : string.Empty; ;
                                complainType.CreatedOn = item.AddedDate.ToString("dd-MM-yyyy") ?? string.Empty;
                                complainType.TLPMCoolments = item.TlExplanation;
                                responseUserData.ComplainType.Add(complainType);
                            }
                        }

                        //var userAppraisals = userEntity.EmployeeAppraiseEmployee?.Where(a => a.IsDelete == false && a.IsActive == true
                        //&& (dateFrom != null ? (a.AddDate.Value.Date >= dateFrom || a.ClientDate.Value.Date >= dateFrom) : true)
                        //&& (dateTo != null ? (a.AddDate.Value.Date <= dateTo ||  a.ClientDate.Value.Date <= dateTo): true))?.ToList() ?? new List<EmployeeAppraise>();

                        var userAppraisals = userEntity.EmployeeAppraiseEmployee?.Where(a => a.IsDelete == false && a.IsActive == true
                       && (dateFrom != null ? (a.ClientDate.HasValue ? (a.AddDate.Value.Date >= dateFrom || a.ClientDate.Value.Date >= dateFrom) : a.AddDate.Value.Date >= dateFrom) : true)
                       && (dateTo != null ? (a.ClientDate.HasValue ? (a.AddDate.Value.Date <= dateTo || a.ClientDate.Value.Date <= dateTo) : a.AddDate.Value.Date <= dateTo) : true))?.ToList() ?? new List<EmployeeAppraise>();


                        if (userAppraisals != null && userAppraisals.Any())
                        {
                            foreach (var item in userAppraisals)
                            {
                                AppraisedTypeModel appraisedType = new AppraisedTypeModel();
                                appraisedType.Type = item.AppraiseType > 0 ? ((AppraiseType)item.AppraiseType).ToString() : string.Empty;
                                appraisedType.EmsId = item.Id.ToString();
                                //appraisedType.Project = string.Empty;
                                appraisedType.CreatedOn = item.AddDate?.ToString("dd-MM-yyyy") ?? string.Empty;
                                appraisedType.TLPMCoolments = item.TlComment;
                                responseUserData.AppraisedType.Add(appraisedType);
                            }
                        }

                        if (responseUserData != null)
                        {
                            responseMsg.Data = responseUserData;
                            responseMsg.Message = "success";
                            responseMsg.Status = true;
                            responseMsg.Code = HttpStatusCode.OK;
                        }
                    }
                    else
                    {
                        responseMsg.Status = false;
                        responseMsg.Code = HttpStatusCode.NotFound;
                        responseMsg.Errors = new string[] { "User not found in system, please check email is valid" };
                    }

                }
                else
                {
                    responseMsg.Status = false;
                    responseMsg.Message = "Invalid authentication found";
                    responseMsg.Code = HttpStatusCode.NetworkAuthenticationRequired;
                    responseMsg.Errors = new string[] { "Invalid authentication found" };
                }
            }
            catch (Exception ex)
            {
                responseMsg.Status = false;
                responseMsg.Code = HttpStatusCode.InternalServerError;
                responseMsg.Errors = new string[] { (ex.InnerException ?? ex).Message };
            }

            return Ok(responseMsg);
        }

        [Route("~/User/JoiningDate")]
        [HttpPost]
        //{"EmailOffice":"devendra.sunaria@dotsquares.com","CRMId":"0","EMSProjectID":"79",
        //"Hours":"5:00","Description":"Test","Date":"08/04/2020","Type":"Additional Free","EMSTimesheetId":"0"}
        public IActionResult JoiningDate(List<JoiningModel> model)
        {
            ResponseStatusModel<string> authResponse = new ResponseStatusModel<string>();
            ResponseModel<List<JoiningModel>> response = new ResponseModel<List<JoiningModel>>();
            authResponse = AuthorizeRequestStatus();
            if (authResponse.Status)
            {
                if (model != null && model.Count > 0)
                {
                    List<JoiningModel> usersModel = new List<JoiningModel>();
                    try
                    {
                        foreach (var item in model)
                        {
                            if (!string.IsNullOrEmpty(item.Email))
                            {
                                var validEmail = EmailIsValid(item.Email);

                                if (!validEmail)
                                {
                                    response.Status = false;
                                    response.Message = "email address is not valid.";
                                    return Ok(response);
                                }
                            }
                            try
                            {
                                var userInfo = userLoginService.GetLoginDeatilByEmail(item.Email);
                                if (userInfo != null)
                                {
                                    JoiningModel userDetail = new JoiningModel();
                                    userDetail.Email = !string.IsNullOrEmpty(userInfo.EmailOffice) ? userInfo.EmailOffice : string.Empty;
                                    userDetail.JoinedDate = userInfo.JoinedDate?.ToString("yyyy-MM-dd");
                                    usersModel.Add(userDetail);
                                }
                                else
                                {
                                    JoiningModel userDetail = new JoiningModel();
                                    userDetail.Email = item.Email;
                                    userDetail.JoinedDate = "No record found for this email address.";
                                    usersModel.Add(userDetail);
                                }

                            }
                            catch (Exception ex)
                            {
                                response.Status = false;
                                response.Message = (ex.InnerException ?? ex).ToString();
                                return Ok(response);
                            }
                        }

                        response.Data = usersModel;
                        response.Status = true;

                    }
                    catch (Exception exep)
                    {
                        response.Status = false;
                        response.Message = (exep.InnerException ?? exep).ToString();
                        return Ok(response);
                    }
                }
                return Ok(response);
            }
            else
            {
                response.Status = false;
                response.Message = "Invalid authentication found";
                response.Code = HttpStatusCode.NetworkAuthenticationRequired;
                response.Errors = new string[] { "Invalid authentication found" };
                return Ok(response);
            }
        }

        [Route("~/User/LeaveBalance")]
        [HttpPost]
        //{"EmailOffice":"devendra.sunaria@dotsquares.com","CRMId":"0","EMSProjectID":"79",
        //"Hours":"5:00","Description":"Test","Date":"08/04/2020","Type":"Additional Free","EMSTimesheetId":"0"}
        public IActionResult GetUserLeaeveBalance(List<LeaveBalanceModel> model)
        {
            ResponseStatusModel<string> authResponse = new ResponseStatusModel<string>();
            ResponseModel<List<CurrentLeaveModel>> response = new ResponseModel<List<CurrentLeaveModel>>();
            authResponse = AuthorizeRequestStatus();
            if (authResponse.Status)
            {
                if (model.Count > 0)
                {
                    List<CurrentLeaveModel> responseUser = new List<CurrentLeaveModel>();
                    foreach (var userAttendance in model)
                    {
                        try
                        {
                            CurrentLeaveModel dto = new CurrentLeaveModel();
                            var userDetails = userLoginService.GetUserByAttendanceId(userAttendance.attendanceId);
                            if (userDetails != null)
                            {
                                var monthYearValue = (DateTime.Now.Year * 12) + DateTime.Now.Month;
                                DateTime now = DateTime.Now;
                                var startDate = new DateTime(now.Year, now.Month, 1);
                                var endDate = startDate.AddMonths(1).AddDays(-1);
                                int id = userDetails.Uid;

                                #region "Leave Balance Calculation"

                                DataTable leaveBalance = new DataTable();
                                if (userDetails.IsFromDbdt == false || userDetails.IsFromDbdt == null)
                                {
                                    leaveBalance = levDetailsService.GetLeaveBalance(userAttendance.attendanceId, monthYearValue);
                                }
                                else if (userDetails.IsFromDbdt == true)
                                {
                                    leaveBalance = levDetailsDTService.GetDTLeaveBalance(userAttendance.attendanceId, monthYearValue);
                                }
                                var leaveBalanceList = DtoBinder(leaveBalance);
                                LeaveTypesDto saralLeaveBalance = new LeaveTypesDto();
                                foreach (var item in leaveBalanceList)
                                {
                                    switch (item.LeaveName)
                                    {
                                        case "Loss Of Pay":
                                            saralLeaveBalance.LossPayLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.UnpaidLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
                                            saralLeaveBalance.LossPayLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.UnpaidLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
                                            break;
                                        case "CL":
                                            saralLeaveBalance.CasualLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.CasualLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
                                            saralLeaveBalance.CasualLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.CasualLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
                                            break;
                                        case "Compensatory Off":
                                            saralLeaveBalance.CompensatoryOff_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.CompensatoryOff.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
                                            saralLeaveBalance.CompensatoryOff_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.CompensatoryOff.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
                                            break;
                                        case "Paternity Leave":
                                            saralLeaveBalance.PaternityLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.PaternityLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
                                            saralLeaveBalance.PaternityLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.PaternityLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
                                            break;
                                        case "Sick saralLeaveBalance":
                                            saralLeaveBalance.SickLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.SickLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
                                            saralLeaveBalance.SickLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.SickLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
                                            break;
                                        case "Maternity Leave":
                                            saralLeaveBalance.MaternityLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.MaternityLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
                                            saralLeaveBalance.MaternityLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.MaternityLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
                                            break;
                                        case "Earned Leave":
                                            saralLeaveBalance.EarnedLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.EarnedLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
                                            saralLeaveBalance.EarnedLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.EarnedLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
                                            break;
                                        case "Bereavement Leave":
                                            saralLeaveBalance.BereavementLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.BereavementLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
                                            saralLeaveBalance.BereavementLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.BereavementLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
                                            break;
                                        case "Wedding Leave":
                                            saralLeaveBalance.WeddingLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.WeddingLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
                                            saralLeaveBalance.WeddingLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.WeddingLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
                                            break;

                                    }
                                }

                                ApprovedLeaveDto approvedLeaveDto = new ApprovedLeaveDto();
                                approvedLeaveDto.CasualLeave = leaveService.GetApprovedLeaves(id, (int)Enums.LeaveCategory.CasualLeave, startDate, endDate);
                                approvedLeaveDto.EarnedLeave = leaveService.GetApprovedLeaves(id, (int)Enums.LeaveCategory.EarnedLeave, startDate, endDate);
                                approvedLeaveDto.LossPayLeave = leaveService.GetApprovedLeaves(id, (int)Enums.LeaveCategory.UnpaidLeave, startDate, endDate);
                                approvedLeaveDto.SickLeave = leaveService.GetApprovedLeaves(id, (int)Enums.LeaveCategory.SickLeave, startDate, endDate);
                                approvedLeaveDto.BereavementLeave = leaveService.GetApprovedLeaves(id, (int)Enums.LeaveCategory.BereavementLeave, startDate, endDate);
                                approvedLeaveDto.WeddingLeave = leaveService.GetApprovedLeaves(id, (int)Enums.LeaveCategory.WeddingLeave, startDate, endDate);
                                approvedLeaveDto.CompensatoryOff = leaveService.GetApprovedLeaves(id, (int)Enums.LeaveCategory.CompensatoryOff, startDate, endDate);
                                //approvedLeaveDto.LoyaltyLeave = leaveService.GetApprovedLeaves(id,(int)Enums.LeaveCategory.LoyaltyLeave, startDate,endDate);
                                approvedLeaveDto.MaternityLeave = leaveService.GetApprovedLeaves(id, (int)Enums.LeaveCategory.MaternityLeave, startDate, endDate);
                                approvedLeaveDto.PaternityLeave = leaveService.GetApprovedLeaves(id, (int)Enums.LeaveCategory.PaternityLeave, startDate, endDate);

                                CurrentLeaveDto pendingLeave = new CurrentLeaveDto();
                                pendingLeave.CasualLeave = leaveService.GetPendingLeaves(id, (int)LeaveCategory.CasualLeave);
                                pendingLeave.EarnedLeave = leaveService.GetPendingLeaves(id, (int)LeaveCategory.EarnedLeave);
                                pendingLeave.LossPayLeave = leaveService.GetPendingLeaves(id, (int)LeaveCategory.UnpaidLeave);
                                pendingLeave.SickLeave = leaveService.GetPendingLeaves(id, (int)LeaveCategory.SickLeave);
                                pendingLeave.BereavementLeave = leaveService.GetPendingLeaves(id, (int)LeaveCategory.BereavementLeave);
                                pendingLeave.WeddingLeave = leaveService.GetPendingLeaves(id, (int)LeaveCategory.WeddingLeave);
                                pendingLeave.CompensatoryOff = leaveService.GetPendingLeaves(id, (int)LeaveCategory.CompensatoryOff);
                                //pendingLeave.LoyaltyLeave = leaveService.GetPendingLeaves(id, (int)LeaveCategory.LoyaltyLeave);
                                pendingLeave.MaternityLeave = leaveService.GetPendingLeaves(id, (int)LeaveCategory.MaternityLeave);
                                pendingLeave.PaternityLeave = leaveService.GetPendingLeaves(id, (int)LeaveCategory.PaternityLeave);

                                ApprovedLeaveDto getCurrentMonthLeave = new ApprovedLeaveDto();
                                getCurrentMonthLeave.CasualLeave = Convert.ToDouble(approvedLeaveDto.CasualLeave) + pendingLeave.CasualLeave;
                                getCurrentMonthLeave.LossPayLeave = Convert.ToDouble(approvedLeaveDto.LossPayLeave) + pendingLeave.LossPayLeave;
                                getCurrentMonthLeave.CompensatoryOff = Convert.ToDouble(approvedLeaveDto.CompensatoryOff) + pendingLeave.CompensatoryOff;
                                getCurrentMonthLeave.PaternityLeave = Convert.ToDouble(approvedLeaveDto.PaternityLeave) + pendingLeave.PaternityLeave;
                                getCurrentMonthLeave.SickLeave = Convert.ToDouble(approvedLeaveDto.SickLeave) + pendingLeave.SickLeave;
                                getCurrentMonthLeave.MaternityLeave = Convert.ToDouble(approvedLeaveDto.MaternityLeave) + pendingLeave.MaternityLeave;
                                getCurrentMonthLeave.EarnedLeave = Convert.ToDouble(approvedLeaveDto.EarnedLeave) + pendingLeave.EarnedLeave;
                                getCurrentMonthLeave.BereavementLeave = Convert.ToDouble(approvedLeaveDto.BereavementLeave) + pendingLeave.BereavementLeave;
                                getCurrentMonthLeave.WeddingLeave = Convert.ToDouble(approvedLeaveDto.WeddingLeave) + pendingLeave.WeddingLeave;

                                CurrentLeaveDto currentLeaveBalance = new CurrentLeaveDto();
                                currentLeaveBalance.CasualLeave = Convert.ToDouble(saralLeaveBalance.CasualLeave_CB) - getCurrentMonthLeave.CasualLeave;
                                currentLeaveBalance.LossPayLeave = Convert.ToDouble(saralLeaveBalance.LossPayLeave_CB) - getCurrentMonthLeave.LossPayLeave;
                                currentLeaveBalance.CompensatoryOff = Convert.ToDouble(saralLeaveBalance.CompensatoryOff_CB) - getCurrentMonthLeave.CompensatoryOff;
                                currentLeaveBalance.PaternityLeave = Convert.ToDouble(saralLeaveBalance.PaternityLeave_CB) - getCurrentMonthLeave.PaternityLeave;
                                currentLeaveBalance.SickLeave = Convert.ToDouble(saralLeaveBalance.SickLeave_CB) - getCurrentMonthLeave.SickLeave;
                                currentLeaveBalance.MaternityLeave = Convert.ToDouble(saralLeaveBalance.MaternityLeave_CB) - getCurrentMonthLeave.MaternityLeave;
                                currentLeaveBalance.EarnedLeave = Convert.ToDouble(saralLeaveBalance.EarnedLeave_CB) - getCurrentMonthLeave.EarnedLeave;
                                currentLeaveBalance.BereavementLeave = Convert.ToDouble(saralLeaveBalance.BereavementLeave_CB) - getCurrentMonthLeave.BereavementLeave;
                                currentLeaveBalance.WeddingLeave = Convert.ToDouble(saralLeaveBalance.WeddingLeave_CB) - getCurrentMonthLeave.WeddingLeave;
                                #endregion


                                //var leaveData = GetCurrentLeaveBalance(GetAllLeaveBalance(uid), GetLeave(uid));
                                var leaveData = currentLeaveBalance;
                                dto.CasualLeave = leaveData.CasualLeave;
                                dto.LossPayLeave = leaveData.LossPayLeave;
                                dto.CompensatoryOff = leaveData.CompensatoryOff;
                                dto.PaternityLeave = leaveData.PaternityLeave;
                                dto.SickLeave = leaveData.SickLeave;
                                dto.MaternityLeave = leaveData.MaternityLeave;
                                dto.EarnedLeave = leaveData.EarnedLeave;
                                dto.BereavementLeave = leaveData.BereavementLeave;
                                dto.WeddingLeave = leaveData.WeddingLeave;
                                dto.AttendanceId = userAttendance.attendanceId > 0 ? userAttendance.attendanceId : 0;
                                dto.Email = userDetails.EmailOffice;

                                response.Status = true;
                                response.Message = "Success";
                                responseUser.Add(dto);
                            }
                            else
                            {
                                response.Status = false;
                                response.Message = "Invalid Attendance Id";
                                dto.AttendanceId = userAttendance.attendanceId > 0 ? userAttendance.attendanceId : 0;
                                dto.Email = "";
                                responseUser.Add(dto);
                            }

                        }

                        catch (Exception exep)
                        {
                            response.Status = false;
                            response.Message = (exep.InnerException ?? exep).ToString();
                        }
                    }
                    response.Data = responseUser;
                    response.Status = true;
                }
                return Ok(response);
            }
            else
            {
                response.Status = false;
                response.Message = "Invalid authentication found";
                response.Code = HttpStatusCode.NetworkAuthenticationRequired;
                response.Errors = new string[] { "Invalid authentication found" };
                return Ok(response);
            }
        }

        public double GetWorkingDays(DateTime? dateFrom, DateTime? dateTo)
        {
            DateTime startdate = (DateTime)dateFrom;
            DateTime enddate = (DateTime)dateTo.Value.AddDays(1);
            double daycount = 0;
            while (startdate < enddate)
            {
                int DayNumInWeek = (int)startdate.DayOfWeek;
                if (DayNumInWeek != 0)
                {
                    if (DayNumInWeek != 6)
                    { daycount += 1; }
                }
                startdate = startdate.AddDays(1);
            }
            return daycount;
        }
        #region Leave Balance Calculation

        public List<LeaveBalanceDetailsDto> DtoBinder(System.Data.DataTable data)
        {
            List<LeaveBalanceDetailsDto> leaveBalancelist = new List<LeaveBalanceDetailsDto>();
            if (data.Rows.Count > 0)
            {
                foreach (DataRow dr in data.Rows)
                {
                    leaveBalancelist.Add(new LeaveBalanceDetailsDto
                    {
                        EmpId = Convert.ToInt32(dr["EMPID"]),
                        EmpName = dr["EMPNAME"].ToString(),
                        LeaveName = dr["LEVNAME"].ToString(),
                        OpeningBalance = Convert.ToDecimal(dr["OPENING_BALANCE"]),
                        Allotted = Convert.ToDecimal(dr["ALLOTED"]),
                        Lapsed = Convert.ToDecimal(dr["LAPSE"]),
                        EnchaseDays = Convert.ToDecimal(dr["ENCHASEDAYS"]),
                        LeaveAvailed = Convert.ToDecimal(dr["LEAVEAVAILED"]),
                        ClosingBalance = Convert.ToDecimal(dr["CB"])
                    });
                }
            }
            return leaveBalancelist;
        }
        #endregion

        [Route("~/User/UserLogin")]
        [HttpPost]
        public IActionResult UserLogin([FromBody] LoginDto model)
        {
            ResponseStatusModel<string> authResponse = new ResponseStatusModel<string>();
            ResponseModel<LoginUserResponseModel> response = new ResponseModel<LoginUserResponseModel>();
            authResponse = AuthorizeRequestStatus();
            if (authResponse.Status)
            {
                try
                {
                    var userDetail = userLoginService.GetLoginDeatilByUserNameOREmail(model.Email);
                    if (userDetail != null)
                    {
                        if (userDetail.IsActive == false)
                        {
                            response.Status = false;
                            response.Message = "Your account is temporarily  deactivated. Please contact to admin.";
                            return Ok(response);
                        }

                        var keybytes = GetRiJndael_KeyBytes(SiteKeys.Encryption_Key);
                        byte[] encrypted = EncryptStringToBytes(model.Password, keybytes, keybytes);

                        if ((userDetail.PasswordKey == Convert.ToBase64String(encrypted))) //string.IsNullOrEmpty(userDetail.PasswordKey) && (userDetail.Password == source) || 
                        {
                            UserActivity userActivity = userActivityService.GetUserActivityByUidDesc(userDetail.Uid);
                            UserLogin teamManagerDetail = new UserLogin();
                            string TMName = string.Empty;
                            string TMEmail = string.Empty;
                            if (userDetail.PMUid.HasValue)
                            {
                                teamManagerDetail = userLoginService.GetUsersById(userDetail.PMUid.Value);
                                TMName = teamManagerDetail != null ? teamManagerDetail.Name : "";
                                TMEmail = teamManagerDetail != null ? teamManagerDetail.EmailOffice : "";
                            }

                            string LastLoginDate = "", LastLoginTime = "";
                            if (userActivity != null)
                            {
                                LastLoginDate = userActivity.DateModify.ToFormatDateString("dd/MM/yyyy");
                                LastLoginTime = userActivity.DateModify.ToFormatDateString("hh:mm tt");
                            }
                            var data = new LoginUserResponseModel
                            {
                                Name = userDetail.Name,
                                Email = userDetail.EmailOffice,
                                TeamManager = TMName,
                                TeamManagerEmail = TMEmail,
                                LastLoginDate = LastLoginDate,
                                LastLoginTime = LastLoginTime
                            };
                            response.Status = true;
                            response.Message = "OK";
                            response.Data = data;
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = "Incorrect UserName or Password.";
                        }
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "User does not exist.";
                    }
                }
                catch (Exception ex)
                {
                    response.Status = false;
                    response.Message = "Error";
                }
            }
            else
            {
                response.Status = false;
                response.Message = "Invalid authentication found";
                response.Code = HttpStatusCode.NetworkAuthenticationRequired;
                response.Errors = new string[] { "Invalid authentication found" };
                return Ok(response);
            }


            return Ok(response);
        }
        [Route("~/User/ActiveUserList")]
        [HttpPost]
        public IActionResult ActiveUserList()
        {
            ResponseStatusModel<string> authResponse = new ResponseStatusModel<string>();
            ResponseModel<List<ActiveUserResponseModel>> response = new ResponseModel<List<ActiveUserResponseModel>>();
            authResponse = AuthorizeRequestStatus();
            if (authResponse.Status)
            {
                try
                {
                    var userDetail = userLoginService.GetAllUsers().OrderBy(x => x.Name);
                    if (userDetail != null)
                    {
                        List<ActiveUserResponseModel> data = new List<ActiveUserResponseModel>();
                        foreach (var item in userDetail)
                        {
                            ActiveUserResponseModel obj = new ActiveUserResponseModel();
                            obj.Uid = item.Uid;
                            obj.Name = item.Name;
                            obj.Title = item.Title;
                            obj.JobTitle = item.JobTitle;
                            obj.Department = item.Department.Name;
                            obj.Role = item.Role.RoleName;
                            obj.TL = userLoginService.GetNameById(Convert.ToInt32(item.TLId));
                            obj.PM = userLoginService.GetNameById(Convert.ToInt32(item.PMUid));
                            obj.DOB = item.DOB.ToFormatDateString("dd,MMM, yyyy");
                            obj.JoinedDate = item.JoinedDate.ToFormatDateString("dd,MMM, yyyy");
                            obj.EmailOffice = item.EmailOffice;
                            obj.MobileNumber = item.MobileNumber;
                            obj.PhoneNumber = item.PhoneNumber;
                            obj.Address = item.Address;
                            obj.SkypeId = item.SkypeId;
                            obj.MarraigeDate = item.MarraigeDate.ToFormatDateString("dd,MMM, yyyy");
                            obj.Gender = item.Gender;
                            obj.ProfilePicture = item.ProfilePicture;
                            obj.HRMId = item.HRMId;
                            obj.AttendenceId = item.AttendenceId;
                            obj.EmpCode = item.EmpCode;
                            data.Add(obj);
                        }
                        response.Status = true;
                        response.Message = "OK";
                        response.Data = data;
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "User does not exist.";
                    }
                }
                catch (Exception ex)
                {
                    response.Status = false;
                    response.Message = "Error";
                }
            }
            else
            {
                response.Status = false;
                response.Message = "Invalid authentication found";
                response.Code = HttpStatusCode.NetworkAuthenticationRequired;
                response.Errors = new string[] { "Invalid authentication found" };
                return Ok(response);
            }


            return Ok(response);
        }

        private void LogPrintEndingMessage(string filePath, string message)
        {
            string endLine = "-----------------------------------------------------------End----------------------------------------------------------------";
            LogPrint.WriteIntoFile(filePath, message);
            LogPrint.WriteIntoFile(filePath, endLine);
            LogPrint.WriteIntoFile(filePath, string.Empty);
        }
    }
}