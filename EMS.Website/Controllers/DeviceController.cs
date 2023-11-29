using DataTables.AspNet.Core;
using DataTables.AspNet.AspNetCore;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EMS.Web.Code.LIBS;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class DeviceController : BaseController
    {
        #region Constructor and Member

        private readonly IDeviceService deviceService;
        private readonly IUserLoginService userLoginService;
        private readonly IDeviceDetailService deviceDetailService;

        public DeviceController(IDeviceService _deviceService,
            IUserLoginService _userLoginService,
            IDeviceDetailService _deviceDetailService)
        {
            deviceService = _deviceService;
            userLoginService = _userLoginService;
            deviceDetailService = _deviceDetailService;
        }

        private bool isPMTypeUser
        {
            get
            {
                return CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO || SiteKey.AllowDeviceUserId.Split(',').ToList().Contains(CurrentUser.Uid.ToString());
            }
        }

        #endregion

        #region Device History

        [HttpGet]
        [CustomActionAuthorization()]
        public ActionResult Index()
        {
            var model = new AssignedHistoryIndexDto();

            model.DeviceTypeList = WebExtensions.GetSelectList<Enums.DeviceType>();
            if (CurrentUser.RoleId == (int)Enums.UserRoles.PMO || CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM)
            {
                model.UserList = userLoginService.GetUsersByPM(PMUserId).ToSelectList(x => x.Name, x => x.Uid);
            }
            model.AllowManage = isPMTypeUser;

            return View(model);
        }

        [HttpPost]
        [CustomActionAuthorization()]
        public IActionResult Index(IDataTablesRequest request, int historyType, byte? deviceType, int? deviceId, int? userId)
        {
            var pagingServices = new PagingService<DeviceDetail>(request.Start, request.Length);
            var filterExpr = PredicateBuilder.True<DeviceDetail>().And(x => x.Device.PMUid == PMUserId);

            if (historyType == 1)
            {
                filterExpr = filterExpr.And(x => !x.SubmitToUid.HasValue);
            }

            if (deviceType.HasValue)
            {
                filterExpr = filterExpr.And(x => x.Device.DeviceType == deviceType.Value);
            }

            if (deviceId.HasValue)
            {
                filterExpr = filterExpr.And(x => x.DeviceId == deviceId.Value);
            }

            /* filter which user have devices */
            if (userId.HasValue)
            {
                filterExpr = filterExpr.And(x => x.UserLogin.Uid == userId.Value);
            }

            bool isPMTypeCurrentUser = isPMTypeUser;

            if (isPMTypeCurrentUser)
            {
                filterExpr = filterExpr.And(x => x.UserLogin.PMUid == PMUserId || x.AssignedToUid == CurrentUser.Uid);
            }
            else
            {
                filterExpr = filterExpr.And(x => x.AssignedToUid == CurrentUser.Uid);
            }

            pagingServices.Filter = filterExpr;

            pagingServices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.Id);
            };

            int totalCount = 0;
            var response = deviceDetailService.GetDeviceDetailByPaging(out totalCount, pagingServices);


            return DataTablesJsonResult(totalCount, request, response.Select((d, index) =>
            {
                var prevDetail = d.Device.DeviceDetails.Where(x => x.SubmitDateTime.HasValue && x.Id < d.Id && x.SerialNumber == d.SerialNumber)
                                  .OrderByDescending(x => x.CreateDate)                                 
                                  .FirstOrDefault();
                var detail = new
                {
                    rowIndex = (index + 1) + (request.Start),
                    d.Id,
                    DeviceName = $"{((Enums.DeviceType)d.Device.DeviceType).GetEnumDisplayName()} : {d.Device.Name}",
                    SIMDetails = d.Device.SimNetwork.HasValue() ? $"{d.Device.SimNetwork} / {d.Device.SimNumber}" : "",
                    d.Condition,
                    AssignedTo = d.UserLogin?.Name,
                    AssignedBy = d.UserLogin1?.Name,
                    AssignedDate = d.SubmitDateTime.HasValue ? $"{d.AssignedDateTime.ToFormatDateString("dd MMM yyyy")} to {d.SubmitDateTime.ToFormatDateString("dd MMM yyyy")}" : d.AssignedDateTime.ToFormatDateString("dd MMM yyyy"),
                    PrevDeviceUser = prevDetail?.UserLogin.Name,
                    PrevDeviceUserPeriod = $"{prevDetail?.AssignedDateTime.ToFormatDateString("dd MMM yyyy")} to {prevDetail?.SubmitDateTime.ToFormatDateString("dd MMM yyyy")}",
                    RecieveAllowed = isPMTypeCurrentUser && !d.SubmitDateTime.HasValue,
                    IsSubmitted = d.SubmitDateTime.HasValue,
                    d.SerialNumber
                };
                return detail;
            }));
        }

        [HttpGet]
        [CustomActionAuthorization()]
        public ActionResult AddEditAssignDevice(int? id)
        {
            var model = BindModel();

            if (id.HasValue)
            {
                var deviceDetail = deviceDetailService.GetDeviceDetailById(id.Value);
                model.Id = id.Value;
                model.Condition = deviceDetail.Condition;
            }
            return PartialView("_AddEditAssignDevice", model);
        }

        [HttpPost]
        [CustomActionAuthorization()]
        public ActionResult AddEditAssignDevice(AssignDeviceDto model)
        {
            try
            {
                if (model.Id > 0)
                {
                    var result = deviceDetailService.UpdateConditon(model);
                    if(result !=null)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Device Condition Update Sucessfully", IsSuccess = true });
                    }
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "OOPS! Something went wrong.", IsSuccess = true });
                }
                else
                {
                    if (IsQuantityAvailable(model.DeviceId))
                    {
                        model.CreateByUid = CurrentUser.Uid;
                        model.ModifyByUid = CurrentUser.Uid;

                        var result = deviceDetailService.SaveAssignDevice(model);
                        if (result != null)
                        {
                            return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record Saved Sucessfully", IsSuccess = true });
                        }
                        else
                        {
                            return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = "Something went wrong", IsSuccess = false });
                        }
                    }
                    else
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = "Quantity is not availabe for selected device", IsSuccess = false });
                    }
                }

            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = ex.Message, IsSuccess = false });
            }
        }

        [HttpGet]
        [CustomActionAuthorization()]
        public ActionResult GetDeviceNameList(int deviceType)
        {
            return NewtonSoftJsonResult(deviceService.GetDeviceList(deviceType, PMUserId)
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    AvailableStock = s.Quantity - s.DeviceDetails.Count(x => !x.SubmitToUid.HasValue)
                }));
        }

        private AssignDeviceDto BindModel()
        {
            var model = new AssignDeviceDto();

            model.DeviceTypeList = Code.LIBS.WebExtensions.GetSelectList<Enums.DeviceType>();

            List<UserLogin> users = new List<UserLogin>();

            if (isPMTypeUser)
            {
                users = userLoginService.GetUsersByPM(PMUserId);
            }
            else
            {
                users.Add(new UserLogin { Name = CurrentUser.Name, Uid = CurrentUser.Uid });
            }
            model.AssignedToUid = CurrentUser.Uid;
            model.AssignedUserList = users.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Uid.ToString()
            }).ToList();

            return model;
        }

        private bool IsQuantityAvailable(int deviceId)
        {
            var isAvailable = false;
            try
            {
                var device = deviceService.GetDeviceById(deviceId);
                isAvailable = device.Quantity > device.DeviceDetails.Count(x => !x.SubmitToUid.HasValue);
            }
            catch
            {
            }
            return isAvailable;
        }

        [HttpGet]
        [CustomActionAuthorization()]
        public ActionResult AddReturnDevice(int id)
        {
            if (id > 0)
            {
                var detail = deviceDetailService.GetDeviceDetailById(id);

                if (detail != null)
                {
                    var model = new ReturnDeviceDto();

                    model.ReturnedUserList = userLoginService.GetUsersByRoles(new int[] { (int)Enums.UserRoles.PM, (int)Enums.UserRoles.PMO, (int)Enums.UserRoles.UKBDM }, PMUserId)
                        .Select(x => new SelectListItem
                        {
                            Text = x.Name,
                            Value = x.Uid.ToString()
                        }).ToList();

                    model.DeviceDetailId = id;
                    model.ReturnToUid = CurrentUser.Uid;
                    model.AssignedDate = detail.AssignedDateTime.ToFormatDateString("yyyy-MM-dd");

                    return PartialView("_AddReturnDevice", model);
                }
            }

            return MessagePartialView("No record found");
        }

        [HttpPost]
        [CustomActionAuthorization()]
        public ActionResult AddReturnDevice(ReturnDeviceDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = deviceDetailService.UpdateReturnDevice(model);

                    if (result != null)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Device received sucessfully.", IsSuccess = true });
                    }
                    else
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = "Something went Wrong.", IsSuccess = false });
                    }
                }
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = ex.Message, IsSuccess = false });
            }
            return CreateModelStateErrors();
        }

        #endregion

        #region Device Master

        [HttpGet]
        [CustomActionAuthorization()]
        public ActionResult DeviceMaster()
        {
            return View();
        }

        [HttpPost]
        [CustomActionAuthorization()]
        public IActionResult DeviceMaster(IDataTablesRequest request)
        {
            var pagingServices = new PagingService<Device>(request.Start, request.Length);
            var filterExpr = PredicateBuilder.True<Device>();

            filterExpr = filterExpr.And(x => x.PMUid == PMUserId);

            pagingServices.Filter = filterExpr;

            pagingServices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.CreateDate);
            };

            int totalCount = 0;
            var response = deviceService.GetDeviceByPaging(out totalCount, pagingServices);

            return DataTablesJsonResult(totalCount, request, response.Select((device, index) =>
            {
                var dev = new
                {
                    rowIndex = (index + 1) + (request.Start),
                    Id = device.Id,
                    DeviceName = $"{((Enums.DeviceType)device.DeviceType).GetEnumDisplayName()} : {device.Name}",
                    SIMDetails = device.SimNetwork.HasValue() ? $"{device.SimNetwork} / {device.SimNumber}" : "",
                    Condition = device.Condition,
                    Quantity = device.Quantity,
                    AssignedQuantity = device.DeviceDetails.Count(x => !x.SubmitDateTime.HasValue),
                    AddedBy = device.UserLogin.Name,
                    AddedDate = device.CreateDate.ToFormatDateString("dd MMM, yyyy hh:mm tt"),
                    ModifiedBy = device.UserLogin1?.Name,
                    ModifiedDate = device.ModifyDate.ToFormatDateString("dd MMM, yyyy hh:mm tt"),
                };
                return dev;
            }));
        }

        [HttpGet]
        [CustomActionAuthorization()]
        public ActionResult AddEditDevice(int? id)
        {
            var model = new DeviceDataDto();

            if (id.HasValue)
            {
                var device = deviceService.GetDeviceById(id.Value);
                if (device.PMUid == PMUserId)
                {
                    model.Id = id.Value;
                    model.DeviceType = device.DeviceType;
                    model.Name = device.Name;
                    model.Quantity = device.Quantity;
                    model.Condition = device.Condition;
                    model.SimNumber = device.SimNumber;
                    model.SimNetwork = device.SimNetwork;
                    model.AssignedQuantity = device.DeviceDetails.Count(x => !x.SubmitDateTime.HasValue);
                }
                else
                {
                    return MessagePartialView("No record found");
                }
            }

            model.DeviceTypeList = Code.LIBS.WebExtensions.GetSelectList<Enums.DeviceType>();

            return PartialView("_AddEditDevice", model);

        }

        [HttpPost]
        [CustomActionAuthorization()]
        public ActionResult AddEditDevice(DeviceDataDto model)
        {
            try
            {

                model.CurrentUserId = CurrentUser.Uid;
                model.PMUid = PMUserId;
                model.Quantity = model.DeviceType == (int)Enums.DeviceType.Sim ? 1 : model.Quantity;

                var result = deviceService.Save(model);
                if (result != null)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record Saved Sucessfully.", IsSuccess = true });
                }
                else
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = "Something went Wrong.", IsSuccess = false });
                }

            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = ex.Message, IsSuccess = false });
            }

        }


        //[HttpGet]
        //public ActionResult DeleteDevice()
        //{
        //    return PartialView("_ModalDelete", new Modal
        //    {
        //        Message = "Are you sure you want to delete this Device?",
        //        Size = Enums.ModalSize.Small,
        //        Header = new ModalHeader { Heading = "Delete Device" },
        //        Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
        //    });
        //}

        //[HttpPost]
        //public ActionResult DeleteDevice(int id)
        //{
        //    try
        //    {
        //        if (id > 0)
        //        {
        //            deviceService.Delete(id);
        //            return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = true, Message = "Device Deleted Successfully!!" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = ex.Message });
        //    }
        //    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "" });
        //}

        #endregion
    }
}