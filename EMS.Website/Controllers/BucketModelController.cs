using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EMS.Service;
using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using Microsoft.AspNetCore.Http.Features;
using EMS.Web.Code.Attributes;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class BucketModelController : BaseController
    {
        #region Fields and Constructor
        private readonly IBucketModelService bucketModelService;
        public BucketModelController(IBucketModelService bucketModelService)
        {
            this.bucketModelService = bucketModelService;
        }
        #endregion
        
        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult Index(IDataTablesRequest request)
        {
            var pagingServices = new PagingService<BucketModel>(request.Start, request.Length);
            var filterExpr = PredicateBuilder.True<BucketModel>();
            if (!string.IsNullOrWhiteSpace(request.Search.Value))
            {
                filterExpr = filterExpr.And(x => x.ModelName.Contains(request.Search.Value));
            }


            pagingServices.Filter = filterExpr;

            pagingServices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.ModifyDate);
            };

            int totalCount = 0;
            var response = bucketModelService.GetBucketModelByPaging(out totalCount, pagingServices);
            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                BucketId = r.BucketId,
                rowIndex = (index + 1) + (request.Start),
                ModelName = r.ModelName,
                BucketCode = r.ModelCode,
                Status = r.IsActive.HasValue ? r.IsActive.Value : false,
            }));
        }

        #region Add Bucket Model

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Add(int? id)
        {
            BucketModelDto model = new BucketModelDto();
            if (id > 0)
            {
                var bucketModel = bucketModelService.GetBucketModelById(id.Value);
                if (bucketModel != null)
                {
                    model.BucketId = bucketModel.BucketId;
                    model.ModelName = bucketModel.ModelName;
                    model.ModelCode = bucketModel.ModelCode;

                    model.IsActive = bucketModel.IsActive.HasValue ? bucketModel.IsActive.Value : false;
                }
            }
            return PartialView("_AddEditBucketModel", model);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult Add(BucketModelDto model)
        {
            if (ModelState.IsValid)
            {
                var isBucketNameorCodeExists = bucketModelService
                    .IsBucketModelNameAndCodeExists(model.BucketId, model.ModelName, model.ModelCode);
                if (isBucketNameorCodeExists)
                {
                    return NewtonSoftJsonResult(
                       new RequestOutcome<string> { Message = "Record already exists, please try another bucket name or code.", IsSuccess = false });
                }

                model.IP = ContextProvider.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();// Request.UserHostAddress;
                var result = bucketModelService.Save(model);
                if (result == null)
                {
                    return NewtonSoftJsonResult(
                        new RequestOutcome<string> { Message = "Record not saved.", IsSuccess = false });
                }
                return NewtonSoftJsonResult(
                    new RequestOutcome<string> { Message = "Record saved successfully", IsSuccess = true });

            }
            return CreateModelStateErrors();
        }
        #endregion

        #region update Technology
        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult UpdateStatus(int id)
        {
            TechnologyDto model = null;
            if (id > 0)
            {
                bucketModelService.UpdateStatus(id);
            }


            return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "Record status has been updated successfully", IsSuccess = true, Data = model });
        }
        #endregion
    }
}