using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Data.Model;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.Controllers;
using EMS.Website.Models.StudyDocuments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using static EMS.Core.Enums;

namespace EMS.Website.Controllers
{
    [CustomAuthorization]
    public class StudyDocumentsController : BaseController
    {
        private readonly int PageDataLength = 12;
        private readonly IStudyDocumentsService _studyDocumentsService;
        private readonly IUserLoginService _userLoginService;
        private readonly ITechnologyService _technologyService;

        public StudyDocumentsController(IStudyDocumentsService studyDocumentsService
            , IUserLoginService userLoginService
            , ITechnologyService technologyService)
        {
            _studyDocumentsService = studyDocumentsService;
            _userLoginService = userLoginService;
            _technologyService = technologyService;
        }

        // list
        public IActionResult Index()
        {
            ViewBag.TechnologyList = _technologyService.GetTechnologyList().OrderBy(x=>x.Title).
                        Select(n => new SelectListItem
                        {
                            Text = n.Title,
                            Value = n.TechId.ToString(),
                            //Selected = n.Id == model.TechnologyId
                        }).ToList();
            var hasPermission = HasPermission();
            return View(hasPermission);
        }
        // list
        [HttpPost]
        public IActionResult Index(IDataTablesRequest request, string TechnologyId)
        {
            try
            {
                var pagingServices = new PagingService<StudyDocuments>(request.Start, request.Length);
                var filterExpr = PredicateBuilder.True<StudyDocuments>();

                // filter
                filterExpr = filterExpr.And(x => x.Isdelete != true);

                // dir. view all
                if (CurrentUser.RoleId == (int)UserRoles.Director)
                {
                    // do nothing
                }
                else if (HasHRBPPermission())// sr. hr view all
                {
                    // do nothing
                }
                else if (CurrentUser.RoleId == (int)UserRoles.PM)// pm team users and self
                {
                    var pmTeamUser = _userLoginService.GetUsersByPM(CurrentUser.Uid).Select(x => x.Uid);
                    pmTeamUser.Append(CurrentUser.Uid);
                    filterExpr = filterExpr.And(x => pmTeamUser.Contains(x.Addedby));
                }
                else// remaning user
                {
                    filterExpr = filterExpr.And(x => x.Addedby == CurrentUser.Uid);
                }

                // search
                if (!string.IsNullOrEmpty(request.Search.Value))
                {
                    var search = request.Search.Value.ToString().Trim().ToLower();
                    filterExpr = filterExpr.And(x => x.Title.Trim().ToLower().Contains(search));
                }

                // technology ids 
                if (!string.IsNullOrWhiteSpace(TechnologyId))
                {
                    var ids = TechnologyId.Split(',');
                    filterExpr = filterExpr.And(x => ids.Contains(x.Technologyid.ToString()));
                }

                // pagging services
                pagingServices.Filter = filterExpr;
                pagingServices.Sort = (o) =>
                {
                    return o.OrderByDescending(c => c.Id);
                };

                // datatable
                int totalCount = 0;
                var entity = _studyDocumentsService.GetStudyDocuments(pagingServices, out totalCount);
                var dataTable = DataTablesJsonResult(totalCount, request, entity.Select((d, index) =>
                {
                    var detail = new
                    {
                        rowIndex = (index + 1) + (request.Start),
                        keyId = d.Keyid,
                        id = d.Id,
                        title = d.Title.Trim().ToTitleCase(),
                        description = d.Description?.TrimLength(100),
                        addedDate = d.Addeddate?.Date.ToFormatDateString("MMM, dd yyyy hh:mm tt"),
                        addedBy = _userLoginService.GetUserInfoByID(d.Addedby).Name.ToTitleCase(),
                        isApproved = d.Isapproved.ToString().ToLower(),
                        unApprovedReason = d.Unapprovedreson ?? "NA",
                        isActive = d.Isactive.ToString().ToLower(),
                        documentCount = d.StudyDocumentFiles.Count,
                        technology = d.Technology.Title,
                    };

                    return detail;

                }));

                return dataTable;
            }
            catch (Exception ex)
            {

            }
            return Ok();
        }

        // add/edit
        public IActionResult AddEdit(string id)
        {
            StudyDocumentsDto model = new StudyDocumentsDto();
            try
            {
                model.IsActive = true;
                if (!string.IsNullOrWhiteSpace(id))
                {
                    model = _studyDocumentsService.GetStudyDocumentsAndFilesByKeyId(id);
                    if (model is null)
                    {
                        return NotFound();
                    }
                    // only un-approved is editable
                    if (!HasPermission() && model.IsApproved)
                    {
                        return NotFound();
                    }
                }
                ViewBag.TechnologyList = _technologyService.GetTechnologyList().OrderBy(o=>o.Title).
                        Select(n => new SelectListItem
                        {
                            Text = n.Title,
                            Value = n.TechId.ToString(),
                            //Selected = n.Id == model.TechnologyId
                        }).ToList();
            }
            catch (Exception ex)
            {

            }
            return View(model);
        }
        // add/edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEdit(StudyDocumentsDto model)
        {
            var fileExtentions = new List<string>
            {
                ".docx",
                ".DOCX",
                ".pdf",
                ".PDF"
            };

            try
            {
                if (!ModelState.IsValid)
                {
                    return MessagePartialView("Validation failed");
                }
                if (!model.studyDocumentFiles.Any() && (Request.Form.Files == null || !Request.Form.Files.Any()))
                {
                    return MessagePartialView("Validation failed");
                }
                if (Request.Form.Files.Any() && Request.Form.Files.Any(x => !fileExtentions.Contains(Path.GetExtension(x.FileName))))
                {
                    return MessagePartialView("Validation failed");
                }
                // a id for display name is unique
                var matchCount = 0;
                for (var i = 0; i < model.studyDocumentFiles.Count; i++)
                {
                    matchCount = 0;
                    for (var j = 0; j < model.studyDocumentFiles.Count; j++)
                    {
                        if (string.Equals(model.studyDocumentFiles[i].DisplayName, model.studyDocumentFiles[j].DisplayName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            matchCount++;
                        }
                        // match found except self
                        if (matchCount >= 2)
                        {
                            return MessagePartialView("Display name should be unique.");
                        }
                    }
                }

                if (model.Id > 0)
                {
                    // main
                    model.UpdatedBy = CurrentUser.Uid;
                    model.IsDelete = false;
                    model.IsApproved = false;
                    model.Ip = ContextProvider.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();

                    // child
                    // add in model and save file in folder
                    AddAndSvaeFileInFolder(model);

                    var deleteFiles = _studyDocumentsService.UpdateStudyDocuments(model);
                    // delete from folder
                    foreach (var file in deleteFiles)
                    {
                        var filePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath, "upload", "studydocuments", file.Filename);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                }
                else
                {
                    // main
                    model.AddedBy = CurrentUser.Uid;
                    model.UpdatedBy = model.AddedBy;
                    model.IsDelete = false;
                    model.IsApproved = false;
                    model.KeyId = Guid.NewGuid().ToString();
                    model.Ip = ContextProvider.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();

                    // child
                    // add in model and save file in folder
                    AddAndSvaeFileInFolder(model);

                    _studyDocumentsService.SaveStudyDocuments(model);
                }
            }
            catch (Exception ex)
            {
                return MessagePartialView("Error occurred.");
            }

            ShowSuccessMessage("Success", $"Record has been {(model.Id > 0 ? "updated" : "added")} successfully.\nnow it is pending for approval.", false);
            return NewtonSoftJsonResult(new RequestOutcome<string>
            {
                IsSuccess = true,
                RedirectUrl = Url.Action(nameof(Index))
            });
        }

        // create duplicate entry
        [HttpPost]
        public ActionResult createDuplicateEntry(string id)// keyid
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest();
                }
                var oldRecord = _studyDocumentsService.GetStudyDocumentsAndFilesByKeyId(id);
                if (oldRecord == null)
                {
                    return NotFound();
                }

                StudyDocumentsDto model = new StudyDocumentsDto();
                // main
                model.Title = oldRecord.Title;
                model.Description = oldRecord.Description;
                model.AddedBy = oldRecord.AddedBy;// original owner
                model.UpdatedBy = CurrentUser.Uid;
                model.IsDelete = false;
                model.IsApproved = false;
                model.Ip = ContextProvider.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
                model.TechnologyId = oldRecord.TechnologyId;

                // child
                StudyDocumentFilesDto childModel = null;
                foreach (var file in oldRecord.studyDocumentFiles)
                {
                    // file name
                    var fileExtention = Path.GetExtension(file.FileName);
                    var guid = Guid.NewGuid().ToString();
                    var fileName = $"{guid}_{Path.GetFileNameWithoutExtension(file.FileName).TrimLength(50, false)}{fileExtention}";

                    // save file in folder
                    var folderPath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath, "upload", "studydocuments");
                    var destnationFilePath = Path.Combine(folderPath, fileName);
                    var sourceFilePath = Path.Combine(folderPath, file.FileName);// old file
                    if (System.IO.File.Exists(sourceFilePath))
                    {
                        System.IO.File.Copy(sourceFilePath, destnationFilePath, true);
                    }

                    // model
                    childModel = new StudyDocumentFilesDto();
                    childModel.KeyId = guid;
                    childModel.FileName = fileName;
                    childModel.DisplayName = file.DisplayName;
                    // main
                    model.studyDocumentFiles.Add(childModel);
                }
                // insert in table
                _studyDocumentsService.SaveStudyDocuments(model);

            }
            catch (Exception ex)
            {
                return MessagePartialView("Error occurred.");
            }

            ShowSuccessMessage("Success", $"Duplicate record created successfully.\nnow it is pending for approval.", false);
            return NewtonSoftJsonResult(new RequestOutcome<string>
            {
                IsSuccess = true,
                RedirectUrl = Url.Action(nameof(Index))
            });
        }

        // delete
        [HttpPost]
        public IActionResult DeleteStudyDocuments(string id)// comma seprated id
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return NewtonSoftJsonResult(false);
                }
                var msg = "Deleted successfully.";
                // normal user only delete un-approved doc.
                if (!HasPermission())
                {
                    var ids = _studyDocumentsService.GetUnApprovedStudyDocIds(id);
                    id = string.Join(',', ids);
                    msg = "Only un-approved record(s) deleted.";
                }

                _studyDocumentsService.DeleteStudyDocuments(id, CurrentUser.Uid);
                ShowSuccessMessage("Success", msg, false);

                return NewtonSoftJsonResult(true);
            }
            catch (Exception)
            {
                return NewtonSoftJsonResult(false);
            }
        }

        // un-approve
        public IActionResult ApproveStudyDocuments()
        {
            try
            {
                if (!HasPermission())
                {
                    return MessagePartialView("Badrequest");
                }

                return PartialView("_UnapprovedReasonPartial", new StudyDocumentsUnapprovedReasonDto());
            }
            catch (Exception ex)
            {
                return MessagePartialView("Error occurred");
            }
        }

        // approve/un-approv
        [HttpPost]
        public IActionResult ApproveStudyDocuments(StudyDocumentsUnapprovedReasonDto model)
        {
            try
            {
                if (!HasPermission())
                {
                    return MessagePartialView("Badrequest");
                }
                if (!ModelState.IsValid)
                {
                    return MessagePartialView("Validation failed.");
                }


                model.UpdatedBy = CurrentUser.Uid;
                model.Ip = ContextProvider.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
                _studyDocumentsService.ApproveStudyDocumentsBySDIds(model);

                ShowSuccessMessage("Success", $"{(model.IsApproved ? "Approved" : "Un-approved")} successfully", false);
                return NewtonSoftJsonResult(new RequestOutcome<string>
                {
                    IsSuccess = true,
                    RedirectUrl = Url.Action(nameof(Index))
                });
            }
            catch (Exception)
            {
                return MessagePartialView("Error occurred");
            }
        }

        // search and manage
        public IActionResult SearchAndManage(string id)
        {
            ViewBag.TechnologyList = _technologyService.GetTechnologyList().OrderBy(O=>O.Title).
                        Select(n => new SelectListItem
                        {
                            Text = n.Title,
                            Value = n.TechId.ToString(),
                            //Selected = n.Id == model.TechnologyId
                        }).ToList();

            var model = new StudyDocumentsSearchTextResultDto();

            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    // get searched data
                    var data = _studyDocumentsService.GetAllStudyDocumentsByKeyId(id);

                    // set in model
                    foreach (var sd in data)
                    {
                        // main
                        var sdModel = new StudyDocumentsSearchResultDto();
                        sdModel.Title = sd.Title.Trim().ToTitleCase();
                        sdModel.Description = sd.Description?.Trim().TrimLength(100).ToTitleCase();
                        sdModel.KeyId = sd.Keyid;
                        sdModel.AddedBy = _userLoginService.GetUserInfoByID(sd.Addedby).Name.ToTitleCase();
                        sdModel.AddedDate = Convert.ToString(sd.Addeddate?.Date);
                        sdModel.UpdatedDate = Convert.ToString(sd.Updateddate?.Date);
                        sdModel.DocumentCount = sd.StudyDocumentFiles.Count;
                        sdModel.Technology = sd.Technology.Title;

                        // add
                        model.studyDocumentsSearchResultDtos.Add(sdModel);
                    }
                    model.TotalRecords = data.Count;
                }
            }
            catch (Exception ex)
            {

            }
            return View(model);
        }
        // search and manage
        [HttpPost]
        public IActionResult SearchAndManage(StudyDocumentsSearchDto searchModel)
        {
            var model = new StudyDocumentsSearchTextResultDto();

            try
            {
                if (string.IsNullOrWhiteSpace(searchModel.SearchText) && searchModel.TechnologyId.Length == 0)
                {
                    return View("_SearchPartial", model);
                }

                int totalCount = 0;

                // paging with predicate builder
                searchModel.DataLength = PageDataLength;

                var pagingServices = new PagingService<StudyDocuments>();
                pagingServices.Start = searchModel.PageNo + 1;
                pagingServices.Length = searchModel.DataLength;

                var predicate = PredicateBuilder.True<StudyDocuments>();
                predicate = predicate.And(x => x.Isdelete == false && x.Isactive == true && x.Isapproved == true);

                // title and description search
                if (!string.IsNullOrWhiteSpace(searchModel.SearchText))
                {
                    predicate = predicate.And(x => x.Title.Contains(searchModel.SearchText, StringComparison.InvariantCultureIgnoreCase) || (x.Description != null && x.Description.Contains(searchModel.SearchText, StringComparison.InvariantCultureIgnoreCase)));
                }

                // technology ids 
                if (searchModel.TechnologyId.Length > 0)
                {
                    predicate = predicate.And(x => searchModel.TechnologyId.Contains(x.Technologyid.Value));
                }

                pagingServices.Filter = predicate;
                pagingServices.Sort = (o) =>
                {
                    return o.OrderByDescending(x => x.Updateddate);
                };

                // get searched data
                var data = _studyDocumentsService.GetStudyDocumentsBySearchText(pagingServices, out totalCount);

                // set in model
                foreach (var sd in data)
                {
                    // main
                    var sdModel = new StudyDocumentsSearchResultDto();
                    sdModel.Title = sd.Title.Trim().ToTitleCase();
                    sdModel.Description = sd.Description?.Trim().TrimLength(100).ToTitleCase();
                    sdModel.KeyId = sd.Keyid;
                    sdModel.AddedBy = _userLoginService.GetUserInfoByID(sd.Addedby).Name.ToTitleCase();
                    sdModel.AddedDate = Convert.ToString(sd.Addeddate?.Date);
                    sdModel.UpdatedDate = Convert.ToString(sd.Updateddate?.Date);
                    sdModel.Technology = sd.Technology.Title;

                    // add
                    model.studyDocumentsSearchResultDtos.Add(sdModel);
                }
                model.TotalRecords = totalCount;
                model.IsNotLastRecords = data.Count < searchModel.DataLength;
                model.IsLoadMore = searchModel.IsLoadMore;

                // load more
                ViewBag.IsLoadMore = searchModel.IsLoadMore;
                //last record
                ViewBag.IsNotLastRecords = model.IsNotLastRecords;

            }
            catch (Exception ex)
            {

            }
            return View("_SearchPartial", model);
        }

        // select study documents details
        public IActionResult SearchDetails(string id)
        {
            var sdModel = new StudyDocumentsSearchResultDto();

            try
            {
                // get details data
                var sd = _studyDocumentsService.GetStudyDocumentsByKeyId(id);
                if (sd != null)
                {
                    // set in model
                    // main                
                    sdModel.Title = sd.Title.Trim().ToTitleCase();
                    sdModel.Description = sd.Description?.Trim().ToTitleCase();
                    sdModel.KeyId = sd.Keyid;
                    sdModel.AddedBy = _userLoginService.GetUserInfoByID(sd.Addedby).Name.ToTitleCase();
                    sdModel.AddedDate = sd.Addeddate?.Date.ToFormatDateString("MMM, dd yyyy hh:mm tt");
                    sdModel.UpdatedDate = sd.Updateddate?.Date.ToFormatDateString("MMM, dd yyyy hh:mm tt");
                    sdModel.DocumentCount = sd.StudyDocumentFiles.Count;
                    sdModel.IsApproved = sd.Isapproved.Value;
                    sdModel.Technology = sd.Technology.Title;

                    // child
                    foreach (var sdf in sd.StudyDocumentFiles)
                    {
                        var sdfModel = new StudyDocumentFilesSearchResultDto();
                        sdfModel.KeyId = sdf.Keyid;
                        sdfModel.DisplayName = sdf.Displayname;

                        // add
                        sdModel.studyDocumentFiles.Add(sdfModel);
                    }

                    // user permission
                    var userPermission = _studyDocumentsService.GetAllStudyDocumentsPermissionsBySDId(sd.Id);
                    foreach (var sdup in userPermission)
                    {
                        var sdPermissionModel = new UserPermissionsSearchResultDto();
                        sdPermissionModel.UserId = sdup.Userid;
                        sdPermissionModel.StartDate = sdup.Startdate.Value;
                        sdPermissionModel.EndDate = sdup.Enddate.Value;

                        // add
                        sdModel.userPermissions.Add(sdPermissionModel);
                    }
                }
                if (CurrentUser.RoleId == (int)UserRoles.PM)// pm team users
                {
                    var r = true;
                    if (sdModel.userPermissions.Count > 0)
                    {
                        var pmTeamUser = _userLoginService.GetUsersByPM(CurrentUser.Uid).Select(x => x.Uid);
                        r = sdModel.userPermissions.Any(x => x.UserId == CurrentUser.Uid);// team
                    }
                }

                // self
                sdModel.HasPermission = sdModel.userPermissions.Count > 0 && sdModel.userPermissions.Any(x => x.UserId == CurrentUser.Uid);
                sdModel.IsExtend = sdModel.userPermissions.Count > 0 && sdModel.userPermissions.Any(x => x.UserId == CurrentUser.Uid);
                // dir. view all
                if (CurrentUser.RoleId == (int)UserRoles.Director)
                {
                    sdModel.HasPermission = true;
                }
                else if (HasHRBPPermission())// sr. hr view all
                {
                    sdModel.HasPermission = true;
                }
                else if (CurrentUser.RoleId == (int)UserRoles.PM)// pm team users
                {
                    if (!sdModel.HasPermission && sdModel.userPermissions.Count > 0)
                    {
                        var pmTeamUser = _userLoginService.GetUsersByPM(CurrentUser.Uid).Select(x => x.Uid);
                        sdModel.HasPermission = sdModel.userPermissions.Any(x => pmTeamUser.Contains(x.UserId));
                    }
                    else
                    {
                        sdModel.HasPermission = true;
                    }
                }
                else// remaning user
                {
                    sdModel.HasPermission = (sdModel.userPermissions.Count > 0 && sdModel.userPermissions.Any(x => x.UserId == CurrentUser.Uid && DateTime.Now.Date >= x.StartDate && DateTime.Now.Date <= x.EndDate));
                }

            }
            catch (Exception ex)
            {

            }
            return PartialView("_SearchDetailsPartial", sdModel);
        }

        // secure view of documents
        public IActionResult OnlyView(string id, string keyId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(keyId))
                {
                    return NotFound();
                }

                var data = _studyDocumentsService.GetStudyDocumentsByKeyId(id);
                if (data == null || data.Isapproved == false || data.StudyDocumentFiles.Count == 0)
                {
                    return NotFound();
                }

                var file = data.StudyDocumentFiles.SingleOrDefault(x => x.Keyid == keyId);
                if (file == null)
                {
                    return NotFound();
                }

                string fileName = file.Filename;
                string wwwRoot = ContextProvider.HostEnvironment.WebRootPath;
                string docPath = Path.Combine(wwwRoot, "Upload", "StudyDocuments");
                string filePath = Path.Combine(docPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound();
                }

                byte[] bytes = System.IO.File.ReadAllBytes(filePath);
                string base64Encoded = System.Convert.ToBase64String(bytes);

                string mimeType = string.Empty;
                switch (Path.GetExtension(fileName))
                {
                    case ".dwf":
                        mimeType = "Application/x-dwf";
                        break;
                    case ".pdf":
                        mimeType = "Application/pdf";
                        break;
                    case ".doc":
                    case ".docx":
                        mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        break;
                    case ".ppt":
                    case ".pps":
                        mimeType = "Application/vnd.ms-powerpoint";
                        break;
                    case ".xls":
                        mimeType = "Application/vnd.ms-excel";
                        break;
                    default:
                        mimeType = "Application/octet-stream";
                        break;
                }

                var model = new OnlyViewModel
                {
                    FileName = fileName,
                    ContentType = mimeType,
                    Data = base64Encoded,
                };

                return View(model);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }
        }

        // request to set permission
        public IActionResult RequestToViewDocuments(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = false, ErrorMessage = "Not found" });
                }

                var data = _studyDocumentsService.GetStudyDocumentsByKeyId(id);
                if (data == null || data.Isapproved == false)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = false, ErrorMessage = "Not found" });
                }

                // insert entry in table
                var entity = new RequestedStudyDocuments
                {
                    Studydocumentid = data.Id,
                    Requestedby = CurrentUser.Uid,
                    Addeddate = DateTime.Now,
                    Ip = ContextProvider.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString(),
                };
                _studyDocumentsService.InsertRequestedStudyDocument(entity);
                // user permission
                var userPermission = _studyDocumentsService.GetAllStudyDocumentsPermissionsBySDId(data.Id);
                var IsExtend = userPermission.Count > 0 && userPermission.Any(x => x.Userid == CurrentUser.Uid);
                // send mail to hr
                SendRequestToViewDocumentsEmail(data, IsExtend);
                
                if (IsExtend)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = true, Message = "Request sent successfully,\nYour request to extend access duration, has been raised, please wait for approval." });
                }
                else {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = true, Message = "Request sent successfully,\nyou will receive a confirmation mail for the requested." });
                }
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = false, ErrorMessage = "Bad request" });
            }
        }

        // add/edit permission
        public IActionResult AddEditPermission(string id, string uid)
        {
            try
            {
                if (!HasPermission())
                {
                    return BadRequest();
                }
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest();
                }
                var _uid = Decrypt<int>(uid);
                var user = _userLoginService.GetUserInfoByID(_uid);
                if (user == null)
                {
                    return BadRequest();
                }

                var entity = _studyDocumentsService.GetStudyDocumentsByKeyId(id);
                if (entity == null || entity.Isapproved == false)
                {
                    return NotFound();
                }

                var model = new StudyDocumentsPermissionDto
                {
                    Id = entity.Id,
                    KeyId = entity.Keyid,
                    Title = entity.Title,
                    Description = entity.Description,
                    IsActive = entity.Isactive.Value,
                    Technology = entity.Technology.Title,
                };
                // files
                entity.StudyDocumentFiles.ToList().ForEach(x =>
                {
                    model.studyDocumentFiles.Add(new FilesPermissionDto
                    {
                        FileName = x.Filename,
                        DisplayName = x.Displayname
                    });
                });
                // permission
                var userPermission = _studyDocumentsService.GetStudyDocumentsPermissionsBySDId(entity.Id, _uid);
                if (userPermission != null)
                {
                    model.userPermission.Id = userPermission.Id;
                    model.userPermission.StartDate = userPermission.Startdate.Value.Date.ToString("dd/MM/yyyy");
                    model.userPermission.EndDate = userPermission.Enddate.Value.Date.ToString("dd/MM/yyyy");
                    model.userPermission.UserId = userPermission.Userid;

                    model.AddDelPermission = true;
                }
                else
                {
                    model.userPermission.UserId = _uid;
                    model.userPermission.StartDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
                    model.userPermission.EndDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
                }
                model.userPermission.EncryptedUserId = Encrypt(model.userPermission.UserId);
                model.userPermission.RequestedUser = user.Name;

                return View(model);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }
        // add/edit permission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEditPermission(StudyDocumentsPermissionDto model)
        {
            try
            {
                if (!HasPermission())
                {
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    ShowErrorMessage("Error", "Validation failed", false);
                    return RedirectToAction(nameof(AddEditPermission), new { uid = model.userPermission.EncryptedUserId });
                }

                // update main table
                var SDModel = new StudyDocumentsDto
                {
                    Id = model.Id,
                    UpdatedBy = CurrentUser.Uid,
                    Ip = ContextProvider.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString()
                };
                _studyDocumentsService.UpdateStudyDocumentsOnly(SDModel);

                // set data in permission table
                model.userPermission.UserId = Decrypt<int>(model.userPermission.EncryptedUserId);
                var entity = _studyDocumentsService.GetStudyDocumentsPermissionsBySDId(model.Id, model.userPermission.UserId);
                if (entity == null && model.AddDelPermission)
                {
                    // add
                    entity = new StudyDocumentsPermissions
                    {
                        Studydocumentsid = model.Id,
                        Userid = model.userPermission.UserId,
                        Startdate = model.userPermission.StartDate.ToDateTimeDDMMYYYY(),
                        Enddate = model.userPermission.EndDate.ToDateTimeDDMMYYYY()
                    };
                    _studyDocumentsService.SaveStudyDocumentsPermission(entity);
                }
                else if (entity != null && model.AddDelPermission)
                {
                    // update
                    entity.Startdate = model.userPermission.StartDate.ToDateTimeDDMMYYYY();
                    entity.Enddate = model.userPermission.EndDate.ToDateTimeDDMMYYYY();
                    _studyDocumentsService.UpdateStudyDocumentsPermission(entity);
                }
                else if (entity != null)
                {
                    // remove
                    _studyDocumentsService.DeleteStudyDocumentsPermission(entity);
                }

                // mail
                SendConfirmationToViewDocumentsEmail(model);

                ShowSuccessMessage("Success", $"Successfully {(model.AddDelPermission ? "added" : "removed")} permission", false);
                return RedirectToAction(nameof(AddEditPermission), new { uid = model.userPermission.EncryptedUserId });
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error", "Error occurred.", false);
                return RedirectToAction(nameof(AddEditPermission), new { uid = model.userPermission.EncryptedUserId });
            }

        }

        // add/delete user permission
        public IActionResult AddDelUsersPermission()
        {
            try
            {
                if (!HasPermission())
                {
                    return MessagePartialView("Badrequest");
                }
                List<UserLogin> usersList = null;

                // dir. view all
                if (CurrentUser.RoleId == (int)UserRoles.Director)
                {
                    usersList = _userLoginService.GetAllActiveUsersList();
                }
                else if (HasHRBPPermission())// sr. hr view all
                {
                    usersList = _userLoginService.GetAllActiveUsersList();
                }
                else if (CurrentUser.RoleId == (int)UserRoles.PM)// pm team users
                {
                    usersList = _userLoginService.GetUsersByPM(CurrentUser.Uid);
                }
                else// remaning user
                {
                    usersList = new List<UserLogin>();
                }

                var usersSelectList = new List<SelectListItem>();
                usersList.ForEach(x =>
                {
                    usersSelectList.Add(new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Uid.ToString()
                    });
                });
                ViewBag.UsersList = usersSelectList;

                var model = new StudyDocumentAddDelUsersPermission();

                return PartialView("_AddDelUsersPermissionPartial", model);
            }
            catch (Exception ex)
            {
                return MessagePartialView("Error occurred");
            }
        }
        // add/delete user permission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddDelUsersPermission(StudyDocumentAddDelUsersPermission model)
        {
            try
            {
                if (!HasPermission())
                {
                    return MessagePartialView("Badrequest");
                }
                if (!ModelState.IsValid)
                {
                    return MessagePartialView("Validation failed");
                    //return MessagePartialView(GetAllModelError(ModelState)); 
                }

                // permission
                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = CurrentUser.Uid;
                model.Ip = ContextProvider.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();

                _studyDocumentsService.UpdateStudyDocumentsUserPermission(model);

                var permitAllowed = model.UserId;

                if (permitAllowed.Length > 0 && permitAllowed != null)
                {
                    foreach (var uId in permitAllowed)
                    {

                    try
                    {
                        var user = _userLoginService.GetUserInfoByID(uId);
                        FlexiMail objSendMail = new FlexiMail();
                        objSendMail.ValueArray = new string[]
                        {
                    user.Name,
                    "Requested",
                    (model.AddDelPermission ? "added" : "removed")
                        };

                        objSendMail.From = CurrentUser.EmailOffice;
                        objSendMail.To = user.EmailOffice;
                        objSendMail.Subject = $"Your request for view documents permission has been {(model.AddDelPermission ? "added" : "removed")}.";
                        objSendMail.MailBodyManualSupply = false;
                        objSendMail.EmailTemplateFileName = "StudyDocumentsViewConfirmationEmail.html";

                        System.Threading.Tasks.Task.Run(() => objSendMail.Send());
                    }
                    catch
                    {

                    }

                    }
                }


                ShowSuccessMessage("Success", $"{(model.AddDelPermission ? "Allow" : "Restrict")} permission to selected user(s) successfully.", false);
                return NewtonSoftJsonResult(new RequestOutcome<string>
                {
                    IsSuccess = true,
                    RedirectUrl = Url.Action(nameof(Index))
                });
            }
            catch (Exception ex)
            {
                return MessagePartialView("Error occurred");
            }
        }

        // quick view details
        public IActionResult QuickView(string id)// keyid
        {
            try
            {
                if (!HasPermission())
                {
                    return MessagePartialView("Badrequest");
                }
                if (string.IsNullOrWhiteSpace(id))
                {
                    return MessagePartialView("Badrequest");
                }

                var entity = _studyDocumentsService.GetStudyDocumentsByKeyId(id);
                if (entity == null)
                {
                    return MessagePartialView("Badrequest");
                }

                var model = new StudyDocumentsQuickViewDto
                {
                    Id = entity.Id,
                    KeyId = entity.Keyid,
                    Title = entity.Title,
                    Description = entity.Description,
                    IsActive = entity.Isactive.Value,
                    Technology = entity.Technology.Title,
                };
                // files
                entity.StudyDocumentFiles.ToList().ForEach(x =>
                {
                    model.studyDocumentFiles.Add(new FilesQuickViewDto
                    {
                        FileName = x.Filename,
                        DisplayName = x.Displayname,
                        KeyId = x.Keyid
                    });
                });
                // permission
                var userPermissions = _studyDocumentsService.GetAllStudyDocumentsPermissionsBySDId(entity.Id);
                userPermissions.ForEach(x =>
                {
                    model.userPermissions.Add(new UserPermissionsQuickViewDto
                    {
                        Id = x.Id,
                        UserId = x.Userid,
                        EncryptedUserId = Encrypt(x.Userid),
                        UserName = _userLoginService.GetUserInfoByID(x.Userid).Name
                    });
                });

                return PartialView("_QuickViewPartial", model);
            }
            catch (Exception ex)
            {
                return MessagePartialView("Error occurred");
            }
        }

        // delete SD file
        [HttpPost]
        public IActionResult DeleteStudyDocumentFile(string id, string fileKeyId)// keyid
        {
            try
            {
                if (!HasPermission())
                {
                    return MessagePartialView("Badrequest");
                }
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(fileKeyId))
                {
                    return MessagePartialView("Badrequest");
                }

                // update main table
                var SDModel = new StudyDocumentsDto
                {
                    KeyId = id,
                    UpdatedBy = CurrentUser.Uid,
                    Ip = ContextProvider.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString()
                };
                var entity = _studyDocumentsService.UpdateStudyDocumentsOnlyByKeyId(SDModel);

                if (entity.StudyDocumentFiles.Count == 1)
                {
                    return MessagePartialView("you cannot remove all files, one is required or you can delete the record from list");
                }
                // remove file
                var deleteFile = _studyDocumentsService.DeleteStudyDocumentFile(fileKeyId);
                if (deleteFile == null)
                {
                    return MessagePartialView("Badrequest");
                }
                // delete from folder
                var filePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath, "upload", "studydocuments", deleteFile.Filename);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                return MessagePartialView("Successfully removed", messageType: MessageType.Success);
            }
            catch (Exception)
            {
                return MessagePartialView("Error occurred");
            }
        }


        #region private
        // add new file in model and save in folder
        private void AddAndSvaeFileInFolder(StudyDocumentsDto model)
        {
            foreach (var file in Request.Form.Files)
            {
                var fileExtention = Path.GetExtension(file.FileName);
                var guid = Guid.NewGuid().ToString();
                var fileName = $"{guid}_{Path.GetFileNameWithoutExtension(file.FileName).TrimLength(50, false)}{fileExtention}";

                // update filename and keyid in new added
                var newfiles = model.studyDocumentFiles.FirstOrDefault(x => x.Id == 0 && x.FileName.Equals(file.FileName, StringComparison.InvariantCultureIgnoreCase));
                if (newfiles == null)
                {
                    throw new Exception("Validation failed");
                }
                newfiles.KeyId = guid;
                newfiles.FileName = fileName;

                // save file in folder
                var folderPath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath, "upload", "studydocuments");
                var filePath = Path.Combine(folderPath, fileName);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
        }
        // send request mail
        private async System.Threading.Tasks.Task SendRequestToViewDocumentsEmail(StudyDocuments data,bool IsExtend)
        {
            try
            {
                string _type = "grant";
                if(IsExtend)
                {
                    _type = "extend";
                }
                FlexiMail objSendMail = new FlexiMail();
                var EncryptedUserId = Encrypt(CurrentUser.Uid);
                objSendMail.ValueArray = new string[]
                {
                    data.Title,
                    CurrentUser.Name,
                    $"{SiteKey.DomainName}studydocuments/AddEditPermission/{data.Keyid }?uid={EncryptedUserId}",
                    _type
                };

                objSendMail.From = CurrentUser.EmailOffice;
                objSendMail.To = SiteKey.HREmailId;
                // pm mail
                var pmMail = _userLoginService.GetUsersById(CurrentUser.PMUid)?.EmailOffice;
                if (!string.IsNullOrWhiteSpace(pmMail))
                {
                    objSendMail.CC = pmMail;
                }
                if (IsExtend)
                {
                    objSendMail.Subject = $"{CurrentUser.Name} sent a request for extend permission of study center.";
                }
                else
                {
                    objSendMail.Subject = $"{CurrentUser.Name} sent a request for grant permission of study center.";
                }
                objSendMail.MailBodyManualSupply = false;
                objSendMail.EmailTemplateFileName = "StudyDocumentsViewRequestEmail.html";

                await System.Threading.Tasks.Task.Run(() => objSendMail.Send());
            }
            catch
            {

            }
        }
        // send confirmation mail
        private async System.Threading.Tasks.Task SendConfirmationToViewDocumentsEmail(StudyDocumentsPermissionDto model)
        {
            try
            {
                var user = _userLoginService.GetUserInfoByID(model.userPermission.UserId);
                FlexiMail objSendMail = new FlexiMail();
                objSendMail.ValueArray = new string[]
                {
                    user.Name,
                    model.Title,
                    (model.AddDelPermission ? "added" : "removed")
                };

                objSendMail.From = CurrentUser.EmailOffice;
                objSendMail.To = user.EmailOffice;
                objSendMail.Subject = $"Your request for {model.Title} documents view permission has been {(model.AddDelPermission ? "added" : "removed")}.";
                objSendMail.MailBodyManualSupply = false;
                objSendMail.EmailTemplateFileName = "StudyDocumentsViewConfirmationEmail.html";

                await System.Threading.Tasks.Task.Run(() => objSendMail.Send());
            }
            catch
            {

            }
        }
        // encrypt 
        private string Encrypt(object val)
        {
            var keybytes = Encryption.GetRiJndael_KeyBytes(SiteKey.Encryption_Key);
            return Convert.ToBase64String(Encryption.EncryptStringToBytes(val.ToString(), keybytes, keybytes));
        }
        // decrypt 
        private T Decrypt<T>(object val)
        {
            var _val = val.ToString();
            _val = _val.Replace(" ", "+");

            //*********** GetRiJndael Decrypt**************Start/// 
            var keybytes = Encryption.GetRiJndael_KeyBytes(SiteKey.Encryption_Key);

            byte[] TempEncrypted = Encoding.UTF8.GetBytes(_val);
            byte[] TempmyRijndaelKey2 = Encoding.UTF8.GetBytes(Convert.ToBase64String(keybytes));
            byte[] myRijndaelKey = Convert.FromBase64String(Encoding.ASCII.GetString(TempmyRijndaelKey2));
            try
            {
                byte[] encrypted = Convert.FromBase64String(Encoding.ASCII.GetString(TempEncrypted));
                _val = Encryption.DecryptStringFromBytes(encrypted, myRijndaelKey, myRijndaelKey);
            }
            catch (Exception ex)
            {

            }

            return (T)Convert.ChangeType(_val, typeof(T));
        }
        // hr has all and ashish sir has for team
        private bool HasPermission()
        {
            // dir. view all
            if (CurrentUser.RoleId == (int)UserRoles.Director)
            {
                return true;
            }
            else if (HasHRBPPermission())// sr. hr view all
            {
                return true;
            }
            else if (CurrentUser.RoleId == (int)UserRoles.PM)// pm team users
            {
                return true;
            }
            else// remaning user
            {
                return false;
            }
        }
        // hrbp 
        private bool HasHRBPPermission()
        {
            if (CurrentUser.RoleId == (int)UserRoles.HRBP &&
                (CurrentUser.DesignationId == (int)UserDesignation.HRSrManager
                || CurrentUser.DesignationId == (int)UserDesignation.HRBPAGM
                || CurrentUser.DesignationId == (int)UserDesignation.HRBPLead
                || CurrentUser.DesignationId == (int)UserDesignation.HRBPVicePresident
                ))// sr. hr view all
            {
                return true;
            }
            return false;
        }
        // get all model errors
        private string GetAllModelError(ModelStateDictionary modelState)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var modelStateKey in modelState.Keys)
            {
                var modelStateVal = modelState[modelStateKey];
                foreach (var error in modelStateVal.Errors)
                {
                    sb.Append($"key={modelStateKey}, error={error.ErrorMessage}");
                    sb.Append("<br>");
                }
            }

            return sb.ToString();
        }
        #endregion
    }
}
