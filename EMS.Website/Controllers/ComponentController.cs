using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.AspNetCore.Mvc;
using EMS.Data;
using EMS.Service;
using EMS.Dto;
using EMS.Web.Code.Attributes;
using System.Configuration;
using System.IO;
using EMS.Web.Code.LIBS;
using DataTables.AspNet.Core;
using DataTables.AspNet.AspNetCore;
using EMS.Core;
using PagedList;
using static EMS.Core.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;


namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class ComponentController : BaseController
    {


        #region "Fields"
        private IComponentService componentServices;
        private IComponentCategoryService componentcategoryService;
        #endregion
        #region "Constructor"
        public ComponentController(IComponentService componentServices, IComponentCategoryService componentcategoryService)
        {
            this.componentServices = componentServices;
            this.componentcategoryService = componentcategoryService;
        }
        #endregion

        [HttpGet]
        public ActionResult Index()
        {

            ComponentDto ComponentViewModel = new ComponentDto();
            if (CurrentUser != null)
            {
                ComponentViewModel.IsSuperAdmin = CurrentUser.IsSuperAdmin ? true : false;
                ComponentViewModel.CreatedByUid = CurrentUser.Uid;
            }

            ComponentViewModel.CategoryList = componentcategoryService.GetComponentCategory().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return View(ComponentViewModel);
        }

        [HttpPost]
        public IActionResult Index(IDataTablesRequest request, string txtSearch, string ComponentCategoryId, string type, string spam)
        {
            if (type == "2")
            {

                return RedirectToAction("GetComponentDesignImages");
            }
            var pagingServices = new PagingService<Component>(request.Start, request.Length);
            var expr = PredicateBuilder.True<Component>();
            //expr = expr.And(e => e.CreatedByUid == CurrentUser.Uid);
            if (!String.IsNullOrEmpty(ComponentCategoryId))
            {
                int componenecategoryId = Convert.ToInt32(ComponentCategoryId);
                expr = expr.And(e => e.ComponentCategoryId == componenecategoryId);
            }

            if (!String.IsNullOrEmpty(type))
            {
                switch (type)
                {
                    case "1":
                        expr = expr.And(e => e.Type == 1);
                        break;
                    case "2":
                        expr = expr.And(e => e.Type == 2);
                        break;
                    case "3":
                        expr = expr.And(e => e.Type == 3);
                        break;
                }

            }
            if (!string.IsNullOrEmpty(txtSearch))
            {
                expr = expr.And(e => e.Title.Contains(txtSearch) || e.Tags.Contains(txtSearch) || e.Description.Contains(txtSearch) || e.ComponentCategory.Name.Contains(txtSearch) || e.ImageName.Contains(txtSearch) || e.DesignImages.Contains(txtSearch) || e.DataUrl.Contains(txtSearch));
            }
            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "title":
                            return o.OrderByColumn(item, c => c.Title);
                        case "tags":
                            return o.OrderByColumn(item, c => c.Tags);
                        case "modified":
                            return o.OrderByColumn(item, c => c.Created);

                        case "uploadedBy":
                            return o.OrderByColumn(item, c => c.UserLogin.Name);
                        default:
                            return o.OrderByColumn(item, c => c.Created);
                    }
                }
                return o.OrderByDescending(c => c.Created);
            };
            int totalCount = 0;
            var response = componentServices.GetComponentByPaging(out totalCount, pagingServices);
            var Data = response.Select((r, Index) =>
                new
                {
                    Id = r.Id,//0
                    rowIndex = (Index + 1) + (request.Start),//1
                    Title = r.Title,//2
                    Category = r.ComponentCategory.Name,//3
                    Description = r.Description,//4
                    DataUrl = r.DataUrl,//5
                    ImageName = r.ImageName,//6
                    Created = r.Created.ToString(),//7
                    CreatedByUid = r.UserLogin.Name//8
                }
            ).ToList();

            return DataTablesJsonResult(totalCount, request, Data);
        }


        [HttpGet]
        public ActionResult AddEditComponent(int? id)
        {
            ComponentDto ComponentViewModel = new ComponentDto();
            ComponentViewModel.Type = 1;
            if (id != null)
            {

                Component component = new Component();
                ComponentViewModel.IsSuperAdmin = CurrentUser.IsSuperAdmin ? true : false;
                component = componentServices.GetComponentById(Convert.ToInt32(id));
                if (component != null)
                {
                    ComponentViewModel.Id = component.Id;
                    ComponentViewModel.Title = component.Title;
                    ComponentViewModel.Tags = component.Tags;
                    ComponentViewModel.Description = component.Description;
                    ComponentViewModel.Type = component.Type;
                    ComponentViewModel.ComponentCategoryId = component.ComponentCategoryId;
                    ComponentViewModel.ImageName = component.ImageName;
                    ComponentViewModel.DesignImages = component.DesignImages;
                    ComponentViewModel.DataUrl = component.DataUrl;
                }

            }

            ComponentViewModel.CategoryList = componentcategoryService.GetComponentCategory().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            ComponentViewModel.TypeList = Enum.GetValues(typeof(TypeValues)).Cast<TypeValues>()
               .Select(c => new SelectListItem() { Text = c.ToString(), Value = ((int)c).ToString() }).ToList();
            return PartialView("_AddEditComponent", ComponentViewModel);
        }

        [HttpPost]
        public ActionResult AddEditComponent(ComponentDto uploadDocumentViewModel, IFormFile ImageName, IFormFile DesignImageName, IFormFile PsdImageName)
        {
            if (ModelState.IsValid && CurrentUser != null && CurrentUser.Uid != 0)
            {
                int size = 0;
                string[] requiredExtension;
                bool hasFile = false;
                bool hasDesignFile = false;

                if (DesignImageName != null)
                {
                    hasDesignFile = true;
                }
                #region "Image size and type Validation"
                if (SiteKey.Size != null && SiteKey.ImageExtension != null && SiteKey.DesignImageExtension != null && SiteKey.PsdImageExtension != null)
                {
                    size = Convert.ToInt32(SiteKey.Size);
                    requiredExtension = SiteKey.Extension.Split(',');

                    if (ImageName != null)
                    {
                        var fileExt = Path.GetExtension(ImageName.FileName.ToLower());
                        #region Other Images
                        if (!requiredExtension.Contains(fileExt))
                        {
                            return Json(new { isSuccess = false, message = "Uploaded Image format is not valid. Please Upload only zip or rar file." });
                        }
                        else if (ImageName.Length > size)
                        {
                            return Json(new { isSuccess = false, message = "Uploaded Image size should not be grater than 200 MB" });
                        }
                        #endregion
                    }

                    if (DesignImageName != null)
                    {
                        var fileExt = Path.GetExtension(DesignImageName.FileName.ToLower());
                        string[] imageExt =SiteKey.ImageExtension.Split(',');
                        if (!imageExt.Contains(fileExt))
                        {
                            return Json(new { isSuccess = false, message = "Uploaded Image format is not valid" });
                        }
                        else if (DesignImageName.Length > size)
                        {
                            return Json(new { isSuccess = false, message = "Uploaded Other document size should not be grater than 200 MB" });
                        }
                    }
                    if (PsdImageName != null)
                    {
                        var fileExt = Path.GetExtension(PsdImageName.FileName.ToLower());
                        string[] imageExt = ".psd".Split(',');
                        if (!imageExt.Contains(fileExt))
                        {
                            return Json(new { isSuccess = false, message = "Uploaded Image format is not valid. Please upload PSD file." });
                        }
                        else if (PsdImageName.Length > size)
                        {
                            return Json(new { isSuccess = false, message = "Uploaded Other document size should not be grater than 200 MB" });
                        }
                    }
                    //string[] ImageExtension = ConfigurationManager.AppSettings["ImageExtension"].Split(',');
                    //string[] DesignImageExtension = ConfigurationManager.AppSettings["DesignImageExtension"].Split(',');
                    //string[] PsdImageExtension = ConfigurationManager.AppSettings["PsdImageExtension"].Split(',');
                }
                #endregion
                if (uploadDocumentViewModel.Type == 0)
                {
                    return Json(new { isSuccess = false, message = "Please select type" });
                }
                Component component = new Component();
                if (uploadDocumentViewModel.Id != 0)
                {
                    component = componentServices.GetComponentById(Convert.ToInt32(uploadDocumentViewModel.Id));
                }
                component.Title = uploadDocumentViewModel.Title;
                component.Tags = uploadDocumentViewModel.Tags;
                component.ComponentCategoryId = uploadDocumentViewModel.ComponentCategoryId;
                component.Description = uploadDocumentViewModel.Description;
                component.Type = uploadDocumentViewModel.Type;
                component.DataUrl = uploadDocumentViewModel.DataUrl;
                component.Created = DateTime.Now;
                component.Modified = DateTime.Now;
                component.CreatedByUid = CurrentUser.Uid;
                component.IsActive = true;
                if (component != null)
                {
                    component.ImageName = uploadDocumentViewModel.ImageName;
                    //component.DesignImages = uploadDocumentViewModel.DesignImages;
                }
                if (ImageName == null)
                {
                    component.ImageName = null;
                }
                else
                {
                    string image = GeneralMethods.SaveFile(ImageName, "Upload/ComponentImage/", "");
                    component.ImageName = image;
                }
                if (DesignImageName != null)
                {
                    string designimage = GeneralMethods.SaveFile(DesignImageName, "Upload/ComponentImage/", "");
                    component.DesignImages = designimage;
                }
                if (PsdImageName == null)
                {
                    component.PsdImages = null;
                }
                else
                {
                    string psdimage = GeneralMethods.SaveFile(PsdImageName, "Upload/ComponentImage/", "");
                    component.PsdImages = psdimage;
                }
                componentServices.Save(component);
                if (uploadDocumentViewModel.Id == 0)
                {
                    return Json(new { isSuccess = true, message = "Component has been successfully added", redirectUrl = Url.Action("Index", "Component") });
                }
                else
                {
                    ShowSuccessMessage("Success", "Component has been successfully updated", false);
                    return RedirectToAction("Index");
                }
            }
            return View(uploadDocumentViewModel);
        }
        public ActionResult DeleteComponent(int id)
        {
            Component data = componentServices.GetComponentById(id);
            if (data != null)
            {
                componentServices.Delete(data);
            }
            ShowSuccessMessage("Success", "Component has been successfully deleted", false);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult DeleteDocument(int componentId, string documentId)
        {
            Component componentDocument = new Component();
            componentDocument = componentServices.GetComponentById(componentId);
            string message = "";
            bool isSuccess = false;
            switch (documentId)
            {
                case "lnkDelMockupDoc":
                    if (!String.IsNullOrEmpty(componentDocument.ImageName))
                    {
                        componentDocument.ImageName = string.Empty;
                        message = "File has been successfully deleted";
                        isSuccess = true;
                    }
                    else
                    {
                        message = "File can not be deleted. Atleast one file required";
                        isSuccess = false;
                    }
                    break;
            }
            componentServices.Save(componentDocument);

            return Json(new { isSuccess, message, redirectUrl = Url.Action("Index", "Component", new { id = componentId }), updateTargetId = "error-ModalMessage" });
        }

        public ActionResult GetComponentDesignImages(int? page, int id, string txtSearch, string type)
        {
            List<Component> componentDocumentList = new List<Component>();
            componentDocumentList = componentServices.GetList();
            if (type == "1")
            {
                componentDocumentList = componentDocumentList.Where(x => x.Type == 1 && (id > 0 ? x.ComponentCategoryId == id : true)).ToList();

            }
            if (type == "2")
            {
                componentDocumentList = componentDocumentList.Where(x => x.Type == 2 && (id > 0 ? x.ComponentCategoryId == id : true)).ToList();

            }

            if (txtSearch != null && txtSearch != "" && txtSearch != "undefined")
            {
                componentDocumentList = componentDocumentList.Where(x => x.Title.Contains(txtSearch)).ToList();
            }

            List<ComponentDto> componentsDetailList = new List<ComponentDto>();
            componentDocumentList.ForEach(a =>
            {
                ComponentDto componentsDetail = new ComponentDto();
                componentsDetail.Title = a.Title;
                componentsDetail.Tags = a.Tags;
                componentsDetail.Description = a.Description;
                componentsDetail.UploadedBy = a.UserLogin.Name;
                componentsDetail.ImageName = a.ImageName;
                componentsDetail.DesignImages = a.DesignImages;
                componentsDetail.PsdImages = a.PsdImages;
                componentsDetail.Created = a.Created;
                componentsDetail.Id = Convert.ToInt32(a.Id);
                componentsDetail.UId = a.CreatedByUid;
                componentsDetailList.Add(componentsDetail);
            });
            int pageSize = 12;
            int PageNumber = (page ?? 1);
     
            return PartialView("_ComponentDesignImages", componentsDetailList.ToPagedList(PageNumber, pageSize));
        }

    }
}