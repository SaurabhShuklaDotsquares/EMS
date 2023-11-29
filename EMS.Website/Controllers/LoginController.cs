using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.LIBS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
//using static EMS.Core.MD5Encryption;
using static EMS.Core.Encryption;
using System.Security.Cryptography;
using System.IO;
using EMS.Website.LIBS;

namespace EMS.Web.Controllers
{
    public class LoginController : BaseController
    {
        #region "Fields and Constructor"

        private readonly ILoginService userLoginService;
        private readonly IMenuService menuService;
        private readonly IUserActivityService userActivityService;
        private readonly IProjectService projectService;
        private readonly IProjectClosureService projectClosureService;
        private readonly ILeaveService _leaveService;

        public LoginController(ILoginService _userMasterService, IMenuService _menuService, IUserActivityService _userActivityService,
                                IProjectService _projectService, IProjectClosureService _projectClosureService, ILeaveService leaveService)
        {
            this.userLoginService = _userMasterService;
            this.menuService = _menuService;
            this.userActivityService = _userActivityService;
            this.projectService = _projectService;
            this.projectClosureService = _projectClosureService;
            _leaveService = leaveService;
        }
        #endregion

        #region Index

        [HttpGet]
        public ActionResult Index(string returnurl)
        {
            LoginDto loginDto = new LoginDto();
            if (CurrentUser.IsAuthenticated && Request.Cookies["UserSessionCookies"] != null)
            {
                if (!string.IsNullOrEmpty(returnurl))
                    Response.Redirect(returnurl);

                Response.Redirect(SiteKey.DomainName + "home");
            }
            return View(new LoginDto { ReturnUrl = returnurl });
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginDto model)
        {
            if (ModelState.IsValid)
            {
                #region encryption (this section used for Phase 2 for password Encryption)
                // chnages for using to login by Hash key
                var source = model.Password;
                var userDetail = userLoginService.GetLoginDeatilByUserNameOREmail(model.Email);
                if (userDetail != null)
                {
                    if (userDetail.IsActive == false)
                    {
                        ViewBag.Invalid = "Your account is temporarily  deactivated. Please contact to admin.";
                        return View();
                    }

                    var keybytes = GetRiJndael_KeyBytes(SiteKey.Encryption_Key);
                    byte[] encrypted = EncryptStringToBytes(source, keybytes, keybytes);

                    ////****Login with hash and make password null as per new requerenment
                    //if(!string.IsNullOrEmpty(userDetail.Password)&&userDetail.PasswordKey==null)
                    //{
                    //   // string source = userDetail.Password.Trim();
                    //    string hash = Convert.ToBase64String(EncryptStringToBytes(source, keybytes, keybytes));
                    //    userDetail.PasswordKey = hash;
                    //    userLoginService.ChangePassword(userDetail);
                    //}
                    //**************************************/
                    // add (userDetail.password  == Convert.ToBase64String(encrypted)) for current password user

                    //*************************************//

                    
                    if ((userDetail.PasswordKey == Convert.ToBase64String(encrypted))) //string.IsNullOrEmpty(userDetail.PasswordKey) && (userDetail.Password == source) || 
                    {                        
                        var cookieOptions = new CookieOptions { Expires = DateTime.Now.AddMinutes(10) };
                        if (model.RememberMe)
                        {
                            Response.Cookies.Append("UserName", model.Email.Trim(), cookieOptions);
                            Response.Cookies.Append("Password", model.Password, cookieOptions);
                        }
                        else
                        {
                            cookieOptions.Expires = DateTime.Now.AddDays(-1);
                            Response.Cookies.Append("UserName", string.Empty, cookieOptions);
                            Response.Cookies.Append("Password", string.Empty, cookieOptions);
                        }
                        double _notificationMinute = 0;
                        if (userDetail.PMUid != null)
                        {
                            var _prefereces = _leaveService.GetPreferecesByPMUid(userDetail.PMUid.Value);
                            if (_prefereces.ReviewNotificationMinutes != null)
                            {
                                int Minutes = _prefereces.ReviewNotificationMinutes.Value;
                                _notificationMinute = 60 * Minutes * 1000;
                            }
                        }
                        
                        var userDto = new UserSessionDto
                        {
                            Uid = userDetail.Uid,
                            UserName = userDetail.UserName,
                            Name = userDetail.Name,
                            JobTitle = userDetail.JobTitle,
                            ApiPassword = !string.IsNullOrEmpty(userDetail.ApiPassword) ? userDetail.ApiPassword : string.Empty,
                            IsSuperAdmin = userDetail.IsSuperAdmin ?? false,
                            EmailOffice = userDetail.EmailOffice,
                            MobileNumber = !string.IsNullOrEmpty(userDetail.MobileNumber) ? userDetail.MobileNumber : string.Empty,
                            IsSPEG = userDetail.IsSPEG,
                            DeptId = userDetail.DeptId ?? 0,
                            RoleId = userDetail.RoleId ?? 0,
                            TLId = userDetail.TLId ?? 0,
                            PMUid = userDetail.PMUid ?? 0,
                            CRMUserId = userDetail.CRMUserId ?? 0,
                            AttendenceId = userDetail.AttendenceId ?? 0,
                            Gender = userDetail.Gender ?? string.Empty,
                            DesignationId = userDetail.DesignationId ?? 0,
                            DesignationName = userDetail.Designation.Name ?? string.Empty,
                            NotificationMinute = _notificationMinute
                        };
                        
                        await CreateAuthenticationTicket(userDto);
                        
                        // User activity
                        UserActivity userActivity = userActivityService.GetUserActivityByUid(userDetail.Uid);
                        
                        if (userActivity != null)
                        {
                            
                            var dictProjectStatus = WebsiteSession.SessionProjectStatus;

                            dictProjectStatus.AddOrReplace("ProjectId", userActivity.ProjectId.HasValue ? userActivity.ProjectId.Value.ToString() : string.Empty);
                            dictProjectStatus.AddOrReplace("ProjectName", userActivity.ProjectName);
                            dictProjectStatus.AddOrReplace("UserStatus", userActivity.Status);
                            dictProjectStatus.AddOrReplace("FreeText", userActivity.Comment);

                            WebsiteSession.UpdateSessionProjectStatus(dictProjectStatus);
                            
                        }
                        else
                        {
                            WebsiteSession.RemoveProjectStatusSession();
                        }

                        if (!string.IsNullOrEmpty(model.ReturnUrl))
                        {                            
                            Response.Redirect(model.ReturnUrl);
                        }
                        else
                        {                            
                            Response.Redirect(SiteKey.DomainName + "home");
                        }
                    }
                }

                //    var userDto = new UserSessionDto
                //    {
                //        Uid = userDetail.Uid,
                //        UserName = userDetail.UserName,
                //        Name = userDetail.Name,
                //        JobTitle = userDetail.JobTitle,
                //        ApiPassword = !string.IsNullOrEmpty(userDetail.ApiPassword) ? userDetail.ApiPassword : string.Empty,
                //        IsSuperAdmin = userDetail.IsSuperAdmin ?? false,
                //        EmailOffice = userDetail.EmailOffice,
                //        MobileNumber = !string.IsNullOrEmpty(userDetail.MobileNumber) ? userDetail.MobileNumber : string.Empty,
                //        IsSPEG = userDetail.IsSPEG,
                //        DeptId = userDetail.DeptId ?? 0,
                //        RoleId = userDetail.RoleId ?? 0,
                //        TLId = userDetail.TLId ?? 0,
                //        PMUid = userDetail.PMUid ?? 0,
                //        CRMUserId = userDetail.CRMUserId ?? 0,
                //    };

                //    await CreateAuthenticationTicket(userDto);
                //    // User activity
                //    UserActivity userActivity = userActivityService.GetUserActivityByUid(userDetail.Uid);
                //    if (userActivity != null)
                //    {
                //        var dictProjectStatus = WebsiteSession.SessionProjectStatus;

                //        dictProjectStatus.AddOrReplace("ProjectId", userActivity.ProjectId.HasValue ? userActivity.ProjectId.Value.ToString() : string.Empty);
                //        dictProjectStatus.AddOrReplace("ProjectName", userActivity.ProjectName);
                //        dictProjectStatus.AddOrReplace("UserStatus", userActivity.Status);
                //        dictProjectStatus.AddOrReplace("FreeText", userActivity.Comment);

                //        WebsiteSession.UpdateSessionProjectStatus(dictProjectStatus);
                //    }

                //    if (!string.IsNullOrEmpty(model.ReturnUrl))
                //        Response.Redirect(model.ReturnUrl);

                //    Response.Redirect(SiteKey.DomainName + "home");
                //}
                #endregion
            }
            ViewBag.Invalid = "UserName or Password do not match.";
            return View();
        }

        #endregion

        #region Forgot Password

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            ForgotPasswordDto model = new ForgotPasswordDto();
            return View(model);
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordDto model)
        {
            if (model != null && model.Email != null && model.Email != "")
            {
                try
                {
                    var user = userLoginService.GetLoginDeatilByEmail(model.Email);
                    if (user == null)
                    {
                        ViewBag.Invalid = "Invalid email";
                        model.Email = "";
                        return View();
                    }
                    var keybytes = GetRiJndael_KeyBytes(SiteKey.Encryption_Key);
                    string Decrypted = user.PasswordKey;
                    if (user.PasswordKey != null)
                    {
                        byte[] TempEncrypted = Encoding.UTF8.GetBytes(user.PasswordKey);
                        byte[] TempmyRijndaelKey2 = Encoding.UTF8.GetBytes(Convert.ToBase64String(keybytes));

                        byte[] encrypted = Convert.FromBase64String(Encoding.ASCII.GetString(TempEncrypted));
                        byte[] myRijndaelKey = Convert.FromBase64String(Encoding.ASCII.GetString(TempmyRijndaelKey2));

                        Decrypted = DecryptStringFromBytes(encrypted, myRijndaelKey, myRijndaelKey);
                    }
                    //                    string RandomOtp = GenerateaRandomPass();
                    ForgetEmail(user.Name, user.EmailOffice, user.UserName, Decrypted);

                    //using (MD5 md5Hash = MD5.Create())
                    //{
                    //    var pass = RandomOtp;
                    //    string hash = GetMd5Hash(md5Hash, pass);
                    //    user.Password = RandomOtp;
                    //    user.PasswordKey = hash;
                    //}
                    //userLoginService.save(user);
                    ViewBag.Invalid = "Password has been sent your email. ";
                    return View();
                }
                catch (Exception ex)
                {
                    return View(model);
                }
            }

            return View(model);
        }

        #endregion

        #region Private Method

        private string GenerateaRandomPass()
        {
            string[] allowchar = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", };
            int OtpLength = 6;
            string OTP = string.Empty;
            //string sTempChar = string.Empty;
            Random rand = new Random();
            for (int i = 0; i < OtpLength; i++)
            {
                int p = rand.Next(0, allowchar.Length);
                OTP += allowchar[rand.Next(0, allowchar.Length)];
            }
            return OTP;
        }
        private static void ForgetEmail(string Name, string EmailTo, string UserName, string Password)
        {
            StringBuilder BodyContent = new StringBuilder();
            BodyContent.Append("<table style='width:100%;font-family:Arial; font-size:13px;' cellpadding='0' cellspacing='0'>");
            BodyContent.Append("<tr><td>Dear " + Name + ",<br /></td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");

            BodyContent.Append("<tr><td>Below are your credentials-</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td><b>Username: </b>" + UserName + "</td></tr>");
            BodyContent.Append("<tr><td><b>Password: </b>" + Password + "</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td>Kindly change the password once you login.</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");

            BodyContent.Append("<tr><td>Thanks & Regards</td></tr>");
            BodyContent.Append("<tr><td><b>System Administrator</b></td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td><span style='font-weight:bold; font-family:Arial; font-size:11px; font-style:italic; color:gray;'>[Note : Please do not reply to this automated message; replies to this message are undeliverable. Contact your senior for any query.]</span></td></tr>");
            BodyContent.Append("</table>");

            FlexiMail objMail = new FlexiMail
            {
                From = SiteKey.From,
                To = EmailTo,
                CC = "",
                BCC = "",
                Subject = "Forgot Password",
                MailBodyManualSupply = true,
                MailBody = BodyContent.ToString()
            };

            objMail.Send();
        }


        #endregion

        /// <summary>
        /// For Get password key list and insert the password in password key 
        /// </summary>
        public void GetUserLoginsPassword()
        {
            var users = userLoginService.GetPasswordKeyList();
            if (users != null && users.Count > 0)
            {

                foreach (var user in users)
                {
                    if (!string.IsNullOrEmpty(user.PasswordBackUp))
                    {
                        string source = user.PasswordBackUp.Trim();
                        var keybytes = GetRiJndael_KeyBytes(SiteKey.Encryption_Key);
                        string hash = Convert.ToBase64String(EncryptStringToBytes(source, keybytes, keybytes));
                        user.PasswordKey = hash;
                    }

                    //  userLoginService.Updatepasswordkey(users);
                }
                userLoginService.Updatepasswordkey(users);

            }
        }

    }
}