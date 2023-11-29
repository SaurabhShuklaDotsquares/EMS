using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SelectPdf;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.IO;
using System.Net.Mail;
using System.Text;
using System;
using DataTables.AspNet.Core;
using EMS.Web.Modals;
using System.Linq;
using EMS.Web.Code.LIBS;
using EMS.Data.Model;
using EMS.Service.LibraryManagement;
using EMS.Web.Models.Others;
using Microsoft.AspNetCore.Hosting;
using NPOI.POIFS.FileSystem;

namespace EMS.Website.Controllers
{
    [CustomAuthorization()]
    public class DocumentLibraryController : BaseController
    {
        private readonly IDocumentLibraryService _documentLibraryService;
        private readonly IHostingEnvironment _env;
        public DocumentLibraryController(IDocumentLibraryService documentLibraryService, IHostingEnvironment env)
        {
            _documentLibraryService = documentLibraryService;
            _env = env;
        }
        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult Index()
        {
            var model = new DocumentLibraryIndexDto();
            List<SelectListItem> lstStatus = new List<SelectListItem>();
            lstStatus.Add(new SelectListItem { Text = "All", Value = "All" });
            lstStatus.Add(new SelectListItem { Text = "Active", Value = "Active" });
            lstStatus.Add(new SelectListItem { Text = "Deactive", Value = "Deactive" });

            model.StatusList = lstStatus;
            model.IsApprover = "Active";
            return View(model);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult Index(IDataTablesRequest request, DocumentLibraryFilter searchFilter)
        {
            var pagingServices = new PagingService<DocumentLibrary>(request.Start, request.Length);
            var expr = PredicateBuilder.True<DocumentLibrary>();


            if (!string.IsNullOrEmpty(searchFilter.Status))
            {
                if (searchFilter.Status == "Deactive")
                {
                    expr = expr.And(x => x.IsActive == false);
                }
                else if (searchFilter.Status == "Active")
                {
                    expr = expr.And(x => x.IsActive == true);
                }
            }
            //if (searchFilter.department > 0)
            //{
            //    expr = expr.And(x => x.User.DeptId == searchFilter.department);
            //}
            //if (searchFilter.Uid_User > 0)
            //{
            //    expr = expr.And(x => x.UserId == searchFilter.Uid_User);
            //}
            //if (searchFilter.ExperienceType > 0)
            //{
            //    expr = expr.And(x => x.ExperienceId == searchFilter.ExperienceType);
            //}            


            //DateTime? startDate = searchFilter.DateFrom.ToDateTime("dd/MM/yyyy");
            //DateTime? endDate = searchFilter.DateTo.ToDateTime("dd/MM/yyyy");

            //if (startDate.HasValue && endDate.HasValue)
            //{
            //    expr = expr.And(L => L.CreatedDate >= startDate && L.CreatedDate <= endDate);
            //}
            //else if (startDate.HasValue)
            //{
            //    expr = expr.And(L => L.CreatedDate >= startDate);
            //}
            //else if (endDate.HasValue)
            //{
            //    expr = expr.And(L => L.CreatedDate <= endDate);
            //}
            //if (!RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId))
            //{
            //    expr = expr.And(x => x.UserId == CurrentUser.Uid || SiteKey.AshishTeamPMUId == CurrentUser.Uid);
            //}
            if (request.Search.Value.HasValue())
            {
                string searchValue = request.Search.Value.Trim().ToLower();
                expr = expr.And(x => x.DocumentTitle != null && x.DocumentTitle.ToLower().Contains(searchValue) || x.DocumentType.ToLower().Contains(searchValue) || x.Version.ToLower().Contains(searchValue));
            }
            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "documentTitle":
                            return o.OrderByColumn(item, c => c.DocumentTitle);
                        case "documentType":
                            return o.OrderByColumn(item, c => c.DocumentType);
                        case "version":
                            return o.OrderByColumn(item, c => c.Version);
                        case "CreateDate":
                            return o.OrderByColumn(item, c => c.CreatedDate);
                        case "Status":
                            return o.OrderByColumn(item, c => c.IsActive);
                    }
                }

                return o.OrderByDescending(c => c.CreatedDate);
            };
            int totalCount = 0;
            var isPMUser = IsPMEvent;
            var response = _documentLibraryService.GetRecourdByPaging(out totalCount, pagingServices);

            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                rowIndex = (index + 1) + (request.Start),
                Id = r.Id,
                DocumentTitle = r.DocumentTitle,
                DocumentType = r.DocumentType,
                Version = r.Version,
                CreateDate = r.UpdatedDate == null ? r.CreatedDate.Value.ToString("dd/MM/yyyy hh:mm tt") : r.UpdatedDate.Value.ToString("dd/MM/yyyy hh:mm tt"),
                IsActive = r.IsActive,
                FilePath = r.FilePath,
            }));
        }
        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult AddEdit(string guid)
        {
            DocumentLibraryDto model = new DocumentLibraryDto();
            if (guid != null)
            {
                var entity = _documentLibraryService.GetFindById(Convert.ToInt32(guid));
                if (entity != null)
                {
                    model.Id = entity.Id;
                    model.DocumentTitle = entity.DocumentTitle;
                    model.DocumentType = entity.DocumentType;
                    model.Version = entity.Version;
                    model.FilePath = entity.FilePath;
                }
            }
            else
            {
                model.Version = "1.0";
            }
            return View(model);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult AddEdit(DocumentLibraryDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Files != null)
                    {
                        string folderPath = "Upload/LibraryDocument";
                        model.FilePath = UploadSaveFile(model.Files, folderPath, "V" + model.Version);
                        model.FilePath = model.FilePath.HasValue() ? $"{model.FilePath}" : null;
                    }
                    var result = _documentLibraryService.Save(model);

                    if (result != null && result.Id > 0)
                    {
                        ShowSuccessMessage("Success", "Document saved successfully", false);
                    }
                    else
                    {
                        ShowWarningMessage("error", "Unable to save record", false);
                    }
                }
                catch (Exception ex)
                {
                    ShowWarningMessage("error", ex.InnerException?.InnerException?.Message ?? ex.Message, false);
                }
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult DeleteRecord(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure you would like to delete this document?",
                Size = Enums.ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Document?" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult deleteRecord(int id)
        {
            try
            {
                if (id > 0)
                {
                    _documentLibraryService.Delete(id);
                    ShowSuccessMessage("Success", "Record deleted successfully.", false);
                }
                else
                {
                    ShowSuccessMessage("error", "Unable to delete this document.", true);
                }
            }
            catch (Exception ex)
            {
                ShowSuccessMessage("error", ex.Message, false);
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult ApprovedStatus(int id)
        {
            if (id > 0)
            {
                var entity = _documentLibraryService.GetFindById(id);
                if (entity != null)
                {
                    if (entity.IsActive == true)
                    {
                        entity.IsActive = false;
                    }
                    else
                    {
                        entity.IsActive = true;
                    }
                    _documentLibraryService.UpdateActiveDeactiveStatus(entity);
                }
            }
            //ShowSuccessMessage("Success", "Change Status successfully.", false);
            //return RedirectToAction("Index");
            return NewtonSoftJsonResult(new RequestOutcome<string>
            {
                IsSuccess = true,
                Message = "Change Status successfully."
            });
        }
        public FileResult Download(int id)
        {
            string fileSavePath = Path.Combine(_env.WebRootPath, "Upload", "LibraryDocument");
            if (id > 0)
            {
                var entity = _documentLibraryService.GetFindById(id);
                if (entity != null)
                {
                    var files = fileSavePath + "\\" + entity.FilePath;
                    byte[] fileBytes = System.IO.File.ReadAllBytes(files);
                    string fileName = entity.FilePath;
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
            }
            return null;
        }
        public string UploadSaveFile(IFormFile FileUpload, string Folder, string prefix)
        {
            if (FileUpload != null && FileUpload.Length > 0)
            {
                string fileName = FileUpload.FileName;
                string ext = Path.GetExtension(fileName.ToLower());
                fileName = Path.GetFileNameWithoutExtension(fileName);
                fileName = fileName.Replace(' ', '-');

                fileName = fileName + "_" + prefix + "_" + DateTime.Now.ToString("MM-dd-yyyy" + 'T' + "HH-mm-ss") + ext;

                string FilePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/" + Folder, fileName);
                using (var stream = new FileStream(FilePath, FileMode.Create))
                {
                    FileUpload.CopyTo(stream);
                }
                return fileName;
            }

            return String.Empty;
        }
    }
}
