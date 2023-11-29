using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using EMS.Core;

namespace EMS.Web.Code.LIBS
{
    public class CustomPrincipal
    {
        private readonly ClaimsPrincipal claimsPrincipal;

        public CustomPrincipal(ClaimsPrincipal principal)
        {
            claimsPrincipal = principal;

            this.IsAuthenticated = claimsPrincipal.Identity.IsAuthenticated;

            if (this.IsAuthenticated)
            {
                this.Uid = int.Parse(claimsPrincipal.Claims.FirstOrDefault(u => u.Type == ClaimTypes.PrimarySid)?.Value);
                this.Name = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name)?.Value;
                this.UserName = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(this.UserName))?.Value;
                this.JobTitle = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(this.JobTitle))?.Value;
                this.EmailOffice = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(this.EmailOffice))?.Value;
                this.MobileNumber = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(this.MobileNumber))?.Value;
                this.ApiPassword = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(this.ApiPassword))?.Value;
                this.TLId = int.Parse(claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(this.TLId))?.Value);
                this.CRMUserId = int.Parse(claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(this.CRMUserId))?.Value);
                this.IsSuperAdmin = bool.Parse(claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(this.IsSuperAdmin))?.Value);
                this.IsSPEG = bool.Parse(claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(this.IsSPEG))?.Value);
                this.DeptId = int.Parse(claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(this.DeptId))?.Value);
                this.RoleId = int.Parse(claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(this.RoleId))?.Value);
                this.DesignationId = int.Parse(claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(this.DesignationId))?.Value);
                this.DesignationName = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(this.DesignationName))?.Value;
                this.IsSPEG = bool.Parse(claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(this.IsSPEG))?.Value);
                this.AttendenceId = int.Parse(claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(this.AttendenceId))?.Value);
                this.Gender = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(this.Gender))?.Value;
                this.NotificationMinute = int.Parse(claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(this.NotificationMinute))?.Value);
                //this.MarriageDate = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(this.MarriageDate))?.Value;

                //this._pmUid = int.Parse(claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(this.PMUid))?.Value);
                if (RoleId == (int)Enums.UserRoles.PM || RoleId == (int)Enums.UserRoles.PMO)
                {
                    _pmUid = int.Parse(claimsPrincipal.Claims.FirstOrDefault(u => u.Type == ClaimTypes.PrimarySid)?.Value);

                }
                else
                {
                    _pmUid = int.Parse(claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(PMUid))?.Value);
                }
            }
        }

        public bool IsAuthenticated { get; private set; }        
        public int Uid { get; private set; }
        public string UserName { get; private set; }
        public string Name { get; private set; }
        public string JobTitle { get; private set; }
        public int DeptId { get; private set; }
        public int RoleId { get; private set; }
        public int DesignationId { get; private set; }
        public string DesignationName { get; private set; }
        public double NotificationMinute { get; private set; }


        public int TLId { get; private set; }
        public string EmailOffice { get; private set; }
        public string MobileNumber { get; private set; }
        public string Gender { get; private set; }
        public string MarriageDate { get; private set; }

        public int CRMUserId { get; private set; }
        public int AttendenceId { get; private set; }
        public string ApiPassword { get; private set; }
        public bool IsSuperAdmin { get; private set; }
        public bool IsSPEG { get; private set; }
        //  public int PMUid { get; private set; }
        private int _pmUid;

        public int PMUid
        {
            get { return _pmUid; }
            set
            {
                UpdateClaim(nameof(PMUid), value.ToString());
                _pmUid = value;
            }
        }

        private void UpdateClaim(string key, string value)
        {
            var claims = claimsPrincipal.Claims.ToList();
            if (claims.Any())
            {
                var pmClaim = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == key);
                if (pmClaim != null)
                {
                    claims.Remove(pmClaim);
                    claims.Add(new Claim(key, value));
                }
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };
            ContextProvider.HttpContext.SignInAsync(
                  CookieAuthenticationDefaults.AuthenticationScheme,
                   new ClaimsPrincipal(claimsIdentity),
                   authProperties
                 ).Wait();
        }
    }
}