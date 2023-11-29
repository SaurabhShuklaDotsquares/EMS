
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.LIBS;
using Microsoft.AspNetCore.Mvc;
using System;
using static EMS.Core.Encryption;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMS.Web.Code.Attributes;
using System.Threading.Tasks;
using EMS.Web.Controllers;

namespace EMS.Website.Controllers
{
   
    public class SecretController : BaseController
    {
        private readonly IUserLoginService userLoginService;
        private bool IfAshishTeamPMUId { get { return (CurrentUser.PMUid == SiteKey.AshishTeamPMUId || CurrentUser.Uid == SiteKey.AshishTeamPMUId) ? true : false; } }
        public SecretController(IUserLoginService _userLoginService)
        {
            userLoginService = _userLoginService;
        }

        [HttpGet]
        [CustomAuthorization(IsAshishTeam: true)]
        public ActionResult Index()
        {
            if(CurrentUser.Uid!= SiteKey.AshishTeamPMUId)
            {
               return AccessDenied();
            }
            else
            {            
            return View();
            }
        }

        [HttpPost]
        [CustomAuthorization(IsAshishTeam: true)]
        public JsonResult ShowSecretUserPassword(int userId)
        {

            ShowPasswordDto Userpass = new ShowPasswordDto();
            if (userId > 0)
            {
                UserLogin user = new UserLogin();
                user = userLoginService.GetUsersById(userId);
                if (user != null && !string.IsNullOrEmpty(user.PasswordKey))
                {
                    var keybytes = GetRiJndael_KeyBytes(SiteKey.Encryption_Key);
                    byte[] TempEncrypted = Encoding.UTF8.GetBytes(user.PasswordKey);
                    byte[] TempmyRijndaelKey2 = Encoding.UTF8.GetBytes(Convert.ToBase64String(keybytes));
                    byte[] encrypted = Convert.FromBase64String(Encoding.ASCII.GetString(TempEncrypted));
                    byte[] myRijndaelKey = Convert.FromBase64String(Encoding.ASCII.GetString(TempmyRijndaelKey2));
                    Userpass.UserId = user.Uid;
                    Userpass.OriginalPassword = DecryptStringFromBytes(encrypted, myRijndaelKey, myRijndaelKey);
                }
                else
                {
                    Userpass.UserId = userId;
                    Userpass.OriginalPassword = "user not found !!";
                }
            }
            return Json(new { data = Userpass });
          //RedirectToAction("Index", Userpass.UserId);
        }



    }
}
