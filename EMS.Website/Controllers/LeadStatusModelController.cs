using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using EMS.Service;
using DataTables.AspNet.Core;
using EMS.Data;
using EMS.Core;
using EMS.Dto;
using EMS.Web.Code.Attributes;
using EMS.Web.Models.Others;
using Microsoft.AspNetCore.Mvc.Rendering;
using EMS.Web.Code.LIBS;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class LeadStatusModelController : BaseController
    {
        #region Fields and Constructor
        private readonly ILeadStatusModelService leadStatusModelService;
        public LeadStatusModelController(ILeadStatusModelService leadStatusModelService)
        {
            this.leadStatusModelService = leadStatusModelService;
        }
        
        #endregion
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IDataTablesRequest request)
        {
            var pagingServices = new PagingService<LeadStatu>(request.Start, request.Length);

            pagingServices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.ModifyDate);
            };

            int totalCount = 0;
            var response = leadStatusModelService.GetLeadStatusModelByPaging(out totalCount, pagingServices);
            var ls = new List<LeadStatu>();
            foreach (var item in response)
            {
                if (item.ParentId.HasValue && item.ParentId.Value > 0)
                {
                    item.StatusName = String.Concat(Enumerable.Repeat("--", 1).ToArray()) + item.StatusName;
                }
                ls.Add(item);
            }


                return DataTablesJsonResult(totalCount, request, ls.Select((r, index) => new
            {
                StatusId = r.StatusId,
                rowIndex = (index + 1) + (request.Start),
                StatusName = r.StatusName,
            }));
        }

        #region Add Lead Status Model

        [HttpGet]
        public ActionResult Add(int? id)
        {
            LeadStatusModelDto model = new LeadStatusModelDto();
            if (id > 0)
            {
                var leadStatusModel = leadStatusModelService.GetLeadStatusModelById(id.Value);
                if (leadStatusModel != null)
                {
                    model.StatusId = leadStatusModel.StatusId;
                    model.ParentId= leadStatusModel.ParentId.HasValue? leadStatusModel.ParentId.Value:0;
                    model.StatusName = leadStatusModel.StatusName;
                    model.MailContent = leadStatusModel.MailContent;
                    model.FromEmail = leadStatusModel.FromEmail;
                    model.To = leadStatusModel.To;
                    model.CC = leadStatusModel.CC;
                    model.BCC = leadStatusModel.BCC;
                    model.StatusName = leadStatusModel.StatusName;
                    model.IP = leadStatusModel.IP;
                }
            }
            var leadstatuslist = leadStatusModelService.GetLeadStatusModelList();

            model.LeadStatusModelList = leadstatuslist.Select(x => new SelectListItem()
            {
                Text = x.StatusName,
                Value = x.StatusId.ToString()
            }).ToList();
            return PartialView("_AddEditLeadStatusModel", model);
        }

        [HttpPost]
        public ActionResult Add(LeadStatusModelDto model)
        {
            if (ModelState.IsValid)
            {
                var isLeadStatusNameExists = leadStatusModelService
                    .IsLeadStatusModelExists(model.StatusId, model.StatusName);
                if (isLeadStatusNameExists)
                {
                    return NewtonSoftJsonResult(
                        new RequestOutcome<string> { Message = "Record already exists, please try another name.", IsSuccess = false });
                }

                model.IP = GeneralMethods.Getip();// Request.UserHostAddress;
                var result = leadStatusModelService.Save(model);
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

        #region Delete Lead status
        [HttpGet]
        public ActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete?",
                Size = Enums.ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Task?" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        public ActionResult delete(int id)
        {
            try
            {
                if (id > 0)
                {
                        leadStatusModelService.DeleteLeadStatusModelById(id);
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record deleted successfully.", IsSuccess = true });
                }

                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = "Unable to delete task" });
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = ex.Message });
            }
        }
        #endregion

    }
}