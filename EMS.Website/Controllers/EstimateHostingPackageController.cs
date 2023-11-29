using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static EMS.Core.Enums;

namespace EMS.Website.Controllers
{
    [CustomAuthorization()]
    public class EstimateHostingPackageController : BaseController
    {
        #region [Fields]
        private readonly IEstimateHostingPackageService estimateHostingPackageService;
        private readonly IEstimateService estimateService;
        private readonly ICurrencyService currencyService;
        #endregion

        #region [Constructor]
        public EstimateHostingPackageController(IEstimateHostingPackageService _estimateHostingPackageService,
            IEstimateService _estimateService,
            ICurrencyService _currencyService)
        {
            this.estimateHostingPackageService = _estimateHostingPackageService;
            this.estimateService = _estimateService;
            this.currencyService = _currencyService;
        }

        #endregion

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IDataTablesRequest request, string packagename)
        {
            var pagingServices = new PagingService<EstimateHostingPackage>(request.Start, request.Length);
            var expr = PredicateBuilder.True<EstimateHostingPackage>();
            if (!string.IsNullOrEmpty(packagename))
            {
                expr = expr.And(x => x.Name.Contains(packagename));
            }

            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "title":
                            return o.OrderByColumn(item, c => c.Name);
                        default:
                            return o.OrderByColumn(item, c => c.CreatedDate);
                    }
                }
                return o.OrderByDescending(c => c.CreatedDate);
            };
            int totalCount = 0;
            var response = estimateHostingPackageService.GetByPaging(out totalCount, pagingServices);
            return DataTablesJsonResult(totalCount, request, response.Select((r, Index) => new
            {
                r.Id,
                rowIndex = (Index + 1) + (request.Start),
                EstimateTechnologyName = string.Join(", ", r.EstimateHostingPackageTechnology.Select(x => x.EstimateTechnology.Title)),
                r.Name,
                r.PackageDetail,
                CountryName = r.Country?.Name,
                r.HostingCost,
                HostingCostType = ((EstimateCostType)r.HostingCostType).GetDescription(),
                r.SetupCost,
                SetupCostType = ((EstimateCostType)r.SetupCostType).GetDescription(),
                r.IsActive,
                CreatedDate = r.CreatedDate.ToShortDateString(),
                ModifiedDate = r.ModifiedDate.ToShortDateString(),
            }));
        }

        [HttpGet()]
        public IActionResult Manage(int id)
        {
            var model = new EstimateHostingPackageDto();
            if (id > 0)
            {
                model = estimateHostingPackageService.GetById(id);
            }
            ViewBag.EstimateTechnology = estimateService.GetEstimateTechnologyItemList();
            ViewBag.Country = estimateService.GetCountrySelectList();
            return View(model);
        }

        [HttpPost()]
        public IActionResult Manage(EstimateHostingPackageDto model)
        {
            if (ModelState.IsValid)
            {
                if (estimateHostingPackageService.Save(model))
                {
                    ShowSuccessMessage("Success", "Estimate Hosting Package saved successfully", false);
                    return RedirectToAction("Index");
                }
            }
            ViewBag.EstimateTechnology = estimateService.GetEstimateTechnologyItemList();
            ViewBag.Country = estimateService.GetCountrySelectList();
            return View(model);
        }

        #region [Currency]
        public IActionResult Currency(int countryId)
        {
            var model = currencyService.GetCurrencyByEstimateCountry(countryId);
            if (model != null)
            {
                return Ok(new { Id = model.Id, CurrSign = model.CurrSign });
            }
            return null;
        }
        #endregion

        #region [Private]
        #endregion
    }
}