using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.Controllers;
using EMS.Web.Modals;
using EMS.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Transactions;
using static EMS.Core.Enums;
using System.Net.Http;
using System.Net.Http.Headers;
//using static EMS.Core.MD5Encryption;
using static EMS.Core.Encryption;
using System.Security.Cryptography;
using System.IO.Compression;
using EMS.Website.LIBS;
using EMS.Service.SARAL;
using EMS.Service.SARALDT;

namespace EMS.Website.Controllers
{
    [CustomAuthorization()]
    public class UserController : BaseController
    {

        #region [ Fields ]
        private readonly IUserLoginService userLoginService;
        private readonly ITechnologyService technologyService;
        private readonly IProjectService projectService;
        private readonly IUserActivityService userActivityService;
        private readonly IDepartmentService departmentService;
        private readonly IEmployeeMedicalService medicalDataService;
        private readonly IDomainTypeService domainTypeService;
        private readonly ILibraryDownloadService libraryDownloadService;
        private readonly ITeamHierarchyService teamHierarchyService;
        private readonly IRoleService roleService;
        private readonly ILevDetailsService levDetailsDTPLService;//DTPL
        private readonly ILevDetailsDTService levDetailsDTService;//DT
        string logPath = "D:/local/EMSWebCore/EMS.Website/wwwroot/User_Log/user-log.txt";
        #endregion [ Fields ]

        #region [ Constructor ]
        public UserController(IUserLoginService _userLoginService, IProjectService _projectService, ITechnologyService _technologyService,
             IUserActivityService _userActivityService, IDepartmentService _departmentService, IEmployeeMedicalService _medicalDataService,
             IDomainTypeService _domainTypeService, ILibraryDownloadService _libraryDownloadService, ITeamHierarchyService _teamHierarchyService, IRoleService _roleService,
            ILevDetailsService _levDetailsDTPLService, ILevDetailsDTService _levDetailsDTService)
        {
            userLoginService = _userLoginService;
            technologyService = _technologyService;
            projectService = _projectService;
            userActivityService = _userActivityService;
            departmentService = _departmentService;
            medicalDataService = _medicalDataService;
            domainTypeService = _domainTypeService;
            libraryDownloadService = _libraryDownloadService;
            teamHierarchyService = _teamHierarchyService;
            roleService = _roleService;
            levDetailsDTPLService = _levDetailsDTPLService;
            levDetailsDTService = _levDetailsDTService;

        }
        #endregion [ Constructor ]

        #region [ Manage Profile ]

        /// <summary>
        /// Display User profile, 
        /// </summary>
        /// 
        [CustomActionAuthorization]
        [HttpGet]
        public ActionResult Manageprofile()
        {
            //Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(string.Format("en-GB"));

            Data.UserLogin UserModel = new Data.UserLogin();
            UserProfileDto UserProfile = new UserProfileDto();
            try
            {

                UserModel = userLoginService.GetUserInfoByID(CurrentUser.Uid);
                UserProfile.Uid = UserModel.Uid;
                UserProfile.EmployeeCode = !string.IsNullOrWhiteSpace(UserModel.EmpCode) ? UserModel.EmpCode : "";
                UserProfile.Title = UserModel.Title;
                UserProfile.Name = UserModel.Name;
                UserProfile.JobTitle = !string.IsNullOrWhiteSpace(UserModel.JobTitle) ? UserModel.JobTitle : UserModel.Role.RoleName;
                UserProfile.DOB = UserModel.DOB.ToFormatDateString("dd/MM/yyyy");
                UserProfile.JoinedDate = UserModel.JoinedDate.ToFormatDateString("MMM, dd yyyy");
                UserProfile.EmailOffice = UserModel.EmailOffice;
                UserProfile.EmailPersonal = UserModel.EmailPersonal;
                UserProfile.MobileNumber = UserModel.MobileNumber;
                UserProfile.PhoneNumber = UserModel.PhoneNumber;
                UserProfile.AlternativeNumber = UserModel.AlternativeNumber;
                UserProfile.Address = UserModel.Address;
                UserProfile.SkypeId = UserModel.SkypeId;
                UserProfile.IsInterestedPffaccount = UserModel.IsInterestedPffaccount;
                UserProfile.UANNumber = UserModel.UANNumber;
                UserProfile.MarraigeDate = UserModel.MarraigeDate.ToFormatDateString("dd/MM/yyyy");
                UserProfile.Gender = UserModel.Gender;

                //profile images
                UserProfile.ProfilePicture = UserModel.ProfilePicture;

                // UserProfile.AddDate = (DateTime)UserModel.AddDate;
                UserProfile.AddDate = UserModel.AddDate.HasValue ? UserModel.AddDate.Value : DateTime.Now;
                UserProfile.RoleName = UserModel.Role != null ? UserModel.Role.RoleName : "";
                UserProfile.RoleCategoryName = UserModel.Role.RoleCategory != null ? UserModel.Role.RoleCategory.Name : "";
                UserProfile.DesignationName = UserModel.Designation != null ? UserModel.Designation.Name : "";
                UserProfile.DeptName = UserModel.Department != null ? UserModel.Department.Name : "";
                //UserProfile.GroupName = UserModel.Group.GroupName;
                //UserProfile.DeptName = UserModel.Department.Name;
                UserProfile.OtherTechnology = UserModel.OtherTechnology;
                UserProfile.AadharNumber = UserModel.AadharNumber;
                UserProfile.PanNumber = UserModel.PanNumber;
                UserProfile.PassportNumber = UserModel.PassportNumber;
                UserProfile.OtherTechnology = UserModel.OtherTechnology;
                UserProfile.BloodGroupId = Convert.ToInt32(UserModel.BloodGroupId) == 0 ? 0 : Convert.ToInt32(UserModel.BloodGroupId);
                if (UserModel.PMUid.HasValue)
                {
                    var userPM = userLoginService.GetUserInfoByID(UserModel.PMUid.Value);
                    UserProfile.PMName = userPM != null ? userPM.Name : "";
                }
                UserProfile.TLId = UserModel.TLId.HasValue ? UserModel.TLId.Value : 0;
                //SetTLList(UserModel.PMUid.Value, UserModel.RoleId.Value);
                //}
                UserProfile.ProfileDocumentsList = UserModel.UserDocument.Select(y => new UserProfileDocumentDto()
                {
                    Id = y.Id,
                    DocumentPath = y.DocumentPath,
                    UId = y.Uid.Value
                }).ToList();

                UserProfile.TechnologyList = technologyService.GetTechnologyList().OrderBy(x => x.Title)
                                    .Select(x => new UserTechnologyDto { TechId = x.TechId, TechName = x.Title }).ToList();

                UserProfile.DomainExpert = domainTypeService.GetDomainList().OrderBy(x => x.DomainName)
                                   .Select(x => new DomainExpertDto { DomainId = x.DomainId, DomainName = x.DomainName }).ToList();

                UserProfile.SpecTypeList = WebExtensions.GetList<Enums.TechnologySpecializationType>();

                if (UserModel.User_Tech.Any())
                {
                    UserProfile.TechnologyList.ForEach(t =>
                    {
                        var usrTech = UserModel.User_Tech.FirstOrDefault(ut => ut.TechId == t.TechId);
                        if (usrTech != null)
                        {
                            t.Selected = true;
                            t.SpecTypeId = usrTech.SpecTypeId;
                        }
                    });
                }

                if (UserModel.DomainExperts.Any())
                {
                    UserProfile.DomainExpert.ForEach(t =>
                    {
                        var userDomain = UserModel.DomainExperts.FirstOrDefault(ut => ut.DomainId == t.DomainId);
                        if (userDomain != null)
                        {
                            t.Selected = true;
                            t.DomainId = userDomain.DomainId;
                        }
                    });
                }
                else
                {
                    UserProfile.DomainExpert.ForEach(t =>
                    {
                        var userDomain = UserModel.DomainExperts.FirstOrDefault(ut => ut.DomainId == t.DomainId);
                        if (userDomain != null)
                        {
                            t.Selected = false;
                            t.DomainId = userDomain.DomainId;
                        }
                    });
                }


                List<Data.UserLogin> userTLList = new List<Data.UserLogin>();

                userTLList = userLoginService.GetTLSDUsers((CurrentUser.RoleId == (int)Core.Enums.UserRoles.PM || CurrentUser.RoleId == (int)Core.Enums.UserRoles.PMO) ? CurrentUser.Uid : CurrentUser.PMUid);
                ViewBag.TLList = userTLList;

                IEnumerable<Core.Enums.BloodGroups> actionTypes = Enum.GetValues(typeof(Core.Enums.BloodGroups)).Cast<Core.Enums.BloodGroups>();
                ViewBag.bloodGroup = actionTypes.Select(c => new SelectListItem { Text = c.GetEnumDisplayName(), Value = ((int)c).ToString() }).ToList();





            }
            catch (Exception) { }
            return View(UserProfile);
        }

        /// <summary>
        /// Save User profile
        /// </summary>
        /// <param name="UserModel">user profile data</param>
        /// <param name="Technology">Technology data</param>
        /// <param name="Specialist">Specialist data</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ManageProfile(UserProfileDto UserModel)
        {
            var files = HttpContext.Request.Form.Files;
            Data.UserLogin UserModelDB = null;
            if (CurrentUser != null && CurrentUser.Uid != 0)
            { //Update

                UserModelDB = userLoginService.GetUserInfoByID(CurrentUser.Uid);

                #region UserProfile Update
                bool isSendHrPfEmail = (UserModel.IsInterestedPffaccount.Value != UserModelDB.IsInterestedPffaccount);

                UserModelDB.Title = UserModel.Title;
                UserModelDB.EmpCode = UserModel.EmployeeCode;
                UserModelDB.Gender = UserModel.Gender;
                //profile images
                string selectedProfileName;
                string uniqueFileName = UploadedFile(UserModel, out selectedProfileName);
                UserModelDB.ProfilePicture = uniqueFileName ?? UserModel.ProfilePicture;

                UserModelDB.Name = UserModel.Name;
                UserModelDB.JobTitle = !string.IsNullOrWhiteSpace(UserModel.JobTitle)? UserModel.JobTitle:UserModel.RoleName;
                UserModelDB.DOB = UserModel.DOB.ToDateTime("dd/MM/yyyy");

                UserModelDB.IsInterestedPffaccount = UserModel.IsInterestedPffaccount;
                UserModelDB.UANNumber = UserModel.UANNumber;
                UserModelDB.EmailOffice = UserModel.EmailOffice;
                UserModelDB.EmailPersonal = UserModel.EmailPersonal;
                UserModelDB.MobileNumber = UserModel.MobileNumber;
                UserModelDB.PhoneNumber = UserModel.PhoneNumber;
                UserModelDB.OtherTechnology = UserModel.OtherTechnology;
                UserModelDB.AlternativeNumber = UserModel.AlternativeNumber;
                UserModelDB.Address = UserModel.Address;
                UserModelDB.SkypeId = UserModel.SkypeId;
                UserModelDB.MarraigeDate = UserModel.MarraigeDate.ToDateTime("dd/MM/yyyy");
                UserModelDB.AadharNumber = UserModel.AadharNumber;
                if (CurrentUser.RoleId != (int)Core.Enums.UserRoles.HRBP && CurrentUser.RoleId != (int)Core.Enums.UserRoles.PM && CurrentUser.RoleId != (int)Core.Enums.UserRoles.PMO)
                {
                    UserModelDB.TLId = UserModel.TLId.HasValue ? UserModel.TLId.Value : 0;
                }
                UserModelDB.IP = GeneralMethods.Getip();
                UserModelDB.AddDate = UserModel.AddDate != null ? UserModel.AddDate : DateTime.Now;
                UserModelDB.ModifyDate = DateTime.Now;
                UserModelDB.PassportNumber = UserModel.PassportNumber != null ? UserModel.PassportNumber.ToUpper() : null;
                UserModelDB.PanNumber = UserModel.PanNumber != null ? UserModel.PanNumber.ToUpper() : null;
                UserModelDB.OtherTechnology = UserModel.OtherTechnology;

                if (UserModel.BloodGroupId != 0)
                {
                    UserModelDB.BloodGroupId = UserModel.BloodGroupId;
                }
                else
                {
                    UserModelDB.BloodGroupId = null;
                }


                #endregion


                var isUserTechDeleted = userLoginService.UserTechDeleted(UserModelDB);
                if (isUserTechDeleted)
                {
                    if (UserModelDB.User_Tech != null)
                    {
                        UserModelDB.User_Tech.Clear();
                    }

                    if (UserModel.TechnologyList.Any())
                    {
                        var technologies = UserModel.TechnologyList.Where(x => x.SpecTypeId.HasValue);

                        foreach (var tech in technologies)
                        {
                            UserModelDB.User_Tech.Add(new User_Tech
                            {

                                TechId = tech.TechId,
                                SpecTypeId = tech.SpecTypeId,
                                Uid = CurrentUser.Uid
                            });
                        }
                    }
                }

                var isUserDomainDeleted = userLoginService.UserDomainDeleted(UserModelDB);
                if (isUserDomainDeleted)
                {
                    if (UserModelDB.DomainExperts != null)
                    {
                        UserModelDB.DomainExperts.Clear();
                    }

                    if (UserModel.DomainExpert.Any())
                    {
                        var domains = UserModel.DomainExpert;

                        foreach (var domain in domains)
                        {
                            UserModelDB.DomainExperts.Add(new DomainExperts
                            {

                                DomainId = domain.DomainId,
                                Uid = CurrentUser.Uid
                            });
                        }
                    }
                }
                UserModelDB = BindProfileDocumentEntityData(UserModelDB, selectedProfileName);
                userLoginService.Save(UserModelDB);

                if (UserModel.Uid != 0)
                {
                    UpdateUserHRM(UserModelDB);
                }

                string Message = "Record has been updated successfully.";

                var crmResponse = UpdateDeveloperInfoInCRM(UserModelDB.Uid);

                if (crmResponse == null || !crmResponse.Status)
                {
                    var errorMessage = crmResponse?.Errors != null ? string.Join(", ", crmResponse.Errors) : crmResponse?.Message ?? "Some error while calling CRM API";

                    Message = $"Record has been updated successfully. But unable to update user info on CRM. Error: {errorMessage}";
                }

                if (UserModel.IsInterestedPffaccount.HasValue && UserModel.IsInterestedPffaccount.Value == true && isSendHrPfEmail)
                {

                    var userList = userLoginService.GetUserByRole((int)UserRoles.HRBP).Where(x => x.IsActive == true).Select(y => new SelectListItem
                    {
                        Text = y.Name,
                        Value = y.EmailOffice.ToString(),
                    }).OrderBy(t => t.Text).ToList();

                    var rsEmails = userList.Select(x => x.Value).ToList();

                    var v0 = UserModel.Name;
                    var v1 = UserModel.EmailOffice;
                    var v2 = UserModel.MobileNumber;
                    var v3 = UserModel.DeptName;
                    var v4 = UserModel.RoleName;

                    var ValueArray = new string[] { v0, v1, v2, v3, v4 };
                    FlexiMail objSendMail = new FlexiMail();
                    objSendMail.ValueArray = ValueArray;
                    objSendMail.Subject = "PFF Account Email";
                    objSendMail.MailBodyManualSupply = true;
                    objSendMail.MailBody = objSendMail.GetHtml("PffEmail.html");
                    objSendMail.From = SiteKey.From;

                    if (rsEmails != null && rsEmails.Count > 0)
                    {
                        objSendMail.To = string.Join(",", rsEmails);
                        //objSendMail.To = "manish.tiwari@dotsquares.com";
                        // objSendMail.Send();
                    }
                }

                //ShowSuccessMessage("Success!", "Profile has been updated successfully", false);

                AjaxResponseDto ajaxResponseDto = new AjaxResponseDto
                {
                    Success = true,
                    Message = Message,
                    Data = new { redirectUrl = Url.Action("ManageProfile", "User") }
                };

                return Json(ajaxResponseDto);
            }
            else
            {
                AjaxResponseDto ajaxResponseDto = new AjaxResponseDto
                {
                    Success = false,
                    Message = "Unable to update user profile"
                };

                return Json(ajaxResponseDto);
            }
        }

        private string UploadedFile(UserProfileDto model, out string selectedFileName)
        {
            string uniqueFileName = selectedFileName = null;
            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(ContextProvider.HostEnvironment.WebRootPath, $"Images//profile");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                //prev. file
                string prevpath = Path.Combine(uploadsFolder, model.ProfilePicture ?? "");
                if (System.IO.File.Exists(prevpath))
                {
                    System.IO.File.Delete(prevpath);
                }
                //new file
                selectedFileName = model.ProfileImage.FileName;
                uniqueFileName = model.ProfileImage.FileName.ToUnique();
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        private UserLogin BindProfileDocumentEntityData(UserLogin _userLogin, string selectedProfileName)
        {
            var files = HttpContext.Request.Form.Files;
            _userLogin.UserDocument = _userLogin.UserDocument ?? new List<UserDocument>();

            if (files.Count > 0)
            {
                var imageDirectory = Path.Combine(ContextProvider.HostEnvironment.WebRootPath, $"Images//profile");
                if (!Directory.Exists(imageDirectory))
                    Directory.CreateDirectory(imageDirectory);
                foreach (var item in files)
                {
                    if (item.FileName == selectedProfileName)
                        continue;
                    var ImageName = item.FileName.ToUnique();
                    using (var fileStream = new FileStream(Path.Combine(ContextProvider.HostEnvironment.WebRootPath, $"Images//profile//{ImageName}"), FileMode.Create))
                    {
                        item.CopyTo(fileStream);
                    }
                    UserDocument userDoc = new UserDocument();
                    userDoc.Uid = _userLogin.Uid;
                    userDoc.DocumentPath = ImageName;
                    userDoc.AddedDate = DateTime.Now;
                    _userLogin.UserDocument.Add(userDoc);
                }
            }


            return _userLogin;
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult DeleteDocumentFile(int id)
        {
            try
            {
                var response = userLoginService.DeleteDocumentFile(id);
                return NewtonSoftJsonResult(response);
            }
            catch (Exception)
            {
                return NewtonSoftJsonResult(false);
            }
        }
        #endregion [ Manage Profile ]

        #region Update CRM Service

        [HttpGet]
        public ActionResult UpdateAllUserTechInfoInCRM()
        {
            if (CurrentUser.Uid != SiteKey.AshishTeamPMUId)
            {
                return AccessDenied();
            }

            var activeUserList = userLoginService.GetAllActiveUsersList();


            var model = new UserTechInfoInCRMModel
            {
                Uid = 0,
                data = activeUserList.Select(u => new UserTechInfoInCRM
                {
                    Uid = u.Uid,
                    UserName = u.UserName,
                    EmailOffice = u.EmailOffice,
                    UserPMName = (u.PMUid > 0 ? activeUserList.Where(p => p.Uid == u.PMUid).FirstOrDefault().UserName : "N/A"),
                    HasTechInfo = u.User_Tech.Count > 0,
                    ResultFromCRM = "--"
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateAllUserTechInfoInCRM(int Uid)
        {
            if (CurrentUser.Uid != SiteKey.AshishTeamPMUId)
            {
                return AccessDenied();
            }
            var Message = "--";
            bool Success = false;
            if (Uid > 0)
            {
                var crmResponse = UpdateDeveloperInfoInCRM(Uid, false);
                if (crmResponse == null || !crmResponse.Status)
                {
                    Message = crmResponse?.Errors != null ? string.Join(", ", crmResponse.Errors) : crmResponse?.Message ?? "Some error while calling CRM API";
                    Success = false;
                }
                else
                {
                    Message = "Record has been updated successfully.";
                    Success = true;
                }

            }

            AjaxResponseDto ajaxResponseDto = new AjaxResponseDto
            {
                Data = Uid,
                Success = Success,
                Message = Message
            };

            return Json(ajaxResponseDto);

            //var activeUserList = userLoginService.GetAllActiveUsersList();


            //var model = new UserTechInfoInCRMModel
            //{
            //    Uid = 0,
            //    data = activeUserList.Select(u => new UserTechInfoInCRM
            //    {
            //        Uid = u.Uid,
            //        UserName = u.UserName,
            //        EmailOffice = u.EmailOffice,
            //        UserPMName = (u.PMUid > 0 ? activeUserList.Where(p => p.Uid == u.PMUid).FirstOrDefault().UserName : "N/A"),
            //        HasTechInfo = u.User_Tech.Count > 0,
            //        ResultFromCRM = "--"
            //    }).ToList()
            //};

            //Dictionary<int, string> disResultFromCRM = new Dictionary<int, string>();

            //for (int i = 0; i < activeUserList.Count(); i++)
            //{
            //    var crmResponse = UpdateDeveloperInfoInCRM(activeUserList[i].Uid);
            //    if (crmResponse == null || !crmResponse.Status)
            //    {
            //        var ResultFromCRM = crmResponse?.Errors != null ? string.Join(", ", crmResponse.Errors) : crmResponse?.Message ?? "Some error while calling CRM API";
            //        disResultFromCRM.Add(activeUserList[i].Uid, ResultFromCRM);
            //    }
            //    else
            //    {
            //        disResultFromCRM.Add(activeUserList[i].Uid, "Record has been updated successfully.");
            //    }
            //}

            //var UserTechInfoInCRM = activeUserList.Select(u => new 
            //{
            //    u.Uid,
            //    u.UserName,
            //    u.EmailOffice,
            //    UserPMName = (u.PMUid > 0 ? activeUserList.Where(p => p.Uid == u.PMUid).FirstOrDefault().UserName : "N/A"),
            //    HasTechInfo = u.User_Tech.Count > 0,
            //    ResultFromCRM = disResultFromCRM[u.Uid]
            //});

            //return View(model);
        }


        private ResponseModel<string> UpdateDeveloperInfoInCRM(int Uid, bool writeLog = true)
        {
            var crmResponse = new ResponseModel<string>();
            try
            {
                if (SiteKey.IsLive)
                {
                    string developerInfo = CreateDeveloperInfoJSON(Uid);
                    var request = WebRequest.CreateHttp(SiteKey.UpdateDeveloperInfo);
                    if (writeLog)
                    {
                        WriteLogFile($"===== Update Developer Info Start {DateTime.Now} ======\n\nRequest Parameters : {developerInfo}");
                    }


                    request.Headers.Add("userid", SiteKey.CRMApiUser);
                    request.Headers.Add("password", SiteKey.CRMApiPassword);

                    var data = Encoding.ASCII.GetBytes(developerInfo);

                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = data.Length;

                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    var response = (HttpWebResponse)(request.GetResponse());

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        if (responseData.HasValue())
                        {
                            if (writeLog)
                            {
                                WriteLogFile($"Response : {responseData}");
                            }

                            crmResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseModel<string>>(responseData);
                        }
                        else
                        {
                            if (writeLog)
                            {
                                WriteLogFile($"Response : No response from API");
                            }
                        }
                    }
                    else
                    {
                        if (writeLog)
                        {
                            WriteLogFile($"Error Response : Code = {response.StatusCode} Description = {response.StatusDescription}");
                        }

                        crmResponse.Code = response.StatusCode;
                        crmResponse.Errors = new string[] { response.StatusDescription };
                    }
                }
            }
            catch (Exception ex)
            {
                if (writeLog)
                {
                    WriteLogFile($"Exception Response : {(ex.InnerException ?? ex).Message}");
                }

                crmResponse.Code = HttpStatusCode.InternalServerError;
                crmResponse.Errors = new string[] { (ex.InnerException ?? ex).Message };
            }
            if (writeLog)
            {
                WriteLogFile($"===== Update Developer Info End {DateTime.Now} ======");
            }

            return crmResponse;
        }


        private string CreateDeveloperInfoJSON(int Uid)
        {
            try
            {
                UserLogin UserModel = userLoginService.GetUserInfoByID(Uid);
                UserJsonModelForDeveloperInfo jsonInfo = new UserJsonModelForDeveloperInfo();
                jsonInfo.email_address = UserModel.EmailOffice;
                jsonInfo.hrm_id = (UserModel.HRMId == null ? "0" : Convert.ToString(UserModel.HRMId));
                foreach (var item in UserModel.User_Tech)
                {
                    UserTechnologyInfo info = new UserTechnologyInfo();
                    info.technology_ems_id = item.Technology.TechId.ToString();
                    info.technology_ems_name = item.Technology.Title;
                    info.specialization = ((Enums.TechnologySpecializationType)item.SpecTypeId).GetDescription().ToLower().Trim();
                    jsonInfo.data.Add(info);
                }
                return JsonConvert.SerializeObject(jsonInfo);
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        #endregion


        #region [ Update UserHRM ]
        [NonAction]
        private void UpdateUserHRM(UserLogin user)
        {
            string strLog = string.Empty;
            try
            {
                bool isServiceLive = false;
                if (SiteKey.IsServiceLive != null && SiteKey.IsServiceLive == "1")
                {
                    var ServiceLive = SiteKey.IsServiceLive;
                    isServiceLive = true;
                }

                if (isServiceLive)
                {
                    int gender = user.Gender == "M" ? 1 : 2;
                    string pmMailId = (user.PMUid != null && user.PMUid != 0) ? userLoginService.GetUserInfoByID(user.PMUid.Value).EmailOffice : null;

                    List<string> Technology = new List<string>();
                    foreach (var tech in user.User_Tech)
                    {
                        Technology.Add(technologyService.GetTechnologyById(tech.TechId).Title);
                    }


                    #region "Update User using WebServices"
                    string userJson = JsonConvert.SerializeObject(new
                    {
                        ActionType = "update",
                        HrmId = user.HRMId,
                        EmsUserId = user.Uid,
                        DepartmentCode = user.Department?.Deptcode,
                        PMEmailId = pmMailId,
                        EmailOffice = user.EmailOffice,
                        AadhaarNumber = user.AadharNumber,
                        BloodGroup = user.BloodGroup != null ? user.BloodGroup.Name : "",
                        PanNumber = user.PanNumber,
                        PassportNumber = user.PassportNumber,
                        Gender = gender,
                        DOB = user.DOB.Value.ToString("yyyy-MM-dd"),
                        RoleCode = Enum.GetName(typeof(Enums.UserRoles), user.RoleId),
                        MobileNumber = user.MobileNumber,
                        AlternativeNumber = user.AlternativeNumber,
                        AddressOne = user.Address,
                        AddressTwo = "",
                        MarriageDate = user.MarraigeDate != null ? user.MarraigeDate.Value.ToString("yyyy-MM-dd") : "",
                        EmailPersonal = user.EmailPersonal,
                        SkypeId = user.SkypeId,
                        TechnologyName = Technology
                    });
                    strLog += userJson + "\n";
                    string res = string.Empty;
                    if (SiteKey.HrmApiKey != null && SiteKey.HrmApiPassword != null)
                    {
                        string Hrmapikey = SiteKey.HrmApiKey;
                        string Hrmapipassword = SiteKey.HrmApiPassword;
                        string hrmServiceURL = SiteKey.HrmServiceURL;
                        var httpWebRequest = (HttpWebRequest)WebRequest.Create(hrmServiceURL);
                        httpWebRequest.ContentType = "application/json; charset=utf-8";
                        httpWebRequest.Method = "POST";
                        httpWebRequest.Accept = "application/json; charset=utf-8";



                        httpWebRequest.Headers.Add("Hrmapikey", Hrmapikey);
                        httpWebRequest.Headers.Add("Hrmapipassword", Hrmapipassword);
                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            streamWriter.Write(userJson);
                            streamWriter.Flush();
                            streamWriter.Close();
                            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                            {
                                var result = streamReader.ReadToEnd();
                                res = result.ToString();
                                strLog += res + "\t \t" + "DatedOn:" + DateTime.Now + "\n <========================================>";
                                GeneralMethods.LibraryLogWriter("UpdateUserHRM", strLog);
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                string Message = (ex.InnerException ?? ex).Message;
                strLog += Message + "\t \t" + "DatedOn:" + DateTime.Now + "\n <========================================>";
                GeneralMethods.LibraryLogWriter("UpdateUserHRM", strLog);
            }
            #endregion
        }

        #endregion [ Update UserHRM ]



        #region [ Change Password ]
        /// <summary>
        /// Change Password
        /// </summary>
        /// <returns></returns>
        [CustomActionAuthorization()]
        public ActionResult ChangePassword()
        {
            ChangePasswordDto changePasswordViewModel = new ChangePasswordDto();
            return View(changePasswordViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordDto changePasswordViewModel)
        {

            if (ModelState.IsValid)
            {
                if (CurrentUser != null && CurrentUser.Uid > 0)
                {
                    UserLogin userLogin = userLoginService.GetUserInfoByID(CurrentUser.Uid);

                    #region for Encryption
                    if (userLogin != null)
                    {
                        using (Rijndael myRijndael = Rijndael.Create())
                        {
                            var keybytes = GetRiJndael_KeyBytes(SiteKey.Encryption_Key);

                            string old_password = changePasswordViewModel.Password.Trim();
                            byte[] encrypted = EncryptStringToBytes(old_password, keybytes, keybytes);

                            //if (VerifyMd5Hash(md5Hash, old_password, userLogin.PasswordKey))
                            //*****Add condition userLogin.PasswordKey.CompareTo(old_password) == 0) for new user***//
                            if ((userLogin.PasswordKey.CompareTo(old_password) == 0) || Convert.ToBase64String(encrypted).CompareTo(userLogin.PasswordKey) == 0)  //string.IsNullOrEmpty(userLogin.PasswordKey) &&
                            {
                                string source = changePasswordViewModel.NewPassword.Trim();
                                string hash = Convert.ToBase64String(EncryptStringToBytes(source, keybytes, keybytes));

                                //userLogin.NewPassword = source;
                                //userLogin.Password = source;
                                userLogin.PasswordKey = hash;
                                userLoginService.ChangePassword(userLogin);
                                ShowSuccessMessage("Success", "Your password has been successfully updated", true);
                            }
                            else
                            {
                                ShowErrorMessage("Error", "Old Password does not match", false);
                            }
                        }
                    }

                    #endregion

                    #region before Encryption
                    //if (userLogin != null && userLogin.Password == changePasswordViewModel.Password)
                    //{

                    //string password = changePasswordViewModel.NewPassword.Trim();

                    //using (MD5 md5Hash = MD5.Create())
                    //{
                    //    string hash = GetMd5Hash(md5Hash, password);
                    //    userLogin.PasswordKey = hash;
                    //}

                    //userLogin.Password = password;
                    //userLoginService.ChangePassword(userLogin);
                    //ShowSuccessMessage("Success", "Your password has been successfully updated", true);

                    //}
                    //else
                    //{
                    //    ShowErrorMessage("Error", "Old Password does not match", false);
                    //}


                    #endregion
                    //userLoginService.ChangePassword(userLogin);
                }
            }
            return View();
        }

        public bool ChangePasswordAllUsers()
        {
            var status = false;
            var users = userLoginService.GetUsers();
            try
            {
                foreach (var user in users)
                {
                    if (string.IsNullOrEmpty(user.PasswordKey))
                    {
                        user.PasswordKey = "dots@123";
                    }

                    using (Rijndael myRijndael = Rijndael.Create())
                    {
                        var keybytes = GetRiJndael_KeyBytes(SiteKey.Encryption_Key);

                        string passwordkey = user.PasswordKey;
                        byte[] encrypted = EncryptStringToBytes(passwordkey, keybytes, keybytes);

                        user.PasswordKey = Convert.ToBase64String(encrypted);
                        userLoginService.Save(user);
                    }
                }
                status = true;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error!", ex.Message, false);
            }
            return status;
        }

        #endregion  [ Change Password ]

        #region [ Project Status ]

        [HttpGet]
        public ActionResult ProjectList()
        {
            Data.UserLogin UserModelDB = null;
            UserModelDB = userLoginService.GetUserInfoByID(CurrentUser.Uid);

            ////List<HomeProjectListDto> projectList = projectService.GetProjectListByGroupId(UserModelDB.UserGroupID, CurrentUser.PMUid, CurrentUser.Uid).ToList();
            List<HomeProjectListDto> projectList = projectService.GetProjectListByUser(CurrentUser.Uid, CurrentUser.PMUid).ToList();
            ViewData["ProjectList"] = projectList.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToList();

            List<HomeProjectListDto> projectListAdditional = projectService.GetProjectListByDeptId(CurrentUser.DeptId, CurrentUser.PMUid, CurrentUser.Uid).ToList();
            ViewData["ProjectListAdditional"] = projectListAdditional.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToList();

            UserProjectStatusDto model = new UserProjectStatusDto();
            model.ProjectId = WebsiteSession.SessionProjectStatus.NewGetObjectOrDefault("ProjectId") != null ? WebsiteSession.SessionProjectStatus.NewGetObjectOrDefault("ProjectId") + ":0" : "-1";
            model.ProjectIdAdditional = model.ProjectId;
            model.UserName = CurrentUser.UserName;
            model.UserStatus = WebsiteSession.SessionProjectStatus.NewGetObjectOrDefault("UserStatus") != null ? WebsiteSession.SessionProjectStatus.NewGetObjectOrDefault("UserStatus") : string.Empty;
            model.FreeText = WebsiteSession.SessionProjectStatus.NewGetObjectOrDefault("FreeText") != null ? WebsiteSession.SessionProjectStatus.NewGetObjectOrDefault("FreeText") : string.Empty;
            model.ManagingProjects = userActivityService.GetUserActivityByUid(CurrentUser.Uid)?.UserActivityManageProject?.Select(x => $"{x.ProjectId}:0")?.ToList();
            return PartialView("_ProjectStatus", model);
        }

        [HttpPost]
        public ActionResult SelectProject(string id, string text, string FreeProject = null, bool IsFree = false, bool IsExit = false, bool IsAway = false, bool IsAdditional = false, bool IsManagingProject = false)
        {
            try
            {
                FreeProject = !string.IsNullOrEmpty(FreeProject) ? FreeProject.TrimLength(500) : null;
                string CurrentProjectId = ((IsFree || IsExit) ? "0" : (string.IsNullOrEmpty(id) || id == "0") ? "0" : (id == "-1" ? "0" : id.Split(':')[0]));
                //string CurrentSubProjectId = ((IsFree || IsExit) ? "0" : (String.IsNullOrEmpty(id) || id == "0") ? "0" : (id == "-1" ? "0" : id.Split(':')[1]));
                string CurrentProjectName = (IsFree ? FreeProject != null ? FreeProject : string.Empty : text);

                int projectId = Convert.ToInt32(CurrentProjectId);

                List<UserActivityManageProject> UserActivityManageProjectList = null;
                if (IsManagingProject && CurrentUser.PMUid == SiteKey.AshishTeamPMUId)
                {
                    var idsList = id.Split(",");
                    var projectIds = new List<int>();
                    foreach (var item in idsList)
                    {
                        projectIds.Add(Convert.ToInt32(item.Split(':')[0]));
                    }

                    CurrentProjectName = $"{projectIds.Count} {(projectIds.Count > 1 ? "Projects" : "Project")}";
                    CurrentProjectId = null;
                    UserActivityManageProjectList = projectService.GetUserActivityManageProject(projectIds, CurrentUser.PMUid);
                }

                // Check if project is selected for Additional Support and User has sent Request or has Approval for same
                // If not, then open Add. Support popup for selected project

                #region Check for Add. Support

                if (IsAdditional && projectId > 0 && (RoleValidator.DV_RoleIds.Contains(CurrentUser.RoleId)))
                {
                    var project = projectService.GetProjectById(projectId);
                    if (!project.IsInHouse)
                    {
                        var hasAdditionalSupport = project.ProjectAdditionalSupports.Any(p =>
                                  p.ProjectId == projectId &&
                                  p.StartDate <= DateTime.Today && p.EndDate >= DateTime.Today &&
                                  p.Status != (byte)AddSupportRequestStatus.UnApproved
                                  //&& p.UserLogins.Any(u => u.Uid == CurrentUser.Uid) 
                                  //**** this above line will be use after update table with make collection of userlogins , so don't remove.
                                  );

                        if (!hasAdditionalSupport)
                        {
                            // Return to open Request Additional Support popup for selected Project
                            return Json(new { addAdditionalSupport = true, projectId = projectId });
                        }
                    }
                }

                #endregion

                string Status = (IsFree ? "Free" : ((string.IsNullOrEmpty(id) || id == "0") ? "Free" : (IsAdditional ? "Additional Support" : (IsManagingProject ? "Support Team" : "Running"))));
                Status = IsExit ? "Exit" : IsAway ? "Away" : Status;

                var dictProjectStatus = WebsiteSession.SessionProjectStatus;

                dictProjectStatus.AddOrReplace("ProjectId", CurrentProjectId);
                dictProjectStatus.AddOrReplace("ProjectName", CurrentProjectName.TrimLength(30));
                dictProjectStatus.AddOrReplace("UserStatus", Status);
                dictProjectStatus.AddOrReplace("FreeText", FreeProject);

                WebsiteSession.UpdateSessionProjectStatus(dictProjectStatus);


                //WebsiteSession.SessionProjectStatus.AddOrReplace("ProjectId", CurrentProjectId);
                //WebsiteSession.SessionProjectStatus.AddOrReplace("ProjectName", CurrentProjectName.TrimLength(30));
                //WebsiteSession.SessionProjectStatus.AddOrReplace("UserStatus", Status);
                //WebsiteSession.SessionProjectStatus.AddOrReplace("FreeText", FreeProject);




                DateTime DateModify = DateTime.Now;
                using (TransactionScope scope = new TransactionScope())
                {
                    UserActivity objUAct = new UserActivity(); // userActivityService.GetUserActivityByUid(CurrentUser.Uid);
                    objUAct.ActivityID = 0; // For new entry always
                    if (Convert.ToInt32(CurrentProjectId) > 0)
                    {
                        objUAct.ProjectId = Convert.ToInt32(CurrentProjectId);
                    }
                    objUAct.ProjectName = CurrentProjectName;
                    objUAct.Uid = CurrentUser.Uid;
                    objUAct.Date = DateModify.Date;
                    objUAct.DateAdded = DateModify;
                    objUAct.DateModify = DateModify;
                    objUAct.Status = Status;
                    objUAct.UserActivityManageProject = UserActivityManageProjectList;

                    userActivityService.Save(objUAct);

                    scope.Complete();
                }


                var result = new { userstatus = WebsiteSession.SessionProjectStatus.NewGetObjectOrDefault("UserStatus"), projectname = WebsiteSession.SessionProjectStatus.NewGetObjectOrDefault("ProjectName") };
                return Json(result);
            }
            catch
            {
                return Json(null);
            }
        }

        public ActionResult SetProjectStatus()
        {
            string ProjectId = null, projectName = null, userStatus = null, freeText = null;

            if (WebsiteSession.SessionProjectStatus == null ||
                WebsiteSession.SessionProjectStatus.Count == 0 ||
               string.IsNullOrEmpty(WebsiteSession.SessionProjectStatus.NewGetObjectOrDefault("UserStatus")))
            {
                UserActivity objUAct = userActivityService.GetUserActivityByUid(CurrentUser.Uid);

                ProjectId = (objUAct != null && objUAct.ProjectId.HasValue) ? objUAct.ProjectId.Value.ToString() : string.Empty;
                projectName = objUAct != null ? objUAct.ProjectName : string.Empty;
                userStatus = objUAct != null ? objUAct.Status : string.Empty;
                freeText = objUAct != null ? objUAct.Comment : string.Empty;
            }
            else
            {
                ProjectId = WebsiteSession.SessionProjectStatus.NewGetObjectOrDefault("ProjectId");
                projectName = WebsiteSession.SessionProjectStatus.NewGetObjectOrDefault("ProjectName");
                userStatus = WebsiteSession.SessionProjectStatus.NewGetObjectOrDefault("UserStatus");
                freeText = WebsiteSession.SessionProjectStatus.NewGetObjectOrDefault("FreeText");
            }

            var dictProjectStatus = WebsiteSession.SessionProjectStatus;

            dictProjectStatus.AddOrReplace("ProjectId", ProjectId);
            dictProjectStatus.AddOrReplace("ProjectName", projectName);
            dictProjectStatus.AddOrReplace("UserStatus", userStatus);
            dictProjectStatus.AddOrReplace("FreeText", freeText);

            WebsiteSession.UpdateSessionProjectStatus(dictProjectStatus);


            var result = new { userstatus = userStatus, projectname = !string.IsNullOrEmpty(projectName) ? projectName.TrimLength(30) : !string.IsNullOrEmpty(freeText) ? freeText.TrimLength(30) : string.Empty };
            return Json(result);
        }

        #endregion [ Project Status ]

        #region Manage User

        [CustomActionAuthorization]
        [HttpGet]
        public ActionResult ManageUser()
        {
            //var pmList1 = userLoginService.GetPMAndPMOHRUsers(true);

            //add directors in old list
            var pmList1 = userLoginService.GetPMAndPMOHRDirectorUsers(true);

            //if (CurrentUser.RoleId == (int)Enums.UserRoles.OP)
            //{
            //    pmList1 = pmList1.Where(p => p.RoleId != (int)Enums.UserRoles.HR).ToList();
            //}
            if (RoleValidator.HROperations_RoleIds.Contains(CurrentUser.RoleId))
            {
                pmList1 = pmList1.Where(p => p.RoleId != (int)Enums.UserRoles.HRBP).ToList();
            }
            var pmList = pmList1.Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.Uid.ToString(), Selected = (x.Uid == pmList1.FirstOrDefault().Uid ? true : false) }).OrderBy(x => x.Text).ToList();
            pmList.Insert(0, new SelectListItem() { Text = "-All-", Value = "0" });
            ViewBag.PMList = pmList;

            return View();
        }

        [CustomActionAuthorization]
        [HttpGet]
        public ActionResult ManagePff()
        {
            if (CurrentUser.RoleId == (int)Enums.UserRoles.PM && CurrentUser.DeptId != (int)Enums.ProjectDepartment.AccountDepartment)
            {
                return RedirectToAction("accessdenied", "error");
            }

            //var pmList1 = userLoginService.GetPMAndPMOHRUsers(true);

            //add directors in old list
            var pmList1 = userLoginService.GetPMAndPMOHRDirectorUsers(true);



            //if (CurrentUser.RoleId == (int)Enums.UserRoles.OP)
            //{
            //    pmList1 = pmList1.Where(p => p.RoleId != (int)Enums.UserRoles.HR).ToList();
            //}
            if (RoleValidator.HROperations_RoleIds.Contains(CurrentUser.RoleId))
            {
                pmList1 = pmList1.Where(p => p.RoleId != (int)Enums.UserRoles.HRBP).ToList();
            }
            var pmList = pmList1.Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.Uid.ToString(), Selected = (x.Uid == pmList1.FirstOrDefault().Uid ? true : false) }).OrderBy(x => x.Text).ToList();
            pmList.Insert(0, new SelectListItem() { Text = "-All-", Value = "0" });
            ViewBag.PMList = pmList;

            return View();
        }

        [HttpPost]
        public IActionResult ManagePffList(IDataTablesRequest request, string username, int status, int? pmId)
        {
            try
            {
                if (CurrentUser.RoleId == (int)Enums.UserRoles.PM && CurrentUser.DeptId != (int)Enums.ProjectDepartment.AccountDepartment)
                {
                    AccessDenied();

                }

                var pagingService = new PagingService<UserLogin>(request.Start, request.Length);

                var expr = PredicateBuilder.True<UserLogin>();
                expr = expr.And(e => e.IsInterestedPffaccount == true);
                //if (CurrentUser.RoleId != (int)Enums.UserRoles.HR && CurrentUser.RoleId != (int)Enums.UserRoles.OP && CurrentUser.RoleId != (int)Enums.UserRoles.PM && CurrentUser.RoleId != (int)Enums.UserRoles.PMO && CurrentUser.RoleId != (int)Enums.UserRoles.PMOAU)
                //{
                //    if (CurrentUser.PMUid > 0)
                //    {
                //        if (CurrentUser.RoleId == (int)Enums.UserRoles.TL)
                //        {
                //            int[] sdList = userLoginService.GetTLUsers(CurrentUser.Uid).Select(T => T.Uid).ToArray();
                //            expr = expr.And(e => (e.TLId == CurrentUser.Uid || sdList.Contains((int)e.TLId)) && e.RoleId != (int)Enums.UserRoles.HR && e.Uid != CurrentUser.Uid);
                //        }
                //        else
                //        {
                //            expr = expr.And(e => e.PMUid == CurrentUser.PMUid);
                //            expr = expr.And(e => e.TLId == CurrentUser.Uid);
                //            expr = expr.And(e => e.Uid != CurrentUser.Uid);
                //        }
                //    }
                //}
                //else
                //{
                //    if (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO || CurrentUser.RoleId == (int)Enums.UserRoles.PMOAU)
                //    {
                //        expr = expr.And(e => e.PMUid == CurrentUser.Uid);
                //        expr = expr.And(e => e.Uid != CurrentUser.Uid);
                //    }
                //    else if (CurrentUser.RoleId == (int)Enums.UserRoles.OP)
                //    {
                //        if (pmId > 0)
                //        {
                //            expr = expr.And(e => e.PMUid == pmId);
                //        }
                //        expr = expr.And(e => e.RoleId != (int)Enums.UserRoles.HR || e.Uid == pmId);
                //        expr = expr.And(e => e.Uid != CurrentUser.Uid);
                //    }
                //    else
                //    {
                //        expr = expr.And(e => e.PMUid == (CurrentUser.PMUid == 0 ? CurrentUser.Uid : CurrentUser.PMUid));
                //    }
                //}
                if (username != null)
                {
                    expr = expr.And(e => e.Name.Contains(username));
                }
                if (status != 0)
                {
                    if (status == 1)
                    {
                        expr = expr.And(e => e.IsActive == true);
                    }
                    else if (status == 2)
                    {
                        expr = expr.And(e => e.IsActive == false);
                    }
                }

                pagingService.Filter = expr;

                pagingService.Sort = (o) =>
                {
                    foreach (var item in request.SortedColumns())
                    {
                        switch (item.Name)
                        {
                            case "name":
                                return o.OrderByColumn(item, c => c.Name);
                            case "status":
                                return o.OrderByColumn(item, c => c.IsActive);
                        }
                    }

                    return o.OrderByDescending(c => c.IsActive == true).ThenBy(x => x.Name);
                };

                int totalCount = 0;
                var response = userLoginService.GetUsersList(out totalCount, pagingService);
                return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
                {
                    RowId = request.Start + index + 1,
                    userId = r.Uid,
                    name = r.Name,
                    role = r.Role?.RoleName ?? "",
                    teamLead = r.TLId != null ? GetTeamLead(r.TLId.Value) : "",
                    department = r.Department?.Name ?? "",
                    //technology = r.User_Tech.Count > 0 ? GetTechnologies(r.User_Tech.ToList()) : "-",
                    aadharNumber = r.AadharNumber ?? "",
                    panNumber = string.IsNullOrEmpty(r.PanNumber) ? "-" : r.PanNumber,
                    passportnumber = r.PassportNumber ?? "",
                    //status = r.IsActive,
                    empCode = r.EmpCode ?? "",
                    uanNumber = r.UANNumber ?? "-",
                    emailOffice = r.EmailOffice ?? "",
                    mobileNumber = r.MobileNumber ?? "",
                    bloodGroup = r.BloodGroupId.HasValue ? ((Enums.BloodGroups)r.BloodGroupId.Value).GetEnumDisplayName() : string.Empty,
                    title = r.Title ?? "",
                    designation = r.Designation.Name ?? "",
                    IsHavePfFiles = r.UserDocument.Any()
                }));
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error!", ex.Message, false);
                return RedirectToAction("manageuser");
            }
        }


        [HttpGet]
        public ActionResult Birthdays()
        {
            var pmList1 = userLoginService.GetPMAndPMOHRUsers(true);

            //if (CurrentUser.RoleId == (int)Enums.UserRoles.OP)
            if (CurrentUser.RoleId == ((int)Enums.UserRoles.HRBP))
            {
                pmList1 = pmList1.Where(p => p.RoleId != (int)Enums.UserRoles.HRBP).ToList();
            }
            var pmList = userLoginService.GetPMAndPMOHRUsers().Select(p => new SelectListItem { Text = p.Name, Value = p.Uid.ToString() }).ToList();
            //var pmList = pmList1.Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.Uid.ToString(), Selected = (x.Uid == pmList1.FirstOrDefault().Uid ? true : false) }).ToList();
            pmList.Insert(0, new SelectListItem() { Text = "-Select-", Value = "0" });
            ViewBag.PMList = pmList;
            return View();
        }



        [HttpPost]
        public IActionResult ManageUsersList(IDataTablesRequest request, string username, int status, int? pmId)
        {
            try
            {
                var pagingService = new PagingService<UserLogin>(request.Start, request.Length);

                var expr = PredicateBuilder.True<UserLogin>();
                if (!RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId) && CurrentUser.RoleId != (int)Enums.UserRoles.PM && CurrentUser.RoleId != (int)Enums.UserRoles.PMO && CurrentUser.RoleId != (int)Enums.UserRoles.PMOAU)
                {
                    if (CurrentUser.PMUid > 0)
                    {
                        if (RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId))
                        {
                            int[] sdList = userLoginService.GetTLUsers(CurrentUser.Uid).Select(T => T.Uid).ToArray();
                            expr = expr.And(e => (e.TLId == CurrentUser.Uid || sdList.Contains((int)e.TLId)) && e.RoleId != (int)Enums.UserRoles.HRBP && e.Uid != CurrentUser.Uid);
                        }
                        else
                        {
                            expr = expr.And(e => e.PMUid == CurrentUser.PMUid);
                            expr = expr.And(e => e.TLId == CurrentUser.Uid);
                            expr = expr.And(e => e.Uid != CurrentUser.Uid);
                        }
                    }
                }
                else
                {
                    if (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO || CurrentUser.RoleId == (int)Enums.UserRoles.PMOAU)
                    {
                        expr = expr.And(e => e.PMUid == CurrentUser.Uid);
                        expr = expr.And(e => e.Uid != CurrentUser.Uid);
                    }
                    else if (RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId))
                    {
                        if (pmId > 0)
                        {
                            expr = expr.And(e => e.PMUid == pmId);
                        }
                        expr = expr.And(e => e.RoleId != (int)Enums.UserRoles.HRBP || e.Uid == pmId);
                        expr = expr.And(e => e.Uid != CurrentUser.Uid);
                    }
                    else
                    {
                        expr = expr.And(e => e.PMUid == (CurrentUser.PMUid == 0 ? CurrentUser.Uid : CurrentUser.PMUid));
                    }
                }
                if (username != null)
                {
                    expr = expr.And(e => e.Name.Contains(username));
                }
                if (status != 0)
                {
                    if (status == 1)
                    {
                        expr = expr.And(e => e.IsActive == true);
                    }
                    else if (status == 2)
                    {
                        expr = expr.And(e => e.IsActive == false);
                    }
                }

                pagingService.Filter = expr;

                pagingService.Sort = (o) =>
                {
                    foreach (var item in request.SortedColumns())
                    {
                        switch (item.Name)
                        {
                            case "name":
                                return o.OrderByColumn(item, c => c.Name);
                            case "status":
                                return o.OrderByColumn(item, c => c.IsActive);
                        }
                    }

                    return o.OrderByDescending(c => c.IsActive == true).ThenBy(x => x.Name);
                };

                int totalCount = 0;
                var response = userLoginService.GetUsersList(out totalCount, pagingService);
                return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
                {
                    RowId = request.Start + index + 1,
                    userId = r.Uid,
                    name = r.Name,
                    role = r.Role?.RoleName ?? "",
                    teamLead = r.TLId != null ? GetTeamLead(r.TLId.Value) : "",
                    department = r.Department?.Name ?? "",
                    //technology = r.User_Tech.Count > 0 ? GetTechnologies(r.User_Tech.ToList()) : "-",
                    aadharnumber = r.AadharNumber ?? "",
                    //pannumber = string.IsNullOrEmpty(r.PanNumber) ? "-" : r.PanNumber,
                    passportnumber = r.PassportNumber ?? "",
                    //status = r.IsActive,
                    empCode = r.EmpCode ?? "",
                    emailOffice = r.EmailOffice ?? "",
                    mobileNumber = r.MobileNumber ?? "",
                    bloodGroup = r.BloodGroupId.HasValue ? ((Enums.BloodGroups)r.BloodGroupId.Value).GetEnumDisplayName() : string.Empty,
                    title = r.Title ?? "",
                    designation = r.Designation != null ? r.Designation.Name : ""
                }));
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error!", ex.Message, false);
                return RedirectToAction("manageuser");
            }
        }


        [HttpPost]
        public IActionResult BirthdaysList(IDataTablesRequest request, int? pmId, string FromDate, string ToDate)
        {
            try
            {
                var pagingService = new PagingService<UserLogin>(request.Start, request.Length);

                var expr = PredicateBuilder.True<UserLogin>();
                if (!RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId) && CurrentUser.RoleId != (int)Enums.UserRoles.PM && CurrentUser.RoleId != (int)Enums.UserRoles.PMO)
                {
                    if (CurrentUser.PMUid > 0)
                    {
                        expr = expr.And(e => e.PMUid == CurrentUser.PMUid);
                        expr = expr.And(e => e.TLId == CurrentUser.Uid);
                        expr = expr.And(e => e.Uid != CurrentUser.Uid);
                    }
                }
                else
                {
                    if (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO)
                    {
                        // 
                        expr = expr.And(e => e.PMUid == CurrentUser.Uid);
                        expr = expr.And(e => e.Uid != CurrentUser.Uid);
                        expr = expr.And(e => e.TLId == CurrentUser.Uid);
                    }
                    else if (RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId))
                    {
                        if (pmId > 0)
                        {
                            expr = expr.And(e => e.TLId == pmId);
                        }
                        expr = expr.And(e => e.RoleId != (int)Enums.UserRoles.HRBP || e.Uid == pmId);
                        expr = expr.And(e => e.Uid != CurrentUser.Uid);
                    }
                    else
                    {
                        expr = expr.And(e => e.PMUid == (CurrentUser.PMUid == 0 ? CurrentUser.Uid : CurrentUser.PMUid));
                    }
                }

                DateTime? fromDate = !string.IsNullOrWhiteSpace(FromDate) ?
                    DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;
                DateTime? toDate = !string.IsNullOrWhiteSpace(ToDate) ?
                    DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;

                if (FromDate != null && ToDate != null)
                {
                    expr = expr.And(e => e.DOB.Value.Day >= fromDate.Value.Day && e.DOB.Value.Month >= fromDate.Value.Month &&
                    e.DOB.Value.Day <= toDate.Value.Day && e.DOB.Value.Month <= toDate.Value.Month);
                }
                else if (FromDate != null)
                {
                    expr = expr.And(e => e.DOB.Value.Day == fromDate.Value.Day && e.DOB.Value.Month == fromDate.Value.Month);
                }
                else if (ToDate != null)
                {
                    expr = expr.And(e => e.DOB.Value.Day == toDate.Value.Day && e.DOB.Value.Month == toDate.Value.Month);
                }
                //else {
                //    expr = expr.And(e => e.DOB.Value.Day == DateTime.Now.Day && e.DOB.Value.Month == DateTime.Now.Month);
                //}

                ContextProvider.HttpContext.Session.SetString("ddl_pm", pmId > 0 ? pmId.ToString() : "");
                ContextProvider.HttpContext.Session.SetString("txt_dateFrom", FromDate != null ? FromDate.ToString() : "");
                ContextProvider.HttpContext.Session.SetString("txt_dateTo", ToDate != null ? ToDate.ToString() : "");

                expr = expr.And(e => e.IsActive == true);

                pagingService.Filter = expr;

                pagingService.Sort = (o) =>
                {
                    foreach (var item in request.SortedColumns())
                    {
                        switch (item.Name)
                        {
                            case "name":
                                return o.OrderByColumn(item, c => c.Name);
                            case "dob":
                                return o.OrderByColumn(item, c => c.DOB);
                        }
                    }

                    return o.OrderBy(c => c.DOB.Value.Month).ThenBy(c => c.DOB.Value.Day);
                };

                int totalCount = 0;
                var response = userLoginService.GetUsersList(out totalCount, pagingService);
                return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
                {
                    RowId = request.Start + index + 1,
                    userId = r.Uid,
                    name = r.Name,
                    dob = r.DOB.ToFormatDateString("dd MMMM"),
                    role = r.Role?.RoleName ?? "",
                    teamLead = r.TLId != null ? GetTeamLead(r.TLId.Value) : "",
                    department = r.Department?.Name ?? "",
                    title = r.Title ?? "",
                    designation = r.Designation.Name ?? "",
                    empCode = r.EmpCode ?? "",
                    emailOffice = r.EmailOffice ?? "",
                }));
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error!", ex.Message, false);
                return RedirectToAction("birthdays");
            }
        }


        public ActionResult ExportBirthdayDataReportToExcel(IDataTablesRequest request, int? pmid)
        {


            var expr = PredicateBuilder.True<UserLogin>();

            //var pagingService1 = new PagingService<UserLogin>(request.Start, request.Length);
            PagingService<UserLogin> pagingService = new PagingService<UserLogin>(request == null ? 0 : request.Start, request == null ? 0 : request.Length);

            string PMId = HttpContext.Session.GetString("ddl_pm");
            string Datefrom = HttpContext.Session.GetString("txt_dateFrom");
            string DateTo = HttpContext.Session.GetString("txt_dateTo");

            DateTime? frmDate = !string.IsNullOrWhiteSpace(Datefrom) ?
                    DateTime.ParseExact(Datefrom, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;
            DateTime? tDate = !string.IsNullOrWhiteSpace(DateTo) ?
                DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;


            if (!RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId) && CurrentUser.RoleId != (int)Enums.UserRoles.PM && CurrentUser.RoleId != (int)Enums.UserRoles.PMO)
            {
                if (CurrentUser.PMUid > 0)
                {
                    expr = expr.And(e => e.PMUid == CurrentUser.PMUid);
                    expr = expr.And(e => e.TLId == CurrentUser.Uid);
                    expr = expr.And(e => e.Uid != CurrentUser.Uid);
                }
            }
            else
            {
                if (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO)
                {
                    // 
                    expr = expr.And(e => e.PMUid == CurrentUser.Uid);
                    expr = expr.And(e => e.Uid != CurrentUser.Uid);
                    expr = expr.And(e => e.TLId == CurrentUser.Uid);
                }
                else if (RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId))
                {
                    int temp_id;
                    int? pmId = int.TryParse(PMId, out temp_id) ? int.Parse(PMId) : (int?)null;
                    if (pmId > 0)
                    {
                        expr = expr.And(e => e.TLId == pmId);
                    }
                    expr = expr.And(e => e.RoleId != (int)Enums.UserRoles.HRBP || e.Uid == pmId);
                    expr = expr.And(e => e.Uid != CurrentUser.Uid);
                }
                else
                {
                    expr = expr.And(e => e.PMUid == (CurrentUser.PMUid == 0 ? CurrentUser.Uid : CurrentUser.PMUid));
                }
            }

            if (frmDate != null && tDate != null)
            {
                expr = expr.And(e => e.DOB.Value.Day >= frmDate.Value.Day && e.DOB.Value.Month >= frmDate.Value.Month &&
                e.DOB.Value.Day <= tDate.Value.Day && e.DOB.Value.Month <= tDate.Value.Month);
            }
            else if (frmDate != null)
            {
                expr = expr.And(e => e.DOB.Value.Day == frmDate.Value.Day && e.DOB.Value.Month == frmDate.Value.Month);
            }
            else if (tDate != null)
            {
                expr = expr.And(e => e.DOB.Value.Day == tDate.Value.Day && e.DOB.Value.Month == tDate.Value.Month);
            }

            expr = expr.And(e => e.IsActive == true);

            pagingService.Filter = expr;

            pagingService.Sort = (o) =>
            {
                return o.Where(c => c.IsActive == true).OrderBy(a => a.DOB.Value.Month).ThenBy(x => x.DOB.Value.Day);
            };

            int totalCount = 0;

            var response = userLoginService.GetUsersList(out totalCount, pagingService);

            List<UserProfileDto> responseReport = response.Select(s => new UserProfileDto()
            {
                Name = s.Name,
                Title = s.Title,
                EmailOffice = s.EmailOffice,
                DOB = s.DOB != null ? s.DOB.ToFormatDateString("dd MMMM") : string.Empty,
                RoleName = s.Role?.RoleName ?? "",
                TLName = s.TLId != null ? GetTeamLead(s.TLId.Value) : "",
                DeptName = s.Department?.Name ?? "",

            }).ToList();


            string Reportname = "User_Birthday_Report";
            int subsheet = 0;
            List<ExportExcelColumn> excelColumn = new List<ExportExcelColumn>();

            excelColumn.Add(new ExportExcelColumn { ColumnName = "Sr. No.", PropertyName = "SrNo", ColumnWidth = 1000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Name", PropertyName = "Name", ColumnWidth = 5000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "DOB", PropertyName = "DOB", ColumnWidth = 5000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Office Email", PropertyName = "Office Email", ColumnWidth = 50000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "TEAM/PROJECT LEAD", PropertyName = "TEAM/PROJECT LEAD", ColumnWidth = 5000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "DEPARTMENT", PropertyName = "DEPARTMENT", ColumnWidth = 5000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "ROLE/DESIGNATION", PropertyName = "ROLE/DESIGNATION", ColumnWidth = 3500 });


            var memoryStream = ToExportToExcel(responseReport, subsheet, Reportname, excelColumn);

            return File(memoryStream.ToArray(), "application/vnd.ms-excel", Reportname.Trim().Replace(" ", "_") + "_" + DateTime.Now.ToString("hh_mm_ss_tt") + ".xls");
        }

        public static MemoryStream ToExportToExcel(List<UserProfileDto> lsObj, int isSubsheet, string Reportname, List<ExportExcelColumn> excelColumn)
        {
            MemoryStream response = new MemoryStream();
            if (lsObj != null && lsObj.Count() > 0)
            {
                bool columnFlag = false;
                List<string> props = new List<string>();
                List<string> childprops = new List<string>();
                //Get the column names of Employee Birthday Data
                if (excelColumn != null && excelColumn.Count > 0)
                {
                    columnFlag = true;
                    props = excelColumn.Select(s => s.PropertyName.Trim()).ToList();
                }

                //Get the column names of Employee Birthday Data        
                if (props != null && props.Count > 0)
                {
                    var workbook = new HSSFWorkbook();
                    var headerLabelCellStyle = workbook.CreateCellStyle();
                    headerLabelCellStyle.Alignment = HorizontalAlignment.LEFT;
                    headerLabelCellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    headerLabelCellStyle.BorderBottom = CellBorderType.THIN;
                    headerLabelCellStyle.WrapText = true;

                    var headerLabelFont = workbook.CreateFont();
                    headerLabelFont.Boldweight = (short)FontBoldWeight.BOLD;
                    headerLabelFont.Color = HSSFColor.BLACK.index;
                    headerLabelCellStyle.SetFont(headerLabelFont);

                    var headerCellStyle = workbook.CreateCellStyle();
                    headerCellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    headerLabelFont.FontHeightInPoints = 11;
                    headerCellStyle.SetFont(headerLabelFont);

                    var sheet = workbook.CreateSheet(Reportname);
                    var attendeeLabelCellStyle = workbook.CreateCellStyle();

                    attendeeLabelCellStyle.Alignment = HorizontalAlignment.LEFT;
                    attendeeLabelCellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    attendeeLabelCellStyle.BorderBottom = CellBorderType.THIN;
                    attendeeLabelCellStyle.WrapText = true;
                    attendeeLabelCellStyle.FillForegroundColor = HSSFColor.GREY_50_PERCENT.index;
                    attendeeLabelCellStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
                    var attendeeLabelFont = workbook.CreateFont();
                    attendeeLabelFont.Boldweight = (short)FontBoldWeight.BOLD;
                    attendeeLabelFont.Color = HSSFColor.WHITE.index;
                    attendeeLabelCellStyle.SetFont(attendeeLabelFont);

                    //Create a header row
                    var headerRow = sheet.CreateRow(0);
                    //Set the column names in the header row

                    var count = 0;
                    foreach (var item in props)
                    {
                        headerRow.CreateCell(count, CellType.STRING).SetCellValue(
                         columnFlag ? excelColumn.Where(x => x.PropertyName == item).Select(x => x.ColumnName).FirstOrDefault() : item);
                        count = count + 1;
                    }

                    for (int i = 0; i < headerRow.LastCellNum; i++)
                    {
                        headerRow.GetCell(i).CellStyle = attendeeLabelCellStyle;
                        headerRow.Height = 350;
                    }

                    //(Optional) freeze the header row so it is not scrolled
                    sheet.CreateFreezePane(0, 1, 0, 1);

                    int rowNumber = 1; // Index of Row for data

                    var rowCellStyleattendeeothercolumn = workbook.CreateCellStyle();
                    rowCellStyleattendeeothercolumn.VerticalAlignment = VerticalAlignment.CENTER;

                    var otherattendeeLabelFont = workbook.CreateFont();
                    otherattendeeLabelFont.FontHeightInPoints = 9;
                    rowCellStyleattendeeothercolumn.SetFont(otherattendeeLabelFont);

                    for (int i = 0; i < lsObj.Count(); i++)
                    {
                        var row = sheet.CreateRow(rowNumber);

                        count = 0;
                        row.CreateCell(count++).SetCellValue(i + 1); // Sr. No.
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].Title) + ' ' + Convert.ToString(lsObj[i].Name));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].DOB));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].EmailOffice));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].TLName));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].DeptName));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].RoleName));

                        rowNumber++;

                        for (int k = 0; k < headerRow.LastCellNum; k++)
                        {
                            string columnName = headerRow.GetCell(k).ToString();
                            sheet.SetColumnWidth(k, 5500);
                        }
                        for (int k = 6; k < headerRow.LastCellNum; k++)
                        {
                            if (row != null)
                            {
                                string columnName = row.GetCell(k).ToString();
                                sheet.SetColumnWidth(k, 5000);
                            }
                        }

                        row.Height = 350;
                    }
                    workbook.Write(response);
                    //Return the result to the end user
                }
            }
            return response;
        }


        [HttpGet]
        public string GetTeamLead(int id)
        {
            var user = string.Empty;
            var userdata = userLoginService.GetUserInfoByID(id);
            if (userdata != null)
            {
                user = userdata.Name;
            }
            else
            {
                user = "-";
            }

            return user;
        }

        [HttpGet]
        public string GetTechnologies(List<User_Tech> techlist)
        {
            var technologies = string.Empty;
            foreach (var tech in techlist)
            {
                technologies += (tech.Technology.Title + ", ");
            }
            return technologies.TrimLength(50).Trim().TrimEnd(',');
        }

        [CustomActionAuthorization]
        [HttpGet]
        public ActionResult EditUser(int? id)
        {
            bool canAccess = false;
            if (CurrentUser.IsSuperAdmin)
            {
                canAccess = true;
            }
            else if (id != null)
            {
                canAccess = true;
            }
            if (canAccess)
            {
                Data.UserLogin UserModel = new Data.UserLogin();
                ManageUserDto UserProfile = new ManageUserDto();
                UserProfile.EmployeeMedicalData = new MedicalDataDto();
                EmployeeMedicalData UserMedicalModel = new EmployeeMedicalData();
                try
                {
                    var DepartmentList = departmentService.GetDepartments().Select(x => new SelectListItem { Text = x.Name, Value = x.DeptId.ToString() }).ToList(); ;
                    ViewBag.DepartmentList = DepartmentList;

                    var RoleCategoryList = userLoginService.GetRolesCategory().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList(); ;
                    ViewBag.RoleCategoryList = RoleCategoryList;

                    UserProfile.TechnologyList = technologyService.GetTechnologyList().OrderBy(x => x.Title)
                                    .Select(x => new UserTechnologyDto { TechId = x.TechId, TechName = x.Title }).ToList();
                    UserProfile.SpecTypeList = WebExtensions.GetList<Enums.TechnologySpecializationType>();

                    UserProfile.DomainExpert = domainTypeService.GetDomainList().OrderBy(x => x.DomainName)
                              .Select(x => new DomainExpertDto { DomainId = x.DomainId, DomainName = x.DomainName }).ToList();

                    if (id != null)
                    {
                        UserModel = userLoginService.GetUserInfoByID(id.Value);

                        UserMedicalModel = medicalDataService.GetEmployeeMedicalDataByUserid(id.Value);
                        UserProfile.Uid = UserModel.Uid;
                        UserProfile.AttendanceId = UserModel.AttendenceId.HasValue ? UserModel.AttendenceId : null;
                        UserProfile.EmployeeCode = !string.IsNullOrWhiteSpace(UserModel.EmpCode) ? UserModel.EmpCode : "";
                        UserProfile.IsActive = UserModel.IsActive.Value;
                        UserProfile.Title = UserModel.Title;
                        UserProfile.Name = UserModel.Name;
                        UserProfile.UserName = UserModel.UserName;
                        // UserProfile.Password = UserModel.Password;
                        // string pass = EncryptDecrypt.DecryptTo(UserModel.PasswordKey);
                        UserProfile.PasswordKey = UserModel.PasswordKey;
                        var roleCategoryId = roleService.GetRoleById(CurrentUser.RoleId).RoleCategoryId;
                        if (roleCategoryId != null)
                        {
                            UserProfile.RoleCateGoryId = UserModel.Role.RoleCategory != null ? UserModel.Role.RoleCategory.Id : 0;
                            UserProfile.RoleCategoryName = UserModel.Role.RoleCategory != null ? UserModel.Role.RoleCategory.Name : string.Empty;

                            var RoleList = userLoginService.GetRolesByRoleCategoyId(UserProfile.RoleCateGoryId).Select(x => new SelectListItem { Text = x.RoleName, Value = x.RoleId.ToString() }).ToList();
                            ViewBag.RoleList = RoleList;

                        }
                        else
                        {
                            var RoleList = userLoginService.GetRolesByRoleCategoyId(null).Select(x => new SelectListItem { Text = x.RoleName, Value = x.RoleId.ToString() }).ToList();
                            ViewBag.RoleList = RoleList;
                        }
                        if (UserModel.RoleId != null)
                        {
                            UserProfile.RoleId = UserModel.RoleId;
                            var DesignationList = userLoginService.GetDesignationList(UserModel.RoleId).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
                            ViewBag.DesignationList = DesignationList;
                        }
                        else
                        {
                            var DesignationList = userLoginService.GetDesignationList(null).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
                            ViewBag.DesignationList = DesignationList;
                        }
                        if (UserModel.DesignationId != null)
                        {

                            UserProfile.DesignationId = UserModel.DesignationId;

                        }
                        UserProfile.JobTitle = !string.IsNullOrWhiteSpace(UserModel.JobTitle) ? UserModel.JobTitle : UserModel.Role.RoleName;
                        UserProfile.DOB = UserModel.DOB.ToFormatDateString("dd/MM/yyyy");
                        UserProfile.JoinedDate = UserModel.JoinedDate.ToFormatDateString("MMM, dd yyyy");
                        UserProfile.EmailOffice = UserModel.EmailOffice;
                        UserProfile.EmailPersonal = UserModel.EmailPersonal;
                        UserProfile.MobileNumber = UserModel.MobileNumber;
                        UserProfile.PhoneNumber = UserModel.PhoneNumber;
                        UserProfile.AlternativeNumber = UserModel.AlternativeNumber;
                        UserProfile.Address = UserModel.Address;
                        UserProfile.SkypeId = UserModel.SkypeId;
                        UserProfile.MarraigeDate = UserModel.MarraigeDate.ToFormatDateString("dd/MM/yyyy");
                        UserProfile.Gender = UserModel.Gender;
                        UserProfile.AddDate = UserModel.AddDate ?? DateTime.Now;
                        UserProfile.RoleName = UserModel.Role?.RoleName ?? "";
                        UserProfile.OtherTechnology = UserModel.OtherTechnology;
                        UserProfile.IsResigned = UserModel.IsResigned;

                        if (UserModel.DeptId != null)
                        {
                            UserProfile.DeptId = UserModel.DeptId ?? 0;
                            UserProfile.DeptName = UserModel.Department.Name;
                            UserProfile.DeptName = UserModel.Department.Name;
                        }
                        UserProfile.AadharNumber = UserModel.AadharNumber;
                        UserProfile.PanNumber = UserModel.PanNumber;
                        UserProfile.PassportNumber = UserModel.PassportNumber;
                        UserProfile.BloodGroupId = Convert.ToInt32(UserModel.BloodGroupId) == 0 ? 0 : Convert.ToInt32(UserModel.BloodGroupId);

                        if (UserMedicalModel != null)
                        {
                            UserProfile.EmployeeMedicalData.Id = UserMedicalModel.Id;
                            UserProfile.EmployeeMedicalData.PremiumTotal = UserMedicalModel.PremiumTotal != null ? UserMedicalModel.PremiumTotal.Value : 0;
                            UserProfile.EmployeeMedicalData.PremiumPerMonth = UserMedicalModel.PremiumPerMonth != null ? Math.Round(UserMedicalModel.PremiumPerMonth.Value, 2) : 0;
                            UserProfile.EmployeeMedicalData.TotalCoverage = UserMedicalModel.TotalCoverage
                                != null ? UserMedicalModel.TotalCoverage.Value : 0;
                            UserProfile.EmployeeMedicalData.Validity = UserMedicalModel.Validity
                                != null ? UserMedicalModel.Validity.Value : 0;
                        }

                        var userPM = userLoginService.GetUserInfoByID(UserModel.PMUid.Value);
                        UserProfile.PMUid = UserModel.PMUid.Value;
                        UserProfile.PMName = userPM != null ? userPM.Name : "";
                        UserProfile.TLId = UserModel.TLId.HasValue ? UserModel.TLId.Value : 0;

                        //if (CurrentUser.RoleId == (int)Enums.UserRoles.TL)
                        //{
                        //    UserProfile.TLName = CurrentUser.Name;
                        //}

                        if (UserModel.TLId != null && UserModel.TLId != 0)
                        {
                            UserProfile.TLName = userLoginService.GetUserInfoByID(Convert.ToInt32(UserModel.TLId)).Name;
                        }
                        else
                        {
                            UserProfile.TLName = string.Empty;
                        }

                        //if(UserModel.TLId!=null && UserModel.TLId!=0)
                        //{
                        //    UserProfile.TLName = userLoginService.GetUserInfoByID(Convert.ToInt32(UserModel.TLId)).Name;
                        //}
                        //else
                        //{
                        //    UserProfile.TLName = string.Empty;
                        //}

                        if (UserModel.User_Tech.Any())
                        {
                            UserProfile.TechnologyList.ForEach(t =>
                            {
                                var usrTech = UserModel.User_Tech.FirstOrDefault(ut => ut.TechId == t.TechId);
                                if (usrTech != null)
                                {
                                    t.Selected = true;
                                    t.SpecTypeId = usrTech.SpecTypeId;
                                }
                            });
                        }

                        if (UserModel.DomainExperts.Any())
                        {
                            UserProfile.DomainExpert.ForEach(t =>
                            {
                                var usrDomain = UserModel.DomainExperts.FirstOrDefault(ut => ut.DomainId == t.DomainId);
                                if (usrDomain != null)
                                {
                                    t.Selected = true;
                                    t.DomainId = usrDomain.DomainId;
                                }
                            });
                        }
                        else
                        {
                            var DomainList = domainTypeService.GetDomainList();

                            // var UserProfile.DomainExpert
                            //List<DomainExpertDto> DomainExpert = new List<DomainExpertDto>();
                            foreach (var domain in DomainList)
                            {
                                var usrDomain = new DomainExpertDto();
                                usrDomain.Selected = false;
                                usrDomain.DomainId = domain.DomainId;
                                usrDomain.DomainName = domain.DomainName;
                                UserProfile.DomainExpert.Add(usrDomain);
                            }
                            // UserProfile.DomainExpert = responseParticipants.SelectMany(n => n.DomainList.Select(m => new SelectListItem { Text = m.U.Name, Value = m.U.Uid.ToString() })).ToList();
                            // UserProfile.DomainExpert = DomainList.SelectMany(m => new{ DomainId = m.DomainId, DomainName = m.DomainName.ToString(), })).ToList();

                            //UserProfile.DomainExpert.ForEach(t => {
                            //    var usrDomain = UserModel.DomainExperts.FirstOrDefault(ut => ut.DomainId == t.DomainId);
                            //    if (usrDomain != null) {
                            //        t.Selected = false;
                            //        t.DomainId = usrDomain.DomainId;
                            //    }
                            //});
                        }


                        var userTLList = new List<Data.UserLogin>();

                        if (RoleValidator.HRWithOutTrainee_DesignationIds.Contains(CurrentUser.DesignationId))
                        {
                            var pmList = userLoginService.GetPMAndPMOHRUsers(true).OrderBy(G => G.Uid).Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() }).ToList();
                            ViewBag.PMList = pmList;
                        }
                        else
                        {
                            var GetUser = userLoginService.GetUserInfoByID(CurrentUser.PMUid);
                            if (GetUser != null && RoleValidator.HRWithOutTrainee_DesignationIds.Contains(GetUser.DesignationId.Value))
                            {
                                var pmList = userLoginService.GetUsers().Where(p => p.Uid.Equals(CurrentUser.RoleId == (int)Enums.UserRoles.PM
                                     ? CurrentUser.Uid : CurrentUser.PMUid) && RoleValidator.HRWithOutTrainee_DesignationIds.Contains(p.DesignationId.Value)).Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() }).ToList();
                                ViewBag.PMList = pmList;
                            }
                            else
                            {
                                var pmList = userLoginService.GetUsers().Where(p => p.Uid.Equals((CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO)
                                                         ? CurrentUser.Uid : CurrentUser.PMUid) && (p.RoleId == (int)Enums.UserRoles.PM || p.RoleId == (int)Enums.UserRoles.PMO)).Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() }).ToList();

                                ViewBag.PMList = pmList;
                            }
                        }

                        List<UserLogin> ddlteamLead = new List<UserLogin>();
                        if (UserModel.RoleId != (int)Enums.UserRoles.PM && UserModel.RoleId != (int)Enums.UserRoles.PMO)
                        {
                            var pmUser = userLoginService.GetUserInfoByID(UserModel.PMUid.Value);
                            if (RoleValidator.HRWithOutTrainee_DesignationIds.Contains(pmUser.DesignationId.Value))
                            {
                                //ddlteamLead = userLoginService.GetUserByRole((int)Enums.UserRoles.HRBP, true).Distinct().OrderBy(T => T.Name).Where(P => P.PMUid == UserModel.PMUid.Value || P.Uid == UserModel.PMUid.Value).ToList();
                                ddlteamLead = userLoginService.GetUserByDesignation(RoleValidator.HRWithOutTrainee_DesignationIds, true).Distinct().OrderBy(T => T.Name).Where(P => P.PMUid == UserModel.PMUid.Value || P.Uid == UserModel.PMUid.Value).ToList();
                            }

                            else
                            {
                                ddlteamLead = userLoginService.GetTLSDUsers(UserModel.PMUid.Value);
                            }
                            ViewBag.TLList = ddlteamLead;
                        }
                        //userTLList = userLoginService.GetTLSDUsers(UserModel.PMUid.Value);


                        IEnumerable<Core.Enums.BloodGroups> actionTypes = Enum.GetValues(typeof(Core.Enums.BloodGroups)).Cast<Core.Enums.BloodGroups>();
                        ViewBag.bloodGroup = actionTypes.Select(c => new SelectListItem { Text = c.GetEnumDisplayName(), Value = ((int)c).ToString() }).ToList();
                    }
                    else
                    {
                        //if (CurrentUser.RoleId != (int)Enums.UserRoles.HR || CurrentUser.RoleId != (int)Enums.UserRoles.PM || CurrentUser.RoleId != (int)Enums.UserRoles.PMO || CurrentUser.RoleId != (int)Enums.UserRoles.UKPM)
                        //{
                        //    var userPM = userLoginService.GetUserInfoByID(CurrentUser.PMUid);
                        //    UserProfile.PMUid = CurrentUser.PMUid;
                        //    UserProfile.PMName = userPM != null ? userPM.Name : "";
                        //    UserProfile.TLId = CurrentUser.TLId != 0 ? CurrentUser.TLId : 0;
                        //    UserProfile.TLName = CurrentUser.Name;
                        //    //UserProfile.RoleName = userLoginService.getrol
                        //}
                        UserProfile.IsActive = true;
                        UserProfile.Gender = "M";
                        List<Data.UserLogin> userTLList = new List<Data.UserLogin>();

                        //if (CurrentUser.RoleId == (int)Enums.UserRoles.HR)
                        if (CurrentUser.IsSuperAdmin || RoleValidator.HRWithOutTrainee_DesignationIds.Contains(CurrentUser.DesignationId))
                        {
                            var pmList = userLoginService.GetPMAndPMOHRUsers(true).OrderBy(G => G.Uid).Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() }).ToList();
                            ViewBag.PMList = pmList;
                            UserProfile.PMUid = null;
                        }
                        else
                        {
                            var GetUser = userLoginService.GetUserInfoByID(CurrentUser.PMUid);
                            if (GetUser != null && RoleValidator.HRWithOutTrainee_DesignationIds.Contains(GetUser.DesignationId.Value))
                            {
                                var pmList = userLoginService.GetUsers().Where(p => p.Uid.Equals(CurrentUser.RoleId == (int)Enums.UserRoles.PM
                                     ? CurrentUser.Uid : CurrentUser.PMUid) && RoleValidator.HRWithOutTrainee_DesignationIds.Contains(p.DesignationId.Value)).Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() }).ToList();
                                ViewBag.PMList = pmList;
                            }
                            else
                            {
                                var pmList = userLoginService.GetUsers().Where(p => p.Uid.Equals(CurrentUser.RoleId == (int)Enums.UserRoles.PM
                                                         ? CurrentUser.Uid : CurrentUser.PMUid) && (p.RoleId == (int)Enums.UserRoles.PM)).Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() }).ToList();
                                ViewBag.PMList = pmList;
                            }
                        }
                        ViewBag.TLList = userTLList;

                        IEnumerable<Core.Enums.BloodGroups> actionTypes = Enum.GetValues(typeof(Core.Enums.BloodGroups)).Cast<Core.Enums.BloodGroups>();
                        ViewBag.bloodGroup = actionTypes.Select(c => new SelectListItem { Text = c.GetEnumDisplayName(), Value = ((int)c).ToString() }).ToList();
                        var RoleList = userLoginService.GetRolesWithoutPM(true).Select(x => new SelectListItem { Text = x.RoleName, Value = x.RoleId.ToString() }).ToList();
                        ViewBag.RoleList = RoleList;
                        var DesignationList = userLoginService.GetDesignationList(null).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
                        ViewBag.DesignationList = DesignationList;
                    }
                }
                catch (Exception) { }
                return View(UserProfile);
            }
            else
            {
                //return Content("you are not allowed to access to page");
                return RedirectToAction("accessdenied", "error");
            }
        }



        [HttpPost]
        public ActionResult EditUser(ManageUserDto UserModel)
        {
            var fileLogPath = SiteKey.IsLive ? System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "User_Log", "user-log.txt") : logPath;
            
            LogPrint.WriteIntoFile(fileLogPath, $"---------------------------ems Edit user started---------------------------");

            AjaxResponseDto ajaxResponseDto = new AjaxResponseDto();

            UserLogin UserModelDB = new UserLogin();
            EmployeeMedicalData EmployeeMedicalModelDB = new EmployeeMedicalData();
            EmployeeMedicalData EmployeeMedicalModel = new EmployeeMedicalData();
            if (UserModel.AttendanceId.HasValue)
            {
                bool isAttendanceIdValid = userLoginService.CheckUserAttendanceId(UserModel.EmailOffice, Convert.ToInt32(UserModel.AttendanceId));
                if (!isAttendanceIdValid)
                {
                    ajaxResponseDto.Success = false;
                    ajaxResponseDto.Message = "Attendance id already exists with another user.";
                    return Json(ajaxResponseDto);
                }
            }
            if (UserModel.Uid != 0)
            {
                UserModelDB = userLoginService.GetUserInfoByID(UserModel.Uid);
                EmployeeMedicalModelDB = medicalDataService.GetEmployeeMedicalDataByUserid(UserModel.Uid);
                if (EmployeeMedicalModelDB != null)
                {
                    EmployeeMedicalModel.Id = EmployeeMedicalModelDB.Id;
                    //UserModel.EmployeeMedicalData.Id = EmployeeMedicalModelDB.Id;
                    //UserModel.EmployeeMedicalData.PremiumTotal = EmployeeMedicalModelDB.PremiumTotal != null ? EmployeeMedicalModelDB.PremiumTotal.Value : 0;
                    //UserModel.EmployeeMedicalData.PremiumPerMonth = EmployeeMedicalModelDB.PremiumPerMonth != null ? Math.Round(EmployeeMedicalModelDB.PremiumPerMonth.Value,2) : 0;
                    //UserModel.EmployeeMedicalData.TotalCoverage = EmployeeMedicalModelDB.TotalCoverage != null ? EmployeeMedicalModelDB.TotalCoverage.Value : 0;
                    //UserModel.EmployeeMedicalData.Validity = EmployeeMedicalModelDB.Validity != null ? EmployeeMedicalModelDB.Validity.Value : 0;
                }
            }
            else
            {
                List<UserLogin> UserModelDBList = new List<UserLogin>();
                var result = userLoginService.CheckUser(UserModel.EmailOffice, UserModel.UserName);
                if (result == "emailerror")
                {
                    ajaxResponseDto.Success = false;
                    ajaxResponseDto.Message = "Office Mail id already exists, please try another one.";
                    return Json(ajaxResponseDto);
                }
                else if (result == "usernameerror")
                {
                    ajaxResponseDto.Success = false;
                    ajaxResponseDto.Message = "User name already exists, please try another username.";
                    return Json(ajaxResponseDto);
                }

                //using (MD5 md5Hash = MD5.Create())
                //{
                //    var pass = string.IsNullOrEmpty(UserModel.Password) ? "" : UserModel.Password;
                //    string hash = GetMd5Hash(md5Hash, pass);
                //    UserModelDB.Password = pass;
                //    UserModelDB.PasswordKey = hash;
                //}
                string original = string.IsNullOrEmpty(UserModel.Password) ? "" : UserModel.Password;
                using (Rijndael myRijndael = Rijndael.Create())
                {
                    var keybytes = GetRiJndael_KeyBytes(SiteKey.Encryption_Key);
                    byte[] encrypted = EncryptStringToBytes(original, keybytes, keybytes);

                    //UserModelDB.Password = original;
                    UserModelDB.PasswordKey = Convert.ToBase64String(encrypted);

                }

            }
            string _RoleName = roleService.GetRoleById(UserModel.RoleId.Value).RoleName;
            if (string.IsNullOrWhiteSpace(_RoleName))
            {
                _RoleName = " ";
            }
            UserModel.JobTitle = !string.IsNullOrWhiteSpace(UserModel.JobTitle) ? UserModel.JobTitle : _RoleName;
            #region UserProfile Update

            UserModelDB.Title = UserModel.Title;
            UserModelDB.EmpCode = UserModel.EmployeeCode;
            UserModelDB.AttendenceId = UserModel.AttendanceId.HasValue ? UserModel.AttendanceId : null;
            UserModelDB.Gender = UserModel.Gender;
            UserModelDB.IsActive = UserModel.IsActive;
            UserModelDB.Name = UserModel.Name;
            UserModelDB.UserName = string.IsNullOrEmpty(UserModel.UserName) ? "" : UserModel.UserName;

            // UserModelDB.Password = string.IsNullOrEmpty(UserModel.Password) ? "" : UserModel.Password;

            UserModelDB.DeptId = UserModel.DeptId;
            UserModelDB.PMUid = UserModel.PMUid != null ? UserModel.PMUid : 0;
            UserModelDB.RoleId = UserModel.RoleId.Value;
            //UserModelDB.RoleCategoryId = UserModel.RoleCateGoryId.Value;
            UserModelDB.DesignationId = UserModel.DesignationId.Value;
            UserModelDB.JobTitle = !string.IsNullOrWhiteSpace(UserModel.JobTitle) ? UserModel.JobTitle : _RoleName;
            UserModelDB.DOB = UserModel.DOB.ToDateTime("dd/MM/yyyy");
            UserModelDB.JoinedDate = UserModel.JoinedDate.ToDateTime("dd/MM/yyyy") ?? UserModelDB.JoinedDate;
            UserModelDB.EmailOffice = UserModel.EmailOffice;
            UserModelDB.EmailPersonal = UserModel.EmailPersonal;
            UserModelDB.MobileNumber = UserModel.MobileNumber;
            UserModelDB.PhoneNumber = UserModel.PhoneNumber;
            UserModelDB.AlternativeNumber = UserModel.AlternativeNumber;
            UserModelDB.Address = UserModel.Address;
            UserModelDB.SkypeId = UserModel.SkypeId;
            UserModelDB.MarraigeDate = UserModel.MarraigeDate.ToDateTime("dd/MM/yyyy");
            UserModelDB.AadharNumber = UserModel.AadharNumber;
            UserModelDB.TLId = UserModel.TLId ?? 0;
            UserModelDB.IP = GeneralMethods.Getip();
            UserModelDB.AddDate = UserModel.AddDate != null ? UserModel.AddDate : DateTime.Now;
            UserModelDB.ModifyDate = DateTime.Now;
            UserModelDB.PassportNumber = (UserModel.PassportNumber ?? "").ToUpper();
            UserModelDB.PanNumber = (UserModel.PanNumber ?? "").ToUpper();
            UserModelDB.OtherTechnology = UserModel.OtherTechnology;
            if (UserModel.BloodGroupId != 0)
            {
                UserModelDB.BloodGroupId = UserModel.BloodGroupId;
            }
            else
            {
                UserModelDB.BloodGroupId = null;
            }

            UserModelDB.IsResigned = UserModel.IsResigned;

            #endregion
            var isUserTechDeleted = userLoginService.UserTechDeleted(UserModelDB);

            if (isUserTechDeleted)
            {
                if (UserModelDB.User_Tech != null)
                {
                    UserModelDB.User_Tech.Clear();
                }

                if (UserModel.TechnologyList.Any())
                {
                    var technologies = UserModel.TechnologyList.Where(x => x.SpecTypeId.HasValue);

                    foreach (var tech in technologies)
                    {
                        UserModelDB.User_Tech.Add(new User_Tech
                        {

                            TechId = tech.TechId,
                            SpecTypeId = tech.SpecTypeId,
                            Uid = UserModel.Uid
                        });
                    }
                }
            }


            var isUserDomainDeleted = userLoginService.UserDomainDeleted(UserModelDB);
            if (isUserDomainDeleted)
            {
                if (UserModelDB.DomainExperts != null)
                {
                    UserModelDB.DomainExperts.Clear();
                }

                if (UserModel.DomainExpert.Any())
                {
                    var domains = UserModel.DomainExpert;

                    foreach (var domain in domains)
                    {
                        UserModelDB.DomainExperts.Add(new DomainExperts
                        {

                            DomainId = domain.DomainId,
                            Uid = CurrentUser.Uid
                        });
                    }
                }
            }

            userLoginService.Save(UserModelDB);


            EmployeeMedicalModel.UserId = UserModelDB.Uid;
            EmployeeMedicalModel.EmployeeCode = !string.IsNullOrWhiteSpace(UserModel.EmployeeCode) ? UserModel.EmployeeCode : "";
            EmployeeMedicalModel.Title = UserModelDB.Title == "1" ? (byte)1 : (byte)2;
            EmployeeMedicalModel.Name = UserModelDB.Name;
            EmployeeMedicalModel.Gender = UserModelDB.Gender == "M" ? (byte)1 : (byte)2;
            EmployeeMedicalModel.Designation = UserModelDB.Designation.Name;
            EmployeeMedicalModel.Dob = UserModelDB.DOB.Value;
            EmployeeMedicalModel.AddedDate = DateTime.Now;
            EmployeeMedicalModel.IsActive = UserModelDB.IsActive.Value;

            EmployeeMedicalModel.ShowRelative = false;
            EmployeeMedicalModel.PremiumTotal = UserModel.EmployeeMedicalData.PremiumTotal;
            EmployeeMedicalModel.PremiumPerMonth = UserModel.EmployeeMedicalData.PremiumPerMonth;
            EmployeeMedicalModel.TotalCoverage = UserModel.EmployeeMedicalData.TotalCoverage;
            EmployeeMedicalModel.Validity = 1;
            if (EmployeeMedicalModelDB == null)
            {
                medicalDataService.Save(EmployeeMedicalModel, true);
            }
            else
            {
                medicalDataService.Save(EmployeeMedicalModel, false);
            }

            if (UserModel.Uid != 0)
            {
                UpdateUserHRM(UserModelDB);
            }

            string Message = string.Empty;

            if (UserModel.Uid == 0)
            {
                ajaxResponseDto.Success = true;
                Message = "Record has been added successfully.";
                ajaxResponseDto.Message = Message;
                ajaxResponseDto.Data = new { redirectUrl = Url.Action("ManageUser", "User") };
                return Json(ajaxResponseDto);
            }


            #region Update developer info in CRM

            if (UserModelDB.Uid != 0)
            {

                var crmResponse = UpdateDeveloperInfoInCRM(UserModelDB.Uid);
                if (crmResponse == null || !crmResponse.Status)
                {
                    var errorMessage = crmResponse?.Errors != null ? string.Join(", ", crmResponse.Errors) : crmResponse?.Message ?? "Some error while calling CRM API";
                    Message = $"Record has been updated successfully. But unable to update user info on CRM. Error: {errorMessage}";
                }

            }

            #endregion

            #region "PMS API Call"
            RequestUpdateMemberObject reqObj = new RequestUpdateMemberObject
            {
                Email = UserModel.EmailOffice,
                designationId = UserModel.DesignationId.Value,
                TeamManagerEmail = UserModel.PMUid != null ? userLoginService.GetUserInfoByID(UserModel.PMUid.Value).EmailOffice : "",
                ReportsToEmail = UserModel.TLId != null ? userLoginService.GetUserInfoByID(UserModel.TLId.Value).EmailOffice : ""
            };
            PMS_APICall(reqObj, fileLogPath);
            #endregion
            #region "HRM API Call"
            RequestUpdateMemberObjectHRM reqObjHRM = new RequestUpdateMemberObjectHRM
            {
                emailId = UserModel.EmailOffice,
                ems_designation_id = UserModel.DesignationId.Value.ToString(),
                ems_role_id = UserModel.RoleId.Value.ToString(),
                ems_category_id = UserModel.RoleCateGoryId.Value.ToString(),
                ActionType = "SetDesignation"
            };
            HRM_APICall(reqObjHRM, fileLogPath);
            #endregion
            #region "Call SARAL SP"
            if (SiteKey.IsSaralLive)
            {
                SARAL_SPCall(UserModelDB, fileLogPath);
            }
            #endregion
            ajaxResponseDto.Success = true;
            ajaxResponseDto.Message = Message == "" ? UserModel.Uid > 0 ? "Record has been updated successfully." : "Record has been added successfully." : Message;
            ajaxResponseDto.Data = new { redirectUrl = Url.Action("ManageUser", "User") };

            return Json(ajaxResponseDto);


        }
        //SARAL_SPCall
        private void SARAL_SPCall(UserLogin UserModelDB, string fileLogPath)
        {

            LogPrint.WriteIntoFile(fileLogPath, $"---------------------------SARAL Desigantion Update SP---------------------------");
            DateTime currentDate = DateTime.Now;
            if (UserModelDB.AttendenceId != null && UserModelDB.Designation != null)
            {
                if (UserModelDB.IsFromDbdt != null && UserModelDB.IsFromDbdt.Value == true)
                {
                    try
                    {
                        LogPrint.WriteIntoFile(fileLogPath, $"---------------------------SARAL DT---------------------------");
                        LogPrint.WriteIntoFile(fileLogPath, $"Request: {"AttendenceId=" + UserModelDB.AttendenceId.ToString() + "Name=" + UserModelDB.Name + "Designation" + UserModelDB.Designation.Name + "Email=" + UserModelDB.EmailOffice + "DateFrom=" + new DateTime(currentDate.Year, currentDate.Month, 1) + "DBNAME=DT"}");
                        //levDetailsDTService.SetDTUserDesignation(UserModelDB.AttendenceId.ToString(), UserModelDB.Name, UserModelDB.Designation.Name, UserModelDB.EmailOffice, new DateTime(currentDate.Year, currentDate.Month, 1), "DT");
                        levDetailsDTPLService.SetUserDesignation(UserModelDB.AttendenceId.ToString(), UserModelDB.Name, UserModelDB.Designation.Name, UserModelDB.EmailOffice, new DateTime(currentDate.Year, currentDate.Month, 1), "DT");
                        LogPrint.WriteIntoFile(fileLogPath, $"SARAL Data created on:{DateTime.Now}");
                        LogPrint.WriteIntoFile(fileLogPath, "");
                    }
                    catch (Exception ex)
                    {
                        LogPrint.WriteIntoFile(fileLogPath, $"SARAL Data created on:{DateTime.Now}");
                        LogPrint.WriteIntoFile(fileLogPath, $"Error Message:{ex.Message}");
                        LogPrint.WriteIntoFile(fileLogPath, "");
                    }
                }
                else
                {
                    try
                    {
                        LogPrint.WriteIntoFile(fileLogPath, $"---------------------------SARAL DTPL---------------------------");
                        LogPrint.WriteIntoFile(fileLogPath, $"Request: {"AttendenceId=" + UserModelDB.AttendenceId.ToString() + "Name=" + UserModelDB.Name + "Designation" + UserModelDB.Designation.Name + "Email=" + UserModelDB.EmailOffice + "DateFrom=" + new DateTime(currentDate.Year, currentDate.Month, 1) + "DBNAME=DTPL"}");
                        levDetailsDTPLService.SetUserDesignation(UserModelDB.AttendenceId.ToString(), UserModelDB.Name, UserModelDB.Designation.Name, UserModelDB.EmailOffice, new DateTime(currentDate.Year, currentDate.Month, 1), "DTPL");
                        LogPrint.WriteIntoFile(fileLogPath, $"SARAL Data created on:{DateTime.Now}");
                        LogPrint.WriteIntoFile(fileLogPath, "");
                    }
                    catch (Exception ex)
                    {
                        LogPrint.WriteIntoFile(fileLogPath, $"SARAL Data created on:{DateTime.Now}");
                        LogPrint.WriteIntoFile(fileLogPath, $"Error Message:{ex.Message}");
                        LogPrint.WriteIntoFile(fileLogPath, "");
                    }
                }
            }
            else
            {
                LogPrint.WriteIntoFile(fileLogPath, $"{"AttendenceId=" + UserModelDB.AttendenceId + "Name=" + UserModelDB.Name + "Email=" + UserModelDB.EmailOffice + "Designation=" + JsonConvert.SerializeObject(UserModelDB.Designation)}");
                LogPrint.WriteIntoFile(fileLogPath, $"SARAL Update failed on :{DateTime.Now}");
                LogPrint.WriteIntoFile(fileLogPath, "");
            }
        }
        //PMS API Call
        private bool PMS_APICall(RequestUpdateMemberObject request, string fileLogPath)
        {
            string url = SiteKey.PMSUserUpdateServiceApiURL + "/UpdateMember";
            bool apiResult = false;
            using (HttpClient client = new HttpClient())
            {
                LogPrint.WriteIntoFile(fileLogPath, $"---------------------------PMS Desigantion Update API---------------------------");
                LogPrint.WriteIntoFile(fileLogPath, $"Request: {JsonConvert.SerializeObject(request)}");
                try
                {
                    var content = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");
                    content.Headers.Add("ApiKey", SiteKey.PMSApiKey);
                    content.Headers.Add("ApiPassword", SiteKey.PMSApiPassword);
                    var result = client.PostAsync(url, content).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var jsonres = result.Content.ReadAsStringAsync().Result;
                        LogPrint.WriteIntoFile(fileLogPath, $"EMS Data created on:{DateTime.Now}");
                        LogPrint.WriteIntoFile(fileLogPath, $"Response:{jsonres}");
                        LogPrint.WriteIntoFile(fileLogPath, "");
                        apiResult = true;
                    }
                    else
                    {
                        LogPrint.WriteIntoFile(fileLogPath, $"EMS Data created on:{DateTime.Now}");
                        LogPrint.WriteIntoFile(fileLogPath, $"Response: {result.ToString()}");
                        LogPrint.WriteIntoFile(fileLogPath, "");
                    }
                }
                catch (Exception ex)
                {

                    LogPrint.WriteIntoFile(fileLogPath, $"EMS Data created on:{DateTime.Now}");
                    LogPrint.WriteIntoFile(fileLogPath, $"Error Message:{ex.Message}");
                    LogPrint.WriteIntoFile(fileLogPath, "");
                }
            }
            return apiResult;
        }
        //HRM API Call
        private bool HRM_APICall(RequestUpdateMemberObjectHRM request, string fileLogPath)
        {
            string url = SiteKey.HrmServiceURL + "/setEmpDesignation";
            bool apiResult = false;
            using (HttpClient client = new HttpClient())
            {
                LogPrint.WriteIntoFile(fileLogPath, $"---------------------------HRM Desigantion Update API---------------------------");
                LogPrint.WriteIntoFile(fileLogPath, $"Request: {JsonConvert.SerializeObject(request)}");
                try
                {
                    var content = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");

                    content.Headers.Add("Hrmapikey", SiteKey.HrmApiKey);
                    content.Headers.Add("Hrmapipassword", SiteKey.HrmApiPassword);
                    var result = client.PostAsync(url, content).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var jsonres = result.Content.ReadAsStringAsync().Result;
                        
                        LogPrint.WriteIntoFile(fileLogPath, $"EMS Data created on:{DateTime.Now}");
                        LogPrint.WriteIntoFile(fileLogPath, $"Response:{jsonres}");
                        LogPrint.WriteIntoFile(fileLogPath, "");
                        apiResult = true;
                    }
                    else
                    {                        
                        LogPrint.WriteIntoFile(fileLogPath, $"EMS Data created on:{DateTime.Now}");
                        LogPrint.WriteIntoFile(fileLogPath, $"Response: {result.ToString()}");
                        LogPrint.WriteIntoFile(fileLogPath, "");
                    }
                }
                catch (Exception ex)
                {                    
                    LogPrint.WriteIntoFile(fileLogPath, $"EMS Data created on:{DateTime.Now}");
                    LogPrint.WriteIntoFile(fileLogPath, $"Error Message:{ex.Message}");
                    LogPrint.WriteIntoFile(fileLogPath, "");
                }
            }
            return apiResult;
        }
        [HttpGet]
        public ActionResult BindTeamLead(int PMId)
        {
            var pmUser = userLoginService.GetUserInfoByID(PMId);
            if (pmUser.RoleId == ((int)Enums.UserRoles.HRBP))
            {
                List<UserLogin> ddlteamLead = userLoginService.GetUserByRole((int)Enums.UserRoles.HRBP, true).Distinct().OrderBy(T => T.Name).Where(P => P.PMUid == PMId || P.Uid == PMId).ToList();
                var ddlteamLeadList = ddlteamLead.Select(x => new SelectListItem() { Text = x.Name, Value = x.Uid.ToString() }).ToList();
                ddlteamLeadList.Insert(0, new SelectListItem() { Text = "-Select-", Value = "" });
                var response = new
                {
                    TeamLeadList = ddlteamLeadList,
                };
                return Json(response);
            }
            else
            {
                var ddlteamLead = userLoginService.GetTLSDUsers(PMId);
                var ddlteamLeadList = ddlteamLead.Select(x => new SelectListItem() { Text = x.Name, Value = x.Uid.ToString() }).ToList();
                ddlteamLeadList.Insert(0, new SelectListItem() { Text = "-Select-", Value = "" });
                var response = new
                {
                    TeamLeadList = ddlteamLeadList,
                };
                return Json(response);
            }
        }

        [HttpPost]
        public ActionResult CheckLoginUser()
        {
            AjaxResponseDto ajaxResponseDto = new AjaxResponseDto();
            if (CurrentUser == null)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = true, RedirectUrl = Url.Action("login") });
            }
            else
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = false, RedirectUrl = Url.Action("") });
            }

        }
        #endregion

        [HttpGet]
        public ActionResult Referfriend()
        {
            UserProfileDto userProfileDto = new UserProfileDto();
            UserLogin userLogin = userLoginService.GetUserInfoByID(CurrentUser.Uid);
            userProfileDto.EmployeeCode = userLogin?.EmpCode ?? "";
            return View(userProfileDto);
        }

        [HttpGet]
        public IActionResult ShowUserPassword(int userId)
        {
            ShowPasswordDto Userpass = new ShowPasswordDto();
            if (userId > 0)
            {
                UserLogin user = new UserLogin();
                user = userLoginService.GetUsersById(userId);
                var keybytes = GetRiJndael_KeyBytes(SiteKey.Encryption_Key);
                byte[] TempEncrypted = Encoding.UTF8.GetBytes(user.PasswordKey);
                byte[] TempmyRijndaelKey2 = Encoding.UTF8.GetBytes(Convert.ToBase64String(keybytes));
                byte[] encrypted = Convert.FromBase64String(Encoding.ASCII.GetString(TempEncrypted));
                byte[] myRijndaelKey = Convert.FromBase64String(Encoding.ASCII.GetString(TempmyRijndaelKey2));
                Userpass.UserId = user.Uid;
                Userpass.OriginalPassword = DecryptStringFromBytes(encrypted, myRijndaelKey, myRijndaelKey);
            }
            return PartialView("_ShowPassowrd", Userpass);
        }

        [HttpGet]
        public IActionResult ChangeUserPassword(int userId)
        {
            ChangeUserPasswordDto changeUserPasswordModel = new ChangeUserPasswordDto();
            if (userId > 0)
            {
                UserLogin user = new UserLogin();
                user = userLoginService.GetUsersById(userId);

                changeUserPasswordModel.UserId = user.Uid;
            }
            return PartialView("_ChangePassowrd", changeUserPasswordModel);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ChangeUserPassword(ChangeUserPasswordDto changeUserPasswordViewModel)
        {

            if (ModelState.IsValid)
            {
                if (CurrentUser != null && CurrentUser.Uid > 0)
                {

                    UserLogin userLogin = userLoginService.GetUserInfoByID(changeUserPasswordViewModel.UserId);
                    using (Rijndael myRijndael = Rijndael.Create())
                    {
                        var keybytes = GetRiJndael_KeyBytes(SiteKey.Encryption_Key);

                        string source = changeUserPasswordViewModel.NewPassword.Trim();
                        string hash = Convert.ToBase64String(EncryptStringToBytes(source, keybytes, keybytes));

                        //userLogin.Password = source;
                        userLogin.PasswordKey = hash;
                        userLoginService.ChangePassword(userLogin);
                        ShowSuccessMessage("Success", "Password has been successfully updated", false);

                    }


                    //userLoginService.ChangePassword(userLogin);
                }
                return RedirectToAction("ManageUser"); //Json(new { isSuccess = true, redirectUrl = Url.Action("ManageUser", "User") });
            }
            else
            {
                ShowErrorMessage("Failed", "Server side validation failed", false);
                return RedirectToAction("ManageUser");
            }

        }
        //[HttpPost]
        //public ActionResult Referfriend(UserProfileDto userProfileDto)
        //{
        //    string Url = "http://ds19.projectstatus.co.uk/dsrecruitment?empCode=" + userProfileDto?.EmployeeCode ?? null;
        //    return Redirect(Url);
        //}

        [HttpGet]
        public ActionResult SetEncryptPass()
        {
            if (SiteKey.AshishTeamPMUId != CurrentUser.Uid)
            {
                return AccessDenied();
            }
            return View();
        }

        [HttpPost]
        public ActionResult SetEncryptPassword()
        {
            if (SiteKey.AshishTeamPMUId != CurrentUser.Uid)
            {
                return AccessDenied();
            }

            return Json(ChangePasswordAllUsers());
        }


        [HttpGet]
        public IActionResult DownloadPfDocument(int id)
        {
            var user = userLoginService.GetUsersById(id);
            var files = userLoginService.GetPfDocumentFile(id);
            byte[] archiveFile;
            using (var archiveStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
                {
                    foreach (var imageName in files)
                    {
                        var filePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath, $"Images//profile//{imageName}");
                        var bytes = FileToByteArray(filePath);
                        var zipArchiveEntry = archive.CreateEntry(imageName, CompressionLevel.Fastest);

                        using (var zipStream = zipArchiveEntry.Open())
                            zipStream.Write(bytes, 0, bytes.Length);
                    }
                }

                archiveFile = archiveStream.ToArray();
            }

            return File(archiveFile, "application/zip", $"{user.Name.Replace(" ", "_")}_{(!string.IsNullOrEmpty(user.EmpCode) ? user.EmpCode : id.ToString())}.zip");
        }

        private byte[] FileToByteArray(string fileName)
        {
            byte[] fileContent = null;
            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
            long byteLength = new System.IO.FileInfo(fileName).Length;
            fileContent = binaryReader.ReadBytes((Int32)byteLength);
            fs.Close();
            fs.Dispose();
            binaryReader.Close();
            return fileContent;
        }


        [HttpGet]
        public ActionResult BindRoleList(int? RoleCateGoryId)
        {
            var ddlRole = userLoginService.GetRolesByRoleCategoyId(RoleCateGoryId ?? 0);
            var ddlRoleList = ddlRole.Select(x => new SelectListItem() { Text = x.RoleName, Value = x.RoleId.ToString() }).ToList();
            ddlRoleList.Insert(0, new SelectListItem() { Text = "-Select-", Value = "" });

            var response = new
            {
                RoleList = ddlRoleList,
            };
            return Json(response);

        }

        [HttpGet]
        public ActionResult BindDesignationList(int? RoleId)
        {
            var ddlDesignation = userLoginService.GetDesignationByRoleId(RoleId ?? 0);
            var ddlDesignationList = ddlDesignation.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            ddlDesignationList.Insert(0, new SelectListItem() { Text = "-Select-", Value = "" });

            var response = new
            {
                DesignationList = ddlDesignationList,
            };
            return Json(response);

        }
    }
}