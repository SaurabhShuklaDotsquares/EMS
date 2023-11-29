using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EMS.Web.Controllers
{
    public class PILogController : BaseController
    {
        #region Fields and Constructor

        private readonly IPILogService PILogService;
        private readonly IUserLoginService userLoginService;
        private readonly IProcessService processService;

        public PILogController(IPILogService _PILogService, IUserLoginService _userLoginService, IProcessService _processService)
        {
            PILogService = _PILogService;
            userLoginService = _userLoginService;
            processService = _processService ?? throw new ArgumentNullException("_processService");
        }

        #endregion

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Index()
        {
            var model = new PILogIndexDto();

            if (CurrentUser.IsSPEG)
            {
                model.PMUserList = userLoginService.GetUserByRole((int)Enums.UserRoles.PM, true)
                                    .Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() })
                                    .ToList();
            }
            model.PILogStatusList = Extensions.EnumToDictionaryWithDescription(typeof(Enums.PILogStatus))
                                               .Select(x => new SelectListItem { Text = x.Key, Value = x.Value.ToString() })
                                               .ToList();
            return View(model);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult Index(IDataTablesRequest request, int? pmId, Enums.PILogStatus? status)
        {
            var pagingServices = new PagingService<PILog>(request.Start, request.Length);
            var filterExpr = PredicateBuilder.True<PILog>();
            var currentUserId = CurrentUser.Uid;

            if (!CurrentUser.IsSPEG)
            {
                if (CurrentUser.RoleId == (int)Enums.UserRoles.PM)
                {
                    filterExpr = filterExpr.And(x => x.UserLogin.PMUid == currentUserId || x.CreateByUid == currentUserId);
                }
                else
                {
                    filterExpr = filterExpr.And(x => x.CreateByUid == currentUserId);
                }
            }

            if (pmId.HasValue && pmId.Value > 0)
            {
                filterExpr = filterExpr.And(x => x.UserLogin.PMUid == pmId.Value || x.CreateByUid == pmId.Value);
            }

            if (status.HasValue && Enum.IsDefined(typeof(Enums.PILogStatus), status.Value))
            {
                byte logStatus = (byte)status.Value;
                filterExpr = filterExpr.And(x => x.Status == logStatus);
            }

            pagingServices.Filter = filterExpr;

            pagingServices.Sort = (o) =>
            {
                return o.OrderByDescending(c => c.ModifyDate);
            };

            int totalCount = 0;
            var response = PILogService.GetLogsByPaging(out totalCount, pagingServices);

            return DataTablesJsonResult(totalCount, request, response.Select((log, index) => new
            {
                rowIndex = (index + 1) + (request.Start),
                Id = log.Id,
                ProcessName = log.ProcessName,
                PotentialImprovementArea = log.PotentialArea,
                CreateDate = log.CreateDate.ToFormatDateString("dd MMM yyyy hh:mm tt"),
                EstimatedSchedule = log.EstimatedSchedule.ToFormatDateString("dd MMM yyyy"),
                Status = ((Enums.PILogStatus)log.Status).ToString(),
                SuggestedBy = log.UserLogin.Name,
                ApprovalAllowed = CurrentUser.IsSPEG && log.Status == (byte)Enums.PILogStatus.Pending || log.Status == (byte)Enums.PILogStatus.InProcess,
                EditAllowed = log.CreateByUid == currentUserId && log.Status == (byte)Enums.PILogStatus.Pending,
                RollOutAllowed = CurrentUser.IsSPEG && log.Status == (byte)Enums.PILogStatus.Approved
            }));
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult AddEdit(int? id)
        {
            try
            {
                var model = new PILogRequestDto();

                var processes = processService.GetAllProcess().OrderBy(x=>x.ProcessName);
                var otherProcesses = processService.GetAllProcess().FirstOrDefault(x => string.Equals(x.ProcessName, "Other",StringComparison.OrdinalIgnoreCase));

                foreach (var process in processes)
                {
                    if (!string.Equals(process.ProcessName, "Other", StringComparison.OrdinalIgnoreCase))
                    {
                        model.ProcessList.Add(new SelectListItem { Text = process.ProcessName, Value = process.Id.ToString() });
                    }
                }

                if(otherProcesses != null)
                {
                    model.ProcessList.Add(new SelectListItem { Text = otherProcesses.ProcessName, Value = otherProcesses.Id.ToString() });
                }

                if (id.HasValue && id.Value > 0)
                {
                    var logEntity = PILogService.GetPILogById(id.Value);

                    if (logEntity != null)
                    {
                        if (logEntity.CreateByUid == CurrentUser.Uid && logEntity.Status == (byte)Enums.PILogStatus.Pending)
                        {
                            model.Id = logEntity.Id;
                            model.ProcessName = logEntity.ProcessName;
                            model.PotentialArea = logEntity.PotentialArea;
                            model.ProcessId = logEntity.ProcessId;
                        }
                        else
                        {
                            return MessagePartialView("Invalid access or request has been processed");
                        }
                    }
                    else
                    {
                        return MessagePartialView("Unable to find record");
                    }
                }

                return PartialView("_AddEdit", model);
            }
            catch (Exception ex)
            {
                return CustomErrorView(ex.Message);
            }
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult AddEdit(PILogRequestDto model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    model.CurrentUserId = CurrentUser.Uid;

                    var result = PILogService.Save(model);

                    if (result != null && result.Id > 0)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = "PI Log saved successfully"
                        });
                    }
                    else
                    {
                        return MessagePartialView("Unable to save record");
                    }
                }
                catch (Exception ex)
                {
                    return MessagePartialView(ex.Message);
                }
            }
            else
            {
                return CreateModelStateErrors();
            }
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult RequestApproval(int id)
        {
            try
            {
                if (id > 0)
                {
                    var logEntity = PILogService.GetPILogById(id);
                    var ModifyBy = userLoginService.GetUserInfoByID(logEntity.ModifyByUid);
                    if (logEntity != null)
                    {
                        var model = new PILogApprovalDto
                        {
                            Id = logEntity.Id,
                            ProcessName = logEntity.ProcessName,
                            PotentialArea = logEntity.PotentialArea,
                            CreateBy = logEntity.UserLogin.Name,
                            CancelReason = logEntity.CancelReason,
                            EstimatedSchedule = logEntity.EstimatedSchedule.ToFormatDateString("dd MMM yyyy"),
                            Remarks = logEntity.Remarks,
                            Status = logEntity.Status,
                            ModifyBy = ModifyBy.Name,
                            CreateDate = logEntity.CreateDate.ToFormatDateString("dd MMM yyyy hh:mm tt"),
                            ModifyDate = logEntity.ModifyDate.ToFormatDateString("dd MMM yyyy"),
                            ApprovalAllowed = CurrentUser.IsSPEG && (logEntity.Status == (byte)Enums.PILogStatus.Pending || logEntity.Status == (byte)Enums.PILogStatus.InProcess),
                            RollOutAllowed = CurrentUser.IsSPEG && logEntity.Status == (byte)Enums.PILogStatus.Approved
                        };

                        if (model.ApprovalAllowed)
                        {
                            model.StatusList = new List<SelectListItem>
                            {
                                new SelectListItem { Text= Enums.PILogStatus.InProcess.ToString(), Value= ((byte)Enums.PILogStatus.InProcess).ToString()},
                                new SelectListItem { Text= Enums.PILogStatus.Approved.ToString(), Value= ((byte)Enums.PILogStatus.Approved).ToString()},
                                new SelectListItem { Text= Enums.PILogStatus.Cancelled.ToString(), Value= ((byte)Enums.PILogStatus.Cancelled).ToString()}
                            };
                        }

                        return PartialView("_RequestApproval", model);
                    }
                    else
                    {
                        return MessagePartialView("Record not found");
                    }
                }
                else
                {
                    return MessagePartialView("Invalid log id");
                }
            }
            catch (Exception ex)
            {
                return CustomErrorView(ex.Message);
            }
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult RequestApproval(PILogApprovalDto model)
        {
            #region Custom Validation

            if (model.Status == (byte)Enums.PILogStatus.Cancelled && string.IsNullOrWhiteSpace(model.CancelReason))
            {
                ModelState.AddModelError("CancelReason", "Cancel Reason is required");
            }
            else if (model.Status == (byte)Enums.PILogStatus.Approved)
            {
                if (string.IsNullOrWhiteSpace(model.Remarks))
                {
                    ModelState.AddModelError("Remarks", "Remarks field is required");
                }
                if (string.IsNullOrWhiteSpace(model.EstimatedSchedule))
                {
                    ModelState.AddModelError("EstimatedSchedule", "Estimated Schedule is required");
                }
            }
            else if (model.Status == (byte)Enums.PILogStatus.InProcess && string.IsNullOrWhiteSpace(model.Remarks))
            {
                ModelState.AddModelError("Remarks", "Remarks field is required");
            }

            #endregion

            if (ModelState.IsValid)
            {
                try
                {
                    model.CurrentUserId = CurrentUser.Uid;

                    var result = PILogService.UpdateApproval(model);

                    if (result != null && result.Id > 0)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = $"PI Log marked as {(Enums.PILogStatus)result.Status} successfully"
                        });
                    }
                    else
                    {
                        return MessagePartialView("Unable to update record");
                    }
                }
                catch (Exception ex)
                {
                    return MessagePartialView(ex.Message);
                }
            }
            else
            {
                return CreateModelStateErrors();
            }
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult RollOut(int id)
        {
            if (id > 0 && CurrentUser.IsSPEG)
            {
                try
                {
                    var result = PILogService.RollOut(id, CurrentUser.Uid);

                    if (result != null && result.Id > 0)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = $"Log marked as {(Enums.PILogStatus)result.Status} successfully"
                        });
                    }
                    else
                    {
                        return MessagePartialView("Unable to update record");
                    }
                }
                catch (Exception ex)
                {
                    return MessagePartialView(ex.Message);
                }
            }
            else
            {
                return MessagePartialView("Invalid Log Id");
            }
        }
    }
}