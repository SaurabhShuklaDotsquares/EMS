using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Dto.LibraryManagement;
using EMS.Service;
using EMS.Service.LibraryManagement;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.Website.Controllers
{
    [CustomAuthorization()]
    public class CVsController : BaseController
    {
        #region "Variables and constructor"
        private readonly ICvsTypeService cvsTypeService;
        public CVsController(ICvsTypeService cvsTypeService)
        {
            this.cvsTypeService = cvsTypeService;
        }
        #endregion
        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult Index(IDataTablesRequest request, string searchName)
        {
            var pagingService = new PagingService<CvsType>(request.Start, request.Length);
            var expr = PredicateBuilder.True<CvsType>();
            if (!String.IsNullOrEmpty(searchName))
            {
                expr = expr.And(e => e.Name.Contains(searchName) || e.DisplayOrder.ToString().Contains(searchName));
            }

            pagingService.Filter = expr;

            pagingService.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "displayOrder":
                            return o.OrderByColumn(item, c => c.DisplayOrder);
                        case "cvsName":
                            return o.OrderByColumn(item, c => c.Name);
                        default:
                            return o.OrderByColumn(item, c => c.Name);
                    }
                }
                return o.OrderByDescending(c => c.CvsId);
            };
            int totalCount = 0;
            var response = cvsTypeService.GetCVsTypeByPaging(out totalCount, pagingService);
            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                cvs_ID = r.CvsId,
                rowId = (index + 1) + (request.Start),
                displayOrder = r.DisplayOrder.HasValue ? r.DisplayOrder.Value.ToString() : "",
                cvsName = r.Name,
                r.IsActive
            }));
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult UpdateStatus(int id)
        {
            if (id != 0)
            {
                CvsType cvsType = new CvsType();
                cvsType = cvsTypeService.GetCvsTypeRecordDetail(id);
                if (cvsType.IsActive == true)
                {
                    cvsType.IsActive = false;
                }
                else
                {
                    cvsType.IsActive = true;
                }
                cvsTypeService.UpdateStatus(cvsType);
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult AddEdit(int? id)
        {
            CvsTypeDto cvsTypeDto = new CvsTypeDto();
            if (id != null)
            {
                CvsType cvsType = new CvsType();
                cvsType = cvsTypeService.GetCvsTypeRecordDetail(Convert.ToInt32(id));
                cvsTypeDto.CvsId = cvsType.CvsId;
                cvsTypeDto.Name = cvsType.Name;
                cvsTypeDto.DisplayOrder = cvsType.DisplayOrder;
                cvsTypeDto.IsActive = cvsType.IsActive.HasValue && cvsType.IsActive.Value != true ? false : true;
            }
            return PartialView("_AddEditCVsType", cvsTypeDto);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult AddEdit(CvsTypeDto cvsTypeDto)
        {

            if (CurrentUser != null && CurrentUser.Uid != 0 && ModelState.IsValid)
            {
                bool success = false;

                CvsType cvsType = new CvsType();
                var previousDisplayOrder = cvsTypeService.GetLastDisplayOrder();
                if (cvsTypeDto.CvsId != 0)
                {
                    cvsType = cvsTypeService.GetCvsTypeRecordDetail(cvsTypeDto.CvsId);
                    if (cvsType != null)
                    {
                        cvsType.Name = cvsTypeDto.Name;
                        cvsType.DisplayOrder = cvsTypeDto.DisplayOrder.HasValue ? cvsTypeDto.DisplayOrder : previousDisplayOrder;
                        cvsType.IsActive = cvsTypeDto.IsActive;
                    }
                }
                else
                {
                    cvsType.Name = cvsTypeDto.Name;
                    cvsType.DisplayOrder = cvsTypeDto.DisplayOrder.HasValue ? cvsTypeDto.DisplayOrder : previousDisplayOrder + 1;
                    cvsType.IsActive = cvsTypeDto.IsActive;
                }

                success = cvsTypeService.Save(cvsType);
                if (success)
                {
                    if (cvsTypeDto.CvsId == 0)
                    {
                        ShowSuccessMessage("Success", "CVs type has been successfully added!", false);
                    }
                    else
                    {
                        ShowSuccessMessage("Success", "CVs type has been successfully updated!", false);
                    }

                }
                else
                {
                    ShowErrorMessage("Failed", "CVs type already exist", false);
                }
                return Json(new { isSuccess = true, redirectUrl = Url.Action("Index", "CVs") });
            }
            else
            {
                return CreateModelStateErrors();
            }
        }
    }
}
