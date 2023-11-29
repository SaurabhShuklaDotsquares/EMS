using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EMS.API.DAL;
using EMS.API.LIBS;
using EMS.API.Model;
using EMS.Data;
using EMS.Repo;
using EMS.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : BaseApiController
    {
        private IUserLoginService userLoginService;
        private ICRMData crmDataService;
        private IManageLog serviceManageLog;
        private IProjectService projectService;

        public ProjectController(IApiKeyService _apiKeyService, IUserLoginService _userLoginService,
        ICRMData _crmDataService, IManageLog _serviceManageLog, IProjectService _projectService) : base(_apiKeyService)
        {
            userLoginService = _userLoginService;
            crmDataService = _crmDataService;
            serviceManageLog = _serviceManageLog;
            projectService = _projectService;
        }

        #region ProjectUpdateInfo
        // ========== Input JSON request of ProjectUpdateInfo method  ========
        //{"project_id":"997","project_name":"project_name_997_new","tm_email":"nikhil.agarwal@dotsquares.com","pm_email":"giriraj.vyas","modelname":"Developer/Designer only","project_status":"Complete","start_date":"2017-09-18","estimate_time":"10","bill_team":"India","client_name":"forename617 surname617","developer":"0","primary_technology":"ajax, ajax, ajax","other_contact":[{"email":"rahul.verma@dotsquares.com"},{"email":"giriraj.vyas@dotsquares.com"}],"dev_detail":[{
        //"virtual":{"username":"ajay","email":"ajay.mani@dotsquares.com"},
        //"actual":{"username":"test","email":"babulal.sharma@dotsquares.com"}}]}        
        [Route("~/Project/ProjectUpdateInfo")]
        [HttpPost]
        public ResponseModel<string> ProjectUpdateInfo(ProjectInfoReqModel model)
        {
            
            ResponseModel<string> response = new ResponseModel<string>(); 
            

            try
            {
                if (model != null)
                {
                    serviceManageLog.WriteLogFile("ProjectLogFile",$"============================== CRM request to add/update Project info of CRMId ({ model.project_id }) at { DateTime.Now } ============================== {Environment.NewLine} Data : {JsonConvert.SerializeObject(model)} {Environment.NewLine}");
                }
                else
                {


                }
                // Request Authentication
                response = AuthorizeRequest();

                if (response.Status)
                {
                    if (model != null)
                    {

                        if (Convert.ToInt32(model.project_id) <= 0)
                        {
                            response.Status = false;
                            response.Errors = new string[] { "Project Id required." };
                            return response;
                        }
                        else if (string.IsNullOrWhiteSpace(model.tm_email))
                        {
                            response.Status = false;
                            response.Errors = new string[] { "Project TM Email id required." };
                            return response;
                        }
                        else if (string.IsNullOrEmpty(model.project_name))
                        {
                            response.Status = false;
                            response.Errors = new string[] { "Project name is required." };
                            return response;
                        }
                        else if (string.IsNullOrEmpty(model.modelname))
                        {
                            response.Status = false;
                            response.Errors = new string[] { "Model name is required." };
                            return response;
                        }
                        else if (string.IsNullOrEmpty(model.project_status))
                        {
                            response.Status = false;
                            response.Errors = new string[] { "Project status name is required." };
                            return response;
                        }



                        var projectManager = userLoginService.GetLoginDeatilByEmail(model.tm_email);

                        if (projectManager != null)
                        {

                            var returnResponse = crmDataService.UpdateCRMData(projectManager.Uid, model);

                            response.Status = returnResponse.Status;
                            response.Message = returnResponse.Message;

                            serviceManageLog.WriteLogFile("ProjectLogFile", $"Response : { JsonConvert.SerializeObject(response)} {Environment.NewLine}");

                            return response;

                            //response.Status = true;
                            //response.Message = $"Project has been updated successfully for project CRMId ({ model.project_id })";
                            //return response;
                        }
                        else
                        {
                            //serviceManageLog.WriteLogFile("ProjectLogFile", $"===== Project Manager not found ====={Environment.NewLine}");
                            response.Status = false;
                            response.Errors = new string[] { "Project Manager not found." };
                            serviceManageLog.WriteLogFile("ProjectLogFile", $"Response : { JsonConvert.SerializeObject(response)} {Environment.NewLine}");
                            return response;

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                serviceManageLog.WriteLogFile("ProjectLogFile", $"===== Exception occurred {(ex.InnerException ?? ex).Message} ====={Environment.NewLine}");
                response.Status = false;
                response.Code = HttpStatusCode.InternalServerError;
                response.Errors = new string[] { (ex.InnerException ?? ex).Message };
            }

            serviceManageLog.WriteLogFile("ProjectLogFile", $"Response : { JsonConvert.SerializeObject(response)} {Environment.NewLine}");

            return response;
        }


        // {"ProjectCRMId":"7672", "IsCMMI":"0"}
        [Route("~/Project/ActiveCMMI")]
        [HttpPost]
        public ResponseModel<string> ActiveCMMI(ProjectCMMIActive model)
        {
            ResponseModel<string> response = new ResponseModel<string>();

            try
            {
                // Request Authentication
                response = AuthorizeRequest();

                if (response.Status)
                {
                    if (model != null)
                    {
                        var project = projectService.GetProjectByCRMId(model.ProjectCRMId);
                        if (project == null)
                        {
                            response.Status = false;
                            response.Code = HttpStatusCode.NotFound;
                            response.Errors = new string[] { "Project not found for provided CRM Id" };
                        }
                        else
                        {
                            bool isCMMIActive = false;
                            if(model.IsCMMI=="1"||model.IsCMMI=="true")
                            {
                                isCMMIActive = true;
                            }
                            else
                            {
                                if (model.IsCMMI == "0" || model.IsCMMI == "false")
                                {
                                    isCMMIActive = false;
                                }
                                else
                                {
                                    response.Status = false;
                                    response.Code = HttpStatusCode.BadRequest;
                                    response.Errors = new string[] { "Request parameters are not in correct format!" };
                                    return response;
                                }
                                   
                            }
                            project.IsCmmi = isCMMIActive;
                            project.ModifyDate = DateTime.Now;
                            projectService.UpdateStatus(project);
                            response.Status = true;
                            response.Message = string.Format("CMMI flag has been {0} successfully", isCMMIActive ? "Activated" : "Deactivated");
                        }
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

            return response;
        }


        #endregion

        /// <summary>
        ///  Checks whether project exist by EMS Project ID
        /// </summary>
        /// <param name="EMSProjectID">EMS Project Id to find</param>
        /// <returns>Response</returns>
        [Route("~/Project/CheckEMSProjectExist")]
        [HttpPost]
        public ResponseModel<string> CheckEMSProjectExist(ProjectData model)
        {
            ResponseModel<string> response = new ResponseModel<string>();
            try
            {
                // Request Authentication
                response = AuthorizeRequest();

                if (response.Status)
                {
                    var project = projectService.GetProjectById(model.EMSProjectID);
                    
                    if (project == null)
                    {
                        response.Status = false;
                        response.Code = HttpStatusCode.NotFound;
                        response.Errors = new string[] { "Project not found for provided EMS Project ID" };
                    }
                    else
                    {
                        response.Status = true;
                        response.Code = HttpStatusCode.OK;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Code = HttpStatusCode.InternalServerError;
                response.Errors = new string[] { (ex.InnerException ?? ex).Message };
            }
            return response;
        }
    }
}