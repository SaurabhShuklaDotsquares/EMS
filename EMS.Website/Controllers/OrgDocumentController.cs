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
    public class OrgDocumentController : BaseController
    {
        #region "Fields"

        private readonly IOrgDocumentService orgDocumentService;
        private readonly IDepartmentService departmentService;
        private readonly IRoleService roleService;
        private readonly IUserLoginService userLoginService;

        #endregion

        #region "Constructor"

        public OrgDocumentController(IOrgDocumentService _orgDocumentService, IDepartmentService _departmentService, IRoleService _roleService, IUserLoginService _userLoginService)
        {
            orgDocumentService = _orgDocumentService;
            departmentService = _departmentService;
            roleService = _roleService;
            userLoginService = _userLoginService;
        }

        #endregion

        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                var companyDocuments = orgDocumentService.GetOrgDocumentsByRoles(IfAshishTeamPMUId, CurrentUser.DeptId, CurrentUser.RoleId, DocumentForAshishTeamOnly);
                return View(companyDocuments);
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.Message);
            }
        }

        [HttpGet]
        [CustomActionAuthorization(IsSPEGUser: true)]
        public ActionResult AddEdit(int? id)
        {
            var model = new OrgDocumentDto
            {
                AllowEdit = true
            };

            model.DocumentTypeList = Extensions.EnumToDictionaryWithDescription(typeof(Enums.OrgDocumentType))
                                               .Select(x => new SelectListItem { Text = x.Key, Value = x.Value.ToString() })
                                               .ToList();

            model.DepartmentList = departmentService.GetActiveDepartments()
                                    .Select(x => new SelectListItem { Text = x.Name, Value = x.DeptId.ToString() })
                                    .ToList();

            model.RoleList = roleService.GetActiveRoles()
                                    .Select(x => new SelectListItem { Text = x.RoleName, Value = x.RoleId.ToString() })
                                    .ToList();

            if (id.HasValue && id.Value > 0)
            {
                var docEntity = orgDocumentService.GetOrgDocumentById(id.Value);
                if (docEntity != null)
                {
                    model.Id = docEntity.Id;
                    model.DocType = docEntity.OrgDocumentMaster.DocType;
                    model.OrgDocumentMasterId = docEntity.OrgDocumentMasterId;
                    model.IsMajorVer = docEntity.IsMajorVer;
                    model.IsSendEmail = docEntity.IsSendEmail;
                    // model.RoleIds = docEntity.Roles.Select(x => x.RoleId).ToArray();
                    model.RoleIds = docEntity.OrgDocumentRole.Select(x => x.RoleId).ToArray();

                    //model.DepartmentIds = docEntity.Departments.Select(x => x.DeptId).ToArray();
                    model.DepartmentIds = docEntity.OrgDocumentDepartment.Select(x => x.DepartmentId).ToArray();

                    model.DocumentPath = docEntity.DocumentPath;
                    model.HighLevelChanges = docEntity.HighLevelChanges;
                    model.IsApproved = docEntity.IsApproved;
                    model.AllowEdit = docEntity.CreateByUid == CurrentUser.Uid && (!docEntity.IsApproved || !docEntity.OrgDocumentApproves.Any(x => x.IsApproved));
                    model.CreateByUid = docEntity.CreateByUid;
                    model.OrgDocumentMasterName = $"{docEntity.OrgDocumentMaster.Name} v{docEntity.Ver}";
                    model.OrgDocumentMasterList = orgDocumentService.GetOrgDocumentMasters(docEntity.OrgDocumentMaster.DocType)
                                                    .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                                                    .ToList();
                }
                else
                {
                    return MessagePartialView("No document found");
                }

            }

            return View(model);

        }

        [HttpPost]
        [CustomActionAuthorization(IsSPEGUser: true)]
        public ActionResult AddEdit(OrgDocumentDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Id > 0)
                    {
                        var docEntity = orgDocumentService.GetOrgDocumentById(model.Id);

                        if (docEntity != null)
                        {
                            if (docEntity.CreateByUid == CurrentUser.Uid && !docEntity.IsApproved)
                            {
                                if (!docEntity.OrgDocumentApproves.Any(x => x.IsApproved))
                                {
                                    if (model.Document != null && model.Document.Length > 0)
                                    {
                                        UploadDoc(model);

                                        docEntity.DocumentPath = !string.IsNullOrWhiteSpace(model.DocumentPath) ? model.DocumentPath : docEntity.DocumentPath;
                                        docEntity.Ver = !string.IsNullOrWhiteSpace(model.Ver) ? model.Ver : docEntity.Ver;
                                    }

                                    docEntity.OrgDocumentMasterId = model.OrgDocumentMasterId;
                                    docEntity.IsMajorVer = model.IsMajorVer;
                                }

                                docEntity.IsSendEmail = model.IsSendEmail;
                                docEntity.HighLevelChanges = model.HighLevelChanges;
                            }

                            orgDocumentService.Update(docEntity, model.RoleIds, model.DepartmentIds);

                            ShowSuccessMessage("Success", $"Document updated successfully", false);
                            return NewtonSoftJsonResult(new RequestOutcome<string>
                            {
                                IsSuccess = true,
                                RedirectUrl = Url.Action("AddEdit", new { id = docEntity.Id })
                            });
                        }
                        else
                        {
                            return MessagePartialView("Unable to get document");
                        }
                    }
                    else
                    {
                        model.CreateByUid = CurrentUser.Uid;

                        if (model.Document != null && model.Document.Length > 0)
                        {
                            UploadDoc(model);

                            var resultEntity = orgDocumentService.Save(model);

                            if (resultEntity != null && resultEntity.Id > 0)
                            {
                                if (resultEntity.OrgDocumentApproves.Any())
                                {
                                    //SendEmailToReviewers(resultEntity);
                                }

                                ShowSuccessMessage("Success", $"Document : {model.OrgDocumentMasterName} v{model.Ver} added successfully", false);
                                return NewtonSoftJsonResult(new RequestOutcome<string>
                                {
                                    IsSuccess = true,
                                    RedirectUrl = Url.Action("AddEdit")
                                });
                            }
                            else
                            {
                                return MessagePartialView("Unable to save document");
                            }
                        }
                        else
                        {
                            return MessagePartialView("Document not found to upload");
                        }
                    }

                }
                catch (Exception ex)
                {
                    return MessagePartialView(ex.Message);
                }
            }
            else
            {
                return CreateModelStateErrors();
            }
        }

        [HttpGet]
        public ActionResult GetDocumentMaster(byte id)
        {
            var masters = orgDocumentService.GetOrgDocumentMasters(id)
                                         .Where(x => (!DocumentForAshishTeamOnly.Contains(x.Id) || (DocumentForAshishTeamOnly.Contains(x.Id) && IfAshishTeamPMUId))
                    )                    .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                                         .ToList();
            return NewtonSoftJsonResult(new RequestOutcome<List<SelectListItem>> { IsSuccess = true, Data = masters });
        }

        [HttpGet]
        public ActionResult GetDocumentByMasterId(int id)
        {
            OrgDocumentDto data = null;
            if (IsAjaxRequest)
            {
                var document = orgDocumentService.GetBaselineOrgDocumentByMasterId(id);

                if (document != null)
                {
                    data = new OrgDocumentDto
                    {
                        Id = document.Id,
                        DocumentPath = document.DocumentPath,
                        Ver = document.Ver,
                        OrgDocumentName = document.OrgDocumentMaster.Name,
                        DocTypeName = ((Enums.OrgDocumentType)document.OrgDocumentMaster.DocType).GetEnumDisplayName(),
                        ApprovedDate = document.ApprovedDate.ToFormatDateString("dd/MM/yyyy hh:mm tt"),
                        CreateBy = document.UserLogin.Name,
                        //RoleIds = document.Roles.Select(x => x.RoleId).ToArray(),
                        //DepartmentIds = document.Departments.Select(x => x.DeptId).ToArray()
                        RoleIds = document.OrgDocumentRole.Select(x => x.RoleId).ToArray(),
                        DepartmentIds = document.OrgDocumentDepartment.Select(x => x.DepartmentId).ToArray()

                    };
                }
            }

            return NewtonSoftJsonResult(new RequestOutcome<OrgDocumentDto> { IsSuccess = true, Data = data });
        }

        [HttpGet]
        [CustomActionAuthorization(IsSPEGUser: true)]
        public ActionResult Review()
        {
            return View();
        }

        [HttpPost]
        [CustomActionAuthorization(IsSPEGUser: true)]
        public IActionResult Review(IDataTablesRequest request)
        {
            var pagingServices = new PagingService<OrgDocument>(request.Start, request.Length);
            var filterExpr = PredicateBuilder.True<OrgDocument>();

            pagingServices.Filter = filterExpr.And(x => x.OrgDocumentApproves.Any());
            pagingServices.Filter = filterExpr.And(x => (!DocumentForAshishTeamOnly.Contains(x.OrgDocumentMasterId) || (DocumentForAshishTeamOnly.Contains(x.OrgDocumentMasterId) && IfAshishTeamPMUId)));

            pagingServices.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "Name":
                            return o.OrderByColumn(item, c => c.UserLogin.Name);

                        default:
                            return o.OrderByColumn(item, c => c.IsApproved).ThenByDescending(c => c.ApprovedDate);

                    }
                }
                return o.OrderByDescending(c => c.CreateDate);
            };

            int totalCount = 0;
            var documents = orgDocumentService.GetOrgDocByPaging(out totalCount, pagingServices);

            return DataTablesJsonResult(totalCount, request, documents.Select((d, index) => new
            {
                d.Id,
                rowIndex = (index + 1) + (request.Start),
                Document = d.OrgDocumentMaster.Name,
                DocumentPath = d.DocumentPath,
                Status = d.IsApproved ? "Approved" : "Pending",
                ApprovedDate = d.ApprovedDate.ToFormatDateString("dd MMM yyyy, hh:mm tt"),
                Version = d.Ver,
                Reviewers = string.Join("<br>", d.OrgDocumentApproves.Select(r => string.Format("{0} ({1})", r.UserLogin.Name, r.IsApproved ?
                                $"Approved {r.ApprovedDate.ToFormatDateString("dd MMM yyyy, hh:mm tt")}" : "Pending"))),
                AllowApproval = !d.IsApproved && d.OrgDocumentApproves.Any(r => r.ApproverUid == CurrentUser.Uid),
                SelfApproved = d.OrgDocumentApproves.Any(r => r.IsApproved && r.ApproverUid == CurrentUser.Uid),
                //Departments = d.Departments.Any() ? string.Join(", ", d.Departments.Select(x => x.Name)) : null,
                //Roles = d.Roles.Any() ? string.Join(", ", d.Roles.Select(x => x.RoleName)) : null,
                Departments = d.OrgDocumentDepartment.Any() ? string.Join(", ", d.OrgDocumentDepartment.Select(x => x.Department.Name)) : null,
                Roles = d.OrgDocumentRole.Any() ? string.Join(", ", d.OrgDocumentRole.Select(x => x.Role.RoleName)) : null,
            }));
        }

        [HttpGet]
        [CustomActionAuthorization(IsSPEGUser: true)]
        public ActionResult OrgDocApprove(int id)
        {
            if (id > 0)
            {
                OrgDocumentApproveDto model = new OrgDocumentApproveDto
                {
                    OrgDocId = id
                };

                return PartialView("_OrgDocApproved", model);
            }
            else
            {
                return MessagePartialView("Invalid document id");
            }
        }

        [HttpGet]
        [CustomActionAuthorization(IsSPEGUser: true)]
        public ActionResult OrgDocRevisionHistory(int id)
        {
            if (id > 0)
            {
                var archivedDocs = orgDocumentService.GetOrgDocumentHistoryByMasterId(id);

                return PartialView("_OrgDocRevisionHistory", archivedDocs);
            }
            else
            {
                return MessagePartialView("Invalid master id");
            }
        }

        [HttpPost]
        [CustomActionAuthorization(IsSPEGUser: true)]
        public ActionResult OrgDocApprove(OrgDocumentApproveDto model)
        {
            try
            {
                if (model.OrgDocId > 0 && !string.IsNullOrWhiteSpace(model.Comments))
                {
                    model.ApproverUid = CurrentUser.Uid;
                    var result = orgDocumentService.UpdateApproveStatus(model);

                    if (result != null)
                    {
                        if (result.IsBaseline && result.IsApproved)
                        {
                            //SendEmailToUsers(result);
                        }

                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = $"Approvel on {result.OrgDocumentMaster.Name} v{result.Ver} has been recorded", IsSuccess = true });
                    }
                    else
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Unable to approve document", IsSuccess = true });
                    }
                }
                else
                {
                    return MessagePartialView("Invalid parameters");
                }
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.Message);
            }
        }

        private string GenerateVersion(string currentVersion, bool isMajor)
        {
            if (!string.IsNullOrWhiteSpace(currentVersion))
            {
                currentVersion = currentVersion.Replace("ver.", "").Replace("ver", "").Replace("v", "");
                decimal ver = Convert.ToDecimal(currentVersion);

                currentVersion = (isMajor ? (ver + 1) : (ver + 0.1m)).ToString();

                if (currentVersion.EndsWith(".00"))
                {
                    return currentVersion.Replace(".00", ".0");
                }
                return currentVersion;
            }
            return "1.0";
        }

        private void UploadDoc(OrgDocumentDto model)
        {
            var baselineDoc = orgDocumentService.GetOrgDocumentByMasterId(model.OrgDocumentMasterId, model.Id > 0 ? model.Id : (int?)null);
            model.OrgDocumentMasterName = baselineDoc?.OrgDocumentMaster.Name ?? orgDocumentService.GetOrgDocumentMasterById(model.OrgDocumentMasterId).Name;
            model.Ver = GenerateVersion(baselineDoc?.Ver, model.IsMajorVer);
            string fileExt = System.IO.Path.GetExtension(model.Document.FileName.ToLower());
            //model.DocumentPath = GeneralMethods.SaveFile(model.Document,"Upload/OrgDocument/", string.Empty);
            string fileName = $"{model.OrgDocumentMasterName.ToSelfURL()}_v{model.Ver}_{DateTime.Now.Ticks}{fileExt}";
            model.DocumentPath = $"Upload/OrgDocument/{fileName}";
            string FilePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/" + "Upload/OrgDocument/", fileName);
            using (var stream = new FileStream(FilePath, FileMode.Create))
            {
                model.Document.CopyTo(stream);
            }
          //  model.Document.SaveAs(Server.MapPath("~/" + model.DocumentPath));
        }

        private void SendEmailToReviewers(OrgDocument entity)
        {
            try
            {
                FlexiMail objSendMail = new FlexiMail();
                objSendMail.ValueArray = new string[]
                {
                        entity.OrgDocumentMaster.Name,
                        entity.Ver,
                        entity.UserLogin.Name,
                        entity.CreateDate.ToFormatDateString(),
                        $"{SiteKey.DomainName}{entity.DocumentPath}"
                };
                objSendMail.Subject = $"Document Review - {entity.OrgDocumentMaster.Name}";
                objSendMail.MailBodyManualSupply = true;
                objSendMail.MailBody = objSendMail.GetHtml("OrgDocReview.html");
                objSendMail.To = string.Join(";", entity.OrgDocumentApproves.Select(x => x.UserLogin.EmailOffice));
                objSendMail.From = SiteKey.From;
                objSendMail.Send();
            }
            catch
            {

            }
        }

        private void SendEmailToUsers(OrgDocument entity)
        {
            try
            {
                // var emailIds = userLoginService.GetUserEmailIdsByRolesAndDepartments(entity.Roles.Select(x => x.RoleId).ToArray(), entity.Departments.Select(x => x.DeptId).ToArray());
                var emailIds = userLoginService.GetUserEmailIdsByRolesAndDepartments(entity.OrgDocumentRole.Select(x => x.RoleId).ToArray(), entity.OrgDocumentDepartment.Select(x => x.DepartmentId).ToArray());

                if (emailIds.Any())
                {
                    FlexiMail objSendMail = new FlexiMail();
                    objSendMail.ValueArray = new string[]
                    {
                       $"{entity.OrgDocumentMaster.Name} {entity.Ver}",
                       $"{SiteKey.DomainName}{entity.DocumentPath}",
                       entity.ApprovedDate.ToFormatDateString("dd MMM yyyy")
                    };
                    objSendMail.Subject = $"{entity.OrgDocumentMaster.Name} Document available for download";
                    objSendMail.MailBodyManualSupply = true;
                    objSendMail.MailBody = objSendMail.GetHtml("OrgDocumentApproved.html");
                    objSendMail.To = entity.UserLogin.EmailOffice;
                    objSendMail.BCC = string.Join(";", emailIds);
                    objSendMail.From = SiteKey.From;
                    objSendMail.Send();
                }
            }
            catch
            {
             
            }
        }


        private bool IfAshishTeamPMUId { get { return (CurrentUser.PMUid == SiteKey.AshishTeamPMUId || CurrentUser.Uid == SiteKey.AshishTeamPMUId) ? true : false; }}
        private int[] DocumentForAshishTeamOnly { get{return (String.IsNullOrEmpty(SiteKey.DocumentForAshishTeamOnly) == true ? new int[0] : SiteKey.DocumentForAshishTeamOnly.Split(',').Select(h => Int32.Parse(h)).ToArray()); }}

    }
}