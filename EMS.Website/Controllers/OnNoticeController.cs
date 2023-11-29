using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Dto.EmployeeFeedback;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.Modals;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class OnNoticeController : BaseController
    {
        private readonly IUserLoginService userLoginService;
        private readonly IDepartmentService departmentService;
        private readonly IFeedbackService feedbackservice;


        public OnNoticeController(IUserLoginService _userLoginService, IDepartmentService _departmentService, IFeedbackService _feedbackservice)
        {
            userLoginService = _userLoginService;
            departmentService = _departmentService;
            feedbackservice = _feedbackservice;
        }
        [HttpGet]
        [CustomActionAuthorization]
        public IActionResult Index()
        {
            //if (CurrentUser.RoleId == (int)Enums.UserRoles.Director || (CurrentUser.RoleId == (int)Enums.UserRoles.OP && CurrentUser.JobTitle == "HR"))
            //{
                OnNoticeIndexDto model = new OnNoticeIndexDto();
                model.PMList = userLoginService.GetPMUsers(true).Select(u => new SelectListItem { Value = u.Uid.ToString(), Text = u.Name?.ToTitleCase() }).ToList();
                model.DepartmentList = departmentService.GetActiveDepartments().OrderBy(d => d.Name).Select(d => new SelectListItem { Value = d.DeptId.ToString(), Text = d.Name?.ToTitleCase() }).ToList();
                return View(model);
            //}
            //else
            //{
            //    return AccessDenied();
            //}
        }
        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult Index(IDataTablesRequest request, OnNoticeFilter onNoticeFilter)
        {
            var pagingServices = new PagingService<UserLogin>(request.Start, request.Length);
            var expr = PredicateBuilder.True<UserLogin>().And(u => u.IsResigned);
            if (onNoticeFilter.pmId.HasValue)
            {
                expr = expr.And(u => u.PMUid == onNoticeFilter.pmId.Value);
            }

            if (onNoticeFilter.deptId.HasValue)
            {
                expr = expr.And(u => u.Department.DeptId == onNoticeFilter.deptId.Value);
            }

            if (!string.IsNullOrWhiteSpace(onNoticeFilter.txtEmployee))
            {
                expr = expr.And(u => u.Name.Trim().ToLower().Contains(onNoticeFilter.txtEmployee.Trim().ToLower()) ||
                u.EmpCode.Trim().ToLower().Contains(onNoticeFilter.txtEmployee.Trim().ToLower()));
            }

            if (onNoticeFilter.IsVoluntaryExit.HasValue)
            {
                expr = expr.And(u => u.UserExitProcess.FirstOrDefault()!=null && 
                u.UserExitProcess.FirstOrDefault().IsVoluntaryExit== onNoticeFilter.IsVoluntaryExit);
            }

            //DateTime? dateFrom = onNoticeFilter.dateFrom.ToDateTime("dd/MM/yyyy");
            //DateTime? dateTo = onNoticeFilter.dateTo.ToDateTime("dd/MM/yyyy");

            //if (dateFrom.HasValue && dateTo.HasValue)
            //{
            //    expr = expr.And(u => u.JoinedDate >= dateFrom && u.JoinedDate <= dateTo);
            //}

            //else if (!string.IsNullOrWhiteSpace(onNoticeFilter.dateFrom))
            //{
            //    expr = expr.And(u => u.JoinedDate >= dateFrom);
            //}
            //else if (!string.IsNullOrWhiteSpace(onNoticeFilter.dateTo))
            //{
            //    expr = expr.And(u => u.JoinedDate >= dateTo);
            //}

            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "userName":
                            return o.OrderByColumn(item, c => c.Name).ThenBy(c => c.EmpCode);
                        case "designation":
                            return o.OrderByColumn(item, c => c.Role.RoleName);
                        case "Department":
                            return o.OrderByColumn(item, c => c.Department.Name);                       

                        default:
                            return o.OrderByColumn(item, c => c.Name);

                    }
                }
                return o.OrderBy(c => c.UserExitProcess.FirstOrDefault().IsExitFormalitiesCompleted !=false).ThenBy(c=>c.RelievingDate);
            };
            int totalCount = 0;

            var userExitProcess = feedbackservice.userexitprocesslist();
            var empFeedbackList = feedbackservice.employeefeedback();
            var response = userLoginService.GetUserReportByPaging(out totalCount, pagingServices);
            var result = (dynamic)null;
            try
            {
                result = DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
                {
                    uid = r.Uid,
                    rowIndex = (index + 1) + (request.Start),
                    userName = string.IsNullOrWhiteSpace(r.EmpCode) ? r.Name.ToTitleCase() : $"{r.Name} ({r.EmpCode})".ToTitleCase(),
                    Designation = (r.Role != null ? r.Role.RoleName.ToTitleCase() : ""),
                    JoiningDate = r.JoinedDate.HasValue ? ((DateTime)r.JoinedDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "",
                    pmName = r.PMUid.HasValue ? userLoginService.GetNameById(((int)r.PMUid)).ToTitleCase() : "",
                    department = r.Department?.Name?.ToTitleCase(),
                    isEmailSent = userExitProcess.Where(u => u.EmpUid == r.Uid).FirstOrDefault() != null ?
                    userExitProcess.Where(u => u.EmpUid == r.Uid).FirstOrDefault().IsFeedbackEmailSent : false,
                    isFeedbackReceived = empFeedbackList.Any(a => a.EmpUid == r.Uid),
                    feedBackId = empFeedbackList.Where(a => a.EmpUid == r.Uid).ToList().Take(1).Select(a => a.Id),
                    isformalitiescomplted = userExitProcess.Where(a=>a.EmpUid == r.Uid).FirstOrDefault() !=null
                    ? userExitProcess.Where(a => a.EmpUid == r.Uid).FirstOrDefault().IsExitFormalitiesCompleted : false,
                    relivedDate = r.RelievingDate.HasValue ? Convert.ToDateTime( r.RelievingDate).ToString("dd/MM/yyyy"):"",
                    resignationDate=r.ResignationDate.HasValue? ((DateTime) r.ResignationDate).ToString("dd/MM/yyyy") :""

                }));
            }
            catch (Exception ex)
            {
                var errorMsg = ex.Message;
                throw;
            }


            return result;

        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult SendEmail(int id)
        {

            try
            {
                if (id <= 0)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = false, Message = "In correct user Id" });
                }
                UserLogin userLogin = userLoginService.GetUserInfoByID(id);

                if (userLogin != null)
                {
                    FlexiMail objSendMail = new FlexiMail();
                    objSendMail.ValueArray = new string[]
                    {
                        userLogin.Name.ToTitleCase(),
                        SiteKey.DomainName+"feedback/add"
                    };

                    objSendMail.Subject = $"Exit Feedback link";
                    objSendMail.MailBodyManualSupply = true;
                    objSendMail.MailBody = objSendMail.GetHtml("NoticeEmailLink.html");

                    objSendMail.To = $"{userLogin.EmailOffice};";
                    //objSendMail.BCC = SiteKey.BCC;
                    objSendMail.From = "hr@dotsquares.com";// SiteKey.From;
                    objSendMail.Send();

                    var userEmailData = feedbackservice.findByUid(id);

                    if (userEmailData == null)
                    {
                        ExitProcessDto model = new ExitProcessDto();

                        model.Uid = id;
                        model.IsFeedbackEmailSent = true;
                        model.IsFeedbackReceived = false;
                        model.IsVoluntaryExit = false;
                        model.IsEligibleForRehire = false;

                        feedbackservice.saveprocess(model);

                    }


                    return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = true, Message = "Exit feedback email sent successfully" });
                }
                return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = false, Message = "Unable to find user" });
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = false, Message = ex.Message });
            }

        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult UserNoc(int user) // Uid
        {
            UserNocDto userNocData = new UserNocDto();

            //List<NocMasterDto> UsrNocList = new List<NocMasterDto>();

            if (user > 0)
            {
                //var NocMaster = feedbackservice.NocMasterList();
                var userNocDetails = feedbackservice.usernocList(user).ToList();
                var userEmailData = feedbackservice.findByUid(user);
                var userData = userLoginService.GetUserInfoByID(user);
                var UserFeedbackData = feedbackservice.employeeFeedbackbyUid(user);
                // user noc details data
                List<NocMasterDto> UsrNocList = feedbackservice.NocMasterList().Select(a => (new NocMasterDto()
                {
                    Id = a.Id,
                    Name = a.Name,
                    IsClear=a.IsClear,
                    Value = a.UserNoc.Where(u=>u.Uid == user).FirstOrDefault()!=null? a.UserNoc.Where(u => u.Uid == user).FirstOrDefault().Value:false
                })).ToList();

                // user feedbackdata
                if (UserFeedbackData != null)
                {
                    userNocData.LFProfile = UserFeedbackData.Lfprofile;
                    userNocData.EmailSkypePassReset = UserFeedbackData.EmailSkypePassReset;
                }
                else
                {
                    userNocData.LFProfile = false;
                    userNocData.EmailSkypePassReset = false;
                }

                // user basic details
                userNocData.Name = userData.Name;
                userNocData.EmpCode = userData.EmpCode;
                userNocData.EmailOffice = userData.EmailOffice;
                userNocData.PMName = (userData.Pmu != null ? '(' + userData.Pmu.Name + ')' : "");// GetPMName(Convert.ToInt32(userData.PMUid));
                userNocData.Department = userData.Department.Name;
                userNocData.Designation = userData.JobTitle;
                userNocData.ResignationDate = userData.ResignationDate;
                userNocData.RelieveDate = userData.RelievingDate;

                // user email send data
                userNocData.UserNocList = UsrNocList;
                userNocData.Uid = user;
                if (userEmailData != null)
                {
                    userNocData.Id = userEmailData.Id;
                    userNocData.IsFeedbackEmailSent = userEmailData.IsFeedbackEmailSent;
                    userNocData.IsFeedbackReceived = userEmailData.IsFeedbackReceived;
                    userNocData.IsVoluntaryExit = userEmailData.IsVoluntaryExit;
                    userNocData.VoluntaryComment = userEmailData.VoluntaryComment;
                    userNocData.IsEligibleForRehire = userEmailData.IsEligibleForRehire;
                    userNocData.RehireComment = userEmailData.RehireComment;
                    userNocData.IsIdCardSubmitted = userEmailData.IsIdCardSubmitted ?? false;
                    userNocData.ReleaseDocPrepared = userEmailData.ReleaseDocPrepared??false;
                    userNocData.IsExitFormalitiesCompleted = userEmailData.IsExitFormalitiesCompleted ?? false;

                }
                else {                    
                    userNocData.IsFeedbackEmailSent = false;
                    userNocData.IsFeedbackReceived = false;
                    userNocData.IsVoluntaryExit = false;
                    userNocData.IsEligibleForRehire = false;
                    userNocData.IsIdCardSubmitted = false;
                    userNocData.ReleaseDocPrepared = false;
                    userNocData.IsExitFormalitiesCompleted = false;
                }
            }

            return View(userNocData);
        }

        public string GetPMName(int PmUid)
        {
            var pmName = string.Empty;
            var userdata = userLoginService.GetUserInfoByID(PmUid);
            if (userdata != null)
            {
                pmName = userdata.Name;
            }
            return pmName;
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult UserNoc(UserNocDto model)
        {
            try
            {
                //UserExitProcess
                ExitProcessDto exitProcessDto = new ExitProcessDto();

                exitProcessDto.Id = model.Id;
                exitProcessDto.Uid = model.Uid;
                exitProcessDto.IsFeedbackEmailSent = Convert.ToBoolean(model.IsFeedbackEmailSent);
                exitProcessDto.IsFeedbackReceived = Convert.ToBoolean(model.IsFeedbackReceived);
                exitProcessDto.IsVoluntaryExit = Convert.ToBoolean(model.IsVoluntaryExit);
                exitProcessDto.VoluntaryComment = model.VoluntaryComment;
                exitProcessDto.IsEligibleForRehire = Convert.ToBoolean(model.IsEligibleForRehire);
                exitProcessDto.RehireComment = model.RehireComment;
                exitProcessDto.IsIdCardSubmitted = Convert.ToBoolean(model.IsIdCardSubmitted);
                exitProcessDto.ReleaseDocPrepared = Convert.ToBoolean(model.ReleaseDocPrepared);
                exitProcessDto.IsExitFormalitiesCompleted = Convert.ToBoolean(model.IsExitFormalitiesCompleted);

                feedbackservice.updateExitprocess(exitProcessDto);

                //EmployeeFeedback
                FeedbackDto entity = new FeedbackDto();

                entity.Uid = model.Uid;
                entity.LFProfile = model.LFProfile;
                entity.EmailSkypePassReset = model.EmailSkypePassReset;

                feedbackservice.updateEmpFeedback(entity);

                //UserNoc
                if (model.UserNocList.Count > 0)
                {
                    for (int i = 0; i < model.UserNocList.Count; i++)
                    {
                        NocDetailsDto nocentity = new NocDetailsDto();

                        nocentity.Uid = model.Uid;
                        nocentity.NocId = model.UserNocList[i].Id;
                        nocentity.Value = Convert.ToBoolean(model.UserNocList[i].Value);

                        feedbackservice.saveNoc(nocentity);
                    }
                }

                UpdateAbscondToHrmAPI(model);


                if (model.Id > 0)
                {
                    ShowSuccessMessage("Success", "User NOC details successfully updated", false);
                }
                else {
                    ShowSuccessMessage("Success", "User NOC details successfully Saved", false);
                }                
            }
            catch (Exception ex)
            {
                var errorMsg = ex.Message;
                ShowErrorMessage("Error", "ex.Message", false);
            }
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Updates abscond user to HRM
        /// </summary>
        /// <param name="model">noc model</param>
        /// <returns>whether record updated or not</returns>
        private bool UpdateAbscondToHrmAPI(UserNocDto model)
        {
            try
            {
                var userData = userLoginService.GetUserInfoByID(model.Uid);

                string DataJson = JsonConvert.SerializeObject(new
                {
                    ActionType= "SetAbscond",
                    HrmId = userData.HRMId,
                    EmsUserId = userData.Uid,
                    PMEmailId = userData.PMUid.HasValue ? userLoginService.GetPmEmailId((int)userData.PMUid) : "",
                    EmailOffice = userData.EmailOffice,
                    Description = model.RehireComment,
                    SetAbscondStatus = Convert.ToBoolean(model.IsEligibleForRehire) ? "0" : "1"
                });

                var httpWebRequest = WebRequest.CreateHttp(SiteKey.HrmAbscondServiceURL);
                httpWebRequest.Headers.Add("Hrmapikey", SiteKey.HrmApiKey);
                httpWebRequest.Headers.Add("Hrmapipassword", SiteKey.HrmApiPassword);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Accept = "application/json; charset=utf-8";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(DataJson);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var response = (HttpWebResponse)(httpWebRequest.GetResponse());
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}