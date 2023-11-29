using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMS.Web.Code.Attributes;
using Microsoft.AspNetCore.Mvc;
using EMS.Service;
using EMS.Dto;
using Microsoft.AspNetCore.Mvc.Rendering;
using EMS.Data;
using Newtonsoft.Json;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class EstimatePriceController : BaseController
    {
        #region "Fields"

        private IEstimatePriceService EstimatePriceService;
        private IEstimateService EstimateService;
        #endregion

        #region "Constructor"
        public EstimatePriceController(IEstimatePriceService _EstimatePriceService,
            IEstimateService _EstimateService)
        {
            this.EstimatePriceService = _EstimatePriceService;
            this.EstimateService = _EstimateService;
        }

        #endregion
        public IActionResult Index(int roleId = 0)
        {
            ViewBag.Role = GetEstimateRoleList(roleId);
            var entity = EstimatePriceService.GetEstimateRoleTechnoloyPriceDto(roleId);
            return View(entity);
        }

        [HttpGet]
        public ActionResult ManageEstimatePrice(int id = 0, int? technologyId = null)
        {
            EstimatePriceDto estimatePriceDto = new EstimatePriceDto();
            List<EstimateExpPriceDto> EstimateExpPriceDtoList = new List<EstimateExpPriceDto>();
            estimatePriceDto.TecnologyParentList = EstimateService.GetEstimateTechnologyItemList(); //GetTecnologyParentList();
            estimatePriceDto.EstimateRoleList = GetEstimateRoleList();
            estimatePriceDto.EstimateRoleId = id;

            var estimateroleExp = EstimatePriceService.GetEstimateRoleExpList(id);
            if (estimateroleExp.Count > 0)
            {
                foreach (var erexp in estimateroleExp)
                {
                    var estimatePrice = erexp.EstimateRoleTechnoloyPrice.FirstOrDefault(x => x.EstimateTechnologyId == technologyId);
                    if (estimatePrice != null)
                    {
                        estimatePriceDto.EstimateTechnologyId = estimatePrice.EstimateTechnologyId;
                        EstimateExpPriceDtoList.Add(new EstimateExpPriceDto
                        {
                            EstimateRoleExp = erexp.Name,
                            EstimateRoleExpID = estimatePrice.EstimateRoleExpId,
                            Price = estimatePrice.Price
                        });
                    }
                    else
                    {
                        estimatePriceDto.EstimateTechnologyId = technologyId;
                        EstimateExpPriceDtoList.Add(new EstimateExpPriceDto
                        {
                            EstimateRoleExp = erexp.Name,
                            EstimateRoleExpID = erexp.Id,
                        });
                    }
                }
                estimatePriceDto.EstimateExpPriceDtoList = EstimateExpPriceDtoList;
            }

            return View(estimatePriceDto);
        }

        [HttpPost]
        public ActionResult ManageEstimatePrice(EstimatePriceDto objEstimatePriceDto)
        {
            try
            {
                var EstimateRoleTechnoloyPriceList = new List<EstimateRoleTechnoloyPrice>();
                if (objEstimatePriceDto.EstimateRoleId != 1)
                {
                    objEstimatePriceDto.EstimateTechnologyId = null;
                }
                bool IsEdit = (objEstimatePriceDto.Id > 0) ? true : false;

                foreach (var item in objEstimatePriceDto.EstimateExpPriceDtoList)
                {
                    var EstimateRoleTechnoloyPrice = new EstimateRoleTechnoloyPrice
                    {
                        EstimateRoleExpId = item.EstimateRoleExpID,
                        EstimateTechnologyId = objEstimatePriceDto.EstimateTechnologyId,
                        Price = item.Price,
                    };
                    EstimateRoleTechnoloyPriceList.Add(EstimateRoleTechnoloyPrice);
                }

                EstimatePriceService.SaveEstimatePrice(EstimateRoleTechnoloyPriceList);

                ShowSuccessMessage("Success!", "Record saved successfully", false);
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error", "Somethng Went Wrong..!!", false);
            }
            return RedirectToAction("Index");

        }



        private List<SelectListItem> GetEstimateRoleExpList()
        {
            var EstimateRoleExpList = EstimatePriceService.GetEstimateRoleExpList();
            var selectEstimateRoleExpList = EstimateRoleExpList.Select(x => new SelectListItem
            {
                Text = x.Name.ToString(),
                Value = x.Id.ToString(),
            }).ToList();
            return selectEstimateRoleExpList;
        }
        private List<SelectListItem> GetTecnologyParentList()
        {
            var technologyParentList = EstimatePriceService.GettechnologyParentList();
            var selecttechnologyParentList = technologyParentList.Select(x => new SelectListItem
            {
                Text = x.Title.ToString(),
                Value = x.Id.ToString(),
            }).ToList();
            return selecttechnologyParentList;
        }
        private List<SelectListItem> GetEstimateRoleList(int selectid = 0)
        {
            var EstimateRoleList = EstimatePriceService.GetEstimateRoleList();
            var selectEstimateRoleList = EstimateRoleList.Select(x => new SelectListItem
            {
                Text = x.Name.ToString(),
                Value = x.Id.ToString(),
                Selected = x.Id == selectid,
            }).ToList();
            return selectEstimateRoleList;
        }

        [HttpPost]
        public IActionResult fetchRoleDependentExperience(int EstimateRoleId, int TechnologyParentId)
        {
            List<EstimateExpPriceDto> EstimateExpPriceDtoList = new List<EstimateExpPriceDto>();

            var estimatePrice = EstimatePriceService.GetEstimateRoleTechnoloyPriceByRoleId(EstimateRoleId);

            foreach (var item in estimatePrice)
            {
                EstimateExpPriceDtoList.Add(new EstimateExpPriceDto
                {
                    EstimateRoleExp = item.EstimateRoleExp.Name,
                    EstimateRoleExpID = item.EstimateRoleExpId,
                    Price = item.Price
                });
            }
            var EstimatePriceDto = new EstimatePriceDto();
            EstimatePriceDto.EstimateExpPriceDtoList = EstimateExpPriceDtoList;
            EstimatePriceDto.EstimateRoleId = EstimateRoleId;
            EstimatePriceDto.EstimateTechnologyId = TechnologyParentId;

            return PartialView("_EstimateExpPrice", EstimatePriceDto);

        }
    }
}
