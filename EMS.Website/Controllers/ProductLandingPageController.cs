using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using FineUploader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Linq;

namespace EMS.Web.Controllers
{
    public class ProductLandingPageController : BaseController
    {
        #region Fields and Constructor

        private readonly IProductLandingService productLandingService;
        private readonly IUserLoginService userLoginService;

        public ProductLandingPageController(ProductLandingService _productLandingService, UserLoginService _userLoginService)
        {
            productLandingService = _productLandingService;
            userLoginService = _userLoginService;
        }

        private bool IsMainTeamUser
        {
            get
            {
                return (CurrentUser.Uid == SiteKey.AshishTeamPMUId);
            }
        }

        #endregion

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Index()
        {
            var model = new ProductLandingIndexDto();

            model.IsMainTeamUser = IsMainTeamUser;
            if (model.IsMainTeamUser)
            {
                model.PMUserList = userLoginService.GetUserByRole((int)Enums.UserRoles.PM, true)
                                        .Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() })
                                        .ToList();
            }
            model.ProductLandingPageStatusList = WebExtensions.GetSelectList<Enums.ProductLandingPageStatus>();

            return View(model);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult Index(IDataTablesRequest request, int? pmId, Enums.ProductLandingPageStatus? status)
        {
            var pagingServices = new PagingService<ProductLanding>(request.Start, request.Length);
            var filterExpr = PredicateBuilder.True<ProductLanding>();
            var currentUserId = CurrentUser.Uid;
            var mainTeamPMUId = SiteKey.AshishTeamPMUId;
            var isMainTeamUser = IsMainTeamUser;

            if (isMainTeamUser)
            {
                if (status.HasValue && Enum.IsDefined(typeof(Enums.ProductLandingPageStatus), status.Value))
                {
                    filterExpr = filterExpr.And(x => x.Status == (byte)status.Value);

                    //Show own and own team records only
                    if (status.Value == Enums.ProductLandingPageStatus.Draft)
                    {
                        filterExpr = filterExpr.And(x => x.CreateByUid == mainTeamPMUId || x.UserLogin.PMUid == mainTeamPMUId);
                    }
                }
                else
                {
                    //Show own and own team records with any status and published records of other team members
                    filterExpr = filterExpr.And(x => x.CreateByUid == mainTeamPMUId || x.UserLogin.PMUid == mainTeamPMUId || x.Status != (byte)Enums.ProductLandingPageStatus.Draft);
                }
            }
            else
            {
                if (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO)
                {
                    filterExpr = filterExpr.And(x => x.CreateByUid == currentUserId || x.UserLogin.PMUid == currentUserId);
                }
                else
                {
                    filterExpr = filterExpr.And(x => x.CreateByUid == currentUserId);
                }

                if (status.HasValue && Enum.IsDefined(typeof(Enums.ProductLandingPageStatus), status.Value))
                {
                    filterExpr = filterExpr.And(x => x.Status == (byte)status.Value);
                }
            }

            if (pmId.HasValue && pmId.Value > 0)
            {
                filterExpr = filterExpr.And(x => x.CreateByUid == pmId.Value || x.UserLogin.PMUid == pmId.Value);
            }

            pagingServices.Filter = filterExpr;

            pagingServices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.CreateDate);
            };

            int totalCount = 0;
            var response = productLandingService.GetProductLandingByPaging(out totalCount, pagingServices);

            return DataTablesJsonResult(totalCount, request, response.Select((pl, index) => new
            {
                rowIndex = (index + 1) + (request.Start),
                pl.Id,
                pl.ProductName,
                CreateDate = pl.CreateDate.ToFormatDateString("dd MMM yyyy hh:mm tt"),
                Status = ((Enums.ProductLandingPageStatus)pl.Status).ToString(),
                CreatedBy = pl.UserLogin.Name,
                EditAllowed = pl.Status == (byte)Enums.ProductLandingPageStatus.Draft,
                ViewAllowed = pl.Status == (byte)Enums.ProductLandingPageStatus.Published
            }));
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult CloneTemplate(int id)
        {
            try
            {
                var model = new ProductLandingDto { IsDraft = true };

                var plEntity = productLandingService.GetProductLandingById(id);

                if (plEntity != null && plEntity.Status == (byte)Enums.ProductLandingPageStatus.Published)
                {
                    model = BindModel(plEntity);
                    id = 0;
                    model.Id = 0;
                    model.IsDraft = true;

                    model.Screenshots.ForEach(x => x.Id = 0);

                    return View("AddEdit", model);
                }
                else
                {
                    return CustomErrorView("Unable to find record");
                }
            }
            catch (Exception ex)
            {
                return CustomErrorView(ex.Message);
            }
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult AddEdit(int? id)
        {
            try
            {
                var model = new ProductLandingDto { IsDraft = true };
                bool defaultTemplate = false;
                if (!id.HasValue || id.Value <= 0)
                {
                    //Set default template
                    id = 1;
                    defaultTemplate = true;
                }

                var plEntity = productLandingService.GetProductLandingById(id.Value);

                if (plEntity != null)
                {
                    if (defaultTemplate || ((plEntity.CreateByUid == CurrentUser.Uid || plEntity.UserLogin.PMUid == PMUserId) && plEntity.Status == (byte)Enums.ProductLandingPageStatus.Draft))
                    {
                        model = BindModel(plEntity);
                        if (defaultTemplate)
                        {
                            id = 0;
                            model.Id = 0;
                            model.ProductName = "";
                        }
                        model.IsDraft = true;
                    }
                    else
                    {
                        return CustomErrorView("Invalid access or request already has been processed");
                    }
                }
                else
                {
                    return CustomErrorView("Unable to find record");
                }

                return View("AddEdit", model);
            }
            catch (Exception ex)
            {
                return CustomErrorView(ex.Message);
            }
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult AddEdit(ProductLandingDto model)
        {
            if (!model.IsDraft && !ModelState.IsValid)
            {
                return CreateModelStateErrors();
            }

            if (model.IsDraft && !model.ProductName.HasValue())
            {
                return MessagePartialView("Product name is required");
            }

            try
            {
                model.CurrentUserId = CurrentUser.Uid;
                model.PMUserId = PMUserId;

                var result = productLandingService.Save(model);

                if (result != null && result.Id > 0)
                {
                    ShowSuccessMessage("", $"Record saved successfully for Product : {result.ProductName}", false);
                    return NewtonSoftJsonResult(new RequestOutcome<int>
                    {
                        IsSuccess = true,
                        Message = "Record saved successfully",
                        RedirectUrl = model.IsDraft ? Url.Action("addedit", new { id = result.Id }) : Url.Action("index"),
                        Data = result.Id
                    });
                }
                else
                {
                    return MessagePartialView("Unable to save record");
                }
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.Message);
            }
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult View(int id)
        {
            try
            {
                if (id > 0)
                {
                    var plEntity = productLandingService.GetProductLandingById(id);

                    if (plEntity != null && plEntity.Status > (byte)Enums.ProductLandingPageStatus.Draft)
                    {
                        var model = BindModel(plEntity);

                        return View("View", model);
                    }
                }

                return CustomErrorView("Invalid access or record not found");
            }
            catch (Exception ex)
            {
                return CustomErrorView(ex.Message);
            }
        }

        //[HttpPost]
        //[CustomActionAuthorization]
        //public FineUploaderResult UploadScreenshot(FineUpload upload, int id, string caption)
        //{
        //    if (id > 0 && upload.InputStream != null && upload.InputStream.Length > 0)
        //    {
        //        try
        //        {
        //            var saveFilePath = $"Upload/ProductLandingScreenshot/{Guid.NewGuid()}{Path.GetExtension(upload.Filename)}";
        //            upload.SaveAs(Path.Combine(ContextProvider.HostEnvironment.WebRootPath, saveFilePath));

        //            productLandingService.AddScreenshot(id, saveFilePath, caption);

        //            return new FineUploaderResult(true, new { filePath = saveFilePath });
        //        }
        //        catch (Exception ex)
        //        {
        //            return new FineUploaderResult(false, error: ex.Message);
        //        }
        //    }

        //    return new FineUploaderResult(false, error: "Invalid id or file", preventRetry: true);
        //}

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult UploadImage(IFormFile upload)
        {
            if (upload != null && upload.Length > 0)
            {
                try
                {
                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(upload.FileName.ToLower())}";
                    var saveFilePath = $"Upload/ProductLandingScreenshot/{fileName}";

                    using (var stream = new FileStream(saveFilePath, FileMode.Create))
                    {
                        upload.CopyTo(stream);
                    }

                    //upload.SaveAs(Path.Combine(ContextProvider.HostEnvironment.WebRootPath, saveFilePath));
                    //upload.SaveAs(Server.MapPath($"~/{saveFilePath}"));

                    return NewtonSoftJsonResult(new { Uploaded = 1, FileName = fileName, Url = saveFilePath });
                }
                catch (Exception ex)
                {
                    return NewtonSoftJsonResult(new { error = new { Message = ex.Message } });
                }
            }

            return NewtonSoftJsonResult(new { error = new { Message = "Invalid id or file" } });
        }

        private ProductLandingDto BindModel(ProductLanding plEntity)
        {
            var model = new ProductLandingDto();
            model.Id = plEntity.Id;
            model.ProductName = plEntity.ProductName;
            model.HighlightText = plEntity.HighlightText;
            model.AboutProduct = plEntity.AboutProduct;
            model.Features = plEntity.Features;
            model.Feature1 = plEntity.Feature1;
            model.Feature2 = plEntity.Feature2;
            model.Feature3 = plEntity.Feature3;

            model.Testimonials = plEntity.Testimonials;
            model.ServiceDetail = plEntity.ServiceDetail;
            model.TechnologyDetail = plEntity.TechnologyDetail;

            model.Screenshots = plEntity.ProductLandingScreenshots.Select(x => new ProductLandingScreenshotDto
            {
                Id = x.Id,
                ScreenshotUrl = x.ScreenshotUrl,
                Description = x.Description
            }).ToList();

            return model;
        }
    }
}