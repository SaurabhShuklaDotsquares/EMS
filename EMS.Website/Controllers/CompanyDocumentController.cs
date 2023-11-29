using EMS.Dto;
using EMS.Service;
using EMS.Core;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using EMS.Web.Code.Attributes;
using System.Linq;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class CompanyDocumentController : BaseController
    {
        #region "Fields"
        private ICompanyDocumentService companyDocumentService;
      
        #endregion


        #region "Constructor"
        public CompanyDocumentController(ICompanyDocumentService _companyDocumentService)
        {
            this.companyDocumentService = _companyDocumentService;
        }
        #endregion

        // GET: CompanyDocument
        [CustomActionAuthorization()]
        public ActionResult Index()
        {
            List<CompanyDocumentDto> companyDocuments = new List<CompanyDocumentDto>();
            var data = companyDocumentService.GetCompanyDocumentsByRoles((CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO
                || RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_Sales_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_HR_DesignationIds.Contains(CurrentUser.DesignationId)
                || CurrentUser.RoleId == (int)Enums.UserRoles.HRBP ? true : false), CurrentUser.DeptId);
            return View(data);
            
        }

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (companyDocumentService != null)
                companyDocumentService.Dispose();
            base.Dispose(disposing);
        }
        #endregion
    }
}