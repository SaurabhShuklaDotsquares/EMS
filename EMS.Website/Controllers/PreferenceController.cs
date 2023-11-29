using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Web.Mvc;

namespace EMS.Web.Controllers
{
    public class PreferenceController : BaseController
    {
        #region "Fields"
        private IPreferenceService preferenceService;
        #endregion

        #region "Constructor"
        public PreferenceController(IPreferenceService preferenceService)
        {
            this.preferenceService = preferenceService;

        }
        #endregion

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Index()
        {
            PreferenceDto model = new PreferenceDto();
            Preference objPreference = preferenceService.GetDataByPmid(PMUserId);
            if (objPreference != null)
            {
                model.PriorLeaveDay = objPreference.PriorLeaveDay;
                model.ActivityRefreshTime = objPreference.ActivityRefreshTime;
                model.ELActTimeLimit = objPreference.ELActTimeLimit;
                model.TimeSheetDay = objPreference.TimeSheetDay;
                model.EmailFrom = objPreference.EmailFrom;
                model.EmailPM = objPreference.EmailPM;
                model.EmailHR = objPreference.EmailHR;
                model.EmailDeveloper = objPreference.EmailDeveloper;
                model.TimeSheetEmail = objPreference.TimeSheetEmail;
                model.ProjectClosureEmail = objPreference.ProjectClosureEmail;
                model.SeniorDeveloperDocLink = objPreference.InductionDoc.HasValue() ?
                    !String.IsNullOrEmpty(objPreference.InductionDoc.Split(',')[0]) ? SiteKey.DomainName + "Content/Induction/" + objPreference.InductionDoc.Split(',')[0] : "" : "";
                model.DeveloperDocLink = !String.IsNullOrEmpty(objPreference.InductionDoc) ?
                    !String.IsNullOrEmpty(objPreference.InductionDoc.Split(',')[1]) ? SiteKey.DomainName + "Content/Induction/" + objPreference.InductionDoc.Split(',')[1] : "" : "";
                model.IsAllowLeave = objPreference.IsAllowLeaveByTL;
                model.IsAllowWFH = objPreference.IsAllowWFHByTL;
            }

            return View(model);
        }


        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult Index(PreferenceDto model)
        {
            try
            {

                //string Path = Server.MapPath("~/Content/Induction/");
                string path = Path.Combine(ContextProvider.HostEnvironment.WebRootPath, "~/Content/Induction/");
                //string FileName = ",";
                string DevFile = string.Empty;
                string SrFile = string.Empty;
                int pmuid = Convert.ToInt32(CurrentUser.Uid);
                Preference objPrefernce = preferenceService.GetDataByPmid(pmuid);
                if (objPrefernce != null)
                {
                    SrFile = !String.IsNullOrEmpty(objPrefernce.InductionDoc) ? !String.IsNullOrEmpty(objPrefernce.InductionDoc.Split(',')[0]) ? objPrefernce.InductionDoc.Split(',')[0] : "" : "";
                    DevFile = !String.IsNullOrEmpty(objPrefernce.InductionDoc) ? !String.IsNullOrEmpty(objPrefernce.InductionDoc.Split(',')[1]) ? objPrefernce.InductionDoc.Split(',')[1] : "" : "";
                }
                else
                {
                    objPrefernce = new Preference();
                    objPrefernce.pmid = pmuid;
                }

                objPrefernce.PriorLeaveDay = model.PriorLeaveDay;
                objPrefernce.ActivityRefreshTime = model.ActivityRefreshTime == null ? 100 : model.ActivityRefreshTime;
                objPrefernce.ELActTimeLimit = model.ELActTimeLimit == null ? 10 : model.ELActTimeLimit; 
                objPrefernce.TimeSheetDay = model.TimeSheetDay == null ? 10 : model.TimeSheetDay;
                objPrefernce.EmailFrom = !string.IsNullOrEmpty(model.EmailFrom) ? model.EmailFrom.Trim(';') : "no-reply@dotsquares.com";
                objPrefernce.EmailPM = !string.IsNullOrEmpty(model.EmailPM) ? model.EmailPM.Trim(';') : "";
                objPrefernce.EmailHR = !string.IsNullOrEmpty(model.EmailHR) ? model.EmailHR.Trim(';') : "";
                objPrefernce.EmailDeveloper = !string.IsNullOrEmpty(model.EmailDeveloper) ? model.EmailDeveloper.Trim(';') : "";
                objPrefernce.TimeSheetEmail = !string.IsNullOrEmpty(model.TimeSheetEmail) ? model.TimeSheetEmail.Trim(';') : CurrentUser.EmailOffice;
                objPrefernce.ProjectClosureEmail = !string.IsNullOrEmpty(model.ProjectClosureEmail) ? model.ProjectClosureEmail.Trim(';') : CurrentUser.EmailOffice;
                objPrefernce.IsAllowLeaveByTL = model.IsAllowLeave;
                objPrefernce.IsAllowWFHByTL = model.IsAllowWFH;
                objPrefernce.IsActive = true;

                if (model.SenDeveloperFile != null && model.SenDeveloperFile.FileName != string.Empty)
                {
                    SrFile = "senior" + Path.GetExtension(model.SenDeveloperFile.FileName.ToLower());

                    using (var stream = new FileStream(SrFile + "senior" + Path.GetExtension(model.SenDeveloperFile.FileName.ToLower()), FileMode.Create))
                    {
                        model.SenDeveloperFile.CopyTo(stream);
                    }
                    //model.SenDeveloperFile.SaveAs(path + "senior" + Path.GetExtension(model.SenDeveloperFile.FileName));
                }
                if (model.DeveloperFile != null && model.DeveloperFile.FileName != string.Empty)
                {
                    DevFile = "developer" + Path.GetExtension(model.DeveloperFile.FileName.ToLower());
                    using (var stream = new FileStream(DevFile + "developer" + Path.GetExtension(model.DeveloperFile.FileName.ToLower()), FileMode.Create))
                    {
                        model.DeveloperFile.CopyTo(stream);
                    }
                    //model.DeveloperFile.SaveAs(path + "developer" + System.IO.Path.GetExtension(model.DeveloperFile.FileName));
                }
                objPrefernce.InductionDoc = SrFile + "," + DevFile;
                preferenceService.Save(objPrefernce);
                ShowSuccessMessage("Success!", "Record has been saved successfully !!", false);
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Exception!", ex.Message, false);
                return RedirectToAction("index");
            }
        }


    }
}