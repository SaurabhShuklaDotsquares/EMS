using EMS.Dto;
using EMS.Service;
using EMS.Core;
using System.Collections.Generic;

using EMS.Web.Code.Attributes;
using System.Linq;
using System;
using DataTables.AspNet.Core;
using EMS.Data;
using EMS.Web.Code.LIBS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class DocumentController : BaseController
    {
        #region "Fields"

        private readonly IDepartmentService departmentService;
        private readonly IDocumentService documentService;

        #endregion

        #region "Constructor"

        public DocumentController(IDocumentService _documentService, IDepartmentService _departmentService)
        {
            departmentService = _departmentService;
            this.documentService = _documentService;
        }
        #endregion


        #region Event Index 

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult Index(IDataTablesRequest request)
        {
            var pagingServices = new PagingService<Document>(request.Start, request.Length);
            var expr = PredicateBuilder.True<Document>();

            if (request.Search.Value.HasValue())
            {
                string searchValue = request.Search.Value.Trim().ToLower();
                expr = expr.And(x => x.DocumentName.Contains(searchValue));
            }
            if (CurrentUser.RoleId != (int)Enums.UserRoles.Director && CurrentUser.RoleId != (int)Enums.UserRoles.PM)
            {
                expr = expr.And(x => x.DocumentDepartment.Any(y => y.DepartmentId == CurrentUser.DeptId) || x.IsAll == true);
                expr = expr.And(x => x.IsActive == true);

            }


            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.CreatedDate);
            };

            int totalCount = 0;
            var isPMUser = IsPMEvent;
            var response = documentService.GetDocumentByPaging(out totalCount, pagingServices);
            List<DocumentDto> lst = new List<DocumentDto>();
            foreach (var item in response)
            {
                DocumentDto obj = new DocumentDto();
                obj.Id = item.Id;
                if (item.IsAll)
                {
                    obj.Department = "All Departments";
                }
                else
                {
                    obj.Department = GetDepartMentName(item.DocumentDepartment);
                }
                //object data = item.DocumentDepartment.Select(x =>
                //  {
                //      return string.Join(", ", departmentService.GetDepartmentById(x.DepartmentId).Name).ToList();
                //  });
                obj.DocumentName = item.DocumentName;
                obj.DocumentPath = item.DocumentPath;
                obj.CreatedDate = item.CreatedDate;
                obj.IsActive = item.IsActive;
                lst.Add(obj);
            }
            return DataTablesJsonResult(totalCount, request, lst.Select((r, index) => new
            {
                rowIndex = (index + 1) + (request.Start),
                Id = r.Id,
                DepartmentId = r.Department,
                DocumentName = r.DocumentName,
                DocumentPath = r.DocumentPath,
                CreatedDate = r.CreatedDate.ToFormatDateString("MMM dd, yyyy"),
                IsActive = r.IsActive,
            }));
        }

        private string GetDepartMentName(ICollection<DocumentDepartment> documentDepartment)
        {
            var depId = documentDepartment.Select(a => a.DepartmentId).ToList();

            var result = departmentService.GetNames(depId);
            return string.Join(", ", result);
        }

        #endregion

        #region ADD/EDIT Event

        [HttpGet]
        public ActionResult Add(int? id)
        {
            if (CurrentUser.RoleId == (int)Enums.UserRoles.Director || CurrentUser.IsSuperAdmin)
            {
                List<SelectListItem> selectListItems = new List<SelectListItem>();
                selectListItems.Add(new SelectListItem() { Text = "All", Value = "0" });
                foreach (var item in departmentService.GetActiveDepartments().OrderBy(aa => aa.Name).ToList())
                {
                    selectListItems.Add(new SelectListItem() { Text = item.Name, Value = item.DeptId.ToString() });
                }
                ViewBag.DepartmentList = selectListItems;
                //ViewBag.DepartmentList = departmentService.GetActiveDepartments()
                //                   .Select(x => new SelectListItem { Text = x.Name, Value = x.DeptId.ToString() })
                //                   .ToList();
                DocumentDto model = new DocumentDto();
                if (id.HasValue && id.Value > 0)
                {
                    var entity = documentService.GetDocumentById(id.Value);
                    if (entity != null)
                    {
                        model.Id = entity.Id;
                        model.DocumentName = entity.DocumentName;
                        model.DocumentPath = entity.DocumentPath;
                        model.CreatedDate = entity.CreatedDate;
                        model.UserId = CurrentUser.Uid;
                        model.IsActive = entity.IsActive.HasValue ? entity.IsActive.Value : true;
                        model.IsAll = entity.IsAll ? entity.IsAll : false;
                        if (entity.IsAll == true)
                        {
                            model.DepartmentId = new int[1] { 0 };
                        }
                        else
                        {
                            model.DepartmentId = entity.DocumentDepartment.Select(x => x.DepartmentId).ToArray();
                        }

                    }
                    else
                    {
                        return CustomErrorView("Unable to find record");
                    }
                }

                return View("AddEdit", model);
            }
            else
            {
                return AccessDenied();
            }
        }

        [HttpPost]
        public ActionResult Add(DocumentDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(model.DocumentPath) && (model.Document == null || model.Document.Length == 0))
                    {
                        ShowErrorMessage("Error", "Please upload document.", false);
                        return RedirectToAction("Add", model.Id);
                    }
                    model.UserId = CurrentUser.Uid;
                    if (model.Document != null && model.Document.Length > 0)
                    {
                        UploadDoc(model);
                        model.DocumentPath = !string.IsNullOrWhiteSpace(model.DocumentPath) ? model.DocumentPath : model.DocumentPath;
                    }
                    var result = documentService.Save(model);

                    if (result != null)
                    {
                        ShowSuccessMessage("Success", "Document saved successfully", false);
                        return RedirectToAction("index");

                    }
                    ShowErrorMessage("Error", "Error", true);
                    return RedirectToAction("Add", model.Id);
                }
                catch (Exception ex)
                {
                    return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
                }
            }
            else
            {
                return CreateModelStateErrors();
            }
        }

        #endregion
        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult ApprovedStatus(int id)
        {
            if (id > 0)
            {
                var events = documentService.GetDocumentById(id);
                if (events != null)
                {
                    if (events.IsActive == true)
                    {
                        events.IsActive = false;
                    }
                    else
                    {
                        events.IsActive = true;
                    }
                    documentService.ApprovedStatus(events);
                }
            }
            return RedirectToAction("Index");
        }
        private void UploadDoc(DocumentDto model)
        {
            string fileExt = System.IO.Path.GetExtension(model.Document.FileName.ToLower());
            string fileName = $"{model.DocumentName.ToSelfURL()}_{DateTime.Now.Ticks}{fileExt}";
            model.DocumentPath = $"Upload/Document/{fileName}";
            string FilePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/" + "Upload/Document/", fileName);
            using (var stream = new FileStream(FilePath, FileMode.Create))
            {
                model.Document.CopyTo(stream);
            }
        }
    }
}
