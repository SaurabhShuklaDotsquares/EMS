using System;
using System.IO;
using System.Linq;
using System.Net;
using EMS.API.DAL;
using EMS.API.Model;
using EMS.Core;
using EMS.Data;
using EMS.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectClosureController : BaseApiController
    {
        private readonly IUserLoginService userLoginService;
        private readonly IProjectService projectService;
        private readonly IProjectClosureService projectClosureService;
        private readonly IManageLog serviceManageLog;

        public ProjectClosureController(IApiKeyService _apiKeyService, IUserLoginService _userLoginService,
            IProjectService _projectService, IProjectClosureService _projectClosureService, IManageLog _serviceManageLog) : base(_apiKeyService)
        {
            userLoginService = _userLoginService;
            projectService = _projectService;
            projectClosureService = _projectClosureService;
            serviceManageLog = _serviceManageLog;

        }

        #region AddClosure
        // ========== Input JSON request of AddClosure method  ========
        //{"crmid":"1494","pmemail":"rahul.verma@dotsquares.com","status":"3","oldstatus":"2","reason":"","projecturl":"","dateofclosing":"","startdate":"22\/05\/2019","enddate":"24\/05\/2019","estimatedays":"3","invoicedays":null,
        //"buckethours":0,"istimematerial":"0","ba":{"email":"ashutosh.panwar","Name":"Ashutosh panwar"},"developers":[{"email":"giriraj.vyas@dotsquares.com","Name":"Giriraj Vyas"}],"ClientBadge":""}

        [Route("~/ProjectClosure/AddClosure")]
        [HttpPost]
        public ResponseModel<string> AddClosure(ProjectClosureModel model)
        {
            ResponseModel<string> response = new ResponseModel<string>();

            try
            {
                if (model != null)
                {
                    serviceManageLog.WriteLogFile("Logfile", $"{Environment.NewLine}===== Add Project Closure Requested for CRM ID { model.CRMId } at { DateTime.Now } ====={Environment.NewLine}{Environment.NewLine}Data : {JsonConvert.SerializeObject(model)}{Environment.NewLine}");
                }

                // Request Authentication
                response = AuthorizeRequest();

                if (response.Status)
                {
                    if (model != null)
                    {
                        Project project = null;
                        UserLogin pmUser = null;

                        ProjectClosure closure = new ProjectClosure();
                        DateTime dateOfClosing = model.DateOfClosing.ToDateTime("dd/MM/yyyy") ?? DateTime.Today;

                        #region Check Validations

                        if (model.CRMId <= 0)
                        {
                            response.Status = false;
                            response.Errors = new string[] { "CRM Id required." };
                            LogResponseForClosure(response);
                            return response;
                        }
                        else if (string.IsNullOrWhiteSpace(model.PMEmail))
                        {
                            response.Status = false;
                            response.Errors = new string[] { "Project PM Email id required." };
                            LogResponseForClosure(response);
                            return response;
                        }
                        else if (!Enum.IsDefined(typeof(Enums.APICRMStatus), model.Status))
                        {
                            response.Status = false;
                            response.Errors = new string[] { "Invalid Project Status." };
                            LogResponseForClosure(response);
                            return response;
                        }
                        else if (model.OldStatus.HasValue && !Enum.IsDefined(typeof(Enums.APICRMStatus), model.OldStatus))
                        {
                            response.Status = false;
                            response.Errors = new string[] { "Invalid Project Old Status." };
                            LogResponseForClosure(response);
                            return response;
                        }

                        pmUser = userLoginService.GetLoginDeatilByEmail(model.PMEmail.Trim().ToLower());

                        if (pmUser == null)
                        {
                            response.Message = "PM User not found.";
                            response.Status = false;
                            LogResponseForClosure(response);
                            return response;
                        }

                        project = projectService.GetProjectByCRMId(model.CRMId, pmUser.Uid);

                        if (project == null)
                        {
                            response.Status = false;
                            response.Errors = new string[] { "Project not found." };
                            LogResponseForClosure(response);
                            return response;
                        }
                        else if (project.ProjectClosures.Any(x => x.DateofClosing == dateOfClosing && x.CRMStatus == MapCRMStatus(model.Status) && !x.CRMUpdated))
                        {
                            response.Message = $"Project : {project.Name} already added in this Process.";
                            response.Status = false;
                            LogResponseForClosure(response);
                            return response;
                        }

                        // If request come from Hold to Complete and Complete to Hold status through CRM then not require to check the project existence in EMS and only send success response but update the project status.
                        if (((Enums.APICRMStatus)model.Status == Enums.APICRMStatus.OnHold && (Enums.APICRMStatus)model.OldStatus == Enums.APICRMStatus.Complete) 
                            || ((Enums.APICRMStatus)model.Status == Enums.APICRMStatus.Complete && (Enums.APICRMStatus)model.OldStatus == Enums.APICRMStatus.OnHold)
                            || ((Enums.APICRMStatus)model.Status == Enums.APICRMStatus.PendingPaymentCompleted && (Enums.APICRMStatus)model.OldStatus == Enums.APICRMStatus.OnHold))
                        {

                            project.Status = Common.GetEMSProjectStatusFromAPICRMEnum(((int)model.Status).ToString());

                            var result = projectService.SaveProjectEntity(project);

                            response.Status = true;
                            response.Errors = new string[] { "Project status changed OnHold to Complete and vice-versa in CRM." };
                            LogResponseForClosure(response);
                            return response;
                        }



                        #endregion

                        #region Map fields and Save

                        if (model.Status == Enums.APICRMStatus.Running)
                        {
                            if (project.ProjectClosures.Count > 0) // Project Hold/Complete to Running and closure should be in database
                            {
                                closure = project.ProjectClosures.OrderByDescending(x => x.Id).FirstOrDefault();
                                // Update Status only, for last Project closure to get Converted Projects Report
                                closure.Status = (int)Enums.CloserType.Converted;
                                closure.Modified = DateTime.Now;

                                // Need to update project status in EMS DB -----------
                                closure.Project.Status = Common.GetEMSProjectStatusFromAPICRMEnum(((int)model.Status).ToString());
                                closure.CRMUpdated = true; // For auto approving CRMUpdated is set true

                            }
                            else   // Project Hold/Complete to Running but not found closure report
                            {
                                project.Status = Common.GetEMSProjectStatusFromAPICRMEnum(((int)model.Status).ToString());

                                var result = projectService.SaveProjectEntity(project);

                                response.Message = $"No closure found for Project : {project.Name} to mark as converted.";
                                // Set Status = true because there may be no closures for old projects and
                                // no need to add closure for Running project
                                response.Status = false;
                                LogResponseForClosure(response);
                                return response;
                            }
                        }
                        //else if (project.ProjectClosures.Count > 0 && model.OldStatus > Enums.APICRMStatus.Running)
                        //{
                        //    // Update last project closure

                        //    closure = project.ProjectClosures.OrderByDescending(x => x.Id).FirstOrDefault();
                        //    closure.CRMStatus = MapCRMStatus(model.Status);
                        //    closure.OldCrmstatus = MapCRMStatus(model.OldStatus);
                        //    closure.Modified = DateTime.Now;

                        //    // Need to update project status in EMS DB -----------
                        //    closure.Project.Status = Common.GetEMSProjectStatusFromAPICRMEnum(closure.CRMStatus.ToString());

                        //    if (!closure.CRMUpdated)
                        //    {
                        //        closure.Reason = model.Reason;
                        //        closure.ProjectLiveUrl = model.ProjectURL;
                        //        closure.EstimateDays = model.EstimateDays;
                        //        closure.InvoiceDays = model.InvoiceDays;
                        //        closure.IsTimeMaterial = model.IsTimeMaterial=="1"?true:false;
                        //        closure.Country = project.BillingTeam;
                        //        closure.BucketHours = model.BucketHours > 0.0f ? model.BucketHours.Value : (double?)null;
                        //    }
                        //}
                        else if (model.Status == Enums.APICRMStatus.Complete || model.Status == Enums.APICRMStatus.OnHold || model.Status == Enums.APICRMStatus.PendingPaymentCompleted)   // Project Running to Hold/Complete
                        {
                            // Add new Project Closure
                            closure.ProjectID = project.ProjectId;
                            closure.PMID = pmUser.Uid;
                            closure.Reason = model.Reason;
                            closure.CRMStatus = MapCRMStatus(model.Status);  // Map CRM status to closure report form CRM status dropdown value
                            closure.OldCrmstatus = MapCRMStatus(model.OldStatus); // Map CRM status to closure report form CRM status dropdown value
                            closure.ClientQuality = (int)Enums.ClientQualtiy.Average;
                            closure.Status = (int)Enums.CloserType.Pending;
                            closure.DateofClosing = dateOfClosing;
                            closure.StartDate = model.StartDate.ToDateTime("dd/MM/yyyy");
                            closure.EndDate = model.EndDate.ToDateTime("dd/MM/yyyy");
                            closure.ProjectLiveUrl = model.ProjectURL;
                            closure.EstimateDays = model.EstimateDays;
                            closure.InvoiceDays = model.InvoiceDays;
                            closure.IsTimeMaterial = model.IsTimeMaterial == "1" ? true : false;
                            closure.Country = project.BillingTeam;
                            closure.BucketHours = model.BucketHours > 0.0f ? model.BucketHours.Value : (double?)null;

                            closure.Modified = DateTime.Now;
                            closure.Created = DateTime.Now;

                            if (model.Developers != null && model.Developers.Count > 0)
                            {
                                model.Developers = model.Developers.FindAll(x => !string.IsNullOrWhiteSpace(x.Email));

                                if (model.Developers.Count > 0)
                                {
                                    var leadDevUser = userLoginService.GetLoginDeatilByEmail(model.Developers.First().Email.Trim().ToLower());

                                    if (leadDevUser != null && (leadDevUser.PMUid == pmUser.Uid || leadDevUser.Uid == pmUser.Uid))
                                    {
                                        closure.Uid_Dev = leadDevUser.Uid;
                                        closure.OtherActualDeveloper = model.Developers.Count > 1 ? string.Join(", ", model.Developers.Skip(1).Select(x => x.Name)) : "";
                                    }
                                    else
                                    {
                                        closure.OtherActualDeveloper = string.Join(", ", model.Developers.Select(x => x.Name));
                                    }
                                }
                            }

                            if (model.BA != null && model.BA.Email.HasValue())
                            {
                                var baUser = userLoginService.GetLoginDeatilByEmail(model.BA.Email.Trim().ToLower());

                                if (baUser != null && (baUser.PMUid == pmUser.Uid || baUser.Uid == pmUser.Uid))
                                {
                                    closure.Uid_BA = baUser.Uid;
                                }
                            }
                        }

                        if(!string.IsNullOrWhiteSpace(model.ClientBadge))
                        {
                            closure.ClientBadge = MapClientBadge(model.ClientBadge.ToLower().Trim());
                        }

                        if (closure.CRMStatus > 0)
                        {
                            bool updateClosure = closure.Id > 0;
                            var result = projectClosureService.Save(closure);

                            if (result.Id > 0)
                            {
                                response.Status = true;
                                response.Message = $"Project Closure {(updateClosure ? "updated" : "added")} successfully";

                                //serviceManageLog.WriteLogFile("Logfile", $"===== Project Closure {(updateClosure ? "updated" : "added")} for CRM ID { model.CRMId } ====={Environment.NewLine}");
                            }
                            else
                            {
                                response.Status = false;
                                response.Errors = new string[] { "Unable to add Project Closure" };
                            }
                        }
                        else
                        {
                            response.Status = false;
                            response.Code = HttpStatusCode.OK;
                            response.Errors = new string[] { "No data found to save!" };
                        }

                        #endregion
                    }
                    else
                    {
                        response.Status = false;
                        response.Code = HttpStatusCode.BadRequest;
                        response.Errors = new string[] { "Request parameters are not in correct format!" };
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Code = HttpStatusCode.InternalServerError;
                response.Errors = new string[] { (ex.InnerException ?? ex).Message };
            }
            LogResponseForClosure(response);
            return response;
        }


        private void LogResponseForClosure(ResponseModel<string> response)
        {
            serviceManageLog.WriteLogFile("Logfile", $"Response : {JsonConvert.SerializeObject(response)}{Environment.NewLine}");
        }

        #endregion


        private int? MapClientBadge(string clientBadge) {
            int? retClientbadge = null;
            switch (clientBadge) {
                case "platinum":
                    retClientbadge = (int)Enums.ClientBadge.PLATINUM;
                    break;
                case "gold":
                    retClientbadge = (int)Enums.ClientBadge.GOLD;
                    break;
                case "silver":
                    retClientbadge = (int)Enums.ClientBadge.SILVER;
                    break;
                case "bronze":
                    retClientbadge = (int)Enums.ClientBadge.BRONZE;
                    break;
            }
            return retClientbadge;
        }

        private int? MapCRMStatus(Enums.APICRMStatus? crmStatus)
        {
            int? status = null;
            if (crmStatus.HasValue)
            {
                switch (crmStatus)
                {
                    case Enums.APICRMStatus.Running:
                        status = (int)Enums.CRMStatus.Running;
                        break;
                    case Enums.APICRMStatus.OnHold:
                        status = (int)Enums.CRMStatus.OnHold;
                        break;
                    case Enums.APICRMStatus.Complete:
                        status = (int)Enums.CRMStatus.Completed;
                        break;
                    case Enums.APICRMStatus.OverRun:
                        status = (int)Enums.CRMStatus.OverRun;
                        break;
                    case Enums.APICRMStatus.Remove:
                        status = (int)Enums.CRMStatus.Remove;
                        break;
                    case Enums.APICRMStatus.NotConverted:
                        status = (int)Enums.CRMStatus.NotConverted;
                        break;
                    case Enums.APICRMStatus.NotInitiated:
                        status = (int)Enums.CRMStatus.NotInitiated;
                        break;
                    case Enums.APICRMStatus.PendingPaymentCompleted:
                        status = (int)Enums.CRMStatus.PendingPaymentCompleted;
                        break;
                }
            }
            return status;
        }


    }
}