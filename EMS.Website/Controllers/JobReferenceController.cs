using EMS.Dto;
using EMS.Service;
using EMS.Data;
using EMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using EMS.Web.Code.Attributes;
using DataTables.AspNet.Core;
using EMS.Web.Code.LIBS;
using static EMS.Core.Enums;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class JobReferenceController : BaseController
    {
        #region "Fields"
        private ICurrentOpeningService currentOpeningService;
        private IJobReferenceService jobReferenceService;
        #endregion

        #region "Constructor"
        public JobReferenceController(ICurrentOpeningService _currentOpeningService, IJobReferenceService _jobReferenceService)
        {
            this.currentOpeningService = _currentOpeningService;
            this.jobReferenceService = _jobReferenceService;
        }
        #endregion
        [HttpGet]
        public ActionResult ReferFriend(int? id)
        {
            JobReferFriendDto model = new JobReferFriendDto();
            model.CurrentOpeningId = id;
            return PartialView("_ReferFriend", model);
        }

        [HttpPost]
        public ActionResult ReferFriend(JobReferFriendDto model)
        {
            try
            {
               // var files = Request.Files;
                if (ModelState.IsValid)
                {
                    JobReference entity = new JobReference();
                    entity.Name = model.Name;
                    entity.Email = model.Email;
                    entity.MobileNo = model.MobileNo;
                    entity.CurrentOpeningId = model.CurrentOpeningId;
                    entity.Small_Desc = model.Small_Desc;
                    entity.ReferBy_UserLoginId = CurrentUser.Uid;
                    entity.AddDate = DateTime.Now;
                    entity.ModifyDate = DateTime.Now;
                    entity.IP = GeneralMethods.Getip();// Request.UserHostAddress;
                    try
                    {
                        if (model.Attacchment != null && model.Attacchment.Length > 0)
                        {
                            string fileName = Guid.NewGuid().ToString() + "_" + (model.Attacchment.FileName.Replace("&", string.Empty).Replace("--", "-"));
                            var path = Path.Combine(SiteKey.UploadResumeFolderPath, fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                model.Attacchment.CopyTo(stream);

                            }
                            //model.Attacchment.SaveAs(path);
                            //Now set database entity object after saved file 
                            entity.Attacchment = fileName;
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    jobReferenceService.Save(entity);
                    ShowSuccessMessage("Success", "Reference has been saved successfully.", false);
                    return Json(new { isSuccess = true, redirectUrl = Url.Action("index", "currentopening") });
                }
                else
                {
                    ModelState.AddModelError("", string.Join("; ", ModelState.Values
                                      .SelectMany(x => x.Errors)
                                      .Select(x => x.ErrorMessage)));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.GetBaseException().Message);
            }
            return CreateModelStateErrors();
        }

        [HttpGet]
        public ActionResult ViewReferences()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ViewReferences(IDataTablesRequest request, int? currentOpeningId)
        {
            var pagingService = new PagingService<JobReference>(request.Start, request.Length);
            var expr = PredicateBuilder.False<JobReference>();
            if (CurrentUser.RoleId == (int)Enums.UserRoles.HRBP)
            {
                expr = expr.Or(e => e.CurrentOpeningId == currentOpeningId);
            }
            else
            {
                expr = expr.Or(e => e.CurrentOpeningId == currentOpeningId && e.ReferBy_UserLoginId == CurrentUser.Uid);
            }

            if (!string.IsNullOrWhiteSpace(request.Search.Value))
            {
                int id = 0;
                expr = expr.Or(e => e.Name.StartsWith(request.Search.Value));
                expr = expr.Or(e => e.Email.StartsWith(request.Search.Value));
                expr = expr.Or(e => e.UserLogin != null && e.UserLogin.Name.StartsWith(request.Search.Value));
                if (int.TryParse(request.Search.Value, out id))
                {
                    expr = expr.Or(e => e.Id == id);
                }
            }

            pagingService.Filter = expr;

            pagingService.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "id":
                            return o.OrderByColumn(item, c => c.Id);
                        case "name":
                            return o.OrderByColumn(item, c => c.Name);
                        case "email":
                            return o.OrderByColumn(item, c => c.Email);
                        case "post":
                            return o.OrderByColumn(item, c => c.CurrentOpening.Post);
                        case "mobileNo":
                            return o.OrderByColumn(item, c => c.MobileNo);
                        case "referedBy":
                            return o.OrderByColumn(item, c => c.UserLogin.Name);
                        case "status":
                            return o.OrderByColumn(item, c => c.Status);
                    }
                }

                return o.OrderByDescending(c => c.Id);
            };

            int totalCount = 0;
            var response = jobReferenceService.GetJobReferenceByPage(out totalCount, pagingService);

            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                RowId = (index + 1) + (request.Start),
                r.Id,
                r.CurrentOpeningId,
                r.Name,
                r.Email,
                Post = r.CurrentOpening != null ? r.CurrentOpening.Post : string.Empty,
                r.MobileNo,
                referedBy = r.UserLogin != null ? r.UserLogin.Name : string.Empty,
                Status = r.Status.ToEnum<InterViewStaus>().ToString()
            }));
        }
        public ActionResult ViewDetail(int? id)
        {
            var jobReference = jobReferenceService.GetJobReferenceById(id);
            return PartialView("_ViewDetail", jobReference);
        }
        [HttpPost]
        public ActionResult ViewDetail(JobReferFriendDto model)
        {
            var jobRefernce = jobReferenceService.GetJobReferenceById(model.Id);
            if (jobRefernce != null)
            {
                jobRefernce.Status = model.Status;
                jobReferenceService.Save(jobRefernce);
                ShowSuccessMessage("Success!", "Record has been updated successfully !!", false);
                return RedirectToAction("viewdetail", "jobreference", new { id = jobRefernce.Id });
            }
            else
            {
                ShowErrorMessage("Error", "Record not found to update", true);
            }
            return View(model);
        }
    }
}