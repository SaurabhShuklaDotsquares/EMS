using DataTables.AspNet.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EMS.Data;
using EMS.Service;
using EMS.Core;
using EMS.Web.Code.LIBS;
using EMS.Dto;
using EMS.Web.Code.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Web.Controllers
{
    [CustomAuthorization]
    public class TechnologyController : BaseController
    {
        private readonly ITechnologyService technologyService;
        public TechnologyController(ITechnologyService technologyService)
        {
            this.technologyService = technologyService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IDataTablesRequest request)
        {
            var pagingServices = new PagingService<Technology>(request.Start, request.Length);
            var filterExpr = PredicateBuilder.True<Technology>();
            if (!string.IsNullOrWhiteSpace(request.Search.Value))
            {
                filterExpr = filterExpr.And(x => x.Title.Contains(request.Search.Value));
            }


            pagingServices.Filter = filterExpr;

            pagingServices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.ModifyDate);
            };

            int totalCount = 0;
            var response = technologyService.GetTechnologyByPaging(out totalCount, pagingServices);
            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                TechId = r.TechId,
                rowIndex = (index + 1) + (request.Start),
                Title = r.Title,
                Status = r.IsActive.HasValue ? r.IsActive.Value : false,
            }));
        }

        #region Add Technology


        [HttpGet]
        public ActionResult Add(int? id)
        {
            TechnologyDto model = new TechnologyDto();
            if (id > 0)
            {
                var technology = technologyService.GetTechnologyById(id.Value);
                if (technology != null)
                {
                    model.TechId = technology.TechId;
                    model.Title = technology.Title;
                    model.IsActive = technology.IsActive.HasValue ? technology.IsActive.Value : false;
                }
            }
            return PartialView("_AddEditTechnology", model);
        }

        [HttpPost]
        public ActionResult Add(TechnologyDto model)
        {
            if (ModelState.IsValid)
            {
                var isTechnologyExists = technologyService
                        .IsTechnologyExists(model.TechId, model.Title);
                if (isTechnologyExists)
                {
                    return NewtonSoftJsonResult(
                        new RequestOutcome<string> { Message = "Record already exists, please try another name.", IsSuccess = false });
                }

                var result = technologyService.Save(model);

                if (result == null)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record not saved.", IsSuccess = false });
                }
                return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record saved successfully", IsSuccess = true });

            }
            return CreateModelStateErrors();
        }

        #endregion

        #region update Status
        [HttpGet]
        public ActionResult UpdateStatus(int id)
        {
            TechnologyDto model = null;
            if (id > 0)
            {
                technologyService.UpdateStatus(id);
            }
            return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "Record status has been updated successfully", IsSuccess = true, Data = model });
        }
        #endregion
    }
}