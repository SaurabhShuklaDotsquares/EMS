using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using EMS.Web.Modals;
using System.Linq.Expressions;
using EMS.Website.Code.LIBS;
using EMS.Web.Code.LIBS;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Drawing;

namespace EMS.Website.Controllers
{
    //[CustomAuthorization(IsAshishTeam: true)]
    public class EstimateHourController : BaseController
    {
        private readonly IDomainTypeService domainTypeService;
        private readonly IEstimateHourService estimateHourService;
        private readonly ITechnologyService technologyService;
        private readonly ITechnologyParentService technologyParentService;
        private readonly IUserLoginService userLoginService;
        private readonly ITechnologyParentMappingService technologyParentMappingService;
        private readonly ISerialNumberService serialNumberService;

        private readonly string localDirectory = SiteKey.LocalDirectory;
        private List<string> imageExtensions = new List<string> { ".jpg", ".png", ".jpeg", ".gif" };
        public EstimateHourController(IDomainTypeService _domainTypeService, IEstimateHourService _estimateHourService, ITechnologyService _technologyService, ITechnologyParentService _technologyParentService, IUserLoginService _userLoginService, ITechnologyParentMappingService _technologyParentMappingService)
        {
            domainTypeService = _domainTypeService;
            estimateHourService = _estimateHourService;
            technologyService = _technologyService;
            technologyParentService = _technologyParentService;
            userLoginService = _userLoginService;
            technologyParentMappingService = _technologyParentMappingService;
        }

        public IActionResult Index()
        {
            EstimateHourIndexDto IndexDto = new EstimateHourIndexDto();

            IndexDto.IndustryList = domainTypeService.GetDomainList().Select(n => new SelectListItem { Text = n.DomainName, Value = n.DomainId.ToString() }).OrderBy(x => x.Text).ToList();
            IndexDto.TechnologyList = technologyParentService.GetTechnologyParentList().Select(n => new SelectListItem { Text = n.Title, Value = n.Id.ToString() }).OrderBy(x => x.Text).ToList();

            IndexDto.FileNameList = estimateHourService.GetEstimateHourFileNameTypes().Where(c => c.UploadFileName != null).Select(n=> new SelectListItem { Text=n.UploadFileName,Value=n.Id.ToString()}).OrderBy(z=>z.Text).ToList();
           // IndexDto.FileNameList = estimateHourService.GetEstimateHourList().Where(c => c.FileName != null).Select(x => new { x.FileName }).Distinct().Select(n => new SelectListItem { Text = n.FileName, Value = n.FileName }).OrderBy(x => x.Text).ToList();

            IndexDto.DateFrom = DateTime.Today.AddDays(-30).ToFormatDateString("dd/MM/yyyy");
            IndexDto.DateTo = DateTime.Today.ToFormatDateString("dd/MM/yyyy");
            return View(IndexDto);
        }

        public IActionResult AddEdit(int? id)
        {
            EstimateHourDto model = new EstimateHourDto();
            if (TempData["ID"] != null)
            {
                id = (int)TempData["ID"];
            }
            if (id != null)
            {
                EstimateHour estimateHour = new EstimateHour();
                estimateHour = estimateHourService.GetEstimateHourById(id ?? 0);

                model.RequirementName = estimateHour.RequirementName;
                model.RequirementDesc = estimateHour.RequirementDesc;

                if (TempData["ID"] != null)
                {
                    model.Id = 0;
                    model.OpenBy = "Save And Add New";
                    model.RequirementName = "";
                    model.RequirementDesc = "";
                    model.EstimatedHour = 0;
                }
                else
                {
                    model.EstimatedHour = estimateHour.EstimatedHour;
                    model.Id = estimateHour.Id;
                    model.OpenBy = "Edit";
                }
                model.DomainId = estimateHour.DomainId;
                model.TechnologyParentId = estimateHour.TechnologyParentId;


                model.IsFreeBie = estimateHour.IsFreeBie;
                model.Crmid = estimateHour.Crmid;
                model.EstimateHourFileNameTypeId = estimateHour.EstimateHourFileNameTypeId;
                model.Bauid = estimateHour.Bauid;
                model.Tluid = estimateHour.Tluid;
                model.ComplexityLevel = estimateHour.ComplexityLevel;
                model.ConversionDate = estimateHour.ConversionDate.ToFormatDateString("dd/MM/yyyy") ?? "";
                model.Domains = domainTypeService.GetDomainList().Select(n => new SelectListItem { Text = n.DomainName, Value = n.DomainId.ToString() }).OrderBy(x => x.Text).ToList();
                model.FileNames = estimateHourService.GetEstimateHourFileNameTypes().
                       Select(n => new SelectListItem
                       {
                           Text = n.Name,
                           Value = n.Id.ToString()
                           // ,Selected = library.LibraryComponent.Any(x => x.LibraryComponentTypeId == n.Id)
                       }).ToList();

                if (estimateHour.TechnologyParentId == null || estimateHour.TechnologyParentId == 0)
                {
                    model.TechnologyParents = technologyParentService.GetTechnologyParentList().Select(n => new SelectListItem
                    {
                        Text = n.Title,
                        Value = n.Id.ToString(),
                        Selected = n.TechnologyParentMapping.Any(p => estimateHour.EstimateHourParentTechnology.Any(y => y.TechnologyParentId == p.TechnologyParent.Id))
                        //Selected = n.TechnologyParentMapping.Any(p => p.TechnologyParentId == estimateHour.TechnologyParentId)
                    }).OrderBy(x => x.Text).ToList();
                }
                else
                {
                    model.TechnologyParents = technologyParentService.GetTechnologyParentList().Select(n => new SelectListItem
                    {
                        Text = n.Title,
                        Value = n.Id.ToString(),
                        //Selected = n.TechnologyParentMapping.Any(p => estimateHour.EstimateHourParentTechnology.Any(y => y.TechnologyParentId == p.TechnologyParent.Id))
                        Selected = n.TechnologyParentMapping.Any(p => p.TechnologyParentId == estimateHour.TechnologyParentId)
                    }).OrderBy(x => x.Text).ToList();
                }
                model.Technologies = technologyParentMappingService.GetTechnologyParentList()
                                        .Where(x => model.TechnologyParents.Where(y => y.Selected == true)
                                        .Any(n => Convert.ToInt32(n.Value) == x.TechnologyParentId)
                            ).ToList()
                            .OrderBy(x => x.TechnologyParentId).Select(n => new SelectListItem
                            {
                                Text = n.Technology.Title,
                                Value = n.TechnologyId.ToString(),
                                Selected = estimateHour.EstimateHourTechnology.Any(x => x.TechnologyId == n.TechnologyId),
                                Group = new SelectListGroup() { Name = n.TechnologyParentId.ToString() }
                            }).OrderBy(x => x.Text).ToList();
                //technologyService.GetTechnologyList().Where(x => model.TechnologyParents.Where(y => y.Selected == true)
                //                    .Any(n => Convert.ToInt32(n.Value) == x.TechnologyParentId)).ToList().OrderBy(x => x.TechnologyParentId)
                //                    .Select(n => new SelectListItem { Text = n.Title, Value = n.TechId.ToString(),
                //Selected = estimateHour.EstimateHourTechnology.Any(x => x.TechnologyId == n.TechnologyId),
                //Group = new SelectListGroup()
                //{ Name = n.TechnologyParentId.ToString() }
                //}).ToList();

            }
            else
            {
                //model.Id = 0;
                model.Domains = domainTypeService.GetDomainList().Select(n => new SelectListItem { Text = n.DomainName, Value = n.DomainId.ToString() }).OrderBy(x => x.Text).ToList();
                model.Technologies = technologyService.GetTechnologyList().Select(n => new SelectListItem { Text = n.Title, Value = n.TechId.ToString() }).OrderBy(x => x.Text).ToList();
                model.TechnologyParents = technologyParentService.GetTechnologyParentList().Select(n => new SelectListItem { Text = n.Title, Value = n.Id.ToString() }).OrderBy(x => x.Text).ToList();
                model.FileNames = estimateHourService.GetEstimateHourFileNameTypes().
                       Select(n => new SelectListItem
                       {
                           Text = n.Name,
                           Value = n.Id.ToString()
                           // ,Selected = library.LibraryComponent.Any(x => x.LibraryComponentTypeId == n.Id)
                       }).ToList();

                // model.LibraryComponentTypes = libraryComponentTypeService.GetLibraryComponentTypes().Select(n => new SelectListItem { Text = n.Name, Value = n.Id.ToString() }).ToList();
                // model.LibraryTemplateTypes = libraryTemplateTypeService.GetLibraryTemplateTypes().Select(n => new SelectListItem { Text = n.Name, Value = n.Id.ToString() }).ToList();
            }

            var users = userLoginService.GetUsersByPM(PMUserId);

            //model.Users = users?.Select(n => new SelectListItem { Text = $"{n.Name}{(n.DeptId.HasValue ? " (" + n.Department.Name + ")" : "")}", Value = n.Uid.ToString() }).ToList();

            model.BAUsers = users?.Where(x => RoleValidator.BA_RoleIds.Contains(x.RoleId.Value) || x.RoleId == (int)Enums.UserRoles.PM)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Uid.ToString()
                }).OrderBy(x => x.Text).ToList();
            
            model.TLUsers = users.Where(x =>
            RoleValidator.TL_Technical_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_Sales_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(x.DesignationId.Value)
            //|| RoleValidator.DV_RoleIds.Contains(x.RoleId.Value)
            )
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Uid.ToString()
                }).OrderBy(x => x.Text).ToList();

            return View(model);
        }


        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult AddEdit(EstimateHourDto model, string Save, string SaveAndAddNew,IFormCollection form)
        {
            
            int idAfterSave = 0;
            if (ModelState.IsValid)
            {
                EstimateHour estimateHour = new EstimateHour();

                if (model.Id > 0 && SaveAndAddNew == null)
                {
                    estimateHour = estimateHourService.GetEstimateHourById(model.Id);
                }
                estimateHour.RequirementName = model.RequirementName;
                estimateHour.Crmid = model.Crmid;
                estimateHour.RequirementDesc = model.RequirementDesc;
                estimateHour.IsFreeBie = model.IsFreeBie;
                if (model.IsFreeBie)
                {
                    estimateHour.EstimatedHour = 0;
                }
                else { estimateHour.EstimatedHour = Convert.ToDecimal(model.EstimatedHour.ToString() == "" ? "0" : model.EstimatedHour.ToString()); }
                estimateHour.EstimateHourFileNameTypeId = model.EstimateHourFileNameTypeId; //Request.Form["hdnSelectedFileName"];
                estimateHour.ComplexityLevel = model.ComplexityLevel;
                estimateHour.Bauid = model.Bauid;
                estimateHour.Tluid = model.Tluid;
                estimateHour.DomainId = model.DomainId;
                //estimateHour.TechnologyParentId = Convert.ToInt32(model.TechnologyParent[0]);
                estimateHour.ConversionDate = model.ConversionDate == "" ? null : model.ConversionDate.ToDateTime();
                estimateHour.EstimateHourTechnology.Clear();
                estimateHour.EstimateHourParentTechnology.Clear();
                if (model.TechnologyParent != null && model.TechnologyParent.Length > 0)
                {
                    foreach (var item in model.TechnologyParent)
                    {
                        EstimateHourParentTechnology technologyParent = new EstimateHourParentTechnology
                        {
                            TechnologyParentId = Convert.ToInt32(item),
                        };
                        estimateHour.EstimateHourParentTechnology.Add(technologyParent);
                    }
                }

                if (model.Technology != null && model.Technology.Length > 0)
                {
                    foreach (var item in model.Technology)
                    {
                        EstimateHourTechnology technology = new EstimateHourTechnology
                        {
                            TechnologyId = Convert.ToInt32(item),
                        };
                        estimateHour.EstimateHourTechnology.Add(technology);
                    }
                }

                estimateHourService.Save(estimateHour);
                idAfterSave = estimateHour.Id;
                ShowSuccessMessage("Success", "Estimate saved successfully", false);


            }
            else
            {
                return CreateModelStateErrors();
            }

            var SaveType = Save == null ? SaveAndAddNew : Save;

            if (SaveType == "Save")
            {
                return RedirectToAction("Index");

            }
            else
            {
                TempData["ID"] = idAfterSave;
                //return RedirectToAction("AddEdit", new { id = idAfterSave, SaveType = SaveType });
                return RedirectToAction("AddEdit");

            }
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult AddfileName()
        {
            return PartialView("_AddEditFileName");
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult AddfileName(string Name) //, IFormFile EstimateFiles
        {
            if (ModelState.IsValid)
            {
                //var fcv = fc;
                if (Request.Form.Files.Count > 0)
                {
                    var files = Request.Form.Files[0];

                    EstimateHourFileNameType IsCFileNameExist = null;
                    IsCFileNameExist = estimateHourService.GetEstimateHourFileNameTypes().Where(x => x.Name.ToLower().Trim() == Name.ToLower().Trim()).FirstOrDefault();
                    if (IsCFileNameExist == null)
                    {
                        if (files != null) //&& FileNameTypeDto.EstimateFiles.Count > 0
                        {
                            string fileExt = Path.GetExtension(files.FileName.ToLower());
                            string fileName = Name+fileExt;

                            //var EstimateHourFileList = estimateHourService.GetEstimateHourFileNameTypes().Any(x => x.UploadFileName == fileName);    //.ToList();  //.Where(x => x.LibraryId == model.Id)

                            //if (!EstimateHourFileList)
                            //{
                                uploadImageToFolder(files, fileName);

                                EstimateHourFileNameType estimateHourHourFileNameType = new EstimateHourFileNameType
                                {
                                    Name = Name,
                                    IsActive = true,
                                    AddDate = DateTime.Now,
                                    ModifyDate = DateTime.Now,
                                    UploadFileName = fileName
                                };

                                var result = estimateHourService.SaveEstimateHourFileNameType(estimateHourHourFileNameType);
                                if (result == null)
                                {
                                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record not saved.", IsSuccess = false });
                                }
                                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record saved successfully", IsSuccess = true, Data = result.Id.ToString() });
                                
                            //}
                            //else
                            //{
                            //    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "File With Same Name is already exist", IsSuccess = false,Data="For File" });
                            //}
                        }
                        else
                        {
                            return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Choose any File", IsSuccess = false });
                        }
                    }
                    else
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "File Name is already exist", IsSuccess = false, Data = "For Name" });
                    }
                }
            }
            return CreateModelStateErrors();
        }

        [NonAction]
        private bool uploadImageToFolder(IFormFile formFile, string ImageName)
        {
            if (formFile != null && formFile.Length > 0)
            {
                //Image img = null;
                var extension = Path.GetExtension(ImageName.ToLower());
                string FilePath = Path.Combine(localDirectory, ImageName);
                //string tempDirectory = path + "/temp";
                //string tempPath = Path.Combine(tempDirectory, ImageName);
                //if (!Directory.Exists(tempDirectory))
                //{
                //    Directory.CreateDirectory(tempDirectory);
                //}

                if (!Directory.Exists(localDirectory))
                {
                    Directory.CreateDirectory(localDirectory);
                }

                //if (imageExtensions.Contains(extension))
                //{
                //    // save images with watermark
                //    //using (var fileStream = new FileStream(tempPath, FileMode.Create))
                //    //{
                //    //    formFile.CopyTo(fileStream);
                //    //}
                //    //using (var fileStream = new FileStream(tempPath, FileMode.Open))
                //    //{
                //    //    Stream outputStream = new MemoryStream();
                //    //    AddWatermark(fileStream, "Dotsquares", outputStream);
                //    //    img = Image.FromStream(outputStream);

                //    //    using (Bitmap savingImage = new Bitmap(img.Width, img.Height, img.PixelFormat))
                //    //    {
                //    //        //string path = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/" + "Upload/LibraryFiles/", Guid.NewGuid().ToString()+ Path.GetExtension(ImageName));

                //    //        using (Graphics g = Graphics.FromImage(savingImage))
                //    //        {
                //    //            g.DrawImage(img, new Point(0, 0));
                //    //        }

                //    //        savingImage.Save(fullPath, ImageFormat.Jpeg);

                //    //    }
                //    //    img.Dispose();
                //    //}
                //    //System.IO.File.Delete(tempPath);
                //    // save original images
                //    using (var fileStream = new FileStream(FilePath, FileMode.Create))
                //    {
                //        formFile.CopyTo(fileStream);
                //    }
                //    if (System.IO.File.Exists(FilePath) || System.IO.File.Exists(fullPath))
                //    {
                //        return true;
                //    }
                //}
                //else
                //{
                using (var fileStream = new FileStream(FilePath, FileMode.Create))
                {
                    formFile.CopyTo(fileStream);
                }
                if (System.IO.File.Exists(FilePath))
                {
                    return true;
                }
                //}
            }
            return false;
        }

        [HttpPost]
        public string GetFileNameAutoComplete(string prefix)
      {
            try
            {
                if (!string.IsNullOrWhiteSpace(prefix))
                {
                    prefix = prefix.ToLower();
                    var EstimateHourFileNameTypes = estimateHourService.GetEstimateHourFileNameTypes();
                    var componentList = (from c in EstimateHourFileNameTypes
                                         where c.Name.IndexOf(prefix, StringComparison.InvariantCultureIgnoreCase) > -1
                                         select c.Name).ToArray();
                    return JsonConvert.SerializeObject(componentList);

                }
            }
            catch (Exception ex)
            {

            }
            return "";
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
        public string fetchDependentFileName(string technoId, string industryId)
        {
            try
            {
                List<SelectListItem> FileNameList = new List<SelectListItem>();
                if (technoId == null && industryId == null)
                {
                    FileNameList = estimateHourService.GetEstimateHourList().Where(c => c.EstimateHourFileNameTypeId != null).Select(x => new { x.EstimateHourFileNameTypeId,x.EstimateHourFileNameType.UploadFileName }).Distinct()
                        .Select(n => new SelectListItem { Text = n.UploadFileName, Value = n.EstimateHourFileNameTypeId.ToString() }).OrderBy(x => x.Text).ToList();
                }
                else if (technoId != null && industryId != null)
                {
                    FileNameList = estimateHourService.GetEstimateHourList().Where(c => c.EstimateHourFileNameTypeId != null && c.EstimateHourParentTechnology.Any(x => x.TechnologyParentId == Convert.ToInt32(technoId))
                        && c.DomainId == Convert.ToInt32(industryId)).Select(x => new { x.EstimateHourFileNameTypeId, x.EstimateHourFileNameType.UploadFileName }).Distinct()
                        .Select(n => new SelectListItem { Text = n.UploadFileName, Value = n.EstimateHourFileNameTypeId.ToString() }).OrderBy(x => x.Text).ToList();
                }
                else if (technoId != null)
                {
                    FileNameList = estimateHourService.GetEstimateHourList().Where(c => c.EstimateHourFileNameTypeId != null && c.EstimateHourParentTechnology.Any(x => x.TechnologyParentId == Convert.ToInt32(technoId))).Select(x => new { x.EstimateHourFileNameTypeId, x.EstimateHourFileNameType.UploadFileName }).Distinct()
                        .Select(n => new SelectListItem { Text = n.UploadFileName, Value = n.EstimateHourFileNameTypeId.ToString() }).OrderBy(x => x.Text).ToList();
                }
                else if (industryId != null)
                {
                    FileNameList = estimateHourService.GetEstimateHourList().Where(c => c.EstimateHourFileNameTypeId != null && c.DomainId == Convert.ToInt32(industryId)).Select(x => new { x.EstimateHourFileNameTypeId, x.EstimateHourFileNameType.UploadFileName }).Distinct()
                        .Select(n => new SelectListItem { Text = n.UploadFileName, Value = n.EstimateHourFileNameTypeId.ToString() }).OrderBy(x => x.Text).ToList();
                }

                return JsonConvert.SerializeObject(new { data = FileNameList });
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult ManageEstimateHourList(IDataTablesRequest request, EstimateHourSearchFilter searchFilter)
        {
            try
            {
                var pagingServices = new PagingService<EstimateHour>(request.Start, request.Length);
                //var filterExpr = PredicateBuilder.True<EstimateHour>();
                var expr = GetEstimateFilterExpersion(searchFilter);


                pagingServices.Filter = expr;

                pagingServices.Sort = (o) =>
                {
                    return o.OrderByDescending(c => c.Id);
                };
                TempData.Put("EstimateHoureReportFilters", searchFilter);
                int totalCount = 0;
                decimal totalEstimateHour = 0;
                var response = estimateHourService.GetEstimateHourByPaging(out totalCount, pagingServices);

                totalEstimateHour = estimateHourService.GetEstimateHourByPaging(out totalCount, pagingServices).Sum(x => x.EstimatedHour);
                var FilesName = estimateHourService.GetEstimateHourFileNameTypes().ToList();

                var returnResult = response.Select((d, index) =>
                 {
                     var detail = new
                     {
                         rowIndex = (index + 1) + (request.Start),
                         id = d.Id,
                         requirementName = d.RequirementName.Trim().ToTitleCase(),
                         //searchKeyword = d.SearchKeyword,
                         description = d.RequirementDesc,
                         //createdDate = d.Created.ToFormatDateString("dd MMM yyyy"),
                         conversionDate = d.ConversionDate.ToFormatDateString("dd MMM yyyy"),
                         fileName = FilesName.Where(s=> s.Id== d.EstimateHourFileNameTypeId).Select(c=> c.UploadFileName).FirstOrDefault(),
                         industryAndTechnology = d.Domain.DomainName + "/<br/>" + string.Join(", ", d.EstimateHourParentTechnology.Select(s => s.TechnologyParent.Title).ToList()),//d.TechnologyParent.Title,
                         industry = d.Domain.DomainName,
                         technology = string.Join(", ", d.EstimateHourParentTechnology.Select(s => s.TechnologyParent.Title).ToList()),
                         crmId = d.Crmid,
                         //estimateHours = d.IsFreeBie ?"IsFreeBie":GetEstimateTime(d.EstimatedHour),  //(d.EstimatedHour>8 && d.EstimatedHour < 40)? (d.EstimatedHour/8): d.EstimatedHour > 40? d.EstimatedHour/40: d.EstimatedHour,
                         estimateHours = d.IsFreeBie ? "FreeBie" : d.EstimatedHour.ToString(),  //(d.EstimatedHour>8 && d.EstimatedHour < 40)? (d.EstimatedHour/8): d.EstimatedHour > 40? d.EstimatedHour/40: d.EstimatedHour,
                         freebie = (bool)d.IsFreeBie,
                         tLead_BA = d.Tlu?.Name + "/<br/>" + d.Bau?.Name
                     };
                     totalEstimateHour += d.EstimatedHour;
                     return detail;
                 });

                IDictionary<string, object> additionalParameters = new Dictionary<string, object>();

                additionalParameters.Add(new KeyValuePair<string, object>("totalEstimateHour", GetEstimateTime(totalEstimateHour)));


                return DataTablesJsonResult(totalCount, request, returnResult, additionalParameters);

            }
            catch (Exception ex)
            {

            }
            return Ok();
        }

        [HttpGet]
        [CustomActionAuthorization]
        public async Task<IActionResult> Download(int id)
        {
            try
            {
                EstimateHour estimateHour = estimateHourService.GetEstimateHourById(id);
                EstimateHourFileNameType EHFT = estimateHourService.GetEstimateHourFileNameTypeById(Convert.ToInt32(estimateHour.EstimateHourFileNameTypeId));
                string strLog = string.Empty;

                if (EHFT == null)
                {
                    strLog = $"file not available with id={estimateHour.EstimateHourFileNameTypeId}  dated on {DateTime.Now} {Environment.NewLine}";
                    // GeneralMethods.LibraryLogWriter("LibraryManagement", strLog);
                    ShowErrorMessage("Error", "file not available", false);
                    return RedirectToAction("Index");
                }

                string fileName = EHFT.UploadFileName;//.Substring(libraryFile.FilePath.LastIndexOf('/') + 1);                

                var path = Path.Combine(localDirectory, fileName);
                if (!System.IO.File.Exists(path))
                {
                    strLog = $"file doesn't exist with id={estimateHour.EstimateHourFileNameTypeId}  dated on {DateTime.Now} {Environment.NewLine}";
                    //GeneralMethods.LibraryLogWriter("LibraryManagement", strLog);
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
                    //GeneralMethods.LibraryLogWriter("LibraryManagement", strLog);
                    ShowErrorMessage("Error", strLog, false);
                    return RedirectToAction("Index");
                }
                ShowSuccessMessage("Success", "File Downloaded Successfully",false);
                return File(memory, contentType, Path.GetFileName(path));
            }
            catch
            {
                ShowErrorMessage("Error", "file doesn't exist in directory", false);
                return RedirectToAction("Index");
            }
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
        private String GetEstimateTime(decimal Hours)
        {
            string time;
            if (Hours > 8 && Hours < 40)
            {
                string[] splitTime = (Hours / 8).ToString().Split(".");
                string hours = (Hours % 8).ToString();
                var splitHours = hours.Split('.');
                if (Convert.ToInt32(splitHours[1]) == 0)
                {
                    hours = splitHours[0];
                }
                time = splitTime[0] + " Days, " + hours + " Hours";
            }
            else if (Hours > 40)
            {
                string[] WeekAndDays = (Hours / 40).ToString().Split(".");
                decimal daysInHrs = Convert.ToDecimal(Hours % 40);
                string[] DaysAndHours = (daysInHrs / 8).ToString().Split(".");
                string hours = (daysInHrs % 8).ToString();
                var splitHours = hours.Split('.');
                if (Convert.ToInt32(splitHours[1]) == 0)
                {
                    hours = splitHours[0];
                }
                if (DaysAndHours[0] == "0")
                {
                    time = WeekAndDays[0] + " Weeks, " + hours + " Hours";
                }
                else
                {
                    time = WeekAndDays[0] + " Weeks, " + DaysAndHours[0] + " Days, " + hours + " Hours";
                }


            }
            else
            {
                string hours = Hours.ToString();
                var splitHours = hours.Split('.');
                if (splitHours.Length > 1)
                {
                    if (Convert.ToInt32(splitHours[1]) == 0)
                    {
                        hours = splitHours[0];
                    }
                }
                time = hours + " Hours";
            }
            //(d.EstimatedHour>8 && d.EstimatedHour < 40)? (d.EstimatedHour/8): d.EstimatedHour > 40? d.EstimatedHour/40: d.EstimatedHour,
            return time;
        }


        private Expression<Func<EstimateHour, bool>> GetEstimateFilterExpersion(EstimateHourSearchFilter searchFilter)
        {
            var expr = PredicateBuilder.True<EstimateHour>();

            if (searchFilter.textSearch.HasValue())
            {
                searchFilter.textSearch = searchFilter.textSearch.Trim().ToLower();

                expr = expr.And(L =>
                (L.RequirementName.Contains(searchFilter.textSearch))
                || L.Crmid.ToString().Contains(searchFilter.textSearch)); //|| L.FileName.ToLower().Contains(searchFilter.textSearch)
            }

            if (searchFilter.TechnologyId.HasValue)
            {
                expr = expr.And(X => X.EstimateHourParentTechnology.Any(z => z.TechnologyParentId == searchFilter.TechnologyId));
            }
            if (searchFilter.FileId.HasValue)
            {
                expr = expr.And(X => X.EstimateHourFileNameTypeId == searchFilter.FileId);
            }
            if (searchFilter.IndustryId.HasValue)
            {
                expr = expr.And(X => X.DomainId == searchFilter.IndustryId);
            }
            //DateTime? startDate = searchFilter.DateFrom.ToDateTime("dd/MM/yyyy");
            //DateTime? endDate = searchFilter.DateTo.ToDateTime("dd/MM/yyyy");

            //if (startDate.HasValue && endDate.HasValue)
            //{
            //    expr = expr.And(L => L.Created.Date >= startDate && L.Created.Date <= endDate.Value);
            //}
            //else if (startDate.HasValue)
            //{
            //    expr = expr.And(L => L.Created.Date >= startDate);

            //}
            //else if (endDate.HasValue)
            //{
            //    expr = expr.And(L => L.Created.Date <= endDate.Value);
            //}

            //if ((searchFilter.BA ?? 0) > 0)
            //{
            //    expr = expr.And(l => l.Uid_BA == searchFilter.BA.Value);
            //}
            //if ((searchFilter.TL ?? 0) > 0)
            //{
            //    expr = expr.And(l => l.Uid_TL == searchFilter.TL.Value);
            //}

            expr = expr.And(X => X.EstimateHourFileNameTypeId !=null);

            return expr;
        }

        public ActionResult DownloadEstimateHourExcel()
        {
            var searchFilter = TempData.Get<EstimateHourSearchFilter>("EstimateHoureReportFilters");
            TempData.Keep("EstimateHoureReportFilters");
            if (searchFilter != null)
            {
                var pagingServices = new PagingService<EstimateHour>(0, int.MaxValue);
                //searchFilter.chaseStatus = 3;
                var expr = GetEstimateFilterExpersion(searchFilter);

                //pagingServices.Filter = expr.And(x => x.DateofClosing.HasValue);
                pagingServices.Filter = expr;

                pagingServices.Sort = (o) =>
                {
                    return o.OrderByDescending(c => c.Id);
                };
                int totalCount = 0;
                decimal totalEstimateHour = 0;
                var response = estimateHourService.GetEstimateHourByPaging(out totalCount, pagingServices);
                totalEstimateHour = estimateHourService.GetEstimateHourByPaging(out totalCount, pagingServices).Sum(x => x.EstimatedHour);

                var FilesName = estimateHourService.GetEstimateHourFileNameTypes().ToList();
                if (response.Count > 0)
                {
                    var records = response.Select((r, index) => new
                    {
                        SrNo = (index + 1).ToString(),
                        Industry = r.Domain.DomainName,
                        Technology = string.Join(", ", r.EstimateHourParentTechnology.Select(s => s.TechnologyParent.Title).ToList()),//r.TechnologyParent.Title,
                        Requirement_Name = r.RequirementName.Trim().ToTitleCase(),
                        Requirement_Description = r.RequirementDesc.Trim(),
                        //Estimate_Hours = r.IsFreeBie ? "IsFreeBie" : GetEstimateTime(r.EstimatedHour),  
                        File_Name = FilesName.Where(s => s.Id == r.EstimateHourFileNameTypeId).Select(c => c.UploadFileName).FirstOrDefault(),
                        CRMId = r.Crmid.ToString(),
                        Estimate_Hours = r.IsFreeBie ? "FreeBie" : r.EstimatedHour.ToString(),
                        //(d.EstimatedHour>8 && d.EstimatedHour < 40)? (d.EstimatedHour/8): d.EstimatedHour > 40? d.EstimatedHour/40: d.EstimatedHour,
                        //BA = r.Bau?.Name,
                        Date_Of_Conversion = r.ConversionDate.ToFormatDateString("dd MMM yyyy"),
                        Team_Lead = r.Tlu?.Name,
                        Business_Analyst = r.Bau?.Name,
                        //Description = r.RequirementDesc,
                        //Created_Date = r.Created.ToFormatDateString("dd MMM yyyy, hh:mm tt"),

                        //Estimate_Hours = r.EstimatedHour,
                        //Freebie = (bool)r.IsFreeBie,


                    }).ToList();

                    //records.Add(new
                    //{
                    //    SrNo="",
                    //    Industry = "",
                    //    Technology = "",
                    //    Requirement_Name = "",
                    //    Requirement_Description = "",  
                    //    File_Name = "",
                    //    CRMId = "",
                    //    Estimate_Hours = "",
                    //    Date_Of_Conversion = "",
                    //    Team_Lead = "",
                    //    Business_Analyst = ""
                    //});
                    //records.Add(new
                    //{
                    //    SrNo="",
                    //    Industry = "",
                    //    Technology = "",
                    //    Requirement_Name = "",
                    //    Requirement_Description = "",  
                    //    File_Name = "",
                    //    CRMId = "Total Estimated Hours",
                    //    Estimate_Hours = totalEstimateHour.ToString(),
                    //    Date_Of_Conversion = "",
                    //    Team_Lead = "",
                    //    Business_Analyst = ""
                    //});
                    records.Add(new
                    {
                        SrNo = "",
                        Industry = "",
                        Technology = "",
                        Requirement_Name = "",
                        Requirement_Description = "",
                        File_Name = "",
                        CRMId = "Total Estimated Time",
                        Estimate_Hours = GetEstimateTime(totalEstimateHour).ToString(),
                        Date_Of_Conversion = "",
                        Team_Lead = "",
                        Business_Analyst = ""
                    });

                    string filename = "EstimateHourReport_" + DateTime.Now.Ticks.ToString() + ".xlsx";
                    string[] columns = { "SrNo", "Industry", "Technology", "Requirement_Name", "Requirement_Description", "File_Name", "CRMId", "Estimate_Hours", "Date_Of_Conversion", "Team_Lead", "Business_Analyst" };  //, "Estimate_Hours", "IsFreebie"
                    byte[] filecontent = ExportExcelHelper.ExportExcel(records, "Estimate Hour Report", true, false, columns);

                    return File(filecontent, ExportExcelHelper.ExcelContentType, filename);
                }

                return Content("No record found");
            }

            return Content("Unable to get filters");
        }
    }
}