using DataTables.AspNet.Core;
using EMS.Service;
using EMS.Service.LibraryManagement;
using EMS.Web.Code.Attributes;
using EMS.Web.Controllers;
using EMS.Data;
using EMS.Dto;
using EMS.Dto.LibraryManagement;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMS.Core;
using EMS.Web.Code.LIBS;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Website.Controllers
{
    [CustomAuthorization()]
    public class SalesKitController : BaseController
    {
        #region "Variables and constructor"
        private readonly ISalesKitTypeService salesKitTypeService;

        public SalesKitController(ISalesKitTypeService salesKitTypeService)
        {
            this.salesKitTypeService = salesKitTypeService;
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
            var pagingService = new PagingService<SalesKitType>(request.Start, request.Length);
            var expr = PredicateBuilder.True<SalesKitType>();
            if (!String.IsNullOrEmpty(searchName))
            {
                expr = expr.And(e => e.Name.Contains(searchName)|| e.DisplayOrder.ToString().Contains(searchName));
            }

            pagingService.Filter = expr;

            pagingService.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "salesKit_Name":
                            return o.OrderByColumn(item, c => c.Name);
                        case "displayOrder":
                            return o.OrderByColumn(item, c => c.DisplayOrder);
                        default:
                            return o.OrderByColumn(item, c => c.DisplayName);
                    }
                }
                return o.OrderByDescending(c => c.SalesKitId);
            };
            int totalCount = 0;
            var response = salesKitTypeService.GetSalesKitTypeByPaging(out totalCount, pagingService);
            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                salesKit_ID = r.SalesKitId,
                rowId = (index + 1) + (request.Start),
                salesKit_Name = r.Name,
                salesKit_DisplayName = r.DisplayName,
                displayOrder = r.DisplayOrder.HasValue?r.DisplayOrder.Value.ToString():"",
                parentName = r.ParentId != 0 ? salesKitTypeService.GetSalesKitTypeDetail(Convert.ToInt32(r.ParentId)).Name : "",
                r.IsActive
            }));
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult UpdateStatus(int id)
        {
            if (id != 0)
            {
                SalesKitType salesKit = new SalesKitType();
                salesKit = salesKitTypeService.GetSalesKitTypeDetail(id);
                if (salesKit.IsActive == true)
                {
                    salesKit.IsActive = false;
                }
                else
                {
                    salesKit.IsActive = true;
                }
                salesKitTypeService.UpdateStatus(salesKit);
            }
            return RedirectToAction("Index");
        }
        public List<SelectListItem> GetSalesKitList(bool selectDefault = true)
        {
            var list = salesKitTypeService.GetSalesKitType();

            var salesKitlist = list.Select(x => new SelectListItem { Text = x.Name != null ? x.Name.ToString() : "", Value = x.SalesKitId.ToString(), Selected = selectDefault ? (x.SalesKitId == list.FirstOrDefault().SalesKitId ? true : false) : false }).ToList();
            if (selectDefault)
            {
                salesKitlist.Insert(0, new SelectListItem() { Text = "-Select-", Value = "0" });
            }
            return salesKitlist;
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult AddEdit(int? id)
        {
            SalesKitTypeDto salesKitTypeDto = new SalesKitTypeDto();
            if (id != null)
            {
                SalesKitType salesKit = new SalesKitType();
                salesKit = salesKitTypeService.GetSalesKitTypeDetail(Convert.ToInt32(id));
                salesKitTypeDto.SalesKitId = salesKit.SalesKitId;
                salesKitTypeDto.Name = salesKit.Name;
                salesKitTypeDto.DisplayName = salesKit.DisplayName;
                salesKitTypeDto.ParentId = salesKit.ParentId;
                salesKitTypeDto.DisplayOrder = salesKit.DisplayOrder;
                salesKitTypeDto.IsActive = salesKit.IsActive.HasValue && salesKit.IsActive.Value != true ? false : true;
                salesKitTypeDto.IsChild = salesKit.ParentId.HasValue && salesKit.ParentId.Value != 0 ? true : false;
            }
            salesKitTypeDto.ParentSalesKit = GetSalesKitList(false);
            return PartialView("_AddEditSalesKitType", salesKitTypeDto);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult AddEdit(SalesKitTypeDto salesKitTypeDto)
        {
            if (salesKitTypeDto != null && salesKitTypeDto.IsChild == false)
            {
                ModelState.Remove("ParentId");
            }
            if (CurrentUser != null && CurrentUser.Uid != 0 && ModelState.IsValid)
            {
                bool success = false;

                SalesKitType salesKit = new SalesKitType();
                var previousDisplayOrder = salesKitTypeService.GetLastDisplayOrder(salesKitTypeDto.ParentId);
                if (salesKitTypeDto.SalesKitId != 0)
                {
                    salesKit = salesKitTypeService.GetSalesKitTypeDetail(salesKitTypeDto.SalesKitId);
                    if (salesKit != null)
                    {
                        salesKit.Name = salesKitTypeDto.Name;
                        salesKit.DisplayName = salesKitTypeDto.DisplayName;
                        salesKit.DisplayOrder = salesKitTypeDto.DisplayOrder.HasValue ? salesKitTypeDto.DisplayOrder : previousDisplayOrder;
                        salesKit.IsActive = salesKitTypeDto.IsActive;
                        salesKit.ParentId = salesKitTypeDto.ParentId.HasValue && salesKitTypeDto.ParentId.Value != 0 ? salesKitTypeDto.ParentId.Value : 0;
                    }
                }
                else
                {
                    salesKit.Name = salesKitTypeDto.Name;
                    salesKit.DisplayName = salesKitTypeDto.DisplayName;
                    salesKit.DisplayOrder = salesKitTypeDto.DisplayOrder.HasValue ? salesKitTypeDto.DisplayOrder : previousDisplayOrder + 1;
                    salesKit.IsActive = salesKitTypeDto.IsActive;
                    salesKit.ParentId = salesKitTypeDto.ParentId.HasValue && salesKitTypeDto.ParentId.Value != 0 ? salesKitTypeDto.ParentId.Value : 0;
                }

                success = salesKitTypeService.Save(salesKit);
                if (success)
                {
                    if (salesKitTypeDto.SalesKitId == 0)
                    {
                        ShowSuccessMessage("Success", "Sales Kit type has been successfully added!", false);
                    }
                    else
                    {
                        ShowSuccessMessage("Success", "Sales Kit type has been successfully updated!", false);
                    }

                }
                else
                {
                    ShowErrorMessage("Failed", "Sales Kit type already exist", false);
                }

                return Json(new { isSuccess = true, redirectUrl = Url.Action("Index", "SalesKit") });
            }
            else
            {
                return CreateModelStateErrors();
            }
        }

    }
}
