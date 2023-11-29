using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DataTables.AspNet.Core;
using DataTables.AspNet.AspNetCore;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.Models.Others;
using Microsoft.AspNetCore.Mvc.Rendering;
using EMS.Web.Controllers;

namespace EMS.Website.Controllers
{
    public class EventController : BaseController
    {
        #region Constructor and Member

        private readonly IEventService eventService;
        private readonly IUserLoginService userLoginService;

        public EventController(IEventService _eventService, IUserLoginService userLoginService)
        {
            this.eventService = _eventService;
            this.userLoginService = userLoginService;
        }

        #endregion

        #region Event Index 

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
            var pagingServices = new PagingService<OfficialLeave>(request.Start, request.Length);
            var expr = PredicateBuilder.True<OfficialLeave>();

            expr = expr.And(e => e.LeaveType == "Event");

            //if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
            //{
            //    expr = expr.And(x => x.AddedBy == CurrentUser.Uid || x.AddedByNavigation.PMUid == PMUserId);
            //}
            //else
            //{
            //    expr = expr.And(x => x.AddedBy == CurrentUser.Uid);
            //}

            if (request.Search.Value.HasValue())
            {
                string searchValue = request.Search.Value.Trim().ToLower();
                expr = expr.And(x => x.Title.Contains(searchValue));
            }

            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.LeaveDate);
            };

            int totalCount = 0;
            var isPMUser = IsPMEvent;
            var response = eventService.GetEventByPaging(out totalCount, pagingServices);

            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                rowIndex = (index + 1) + (request.Start),               
                Id = r.LeaveId,
                Title=r.Title,
                LeaveDate=r.LeaveDate.ToFormatDateString("MMM dd, yyyy"),
                IsActive=r.IsActive,
                LeaveType=r.LeaveType,
                AllowDelete = isPMUser,

            }));
        }

        #endregion

        #region ADD/EDIT Event

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Add(int? id)
        {
            if (CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM )
            {
                EventDto model = new EventDto();
                if (id.HasValue && id.Value > 0)
                {
                    var entity = eventService.GetEventById(id.Value);
                    if (entity != null)
                    {
                        model.LeaveId = entity.LeaveId;
                        model.Title = entity.Title;
                        model.LeaveDate = entity.LeaveDate.ToFormatDateString("dd/MM/yyyy");
                        model.CountryId = entity.CountryId;
                        model.IsActive = entity.IsActive;
                        model.LeaveType = entity.LeaveType;
                    }
                    else
                    {
                        return CustomErrorView("Unable to find record");
                    }
                }

                return View("AddEdit", model);
            }
            else
            {
                return AccessDenied();
            }
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult Add(EventDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.CountryId = (byte)Enums.Country.UK;
                    model.IsActive = true;
                    model.LeaveType = "Event";
                    var result = eventService.Save(model);

                    if (result != null )
                    {
                        ShowSuccessMessage("Success", "Event saved successfully", false);

                    }
                    return NewtonSoftJsonResult(new RequestOutcome<string>
                    {
                        IsSuccess = true,
                        Message = "Event saved successfully",
                        RedirectUrl = Url.Action("index")
                    });
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

        #region Delete Event

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Event?",
                Size = Enums.ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Event?" },
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
                    var events = eventService.GetEventById(id);
                    if (events != null && (CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM ))
                    {
                        eventService.Delete(events);
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record deleted successfully.", IsSuccess = true });
                    }
                }

                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = "Unable to delete this event" });
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = ex.Message });
            }
        }
        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult ApprovedStatus(int id)
        {
            if (id > 0)
            {
                var events = eventService.GetEventById(id);
                if (events != null)
                {
                    if (events.IsActive == true)
                    {
                        events.IsActive = false;
                    }
                    else
                    {
                        events.IsActive = true;
                    }
                    eventService.ApprovedStatus(events);
                }
            }
            return RedirectToAction("Index");
        }
        #endregion
    }
}
