using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EMS.Dto;
using EMS.Service;
using Microsoft.AspNetCore.Mvc.Rendering;
using EMS.Core;
using EMS.Data;
using DataTables.AspNet.Core;
using EMS.Web.Code.LIBS;
using EMS.Web.Code.Attributes;
using EMS.Web.Models.Others;

namespace EMS.Web.Controllers
{
    public class OrgImprovementController : BaseController
    {
        #region "Constructor and fields"
        private readonly IOrgImprovementService improvementService;
        private readonly IUserLoginService userLoginService;
        public OrgImprovementController(IOrgImprovementService _improvementService, IUserLoginService _userLoginService)
        {
            this.improvementService = _improvementService;
            this.userLoginService = _userLoginService;
        }
        #endregion
        #region "Org Improvement Index"
        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult Index()
        {
            OrgImprovementDto model = new OrgImprovementDto();

            //model.Users = userLoginService.GetUsersByPM(CurrentUser.PMUid)?
            //    .Select(u => new SelectListItem { Text = u.Name, Value = u.Uid.ToString() }).ToList();
            return View();
        }
        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult Index(IDataTablesRequest request)
        {            
            var pagingServices = new PagingService<OrgImprovement>(request.Start, request.Length);
            var expr = PredicateBuilder.True<OrgImprovement>();

            bool isPMUser = CurrentUser.RoleId == (int)Enums.UserRoles.PM;
            int currentUserId = CurrentUser.Uid;

            if (isPMUser)
            {
                expr = expr.And(x => x.EmployeeUid == currentUserId || x.EmployeeU.PMUid == currentUserId);
            }
            else if (RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_Sales_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_HR_DesignationIds.Contains(CurrentUser.DesignationId))
            {
                expr = expr.And(x => x.EmployeeUid == currentUserId || x.EmployeeU.TLId == currentUserId);
            }
            else
            {
                expr = expr.And(x => x.EmployeeUid == currentUserId);
            }

            if (request.Search.Value.HasValue())
            {
                string searchValue = request.Search.Value.Trim().ToLower();
                expr = expr.And(x => x.EmployeeU != null && x.EmployeeU.Name.ToLower().Contains(searchValue));
            }

            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.ModifyDate);
            };

            int totalCount = 0;

            var response = improvementService.GetImprovementByPaging(out totalCount, pagingServices);

            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                rowIndex = (index + 1) + (request.Start),
                r.Id,
                r.Title,
                r.EmployeeU?.Name,
                Type = ((Enums.ImprovementType)r.TypeId).GetDescription(),
                ImprovementDate = r.ImprovementDate.ToFormatDateString("MMM dd, yyyy"),
                AddedBy = r.AddedByU?.Name,
                AllowEditDelete = isPMUser || r.EmployeeUid == currentUserId
            }));
        }
        #endregion

        [HttpGet]
        [CustomActionAuthorization]
        #region "Add/ Edit Org Improvement"
        public ActionResult Add(int? id)
        {
            try
            {
                OrgImprovementDto model = new OrgImprovementDto();

                model.Users = userLoginService.GetUsersByPM(PMUserId)?
                    .ToSelectList(u => u.Name, u => u.Uid);
                model.ImprovementTypes = WebExtensions.GetSelectList<Enums.ImprovementType>();
                model.TypeId = Enums.ImprovementType.Individual;

                if (id.HasValue && id.Value > 0)
                {
                    var improvement = improvementService.GetImprovementById(id.Value);
                    if (improvement != null)
                    {
                        model.Title = improvement.Title;
                        model.TypeId = (Enums.ImprovementType)improvement.TypeId;
                        model.Description = improvement.Description;
                        model.EmployeeUid = improvement.EmployeeUid;
                        model.ImprovementDate = improvement.ImprovementDate.ToFormatDateString("dd/MM/yyyy");
                    }
                    else
                    {
                        return MessagePartialView("Unable to find record");
                    }

                }
                return PartialView("_AddEditImprovement", model);
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }
        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult Add(OrgImprovementDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {                    
                    model.CurrentUserId = CurrentUser.Uid;
                    //in PM and TL case values will be retrieved from drop down
                    if (CurrentUser.RoleId != (int)Enums.UserRoles.PM && (RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_Sales_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_HR_DesignationIds.Contains(CurrentUser.DesignationId)))
                    {
                        model.EmployeeUid = model.CurrentUserId;
                    }
                    var result = improvementService.Save(model);

                    if (result)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = "Org Improvement saved successfully"
                        });
                    }
                    else
                    {
                        return MessagePartialView("Unable to save record");
                    }
                }
                catch (Exception ex)
                {
                    return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
                }
            }
            else
            {
                return CreateModelStateErrors();
            }
        }
        #endregion

        #region "Delete Org Improvement"
        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this?",
                Size = Enums.ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Org Improvement?" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult delete(int id)
        {
            try
            {
                if (id > 0)
                {
                    var improvement = improvementService.GetImprovementById(id);
                    if (improvement != null && improvement.AddedByUid == CurrentUser.Uid)
                    {
                        improvementService.Delete(id);
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record deleted successfully.", IsSuccess = true });
                    }
                }

                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = "Unable to delete this org improvement" });
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = ex.Message });
            }
        }
        #endregion
    }
}