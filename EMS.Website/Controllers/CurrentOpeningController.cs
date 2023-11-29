using EMS.Dto;
using EMS.Service;
using EMS.Data;
using EMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using EMS.Web.Code.Attributes;
using DataTables.AspNet.Core;
using EMS.Web.Code.LIBS;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class CurrentOpeningController : BaseController
    {
        #region "Fields"
        private ICurrentOpeningService currentOpeningService;
        private IDepartmentService departmentService;
        private IJobReferenceService jobReferenceService;
        #endregion

        #region "Constructor"
        public CurrentOpeningController(ICurrentOpeningService _currentOpeningService, IDepartmentService _departmentService, IJobReferenceService _jobReferenceService)
        {
            this.currentOpeningService = _currentOpeningService;
            this.departmentService = _departmentService;
            this.jobReferenceService = _jobReferenceService;
        }
        #endregion
        // GET: Timesheet
        public ActionResult Index()
        {
            return View(currentOpeningService.GetCurrentOpenings());
        }
        public ActionResult AddEdit(int? id)
        {
            var model = new CurrentOpeningDto();
            var currentOpening = currentOpeningService.GetCurrentOpeningById(id);
            if (currentOpening != null)
            {
                model.Id = currentOpening.Id;
                model.Post = currentOpening.Post;
                model.Technology = currentOpening.Technology;
                model.Min_Experience = currentOpening.Min_Experience;
                model.Small_Description = currentOpening.Small_Description;
                model.IsActive = currentOpening.IsActive ?? false;
                model.Post = currentOpening.Post;
                model.DepartmentId = currentOpening.DepartmentId;
            }
            ViewBag.DepartmentList = departmentService.GetDepartments();
            return View(model);
        }
        [HttpPost]
        public ActionResult AddEdit(CurrentOpeningDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentOpening = currentOpeningService.GetCurrentOpeningById(model.Id) ?? new CurrentOpening();
                    currentOpening.Post = model.Post;
                    currentOpening.Technology = model.Technology;
                    currentOpening.Min_Experience = model.Min_Experience;
                    currentOpening.Small_Description = model.Small_Description;
                    currentOpening.IsActive = model.IsActive;
                    currentOpening.DepartmentId = model.DepartmentId;
                    if (currentOpening.Id > 0)
                    {
                        currentOpening.ModifyDate = DateTime.Now;
                    }
                    else
                    {
                        currentOpening.AddDate = DateTime.Now;
                        currentOpening.ModifyDate = DateTime.Now;
                    }
                    //Save to db 
                    currentOpeningService.Save(currentOpening);
                    ShowSuccessMessage("Success!", "Record has been saved successfully !!", false);
                    return RedirectToAction("index");
                }
                else
                {
                    ShowErrorMessage("Error", string.Join("; ", ModelState.Values
                                             .SelectMany(x => x.Errors)
                                             .Select(x => x.ErrorMessage)), true);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error", ex.GetBaseException().Message, true);
            }
            ViewBag.DepartmentList = departmentService.GetDepartments();
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            var IsSuccess = true;
            try
            {
                var currentOpening = currentOpeningService.GetCurrentOpeningById(id);
                if (currentOpening != null)
                {
                    currentOpeningService.Delete(currentOpening);
                    IsSuccess = true;
                    ShowSuccessMessage("Success", "Record has been deleted sucessfully !!", false);
                }
                else
                {
                    IsSuccess = false;
                    ShowSuccessMessage("Error!!", "Record not found to delete !!", false);
                }
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                ShowSuccessMessage("Error!!", ex.Message, false);
            }
            return Json(new { IsSuccess = IsSuccess, RedirectUrl = Url.Action("Index") });
        }




      
    }
}