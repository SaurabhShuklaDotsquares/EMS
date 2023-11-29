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
using EMS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Website.Controllers
{
    [CustomAuthorization]
    public class DomainController : BaseController
    {
        private readonly IDomainTypeService domainService;
        public DomainController(IDomainTypeService domainService)
        {
            this.domainService = domainService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IDataTablesRequest request)
        {
            var pagingServices = new PagingService<DomainType>(request.Start, request.Length);
            var filterExpr = PredicateBuilder.True<DomainType>();
            if (!string.IsNullOrWhiteSpace(request.Search.Value))
            {
                filterExpr = filterExpr.And(x => x.DomainName.Contains(request.Search.Value));
            }


            pagingServices.Filter = filterExpr;

            pagingServices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.AddDate);
            };

            int totalCount = 0;
            var response = domainService.GetDomainsByPaging(out totalCount, pagingServices);
            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                DomainId = r.DomainId,
                rowIndex = (index + 1) + (request.Start),
                Title = r.DomainName,
                Status = r.IsActive.HasValue ? r.IsActive.Value:false,
            }));
        }



        #region Add Domain

        [HttpGet]
        public ActionResult Add(int? id)
        {
            DomainTypeDto model = new DomainTypeDto();
            if (id > 0)
            {
                var domain = domainService.GetDomainById(id.Value);
                if (domain != null)
                {
                    model.DomainId = domain.DomainId;
                    model.DomainName = domain.DomainName;
                    model.IsActive = domain.IsActive.HasValue ? domain.IsActive.Value : false;
                }
            }
            return PartialView("_AddEditDomain", model);
        }

        [HttpPost]
        public ActionResult Add(DomainTypeDto model)
        {
            if (ModelState.IsValid)
            {
                var isTechnologyExists = domainService
                        .IsTechnologyExists(model.DomainId, model.DomainName);
                if (isTechnologyExists)
                {
                    return NewtonSoftJsonResult(
                        new RequestOutcome<string> { Message = "Record already exists, please try another name.", IsSuccess = false });
                }
                
                var result = domainService.Save(model);

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
            DomainTypeDto model = null;
            if (id > 0)
            {
                domainService.UpdateStatus(id);
            }
            return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "Record status has been updated successfully", IsSuccess = true, Data = model });
        }
        #endregion
    }
}