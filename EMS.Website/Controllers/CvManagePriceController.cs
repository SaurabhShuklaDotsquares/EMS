using EMS.Dto.CVEstimatePrice;
using EMS.Data;
using EMS.Service;
using EMS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using EMS.Dto;
using EMS.Service.CVEstimatePrice;
using static EMS.Core.Enums;
using EMS.Core;
using EMS.Web.Code.LIBS;
using System;
using DataTables.AspNet.Core;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using NPOI.HSSF.Record.Formula.Functions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace EMS.Website.Controllers
{
    public class CvManagePriceController : BaseController
    {
        #region "Fields" 
        private readonly IEstimateService estimateService;
        private IRoleService roleServices;
        private ICVEstimatePriceService cVEstimatePriceService;
        private readonly ICurrencyService _currencyService;
        private readonly ITechnologyService _technologyService;
        #endregion

        #region "Constructor"
        public CvManagePriceController(IEstimateService _estimateService, IRoleService roleServices, ICVEstimatePriceService cVEstimatePriceService, ICurrencyService currencyService, ITechnologyService technologyService)
        {
            this.estimateService = _estimateService;
            this.roleServices = roleServices;
            this.cVEstimatePriceService = cVEstimatePriceService;
            _currencyService = currencyService;
            _technologyService = technologyService;
        }
        #endregion
        public IActionResult Index(int RoleId = 0, int TechId = 0)
        {


            CVEstimatePriceDto cVEstimatePrice = new CVEstimatePriceDto();

            List<RoleCategory> roleCategoryList = roleServices.GetActiveRoleCategory();
            cVEstimatePrice.roleCategoryList = roleServices.GetActiveRoleCategory().Select(x => new DropdownListDto { Text = x.Name, Id = x.Id }).ToList();


            List<Role> roleList = roleServices.GetActiveRoles();
            cVEstimatePrice.RoleList = roleServices.GetActiveRoles().Select(x => new DropdownListDto { Text = x.RoleName, Id = x.RoleId }).ToList();


            List<EstimateTechnology> technologyList = estimateService.GetActiveTechnologyCategory();
            cVEstimatePrice.technologyList = estimateService.GetActiveTechnologyCategory().Select(x => new DropdownListDto { Text = x.Title, Id = x.Id }).ToList();

            List<Currency> currencyList = _currencyService.GetCurrency();
            if (currencyList != null && currencyList.Any())
            {
                var usdRate = currencyList.Where(x => x.Id == (int)CurrencyRates.USD).Select(x => x.ExchangeRate).FirstOrDefault();
                if (usdRate != null && usdRate > 0)
                {
                    cVEstimatePrice.USD = String.Format("{0:0.00}", usdRate.Value);
                }
                var audRate = currencyList.Where(x => x.Id == (int)CurrencyRates.AUD).Select(x => x.ExchangeRate).FirstOrDefault();
                if (audRate != null && audRate > 0)
                {
                    cVEstimatePrice.AUD = String.Format("{0:0.00}", audRate.Value);
                }
                var aedRate = currencyList.Where(x => x.Id == (int)CurrencyRates.AED).Select(x => x.ExchangeRate).FirstOrDefault();
                if (aedRate != null && aedRate > 0)
                {
                    cVEstimatePrice.AED = String.Format("{0:0.00}", aedRate.Value);
                }
            }


            return View(cVEstimatePrice);

        }

        public IActionResult EstimatePrice(int RoleId = 0, int TechId = 0)
        {
            CVEstimatePriceDto cVEstimatePrice = new CVEstimatePriceDto();

            if (RoleId > 0 && TechId > 0)
            {
                List<CvBuilderEstimatePrice> experiencePriceList = cVEstimatePriceService.GetExperincePricesbyTechnology(RoleId, TechId);
                if (experiencePriceList != null)
                {
                    var entryLevelPrice = experiencePriceList.Where(x => x.ExpId == (int)ExperienceLevel.EntryLevelPrice).Select(x => x.Price).FirstOrDefault();
                    if (entryLevelPrice != null)
                    {
                        cVEstimatePrice.EntryLevelPrice = entryLevelPrice.Value;
                    }

                    var oneToTwoPrice = experiencePriceList.Where(x => x.ExpId == (int)ExperienceLevel.OneToTwoPrice).Select(x => x.Price).FirstOrDefault();
                    if (oneToTwoPrice != null)
                    {
                        cVEstimatePrice.OneToTwoPrice = (int)oneToTwoPrice.Value;
                    }
                    var threeToSixPrice = experiencePriceList.Where(x => x.ExpId == (int)ExperienceLevel.ThreeToSixPrice).Select(x => x.Price).FirstOrDefault();
                    if (threeToSixPrice != null)
                    {
                        cVEstimatePrice.ThreeToSixPrice = threeToSixPrice.Value;
                    }
                    var sixToTenPrice = experiencePriceList.Where(x => x.ExpId == (int)ExperienceLevel.SixToTenPrice).Select(x => x.Price).FirstOrDefault();
                    if (sixToTenPrice != null)
                    {
                        cVEstimatePrice.SixToTenPrice = sixToTenPrice.Value;
                    }
                    var tenPlusPrice = experiencePriceList.Where(x => x.ExpId == (int)ExperienceLevel.TenPlusPrice).Select(x => x.Price).FirstOrDefault();
                    if (tenPlusPrice != null)
                    {
                        cVEstimatePrice.TenPlusPrice = tenPlusPrice.Value;
                    }
                }
            }
            else if (RoleId > 0)
            {
                List<CvBuilderEstimatePrice> experiencePriceList = cVEstimatePriceService.GetExpericnePricesbyRole(RoleId);
                if (experiencePriceList != null)
                {
                    var entryLevelPrice = experiencePriceList.Where(x => x.ExpId == (int)ExperienceLevel.EntryLevelPrice).Select(x => x.Price).FirstOrDefault();
                    if (entryLevelPrice != null)
                    {
                        cVEstimatePrice.EntryLevelPrice = entryLevelPrice.Value;
                    }

                    var oneToTwoPrice = experiencePriceList.Where(x => x.ExpId == (int)ExperienceLevel.OneToTwoPrice).Select(x => x.Price).FirstOrDefault();
                    if (oneToTwoPrice != null)
                    {
                        cVEstimatePrice.OneToTwoPrice = oneToTwoPrice.Value;
                    }
                    var threeToSixPrice = experiencePriceList.Where(x => x.ExpId == (int)ExperienceLevel.ThreeToSixPrice).Select(x => x.Price).FirstOrDefault();
                    if (threeToSixPrice != null)
                    {
                        cVEstimatePrice.ThreeToSixPrice = threeToSixPrice.Value;
                    }
                    var sixToTenPrice = experiencePriceList.Where(x => x.ExpId == (int)ExperienceLevel.SixToTenPrice).Select(x => x.Price).FirstOrDefault();
                    if (sixToTenPrice != null)
                    {
                        cVEstimatePrice.SixToTenPrice = sixToTenPrice.Value;
                    }
                    var tenPlusPrice = experiencePriceList.Where(x => x.ExpId == (int)ExperienceLevel.TenPlusPrice).Select(x => x.Price).FirstOrDefault();
                    if (tenPlusPrice != null)
                    {
                        cVEstimatePrice.TenPlusPrice = tenPlusPrice.Value;
                    }
                }
            }
            return PartialView("_EstimatePrice", cVEstimatePrice);
        }

        [HttpPost]
        public IActionResult SaveRecords(CVEstimatePriceDto cVEstimatePrice)
        {
            bool success = false;

            if (cVEstimatePrice.RoleId > 0 && cVEstimatePrice.RoleId != null && cVEstimatePrice.TechnologyId > 0 && cVEstimatePrice.TechnologyId != null)
            {
                List<CvBuilderEstimatePrice> cvBuilderEstimatePrice = cVEstimatePriceService.GetRecordsById((int)cVEstimatePrice.RoleId, (int)cVEstimatePrice.TechnologyId);
                if (cvBuilderEstimatePrice != null)
                {
                    cVEstimatePriceService.DeleteCollection(cvBuilderEstimatePrice);
                }

                List<CvBuilderEstimatePrice> entityList = new List<CvBuilderEstimatePrice>();

                if (cVEstimatePrice.EntryLevelPrice != null && cVEstimatePrice.EntryLevelPrice != 0)
                {
                    CvBuilderEstimatePrice cv = new CvBuilderEstimatePrice();
                    cv.RoleId = cVEstimatePrice.RoleId;
                    cv.TechId = cVEstimatePrice.TechnologyId;
                    cv.ExpId = 1;
                    cv.Price = cVEstimatePrice.EntryLevelPrice;
                    cv.IsActive = true;
                    entityList.Add(cv);
                }
                if (cVEstimatePrice.OneToTwoPrice != null && cVEstimatePrice.OneToTwoPrice != 0)
                {
                    CvBuilderEstimatePrice cv = new CvBuilderEstimatePrice();
                    cv.RoleId = cVEstimatePrice.RoleId;
                    cv.TechId = cVEstimatePrice.TechnologyId;
                    cv.ExpId = 2;
                    cv.Price = cVEstimatePrice.OneToTwoPrice;
                    cv.IsActive = true;
                    entityList.Add(cv);
                }
                if (cVEstimatePrice.ThreeToSixPrice != null && cVEstimatePrice.ThreeToSixPrice != 0)
                {
                    CvBuilderEstimatePrice cv = new CvBuilderEstimatePrice();
                    cv.RoleId = cVEstimatePrice.RoleId;
                    cv.TechId = cVEstimatePrice.TechnologyId;
                    cv.ExpId = 3;
                    cv.Price = cVEstimatePrice.ThreeToSixPrice;
                    cv.IsActive = true;
                    entityList.Add(cv);
                }
                if (cVEstimatePrice.SixToTenPrice != null && cVEstimatePrice.SixToTenPrice != 0)
                {
                    CvBuilderEstimatePrice cv = new CvBuilderEstimatePrice();
                    cv.RoleId = cVEstimatePrice.RoleId;
                    cv.TechId = cVEstimatePrice.TechnologyId;
                    cv.ExpId = 4;
                    cv.Price = cVEstimatePrice.SixToTenPrice;
                    cv.IsActive = true;
                    entityList.Add(cv);
                }
                if (cVEstimatePrice.TenPlusPrice != null && cVEstimatePrice.TenPlusPrice != 0)
                {
                    CvBuilderEstimatePrice cv = new CvBuilderEstimatePrice();
                    cv.RoleId = cVEstimatePrice.RoleId;
                    cv.TechId = cVEstimatePrice.TechnologyId;
                    cv.ExpId = 5;
                    cv.Price = cVEstimatePrice.TenPlusPrice;
                    cv.IsActive = true;
                    entityList.Add(cv);
                }
                success = cVEstimatePriceService.SaveCollection(entityList);
                if (success)
                {
                    ShowSuccessMessage("Success", "Price has been successfully updated.", false);
                }
                else
                {
                    ShowErrorMessage("Error", "Failed..!!", false);

                }
            }
            else if (cVEstimatePrice.RoleId > 0 && cVEstimatePrice.RoleId != null)
            {
                List<CvBuilderEstimatePrice> cvBuilderEstimatePrice = cVEstimatePriceService.GetRecordsByRoleId((int)cVEstimatePrice.RoleId);
                if (cvBuilderEstimatePrice != null)
                {
                    cVEstimatePriceService.DeleteCollection(cvBuilderEstimatePrice);
                }

                List<CvBuilderEstimatePrice> entityList = new List<CvBuilderEstimatePrice>();
                if (cVEstimatePrice.EntryLevelPrice != null && cVEstimatePrice.EntryLevelPrice != 0)
                {
                    CvBuilderEstimatePrice cv = new CvBuilderEstimatePrice();
                    cv.RoleId = cVEstimatePrice.RoleId;
                    cv.ExpId = 1;
                    cv.Price = cVEstimatePrice.EntryLevelPrice;
                    cv.IsActive = true;
                    entityList.Add(cv);
                }
                if (cVEstimatePrice.OneToTwoPrice != null && cVEstimatePrice.OneToTwoPrice != 0)
                {
                    CvBuilderEstimatePrice cv = new CvBuilderEstimatePrice();
                    cv.RoleId = cVEstimatePrice.RoleId;
                    cv.ExpId = 2;
                    cv.Price = cVEstimatePrice.OneToTwoPrice;
                    cv.IsActive = true;
                    entityList.Add(cv);
                }
                if (cVEstimatePrice.ThreeToSixPrice != null && cVEstimatePrice.ThreeToSixPrice != 0)
                {
                    CvBuilderEstimatePrice cv = new CvBuilderEstimatePrice();
                    cv.RoleId = cVEstimatePrice.RoleId;
                    cv.ExpId = 3;
                    cv.Price = cVEstimatePrice.ThreeToSixPrice;
                    cv.IsActive = true;
                    entityList.Add(cv);
                }
                if (cVEstimatePrice.SixToTenPrice != null && cVEstimatePrice.SixToTenPrice != 0)
                {
                    CvBuilderEstimatePrice cv = new CvBuilderEstimatePrice();
                    cv.RoleId = cVEstimatePrice.RoleId;
                    cv.ExpId = 4;
                    cv.Price = cVEstimatePrice.SixToTenPrice;
                    cv.IsActive = true;
                    entityList.Add(cv);
                }
                if (cVEstimatePrice.TenPlusPrice != null && cVEstimatePrice.TenPlusPrice != 0)
                {
                    CvBuilderEstimatePrice cv = new CvBuilderEstimatePrice();
                    cv.RoleId = cVEstimatePrice.RoleId;
                    cv.ExpId = 5;
                    cv.Price = cVEstimatePrice.TenPlusPrice;
                    cv.IsActive = true;
                    entityList.Add(cv);
                }
                success = cVEstimatePriceService.SaveCollection(entityList);
                if (success)
                {
                    ShowSuccessMessage("Success", "Price has been successfully updated.", false);
                }
                else
                {
                    ShowErrorMessage("Error", "Failed..!!", false);

                }
            }
            return Json(new { isSuccess = true, redirectUrl = Url.Action("index", "CvManagePrice") });
        }


        [HttpGet]
        public ActionResult EstimatePriceList()
        {
            CVEstimatePriceDto cVEstimatePrice = new CVEstimatePriceDto();
            
            ViewBag.TechnologyList = _technologyService.GetTechnologyList().OrderBy(x => x.Title).Select(n => new SelectListItem { Text = n.Title, Value = n.TechId.ToString() }).ToList();
            return PartialView(cVEstimatePrice);
        }

        [HttpPost]
        public IActionResult EstimatePriceList(IDataTablesRequest request)
        {
            
            var pagingService = new PagingService<CvBuilderEstimatePrice>(request.Start, request.Length);
            var expr = PredicateBuilder.True<CvBuilderEstimatePrice>();
            string searchFilter = request.Search.Value != null ? request.Search.Value : string.Empty;



            pagingService.Filter = expr;

            var usdCurrency = _currencyService.GetCurrencyByName("USD");
            var audCurrency = _currencyService.GetCurrencyByName("AUD");
            var aedCurrency = _currencyService.GetCurrencyByName("AED");


            int totalCount = 0;
            //var response1 = cVEstimatePriceService.GetCvBuilderEstimatePrice(out totalCount, pagingService);
            var response = cVEstimatePriceService.GetCvBuilderEstimatePriceSP(request.Start, 1000, searchFilter, 0, "");
            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                rowId = (index + 1) + (request.Start),
                roleid = r.RoleId,
                techname = r.TechName,
                techid = r.TechnologyId,
                entryLEVEL = CalculationResult(r.EntryLevel),
                onetoTWO = CalculationResult(r.onetoTwo),
                threetoSIX = CalculationResult(r.threetoSix),
                sixtoTEN = CalculationResult(r.sixtoTen),
                tenPLUS = CalculationResult(r.TenPlus)
            }));
        }

        [HttpGet]
        public ActionResult AddEditPriceList(int RoleId = 0, int TechnologyId = 0)
        {
            CVEstimatePriceDto cVEstimatePrice = new CVEstimatePriceDto();

            if (RoleId > 0 && TechnologyId > 0)
            {
                List<CvBuilderEstimatePrice> experiencePriceList = cVEstimatePriceService.GetExperincePricesbyTechnologyId(RoleId, TechnologyId);
                if (experiencePriceList != null)
                {
                    var entryLevelPrice = experiencePriceList.Where(x => x.ExpId == (int)ExperienceLevel.EntryLevelPrice).Select(x => x.Price).FirstOrDefault();
                    if (entryLevelPrice != null)
                    {
                        cVEstimatePrice.EntryLevelPrice = entryLevelPrice.Value;
                    }

                    var oneToTwoPrice = experiencePriceList.Where(x => x.ExpId == (int)ExperienceLevel.OneToTwoPrice).Select(x => x.Price).FirstOrDefault();
                    if (oneToTwoPrice != null)
                    {
                        cVEstimatePrice.OneToTwoPrice = (int)oneToTwoPrice.Value;
                    }
                    var threeToSixPrice = experiencePriceList.Where(x => x.ExpId == (int)ExperienceLevel.ThreeToSixPrice).Select(x => x.Price).FirstOrDefault();
                    if (threeToSixPrice != null)
                    {
                        cVEstimatePrice.ThreeToSixPrice = threeToSixPrice.Value;
                    }
                    var sixToTenPrice = experiencePriceList.Where(x => x.ExpId == (int)ExperienceLevel.SixToTenPrice).Select(x => x.Price).FirstOrDefault();
                    if (sixToTenPrice != null)
                    {
                        cVEstimatePrice.SixToTenPrice = sixToTenPrice.Value;
                    }
                    var tenPlusPrice = experiencePriceList.Where(x => x.ExpId == (int)ExperienceLevel.TenPlusPrice).Select(x => x.Price).FirstOrDefault();
                    if (tenPlusPrice != null)
                    {
                        cVEstimatePrice.TenPlusPrice = tenPlusPrice.Value;
                    }
                }
            }
            else if (RoleId > 0)
            {
                List<CvBuilderEstimatePrice> experiencePriceList = cVEstimatePriceService.GetExpericnePricesbyRole(RoleId);
                if (experiencePriceList != null)
                {
                    var entryLevelPrice = experiencePriceList.Where(x => x.ExpId == (int)ExperienceLevel.EntryLevelPrice).Select(x => x.Price).FirstOrDefault();
                    if (entryLevelPrice != null)
                    {
                        cVEstimatePrice.EntryLevelPrice = entryLevelPrice.Value;
                    }

                    var oneToTwoPrice = experiencePriceList.Where(x => x.ExpId == (int)ExperienceLevel.OneToTwoPrice).Select(x => x.Price).FirstOrDefault();
                    if (oneToTwoPrice != null)
                    {
                        cVEstimatePrice.OneToTwoPrice = oneToTwoPrice.Value;
                    }
                    var threeToSixPrice = experiencePriceList.Where(x => x.ExpId == (int)ExperienceLevel.ThreeToSixPrice).Select(x => x.Price).FirstOrDefault();
                    if (threeToSixPrice != null)
                    {
                        cVEstimatePrice.ThreeToSixPrice = threeToSixPrice.Value;
                    }
                    var sixToTenPrice = experiencePriceList.Where(x => x.ExpId == (int)ExperienceLevel.SixToTenPrice).Select(x => x.Price).FirstOrDefault();
                    if (sixToTenPrice != null)
                    {
                        cVEstimatePrice.SixToTenPrice = sixToTenPrice.Value;
                    }
                    var tenPlusPrice = experiencePriceList.Where(x => x.ExpId == (int)ExperienceLevel.TenPlusPrice).Select(x => x.Price).FirstOrDefault();
                    if (tenPlusPrice != null)
                    {
                        cVEstimatePrice.TenPlusPrice = tenPlusPrice.Value;
                    }
                }
            }


            return PartialView("_AddEditPriceList", cVEstimatePrice);
        }

        [HttpPost]
        public ActionResult AddEditPriceList(CVEstimatePriceDto cVEstimatePrice)
        {
            if (CurrentUser != null && CurrentUser.Uid != 0)
            {
                if (ModelState.IsValid)
                {
                    //bool IsAlreadyExist = false;
                    bool success = false;
                    if (cVEstimatePrice.Id != null)
                    {

                        CvBuilderEstimatePrice cvBuilderEstimatePrice = new CvBuilderEstimatePrice();
                        cvBuilderEstimatePrice = cVEstimatePriceService.GetPriceListById(Convert.ToInt32(cVEstimatePrice.Id));
                        cvBuilderEstimatePrice.Price = (int)cVEstimatePrice.Price;
                        success = cVEstimatePriceService.Save(cvBuilderEstimatePrice);
                        if (success)
                        {
                            ShowSuccessMessage("Success", "Price has been successfully updated.", false);
                        }
                        else
                        {
                            ShowErrorMessage("Error", "Failed..!!", false);

                        }
                    }
                    else
                    {
                        CvBuilderEstimatePrice cvBuilderEstimatePrice = new CvBuilderEstimatePrice()
                        {
                            Price = cVEstimatePrice.Price
                        };
                        success = cVEstimatePriceService.Save(cvBuilderEstimatePrice);
                        if (success)
                        {
                            ShowSuccessMessage("Success", "Price has been successfully added.", false);
                        }
                        else
                        {
                            ShowErrorMessage("Error", "Failed..!!", false);
                        }
                    }
                }
            }
            return Json(new { isSuccess = true, redirectUrl = Url.Action("index", "CvManagePrice") });

        }
        private string CalculationResult(decimal price)
        {            
            var usdCurrency = _currencyService.GetCurrencyByName("USD");
            var audCurrency = _currencyService.GetCurrencyByName("AUD");
            var aedCurrency = _currencyService.GetCurrencyByName("AED");

            var pound = price;
            var usd = (pound * Convert.ToDecimal(usdCurrency.ExchangeRate));
            var aud = (pound * Convert.ToDecimal(audCurrency.ExchangeRate));
            var aed = Math.Truncate(Math.Truncate((pound * Convert.ToDecimal(aedCurrency.ExchangeRate))) / 10) * 10;
            string Rate = "<span><b>£</b> " + pound.ToString("0.00") + "</span><br/><span><b>USD</b> " + usd.ToString("0.00") + "</span><br/><span><b>AUD</b> " + aud.ToString("0.00") + "</span><br/><span><b>AED</b> " + aed.ToString("0.00") + "</span>";
            return Rate;
        }
    }
}
