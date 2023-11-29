using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Service.LibraryManagement;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.Models.Others;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static EMS.Core.Enums;
using System.Text.RegularExpressions;
using EMS.Web.Modals;
using System.IO.Compression;
using System.Net;
using EMS.Dto.LibraryManagement;

namespace EMS.Web.Controllers
{
    //[CustomAuthorization(IsAshishTeam: true)]
    public class LibraryManagementController : BaseController
    {
        #region "Variables and constructor"
        private readonly ILibraryManagementService libraryManagementService;
        private readonly ILibraryLayoutService libraryLayoutService;
        private readonly IDomainTypeService domainTypeService;
        private readonly ITechnologyService technologyService;
        private readonly ILibrarySearchService librarySearchService;
        private readonly ILibraryFileTypeService libraryFileTypeService;
        private readonly ILibraryFileService libraryFileService;
        private readonly ILibraryComponentFileService libraryComponentFileService;
        private readonly IUserLoginService userLoginService;
        private readonly ILibraryDownloadService libraryDownloadService;
        private readonly ILibraryDownloadHistoryService libraryDownloadHistoryService;
        private readonly ILibraryComponentTypeService libraryComponentTypeService;
        private readonly ITechnologyParentService technologyParentService;
        private readonly ITechnologyParentMappingService technologyParentMappingService;
        private readonly ISerialNumberService serialNumberService;
        private readonly ILibraryTemplateTypeService libraryTemplateTypeService;
        private readonly ISalesKitTypeService salesKitTypeService;
        private readonly ICvsTypeService cvsTypeService;
        private readonly int PageDataLength = 12;
        private static int pageNo = 0;
        private readonly string localDirectory = SiteKey.LocalDirectory;
        //private readonly string localDirectory = "D:\\ComponentFiles\\";
        private static LibraryManagementSearchFilterDto _libraryManagementSearchFilter = new LibraryManagementSearchFilterDto();
        private List<string> imageExtensions = new List<string> { ".jpg", ".png", ".jpeg", ".gif" };
        public bool IsLibraryImportLogCreated { get; set; }
        public int failedTotalRecords { get; set; }
        public LibraryManagementController(ISerialNumberService _serialNumberService, ITechnologyParentMappingService _technologyParentMappingService, ITechnologyParentService _technologyParentService, IHostingEnvironment hostingEnvironment, ILibraryComponentTypeService _libraryComponentTypeService, ILibraryFileService _libraryFileService, ILibraryFileTypeService _libraryFileTypeService, ILibraryManagementService _libraryManagementService, IDomainTypeService _domainTypeService,
            ILibraryLayoutService _libraryLayoutService, ITechnologyService _technologyService, ILibrarySearchService _librarySearchService,
            ILibraryDownloadService _libraryDownloadService, IUserLoginService _userLoginService,
            ILibraryDownloadHistoryService _libraryDownloadHistoryService, ILibraryTemplateTypeService _libraryTemplateTypeService, ISalesKitTypeService _salesKitTypeService, ICvsTypeService _cvsTypeService, ILibraryComponentFileService _libraryComponentFileService
            )
        {
            serialNumberService = _serialNumberService;
            technologyParentMappingService = _technologyParentMappingService;
            technologyParentService = _technologyParentService;
            libraryComponentTypeService = _libraryComponentTypeService;
            libraryManagementService = _libraryManagementService;
            libraryLayoutService = _libraryLayoutService;
            domainTypeService = _domainTypeService;
            technologyService = _technologyService;
            librarySearchService = _librarySearchService;
            libraryFileTypeService = _libraryFileTypeService;
            libraryFileService = _libraryFileService;
            libraryComponentFileService = _libraryComponentFileService;
            userLoginService = _userLoginService;
            libraryDownloadService = _libraryDownloadService;
            libraryDownloadHistoryService = _libraryDownloadHistoryService;
            libraryTemplateTypeService = _libraryTemplateTypeService;
            salesKitTypeService = _salesKitTypeService;
            cvsTypeService = _cvsTypeService;
        }
        #endregion

        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult Index(Guid? id)
        {
            //if (!HasDownloadPermission())
            //{
            //    return AccessDenied();
            //}
            LibraryManagementIndexDto libraryManagementIndexDto = new LibraryManagementIndexDto();
            try
            {
                if (id != null && id != Guid.Empty)
                {
                    pageNo = 0;
                    LibraryManagementDto libraryManagementDto = GetLibraryManagementDto((Guid)id);
                    libraryManagementIndexDto.libraryManagementDto = libraryManagementDto;
                    if (libraryManagementDto.libraryManagementSearchFilterDto != null && libraryManagementDto.libraryManagementSearchFilterDto.LibrarySearchExist)
                    {
                        FillSelectedItemsList(libraryManagementDto, libraryManagementDto.libraryManagementSearchFilterDto);
                        libraryManagementDto.LibraryType = libraryManagementDto.libraryManagementSearchFilterDto.LibraryType.ToString();
                        //KeyId is there in case it is individual copy
                        if (libraryManagementDto.libraryManagementSearchFilterDto.KeyId.HasValue && libraryManagementDto.libraryManagementSearchFilterDto.KeyId != Guid.Empty)
                        {
                            //Fill control default
                            FillControlsDefault(libraryManagementIndexDto);
                            libraryManagementIndexDto.KeyId = libraryManagementDto.libraryManagementSearchFilterDto.KeyId;
                        }
                        else  // Generalized copy(search)
                        {
                            libraryManagementIndexDto.LibraryTypes = WebExtensions.GetSelectList<Enums.LibraryType>();
                            //Finding selected item
                            var itemSelected = libraryManagementIndexDto.LibraryTypes.
                                FirstOrDefault(d => d.Value.Equals(((byte)libraryManagementDto.libraryManagementSearchFilterDto.LibraryType).ToString()));
                            if (itemSelected != null)
                            {
                                itemSelected.Selected = true;
                            }

                            libraryManagementIndexDto.DesignTypes = WebExtensions.GetSelectList<Enums.DesignType>();
                            if (libraryManagementDto.libraryManagementSearchFilterDto.DesignType.HasValue)
                            {
                                //Finding selected item
                                var item = libraryManagementIndexDto.DesignTypes.
                                    FirstOrDefault(d => d.Value.Equals(((byte)libraryManagementDto.libraryManagementSearchFilterDto.DesignType).ToString()));
                                if (item != null)
                                {
                                    item.Selected = true;
                                }
                            }
                            libraryManagementIndexDto.AdvanceSearch = libraryManagementDto.libraryManagementSearchFilterDto.IsAdvanceSearch;
                            libraryManagementIndexDto.Keyword = libraryManagementDto.libraryManagementSearchFilterDto.SearchText;

                            libraryManagementIndexDto.Technologies = technologyService.GetTechnologyList().OrderBy(t => t.Title).
                            Select(d => new SelectListItem
                            {
                                Text = d.Title,
                                Value = d.TechId.ToString(),
                                Selected = libraryManagementDto.libraryManagementSearchFilterDto.Technologies.Contains(d.TechId)
                            }).ToList();

                            libraryManagementIndexDto.Industries = domainTypeService.GetDomainList().OrderBy(i => i.DomainName).
                            Select(d => new SelectListItem
                            {
                                Text = d.DomainName,
                                Value = d.DomainId.ToString(),
                                Selected = libraryManagementDto.libraryManagementSearchFilterDto.Domains.Contains(d.DomainId)
                            }).ToList();

                            libraryManagementIndexDto.Layouts = libraryLayoutService.GetLibraryLayouts().OrderBy(l => l.Name).
                            Select(d => new SelectListItem
                            {
                                Text = d.Name,
                                Value = d.Id.ToString(),
                                Selected = libraryManagementDto.libraryManagementSearchFilterDto.Layouts.Contains(d.Id)
                            }).ToList();
                            libraryManagementIndexDto.Components = libraryComponentTypeService.GetLibraryComponentTypes().
                            OrderBy(c => c.Name).Select(d => new SelectListItem
                            {
                                Text = d.Name,
                                Value = d.Id.ToString(),
                                Selected = libraryManagementDto.libraryManagementSearchFilterDto.Components.Contains(d.Id)
                            }).ToList();
                            //libraryManagementIndexDto.Templates = libraryTemplateTypeService.GetLibraryTemplateTypes().
                            //OrderBy(c => c.Name).Select(d => new SelectListItem
                            //{
                            //    Text = d.Name,
                            //    Value = d.Id.ToString(),
                            //    Selected = libraryManagementDto.libraryManagementSearchFilterDto.Templates.Contains(d.Id)
                            //}).ToList();
                            if (libraryManagementDto.libraryManagementSearchFilterDto.Featured.HasValue)
                            {
                                libraryManagementIndexDto.Featured = libraryManagementDto.libraryManagementSearchFilterDto.Featured.Value ? "1" : "0";
                            }
                            else
                            {
                                libraryManagementIndexDto.Featured = null;
                            }
                            if (libraryManagementDto.libraryManagementSearchFilterDto.IsNDA.HasValue)
                            {
                                libraryManagementIndexDto.IsNda = libraryManagementDto.libraryManagementSearchFilterDto.IsNDA.Value ? "1" : "0";
                            }
                            else
                            {
                                libraryManagementIndexDto.IsNda = null;
                            }
                            if (libraryManagementDto.libraryManagementSearchFilterDto.IsReadyToUse != null)
                            {
                                libraryManagementIndexDto.IsReadyToUse = libraryManagementDto.libraryManagementSearchFilterDto.IsReadyToUse.Value ? "1" : "0";
                            }
                            else
                            {
                                libraryManagementIndexDto.IsReadyToUse = null;
                            }
                        }
                    }
                    else // LibrarySearch doesn't exist
                    {
                        FillControlsDefault(libraryManagementIndexDto);
                    }
                }
                else
                {
                    FillControlsDefault(libraryManagementIndexDto);
                }
            }
            catch (Exception ex) { }
            return View("Index", libraryManagementIndexDto);
        }

        [HttpPost]
        [CustomActionAuthorization()]
        public IActionResult SearchLibrary(LibraryManagementSearchFilterDto libraryManagementSearchFilter)
        {
            //if (!HasDownloadPermission())
            //{
            //    return AccessDenied();
            //}
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
                            || libraryManagementSearchFilter.LibraryType == Enums.LibraryType.Select
                            || libraryManagementSearchFilter.LibraryType == Enums.LibraryType.CVs
                            || libraryManagementSearchFilter.LibraryType == Enums.LibraryType.SalesKit)
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

                        libraryDto.LibraryComponentFileTypes = GetLibraryComponentFileType(library.LibraryComponentFile);
                        if (_libraryManagementSearchFilter.LibraryType == Enums.LibraryType.Component)
                        {
                            if (library.LibraryComponentFile != null)
                            {
                                foreach (var libraryFile in library.LibraryComponentFile)
                                {
                                    LibraryComponentFileDto libraryFileDto = new LibraryComponentFileDto();
                                    libraryFileDto.FileName = Path.GetFileName(libraryFile.FilePath);
                                    libraryFileDto.Id = libraryFile.Id;
                                    libraryFileDto.LibraryId = libraryFile.LibraryId;
                                    libraryFileDto.FilePath = libraryFile.FilePath;
                                    //libraryFileDto.FileImage = Path.GetExtension(libraryFile.FilePath).ToLower() == ".zip" ? "zip-link" : "rar-link";
                                    libraryFileDto.FileImage = libraryFile.LibraryFileType.Icon;
                                    libraryFileDto.FileTypeName = libraryFile.LibraryFileType.Name;
                                    libraryFileDto.PSDFilePath = libraryFile.PsdfilePath;

                                    //libraryDto.LibraryFileDtos.Add(libraryFileDto);
                                    libraryDto.LibraryComponentFileDtos.Add(libraryFileDto);

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

                return PartialView("_AdvanceSearch", libraryManagementDto);
            }
            catch (Exception ex)
            {
                return CreateModelStateErrors();
            }
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult LoadSearchLibrary()
        {
            //if (!HasDownloadPermission())
            //{
            //    return AccessDenied();
            //}
            StringBuilder html = new StringBuilder();
            int totalCount = 0;


            ////set paging (testing)
            //pageNo = pageNo - 1;

            _libraryManagementSearchFilter.PageStart = pageNo * PageDataLength;
            _libraryManagementSearchFilter.DataLength = PageDataLength;
            var expr = GetLibraryFilterExpression(_libraryManagementSearchFilter);
            var libraries = libraryManagementService.GetLibraries(out totalCount, expr);
            LibraryManagementDto libraryManagementDto = new LibraryManagementDto();
            pageNo++;

            if (libraries != null && libraries.Count > 0)
            {
                foreach (var library in libraries)
                {
                    LibraryDto libraryDto = new LibraryDto();
                    libraryDto.Id = library.Id;
                    libraryDto.KeyId = library.KeyId;
                    libraryDto.LibraryTypeId = library.LibraryTypeId;
                    libraryDto.Title = library.Title.Trim().ToTitleCase();
                    libraryDto.Description = !string.IsNullOrWhiteSpace(library.Description) ? library.Description.Trim().ToTitleCase() : null;
                    libraryDto.BannerImage = library.BannerImage;
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
                    //libraryDto.LibraryFileTypes = libraryDto.LibraryFileTypes = library.LibraryFile.GroupBy(lf => lf.LibraryFileType.Id).
                    //    Select(g => g.FirstOrDefault().LibraryFileType.Name).ToList();

                    //libraryDto.LibraryFileTypes = libraryDto.LibraryFileTypes = library.LibraryFile.GroupBy(lf => lf.LibraryFileType.Id).
                    //    Select(g => g.FirstOrDefault().LibraryFileType.Icon).ToList();

                    libraryDto.LibraryFileTypes = GetLibraryFileType(library.LibraryFile);
                    if (_libraryManagementSearchFilter.LibraryType == Enums.LibraryType.Component
                        || _libraryManagementSearchFilter.LibraryType == Enums.LibraryType.Document
                        || _libraryManagementSearchFilter.LibraryType == Enums.LibraryType.Design
                        || _libraryManagementSearchFilter.LibraryType == Enums.LibraryType.Template)
                    {
                        if (library.LibraryFile != null)
                        {
                            foreach (var libraryFile in library.LibraryFile)
                            {
                                LibraryFileDto libraryFileDto = new LibraryFileDto();
                                libraryFileDto.FileName = Path.GetFileName(libraryFile.FilePath);
                                libraryFileDto.Id = libraryFile.Id;
                                libraryFileDto.LibraryId = libraryFile.LibraryId;
                                libraryFileDto.FilePath = libraryFile.FilePath;
                                //libraryFileDto.FileImage = Path.GetExtension(libraryFile.FilePath).ToLower() == ".zip" ? "zip-link" : "rar-link";
                                libraryFileDto.FileImage = libraryFile.LibraryFileType.Icon;
                                libraryFileDto.FileTypeName = libraryFile.LibraryFileType.Name;
                                libraryFileDto.PSDFilePath = libraryFile.PsdfilePath;
                                libraryFileDto.LibraryLayoutTypeId = libraryFile.LibraryLayoutTypeId;
                                libraryFileDto.LibraryLayoutType = libraryFile.LibraryLayoutType;

                                //libraryDto.LibraryFileDtos.Add(libraryFileDto);
                                libraryDto.LibraryFileDtos.Add(libraryFileDto);

                            }
                        }
                    }

                    libraryDto.LibraryComponentFileTypes = GetLibraryComponentFileType(library.LibraryComponentFile);
                    if (_libraryManagementSearchFilter.LibraryType == Enums.LibraryType.Component)
                    {
                        if (library.LibraryComponentFile != null)
                        {
                            foreach (var libraryFile in library.LibraryComponentFile)
                            {
                                LibraryComponentFileDto libraryFileDto = new LibraryComponentFileDto();
                                libraryFileDto.FileName = Path.GetFileName(libraryFile.FilePath);
                                libraryFileDto.Id = libraryFile.Id;
                                libraryFileDto.LibraryId = libraryFile.LibraryId;
                                libraryFileDto.FilePath = libraryFile.FilePath;
                                //libraryFileDto.FileImage = Path.GetExtension(libraryFile.FilePath).ToLower() == ".zip" ? "zip-link" : "rar-link";
                                libraryFileDto.FileImage = libraryFile.LibraryFileType.Icon;
                                libraryFileDto.FileTypeName = libraryFile.LibraryFileType.Name;
                                libraryFileDto.PSDFilePath = libraryFile.PsdfilePath;

                                //libraryDto.LibraryFileDtos.Add(libraryFileDto);
                                libraryDto.LibraryComponentFileDtos.Add(libraryFileDto);

                            }
                        }
                    }
                    libraryManagementDto.libraries.Add(libraryDto);
                }
            }

            libraryManagementDto.LibraryType = _libraryManagementSearchFilter.LibraryType.ToString();
            libraryManagementDto.LibraryFileIds = (_libraryManagementSearchFilter.Layouts != null)
                ? _libraryManagementSearchFilter.Layouts.OfType<int>().ToList() : null;
            //if (_libraryManagementSearchFilter.LibraryType == Enums.LibraryType.Website ||
            //    _libraryManagementSearchFilter.LibraryType == Enums.LibraryType.MobileApp || _libraryManagementSearchFilter.LibraryType == Enums.LibraryType.Design)
            //{
            return PartialView("_SingleSearchDesign", libraryManagementDto);
            //}
            //else
            //{
            //    return PartialView("_SingleSearchDesignComponent", libraryManagementDto);
            //}
        }

        private PagingService<Library> GetLibraryFilterExpression(LibraryManagementSearchFilterDto libraryManagementSearchFilter)
        {
            var pagingServices = new PagingService<Library>(libraryManagementSearchFilter.PageStart, libraryManagementSearchFilter.DataLength);
            var expr = PredicateBuilder.True<Library>();
            expr = expr.And(x => x.IsDeleted != true && x.IsActive == true && x.IsApproved == true);
            if (libraryManagementSearchFilter.KeyId != null && libraryManagementSearchFilter.KeyId != Guid.Empty)
            {
                expr = expr.And(l => l.KeyId == libraryManagementSearchFilter.KeyId);
            }
            else
            {
                expr = expr.And(l => l.LibraryFile != null);
                if ((byte)libraryManagementSearchFilter.LibraryType != 0)
                {
                    expr = expr.And(l => (l.LibraryTypeId == (byte)libraryManagementSearchFilter.LibraryType));
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

                if (libraryManagementSearchFilter.LibraryType == Enums.LibraryType.SalesKit)
                {
                    if (libraryManagementSearchFilter.SalesKitTypeId.HasValue && libraryManagementSearchFilter.SalesKitTypeId.Value > 0)
                    {
                        expr = expr.And(l => l.SalesKitId == libraryManagementSearchFilter.SalesKitTypeId);
                    }
                }
                if (libraryManagementSearchFilter.LibraryType == Enums.LibraryType.CVs)
                {
                    if (libraryManagementSearchFilter.CvsTypeId.HasValue && libraryManagementSearchFilter.CvsTypeId.Value > 0)
                    {
                        expr = expr.And(l => l.CvsId == libraryManagementSearchFilter.CvsTypeId);
                    }
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

        [CustomActionAuthorization()]
        [HttpGet]
        public IActionResult Details(Guid KeyId, long? LibraryFileId = null)
        {
            //if (!HasDownloadPermission())
            //{
            //    return AccessDenied();
            //}
            Library library = libraryManagementService.GetLibraryByGUID(KeyId);
            LibraryDto libraryDto = new LibraryDto();
            if (library == null)
            {
                return MessagePartialView("Invalid library Id");
            }
            else
            {
                libraryDto.KeyId = library.KeyId;
                libraryDto.AddedBy = library.AddedBy;

                libraryDto.Title = library.Title.Trim().ToTitleCase();
                if (LibraryFileId != null)
                {
                    libraryDto.BannerImage = libraryFileService.GetLibraryFile((long)LibraryFileId).FilePath;
                }
                else
                {
                    libraryDto.BannerImage = library.BannerImage;
                }

                libraryDto.LibraryTypeId = library.LibraryTypeId;
                //libraryDto.IP = library.Ip;
                libraryDto.ModifyDate = library.ModifyDate?.ToString("dd MMM yyyy");
                libraryDto.keywords = library.SearchKeyword;
                libraryDto.Description = library.Description;
                libraryDto.Version = library.Version;
                libraryDto.IntegrationHours = library.IntegrationHours;
                libraryDto.IsReadyToUse = library.IsReadyToUse;
                libraryDto.BAName = library.UidBaNavigation?.Name;
                libraryDto.TLName = library.UidTlNavigation?.Name;

                if (library.LibraryTechnology.Count > 0)
                {
                    libraryDto.LibraryTechnologiesComma = String.Join(", ", library.LibraryTechnology.Select(x => x.Technology.Title).ToArray());
                }
                if (library.LibraryIndustry.Count > 0)
                {
                    libraryDto.LibraryIndustriesComma = String.Join(", ", library.LibraryIndustry.Select(x => x.Industry.DomainName).ToArray());
                }
                libraryDto.CreatedDate = library.LibraryCreatedDate?.ToString("dd MMM yyyy");
                var author = library.AuthorUid.HasValue ? library.AuthorU : null;
                string department = author != null ? author.DeptId.HasValue ? author.Department.Name : string.Empty : string.Empty;
                libraryDto.Author = $"{(author != null ? author.Name : string.Empty)}{(!string.IsNullOrEmpty(department) ? " (" + department + ")" : string.Empty)}";

                if (library.LibraryFile != null)
                {
                    string[] imageExtensions = null;
                    var libraryFileType = libraryFileTypeService.GetImageType();
                    imageExtensions = libraryFileTypeService.GetImageType() != null ? libraryFileType.Extension.Split(',') : new string[0];

                    var libraryFiles = library.LibraryFile;
                    if (LibraryFileId != null)
                    {
                        libraryFiles = libraryFiles.Where(x => x.Id == LibraryFileId).ToList();
                    }

                    foreach (var libraryFile in libraryFiles)
                    {
                        LibraryFileDto libraryFileDto = new LibraryFileDto();
                        libraryFileDto.FileName = Path.GetFileName(libraryFile.FilePath);
                        libraryFileDto.FilePath = libraryFile.FilePath;
                        libraryFileDto.Id = libraryFile.Id;
                        libraryFileDto.LibraryId = libraryFile.Id;
                        libraryFileDto.PSDFilePath = libraryFile.PsdfilePath;
                        libraryFileDto.DesignUnitType = libraryFile.DesignUnitType ?? false;
                        //checking whether file extension matches with image extension
                        if (imageExtensions.Contains(Path.GetExtension(libraryFile.FilePath).ToLower()))
                        {
                            libraryDto.ImageFiles.Add(libraryFile.FilePath);
                        }


                        //add psd file path in a list
                        libraryDto.PSDFiles.Add(libraryFile.PsdfilePath ?? "");


                        if (library.LibraryTypeId == (byte)Enums.LibraryType.Design)
                        {
                            libraryDto.LibraryFileDtos = libraryFiles?.
                            Select(x => new LibraryFileDto
                            {
                                Id = x.Id,
                                FileImage = x.LibraryFileType.Icon,
                                FilePath = x.FilePath,
                                FileName = Path.GetFileName(x.FilePath),
                                FileShortName = GetShortFileName(Path.GetFileName(x.FilePath), 33),
                                PSDFilePath = x.PsdfilePath,
                                PsdFileShortName = GetShortFileName(Path.GetFileName(x.PsdfilePath), 33),
                                LibraryLayoutType = x.LibraryLayoutType,
                                LibraryLayoutTypeId = x.LibraryLayoutTypeId,
                                DesignUnitType = x.DesignUnitType ?? false
                            }).ToList();
                        }
                        else
                        {
                            libraryDto.LibraryFileDtos = library.LibraryFile?.
                            Select(x => new LibraryFileDto
                            {
                                Id = x.Id,
                                FileImage = x.LibraryFileType.Icon,
                                FilePath = x.FilePath,
                                FileName = Path.GetFileName(x.FilePath),
                                FileShortName = GetShortFileName(Path.GetFileName(x.FilePath), 33),
                                PSDFilePath = x.PsdfilePath,
                                PsdFileShortName = GetShortFileName(Path.GetFileName(x.PsdfilePath), 33),
                                DesignUnitType = x.DesignUnitType ?? false
                            }).ToList();
                        }
                    }
                }
            }
            return PartialView("Details", libraryDto);
        }

        [CustomActionAuthorization()]
        [HttpGet]
        public IActionResult Design_Details(Guid KeyId, long? LibraryFileId = null)
        {
            //if (!HasDownloadPermission())
            //{
            //    return AccessDenied();
            //}
            Library library = libraryManagementService.GetLibraryByGUID(KeyId);
            LibraryDto libraryDto = new LibraryDto();
            if (library == null)
            {
                return MessagePartialView("Invalid library Id");
            }
            else
            {
                libraryDto.KeyId = library.KeyId;
                libraryDto.AddedBy = library.AddedBy;

                libraryDto.Title = library.Title.Trim().ToTitleCase();
                if (LibraryFileId != null)
                {
                    libraryDto.BannerImage = libraryFileService.GetLibraryFile((long)LibraryFileId).FilePath;
                }
                else
                {
                    libraryDto.BannerImage = library.BannerImage;
                }

                libraryDto.LibraryTypeId = library.LibraryTypeId;
                //libraryDto.IP = library.Ip;
                libraryDto.ModifyDate = library.ModifyDate?.ToString("dd MMM yyyy");
                libraryDto.keywords = library.SearchKeyword;
                libraryDto.Description = library.Description;
                libraryDto.Version = library.Version;
                libraryDto.IntegrationHours = library.IntegrationHours;
                libraryDto.IsReadyToUse = library.IsReadyToUse;
                libraryDto.BAName = library.UidBaNavigation?.Name;
                libraryDto.TLName = library.UidTlNavigation?.Name;

                if (library.LibraryTechnology.Count > 0)
                {
                    libraryDto.LibraryTechnologiesComma = String.Join(", ", library.LibraryTechnology.Select(x => x.Technology.Title).ToArray());
                }
                if (library.LibraryIndustry.Count > 0)
                {
                    libraryDto.LibraryIndustriesComma = String.Join(", ", library.LibraryIndustry.Select(x => x.Industry.DomainName).ToArray());
                }
                libraryDto.CreatedDate = library.LibraryCreatedDate?.ToString("dd MMM yyyy");
                var author = library.AuthorUid.HasValue ? library.AuthorU : null;
                string department = author != null ? author.DeptId.HasValue ? author.Department.Name : string.Empty : string.Empty;
                libraryDto.Author = $"{(author != null ? author.Name : string.Empty)}{(!string.IsNullOrEmpty(department) ? " (" + department + ")" : string.Empty)}";

                if (library.LibraryFile != null)
                {
                    string[] imageExtensions = null;
                    var libraryFileType = libraryFileTypeService.GetImageType();
                    imageExtensions = libraryFileTypeService.GetImageType() != null ? libraryFileType.Extension.Split(',') : new string[0];

                    var libraryFiles = library.LibraryFile;
                    if (LibraryFileId != null)
                    {
                        libraryFiles = libraryFiles.Where(x => x.Id == LibraryFileId).ToList();
                    }

                    foreach (var libraryFile in libraryFiles)
                    {
                        LibraryFileDto libraryFileDto = new LibraryFileDto();
                        libraryFileDto.FileName = Path.GetFileName(libraryFile.FilePath);
                        libraryFileDto.FilePath = libraryFile.FilePath;
                        libraryFileDto.Id = libraryFile.Id;
                        libraryFileDto.LibraryId = libraryFile.Id;
                        libraryFileDto.PSDFilePath = libraryFile.PsdfilePath;
                        libraryFileDto.DesignUnitType = libraryFile.DesignUnitType ?? false;
                        //checking whether file extension matches with image extension
                        if (imageExtensions.Contains(Path.GetExtension(libraryFile.FilePath).ToLower()))
                        {
                            libraryDto.ImageFiles.Add(libraryFile.FilePath);
                        }


                        //add psd file path in a list
                        libraryDto.PSDFiles.Add(libraryFile.PsdfilePath ?? "");


                        if (library.LibraryTypeId == (byte)Enums.LibraryType.Design)
                        {
                            libraryDto.LibraryFileDtos = libraryFiles?.
                            Select(x => new LibraryFileDto
                            {
                                Id = x.Id,
                                FileImage = x.LibraryFileType.Icon,
                                FilePath = x.FilePath,
                                FileName = Path.GetFileName(x.FilePath),
                                FileShortName = GetShortFileName(Path.GetFileName(x.FilePath), 33),
                                PSDFilePath = x.PsdfilePath,
                                PsdFileShortName = GetShortFileName(Path.GetFileName(x.PsdfilePath), 33),
                                LibraryLayoutType = x.LibraryLayoutType,
                                LibraryLayoutTypeId = x.LibraryLayoutTypeId,
                                DesignUnitType = x.DesignUnitType ?? false
                            }).ToList();
                        }
                        else
                        {
                            libraryDto.LibraryFileDtos = library.LibraryFile?.
                            Select(x => new LibraryFileDto
                            {
                                Id = x.Id,
                                FileImage = x.LibraryFileType.Icon,
                                FilePath = x.FilePath,
                                FileName = Path.GetFileName(x.FilePath),
                                FileShortName = GetShortFileName(Path.GetFileName(x.FilePath), 33),
                                PSDFilePath = x.PsdfilePath,
                                PsdFileShortName = GetShortFileName(Path.GetFileName(x.PsdfilePath), 33),
                                DesignUnitType = x.DesignUnitType ?? false
                            }).ToList();
                        }
                    }
                }
            }
            return PartialView("Design_Details", libraryDto);
        }

        [CustomActionAuthorization()]
        [HttpGet]
        public IActionResult ComponentDocumentDetails(Guid KeyId)
        {
            //if (!HasDownloadPermission())
            //{
            //    return AccessDenied();
            //}
            Library library = libraryManagementService.GetLibraryByGUID(KeyId);
            LibraryDto libraryDto = new LibraryDto();
            if (library == null)
            {
                return MessagePartialView("Invalid library Id");
            }
            else
            {
                libraryDto.Title = library.Title.Trim().ToTitleCase();
                libraryDto.BannerImage = library.BannerImage;
                //libraryDto.IP = library.Ip;
                libraryDto.ModifyDate = library.ModifyDate?.ToString("dd MMM yyyy");
                libraryDto.keywords = library.SearchKeyword;
                libraryDto.Description = library.Description;
                libraryDto.Version = library.Version ?? "1.0";
                libraryDto.IntegrationHours = library.IntegrationHours;
                libraryDto.IsReadyToUse = library.IsReadyToUse;
                libraryDto.BAName = library.UidBaNavigation?.Name;
                libraryDto.TLName = library.UidTlNavigation?.Name;
                if (library.LibraryTechnology.Count > 0)
                {
                    libraryDto.LibraryTechnologiesComma = String.Join(", ", library.LibraryTechnology.Select(x => x.Technology.Title).ToArray());
                }
                if (library.LibraryIndustry.Count > 0)
                {
                    libraryDto.LibraryIndustriesComma = String.Join(", ", library.LibraryIndustry.Select(x => x.Industry.DomainName).ToArray());
                }
                libraryDto.CreatedDate = library.LibraryCreatedDate?.ToString("dd MMM yyyy");
                var author = library.AuthorUid.HasValue ? library.AuthorU : null;
                string department = author != null ? author.DeptId.HasValue ? author.Department.Name : string.Empty : string.Empty;
                libraryDto.Author = $"{(author != null ? author.Name : string.Empty)}{(!string.IsNullOrEmpty(department) ? " (" + department + ")" : string.Empty)}";

                if (library.LibraryFile != null)
                {
                    string[] imageExtensions = null;
                    var libraryFileType = libraryFileTypeService.GetImageType();

                    imageExtensions = libraryFileTypeService.GetImageType() != null ? libraryFileType.Extension.Split(',') : new string[0];


                    //libraryDto.ImageFiles = library.LibraryFile.Where(lf => imageExtensions.Contains(Path.GetExtension(lf.FilePath).ToLower())).Select(lf=>lf.FilePath).ToList();
                    foreach (var libraryFile in library.LibraryFile)
                    {
                        LibraryFileDto libraryFileDto = new LibraryFileDto();
                        libraryFileDto.FileName = Path.GetFileName(libraryFile.FilePath);
                        libraryFileDto.FilePath = libraryFile.FilePath;
                        libraryFileDto.Id = libraryFile.Id;
                        libraryFileDto.LibraryId = libraryFile.Id;
                        //checking whether file extension matches with image extension
                        if (imageExtensions.Contains(Path.GetExtension(libraryFile.FilePath).ToLower()))
                        {
                            libraryDto.ImageFiles.Add(libraryFile.FilePath);
                        }
                        //else
                        //{
                        //    libraryDto.LibraryFileDtos.Add(libraryFileDto);
                        //}
                        libraryDto.LibraryFileDtos = library.LibraryFile?.
                        Select(x => new LibraryFileDto
                        {
                            Id = x.Id,
                            //FileImage = Path.GetExtension(x.FilePath).ToLower() == ".zip" ? "zip-link" : "rar-link",
                            FileImage = x.LibraryFileType.Icon,
                            FilePath = x.FilePath,
                            FileName = Path.GetFileName(x.FilePath),
                            FileShortName = GetShortFileName(Path.GetFileName(x.FilePath), 33)
                        }).ToList();

                    }

                }
                if (library.LibraryComponentFile != null)
                {
                    //string[] imageExtensions = null;
                    //var libraryFileType = libraryFileTypeService.GetImageType();

                    //imageExtensions = libraryFileTypeService.GetImageType() != null ? libraryFileType.Extension.Split(',') : new string[0];


                    //libraryDto.ImageFiles = library.LibraryFile.Where(lf => imageExtensions.Contains(Path.GetExtension(lf.FilePath).ToLower())).Select(lf=>lf.FilePath).ToList();
                    foreach (var libraryFile in library.LibraryComponentFile)
                    {
                        LibraryComponentFileDto libraryFileDto = new LibraryComponentFileDto();
                        libraryFileDto.FileName = Path.GetFileName(libraryFile.FilePath);
                        libraryFileDto.FilePath = libraryFile.FilePath;
                        libraryFileDto.Id = libraryFile.Id;
                        libraryFileDto.LibraryId = libraryFile.Id;
                        //checking whether file extension matches with image extension
                        if (imageExtensions.Contains(Path.GetExtension(libraryFile.FilePath).ToLower()))
                        {
                            libraryDto.ImageFiles.Add(libraryFile.FilePath);
                        }
                        //else
                        //{
                        //    libraryDto.LibraryFileDtos.Add(libraryFileDto);
                        //}
                        libraryDto.LibraryComponentFileDtos = library.LibraryComponentFile?.
                        Select(x => new LibraryComponentFileDto
                        {
                            Id = x.Id,
                            //FileImage = Path.GetExtension(x.FilePath).ToLower() == ".zip" ? "zip-link" : "rar-link",
                            FileImage = x.LibraryFileType.Icon,
                            FilePath = x.FilePath,
                            FileName = Path.GetFileName(x.FilePath),
                            FileShortName = GetShortFileName(Path.GetFileName(x.FilePath), 33)
                        }).ToList();

                    }

                }

            }

            return PartialView("DocumentComponentDetails", libraryDto);
        }

        [HttpPost]
        [CustomActionAuthorization()]
        public IActionResult CreateSearch(LibraryManagementSearchFilterDto libraryManagementSearchFilter)
        {
            LibrarySearchDto librarySearchDto = new LibrarySearchDto();

            if (!string.IsNullOrWhiteSpace(libraryManagementSearchFilter.SearchText))
            {
                librarySearchDto.Keyword = libraryManagementSearchFilter.SearchText;
            }
            librarySearchDto.LibraryType = libraryManagementSearchFilter.LibraryType; // Library type should always be there
            librarySearchDto.DesignType = libraryManagementSearchFilter.DesignType; // Library type should always be there

            if (libraryManagementSearchFilter.IsAdvanceSearch)
            {
                librarySearchDto.IsAdvanceSearch = libraryManagementSearchFilter.IsAdvanceSearch;
                if (libraryManagementSearchFilter.Technologies?.Length > 0)
                {
                    librarySearchDto.Technologies = libraryManagementSearchFilter.Technologies;
                }
                if (libraryManagementSearchFilter.Domains?.Length > 0)
                {
                    librarySearchDto.Domains = libraryManagementSearchFilter.Domains;
                }
                if (libraryManagementSearchFilter.Layouts?.Length > 0)
                {
                    librarySearchDto.Layouts = libraryManagementSearchFilter.Layouts;
                }
                if (libraryManagementSearchFilter.Components?.Length > 0)
                {
                    librarySearchDto.Components = libraryManagementSearchFilter.Components;
                }
                if (libraryManagementSearchFilter.Templates != null && libraryManagementSearchFilter.Templates.Length > 0)
                {
                    librarySearchDto.Templates = libraryManagementSearchFilter.Templates;
                }
                if (libraryManagementSearchFilter.IsNDA == null)
                {
                    // Do nothing
                }
                else
                {
                    librarySearchDto.IsNDA = libraryManagementSearchFilter.IsNDA;
                }
                if (libraryManagementSearchFilter.Featured == null)
                {
                    // Do nothing
                }
                else
                {
                    librarySearchDto.Featured = libraryManagementSearchFilter.Featured;
                }
                if (libraryManagementSearchFilter.IsReadyToUse == null)
                {
                    // Do nothing
                }
                else
                {
                    librarySearchDto.IsReadyToUse = libraryManagementSearchFilter.IsReadyToUse;
                }


            }
            librarySearchDto.KeyId = libraryManagementSearchFilter.KeyId;
            librarySearchDto.IP = ContextProvider.HttpContext.Connection.RemoteIpAddress.ToString();
            LibrarySearch librarySearch = librarySearchService.Save(librarySearchDto);
            return Json(new { isSuccess = true, guid = librarySearch.Id });
        }

        private LibraryManagementDto GetLibraryManagementDto(Guid id)
        {
            LibraryManagementSearchFilterDto libraryManagementSearchFilterDto = new LibraryManagementSearchFilterDto();
            if (id != null)
            {
                LibrarySearch librarySearch = librarySearchService.LibrarySearchFindById(id);
                if (librarySearch != null)
                {
                    libraryManagementSearchFilterDto.LibrarySearchExist = true;
                    libraryManagementSearchFilterDto.SearchText = librarySearch.Keyword;
                    libraryManagementSearchFilterDto.KeyId = librarySearch.KeyId; //KeyId is there in case it is individual copy
                    libraryManagementSearchFilterDto.IsNDA = librarySearch.IsNda;
                    libraryManagementSearchFilterDto.Featured = librarySearch.IsFeatured;
                    libraryManagementSearchFilterDto.IsReadyToUse = librarySearch.IsReadyToUse;
                    libraryManagementSearchFilterDto.LibraryType = (Enums.LibraryType)librarySearch.LibraryTypeId;
                    libraryManagementSearchFilterDto.DesignType = librarySearch.DesignTypeId.HasValue ? (Enums.DesignType?)librarySearch.DesignTypeId : null;
                    libraryManagementSearchFilterDto.IsAdvanceSearch = librarySearch.IsAdvanceFiltered;
                    libraryManagementSearchFilterDto.Technologies = librarySearch.LibrarySearchTechnology != null ? librarySearch.LibrarySearchTechnology.Select(lt => (int)lt.TechnologyId).ToArray() : new int[0];
                    libraryManagementSearchFilterDto.Domains = librarySearch.LibrarySearchIndustry != null ? librarySearch.LibrarySearchIndustry.Select(lt => lt.DomainId).ToArray() : new int[0];
                    libraryManagementSearchFilterDto.Layouts = librarySearch.LibrarySearchLayoutTypeMapping != null ? librarySearch.LibrarySearchLayoutTypeMapping.Select(lt => lt.LayoutTypeId).ToArray() : new int[0];
                    libraryManagementSearchFilterDto.Components = librarySearch.LibrarySearchComponent != null ? librarySearch.LibrarySearchComponent.Select(lt => lt.ComponentTypeId).ToArray() : new int[0];
                    //libraryManagementSearchFilterDto.Templates = librarySearch.LibrarySearchTemplate != null ? librarySearch.LibrarySearchTemplate.Select(lt => lt.TemplateTypeId).ToArray() : new int[0];

                    // Search Labels
                    libraryManagementSearchFilterDto.ComponentFilterId = ".component";
                    libraryManagementSearchFilterDto.ComponentLabel = "Component";
                    libraryManagementSearchFilterDto.DesignTypeFilterId = "#designTypes";
                    libraryManagementSearchFilterDto.DesignTypeLabel = "Design Type:";
                    libraryManagementSearchFilterDto.FeaturedStatusFilterId = "#Featuerd";
                    libraryManagementSearchFilterDto.FeaturedStatusLabel = "Featured Status";
                    libraryManagementSearchFilterDto.IndustryFilterId = ".industry";
                    libraryManagementSearchFilterDto.IndustryLabel = "Domain";
                    libraryManagementSearchFilterDto.IsReadyToUseStatusFilterId = "#IsReadyToUse";
                    libraryManagementSearchFilterDto.IsReadyToUseStatusLabel = "Is ReadyToUse";
                    libraryManagementSearchFilterDto.LayoutFilterId = ".layout-filter";
                    libraryManagementSearchFilterDto.LayoutLabel = "Layout";
                    libraryManagementSearchFilterDto.LibraryTypeFilterId = "#LibraryTypeId";
                    libraryManagementSearchFilterDto.LibraryTypeLabel = "Type:";
                    libraryManagementSearchFilterDto.NDAStatusFilterId = "#isNDA";
                    libraryManagementSearchFilterDto.NDAStatusLabel = "NDA Status";
                    libraryManagementSearchFilterDto.TechnologyFilterId = ".technology";
                    libraryManagementSearchFilterDto.TechnologyLabel = "Technology";
                }
                else //library search doesn't exist
                {
                    libraryManagementSearchFilterDto.LibrarySearchExist = false;
                    return new LibraryManagementDto();
                }
            }
            int totalCount = 0;
            libraryManagementSearchFilterDto.DataLength = PageDataLength;
            libraryManagementSearchFilterDto.PageStart = pageNo;
            var expr = GetLibraryFilterExpression(libraryManagementSearchFilterDto);
            var libraries = libraryManagementService.GetLibraries(out totalCount, expr);
            //if (libraryManagementSearchFilterDto.LibraryType == Enums.LibraryType.Design)
            //{
            //    totalCount = libraryManagementService.CountDesignLibraries(expr);
            //}
            LibraryManagementDto libraryManagementDto = new LibraryManagementDto();
            pageNo++;
            if (libraries != null && libraries.Count > 0)
            {
                foreach (var library in libraries)
                {
                    LibraryDto libraryDto = new LibraryDto();
                    libraryDto.Id = library.Id;
                    libraryDto.KeyId = library.KeyId;
                    libraryDto.LibraryTypeId = library.LibraryTypeId;
                    libraryDto.Title = library.Title.Trim().ToTitleCase();
                    libraryDto.Description = !string.IsNullOrWhiteSpace(library.Description) ? library.Description.Trim().ToTitleCase() : null;
                    libraryDto.BannerImage = library.BannerImage;
                    libraryDto.Version = library.Version;
                    libraryDto.IntegrationHours = library.IntegrationHours;
                    libraryDto.IsReadyToUse = library.IsReadyToUse;
                    libraryDto.LiveURL = library.LiveUrl;
                    if (library.LibraryTechnology.Count > 0)
                    {
                        libraryDto.LibraryTechnologiesComma = String.Join(", ", library.LibraryTechnology.Select(x => x.Technology.Title).ToArray());
                    }
                    if (library.LibraryIndustry.Count > 0)
                    {
                        libraryDto.LibraryIndustriesComma = String.Join(", ", library.LibraryIndustry.Select(x => x.Industry.DomainName).ToArray());
                    }
                    libraryDto.CreatedDate = library.LibraryCreatedDate?.ToString("dd MMM yyyy");
                    libraryDto.IsFeatured = library.IsFeatured;
                    libraryDto.AddedBy = library.AddedBy;

                    var author = library.AuthorUid.HasValue ? library.AuthorU : null;
                    string department = author != null ? author.DeptId.HasValue ? author.Department.Name : string.Empty : string.Empty;
                    libraryDto.Author = $"{(author != null ? author.Name : string.Empty)}{(!string.IsNullOrEmpty(department) ? " (" + department + ")" : string.Empty)}";

                    libraryDto.LibraryFileTypes = GetLibraryFileType(library.LibraryFile);

                    //For component File type to show not required
                    libraryDto.LibraryFileDtos = library.LibraryFile?.
                        Select(x => new LibraryFileDto
                        {
                            Id = x.Id,
                            //FileImage = Path.GetExtension(x.FilePath).ToLower() == ".zip" ? "zip-link" : "rar-link",
                            FileImage = x.LibraryFileType.Icon,
                            FilePath = x.FilePath,
                            FileName = Path.GetFileName(x.FilePath),
                            PSDFilePath = x.PsdfilePath,
                            LibraryId = x.LibraryId,
                            LibraryLayoutTypeId = x.LibraryLayoutTypeId,
                            LibraryLayoutType = x.LibraryLayoutType,
                            FileTypeName = x.LibraryFileType.Name,
                        }).ToList();

                    libraryManagementDto.libraries.Add(libraryDto);
                }
            }
            libraryManagementDto.LibraryFileIds = (libraryManagementSearchFilterDto.Layouts != null)
                    ? libraryManagementSearchFilterDto.Layouts.OfType<int>().ToList() : null;
            libraryManagementDto.libraryManagementSearchFilterDto = libraryManagementSearchFilterDto;
            libraryManagementDto.totalRecords = totalCount;
            return libraryManagementDto;
        }

        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult AddEdit(Guid? guid)
        {
            //if (!HasDownloadPermission())
            //{
            //    return AccessDenied();
            //}
            LibraryDto model = new LibraryDto();
            if (guid != null)
            {
                var library = libraryManagementService.GetLibraryByKeyId(guid.Value);
                if (library != null)
                {
                    model.Id = library.Id;
                    //model.KeyId = library.KeyId;
                    model.Title = library.Title;
                    model.CRMUserId = library.CRMUserId;
                    model.LibraryTypeId = library.LibraryTypeId;

                    model.SalesKitId = library.SalesKitId;
                    model.CvsId = library.CvsId;

                    model.Description = library.Description;
                    model.keywords = library.SearchKeyword;
                    model.IsNDA = library.IsNda;
                    model.IsFeatured = library.IsFeatured;
                    model.Team = library.IsInternal;
                    model.IsReadyToUse = library.IsReadyToUse;
                    model.IsLive = library.IsLive;
                    model.LiveURL = library.LiveUrl;
                    model.LibraryTypes = WebExtensions.GetSelectList<Enums.LibraryType>();
                    model.LibraryTypes.ForEach(x => x.Value = x.Value == "0" ? "" : x.Value);

                    model.SalesKitTypes = GetSalesKitTypeSelectList();
                    model.CvsTypes = cvsTypeService.GetCvsType().Select(n => new SelectListItem { Text = n.Name, Value = n.CvsId.ToString() }).ToList();
                    model.SalesKitTypes.Insert(0, new SelectListItem { Text = "Select Sales Kit Type", Value = "" });
                    model.CvsTypes.Insert(0, new SelectListItem { Text = "Select CVs Type", Value = "" });

                    model.BannerImage = library.BannerImage;
                    model.DesignTypeId = library.DesignTypeId;
                    model.DesignTypes = WebExtensions.GetSelectList<Enums.DesignType>();
                    model.OtherIndustry = library.OtherIndustry;
                    model.OtherTechnologyParent = library.OtherTechnologyParent;
                    model.OtherTechnology = library.OtherTechnology;
                    model.AuthorUid = library.AuthorUid;
                    model.UidBA = library.UidBa;
                    model.UidTL = library.UidTl;
                    model.IsActive = library.IsActive;
                    model.Version = library.Version;
                    model.IsReadyToUse = library.IsReadyToUse;
                    model.IntegrationHours = library.IntegrationHours;
                    model.ReDevelopmentHours = library.ReDevelopmentHours.HasValue ? library.ReDevelopmentHours : library.IntegrationHours.HasValue ? (library.IntegrationHours * 30) / 100 : library.ReDevelopmentHours;
                    model.EstimatedHours = library.EstimatedHours.HasValue ? library.EstimatedHours : library.IntegrationHours.HasValue ? library.IntegrationHours / 2 : library.EstimatedHours;
                    model.IsGoodToShow = library.IsGoodToShow;
                    model.LayoutTypes = libraryLayoutService.GetLibraryLayouts().
                                                    Select(n => new SelectListItem
                                                    {
                                                        Text = n.Name,
                                                        Value = n.Id.ToString(),
                                                        Selected = library.LibraryLayoutTypeMapping.Any(
                                                            x => x.LibraryLayoutTypeId == n.Id)
                                                    }).ToList();

                    model.DesignLayoutTypes = libraryLayoutService.GetLibraryLayouts().
                                                    Select(n => new SelectListItem
                                                    {
                                                        Text = n.Name,
                                                        Value = n.Id.ToString()
                                                    }).OrderBy(O => O.Text).ToList();

                    model.Industries = domainTypeService.GetDomainList().
                        Select(n => new SelectListItem
                        {
                            Text = n.DomainName,
                            Value = n.DomainId.ToString(),
                            Selected = library.LibraryIndustry.Any(x => x.IndustryId == n.DomainId)
                        }).ToList();
                    model.LibraryComponentTypes = libraryComponentTypeService.GetLibraryComponentTypes().
                        Select(n => new SelectListItem
                        {
                            Text = n.Name,
                            Value = n.Id.ToString(),
                            Selected = library.LibraryComponent.Any(x => x.LibraryComponentTypeId == n.Id)
                        }).OrderBy(y => y.Text).ToList();

                    model.LibraryTemplateTypes = libraryTemplateTypeService.GetLibraryTemplateTypes().
                        Select(n => new SelectListItem
                        {
                            Text = n.Name,
                            Value = n.Id.ToString(),
                            Selected = library.LibraryComponent.Any(x => x.LibraryComponentTypeId == n.Id)
                        }).ToList();

                    model.TechnologyParents = technologyParentService.GetTechnologyParentList().
                        Select(n => new SelectListItem
                        {
                            Text = n.Title,
                            Value = n.Id.ToString(),
                            Selected = n.TechnologyParentMapping.Any(p => library.LibraryTechnology.Any(y => y.TechnologyId == p.Technology.TechId))
                        }).ToList();

                    model.Technologies = technologyParentMappingService.GetTechnologyParentList()
                                        .Where(x => model.TechnologyParents.Where(y => y.Selected == true)
                                        .Any(n => Convert.ToInt32(n.Value) == x.TechnologyParentId)
                            ).ToList().OrderBy(x => x.TechnologyParentId).Select(n => new SelectListItem
                            {
                                Text = n.Technology.Title,
                                Value = n.TechnologyId.ToString(),
                                Selected = library.LibraryTechnology.Any(x => x.TechnologyId == n.TechnologyId),
                                Group = new SelectListGroup() { Name = n.TechnologyParentId.ToString() }
                            }).ToList();

                    model.LibraryFileList = libraryFileService.GetLibraryFiles().Where(x => x.LibraryId == library.Id).ToList();
                    model.LibraryComponentFileList = libraryComponentFileService.GetLibraryFiles().Where(x => x.LibraryId == library.Id).ToList();

                    if (!string.IsNullOrWhiteSpace(model.BannerImage))
                    {
                        var bannerItem = model.LibraryFileList.Where(x => x.FilePath == model.BannerImage).FirstOrDefault();
                        if (bannerItem != null)
                        {
                            model.CoverImage = model.LibraryFileList.IndexOf(bannerItem);
                        }
                    }
                }
            }
            else
            {
                model.Version = "1.0";
                model.LibraryTypes = WebExtensions.GetSelectList<Enums.LibraryType>();

                model.SalesKitTypes = GetSalesKitTypeSelectList();
                model.CvsTypes = cvsTypeService.GetCvsType().Select(n => new SelectListItem { Text = n.Name, Value = n.CvsId.ToString() }).ToList();
                model.SalesKitTypes.Insert(0, new SelectListItem { Text = "Select Sales Kit Type", Value = "" });
                model.CvsTypes.Insert(0, new SelectListItem { Text = "Select CVs Type", Value = "" });


                model.DesignTypes = WebExtensions.GetSelectList<Enums.DesignType>();
                model.LayoutTypes = libraryLayoutService.GetLibraryLayouts().Select(n => new SelectListItem { Text = n.Name, Value = n.Id.ToString() }).ToList();
                model.DesignLayoutTypes = libraryLayoutService.GetLibraryLayouts().Select(n => new SelectListItem { Text = n.Name, Value = n.Id.ToString() }).OrderBy(O => O.Text).ToList();
                model.Industries = domainTypeService.GetDomainList().Select(n => new SelectListItem { Text = n.DomainName, Value = n.DomainId.ToString() }).ToList();
                model.Technologies = technologyService.GetTechnologyList().Select(n => new SelectListItem { Text = n.Title, Value = n.TechId.ToString() }).ToList();
                model.TechnologyParents = technologyParentService.GetTechnologyParentList().Select(n => new SelectListItem { Text = n.Title, Value = n.Id.ToString() }).ToList();
                model.LibraryComponentTypes = libraryComponentTypeService.GetLibraryComponentTypes().Select(n => new SelectListItem { Text = n.Name, Value = n.Id.ToString() }).OrderBy(y => y.Text).ToList();
                model.LibraryTemplateTypes = libraryTemplateTypeService.GetLibraryTemplateTypes().Select(n => new SelectListItem { Text = n.Name, Value = n.Id.ToString() }).ToList();

                //IsFeatured set by default false
                model.IsFeatured = false;
                //IsActive set by default true
                model.IsActive = model.IsActive;
            }
            var users = userLoginService.GetUsersByPM(PMUserId);
            model.Users = users?.Select(n => new SelectListItem { Text = $"{n.Name}{(n.DeptId.HasValue ? " (" + n.Department.Name + ")" : "")}", Value = n.Uid.ToString() }).ToList();
            model.BAUsers = users?.Where(x => RoleValidator.BA_RoleIds.Contains(x.RoleId.Value) || x.RoleId == (int)Enums.UserRoles.PM)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Uid.ToString()
                }).ToList();

            
            model.TLUsers = users.Where(x =>
            RoleValidator.TL_Technical_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_Sales_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_HR_DesignationIds.Contains(x.DesignationId.Value)
            //|| RoleValidator.DV_RoleIds.Contains(x.RoleId.Value)
            )
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Uid.ToString()
                }).ToList();

            //temporary remove template from this list
            //model.LibraryTypes.RemoveAt(6);

            return View(model);
        }

        private List<SelectListItem> GetSalesKitTypeSelectList()
        {
            var lst = salesKitTypeService.GetSalesKitType();
            var list = new List<SelectListItem>();
            foreach (var item in lst)
            {
                var sublst = salesKitTypeService.GetSubSalesKitType(item.SalesKitId);
                var objlst = new SelectListItem()
                {
                    Text = $"--- { item.Name } ---",
                    Value = item.SalesKitId.ToString(),
                    Disabled = true
                };
                list.Add(objlst);
                foreach (var subitem in sublst.OrderBy(x => x.DisplayOrder))
                {
                    var objSublst = new SelectListItem()
                    {
                        Text = subitem.Name,
                        Value = subitem.SalesKitId.ToString()
                    };
                    list.Add(objSublst);
                }
            }
            return list;
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult AddEdit(LibraryDto model)
        {
            //if (!HasDownloadPermission())
            //{
            //    return AccessDenied();
            //}
            if (model.LibraryTypeId != (byte)Enums.LibraryType.Design)
            {
                int i = 0;
                foreach (var item in model.DesignFiles)
                {
                    ModelState.Remove("DesignFiles[" + i + "].DesignLayoutType");
                    i++;
                }
                model.DesignFiles.Clear();
            }
            else
            {
                int i = 0;
                List<int> tempDesignFiles = new List<int>();
                foreach (var item in model.DesignFiles)
                {
                    if (item.DesignLayoutType == 0)
                    {
                        ModelState.Remove("DesignFiles[" + i + "].DesignLayoutType");
                        tempDesignFiles.Add(i);
                    }
                    i++;
                }
                foreach (var item in tempDesignFiles)
                {
                    model.DesignFiles.RemoveAt(item);
                }
            }
            if(model.LibraryTypeId !=(int)LibraryType.SalesKit)
            {
                ModelState.Remove("SalesKitId");
            }
            if(model.LibraryTypeId !=(int)LibraryType.CVs)
            {
                ModelState.Remove("CvsId");
            }
            if (ModelState.IsValid)
            {
                //Library IsLibraryExists = null;
                //if (model.Id == 0)
                //{
                //    IsLibraryExists = libraryManagementService.GetAllLibraries().Where(x => x.Title.ToLower().Trim() == model.Title.ToLower().Trim()).FirstOrDefault();
                //}

                //if (IsLibraryExists == null)
                //{
                string fileTypeError = string.Empty;
                IFormFile formFile = null;
                try
                {
                    Library library = new Library
                    {
                        Title = model.Title,
                        LibraryTypeId = model.LibraryTypeId,

                        SalesKitId = model.SalesKitId,
                        CvsId = model.CvsId,

                        CRMUserId = model.CRMUserId,
                        SearchKeyword = model.keywords,
                        Description = model.Description,
                        IsNda = model.IsNDA.HasValue ? (bool)model.IsNDA : false,
                        IsFeatured = model.IsFeatured.HasValue ? (bool)model.IsFeatured : false,
                        IsInternal = model.Team.HasValue ? (bool)model.Team : false,
                        IsLive = model.IsLive.HasValue ? (bool)model.IsLive : false,
                        IsReadyToUse = model.IsReadyToUse ? (bool)model.IsReadyToUse : false,
                        LiveUrl = model.LiveURL,
                        AddDate = DateTime.Now,
                        LibraryCreatedDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        AddedBy = CurrentUser.Uid,
                        ModifyByUid = CurrentUser.Uid,
                        DesignTypeId = model.DesignTypeId,
                        OtherIndustry = model.OtherIndustry,
                        OtherTechnologyParent = model.OtherTechnologyParent,
                        OtherTechnology = model.OtherTechnology,
                        IsActive = model.IsActive,
                        AuthorUid = model.AuthorUid,
                        UidTl = model.UidTL,
                        UidBa = model.UidBA,
                        IsGoodToShow = model.IsGoodToShow.HasValue ? (bool)model.IsGoodToShow : false,
                        Version = model.Version,
                        IntegrationHours = model.IntegrationHours,
                        ReDevelopmentHours = !model.ReDevelopmentHours.HasValue && model.IntegrationHours.HasValue ? (model.IntegrationHours * 30) / 100 : model.ReDevelopmentHours,
                        EstimatedHours = !model.EstimatedHours.HasValue && model.IntegrationHours.HasValue ? model.IntegrationHours / 2 : model.EstimatedHours
                    };
                    if (model.AuthorUid.HasValue)
                    {
                        //model.TLName= model.UidTL.HasValue?
                        var user = userLoginService.GetUserInfoByID(model.AuthorUid.Value);
                        model.Author = user != null ? user.Name : string.Empty;
                    }
                    if (model.UidTL.HasValue)
                    {
                        //model.TLName= model.UidTL.HasValue?
                        var user = userLoginService.GetUserInfoByID(model.UidTL.Value);
                        model.TLName = user != null ? user.Name : string.Empty;
                    }
                    if (model.UidBA.HasValue)
                    {
                        var user = userLoginService.GetUserInfoByID(model.UidTL.Value);
                        model.BAName = user != null ? user.Name : string.Empty;
                    }
                    var libraryEntity = new Library();
                    if (model.Id > 0)
                    {
                        library.Id = model.Id;
                        libraryEntity = libraryManagementService.GetLibraryById(model.Id);
                    }
                    else
                    {
                        library.KeyId = Guid.NewGuid();
                    }

                    if (model.Industry != null && model.Industry.Length > 0)
                    {
                        foreach (var item in model.Industry)
                        {
                            LibraryIndustry industry = new LibraryIndustry
                            {
                                IndustryId = Convert.ToInt32(item),
                            };
                            library.LibraryIndustry.Add(industry);
                        }
                    }

                    if (model.Technology != null && model.Technology.Length > 0)
                    {
                        foreach (var item in model.Technology)
                        {
                            LibraryTechnology technology = new LibraryTechnology
                            {
                                TechnologyId = Convert.ToInt32(item),
                            };
                            library.LibraryTechnology.Add(technology);
                        }
                    }

                    if (model.LibraryComponent != null && model.LibraryComponent.Length > 0)
                    {
                        foreach (var item in model.LibraryComponent)
                        {
                            LibraryComponent libraryComponent = new LibraryComponent
                            {
                                LibraryComponentTypeId = Convert.ToInt32(item),
                            };
                            library.LibraryComponent.Add(libraryComponent);
                        }
                    }

                    if (model.LibraryTemplate != null && model.LibraryTemplate.Length > 0)
                    {
                        foreach (var item in model.LibraryTemplate)
                        {
                            LibraryTemplate libraryTemplate = new LibraryTemplate
                            {
                                LibraryTemplateTypeId = Convert.ToInt32(item),
                            };
                            library.LibraryTemplate.Add(libraryTemplate);
                        }
                    }

                    //if (model.LayoutType != null && model.LayoutType.Length > 0)
                    //{
                    //    foreach (var item in model.LayoutType)
                    //    {
                    //        LibraryLayoutTypeMapping libraryLayout = new LibraryLayoutTypeMapping
                    //        {
                    //            LibraryLayoutTypeId = Convert.ToInt32(item),
                    //        };
                    //        library.LibraryLayoutTypeMapping.Add(libraryLayout);
                    //    }
                    //}

                    var libraryFileList = libraryFileService.GetLibraryFiles().Where(x => x.LibraryId == model.Id).ToList();
                    var libraryComponentFileList = libraryComponentFileService.GetLibraryFiles().Where(x => x.LibraryId == model.Id).ToList();

                    /*
                     * Image files with layout type for library type design 
                     */
                    if (model.DesignFiles != null && model.DesignFiles.Count > 0)
                    {
                        foreach (var item in model.DesignFiles)
                        {
                            /*
                             * For Image File
                             */
                            LibraryFile libraryFile = new LibraryFile();
                            var UniqueFileId = Guid.NewGuid();
                            var timeWithMiliseconds = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                            //LibraryLayoutType libLaoutType = new LibraryLayoutType();
                            var LibrarylaoutTypeName = libraryLayoutService.GetLibraryLayouts().Where(a => a.Id == item.DesignLayoutType).Select(z => z.Name).FirstOrDefault();
                            var CustomName = Regex.Replace(model.Title.Replace(' ', '-'), @"[^0-9a-zA-Z-]+", "") + "-" + LibrarylaoutTypeName + "-" + timeWithMiliseconds;
                            string fileExt = Path.GetExtension(item.Image.File.FileName.ToLower());
                            string fileName = $"{CustomName}{fileExt}";
                            int SrNO = serialNumberService.GetNumber() + 1;
                            serialNumberService.Save(new SerialNumber { Id = SrNO });
                            var fileType = libraryFileTypeService.GetLibraryFileTypes().Where(x => x.Extension.Contains(fileExt) && x.Name.ToLower() == "image").FirstOrDefault();
                            if (fileType != null)
                            {
                                libraryFile.KeyId = UniqueFileId;
                                libraryFile.FilePath = "Upload/LibraryFiles/" + fileName;
                                libraryFile.LibraryFileTypeId = fileType.Id;
                                libraryFile.AddDate = DateTime.Now;
                                libraryFile.ModifyDate = DateTime.Now;
                                libraryFile.SrNo = SrNO;
                                libraryFile.FileId = "101";
                                libraryFile.LibraryLayoutTypeId = item.DesignLayoutType;
                                string path = ContextProvider.HostEnvironment.WebRootPath + "/" + "Upload/LibraryFiles/";
                                string FullPath = Path.Combine(path, fileName);
                                uploadImageToFolder(item.Image.File, fileName, FullPath, path);
                            }
                            else
                            {
                                fileTypeError += fileExt + ",";
                            }
                            /*
                             * For PSD File
                             */
                            if (item.PSD.File != null)
                            {
                                UniqueFileId = Guid.NewGuid();
                                timeWithMiliseconds = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                                LibrarylaoutTypeName = libraryLayoutService.GetLibraryLayouts().Where(a => a.Id == item.DesignLayoutType).Select(z => z.Name).FirstOrDefault();
                                CustomName = Regex.Replace(model.Title.Replace(' ', '-'), @"[^0-9a-zA-Z-]+", "") + "-" + LibrarylaoutTypeName + "-" + timeWithMiliseconds;
                                fileExt = Path.GetExtension(item.PSD.File.FileName.ToLower());
                                fileName = $"{CustomName}{fileExt}";
                                SrNO = serialNumberService.GetNumber() + 1;
                                serialNumberService.Save(new SerialNumber { Id = SrNO });
                                fileType = libraryFileTypeService.GetLibraryFileTypes().Where(x => x.Extension.Contains(fileExt) && x.Name.ToLower() == "psd").FirstOrDefault();
                                if (fileType != null)
                                {
                                    libraryFile.PsdfilePath = "Upload/LibraryFiles/" + fileName;
                                    string path = ContextProvider.HostEnvironment.WebRootPath + "/" + "Upload/LibraryFiles/";
                                    string FullPath = Path.Combine(path, fileName);
                                    uploadImageToFolder(item.PSD.File, fileName, FullPath, path);
                                }
                                else
                                {
                                    fileTypeError += fileExt + ",";
                                }
                            }


                            libraryFile.DesignUnitType = item.DesignUnitType;
                            if (!string.IsNullOrEmpty(libraryFile.FileId))
                            {
                                library.LibraryFile.Add(libraryFile);
                                libraryFileList.Add(libraryFile);
                            }
                            //if (library.LibraryFile != null && library.LibraryFile.Count > 0 && model.LibraryTypeId == (byte)Enums.LibraryType.Design)
                            //{
                            //    foreach (var myLibraryLayout in library.LibraryFile)
                            //    {
                            //        LibraryLayoutTypeMapping libraryLayout = new LibraryLayoutTypeMapping
                            //        {
                            //            LibraryLayoutTypeId = myLibraryLayout.LibraryLayoutTypeId.Value,
                            //        };
                            //        library.LibraryLayoutTypeMapping.Add(libraryLayout);
                            //    }
                            //}
                        }
                    }

                    var IsImageExist = false;
                    var fileUploadLimit = 10 - libraryFileList.Count;
                    if (model.LibraryFiles != null && model.LibraryFiles.Count > 0)
                    {
                        int i = 0;
                        foreach (var item in model.LibraryFiles)
                        {
                            if (i < fileUploadLimit)
                            {
                                Random rnd = new Random();
                                var UniqueFileId = Guid.NewGuid();
                                var timeWithMiliseconds = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                                //LibrarylaoutTypeName = libraryLayoutService.GetLibraryLayouts().Where(a => a.Id == item.DesignLayoutType).Select(z => z.Name).FirstOrDefault();
                                var CustomName = Regex.Replace(model.Title.Replace(' ', '-'), @"[^0-9a-zA-Z-]+", "") + "-" + timeWithMiliseconds;
                                string fileExt = Path.GetExtension(item.FileName.ToLower());
                                string fileName = $"{CustomName}{fileExt}";
                                int SrNO = serialNumberService.GetNumber() + 1;
                                serialNumberService.Save(new SerialNumber { Id = SrNO });
                                string pptExtention = ".ppt,.pptx";
                                string docExtention = ".doc,.docx,.xls,.xlsx,.pdf";
                                bool isExtenstionValid = false;
                                var fileType = libraryFileTypeService.GetLibraryFileTypes().Where(x => x.Extension.Contains(fileExt)).FirstOrDefault();
                                if (model.LibraryTypeId == (int)LibraryType.SalesKit && model.SalesKitId.HasValue)
                                {
                                    var data = salesKitTypeService.GetSalesKitTypeDetail(model.SalesKitId.Value);
                                    if (data != null)
                                    {
                                        if (data.ParentId == 4)
                                        {
                                            isExtenstionValid = pptExtention.Contains(fileExt.ToLower());
                                        }
                                        else if (data.ParentId == 1 || data.ParentId == 3)
                                        {
                                            isExtenstionValid = docExtention.Contains(fileExt.ToLower());
                                        }
                                    }
                                }

                                //fileType = libraryFileTypeService.GetLibraryFileTypes().Where(x => x.Extension.Contains(fileExt)).FirstOrDefault();

                                if (model.LibraryTypeId == (int)LibraryType.SalesKit && isExtenstionValid)
                                {
                                    LibraryFile libraryFile = new LibraryFile
                                    {
                                        KeyId = UniqueFileId,
                                        FilePath = "Upload/LibraryFiles/" + fileName,
                                        LibraryFileTypeId = (int)FileType.Document,
                                        AddDate = DateTime.Now,
                                        ModifyDate = DateTime.Now,
                                        SrNo = SrNO,
                                        FileId = "101",
                                    };
                                    library.LibraryFile.Add(libraryFile);
                                    string path = ContextProvider.HostEnvironment.WebRootPath + "/" + "Upload/LibraryFiles/";
                                    string FullPath = Path.Combine(path, fileName);

                                    //if (imageExtensions.Any(x => x.Contains(fileExt)))
                                    //{
                                    //    if (!IsImageExist)
                                    //    {
                                    //        entity.BannerImage = libraryFile.FilePath;
                                    //        library.BannerImage = libraryFile.FilePath;
                                    //        IsImageExist = true;
                                    //    }
                                    //}
                                    uploadImageToFolder(item, fileName, FullPath, path);

                                    libraryFileList.Add(libraryFile);
                                    if (model.Id == 0)
                                    {
                                        if (imageExtensions.Any(x => x.Contains(fileExt)))
                                        {
                                            if (!IsImageExist)
                                            {
                                                formFile = item;
                                                model.BannerImage = libraryFile.FilePath;
                                                library.BannerImage = libraryFile.FilePath;
                                                IsImageExist = true;
                                                //if (library.LibraryTypeId == 1)
                                                //{
                                                //    CreatePortfolio(model, formFile);
                                                //}
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(libraryEntity.BannerImage))
                                        {
                                            if (!IsImageExist)
                                            {
                                                formFile = item;
                                                model.BannerImage = libraryFile.FilePath;
                                                library.BannerImage = libraryFile.FilePath;
                                                IsImageExist = true;
                                            }
                                        }
                                    }
                                }
                                else if (model.LibraryTypeId != (int)LibraryType.SalesKit && fileType != null)
                                {
                                    LibraryFile libraryFile = new LibraryFile
                                    {
                                        KeyId = UniqueFileId,
                                        FilePath = "Upload/LibraryFiles/" + fileName,
                                        LibraryFileTypeId = fileType.Id,
                                        AddDate = DateTime.Now,
                                        ModifyDate = DateTime.Now,
                                        SrNo = SrNO,
                                        FileId = "101",
                                    };
                                    library.LibraryFile.Add(libraryFile);
                                    string path = ContextProvider.HostEnvironment.WebRootPath + "/" + "Upload/LibraryFiles/";
                                    string FullPath = Path.Combine(path, fileName);

                                    //if (imageExtensions.Any(x => x.Contains(fileExt)))
                                    //{
                                    //    if (!IsImageExist)
                                    //    {
                                    //        entity.BannerImage = libraryFile.FilePath;
                                    //        library.BannerImage = libraryFile.FilePath;
                                    //        IsImageExist = true;
                                    //    }
                                    //}
                                    uploadImageToFolder(item, fileName, FullPath, path);

                                    libraryFileList.Add(libraryFile);
                                    if (model.Id == 0)
                                    {
                                        if (imageExtensions.Any(x => x.Contains(fileExt)))
                                        {
                                            if (!IsImageExist)
                                            {
                                                formFile = item;
                                                model.BannerImage = libraryFile.FilePath;
                                                library.BannerImage = libraryFile.FilePath;
                                                IsImageExist = true;
                                                //if (library.LibraryTypeId == 1)
                                                //{
                                                //    CreatePortfolio(model, formFile);
                                                //}
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(libraryEntity.BannerImage))
                                        {
                                            if (!IsImageExist)
                                            {
                                                formFile = item;
                                                model.BannerImage = libraryFile.FilePath;
                                                library.BannerImage = libraryFile.FilePath;
                                                IsImageExist = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    fileTypeError += fileExt + ",";
                                }
                            }
                            i++;
                        }
                    }
                    //else
                    //{
                    //    if (model.Id == 0)
                    //    {
                    //        if (library.LibraryTypeId == 1)
                    //        {
                    //            CreatePortfolio(model, null);
                    //        }
                    //    }
                    //}
                    if (libraryComponentFileList.Count > 0)
                    {
                        foreach (var item in libraryComponentFileList)
                        {
                            DeleteLibraryComponentFile(item.Id);
                        }

                    }
                    //if (model.ComponentFiles != null && model.ComponentFiles.Count > 0)
                    if (model.ComponentFiles != null)
                    {
                        //foreach (var item in model.ComponentFiles)
                        //{
                        var item = model.ComponentFiles;

                        Random rnd = new Random();
                        var UniqueFileId = Guid.NewGuid();
                        var timeWithMiliseconds = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                        //LibrarylaoutTypeName = libraryLayoutService.GetLibraryLayouts().Where(a => a.Id == item.DesignLayoutType).Select(z => z.Name).FirstOrDefault();
                        var CustomName = Regex.Replace(model.Title.Replace(' ', '-'), @"[^0-9a-zA-Z-]+", "") + "-" + timeWithMiliseconds;
                        string fileExt = Path.GetExtension(item.FileName.ToLower());
                        string fileName = $"{CustomName}{fileExt}";
                        int SrNO = serialNumberService.GetNumber() + 1;
                        serialNumberService.Save(new SerialNumber { Id = SrNO });
                        var fileType = libraryFileTypeService.GetLibraryFileTypes().Where(x => x.Extension.Contains(fileExt)).FirstOrDefault();
                        if (fileType != null)
                        {
                            LibraryComponentFile libraryFile = new LibraryComponentFile
                            {
                                KeyId = UniqueFileId,
                                FilePath = "Upload/LibraryComponentFiles/" + fileName,
                                LibraryComponentFileTypeId = fileType.Id,
                                AddDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                SrNo = SrNO,
                                FileId = "101",
                            };
                            library.LibraryComponentFile.Add(libraryFile);
                            string path = ContextProvider.HostEnvironment.WebRootPath + "/" + "Upload/LibraryComponentFiles/";
                            string FullPath = Path.Combine(path, fileName);

                            //if (imageExtensions.Any(x => x.Contains(fileExt)))
                            //{
                            //    if (!IsImageExist)
                            //    {
                            //        entity.BannerImage = libraryFile.FilePath;
                            //        library.BannerImage = libraryFile.FilePath;
                            //        IsImageExist = true;
                            //    }
                            //}
                            uploadImageToFolder(item, fileName, FullPath, path);

                            libraryComponentFileList.Add(libraryFile);

                        }
                        else
                        {
                            fileTypeError += fileExt + ",";
                        }

                    }

                    if (model.CoverImage != null)
                    {
                        if (libraryFileList != null && libraryFileList.Count > 0)
                        {
                            int i = 0;
                            foreach (var item in libraryFileList)
                            {
                                if (i == model.CoverImage)
                                {
                                    model.BannerImage = item.FilePath;
                                    library.BannerImage = item.FilePath;
                                }
                                i++;
                            }
                        }
                    }
                    else
                    {
                        if (!IsImageExist)
                        {
                            if (model.Id > 0)
                            {
                                if (libraryFileList != null && libraryFileList.Count > 0)
                                {
                                    foreach (var item in libraryFileList)
                                    {
                                        string fileExt = Path.GetExtension(item.FilePath.ToLower());
                                        if (imageExtensions.Any(x => x.Contains(fileExt)))
                                        {
                                            if (!IsImageExist)
                                            {
                                                model.BannerImage = item.FilePath;
                                                library.BannerImage = item.FilePath;
                                                IsImageExist = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (libraryEntity != null)
                    {
                        //if (libraryEntity.LibraryLayoutTypeMapping.Count > 0)
                        //{
                        //    libraryManagementService.LibraryLayoutDeleted(libraryEntity);
                        //}
                        libraryEntity.IsActive = model.IsActive;

                        if (libraryEntity.LibraryIndustry.Count > 0)
                        {
                            libraryManagementService.LibraryIndustryDeleted(libraryEntity);
                        }

                        if (libraryEntity.LibraryTechnology.Count > 0)
                        {
                            libraryManagementService.LibraryTechnologyDeleted(libraryEntity);
                        }

                        if (libraryEntity.LibraryComponent.Count > 0)
                        {
                            libraryManagementService.LibraryComponentDeleted(libraryEntity);
                        }

                        if (libraryEntity.LibraryTemplate.Count > 0)
                        {
                            libraryManagementService.LibraryTemplateDeleted(libraryEntity);
                        }
                    }
                    if (model.Id > 0)
                    {
                        var files = libraryEntity.LibraryFile.Where(x => x.PsdfilePath != null).ToList();
                        if (model.LibraryTypeId != (byte)Enums.LibraryType.Design)
                        {
                            if (files.Count > 0)
                            {
                                foreach (var item in files)
                                {
                                    DeleteLibraryFile(item.Id);
                                }
                            }
                        }
                        if (model.LibraryTypeId == (byte)Enums.LibraryType.Design)
                        {
                            var fileList = libraryFileService.GetLibraryFileOnLibraryId(libraryEntity.Id);
                            if (fileList != null && fileList.Count > 0)
                            {
                                libraryFileService.LibraryFileBulkDelete(fileList);
                            }
                        }
                    }

                    var emsLibrary = libraryManagementService.Save(library);

                    if (library.LibraryTypeId == 1 || library.LibraryTypeId == 3)
                    {
                        model.EMSLibraryId = emsLibrary.Id.ToString();
                        CreatePortfolio(model, formFile);
                    }
                    if (!string.IsNullOrEmpty(fileTypeError))
                    {
                        fileTypeError = fileTypeError.TrimEnd(',');
                        var item = fileTypeError.Split(',');
                        if (item.Length >= 2)
                        {
                            ShowWarningMessage("Warning", string.Format("Library saved successfully but {0} extensions are not allowed in the system so files with {0} extensions could not saved.", fileTypeError), false);
                        }
                        else
                        {
                            ShowWarningMessage("Warning", string.Format("Library saved successfully but {0} extension not allowed in the system so files with {0} extension could not saved.", fileTypeError), false);
                        }
                    }
                    else
                    {
                        if (model.LibraryTypeId == (int)LibraryType.SalesKit)
                        {
                            ShowSuccessMessage("Success", "Library saved successfully. but it would be released after management approval.", false);
                        }
                        else
                        {
                            ShowSuccessMessage("Success", "Library saved successfully", false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
                }
                //}
                //else
                //{
                //    ShowWarningMessage("Warning", "Library title already exist", false);
                //    return RedirectToAction("LibraryList");
                //}
            }
            else
            {
                return CreateModelStateErrors();
            }
            return RedirectToAction("LibraryList");
        }

        [NonAction]
        private bool uploadImageToFolder(IFormFile formFile, string ImageName, string fullPath, string path)
        {
            if (formFile != null && formFile.Length > 0)
            {
                Image img = null;
                var extension = Path.GetExtension(ImageName.ToLower());
                string FilePath = Path.Combine(localDirectory, ImageName);
                string tempDirectory = path + "/temp";
                string tempPath = Path.Combine(tempDirectory, ImageName);
                if (!Directory.Exists(tempDirectory))
                {
                    Directory.CreateDirectory(tempDirectory);
                }

                if (!Directory.Exists(localDirectory))
                {
                    Directory.CreateDirectory(localDirectory);
                }

                if (imageExtensions.Contains(extension))
                {
                    // save images with watermark
                    using (var fileStream = new FileStream(tempPath, FileMode.Create))
                    {
                        formFile.CopyTo(fileStream);
                    }
                    using (var fileStream = new FileStream(tempPath, FileMode.Open))
                    {
                        Stream outputStream = new MemoryStream();
                        AddWatermark(fileStream, "Dotsquares", outputStream);
                        img = Image.FromStream(outputStream);

                        using (Bitmap savingImage = new Bitmap(img.Width, img.Height, img.PixelFormat))
                        {
                            //string path = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/" + "Upload/LibraryFiles/", Guid.NewGuid().ToString()+ Path.GetExtension(ImageName));

                            using (Graphics g = Graphics.FromImage(savingImage))
                            {
                                g.DrawImage(img, new Point(0, 0));
                            }

                            savingImage.Save(fullPath, ImageFormat.Jpeg);

                        }
                        img.Dispose();
                    }
                    System.IO.File.Delete(tempPath);
                    // save original images
                    using (var fileStream = new FileStream(FilePath, FileMode.Create))
                    {
                        formFile.CopyTo(fileStream);
                    }
                    if (System.IO.File.Exists(FilePath) || System.IO.File.Exists(fullPath))
                    {
                        return true;
                    }
                }
                else
                {
                    using (var fileStream = new FileStream(FilePath, FileMode.Create))
                    {
                        formFile.CopyTo(fileStream);
                    }
                    if (System.IO.File.Exists(FilePath))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult DeleteLibraryFile(long id)
        {
            try
            {
                var downloadHistory = libraryDownloadHistoryService.GetDownloadHistory(id);
                if (downloadHistory != null && downloadHistory.Count > 0)
                {
                    libraryDownloadHistoryService.DeleteDownloadHistory(downloadHistory);
                }

                var response = libraryFileService.DeleteLibraryFile(id);
                return NewtonSoftJsonResult(response);
            }
            catch (Exception)
            {
                return NewtonSoftJsonResult(false);
            }
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult DeleteLibraryComponentFile(long id)
        {
            try
            {
                var response = libraryFileService.DeleteLibraryComponentFile(id);
                return NewtonSoftJsonResult(response);
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(false);
            }
        }

        [HttpGet]
        [CustomAuthorization(true)]
        public IActionResult LibraryDownloadPermission(int? id)
        {

            LibraryDownloadDto model = new LibraryDownloadDto();
            if (CurrentUser.RoleId == (int)UserRoles.PM)
            {
                int? Roleid = null;
                int? Userid = null;

                if (id != null && id != 0)
                {
                    var downloadpermission = libraryDownloadService.GetLibraryDownloadById(Convert.ToInt64(id));

                    //if (downloadpermission.UserLoginId != null && downloadpermission.UserLoginId != 0)
                    //{
                    //    ViewData["roleid"] = downloadpermission.UserLogin.RoleId;
                    //}
                    //else
                    //{
                    //    ViewData["roleid"] = downloadpermission.RoleId;
                    //}
                    //ViewData["roleid"] = downloadpermission.RoleId;
                    //ViewData["userid"] = downloadpermission.UserLoginId;

                    model.RoleId = Convert.ToInt32(downloadpermission.RoleId);
                    model.UserLoginId = downloadpermission.UserLoginId;

                }
                model.Roles = userLoginService.GetRoles()
                        .Select(x => new SelectListItem { Text = x.RoleName, Value = x.RoleId.ToString(), Selected = (x.RoleId == Roleid ? true : false) })
                        .ToList();
                model.Users = userLoginService.GetUsers()
                        .Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString(), Selected = (x.Uid == Userid ? true : false) })
                        .ToList();
                model.TypeList = libraryFileTypeService.GetLibraryFileTypes()
                    .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                    .ToList();

                return View(model);
            }
            else
            {
                return AccessDenied();
            }
        }

        [HttpPost]
        [CustomActionAuthorization]
        public async Task<IActionResult> LibraryDownloadPermission(LibraryDownloadDto model)
        {
            if (CurrentUser.RoleId == (int)UserRoles.PM)
            {
                try
                {
                    //if (ModelState.IsValid)
                    //{
                    var TypeList = libraryFileTypeService.GetLibraryFileTypes().ToList();
                    //var users = userLoginService.GetUserByRole(model.RoleId??0);
                    SaveDownloadPermission(TypeList, ref model);
                    if (model.Id > 0)
                    {
                        ShowSuccessMessage("Success", "Library Download Permission saved successfully", false);
                    }
                    else
                    {
                        ShowErrorMessage("Error", "Something went wrong..!!", false);
                    }
                    //}
                    //else
                    //{
                    //    return CreateModelStateErrors();
                    //}
                }
                catch (Exception ex)
                {
                    return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
                }
                ////   GetRoleDownloadPermission(int ? roleId, int ? userId)
                // return RedirectToAction("GetRoleDownloadPermission", new {model.id, roleId=model.RoleId, userId =model.UserLoginId});

                return RedirectToAction("LibraryDownloadPermission", new { id = model.Id });

            }
            else
            {
                return AccessDenied();
            }
        }

        public void SaveDownloadPermission(List<LibraryFileType> fileTypes, ref LibraryDownloadDto model)
        {
            LibraryDownloadPermission libraryDownload = null;
            if ((model.UserLoginId == null || model.UserLoginId == 0) && Convert.ToBoolean(Request.Form["forAllUserOfSelectedRole"]) == false)
            {
                for (int j = 0; j < fileTypes.Count; j++)
                {
                    libraryDownload = libraryDownloadService.GetLibraryDownloadByUserId(model.RoleId, fileTypes[j].Id, null);
                    int maxDownload = Convert.ToInt32(Request.Form["type" + j]);
                    int maxDownloadMonth = Convert.ToInt32(Request.Form["typeMonth" + j]);
                    if (libraryDownload == null)
                    {
                        libraryDownload = new LibraryDownloadPermission
                        {
                            RoleId = model.RoleId,
                            LibraryFileTypeId = fileTypes[j].Id,
                            UserLoginId = null,
                            MaximumDownloadInDay = maxDownload,
                            MaximumDownloadInMonth = maxDownloadMonth,

                            AddDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            AllowedDownloadBy = CurrentUser.Uid
                        };
                        libraryDownloadService.Save(libraryDownload);
                    }
                    else
                    {
                        libraryDownload.MaximumDownloadInDay = maxDownload;
                        libraryDownload.MaximumDownloadInMonth = maxDownloadMonth;
                        libraryDownload.UserLoginId = null;
                        libraryDownloadService.Save(libraryDownload);
                    }
                };
            }
            //else if ((model.UserLoginId == null || model.UserLoginId == 0) && Convert.ToBoolean(Request.Form["forAllUserOfSelectedRole"]) == true)
            //{
            //    for (int k = 0; k < users.Count; k++)
            //    {
            //        for (int j = 0; j < fileTypes.Count; j++)
            //        {

            //            var IsAlreadyExist = libraryDownloadService.GetLibraryDownloadByUserId(model.RoleId, fileTypes[j].Id, users[k].Uid);
            //            int maxDownload = Convert.ToInt32(Request.Form["type" + j]);
            //            int maxDownloadMonth = Convert.ToInt32(Request.Form["typeMonth" + j]);
            //            if (IsAlreadyExist == null)
            //            {
            //                LibraryDownloadPermission libraryDownload = new LibraryDownloadPermission
            //                {
            //                    RoleId = model.RoleId,
            //                    LibraryFileTypeId = fileTypes[j].Id,
            //                    UserLoginId = users[k].Uid,
            //                    MaximumDownloadInDay = maxDownload,
            //                    MaximumDownloadInMonth = maxDownloadMonth,
            //                    AddDate = DateTime.Now,
            //                    ModifyDate = DateTime.Now,
            //                    AllowedDownloadBy = CurrentUser.Uid
            //                };
            //                libraryDownloadService.Save(libraryDownload);
            //            }
            //            else
            //            {
            //                IsAlreadyExist.MaximumDownloadInDay = maxDownload;
            //                IsAlreadyExist.MaximumDownloadInMonth = maxDownloadMonth;
            //                libraryDownloadService.Save(IsAlreadyExist);
            //            }
            //        }
            //    }
            //}
            else
            {
                for (int j = 0; j < fileTypes.Count; j++)
                {
                    libraryDownload = libraryDownloadService.GetLibraryDownloadByUserId(null, fileTypes[j].Id, (int)model.UserLoginId);
                    int maxDownload = Convert.ToInt32(Request.Form["type" + j]);
                    int maxDownloadMonth = Convert.ToInt32(Request.Form["typeMonth" + j]);
                    if (libraryDownload == null)
                    {
                        libraryDownload = new LibraryDownloadPermission
                        {
                            RoleId = null, //model.RoleId,
                            LibraryFileTypeId = fileTypes[j].Id,
                            UserLoginId = (int)model.UserLoginId,
                            MaximumDownloadInDay = maxDownload,
                            MaximumDownloadInMonth = maxDownloadMonth,
                            AddDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            AllowedDownloadBy = CurrentUser.Uid
                        };
                        libraryDownloadService.Save(libraryDownload);
                    }
                    else
                    {
                        libraryDownload.RoleId = null;
                        libraryDownload.MaximumDownloadInDay = maxDownload;
                        libraryDownload.MaximumDownloadInMonth = maxDownloadMonth;
                        libraryDownloadService.Save(libraryDownload);
                    }
                }
            }
            model.Id = libraryDownload.Id;
        }
        [CustomActionAuthorization]
        public ActionResult Delete(int? id)
        {
            var uName = "";
            if (id > 0)
            {
                uName = libraryDownloadService.GetLibraryDownloadById(Convert.ToInt64(id))?.UserLogin.Name ?? "";
            }

            return PartialView("_ModalDelete", new Modal
            {
                Message = string.Format("Are you sure to delete \"{0}\" library download permission?", uName),
                Size = Enums.ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Library Permission" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult Delete(int id)
        {
            try
            {
                if (id > 0)
                {
                    var downloadpermission = libraryDownloadService.GetLibraryDownloadById(Convert.ToInt64(id));
                    if (downloadpermission != null)
                    {
                        var downloadpermissionList = libraryDownloadService.GetLibraryDownloadPermissions(downloadpermission.RoleId, downloadpermission.UserLoginId);

                        foreach (var item in downloadpermissionList)
                        {
                            libraryDownloadService.DeleteLibraryDownloadPermission(item);
                        }
                        ShowSuccessMessage("Success!", "Lead has been successfully deleted", false);
                    }
                }
                else
                {
                    ShowErrorMessage("Deletion Error", "Select Valid Record to delete..!!", false);
                }
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = ex.Message });
            }
            return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = true, RedirectUrl = Url.Action("ManageLibraryDownloadPermissionList", "LibraryManagement") });
        }

        public ActionResult DeleteLibraryDownloadPermission(int? id)
        {
            if (id != null && id != 0)
            {
                var downloadpermission = libraryDownloadService.GetLibraryDownloadById(Convert.ToInt64(id));
                var downloadpermissionList = libraryDownloadService.GetLibraryDownloadPermissions(downloadpermission.RoleId, downloadpermission.UserLoginId);

                foreach (var item in downloadpermissionList)
                {
                    libraryDownloadService.DeleteLibraryDownloadPermission(item);
                }
            }
            else
            {
                ShowErrorMessage("Deletion Error", "Select Valid Record to delete..!!", false);
            }
            return View("ManageLibraryDownloadPermissionList");
        }
        [HttpPost]
        public string GetRoleUsers(int roleId)
        {
            try
            {
                var response = userLoginService.GetUserByRole(roleId, true);
                response = (response.Count > 0) ? response.Where(x => x.PMUid == SiteKey.AshishTeamPMUId || x.Uid == SiteKey.AshishTeamPMUId).ToList() : response;
                var userLogins = response.Select(x => new { x.Uid, x.Name, x.UserName }).ToList();
                return JsonConvert.SerializeObject(new { data = userLogins });
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        [HttpPost]
        public string GetRoleDownloadPermission(int? roleId, int? userId)
        {
            try
            {
                var response = libraryDownloadService.GetLibraryDownloadPermissions(roleId, userId);
                var libraryDownloadPermissions = response.Select(x => new { x.Id, x.MaximumDownloadInDay, x.MaximumDownloadInMonth, x.LibraryFileTypeId }).ToList();
                return JsonConvert.SerializeObject(new { data = libraryDownloadPermissions });
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        [HttpPost]
        public string GetTechnologyByParent(int technologyParentId)
        {
            try
            {
                var techParents = technologyParentMappingService.GetTechnologyByParentId(technologyParentId);
                var technologies = techParents.Select(x => new { x.Technology.Title, x.Technology.TechId, x.TechnologyParentId }).ToList();
                return JsonConvert.SerializeObject(new { data = technologies });
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        [HttpPost]
        [CustomActionAuthorization]
        public async Task<IActionResult> DownloadPermission(int id)
        {
            try
            {
                LibraryFile libraryFile = libraryManagementService.GetLibraryFileById(id);

                if (libraryFile == null)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "file not available", IsSuccess = false });
                }
                //var libraryDownLoadPermission = libraryManagementService.GetLibraryFileTypeDownloadPermission(libraryFile);
                var libraryDownLoadPermission = libraryDownloadService.DownloadPermissionCount(CurrentUser.Uid, CurrentUser.RoleId, libraryFile.LibraryFileTypeId);
                var libraryDownLoadPermissionForMonth = libraryDownloadService.DownloadPermissionCountForMonth(CurrentUser.Uid, CurrentUser.RoleId, libraryFile.LibraryFileTypeId);

                if (libraryDownLoadPermission == null || libraryDownLoadPermissionForMonth == null)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "You don't have permission to download this file.", IsSuccess = false });
                }
                else if (libraryDownLoadPermissionForMonth <= libraryDownloadService.getDownLoadCountInMonth(CurrentUser.Uid, libraryFile.LibraryFileTypeId))
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Your max allowed download for a month is exhausted or you don't have permission of this file type.", IsSuccess = false });
                }
                else if (libraryDownLoadPermission <= libraryDownloadService.getDownLoadCount(CurrentUser.Uid, CurrentUser.RoleId, libraryFile.LibraryFileTypeId))
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Your max allowed download is exhausted or you don't have permission of this file type.", IsSuccess = false });
                }

                string fileName = libraryFile.FilePath.Substring(libraryFile.FilePath.LastIndexOf('/') + 1);
                var path = Path.Combine(localDirectory, fileName);
                if (!System.IO.File.Exists(path))
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "File not available.", IsSuccess = false });
                }
                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "", IsSuccess = true });


            }
            catch (Exception)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Something went wrong.", IsSuccess = false });
            }
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult DownloadComponentPermission(int id)
        {
            try
            {
                LibraryComponentFile libraryComponentFile = libraryManagementService.GetLibraryComponentFileById(id);
                if (libraryComponentFile != null)
                {
                    string componentfileName = libraryComponentFile.FilePath.Substring(libraryComponentFile.FilePath.LastIndexOf('/') + 1);
                    var filepath = Path.Combine(localDirectory, componentfileName);
                    if (!System.IO.File.Exists(filepath))
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "File not available.", IsSuccess = false });
                    }
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "", IsSuccess = true });
                }
                if (libraryComponentFile == null)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "file not available", IsSuccess = false });
                }
                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "", IsSuccess = true });
            }
            catch (Exception)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Something went wrong.", IsSuccess = false });
            }
        }

        [HttpGet]
        [CustomActionAuthorization]
        public async Task<IActionResult> Download(int id)
        {
            if (!HasDownloadPermission())
            {
                return AccessDenied();
            }
            LibraryFile libraryFile = libraryManagementService.GetLibraryFileById(id);
            string strLog = string.Empty;

            if (libraryFile == null)
            {
                strLog = $"file not available with id={id.ToString()}  dated on {DateTime.Now} {Environment.NewLine}";
                GeneralMethods.LibraryLogWriter("LibraryManagement", strLog);
                ShowErrorMessage("Error", "file not available", false);
                return RedirectToAction("Index");
            }
            var libraryDownLoadPermission = libraryDownloadService.DownloadPermissionCount(CurrentUser.Uid, CurrentUser.RoleId, libraryFile.LibraryFileTypeId);
            var libraryDownLoadPermissionForMonth = libraryDownloadService.DownloadPermissionCountForMonth(CurrentUser.Uid, CurrentUser.RoleId, libraryFile.LibraryFileTypeId);
            if (libraryDownLoadPermission == null || libraryDownLoadPermissionForMonth == null)
            {
                strLog = $"You don't have permission to download file with id={id.ToString()}  dated on {DateTime.Now} {Environment.NewLine}";
                GeneralMethods.LibraryLogWriter("LibraryManagement", strLog);
                ShowErrorMessage("Error", "You don't have permission to download file", false);
                return RedirectToAction("Index");

            }
            else if (libraryDownLoadPermissionForMonth <= libraryDownloadService.getDownLoadCountInMonth(CurrentUser.Uid, libraryFile.LibraryFileTypeId))
            {
                strLog = $"Your max allowed download for a month is exhausted dated on {DateTime.Now} {Environment.NewLine}";
                GeneralMethods.LibraryLogWriter("LibraryManagement", strLog);
                ShowErrorMessage("Error", "Your max allowed download for a month is exhausted", false);
                return RedirectToAction("Index");
            }
            else if (libraryDownLoadPermission <= libraryDownloadService.getDownLoadCount(CurrentUser.Uid, CurrentUser.RoleId, libraryFile.LibraryFileTypeId))
            {
                strLog = $"Your max allowed download is exhausted  dated on {DateTime.Now} {Environment.NewLine}";
                GeneralMethods.LibraryLogWriter("LibraryManagement", strLog);
                ShowErrorMessage("Error", "Your max allowed download is exhausted", false);
                return RedirectToAction("Index");
            }

            string fileName = (string.IsNullOrEmpty(libraryFile.PsdfilePath)) ? libraryFile.FilePath.Substring(libraryFile.FilePath.LastIndexOf('/') + 1)
                : libraryFile.PsdfilePath.Substring(libraryFile.PsdfilePath.LastIndexOf('/') + 1);

            var path = Path.Combine(localDirectory, fileName);
            if (!System.IO.File.Exists(path))
            {
                strLog = $"file doesn't exist with id={id.ToString()}  dated on {DateTime.Now} {Environment.NewLine}";
                GeneralMethods.LibraryLogWriter("LibraryManagement", strLog);
                ShowErrorMessage("Error", "file doesn't exist", false);
                return RedirectToAction("Index");
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);

            }
            memory.Position = 0;
            LibraryDownloadHistory libraryDownloadHistory = new LibraryDownloadHistory();

            libraryDownloadHistory.LibraryId = libraryFile.LibraryId;
            libraryDownloadHistory.LibraryFileTypeId = libraryFile.LibraryFileTypeId;
            libraryDownloadHistory.LibraryFileId = libraryFile.Id;
            libraryDownloadHistory.DownloadBy = CurrentUser.Uid;
            libraryDownloadHistory.DownloadOn = DateTime.Now;
            libraryDownloadHistory.Ip = ContextProvider.HttpContext.Connection.RemoteIpAddress.ToString();

            libraryDownloadHistoryService.Save(libraryDownloadHistory);
            string contentType = GetContentType(path);
            if (contentType == null)
            {
                strLog = $"Incorrect file format to download";
                GeneralMethods.LibraryLogWriter("LibraryManagement", strLog);
                ShowErrorMessage("Error", strLog, false);
                return RedirectToAction("Index");
            }
            return File(memory, contentType, Path.GetFileName(path));
        }

        [HttpGet]
        [CustomActionAuthorization]
        public async Task<IActionResult> DownloadComponent(int id)
        {

            LibraryComponentFile libraryComponentFile = libraryManagementService.GetLibraryComponentFileById(id);
            string strLog = string.Empty;

            if (libraryComponentFile == null)
            {
                strLog = $"file not available with id={id.ToString()}  dated on {DateTime.Now} {Environment.NewLine}";
                GeneralMethods.LibraryLogWriter("LibraryManagement", strLog);
                ShowErrorMessage("Error", "file not available", false);
                return RedirectToAction("Index");
            }

            string fileName = (string.IsNullOrEmpty(libraryComponentFile.PsdfilePath)) ? libraryComponentFile.FilePath.Substring(libraryComponentFile.FilePath.LastIndexOf('/') + 1)
                : libraryComponentFile.PsdfilePath.Substring(libraryComponentFile.PsdfilePath.LastIndexOf('/') + 1);

            var path = Path.Combine(localDirectory, fileName);
            if (!System.IO.File.Exists(path))
            {
                strLog = $"file doesn't exist with id={id.ToString()}  dated on {DateTime.Now} {Environment.NewLine}";
                GeneralMethods.LibraryLogWriter("LibraryManagement", strLog);
                ShowErrorMessage("Error", "file doesn't exist", false);
                return RedirectToAction("Index");
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);

            }
            memory.Position = 0;
            string contentType = GetContentType(path);
            if (contentType == null)
            {
                strLog = $"Incorrect file format to download";
                GeneralMethods.LibraryLogWriter("LibraryManagement", strLog);
                ShowErrorMessage("Error", strLog, false);
                return RedirectToAction("Index");
            }
            return File(memory, contentType, Path.GetFileName(path));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();

            if (types.ContainsKey(ext))
            {
                return types[ext];
            }
            else
            {
                return null;
            }

        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".ppt", "application/vnd.ms-powerpoint"},
                {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".csv", "text/csv"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".html","text/html" },
                {".psd","image/vnd.adobe.photoshop" },
                {".zip", "application/zip"},
                {".rar", "application/x-rar-compressed"}
            };
        }

        private void FillControlsDefault(LibraryManagementIndexDto libraryManagementIndexDto)
        {
            libraryManagementIndexDto.Technologies = technologyService.GetTechnologyList().OrderBy(t => t.Title).
                Select(d => new SelectListItem { Text = d.Title, Value = d.TechId.ToString() }).ToList();

            libraryManagementIndexDto.Industries = domainTypeService.GetDomainList().OrderBy(i => i.DomainName).
            Select(d => new SelectListItem { Text = d.DomainName, Value = d.DomainId.ToString() }).ToList();
            libraryManagementIndexDto.Featured = "";
            libraryManagementIndexDto.IsNda = "";
            libraryManagementIndexDto.IsReadyToUse = "";
            libraryManagementIndexDto.LibraryTypes = WebExtensions.GetSelectList<Enums.LibraryType>();
            libraryManagementIndexDto.LibraryTypes[0].Selected = true;
            libraryManagementIndexDto.DesignTypes = WebExtensions.GetSelectList<Enums.DesignType>();

            libraryManagementIndexDto.SalesKitTypes = GetSalesKitTypeSelectList();
            libraryManagementIndexDto.CvsTypes = cvsTypeService.GetCvsType().Select(n => new SelectListItem { Text = n.Name, Value = n.CvsId.ToString() }).ToList();

            libraryManagementIndexDto.Layouts = libraryLayoutService.GetLibraryLayouts().Where(x => x.LibraryLayoutTypeMapping.Count() > 0).
                OrderBy(l => l.Name).Select(d => new SelectListItem { Text = d.Name, Value = d.Id.ToString() }).ToList();
            libraryManagementIndexDto.Components = libraryComponentTypeService.GetLibraryComponentTypes().
                OrderBy(c => c.Name).Select(d => new SelectListItem { Text = d.Name, Value = d.Id.ToString() }).ToList();
            libraryManagementIndexDto.Templates = libraryTemplateTypeService.GetLibraryTemplateTypes().
                OrderBy(c => c.Name).Select(d => new SelectListItem { Text = d.Name, Value = d.Id.ToString() }).ToList();
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult AddComponent()
        {
            return PartialView("_AddEditComponent");
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult AddComponent(ComponentTypeDto componentTypeDto)
        {
            if (ModelState.IsValid)
            {
                LibraryComponentType IsComponentExist = null;
                IsComponentExist = libraryComponentTypeService.GetLibraryComponentTypes().Where(x => x.Name.ToLower().Trim() == componentTypeDto.Name.ToLower().Trim()).FirstOrDefault();
                if (IsComponentExist == null)
                {
                    LibraryComponentType libraryComponentType = new LibraryComponentType
                    {
                        Name = componentTypeDto.Name,
                        IsActive = true,
                        AddDate = DateTime.Now,
                        ModifyDate = DateTime.Now
                    };
                    var result = libraryComponentTypeService.Save(libraryComponentType);
                    if (result == null)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record not saved.", IsSuccess = false });
                    }
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record saved successfully", IsSuccess = true, Data = result.Id.ToString() });
                }
                else
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Component is already exist", IsSuccess = false });
                }
            }
            return CreateModelStateErrors();
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult AddLayoutType()
        {
            return PartialView("_AddEditLayoutType");
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult AddLayoutType(LibraryLayoutTypeDto layoutTypeDto)
        {
            if (ModelState.IsValid)
            {
                LibraryLayoutType IsLayoutExist = null;
                IsLayoutExist = libraryLayoutService.GetLibraryLayouts().Where(x => x.Name.ToLower().Trim() == layoutTypeDto.Name.ToLower().Trim()).FirstOrDefault();
                if (IsLayoutExist == null)
                {
                    LibraryLayoutType libraryLayoutType = new LibraryLayoutType
                    {
                        Name = layoutTypeDto.Name,
                        IsActive = true,
                        AddDate = DateTime.Now,
                        ModifyDate = DateTime.Now
                    };
                    var result = libraryLayoutService.Save(libraryLayoutType);
                    if (result == null)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record not saved.", IsSuccess = false });
                    }
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record saved successfully", IsSuccess = true, Data = result.Id.ToString() });
                }
                else
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Layout type is already exist", IsSuccess = false });
                }
            }
            return CreateModelStateErrors();
        }

        [CustomActionAuthorization]
        public ActionResult LibraryList()
        {
            //if (!HasDownloadPermission())
            //{
            //    return AccessDenied();
            //}

            ViewBag.DesignLayoutTypes = libraryLayoutService.GetLibraryLayouts().
                                                   Select(n => new SelectListItem
                                                   {
                                                       Text = n.Name,
                                                       Value = n.Id.ToString()
                                                   }).OrderBy(O => O.Text).ToList();

            ViewBag.LibraryComponentTypes = libraryComponentTypeService.GetLibraryComponentTypes().
                 Select(n => new SelectListItem
                 {
                     Text = n.Name,
                     Value = n.Id.ToString()
                 }).OrderBy(x => x.Text).ToList();


            return View();
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult ManageLibraryList(IDataTablesRequest request, int? userId, int DesignLayoutType, int LibraryComponentType)
        {
            try
            {
                var pagingServices = new PagingService<Library>(request.Start, request.Length);
                var filterExpr = PredicateBuilder.True<Library>();

                filterExpr = filterExpr.And(x => x.IsDeleted != true);

                if (CurrentUser.Uid != SiteKey.AshishTeamPMUId)
                    filterExpr = filterExpr.And(x => x.AddedBy == CurrentUser.Uid);

                if (!string.IsNullOrEmpty(request.Search.Value))
                {
                    var search = request.Search.Value.ToString().Trim().ToLower();
                    filterExpr = filterExpr.And(X => X.Title.Trim().ToLower().Contains(search));
                }


                //add filter for layout and component---------------- Working 

                if (LibraryComponentType != 0)
                {
                    filterExpr = filterExpr.And(X => X.LibraryComponent.Any(y => y.LibraryComponentTypeId == LibraryComponentType));
                }

                if (DesignLayoutType != 0)
                {
                    filterExpr = filterExpr.And(X => X.LibraryFile.Any(y => y.LibraryLayoutTypeId == DesignLayoutType));
                }
                //------------------------------------

                pagingServices.Filter = filterExpr;

                pagingServices.Sort = (o) =>
                {
                    return o.OrderByDescending(c => c.Id);
                };

                int totalCount = 0;
                var response = libraryManagementService.GetLibraryByPaging(out totalCount, pagingServices);
                var a = DataTablesJsonResult(totalCount, request, response.Select((d, index) =>
                {
                    var detail = new
                    {
                        rowIndex = (index + 1) + (request.Start),
                        keyId = d.KeyId,
                        title = d.Title.Trim().ToTitleCase(),
                        searchKeyword = d.SearchKeyword,
                        description = d.Description,
                        isNDA = d.IsNda.ToString().ToUpper(),
                        isFeatured = d.IsFeatured.ToString().ToUpper(),
                        isReadyToUse = d.IsReadyToUse.ToString().ToUpper(),
                        isLive = d.IsLive.ToString().ToUpper(),
                        libraryType = WebExtensions.GetSelectList<Enums.LibraryType>().Where(x => Convert.ToInt32(x.Value) == d.LibraryTypeId).FirstOrDefault().Text,
                        liveUrl = d.LiveUrl,
                        createdDate = d.LibraryCreatedDate?.Date.ToFormatDateString("MMM, dd yyyy hh:mm tt"),
                        createdBy = userLoginService.GetUserInfoByID(d.AddedBy).Name.ToTitleCase(),
                        isApproved = d.IsApproved.ToString().ToUpper(),
                        isActive = d.IsActive.ToString().ToUpper(),
                        file = d.LibraryFile.Count,
                        integrationHours = d.IntegrationHours.HasValue ? d.IntegrationHours : Convert.ToDecimal(0),
                        estimatedHours = d.EstimatedHours.HasValue ? d.EstimatedHours : Convert.ToDecimal(0),
                        reDevelopmentHours = d.ReDevelopmentHours.HasValue ? d.ReDevelopmentHours : Convert.ToDecimal(0),
                        downloadHistory = d.LibraryDownloadHistory.Count,
                        industry = (d.LibraryIndustry != null && d.LibraryIndustry.Count > 0) ? string.Join(", ", d.LibraryIndustry.Select(x => x.Industry.DomainName).ToList()) : "",
                        component = (d.LibraryComponent != null && d.LibraryComponent.Count > 0) ? string.Join(", ", d.LibraryComponent.Select(x => x.LibraryComponentType.Name).ToList()) : "",
                        technology = (d.LibraryTechnology != null && d.LibraryTechnology.Count > 0) ? string.Join(", ", d.LibraryTechnology.Select(x => x.Technology.Title).ToList()) : "",
                        mainImage = d.LibraryFile.Select(x => x.FilePath).FirstOrDefault(),
                    };
                    return detail;
                }));
                return a;
            }
            catch (Exception ex)
            {

            }
            return Ok();
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult ApprovedStatus(Guid? id)
        {
            if (id != null)
            {
                var library = libraryManagementService.GetLibraryByKeyId(id.Value);
                if (library != null)
                {
                    if (library.IsApproved == true)
                    {
                        library.IsApproved = false;
                    }
                    else
                    {
                        library.IsApproved = true;
                    }
                    libraryManagementService.Save(library);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ManageLibraryDownloadPermissionList()
        {

            return View();
        }

        [HttpPost]
        public IActionResult ManageLibraryDownloadPermissionList(IDataTablesRequest request, int? userId)
        {
            try
            {
                var pagingServicesForAll = new PagingService<LibraryDownloadPermission>(0, int.MaxValue);

                var filterExpr = PredicateBuilder.True<LibraryDownloadPermission>();
                //filterExpr = filterExpr.And(x => x.UserLogin.IsActive== true);
                filterExpr = filterExpr.And(x => x.UserLogin.IsActive == true || x.UserLoginId == null);
                if (request != null && !string.IsNullOrEmpty(request.Search.Value))
                {
                    var search = request.Search.Value.ToString().Trim().ToLower();
                    //filterExpr = filterExpr.And(X => X.UserLogin.Name.Trim().ToLower().Contains(search));
                    filterExpr = filterExpr.And(c => (c.RoleId != null ? c.Role.RoleName.Trim().ToLower().Contains(search) : c.UserLogin.Name.Trim().ToLower().Contains(search) || c.UserLogin.Role.RoleName.Trim().ToLower().Contains(search)));
                }

                pagingServicesForAll.Sort = (o) =>
                {
                    return o.OrderBy(c => (c.RoleId != null ? c.Role.RoleName : c.UserLogin.Role.RoleName)).ThenBy(c => c.UserLogin.Name);
                };

                pagingServicesForAll.Filter = filterExpr;


                int totalCount = 0;

                var allUserData = libraryDownloadService.GetLibraryDownloadPermissionByPaging(out totalCount, pagingServicesForAll);

                totalCount = allUserData.Select(x => new { x.RoleId, x.UserLoginId }).Distinct().Count();

                var pagingService = new PagingService<LibraryDownloadPermission>(request.Start, request.Length);

                var response = allUserData.Select(x => new { x.RoleId, x.UserLoginId }).Distinct().Skip((pagingService.Start - 1) * pagingService.Length).Take(pagingService.Length).ToList();

                //var response2 = allUserData.Where(x => x.RoleId == null ).Select(x => new { x.UserLogin.RoleId, x.UserLoginId }).Distinct().Skip((pagingService.Start - 1) * pagingService.Length).Take(pagingService.Length).ToList();

                var fileTypes = libraryFileTypeService.GetLibraryFileTypes().Select(x => new { x.Id, x.Name }).ToList();

                var data = response.Select((d, index) => new
                {
                    id = allUserData.Where(x => x.RoleId == d.RoleId && x.UserLoginId == d.UserLoginId).FirstOrDefault().Id,
                    rowIndex = (index + 1), // + (request.Start),
                    RoleId = d.RoleId,
                    RoleName = (d.RoleId != null ? allUserData.Where(x => x.RoleId == d.RoleId).FirstOrDefault().Role?.RoleName : allUserData.Where(x => x.UserLoginId == d.UserLoginId).FirstOrDefault().UserLogin?.Role?.RoleName),
                    UserLoginId = d.UserLoginId,
                    UserName = (d.UserLoginId != null ? allUserData.Where(x => x.RoleId == d.RoleId && x.UserLoginId == d.UserLoginId).FirstOrDefault().UserLogin.Name : ""),
                    // FileTypes= libraryFileTypeService.GetLibraryFileTypes(),
                    LibraryFileTypeList = allUserData.Where(x => x.RoleId == d.RoleId && x.UserLoginId == d.UserLoginId).Select((l, indexL) => new LibraryFileTypeIdDto()
                    {
                        LibraryFileTypeId = l.LibraryFileTypeId,
                        LibraryFileTypeName = l.LibraryFileType.Name,
                        MaximumDownloadInDay = l.MaximumDownloadInDay != null ? l.MaximumDownloadInDay.ToString() : "",
                        MaximumDownloadInMonth = l.MaximumDownloadInMonth != null ? l.MaximumDownloadInMonth.ToString() : ""

                    }).ToList(),
                    fileTypes

                });

                //var fileTypes = libraryFileTypeService.GetLibraryFileTypes().Select(x=> new { x.Id,x.Name }).ToList();
                //IDictionary<string, object> additionalParameters = new Dictionary<string, object>();
                ////ArrayList arr = new ArrayList();
                //foreach(var item in fileTypes)
                //{
                // //arr.Add(item.Name);
                // additionalParameters.Add(new KeyValuePair<string, object>(item.Id.ToString(), item.Name));
                // //additionalParameters.Add(new KeyValuePair<string, object>("fileTypes0", fileTypes.Select(x=> new { x.Id,x.Name })));

                //}
                ////IDictionary<string, object> addpara =new Dictionary<string, object>();
                ////addpara.Add(new KeyValuePair<string, object>("fileTypes0", arr));
                var result = DataTablesJsonResult(totalCount, request, data);

                return result;
            }
            catch (Exception ex)
            {

            }
            return Ok();
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult ImportLibrary()
        {
            return PartialView("_ImportLibrary");
        }

        [HttpPost]
        [CustomActionAuthorization]
        public async Task<IActionResult> ImportLibrary(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return MessagePartialView("File Not Selected");
            }

            try
            {
                string dirpath = Directory.GetCurrentDirectory();
                string fileExtension = Path.GetExtension(file.FileName.ToLower());
                string fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{DateTime.Now.Ticks}{fileExtension}";
                string path = ContextProvider.HostEnvironment.WebRootPath + "/" + "Upload/LibraryFiles/";
                string fullPath = Path.Combine(path, fileName);
                uploadImageToFolder(file, fileName, fullPath, path);
                List<int> SkipRows = new List<int>();
                List<string> errorLog = new List<string>();
                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    var filePath = Path.Combine(dirpath + "\\wwwroot\\Upload\\LibraryFiles", fileName);
                    var fileLocation = new FileInfo(filePath);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    if (file.Length <= 0)
                    {
                        return BadRequest("File not found");
                    }

                    using (ExcelPackage package = new ExcelPackage(fileLocation))
                    {
                        ExcelWorksheet workSheet = package.Workbook.Worksheets["Sheet1"];
                        int totalRows = workSheet.Dimension.Rows;
                        var DataList = new List<Library>();
                        Library library = null;
                        for (int i = 2; i <= totalRows; i++)
                        {
                            var current = "0";
                            if (workSheet.Cells[i, 16].Value == null)
                            {
                                continue;
                            }
                            else
                            {
                                current = workSheet.Cells[i, 16].Value.ToString();
                            }

                            var previous = workSheet.Cells[i - 1, 16].Value.ToString();
                            var next = (workSheet.Cells[i + 1, 16].Value == null) ? "0" : workSheet.Cells[i + 1, 16].Value.ToString();
                            // For single record
                            if (current != next && current != previous)
                            {
                                library = null;
                                bool response = AddRowsToImport(workSheet, i, fileName, ref library, DataList);
                                if (response)
                                {
                                    DataList.Add(library);
                                }
                                continue;
                            }
                            else if (current != previous && current == next)
                            {
                                library = null;
                                bool response = AddRowsToImport(workSheet, i, fileName, ref library, DataList);
                                continue;
                            }
                            else if (current == previous && current != next)
                            {
                                if (library != null)
                                {
                                    bool response = AddRowsToImport(workSheet, i, fileName, ref library, DataList);
                                    if (response)
                                    {
                                        DataList.Add(library);
                                    }
                                    continue;
                                }
                                else
                                {
                                    bool response = AddRowsToImport(workSheet, i, fileName, ref library, DataList);
                                    if (response)
                                    {
                                        DataList.Add(library);
                                    }
                                    continue;
                                }
                            }
                            else if (current == previous && current != next)
                            {
                                AddRowsToImport(workSheet, i, fileName, ref library, DataList);
                                continue;
                            }
                            else if (current == previous && current == next)
                            {
                                AddRowsToImport(workSheet, i, fileName, ref library, DataList);
                                continue;
                            }
                            else
                            {
                                WriteImportLibraryFileLog(workSheet, i, "Something went wrong...", fileName, i - 1);
                                continue;
                            }
                        }

                        libraryManagementService.SaveCollection(DataList);
                        int insertedTotalRecords = (totalRows - 1) - failedTotalRecords;
                        if (IsLibraryImportLogCreated)
                        {
                            ShowErrorMessage("Error", $"{insertedTotalRecords} records inserted, {failedTotalRecords} records failed, <a style='text-decoration: underline; color:blue;' target='_blank' href='/upload/logs/LibraryImportLog.txt'>Click here to open error log file</a>.", false);
                        }
                        else
                        {
                            ShowSuccessMessage("Success", $"Library import successfully, {insertedTotalRecords} records inserted.", false);
                        }

                        return RedirectToAction("LibraryList");
                    }
                }
            }
            catch (Exception ex)
            {
                string strLog = string.Empty;
                strLog += "#################################################" + Environment.NewLine;
                strLog += "\b Error On: " + DateTime.Now + "" + Environment.NewLine;
                strLog += ex.ToString();
                strLog += "#################################################" + Environment.NewLine;
                GeneralMethods.LibraryLogWriter("LibraryImportErrorLog", strLog);
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
            return Ok();
        }

        private bool AddRowsToImport(ExcelWorksheet workSheet, int i, string fileName, ref Library library, List<Library> DataList)
        {
            try
            {
                int? Layout = null;
                List<int> Industry = new List<int>();
                List<int> Technology = new List<int>();
                List<int> Component = new List<int>();
                List<LibraryFile> LibraryFile = new List<LibraryFile>();
                string LibraryTypeId = string.Empty;
                string DesignTypeId = string.Empty;
                string OtherIndustry = null;
                string ImageFile = null;
                string PsdFile = null;
                string ZipFile = null;
                string fileExt = null;
                LibraryFile libraryFile = null;

                if (library == null)
                {
                    library = new Library();
                    if (workSheet.Cells[i, 3].Value == null)
                    {
                        WriteImportLibraryFileLog(workSheet, i, "Library title is required", fileName, i - 1);
                        return false;
                    }
                    else if (workSheet.Cells[i, 4].Value == null)
                    {
                        WriteImportLibraryFileLog(workSheet, i, "Library type is required", fileName, i - 1);
                        return false;
                    }

                    if (WebExtensions.GetSelectList<Enums.LibraryType>().
                        Where(x => x.Text == workSheet.Cells[i, 4].Value.ToString()).Any())
                    {
                        LibraryTypeId = WebExtensions.GetSelectList<Enums.LibraryType>().
                            Where(x => x.Text == workSheet.Cells[i, 4].Value.ToString()).FirstOrDefault().Value;
                        if (LibraryTypeId == "2")
                        {
                            if (workSheet.Cells[i, 7].Value == null)
                            {
                                WriteImportLibraryFileLog(workSheet, i, "Design type is required", fileName, i - 1);
                                return false;
                            }
                            if (WebExtensions.GetSelectList<Enums.DesignType>().
                                Where(x => x.Text == workSheet.Cells[i, 7].Value.ToString()).Any())
                            {
                                DesignTypeId = WebExtensions.GetSelectList<Enums.DesignType>().
                                    Where(x => x.Text == workSheet.Cells[i, 7].Value.ToString()).FirstOrDefault().Value;
                            }
                            else
                            {
                                WriteImportLibraryFileLog(workSheet, i, "Design type not found: " + workSheet.Cells[i, 7].Value.ToString(), fileName, i - 1);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        WriteImportLibraryFileLog(workSheet, i, "Library Type not found: " + workSheet.Cells[i, 4].Value.ToString(), fileName, i - 1);
                        return false;
                    }
                    /*
                    * LayoutType
                    */
                    if (workSheet.Cells[i, 8].Value != null)
                    {
                        var item = workSheet.Cells[i, 8].Value.ToString();
                        var SingleLayout = libraryLayoutService.GetLibraryLayouts().
                            FirstOrDefault(x => x.Name.Contains(item.Trim()));
                        if (SingleLayout != null)
                        {
                            Layout = SingleLayout.Id;
                        }
                        else
                        {
                            LibraryLayoutType libraryLayoutType = new LibraryLayoutType
                            {
                                Name = item.Trim(),
                                IsActive = true,
                                AddDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };
                            var result = libraryLayoutService.Save(libraryLayoutType);
                            if (result != null)
                            {
                                Layout = result.Id;
                            }
                        }
                    }
                    /*
                     * Industry
                     */
                    if (workSheet.Cells[i, 5].Value != null)
                    {
                        var item = workSheet.Cells[i, 5].Value.ToString();
                        if (!string.IsNullOrWhiteSpace(item.Trim()))
                        {
                            var SingleIndustry = domainTypeService.GetDomainList().
                                FirstOrDefault(x => x.DomainName.Contains(item.Trim()));
                            if (SingleIndustry != null)
                            {
                                Industry.Add(SingleIndustry.DomainId);
                            }
                            else
                            {
                                OtherIndustry = item.Trim();
                            }
                        }
                    }
                    /*
                     * Add Image, PSD and Zip file to Library File
                     */
                    if (workSheet.Cells[i, 13].Value != null)
                    {
                        var item = workSheet.Cells[i, 13].Value.ToString().Trim();
                        ImageFile = "Upload/LibraryFiles/" + item.Split('\\').Last();
                    }
                    if (workSheet.Cells[i, 14].Value != null)
                    {
                        var item = workSheet.Cells[i, 14].Value.ToString().Trim();
                        PsdFile = "Upload/LibraryFiles/" + item.Split('\\').Last();
                    }
                    if (workSheet.Cells[i, 15].Value != null)
                    {
                        var item = workSheet.Cells[i, 15].Value.ToString().Trim();
                        ZipFile = "Upload/LibraryFiles/" + item.Split('\\').Last();
                    }

                    /*
                     * Create New Library object
                     */
                    try
                    {
                        string dateNow = DateTime.Now.ToString("MM/dd/yyyy");
                        library = new Library
                        {
                            Title = workSheet.Cells[i, 3].Value.ToString().Trim(),
                            LibraryTypeId = Convert.ToByte(LibraryTypeId),
                            SearchKeyword = workSheet.Cells[i, 9].Value != null ? workSheet.Cells[i, 9].Value.ToString().Trim() : "",
                            OtherIndustry = OtherIndustry,
                            AddedBy = CurrentUser.Uid,
                            ModifyByUid = CurrentUser.Uid,
                            DesignTypeId = !string.IsNullOrWhiteSpace(DesignTypeId) ? Convert.ToByte(DesignTypeId) : (byte?)null
                        };
                    }
                    catch (Exception ex)
                    {

                    }

                    library.IsFeatured = false;
                    library.IsLive = false;


                    // For Image
                    if (!string.IsNullOrWhiteSpace(ImageFile))
                    {
                        libraryFile = new LibraryFile();
                        var UniqueFileId = Guid.NewGuid();
                        fileExt = Path.GetExtension(ImageFile).ToLower();
                        int SrNO = serialNumberService.GetNumber() + 1;
                        var fileType = libraryFileTypeService.GetLibraryFileTypes().Where(x => x.Extension.Contains(fileExt) && x.Name.ToLower() == "image").FirstOrDefault();
                        if (fileType != null)
                        {
                            serialNumberService.Save(new SerialNumber { Id = SrNO });
                            libraryFile.KeyId = UniqueFileId;
                            libraryFile.FilePath = ImageFile;
                            libraryFile.LibraryFileTypeId = fileType.Id;
                            libraryFile.SrNo = SrNO;
                            libraryFile.FileId = "101";
                            libraryFile.LibraryLayoutTypeId = Layout;
                        }
                        else
                        {
                            WriteImportLibraryFileLog(workSheet, i, "Image file type is not in the list: " + workSheet.Cells[i, 13].Value.ToString(), fileName, i - 1);
                        }
                    }
                    /*
                     * For PSD File
                     */
                    if (!string.IsNullOrWhiteSpace(PsdFile))
                    {
                        if (libraryFile == null)
                        {
                            libraryFile = new LibraryFile();
                        }

                        var UniqueFileId = Guid.NewGuid();
                        fileExt = Path.GetExtension(PsdFile).ToLower();
                        int SrNO = serialNumberService.GetNumber() + 1;
                        var fileType = libraryFileTypeService.GetLibraryFileTypes().Where(x => x.Extension.Contains(fileExt) && x.Name.ToLower() == "psd").FirstOrDefault();
                        if (fileType != null)
                        {
                            if (libraryFile == null)
                            {
                                serialNumberService.Save(new SerialNumber { Id = SrNO });
                                libraryFile.KeyId = UniqueFileId;
                                libraryFile.FilePath = PsdFile;
                                libraryFile.LibraryFileTypeId = fileType.Id;
                                libraryFile.SrNo = SrNO;
                                libraryFile.FileId = "101";
                                libraryFile.LibraryLayoutTypeId = Layout;
                            }
                            else
                            {
                                libraryFile.PsdfilePath = PsdFile;
                            }
                        }
                        else
                        {
                            WriteImportLibraryFileLog(workSheet, i, "PSD Image file type is not in the list: " + workSheet.Cells[i, 14].Value.ToString(), fileName, i - 1);
                        }
                    }
                    if (libraryFile != null)
                    {
                        library.LibraryFile.Add(libraryFile);
                    }
                    // For Zip
                    if (!string.IsNullOrWhiteSpace(ZipFile))
                    {
                        var UniqueFileId = Guid.NewGuid();
                        fileExt = Path.GetExtension(ZipFile).ToLower();
                        int SrNO = serialNumberService.GetNumber() + 1;
                        var fileType = libraryFileTypeService.GetLibraryFileTypes().Where(x => x.Extension.Contains(fileExt) && x.Name.ToLower() == "zip").FirstOrDefault();
                        if (fileType != null)
                        {
                            var IsFileExist = library.LibraryFile.Where(x => x.FilePath == ZipFile).FirstOrDefault();
                            if (IsFileExist == null)
                            {
                                LibraryFile zipLibraryFile = new LibraryFile();
                                serialNumberService.Save(new SerialNumber { Id = SrNO });
                                zipLibraryFile.KeyId = UniqueFileId;
                                zipLibraryFile.FilePath = ZipFile;
                                zipLibraryFile.LibraryFileTypeId = fileType.Id;
                                zipLibraryFile.SrNo = SrNO;
                                zipLibraryFile.FileId = "101";
                                zipLibraryFile.LibraryLayoutTypeId = null;
                                library.LibraryFile.Add(zipLibraryFile);
                            }
                        }
                        else
                        {
                            WriteImportLibraryFileLog(workSheet, i, "Zip file type is not in the list: " + workSheet.Cells[i, 15].Value.ToString(), fileName, i - 1);
                        }
                    }

                    if (workSheet.Cells[i, 10].Value == null)
                    {
                        library.Description = "";
                    }
                    else
                    {
                        library.Description = workSheet.Cells[i, 10].Value.ToString().Trim();
                    }

                    if (workSheet.Cells[i, 2].Value == null)
                    {
                        library.CRMUserId = null;
                    }
                    else
                    {
                        library.CRMUserId = Convert.ToInt32(workSheet.Cells[i, 2].Value.ToString());
                    }

                    if (workSheet.Cells[i, 11].Value == null)
                    {
                        library.IsNda = false;
                    }
                    else
                    {
                        library.IsNda = (workSheet.Cells[i, 11].Value.ToString().ToLower() == "yes") ? true : false;
                    }

                    if (string.IsNullOrWhiteSpace(library.Title))
                    {
                        WriteImportLibraryFileLog(workSheet, i, "Incorrect library title", fileName, i - 1);
                        return false;
                    }
                    //else if (string.IsNullOrWhiteSpace(library.Description))
                    //{
                    //    WriteImportLibraryFileLog(workSheet, i, "Incorrect library description", fileName, i - 1);
                    //    return false;
                    //}
                    //else if (string.IsNullOrWhiteSpace(library.IsNda.ToString())
                    //    || new List<string>() { "true", "false" }.Contains(library.IsNda.ToString()))
                    //{
                    //    WriteImportLibraryFileLog(workSheet, i, "Incorrect NDA value", fileName, i - 1);
                    //    return false;
                    //}
                    //else if (string.IsNullOrWhiteSpace(library.IsFeatured.ToString())
                    //    || new List<string>() { "true", "false" }.Contains(library.IsFeatured.ToString()))
                    //{
                    //    WriteImportLibraryFileLog(workSheet, i, "Incorrect featured value", fileName, i - 1);
                    //    return false;
                    //}
                    //else if (string.IsNullOrWhiteSpace(library.IsLive.ToString())
                    //    || new List<string>() { "true", "false" }.Contains(library.IsLive.ToString()))
                    //{
                    //    WriteImportLibraryFileLog(workSheet, i, "Incorrect live value", fileName, i - 1);
                    //    return false;
                    //}

                    if (Industry != null && Industry.Count > 0)
                    {
                        foreach (var item in Industry)
                        {
                            var IsIndustryExist = library.LibraryIndustry.Where(x => x.IndustryId == Convert.ToInt32(item)).FirstOrDefault();
                            if (IsIndustryExist == null)
                            {
                                LibraryIndustry industry = new LibraryIndustry
                                {
                                    IndustryId = Convert.ToInt32(item),
                                };
                                library.LibraryIndustry.Add(industry);
                            }
                        }
                    }

                    if (Layout != null)
                    {
                        var IsLayoutExist = library.LibraryLayoutTypeMapping.Where(x => x.LibraryLayoutTypeId == Convert.ToInt32(Layout)).FirstOrDefault();
                        if (IsLayoutExist == null)
                        {
                            LibraryLayoutTypeMapping libraryLayout = new LibraryLayoutTypeMapping
                            {
                                LibraryLayoutTypeId = Convert.ToInt32(Layout),
                            };
                            library.LibraryLayoutTypeMapping.Add(libraryLayout);
                        }
                    }
                    return true;
                }
                else
                {
                    /*
                     * Add Image, PSD and Zip file to Library File
                     */
                    if (workSheet.Cells[i, 13].Value != null)
                    {
                        var item = workSheet.Cells[i, 13].Value.ToString().Trim();
                        ImageFile = "Upload/LibraryFiles/" + item.Split('\\').Last();
                    }
                    if (workSheet.Cells[i, 14].Value != null)
                    {
                        var item = workSheet.Cells[i, 14].Value.ToString().Trim();
                        PsdFile = "Upload/LibraryFiles/" + item.Split('\\').Last();
                    }
                    if (workSheet.Cells[i, 15].Value != null)
                    {
                        var item = workSheet.Cells[i, 15].Value.ToString().Trim();
                        ZipFile = "Upload/LibraryFiles/" + item.Split('\\').Last();
                    }
                    /*
                    * LayoutType
                    */
                    if (workSheet.Cells[i, 8].Value != null)
                    {
                        var item = workSheet.Cells[i, 8].Value.ToString();
                        var SingleLayout = libraryLayoutService.GetLibraryLayouts().
                            FirstOrDefault(x => x.Name.Contains(item.Trim()));
                        if (SingleLayout != null)
                        {
                            Layout = SingleLayout.Id;
                        }
                        else
                        {
                            LibraryLayoutType libraryLayoutType = new LibraryLayoutType
                            {
                                Name = item.Trim(),
                                IsActive = true,
                            };
                            var result = libraryLayoutService.Save(libraryLayoutType);
                            if (result != null)
                            {
                                Layout = result.Id;
                            }
                        }
                    }

                    // For Image
                    if (!string.IsNullOrWhiteSpace(ImageFile))
                    {
                        libraryFile = new LibraryFile();
                        var UniqueFileId = Guid.NewGuid();
                        fileExt = Path.GetExtension(ImageFile).ToLower();
                        int SrNO = serialNumberService.GetNumber() + 1;
                        var fileType = libraryFileTypeService.GetLibraryFileTypes().Where(x => x.Extension.Contains(fileExt) && x.Name.ToLower() == "image").FirstOrDefault();
                        if (fileType != null)
                        {
                            serialNumberService.Save(new SerialNumber { Id = SrNO });
                            libraryFile.KeyId = UniqueFileId;
                            libraryFile.FilePath = ImageFile;
                            libraryFile.LibraryFileTypeId = fileType.Id;
                            libraryFile.SrNo = SrNO;
                            libraryFile.FileId = "101";
                            libraryFile.LibraryLayoutTypeId = Layout;
                        }
                        else
                        {
                            WriteImportLibraryFileLog(workSheet, i, "Image file type is not in the list: " + workSheet.Cells[i, 13].Value.ToString(), fileName, i - 1);
                        }
                    }
                    /*
                     * For PSD File
                     */
                    if (!string.IsNullOrWhiteSpace(PsdFile))
                    {
                        if (libraryFile == null)
                        {
                            libraryFile = new LibraryFile();
                        }

                        var UniqueFileId = Guid.NewGuid();
                        fileExt = Path.GetExtension(PsdFile).ToLower();
                        int SrNO = serialNumberService.GetNumber() + 1;
                        var fileType = libraryFileTypeService.GetLibraryFileTypes().Where(x => x.Extension.Contains(fileExt) && x.Name.ToLower() == "psd").FirstOrDefault();
                        if (fileType != null)
                        {
                            if (libraryFile == null)
                            {
                                serialNumberService.Save(new SerialNumber { Id = SrNO });
                                libraryFile.KeyId = UniqueFileId;
                                libraryFile.FilePath = PsdFile;
                                libraryFile.LibraryFileTypeId = fileType.Id;
                                libraryFile.SrNo = SrNO;
                                libraryFile.FileId = "101";
                                libraryFile.LibraryLayoutTypeId = Layout;
                            }
                            else
                            {
                                libraryFile.PsdfilePath = PsdFile;
                            }
                        }
                        else
                        {
                            WriteImportLibraryFileLog(workSheet, i, "PSD Image file type is not in the list: " + workSheet.Cells[i, 14].Value.ToString(), fileName, i - 1);
                        }
                    }
                    if (libraryFile != null)
                    {
                        library.LibraryFile.Add(libraryFile);
                    }
                    // For Zip
                    if (!string.IsNullOrWhiteSpace(ZipFile))
                    {
                        var UniqueFileId = Guid.NewGuid();
                        fileExt = Path.GetExtension(ZipFile).ToLower();
                        int SrNO = serialNumberService.GetNumber() + 1;
                        var fileType = libraryFileTypeService.GetLibraryFileTypes().Where(x => x.Extension.Contains(fileExt) && x.Name.ToLower() == "zip").FirstOrDefault();
                        if (fileType != null)
                        {
                            var IsFileExist = library.LibraryFile.Where(x => x.FilePath == ZipFile).FirstOrDefault();
                            if (IsFileExist == null)
                            {
                                LibraryFile zipLibraryFile = new LibraryFile();
                                serialNumberService.Save(new SerialNumber { Id = SrNO });
                                zipLibraryFile.KeyId = UniqueFileId;
                                zipLibraryFile.FilePath = ZipFile;
                                zipLibraryFile.LibraryFileTypeId = fileType.Id;
                                zipLibraryFile.SrNo = SrNO;
                                zipLibraryFile.FileId = "101";
                                zipLibraryFile.LibraryLayoutTypeId = null;
                                library.LibraryFile.Add(zipLibraryFile);
                            }
                        }
                        else
                        {
                            WriteImportLibraryFileLog(workSheet, i, "Zip file type is not in the list: " + workSheet.Cells[i, 15].Value.ToString(), fileName, i - 1);
                        }
                    }
                    /*
                     * Industry
                     */
                    if (workSheet.Cells[i, 5].Value != null)
                    {
                        var item = workSheet.Cells[i, 5].Value.ToString();
                        if (!string.IsNullOrWhiteSpace(item.Trim()))
                        {
                            var SingleIndustry = domainTypeService.GetDomainList().
                                FirstOrDefault(x => x.DomainName.Contains(item.Trim()));
                            if (SingleIndustry != null)
                            {
                                Industry.Add(SingleIndustry.DomainId);
                            }
                            else
                            {
                                library.OtherIndustry = (string.IsNullOrWhiteSpace(library.OtherIndustry))
                                    ? item.Trim() : ((library.OtherIndustry != item.Trim()) ? library.OtherIndustry + ", " + item.Trim() : library.OtherIndustry);
                            }
                        }
                    }

                    if (Industry != null && Industry.Count > 0)
                    {
                        foreach (var item in Industry)
                        {
                            var IsIndustryExist = library.LibraryIndustry.Where(x => x.IndustryId == Convert.ToInt32(item)).FirstOrDefault();
                            if (IsIndustryExist == null)
                            {
                                LibraryIndustry industry = new LibraryIndustry
                                {
                                    IndustryId = Convert.ToInt32(item),
                                };
                                library.LibraryIndustry.Add(industry);
                            }
                        }
                    }

                    if (Layout != null)
                    {
                        var IsLayoutExist = library.LibraryLayoutTypeMapping.Where(x => x.LibraryLayoutTypeId == Convert.ToInt32(Layout)).FirstOrDefault();
                        if (IsLayoutExist == null)
                        {
                            LibraryLayoutTypeMapping libraryLayout = new LibraryLayoutTypeMapping
                            {
                                LibraryLayoutTypeId = Convert.ToInt32(Layout),
                            };
                            library.LibraryLayoutTypeMapping.Add(libraryLayout);
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                string strLog = string.Empty;
                strLog += "#################################################" + Environment.NewLine;
                strLog += "\b Error On: " + DateTime.Now + "" + Environment.NewLine;
                strLog += ex.ToString();
                strLog += "#################################################" + Environment.NewLine;
                GeneralMethods.LibraryLogWriter("LibraryImportErrorLog", strLog);
                return false;
            }
        }

        [NonAction]
        public void WriteImportLibraryFileLog(ExcelWorksheet workSheet, int i, string errorMessage, string fileName, int row)
        {
            failedTotalRecords++;
            IsLibraryImportLogCreated = true;
            string strLog = string.Empty;
            string LibraryJson = JsonConvert.SerializeObject(new
            {
                CrmId = workSheet.Cells[i, 2].Value,
                ProjectTitle = workSheet.Cells[i, 3].Value,
                LibraryType = workSheet.Cells[i, 4].Value,
                Industry = workSheet.Cells[i, 5].Value,
                Technology = workSheet.Cells[i, 6].Value,
                DesignType = workSheet.Cells[i, 7].Value,
                LayoutType = workSheet.Cells[i, 8].Value,
                SearchKeyword = workSheet.Cells[i, 9].Value,
                Description = workSheet.Cells[i, 10].Value,
                IsNDA = workSheet.Cells[i, 11].Value,
                IsFeatured = workSheet.Cells[i, 12].Value,
                IsLive = workSheet.Cells[i, 13].Value,
                LiveUrl = workSheet.Cells[i, 14].Value,
                Component = workSheet.Cells[i, 15].Value,
            });

            strLog += "#################################################" + Environment.NewLine;
            strLog += "\b Error On: " + DateTime.Now + "" + Environment.NewLine
                + "Message: " + errorMessage + Environment.NewLine +
                "File Name: " + fileName + Environment.NewLine
                + "Skipped Row: " + row + Environment.NewLine;
            strLog += LibraryJson + "\t";
            string res = string.Empty;
            strLog += Environment.NewLine + "#################################################" + Environment.NewLine;
            GeneralMethods.LibraryLogWriter("LibraryImportLog", strLog);
        }

        private string CreatePortfolio(LibraryDto libraryDto, IFormFile file)
        {
            try
            {
                string industries = string.Empty;
                string technologies = string.Empty;

                if (libraryDto.Industry != null && libraryDto.Industry.Length > 0)
                {
                    foreach (var item in libraryDto.Industry)
                    {
                        var industry = domainTypeService.GetDomainById(Convert.ToInt32(item));
                        industries += industry.Alias + ",";
                    }
                    industries = industries.TrimEnd(',');
                }

                if (libraryDto.Technology != null && libraryDto.Technology.Length > 0)
                {
                    foreach (var item in libraryDto.Technology)
                    {
                        var technology = technologyService.GetTechnologyById(Convert.ToInt32(item));
                        technologies += technology.Alias + ",";
                    }
                    technologies = technologies.TrimEnd(',');
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://ourportfolio.projectstatus.in/api/portfolios/add");
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    MultipartFormDataContent content = new MultipartFormDataContent();
                    content.Add(new StringContent(libraryDto.Title), "title");
                    content.Add(new StringContent(libraryDto.Description), "description");
                    content.Add(new StringContent((libraryDto.LiveURL != null) ? libraryDto.LiveURL : "http://example.com"), "live_url");
                    content.Add(new StringContent((libraryDto.CRMUserId == null || libraryDto.CRMUserId.ToString() == "") ? "0000" : libraryDto.CRMUserId.ToString()), "crm_id");
                    content.Add(new StringContent(industries), "industries");
                    content.Add(new StringContent(technologies), "technologies");
                    content.Add(new StringContent("jaipur"), "location");
                    content.Add(new StringContent(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString()), "ip");
                    content.Add(new StringContent(CurrentUser.Name), "created_by");
                    content.Add(new StringContent((libraryDto.keywords != null) ? libraryDto.keywords : ""), "tags");
                    content.Add(new StringContent(""), "unique_in_website");
                    content.Add(new StringContent("1"), "mobile_friendly");
                    content.Add(new StringContent(libraryDto.IsNDA.ToString()), "nda");
                    content.Add(new StringContent(libraryDto.IsFeatured.ToString()), "featured");
                    content.Add(new StringContent(libraryDto.IsReadyToUse.ToString()), "readytouse");
                    //content.Add(new StringContent(libraryDto.CreatedDate), "completed_date");
                    content.Add(new StringContent(DateTime.Now.ToString("yyyy-MM-dd")), "completed_date");
                    content.Add(new StringContent("website"), "portfolio_type");
                    content.Add(new StringContent(libraryDto.Author == null ? string.Empty : libraryDto.Author), "author_name");
                    content.Add(new StringContent(libraryDto.BAName == null ? string.Empty : libraryDto.BAName), "ba_name");
                    content.Add(new StringContent(libraryDto.TLName == null ? string.Empty : libraryDto.TLName), "tl_name");
                    content.Add(new StringContent(libraryDto.IsGoodToShow.ToString()), "is_good_to_show");
                    content.Add(new StringContent(libraryDto.EMSLibraryId.ToString()), "ems_library_id");
                    content.Add(new StringContent(libraryDto.Team.ToString().ToLower() != "true" ? "external" : "internal"), "team");
                    if (file != null)
                    {
                        if (libraryDto.BannerImage != null)
                        {
                            using (Stream stream = file.OpenReadStream())
                            {
                                using (var binaryReader = new BinaryReader(stream))
                                {
                                    var fileContent = binaryReader.ReadBytes((int)file.Length);
                                    content.Add(new StreamContent(new MemoryStream(fileContent)), "image", libraryDto.BannerImage);
                                }
                            }
                        }
                    }
                    try
                    {
                        HttpResponseMessage Res = client.PostAsync("http://ourportfolio.projectstatus.in/api/portfolios/add", content).Result;
                        var jsonResult = Res.Content.ReadAsStringAsync();
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception ex)
            {

            }
            return "";
        }

        public void AddWatermark(FileStream fs, string watermarkText, Stream outputStream)
        {
            Image img = Image.FromStream(fs);
            Font font = new Font("Verdana", 30, FontStyle.Bold, GraphicsUnit.Pixel);
            //Adds a transparent watermark with an 100 alpha value.
            Color color = Color.FromArgb(100, 1, 73, 127);
            Color color1 = Color.FromArgb(58, 224, 166, 34);
            Color color2 = Color.FromArgb(100, 0, 0, 0);
            //The position where to draw the watermark on the image
            int x = img.Width;
            int y = img.Height;
            Point ptTop = new Point(10, 10);
            Point ptBottom = new Point(x / 2 + 200, y - 70);
            Point ptCenter = new Point(30, y / 2);
            Point pt1 = new Point(10, 10);
            Point pt2 = new Point(80, 100);
            Point pt3 = new Point(x / 2 - 70, y / 2);

            Point pt4 = new Point(150, 150);
            Point pt5 = new Point(80, 300);
            Point pt6 = new Point(x / 2 + 300, y / 2);

            Point pt7 = new Point(900, 150);
            Point pt8 = new Point(600, 100);
            Point pt9 = new Point(x / 2 + 50, y / 2 - 140);


            // Bottom
            Point pt10 = new Point(50, y - 70);
            Point pt11 = new Point(x / 2 - 100, y - 70);
            Point pt12 = new Point(50, y - 200);
            Point pt13 = new Point(x / 2 - 100, y - 200);
            Point pt14 = new Point(x / 2 + 150, y - 200);

            SolidBrush sbrush = new SolidBrush(color);
            SolidBrush sbrush1 = new SolidBrush(color1);
            SolidBrush sbrush2 = new SolidBrush(color2);

            Graphics gr = null;
            try
            {
                gr = Graphics.FromImage(img);
            }
            catch
            {
                Image img1 = img;
                img = new Bitmap(img.Width, img.Height);
                gr = Graphics.FromImage(img);
                gr.DrawImage(img1, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
                img1.Dispose();
            }

            gr.DrawString(watermarkText, font, sbrush, ptTop);
            //gr.DrawString(watermarkText, font, sbrush, ptCenter);
            gr.DrawString(watermarkText, font, sbrush, ptBottom);
            //gr.DrawString(watermarkText, font, sbrush1, pt1);
           // gr.DrawString(watermarkText, font, sbrush2, pt2);
            gr.DrawString(watermarkText, font, sbrush, pt3);

            //gr.DrawString(watermarkText, font, sbrush, pt4);
            //gr.DrawString(watermarkText, font, sbrush, pt5);
            //gr.DrawString(watermarkText, font, sbrush, pt6);
            //gr.DrawString(watermarkText, font, sbrush, pt7);
            //gr.DrawString(watermarkText, font, sbrush, pt8);
            //gr.DrawString(watermarkText, font, sbrush, pt9);
           // gr.DrawString(watermarkText, font, sbrush, pt10);
            //gr.DrawString(watermarkText, font, sbrush, pt11);
            //gr.DrawString(watermarkText, font, sbrush, pt12);
            //gr.DrawString(watermarkText, font, sbrush, pt13);
            //gr.DrawString(watermarkText, font, sbrush, pt14);
            gr.Dispose();

            img.Save(outputStream, ImageFormat.Jpeg);
        }

        private bool HasDownloadPermission()
        {
            var downloadPermission = libraryDownloadService.GetLibraryDownloadByOnlyUid(CurrentUser.Uid, CurrentUser.RoleId);
            if (downloadPermission != null && downloadPermission.MaximumDownloadInDay > 0)
            {
                return true;
            }
            else
            {
                return false;
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
        private List<string> GetLibraryComponentFileType(ICollection<LibraryComponentFile> libraryFiles)
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
        private string GetShortFileName(string str, int length)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                if (str.Length <= length)
                {
                    return str;
                }

                string extension = Path.GetExtension(str.ToLower());
                string fileName = Path.GetFileNameWithoutExtension(str);


                return fileName.Substring(0, length - 7) + "..." + extension;
            }
            return "";
        }

        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult Setting()
        {
            return View();
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult Setting(bool Sync = true)
        {
            List<string> ext = new List<string> { ".jpg", ".jpeg", ".png", ".JPG", ".JPEG", ".PNG" };
            string path = ContextProvider.HostEnvironment.WebRootPath + "/" + "Upload/LibraryFiles/";

            string[] localFiles = Directory.GetFiles(SiteKey.LocalDirectory, "*.*")
                                     .Select(Path.GetFileName).Where(x => ext.Contains(Path.GetExtension(x.ToLower())))
                                     .ToArray();
            string[] waterFiles = Directory.GetFiles(path, "*.*")
                                     .Select(Path.GetFileName).Where(x => ext.Contains(Path.GetExtension(x.ToLower())))
                                     .ToArray();
            var newFiles = localFiles.Except(waterFiles);

            List<FileInfo> files = new DirectoryInfo(SiteKey.LocalDirectory).EnumerateFiles("*.*", SearchOption.AllDirectories)
            .Where(x => ext.Contains(Path.GetExtension(x.Name.ToLower())) && !waterFiles.Any(y => y == x.Name))
            .Select(x => new FileInfo(x.FullName)).ToList();
            foreach (FileInfo file in files)
            {
                string fileName = file.Name;
                string fullPath = Path.Combine(path, fileName);
                Image img = null;
                using (FileStream fileStream = file.OpenRead())
                {
                    Stream outputStream = new MemoryStream();
                    AddWatermark(fileStream, "Dotsquares", outputStream);
                    img = Image.FromStream(outputStream);

                    using (Bitmap savingImage = new Bitmap(img.Width, img.Height, img.PixelFormat))
                    {
                        using (Graphics g = Graphics.FromImage(savingImage))
                        {
                            g.DrawImage(img, new Point(0, 0));
                        }
                        savingImage.Save(fullPath, ImageFormat.Jpeg);
                    }
                    img.Dispose();
                }
            }
            if (files.Count > 0)
                if (files.Count > 1)
                    ShowSuccessMessage("Success", $"{files.Count} images watermarked successfully", false);
                else
                    ShowSuccessMessage("Success", $"{files.Count} image watermarked successfully", false);
            else
                ShowWarningMessage("Warning", $"{files.Count} images found to watermark", false);

            return RedirectToAction("Setting");
        }

        private void FillSelectedItemsList(LibraryManagementDto libraryManagementDto,
            LibraryManagementSearchFilterDto searchFilter)
        {
            LibrarySearchSelectedItemsDto librarySearchSelectedItemsDto = new LibrarySearchSelectedItemsDto();
            if ((byte)searchFilter.LibraryType != 0)
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

            if (searchFilter.SalesKitTypeId != null)
            {
                librarySearchSelectedItemsDto = new LibrarySearchSelectedItemsDto();
                librarySearchSelectedItemsDto.FilterId = searchFilter.SalesKitTypeFilterId;
                librarySearchSelectedItemsDto.Label = searchFilter.SalesKitTypeLabel;
                librarySearchSelectedItemsDto.Order = 2;
                if (searchFilter.SalesKitTypeId != null)
                {
                    var salesKitDetail = salesKitTypeService.GetSubSalesKitDetail(Convert.ToInt32(searchFilter.SalesKitTypeId));
                    librarySearchSelectedItemsDto.SelectListItems = new List<SelectListItem>(){
                        new SelectListItem() { Text= salesKitDetail.Name, Value =salesKitDetail.SalesKitId.ToString() }
                    };
                    libraryManagementDto.libraryManagementSelectedItemsDtos.Add(librarySearchSelectedItemsDto);
                }
            }
            if (searchFilter.CvsTypeId != null)
            {
                var cvsDetail = cvsTypeService.GetCvsTypeDetail(Convert.ToInt32(searchFilter.CvsTypeId));

                librarySearchSelectedItemsDto = new LibrarySearchSelectedItemsDto();
                librarySearchSelectedItemsDto.FilterId = searchFilter.CvsTypeFilterId;
                librarySearchSelectedItemsDto.Label = searchFilter.CvsTypeLabel;
                librarySearchSelectedItemsDto.Order = 2;
                if (searchFilter.CvsTypeId != null)
                {
                    librarySearchSelectedItemsDto.SelectListItems = new List<SelectListItem>(){
                        new SelectListItem() { Text= cvsDetail.Name, Value =cvsDetail.CvsId.ToString() }
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

        [HttpPost]
        public ActionResult GetComponents(string Ids)
        {
            try
            {
                List<LibraryComponent> a = new List<LibraryComponent>();
                List<int> TechIds = Ids.Split(',').Select(int.Parse).ToList();
                var component = libraryManagementService.GetAllLibraries().Select(z => new { z.LibraryComponent, z.LibraryTechnology }).Where(x => x.LibraryTechnology.Any(y => TechIds.Contains(y.TechnologyId))).ToList().Select(q => new { q.LibraryComponent }).Where(p => p.LibraryComponent.Count > 0).ToList();

                foreach (var item in component)
                {
                    foreach (var item2 in item.LibraryComponent)
                    {
                        var b = item2;
                        a.Add(b);
                    }
                }
                var u = a.GroupBy(s => s.LibraryComponentTypeId)
                             .Where(g => g.Count() > 1).ToList();
                List<int> n = new List<int>();
                foreach (var item in u)
                {
                    n.Add(item.Key);
                }

                LibraryManagementIndexDto libraryManagementIndexDto = new LibraryManagementIndexDto();

                libraryManagementIndexDto.Components = libraryComponentTypeService.GetLibraryComponentTypes()
                    .Where(x => n.Contains(x.Id)).
                OrderBy(c => c.Name).Select(d => new SelectListItem
                {
                    Text = d.Name,
                    Value = d.Id.ToString()
                }).ToList();
                return PartialView("_ComponentSelect", libraryManagementIndexDto);
            }
            catch (Exception)
            {
                return PartialView("_ComponentSelect", new LibraryManagementIndexDto());
            }
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult AddEditSalesKit(int? id)
        {
            SalesKitTypeDto salesKitTypeDto = new SalesKitTypeDto();
            if (id != null)
            {
                SalesKitType salesKit = new SalesKitType();
                salesKit = salesKitTypeService.GetSalesKitTypeDetail(Convert.ToInt32(id));
                salesKitTypeDto.SalesKitId = salesKit.SalesKitId;
                salesKitTypeDto.Name = salesKit.Name;
                salesKitTypeDto.DisplayName = salesKit.DisplayName;
                salesKitTypeDto.ParentId = salesKit.ParentId;
                salesKitTypeDto.DisplayOrder = salesKit.DisplayOrder;
                salesKitTypeDto.IsActive = salesKit.IsActive.HasValue && salesKit.IsActive.Value != true ? false : true;
                salesKitTypeDto.IsChild = salesKit.ParentId.HasValue && salesKit.ParentId.Value != 0 ? true : false;
            }
            salesKitTypeDto.ParentSalesKit = GetSalesKitList(false);
            return PartialView("_AddEditSalesKit", salesKitTypeDto);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult AddEditSalesKit(SalesKitTypeDto salesKitTypeDto)
        {
            if (salesKitTypeDto != null && salesKitTypeDto.IsChild == false)
            {
                ModelState.Remove("ParentId");
            }
            if (CurrentUser != null && CurrentUser.Uid != 0 && ModelState.IsValid)
            {
                bool success = false;

                SalesKitType salesKit = new SalesKitType();
                var previousDisplayOrder = salesKitTypeService.GetLastDisplayOrder(salesKitTypeDto.ParentId);
                if (salesKitTypeDto.SalesKitId != 0)
                {
                    salesKit = salesKitTypeService.GetSalesKitTypeDetail(salesKitTypeDto.SalesKitId);
                    if (salesKit != null)
                    {
                        salesKit.Name = salesKitTypeDto.Name;
                        salesKit.DisplayName = salesKitTypeDto.DisplayName;
                        salesKit.DisplayOrder = salesKitTypeDto.DisplayOrder.HasValue ? salesKitTypeDto.DisplayOrder : previousDisplayOrder;
                        salesKit.IsActive = salesKitTypeDto.IsActive;
                        salesKit.ParentId = salesKitTypeDto.ParentId.HasValue && salesKitTypeDto.ParentId.Value != 0 ? salesKitTypeDto.ParentId.Value : 0;
                    }
                }
                else
                {
                    salesKit.Name = salesKitTypeDto.Name;
                    salesKit.DisplayName = salesKitTypeDto.DisplayName;
                    salesKit.DisplayOrder = salesKitTypeDto.DisplayOrder.HasValue ? salesKitTypeDto.DisplayOrder : previousDisplayOrder + 1;
                    salesKit.IsActive = salesKitTypeDto.IsActive;
                    salesKit.ParentId = salesKitTypeDto.ParentId.HasValue && salesKitTypeDto.ParentId.Value != 0 ? salesKitTypeDto.ParentId.Value : 0;
                }

                success = salesKitTypeService.Save(salesKit);
                if (success)
                {
                    if (salesKitTypeDto.SalesKitId == 0)
                    {
                        ShowSuccessMessage("Success", "Sales Kit type has been successfully added!", false);
                    }
                    else
                    {
                        ShowSuccessMessage("Success", "Sales Kit type has been successfully updated!", false);
                    }

                }
                else
                {
                    ShowErrorMessage("Failed", "Sales Kit type already exist", false);
                }

                return RedirectToAction("AddEdit");
            }
            else
            {
                return CreateModelStateErrors();
            }
        }
        public List<SelectListItem> GetSalesKitList(bool selectDefault = true)
        {
            var list = salesKitTypeService.GetSalesKitType();

            var salesKitlist = list.Select(x => new SelectListItem { Text = x.Name != null ? x.Name.ToString() : "", Value = x.SalesKitId.ToString(), Selected = selectDefault ? (x.SalesKitId == list.FirstOrDefault().SalesKitId ? true : false) : false }).ToList();
            if (selectDefault)
            {
                salesKitlist.Insert(0, new SelectListItem() { Text = "-Select-", Value = "0" });
            }
            return salesKitlist;
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult AddEditCVs(int? id)
        {
            CvsTypeDto cvsTypeDto = new CvsTypeDto();
            if (id != null)
            {
                CvsType cvsType = new CvsType();
                cvsType = cvsTypeService.GetCvsTypeRecordDetail(Convert.ToInt32(id));
                cvsTypeDto.CvsId = cvsType.CvsId;
                cvsTypeDto.Name = cvsType.Name;
                cvsTypeDto.DisplayOrder = cvsType.DisplayOrder;
                cvsTypeDto.IsActive = cvsType.IsActive.HasValue && cvsType.IsActive.Value != true ? false : true;
            }
            return PartialView("_AddEditCVs", cvsTypeDto);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult AddEditCVs(CvsTypeDto cvsTypeDto)
        {

            if (CurrentUser != null && CurrentUser.Uid != 0 && ModelState.IsValid)
            {
                bool success = false;

                CvsType cvsType = new CvsType();
                var previousDisplayOrder = cvsTypeService.GetLastDisplayOrder();
                if (cvsTypeDto.CvsId != 0)
                {
                    cvsType = cvsTypeService.GetCvsTypeRecordDetail(cvsTypeDto.CvsId);
                    if (cvsType != null)
                    {
                        cvsType.Name = cvsTypeDto.Name;
                        cvsType.DisplayOrder = cvsTypeDto.DisplayOrder.HasValue ? cvsTypeDto.DisplayOrder : previousDisplayOrder;
                        cvsType.IsActive = cvsTypeDto.IsActive;
                    }
                }
                else
                {
                    cvsType.Name = cvsTypeDto.Name;
                    cvsType.DisplayOrder = cvsTypeDto.DisplayOrder.HasValue ? cvsTypeDto.DisplayOrder : previousDisplayOrder + 1;
                    cvsType.IsActive = cvsTypeDto.IsActive;
                }

                success = cvsTypeService.Save(cvsType);
                if (success)
                {
                    if (cvsTypeDto.CvsId == 0)
                    {
                        ShowSuccessMessage("Success", "CVs type has been successfully added!", false);
                    }
                    else
                    {
                        ShowSuccessMessage("Success", "CVs type has been successfully updated!", false);
                    }

                }
                else
                {
                    ShowErrorMessage("Failed", "CVs type already exist", false);
                }
                
                return RedirectToAction("AddEdit");
            }
            else
            {
                return CreateModelStateErrors();
            }
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult AddTemplate()
        {
            return PartialView("_AddEditTemplate");
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult AddTemplate(TemplateTypeDto templateTypeDto)
        {
            if (ModelState.IsValid)
            {
                LibraryTemplateType IsTemplateExist = null;
                IsTemplateExist = libraryTemplateTypeService.GetLibraryTemplateTypes().Where(x => x.Name.ToLower().Trim() == templateTypeDto.Name.ToLower().Trim()).FirstOrDefault();
                if (IsTemplateExist == null)
                {
                    LibraryTemplateType libraryTemplateType = new LibraryTemplateType
                    {
                        Name = templateTypeDto.Name,
                        IsActive = true,
                        AddDate = DateTime.Now,
                        ModifyDate = DateTime.Now
                    };
                    var result = libraryTemplateTypeService.Save(libraryTemplateType);
                    if (result == null)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record not saved.", IsSuccess = false });
                    }
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record saved successfully", IsSuccess = true, Data = result.Id.ToString() });
                }
                else
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Template is already exist", IsSuccess = false });
                }
            }
            return CreateModelStateErrors();
        }

        [HttpPost]
        public string GetTags(string tag)
        {
            try
            {

                if (!string.IsNullOrEmpty(tag))
                {
                    tag = tag.ToLower();
                    var a = libraryManagementService.GetAllLibraries()
                            .Where(y => y.IsActive == true && (!string.IsNullOrEmpty(y.SearchKeyword)))
                            .Select(x => x.SearchKeyword.Split(',').ToList()).ToList();
                    var result = (a.SelectMany(x => x).Select(y => y.Trim()).ToList());
                    var res = result.Where(y => y.ToLower().Contains(tag)).Distinct().Take(15).ToList();
                    return JsonConvert.SerializeObject(res);
                }
            }
            catch (Exception ex)
            {
            }
            return "";
        }

        [HttpGet]
        public int IsLibraryExist(string text)
        {
            try
            {
                var IsLibraryExists = libraryManagementService.GetAllLibraries().Where(x => x.Title.ToLower().Trim() == text.ToLower().Trim()).FirstOrDefault();
                if (IsLibraryExists == null)
                {
                    return 0;
                }
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        [HttpGet]
        [CustomAuthorization(true)]
        public ActionResult DownloadHistory()
        {
            var model = new LibraryDownloadHistoryDto();


            model.Users = userLoginService.GetUsers().Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() }).ToList();
            //model.LibraryTitle = libraryDownloadHistoryService.GetLibrary().ToList();
            model.DateFrom = DateTime.Today.AddDays(-30).ToFormatDateString("dd/MM/yyyy");
            model.DateTo = DateTime.Today.ToFormatDateString("dd/MM/yyyy");
            return View(model);
        }

        [HttpPost]
        [CustomAuthorization(true)]
        public IActionResult DownloadHistory(IDataTablesRequest request, LibraryManagementDownloadHistoryFilter searchFilter)
        {
            var pagingServicesForAll = request != null ? new PagingService<LibraryDownloadHistory>(request.Start, request.Length) : new PagingService<LibraryDownloadHistory>(0, int.MaxValue);

            var expr = GetDownloadHistoryFilterExpersion(searchFilter);

            //var filterExpr = PredicateBuilder.True<LibraryDownloadHistory>();
            //    if (request != null && !string.IsNullOrEmpty(request.Search.Value))
            //    {
            //        var search = request.Search.Value.ToString().Trim().ToLower();
            //        filterExpr = filterExpr.And(X => X.Library.Title.Trim().ToLower().Contains(search));
            //    }
            pagingServicesForAll.Filter = expr;
            pagingServicesForAll.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.Id);
            };

            int totalCount = 0;

            var response = libraryDownloadHistoryService.GetLibraryDownloadHistoryByPaging(out totalCount, pagingServicesForAll);

            var data = response.Select((r, index) => new
            {
                rowIndex = (index + 1) + (request.Start),
                UserName = r.DownloadByNavigation.Name,
                r.Library.Title,
                r.LibraryFileType.Name,
                DownloadOn = r.DownloadOn.ToFormatDateString("MMM, dd yyyy hh:mm tt")
            }).ToList();

            return DataTablesJsonResult(totalCount, request, data);
        }

        [HttpPost]
        public ActionResult DownloadHistorySummary(IDataTablesRequest request, LibraryManagementDownloadHistoryFilter searchFilter)
        {
            var pagingServicesForAll = request != null ? new PagingService<LibraryDownloadHistory>(request.Start, request.Length) : new PagingService<LibraryDownloadHistory>(0, int.MaxValue);

            var expr = GetDownloadHistoryFilterExpersion(searchFilter);
            pagingServicesForAll.Filter = expr;
            pagingServicesForAll.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.Id);
            };

            int totalCount = 0;
            //var response = allUserData.Select(x => new { x.RoleId, x.UserLoginId }).Distinct().Skip((pagingService.Start - 1) * pagingService.Length).Take(pagingService.Length).ToList();

            var response = libraryDownloadHistoryService.GetLibraryDownloadHistoryByPaging(out totalCount, pagingServicesForAll).GroupBy(a => a.LibraryId)
                .Select(b => new { id = b.FirstOrDefault().LibraryId, Totalcount = b.Count(x => x.LibraryId == x.LibraryId) }).ToList();

            var library = libraryDownloadHistoryService.GetLibrary();

            var data = response.Select((r, index) => new
            {
                rowIndex = (index),
                ComponentName = library.Where(a => a.Id == r.id).FirstOrDefault().Title,
                r.Totalcount
            }).ToList();

            //return DataTablesJsonResult(totalCount, request, data);

            IDictionary<string, object> additionalparam = new Dictionary<string, object>();
            additionalparam.Add(new KeyValuePair<string, object>("LibDownloadSummary", data));

            return Json(additionalparam);
        }

        private Expression<Func<LibraryDownloadHistory, bool>> GetDownloadHistoryFilterExpersion(LibraryManagementDownloadHistoryFilter searchFilter)
        {
            var expr = PredicateBuilder.True<LibraryDownloadHistory>();

            if (searchFilter.LibraryTitle.HasValue())
            {
                searchFilter.LibraryTitle = searchFilter.LibraryTitle.Trim().ToLower();

                expr = expr.And(L => L.Library.Title.ToLower().Contains(searchFilter.LibraryTitle));
            }

            DateTime? startDate = searchFilter.DateFrom.ToDateTime("dd/MM/yyyy");
            DateTime? endDate = searchFilter.DateTo.ToDateTime("dd/MM/yyyy");

            if (startDate.HasValue && endDate.HasValue)
            {
                expr = expr.And(L => L.DownloadOn.Date >= startDate && L.DownloadOn.Date <= endDate.Value);

            }
            else if (startDate.HasValue)
            {
                expr = expr.And(L => L.DownloadOn.Date >= startDate);
            }
            else if (endDate.HasValue)
            {
                expr = expr.And(L => L.DownloadOn.Date <= endDate.Value);
            }

            if ((searchFilter.Users ?? 0) > 0)
            {
                expr = expr.And(l => l.DownloadBy == searchFilter.Users.Value);
            }
            return expr;
        }

        [HttpPost]
        [CustomAuthorization(true)]
        public ActionResult ConvertToZip(string filePath)
        {
            string dirRoot = @"D:\local\EMSWebCore\EMS.Website\wwwroot\Upload\LibraryFiles\";
            //get a list of files
            string[] filesToZip = filePath.Split(",").Where(x => !string.IsNullOrEmpty(x)).ToArray();

            string zipFileName = string.Format("zipfile-{0:yyyy-MM-dd_hh-mm-ss-tt}.zip", DateTime.Now);

            using (MemoryStream zipMS = new MemoryStream())
            {
                using (ZipArchive zipArchive = new ZipArchive(zipMS, ZipArchiveMode.Create, true))
                {
                    //loop through files to add
                    foreach (string fileToZip in filesToZip)
                    {
                        //read the file bytes

                        if (System.IO.File.Exists(Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/", fileToZip)))
                        {
                            byte[] fileToZipBytes = System.IO.File.ReadAllBytes(Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/", fileToZip));
                            ZipArchiveEntry zipFileEntry = zipArchive.CreateEntry(fileToZip.Replace(dirRoot, "").Replace('\\', '/'));

                            //add the file contents
                            using (Stream zipEntryStream = zipFileEntry.Open())
                            using (BinaryWriter zipFileBinary = new BinaryWriter(zipEntryStream))
                            {
                                zipFileBinary.Write(fileToZipBytes);
                            }
                        }
                    }
                }

                using (FileStream finalZipFileStream = new FileStream(@"C:\Users\admin\Downloads\" + zipFileName, FileMode.Create))
                {
                    zipMS.Seek(0, SeekOrigin.Begin);
                    zipMS.CopyTo(finalZipFileStream);
                }

            }

            return Json("");
        }


        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult DeleteLibrary(string strKeyIds)
        {
            try
            {
                var response = libraryManagementService.DeleteLibraryByIds(strKeyIds);

                return NewtonSoftJsonResult(true);
            }
            catch (Exception)
            {
                return NewtonSoftJsonResult(false);
            }
        }
    }
}