using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EMS.API.Model;
using EMS.Core;
using EMS.Data;
using EMS.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSheetController : BaseApiController
    {
        #region Reference Variables

        private readonly IUserLoginService userLoginService;
        private readonly IDepartmentService departmentService;
        private readonly ITechnologyService technologyService;
        private readonly IProjectService projectService;
        private readonly ITimesheetService timesheetService;
        private readonly IVirtualDeveloperService virtualDeveloperService;

        #endregion

        #region Constructor
        public TimeSheetController(IApiKeyService _apiKeyService, IUserLoginService _userLoginService,
                                   IDepartmentService _departmentService, ITechnologyService _technologyService,
                                   IProjectService _projectService, ITimesheetService _timesheetService,
                                   IVirtualDeveloperService _virtualDeveloperService) : base(_apiKeyService)
        {
            userLoginService = _userLoginService;
            departmentService = _departmentService;
            technologyService = _technologyService;
            projectService = _projectService;
            timesheetService = _timesheetService;
            virtualDeveloperService = _virtualDeveloperService;
        }
        #endregion

        #region GetTimesheetsData
        // ========== Input JSON request of GetTimesheetsData method  ========
        // {"CRMProjectId":"7672", "DateFrom":"02/05/2019", "DateTo":"31/08/2019"}
        [Route("~/TimeSheet/GetTimesheetsData")]
        [HttpPost]
        public ResponseModel<List<TimeSheetModel>> GetMonthTimesheetsData(TimeSheetInputModel model)
        {
            DateTime? dtfrom = null;
            DateTime? dtTo = null;
            ResponseModel<List<TimeSheetModel>> response = new ResponseModel<List<TimeSheetModel>>();

            ResponseModel<string> authResponse = new ResponseModel<string>();
            authResponse = AuthorizeRequest();
            if (authResponse.Status)
            {
                if ((string.IsNullOrWhiteSpace(model.CRMProjectId)) || (model.CRMProjectId == "0"))
                {
                    response.Message = "CRM project id required. ";
                    response.Status = false;
                    return response;
                }

                if (!string.IsNullOrWhiteSpace(model.DateFrom))
                {
                    try
                    {
                        dtfrom = model.DateFrom.ToDateTime();
                    }
                    catch (Exception ex)
                    {
                        response.Status = false;
                        response.Message = "DateFrom format is not correct";
                        return response;
                    }
                }

                if (!string.IsNullOrWhiteSpace(model.DateTo))
                {
                    try
                    {
                        dtTo = model.DateTo.ToDateTime();
                    }
                    catch (Exception ex)
                    {
                        response.Status = false;
                        response.Message = "DateTo format is not correct";
                        return response;
                    }
                }

                try
                {
                    if (Convert.ToInt32(model.CRMProjectId) != 0)
                    {
                        var timeSheets = timesheetService.GetAllProjectUserTimeSheetByCRMId(Convert.ToInt32(model.CRMProjectId), dtfrom, dtTo);
                        var result = timeSheets.Select(y => new TimeSheetModel
                        {
                            TimeSheetId = Convert.ToInt32(y.UserTimeSheetID),
                            CRMId = y.Project.CRMProjectId.ToString(),
                            ProjectName = y.Project.Name,
                            TimeSheetDate = y.AddDate.ToString("dd/MM/yyyy"),
                            VirtualDeveloper = y.VirtualDeveloper_id != null ? y.VirtualDeveloper.VirtualDeveloper_Name : "",
                            VirtualDeveloperEmail = y.VirtualDeveloper.emailid != null ? y.VirtualDeveloper.emailid : "",
                            ActualDeveloper = y.UserLogin1 != null ? y.UserLogin1.Name : "",
                            ActualDeveloperEmail = y.UserLogin1 != null ? y.UserLogin1.EmailOffice : "",
                            WorkHours = y.WorkHours.ToString(@"hh\:mm"),
                            Description = y.Description != null ? y.Description : ""
                        }).ToList();

                        if (result != null && result.Count > 0)
                        {
                            response.Status = true;
                            response.Code = HttpStatusCode.OK;
                            response.Data = result;
                        }
                        else
                        {
                            response.Status = false;
                            response.Code = HttpStatusCode.BadRequest;
                            response.Message = "No record found.";
                            response.Data = null;
                        }
                    }
                }

                catch (Exception ex)
                {
                    response.Status = false;
                    response.Code = HttpStatusCode.BadRequest;
                    response.Data = null;
                    response.Errors = new string[] { "Request parameters are not in correct format!" };
                }
            }
            else
            {
                response.Status = false;
                response.Code = HttpStatusCode.BadRequest;
                response.Data = null;
                response.Errors = new string[] { "Authentication failed." };
            }
            return response;
        }

        #endregion

        #region GetTimesheetHoursData
        // ========== Input JSON request of GetTimesheetHoursData method  ========
        // {"CRMProjectId":"7672", "DateFrom":"02/05/2019", "DateTo":"31/08/2019"}
        [Route("~/TimeSheet/GetTimesheetHoursData")]
        [HttpPost]
        public ResponseModel<TimeSheetWorkHourResponseModel> GetTimesheetHoursData(TimeSheetWorkHourRequestModel model) {
            ResponseModel<TimeSheetWorkHourResponseModel> response = new ResponseModel<TimeSheetWorkHourResponseModel>();
            ResponseModel<string> authResponse = new ResponseModel<string>();
            authResponse = AuthorizeRequest();
            try {
                if (model != null) {
                    if (authResponse.Status) {
                        DateTime? dateFrom = null;
                        DateTime? dateTo = null;
                        if ((string.IsNullOrWhiteSpace(model.CRMProjectId)) || (model.CRMProjectId == "0")) {
                            response.Message = "CRM project id required. ";
                            response.Status = false;
                            return response;
                        }

                        if (!string.IsNullOrWhiteSpace(model.DateFrom))
                        {
                            try
                            {
                                dateFrom = model.DateFrom.ToDateTime();
                            }
                            catch (Exception ex)
                            {
                                response.Status = false;
                                response.Message = "DateFrom format is not correct";
                                return response;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(model.DateTo))
                        {
                            try
                            {
                                dateTo = model.DateTo.ToDateTime();
                            }
                            catch (Exception ex)
                            {
                                response.Status = false;
                                response.Message = "DateTo format is not correct";
                                return response;
                            }
                        }

                        if (Convert.ToInt32(model.CRMProjectId) > 0) {
                            List<UserTimeSheet> timeSheetList = timesheetService.GetAllTimeSheetByProjectCRMId(Convert.ToInt32(model.CRMProjectId),dateFrom,dateTo);
                            TimeSheetWorkHourResponseModel modelResponse = new TimeSheetWorkHourResponseModel();
                            var virtualDevs = projectService.GetDevelopersByCRMID(Convert.ToInt32(model.CRMProjectId));

                            if (timeSheetList != null && timeSheetList.Count > 0) {
                                double totalDevHours = timeSheetList.Where(t => t.VirtualDeveloper != null && t.VirtualDeveloper.VirtualDeveloper_Name != null && (t.VirtualDeveloper.VirtualDeveloper_Name.ToLower() == "programming" || t.VirtualDeveloper.VirtualDeveloper_Name.ToLower() == "bug fixing" || (virtualDevs != null && virtualDevs.Contains(t.VirtualDeveloper_id)))).Sum(a => a.WorkHours.TotalHours);
                                double totalOtherHours = timeSheetList.Where(t => t.VirtualDeveloper != null && t.VirtualDeveloper.VirtualDeveloper_Name != null && (t.VirtualDeveloper.VirtualDeveloper_Name.ToLower() != "programming" && t.VirtualDeveloper.VirtualDeveloper_Name.ToLower() != "bug fixing" && (virtualDevs != null && !virtualDevs.Contains(t.VirtualDeveloper_id)))).Sum(a => a.WorkHours.TotalHours);
                                modelResponse.TotalDevHours = Math.Round(totalDevHours, 2).ToString();
                                modelResponse.TotalOtherHours = Math.Round(totalOtherHours, 2).ToString();
                                response.Data = modelResponse;
                                response.Status = true;
                                response.Code = HttpStatusCode.OK;
                            }
                            else {
                                //response.Data = modelResponse;
                                response.Status = true;
                                response.Message = "No time-sheet data found";
                                response.Code = HttpStatusCode.OK;

                            }
                        }
                        else {
                            response.Message = "CRM project id is wrong. ";
                            response.Status = false;
                            response.Code = HttpStatusCode.NotFound;
                        }
                    }
                    else {
                        response.Status = false;
                        response.Code = HttpStatusCode.BadRequest;
                        response.Data = null;
                        response.Errors = new string[] { "Authentication failed." };
                    }
                }
                else {
                    response.Status = false;
                    response.Code = HttpStatusCode.BadRequest;
                    response.Errors = new string[] { "Request parameters are not in correct format!" };
                }
            }
            catch (Exception ex) {
                response.Status = false;
                response.Code = HttpStatusCode.InternalServerError;
                response.Errors = new string[] { (ex.InnerException ?? ex).Message };
            }
            return response;
        }

        #endregion
    }
}