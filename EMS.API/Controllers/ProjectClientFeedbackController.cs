using EMS.API.Model;
using EMS.Core;
using EMS.Data;
using EMS.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using EMS.API.DAL;
using Newtonsoft.Json;

namespace EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectClientFeedbackController : BaseApiController
    {
        #region "Reference variables"
        private readonly IProjectService projectService;
        private readonly IProjectClientFeedbackService projectClientFeedbackService;
        private readonly IManageLog serviceManageLog;
        #endregion
        public ProjectClientFeedbackController(IApiKeyService _apiKeyService, IProjectService _projectService,
            IProjectClientFeedbackService _projectClientFeedbackService, IManageLog _serviceManageLog) : base(_apiKeyService)
        {
            projectService = _projectService;
            projectClientFeedbackService = _projectClientFeedbackService;
            serviceManageLog = _serviceManageLog;
        }
        #region "Add update Project Client Feedback"
        //= ========== Input JSON request of AddEditClientFeedback method  ========
        //[{"crm_feedback_id":"23","project_id":"11750","status_id":"5","comment":"","website_name":"SalesForce Implementation",
        //"web_url":"https:\/\/girvanwaugh.my.salesforce.com\/","company_name":"InsightBSA","client_name":"Stephen Harmer ",
        //"business_description":"Building fit out and construction company",
        //"project_scope":"SalesForce implementation development support to adjust the sales process steps and provide integration with Outlook functionality.",
        //"meet_requirements":"Development resource very responsive and distance management worked very well particularly with the time difference that meant progress was made overnight. ",
        //"value_about_dotsquares":"Good understanding of the requirements and system. Easy to deal with and good value development resource.","created":"14-03-2019"}]
        [Route("~/ProjectClientFeedback/AddEditClientFeedback")]
        [HttpPost]
        public ResponseModel<string> AddEditClientFeedback(List<ProjectClientFeedbackModel> models)
        {

            ResponseModel<string> response = new ResponseModel<string>();

            try
            {
                if (models != null)
                {
                    serviceManageLog.WriteLogFile("ProjectClientFeedbackLogfile", $"===== request to add/update Project Client feedback at { DateTime.Now } ====={Environment.NewLine}Data : {JsonConvert.SerializeObject(models)}{Environment.NewLine}");
                }
                response = AuthorizeRequest();

                if (response.Status)
                {

                    if (models != null)
                    {
                        Project project = null;
                        StringBuilder message = new StringBuilder();
                        int feedbackSuccessCount = 0;
                        int errorCount = 0;
                        int index = 0;

                        foreach (var model in models)
                        {
                            #region "check validation"

                            index++;
                            int projectId = 0;
                            int crmFeedbackId = 0;
                          
                            try
                            {
                                crmFeedbackId = Int32.Parse(model.crm_feedback_id);
                            }
                            catch
                            {
                                errorCount++;
                                message.Append($"on Record[{index}] CRM Feedback ID: {model.crm_feedback_id} Feedback Id is required and should be in valid numeric format, ");
                                continue;
                            }
                            try
                            {
                                projectId = Int32.Parse(model.crm_project_id);
                            }
                            catch
                            {
                                errorCount++;
                                message.Append($"on Record[{index}] CRM Project Id: {model.crm_project_id} CRM Project Id is required and should be in valid numeric format, ");
                                continue;
                            }

                             ProjectClientFeedback projectClientFeedback = new ProjectClientFeedback();




                            if (crmFeedbackId > 0)
                            {
                                var feedback = projectClientFeedbackService.GetProjectClientFeedback(crmFeedbackId);
                                if (feedback != null && feedback.Id > 0)
                                {
                                    projectClientFeedback = feedback; // update case
                                    projectClientFeedback.CrmfeedbackId = feedback.CrmfeedbackId;
                                    projectClientFeedback.ModifyDate = DateTime.Now;
                                }
                                else
                                {
                                    projectClientFeedback.CrmfeedbackId = crmFeedbackId;
                                    projectClientFeedback.AddDate = DateTime.Now;
                                    projectClientFeedback.ModifyDate = DateTime.Now;
                                }
                            }
                            else
                            {
                                errorCount++;
                                message.Append($"on Record[{index}] CRM Feedback ID: {model.crm_feedback_id} is required and should be in valid numeric format, ");
                                continue;
                            }
                            if (!string.IsNullOrWhiteSpace(model.created))
                            {
                                try
                                {

                                    projectClientFeedback.CommentDate = model.created.ToDateTimeDDMMYYYY();

                                }
                                catch (Exception ex)
                                {
                                    message.Append($"on Record[{index}] Created date: {model.created} invalid date format(should be in dd-MM-yyyy), ");
                                    errorCount++;
                                    continue;
                                }
                            }
                            else
                            {
                                message.Append($"on Record[{index}] Created date: {model.created} invalid date format, ");
                                errorCount++;
                                continue;
                            }
                            if (projectId > 0)
                            {
                                project = projectService.GetProjectByCRMId(projectId);
                                if (project != null && project.ProjectId > 0)
                                {
                                    projectClientFeedback.ProjectId = project.ProjectId;
                                }
                                else
                                {
                                    errorCount++;
                                    message.Append($"on Record[{index}] Project not found in system according to CRM Id {model.crm_project_id}, ");
                                    continue;
                                }
                            }
                            else
                            {
                                errorCount++;
                                message.Append($"on Record[{index}] CRM Project Id: {model.crm_project_id} should be in valid numeric format like, ");
                                continue;

                            }

                            #endregion

                            projectClientFeedback.Status = model.status_id;
                            projectClientFeedback.Comment = model.comment;
                            projectClientFeedback.WebsiteName = model.website_name;
                            projectClientFeedback.WebUrl = model.web_url;
                            projectClientFeedback.CompanyName = model.company_name;
                            projectClientFeedback.ClientName = model.client_name;
                            projectClientFeedback.BusinessDescription = model.business_description;
                            projectClientFeedback.ProjectScope = model.project_scope;
                            projectClientFeedback.MeetRequirements = model.meet_requirements;
                            projectClientFeedback.ValueAboutDotsquares = model.value_about_dotsquares;

                            try
                            {
                                bool updateFeedback = projectClientFeedback.Id > 0;
                                var result = projectClientFeedbackService.Save(projectClientFeedback);
                                if (result.Id > 0)
                                {
                                    serviceManageLog.WriteLogFile("ProjectClientFeedbackLogfile", $"===== Project Client feedback {(updateFeedback ? "updated" : "added")} for CRM Feedback ID{ model.crm_feedback_id } ====={Environment.NewLine}");
                                }
                                feedbackSuccessCount++;
                            }
                            catch
                            {
                                errorCount++;
                                message.Append($"on Record[{index}]CRM Feedback ID: {model.crm_feedback_id} Unable to add Project Client Feedback, ");
                                continue;
                            }
                        }


                        if (models.Count > 0 && models.Count == feedbackSuccessCount)
                        {
                            response.Status = true;
                            response.Code = HttpStatusCode.OK;
                            response.Message = "Project client feedbacks updated successfully";
                            serviceManageLog.WriteLogFile("ProjectClientFeedbackLogfile", $"{message.ToString().Trim().TrimEnd(',')} ====={Environment.NewLine}");

                        }
                        else if (models.Count > 0 && errorCount == models.Count)
                        {
                            response.Status = false;
                            response.Code = HttpStatusCode.BadRequest;
                            message.Insert(0, "Project client feedback request found errors.");
                            response.Errors = new string[] { message.ToString().Trim().TrimEnd(',') };
                            serviceManageLog.WriteLogFile("ProjectClientFeedbackLogfile", $"{message.ToString().Trim().TrimEnd(',')} ====={Environment.NewLine}");
                        }
                        else if (models.Count > 0 && errorCount > 0)
                        {
                            response.Status = false;
                            response.Code = HttpStatusCode.BadRequest;
                            message.Insert(0, "Project client feedbacks updated but with some errors!! ");
                            response.Errors = new string[] { message.ToString().Trim().TrimEnd(',') };
                            serviceManageLog.WriteLogFile("ProjectClientFeedbackLogfile", $"{message.ToString().Trim().TrimEnd(',')} ====={Environment.NewLine}");
                        }

                    }
                    else
                    {
                        response.Status = false;
                        response.Code = HttpStatusCode.BadRequest;
                        response.Errors = new string[] { "Request parameters are not in correct format!" };
                        serviceManageLog.WriteLogFile("ProjectClientFeedbackLogfile", $"Request parameters are not in correct format! ====={Environment.NewLine}");
                    }

                }
                else
                {
                    response.Status = false;
                    response.Code = HttpStatusCode.BadRequest;
                    response.Data = null;
                    response.Errors = new string[] { "Authentication failed." };
                    serviceManageLog.WriteLogFile("ProjectClientFeedbackLogfile", $"Authentication failed. ====={Environment.NewLine}");
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Code = HttpStatusCode.InternalServerError;
                response.Errors = new string[] { (ex.InnerException ?? ex).Message };
                serviceManageLog.WriteLogFile("ProjectClientFeedbackLogfile", $"{(ex.InnerException ?? ex).Message} ====={Environment.NewLine}");
            }


            return response;
        }
        #endregion
    }
}