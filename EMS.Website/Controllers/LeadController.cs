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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using EMS.Web.Models.Others;
//using PagedList.Mvc;
namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class LeadController : BaseController
    {
        #region "Fields"
        private ILeadServices leadServices;
        private readonly IDomainTypeService domainTypeService;
        private readonly ITechnologyService technologyService;
        #endregion
        #region "Constructor"
        public LeadController(ILeadServices leadServices, IDomainTypeService _domainTypeService, ITechnologyService _technologyService)
        {
            this.leadServices = leadServices;
            this.domainTypeService = _domainTypeService;
            this.technologyService = _technologyService;
        }
        #endregion

        // GET: Lead
        public ActionResult Index()
        {
            return View();
        }

        [CustomActionAuthorization]
        [HttpGet]
        public ActionResult EstimateDocuments()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EstimateDocuments(IDataTablesRequest request, string txtSearch, string doctype, string spam, int leadtype, bool isDSPhoto)
        {
            var pagingServices = new PagingService<EstimateDocument>(request.Start, request.Length);
            var expr = PredicateBuilder.True<EstimateDocument>();
            if (!String.IsNullOrEmpty(doctype))
            {
                switch (doctype)
                {
                    case "WM":
                        expr = expr.And(e => !String.IsNullOrEmpty(e.Wireframe_MockupsDoc));
                        break;
                    case "OD":
                        expr = expr.And(e => !String.IsNullOrEmpty(e.OtherDocument));
                        break;
                    case "PD":
                        expr = expr.And(e => !String.IsNullOrEmpty(e.DocumentPath));
                        break;
                    case "DP":
                        expr = expr.And(e => e.IsDSPhoto == true);
                        break;
                    case "FC":
                        expr = expr.And(e => !String.IsNullOrEmpty(e.Flowcharts));
                        break;
                }

            }
            if (leadtype == 27)
            {
                expr = expr.And(e => e.Lead != null && e.Lead.LeadType == leadtype);
            }
            if (!string.IsNullOrEmpty(txtSearch))
            {
                expr = expr.And(e => e.Title.Contains(txtSearch) || e.Tags.Contains(txtSearch) || e.DocumentPath.Contains(txtSearch) || e.Industry.Contains(txtSearch) || e.Technology.Contains(txtSearch) || e.UserLogin.Name.Contains(txtSearch));
            }
            bool isSpam = Convert.ToBoolean(Convert.ToInt32(spam));
            if (!isSpam)
            {
                expr = expr.And(e => e.IsSpam == false || e.IsSpam == null);
            }
            else
            {
                expr = expr.And(e => e.IsSpam == true);
            }
            expr = expr.And(e => !e.IsDelete);
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
                        case "industry":
                            return o.OrderByColumn(item, c => c.Industry);
                        default:
                            return o.OrderByColumn(item, c => c.Created);
                    }
                }
                return o.OrderByDescending(c => c.Created);
            };
            int totalCount = 0;
            bool isPMUser = CurrentUser.RoleId == (int)Enums.UserRoles.PM;
            var response = leadServices.GetEstimateDocByPaging(out totalCount, pagingServices);
            return DataTablesJsonResult(totalCount, request, response.Select((r, Index) => new
            {
                r.Id,
                rowIndex = (Index + 1) + (request.Start),
                r.Title,
                r.Tags,
                r.DocumentPath,
                Modified = r.Modified.ToString(),
                UploadedBy = r.Uid_UploadedBy != null ? r.UserLogin.Name : "",
                Technology = r.Technology != null ? r.Technology : string.Empty,
                Industry = (r.EstimateDocumentIndustry != null && r.EstimateDocumentIndustry.Count > 0) ? string.Join(",", r.EstimateDocumentIndustry.Where(x => x.Industry != null).Select(x => x.Industry.DomainName).ToList()) : "",
                leadId = r.LeadId.ToString(),
                r.MockupDocument,
                r.OtherDocument,
                r.Wireframe_MockupsDoc,
                r.Flowcharts,
                isPM = isPMUser,
                EstimateTimeInDays = r.EstimateTimeInDays != null ? (WeekAndDay(Convert.ToInt32(r.EstimateTimeInDays))).ToString() : ""


            }));
        }
        [HttpGet]
        public ActionResult UploadDocument(int? id)
        {
            UploadDocumentDto UploadDocumentViewModel = new UploadDocumentDto();
            ViewBag.Industries = domainTypeService.GetDomainList().Select(n => new SelectListItem { Text = n.DomainName, Value = n.DomainId.ToString() }).ToList();
            ViewBag.Technology = technologyService.GetTechnologyList().OrderBy(x => x.Title).Select(n => new SelectListItem { Text = n.Title, Value = n.TechId.ToString() }).ToList();
            if (id != null)
            {
                EstimateDocument estimateDocument = new EstimateDocument();
                estimateDocument = leadServices.GetEstimateById(Convert.ToInt32(id));
                if (estimateDocument != null)
                {
                    var industries = leadServices.GetEstimateDocumentIndustry(estimateDocument.Id);
                    if (industries.Count > 0)
                    {
                        UploadDocumentViewModel.Industry = string.Join(",", industries.Select(x => x.Industry.DomainName).ToList());
                    }
                    else
                    {
                        UploadDocumentViewModel.Industry = estimateDocument.Industry != null ? estimateDocument.Industry : "";
                    }
                    UploadDocumentViewModel.EstimateDocumentId = estimateDocument.Id;
                    UploadDocumentViewModel.Title = estimateDocument.Title;
                    UploadDocumentViewModel.Technology = estimateDocument.Technology;
                    UploadDocumentViewModel.Tags = estimateDocument.Tags;
                    UploadDocumentViewModel.EstimateDocPath = estimateDocument.DocumentPath;
                    UploadDocumentViewModel.Flowcharts = estimateDocument.Flowcharts;
                    UploadDocumentViewModel.MockupDoc = estimateDocument.MockupDocument;
                    UploadDocumentViewModel.OtherDoc = estimateDocument.OtherDocument;
                    UploadDocumentViewModel.WireformMockupDoc = estimateDocument.Wireframe_MockupsDoc;
                    UploadDocumentViewModel.EstimateTimeinDays = estimateDocument.EstimateTimeInDays;
                    UploadDocumentViewModel.IsSpam = Convert.ToBoolean(estimateDocument.IsSpam != null ? estimateDocument.IsSpam : false);
                    UploadDocumentViewModel.IsDSPhoto = Convert.ToBoolean(estimateDocument.IsDSPhoto != null ? estimateDocument.IsDSPhoto : false);
                }

            }
            return PartialView("_AddEditEstimateDocument", UploadDocumentViewModel);
        }
        [HttpPost]
        public ActionResult UploadDocument(UploadDocumentDto uploadDocumentViewModel, IFormFile EstimateDocPath, IFormFile MockupDoc, IFormFile OtherDoc, IFormFile WireformMockupDoc, IFormFile FlowchartsDoc)
        {
            if (ModelState.IsValid && CurrentUser != null && CurrentUser.Uid != 0)
            {

                int size = 0;
                string[] requiredExtension;
                bool hasFile = false;

                if (EstimateDocPath != null || MockupDoc != null || OtherDoc != null || WireformMockupDoc != null || FlowchartsDoc != null)
                {
                    hasFile = true;
                }
                if (!hasFile && uploadDocumentViewModel.EstimateDocumentId == 0)
                {
                    return Json(new { isSuccess = false, message = "Upload atleast 1 document" });
                }
                else
                {
                    #region "Image size and type Validation"
                    if ((hasFile && uploadDocumentViewModel.EstimateDocumentId == 0) || (hasFile && uploadDocumentViewModel.EstimateDocumentId != 0))
                    {

                        //if (ConfigurationManager.AppSettings["size"] != null && ConfigurationManager.AppSettings["extension"] != null && ConfigurationManager.AppSettings["ImageExtension"] != null)
                        if (SiteKey.Size != null && SiteKey.Extension != null && SiteKey.ImageExtension != null)
                        {
                            //size = Convert.ToInt32(ConfigurationManager.AppSettings["size"]);
                            size = Convert.ToInt32(SiteKey.Size);
                            //requiredExtension = ConfigurationManager.AppSettings["extension"].Split(',');
                            requiredExtension = SiteKey.Extension.Split(',');
                            if (EstimateDocPath != null)
                            {
                                var fileExt = Path.GetExtension(EstimateDocPath.FileName.ToLower());
                                if (!requiredExtension.Contains(fileExt))
                                {
                                    return Json(new { isSuccess = false, message = "Uploaded Estimate document format is not valid" });
                                }
                                else if (EstimateDocPath.Length > size)
                                {
                                    return Json(new { isSuccess = false, message = "Uploaded Estimate document size should not be grater than 200 MB" });
                                }
                            }
                            if (MockupDoc != null)
                            {
                                var fileExt = Path.GetExtension(MockupDoc.FileName.ToLower());
                                if (!requiredExtension.Contains(fileExt))
                                {
                                    return Json(new { isSuccess = false, message = "Uploaded Mockup document format is not valid" });
                                }
                                else if (MockupDoc.Length > size)
                                {
                                    return Json(new { isSuccess = false, message = "Uploaded Mockup document size should not be grater than 200 MB" });
                                }
                            }
                            if (OtherDoc != null)
                            {
                                var fileExt = Path.GetExtension(OtherDoc.FileName.ToLower());
                                //string[] imageExt = ConfigurationManager.AppSettings["ImageExtension"].Split(',');
                                string[] imageExt = SiteKey.ImageExtension.Split(',');
                                string[] allExtension = new string[requiredExtension.Length + imageExt.Length];
                                requiredExtension.CopyTo(allExtension, 0);
                                imageExt.CopyTo(allExtension, requiredExtension.Length);
                                if (!allExtension.Contains(fileExt))
                                {
                                    return Json(new { isSuccess = false, message = "Uploaded Other document format is not valid" });
                                }
                                else if (OtherDoc.Length > size)
                                {
                                    return Json(new { isSuccess = false, message = "Uploaded Other document size should not be grater than 200 MB" });
                                }
                            }
                            if (FlowchartsDoc != null)
                            {
                                var fileExt = Path.GetExtension(FlowchartsDoc.FileName.ToLower());
                                if (!requiredExtension.Contains(fileExt))
                                {
                                    return Json(new { isSuccess = false, message = "Uploaded Flowchart document format is not valid" });
                                }
                                else if (FlowchartsDoc.Length > size)
                                {
                                    return Json(new { isSuccess = false, message = "Uploaded Flowchart document size should not be grater than 200 MB" });
                                }
                            }
                            //string[] ImageExtension = ConfigurationManager.AppSettings["ImageExtension"].Split(',');
                            string[] ImageExtension = SiteKey.ImageExtension.Split(',');

                            if (WireformMockupDoc != null)
                            {
                                var fileExt = Path.GetExtension(WireformMockupDoc.FileName.ToLower());
                                if (!ImageExtension.Contains(fileExt))
                                {
                                    return Json(new { isSuccess = false, message = "Uploaded Wireform/Mockup(image) supports only Image format" });
                                }
                                else if (WireformMockupDoc.Length > size)
                                {
                                    return Json(new { isSuccess = false, message = "Uploaded Wireform/Mockup(image) size should not be grater than 200 MB" });
                                }
                            }
                        }
                    }
                    #endregion


                    EstimateDocument estimateDocument = new EstimateDocument();
                    if (uploadDocumentViewModel.EstimateDocumentId != 0)
                    {
                        estimateDocument = leadServices.GetEstimateById(Convert.ToInt32(uploadDocumentViewModel.EstimateDocumentId));
                    }
                    estimateDocument.Title = uploadDocumentViewModel.Title;
                    estimateDocument.Tags = uploadDocumentViewModel.Tags;
                    estimateDocument.Technology = uploadDocumentViewModel.Technology != null ? uploadDocumentViewModel.Technology : string.Empty;
                    estimateDocument.Created = DateTime.Now;
                    estimateDocument.Modified = DateTime.Now;
                    estimateDocument.Uid_UploadedBy = CurrentUser.Uid;
                    estimateDocument.EstimateTimeInDays = uploadDocumentViewModel.EstimateTimeinDays;
                    estimateDocument.IsSpam = uploadDocumentViewModel.IsSpam;
                    estimateDocument.IsDSPhoto = uploadDocumentViewModel.IsDSPhoto;
                    if (EstimateDocPath != null)
                    {
                        string estimateDoc = GeneralMethods.SaveFile(EstimateDocPath, "Upload/EstimateDocument/", "");
                        estimateDocument.DocumentPath = estimateDoc;
                    }
                    if (MockupDoc != null)
                    {
                        string mockupDoc = GeneralMethods.SaveFile(MockupDoc, "Upload/EstimateDocument/", "");
                        estimateDocument.MockupDocument = mockupDoc;
                    }
                    if (WireformMockupDoc != null)
                    {
                        string wireframeMockupDoc = GeneralMethods.SaveFile(WireformMockupDoc, "Upload/EstimateDocument/", "");
                        estimateDocument.Wireframe_MockupsDoc = wireframeMockupDoc;
                        //estimateDocument
                    }
                    if (OtherDoc != null)
                    {
                        string otherDoc = GeneralMethods.SaveFile(OtherDoc, "Upload/EstimateDocument/", "");
                        estimateDocument.OtherDocument = otherDoc;
                    }
                    if (FlowchartsDoc != null)
                    {
                        string flowchartDoc = GeneralMethods.SaveFile(FlowchartsDoc, "Upload/EstimateDocument/", "");
                        estimateDocument.Flowcharts = flowchartDoc;
                    }

                    leadServices.Save(estimateDocument);

                    if (uploadDocumentViewModel.Domains != null && uploadDocumentViewModel.Domains.Length > 0)
                    {
                        List<EstimateDocumentIndustry> estimateDocumentIndustries = new List<EstimateDocumentIndustry>();
                        foreach (var industry in uploadDocumentViewModel.Domains)
                        {
                            var domain = domainTypeService.GetDomainByName(industry);
                            if (domain != null)
                            {
                                estimateDocumentIndustries.Add(new EstimateDocumentIndustry { EstimateDocumentId = estimateDocument.Id, IndustryId = domain.DomainId });
                            }
                        }
                        leadServices.SaveEstimateDocumentIndustry(estimateDocumentIndustries);
                    }
                    ShowSuccessMessage("Success", "Estimate Document has been successfully added", false);
                }


                return Json(new { isSuccess = true, redirectUrl = Url.Action("EstimateDocuments", "Lead") });
            }
            else
            {
                return Json(new { isSuccess = false, message = "Please fill required(*) data" });
            }

        }

        [HttpPost]
        public ActionResult DeleteDocument(int estimateId, string documentId)
        {
            EstimateDocument estimateDocument = new EstimateDocument();
            estimateDocument = leadServices.GetEstimateById(estimateId);
            string message = "";
            bool isSuccess = false;
            switch (documentId)
            {
                case "lnkDelProposalDoc":
                    if (!String.IsNullOrEmpty(estimateDocument.Wireframe_MockupsDoc) || !String.IsNullOrEmpty(estimateDocument.MockupDocument) || !String.IsNullOrEmpty(estimateDocument.OtherDocument) || !String.IsNullOrEmpty(estimateDocument.Flowcharts))
                    {
                        estimateDocument.DocumentPath = string.Empty;
                        message = "File has been successfully deleted";
                        isSuccess = true;
                    }
                    else
                    {
                        message = "File can not be deleted. Atleast one file required";
                        isSuccess = false;
                    }
                    break;
                case "lnkDelWireframeMockupDoc":
                    if (!String.IsNullOrEmpty(estimateDocument.DocumentPath) || !String.IsNullOrEmpty(estimateDocument.MockupDocument) || String.IsNullOrEmpty(estimateDocument.OtherDocument) || !String.IsNullOrEmpty(estimateDocument.Flowcharts))
                    {
                        estimateDocument.Wireframe_MockupsDoc = string.Empty;
                        message = "File has been successfully deleted";
                        isSuccess = true;
                    }
                    else
                    {
                        message = "File can not be deleted. Atleast one file required";
                        isSuccess = false;
                    }
                    message = "File can not be deleted";
                    break;
                case "lnkDelOtherDoc":
                    if (!String.IsNullOrEmpty(estimateDocument.DocumentPath) || !String.IsNullOrEmpty(estimateDocument.Wireframe_MockupsDoc) || !String.IsNullOrEmpty(estimateDocument.MockupDocument) || !String.IsNullOrEmpty(estimateDocument.Flowcharts))
                    {
                        estimateDocument.OtherDocument = string.Empty;
                        message = "File has been successfully deleted";
                        isSuccess = true;
                    }
                    else
                    {
                        message = "File can not be deleted. Atleast one file required";
                        isSuccess = false;
                    }
                    break;
                case "lnkDelMockupDoc":
                    if (!String.IsNullOrEmpty(estimateDocument.DocumentPath) || !String.IsNullOrEmpty(estimateDocument.Wireframe_MockupsDoc) || !String.IsNullOrEmpty(estimateDocument.OtherDocument) || !String.IsNullOrEmpty(estimateDocument.Flowcharts))
                    {
                        estimateDocument.MockupDocument = string.Empty;
                        message = "File has been successfully deleted";
                        isSuccess = true;
                    }
                    else
                    {
                        message = "File can not be deleted. Atleast one file required";
                        isSuccess = false;
                    }
                    break;
                case "lnkDelFlowcharts":
                    if (!String.IsNullOrEmpty(estimateDocument.DocumentPath) || !String.IsNullOrEmpty(estimateDocument.Wireframe_MockupsDoc) || !String.IsNullOrEmpty(estimateDocument.OtherDocument) || !String.IsNullOrEmpty(estimateDocument.MockupDocument))
                    {
                        estimateDocument.Flowcharts = string.Empty;
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
            leadServices.Save(estimateDocument);

            return Json(new { isSuccess, message, redirectUrl = Url.Action("UploadDocument", "Lead", new { id = estimateId }), updateTargetId = "error-ModalMessage" });
        }
        [HttpGet]
        public ActionResult Delete()
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Estimate document?",
                Header = new ModalHeader { Heading = "Delete Estimate Document" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No", DefaultButtonCss = true }
            });
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string message = "";
            bool isSuccess = false;
            try
            {

                if (id > 0)
                {
                    EstimateDocument estimateDocument = new EstimateDocument();
                    estimateDocument = leadServices.GetEstimateById(id);

                    if (estimateDocument != null)
                    {
                        estimateDocument.IsDelete = true;
                        leadServices.Save(estimateDocument);
                        isSuccess = true;
                    }
                    ShowSuccessMessage("Success", "Estimate Document has been successfully delete", false);
                    return Json(new { isSuccess, message, redirectUrl = Url.Action("EstimateDocuments", "Lead"), updateTargetId = "divMessage" });
                }
                else
                {
                    ShowErrorMessage("Error", "Delete operation failed", false);
                    return Json(new { isSuccess, message, redirectUrl = Url.Action("EstimateDocuments", "Lead"), updateTargetId = "divMessage" });
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error", "Delete operation failed", false);
                return Json(new { isSuccess, message, redirectUrl = Url.Action("EstimateDocuments", "Lead"), updateTargetId = "divMessage" });
            }
        }

        public ActionResult GetWireframeMockupImages(int? page)
        {
            List<EstimateDocument> estimateDocumentList = new List<EstimateDocument>();
            estimateDocumentList = leadServices.GetEstimatesDocuments();
            estimateDocumentList = estimateDocumentList.Where(x => !String.IsNullOrEmpty(x.Wireframe_MockupsDoc) && x.IsDelete != true).ToList();
            List<WireframeMockupsImage> wireframeMockupsImagesList = new List<WireframeMockupsImage>();
            estimateDocumentList.ForEach(a =>
            {
                WireframeMockupsImage wireframeMockupsImages = new WireframeMockupsImage();
                wireframeMockupsImages.id = a.Id;
                wireframeMockupsImages.Title = a.Title;
                wireframeMockupsImages.Tags = a.Tags;
                wireframeMockupsImages.UploadedBy = a.UserLogin.Name;
                wireframeMockupsImages.Uid = a.Uid_UploadedBy;
                wireframeMockupsImages.Industry = a.Industry;
                wireframeMockupsImages.Technology = a.Technology;
                wireframeMockupsImages.WireformMockupDoc = a.Wireframe_MockupsDoc;
                wireframeMockupsImages.Created = a.Created.ToString();
                wireframeMockupsImages.EstimateTimeinDays = WeekAndDay(Convert.ToInt32(a.EstimateTimeInDays));
                wireframeMockupsImages.LeadId = Convert.ToInt32(a.LeadId);
                wireframeMockupsImagesList.Add(wireframeMockupsImages);
            });
            int pageSize = 12;
            int PageNumber = (page ?? 1);

            return PartialView("_WireframeMockupsImages", wireframeMockupsImagesList.ToPagedList(PageNumber, pageSize));

        }


        public string WeekAndDay(int noofday)
        {
            int weeks = noofday / 5;
            int days = noofday % 5;
            string result = ""; ;
            string weektypes = "Weeks";
            string daytypes = "Days";

            if (days == 1) { daytypes = "Day"; }
            if (weeks == 1) { weektypes = "Week"; }

            if (days == 0) { daytypes = ""; }
            if (weeks == 0) { weektypes = ""; }

            if (weeks == 0)
                result = days + " " + daytypes;
            if (days == 0)
                result = weeks.ToString() + " " + weektypes;
            if (weeks != 0 && days != 0)
                result = weeks.ToString() + " " + weektypes + "  " + days + " " + daytypes;
            if (weeks == 0 && days == 0)
                result = "";

            return result;
        }


    }
}