using EMS.Dto;
using EMS.Dto.SARAL;
using EMS.Service.SARAL;
using EMS.Service.SARALDT;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.Models;
using EMS.Web.Models.Calendar;
using EMS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using static EMS.Core.Enums;
using EMS.Website.Code.LIBS;
using System.Linq.Expressions;
using System.Data;
using System.Net;
using EMS.Website.Models;
using DataTables.AspNet.Core;
using EMS.Data;
using EMS.Data.Model;
using EMS.Core;
using System.IO;
using EMS.Website.LIBS;

namespace EMS.Website.Controllers
{
    public class WFHController : BaseController
    {
        #region "Fields"       
        private IUserLoginService userLoginService;
        private IWFHService WFHService;
        private ILeaveService leaveService;
        string logPath = "D:/local/EMSWebCore/EMS.Website/wwwroot/WFH_Log/wfh-log.txt";
        #endregion

        #region "Constructor"       
        public WFHController(IUserLoginService _userLoginService, IWFHService _manageWFHService, ILeaveService _leaveService)
        {
            this.userLoginService = _userLoginService;
            this.WFHService = _manageWFHService;
            this.leaveService = _leaveService;

        }
        #endregion


        #region "HttpGet-ManageWFH"
        /// <summary>
        /// ManageWFH Get method use to get WFH request details
        /// </summary>
        /// <param name="returnview"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ManageWFH(string returnview, int? id)
        {            
            ViewBag.ReturntoListView = (returnview != null ? (returnview.ToLower().Trim() != "calendar") : true);
            bool IsEdit = id != null && id > 0 ? true : false;
            bool IsPmHrPmo = (RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId) || CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO
                           || RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
                           || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
                           || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
                           || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
                           || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)
                           || RoleValidator.TL_Sales_DesignationIds.Contains(CurrentUser.DesignationId)
                           || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
                           || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(CurrentUser.DesignationId)
                           || RoleValidator.TL_HR_DesignationIds.Contains(CurrentUser.DesignationId)
                           || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM) ? true : false;
            Dictionary<string, string> hdnFields = new Dictionary<string, string>();
            Wfhactivity wfhDB = new Wfhactivity();
            WFHActivityDto wfhDto = new WFHActivityDto();
            hdnFields.Add("returnview", returnview ?? "");


            if (RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId))
            {
                ViewBag.PMList = userLoginService.GetPMAndPMOHRDirectorUsers().Select(p => new SelectListItem { Text = p.Name, Value = p.Uid.ToString() }).ToList();
            }
            try
            {
                if (IsEdit)
                {
                    #region "Binding Data Model to View Model"

                    wfhDB = WFHService.GetWFHById(id.Value);
                    wfhDto.Uid = wfhDB.Uid;
                    wfhDto.PMid = wfhDB.U.PMUid.HasValue ? wfhDB.U.PMUid.Value : 0;
                    wfhDto.userId = wfhDB.Uid;
                    wfhDto.WFHId = wfhDB.Wfhid;
                    wfhDto.StartDate = wfhDB.StartDate != null ? wfhDB.StartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
                    wfhDto.EndDate = wfhDB.EndDate != null ? wfhDB.EndDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
                    wfhDto.Comment = wfhDB.Comment;
                    wfhDto.IsHalf = wfhDB.IsHalf != null ? wfhDB.IsHalf.Value : false;
                    wfhDto.DateAdded = wfhDB.DateAdded ?? DateTime.Now;
                    wfhDto.DateModify = wfhDB.DateModify ?? DateTime.Now;
                    wfhDto.Status = wfhDB.Status ?? 0;
                    wfhDto.IP = GeneralMethods.Getip();
                    wfhDto.IsCancel = (wfhDto.Status == (int)WFHStatus.Cancelled);
                    wfhDto.IsSelfWFH = wfhDB.Uid == CurrentUser.Uid;
                    wfhDto.FirstHalf = wfhDB.FirstHalf != null ? (bool)wfhDB.FirstHalf ? true : false : false;
                    wfhDto.SecondHalf = wfhDB.SecondHalf != null ? (bool)wfhDB.SecondHalf ? true : false : false;
                    wfhDto.HalfValue = wfhDB.FirstHalf == true ? 1 : wfhDto.SecondHalf == true ? 2 : 0;
                    hdnFields.Add("hdnTLId", wfhDB.U.TLId.ToString());
                    hdnFields.Add("hdnAddDT", wfhDB.DateAdded.ToString() ?? DateTime.Now.ToString("dd/MM/yyyy"));

                    #endregion
                }
                else
                {
                    wfhDto.WFHCategory = (int)Enums.WFHCategory.Full;
                    wfhDto.Status = (int)WFHStatus.Pending;
                    hdnFields.Add("hdnTLId", CurrentUser.TLId.ToString());
                    hdnFields.Add("hdnAddDT", DateTime.Now.ToString("dd/MM/yyyy"));
                    wfhDto.userId = 0;
                }
                wfhDto.selectEmployeeList = IsPmHrPmo ? GetEmployees() : null;
                var halfType = new List<SelectListItem>() { new SelectListItem { Text = "First Half", Value = "1" }, new SelectListItem { Text = "Second Half", Value = "2" } };
                halfType.Insert(0, new SelectListItem() { Text = "-Select-", Value = "" });
                wfhDto.HalfType = halfType;
                ViewBag.HdnFields = hdnFields;
            }
            catch (Exception ex)
            {
            }
            wfhDto.IsAllowWFH = true;

            var pmId = (CurrentUser.RoleId == (int)Enums.UserRoles.PM ? CurrentUser.Uid : CurrentUser.PMUid);

            

            if (pmId > 0)
            {
                var _prefereces = leaveService.GetPreferecesByPMUid(pmId);
                if (_prefereces != null)
                {
                    if (_prefereces.IsAllowWFHByTL && CurrentUser.RoleId != (int)Enums.UserRoles.PM)
                    {
                        wfhDto.IsAllowWFH = false;
                    }
                }
            }
            return View(wfhDto);
        }
        #endregion

        #region GetEmployees List
        /// <summary>
        /// GetEmployees method use to get list of all Employees.
        /// </summary>
        /// <param name="selectDefault"></param>
        /// <returns></returns>
        private List<SelectListItem> GetEmployees(bool selectDefault = true)
        {
            var EmployeeList = userLoginService.GetUsersListByAllDesignation(CurrentUser.Uid, CurrentUser.RoleId, CurrentUser.DesignationId);
            var selectEmpList = EmployeeList.Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.Uid.ToString(), Selected = selectDefault ? (x.Uid == EmployeeList.FirstOrDefault().Uid ? true : false) : false }).ToList();
            if (selectDefault)
                selectEmpList.Insert(0, new SelectListItem() { Text = "-Select-", Value = "" });
            return selectEmpList;
        }

        [HttpPost]
        public ActionResult GetEmployeesByPM(int pmid)
        {
            var users = (pmid > 0 ? userLoginService.GetUsersByPM(pmid) : userLoginService.GetUsers(true)).Select(u => new { text = u.EmpCode != null ? u.Name.ToString() + " [" + u.EmpCode + "]" : u.Name, value = u.Uid });
            return Json(users);
        }

        public ActionResult GetEmployeesByHR()
        {
            List<UserLogin> userList;
            int pmid = (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO || CurrentUser.RoleId == (int)Enums.UserRoles.HRBP) ? CurrentUser.Uid : CurrentUser.PMUid;
            bool IsHRUserRole = CurrentUser.RoleId == (int)Enums.UserRoles.HRBP;
            if (IsHRUserRole)
            {
                userList = userLoginService.GetWorkAlternators(pmid, CurrentUser.Uid, (int)Enums.UserRoles.HRBP);
            }
            else
            {
                userList = userLoginService.GetUsersByPM(pmid);
            }

            var users = userList.Select(u => new { text = u.EmpCode != null ? u.Name.ToString() + " [" + u.EmpCode + "]" : u.Name, value = u.Uid });
            return Json(users);
        }
        #endregion

        #region "HttpPost-ManageWFH"
        /// <summary>
        /// ManageWFH post method is use to save WFH request details.
        /// </summary>
        /// <param name="wfhModelForm"></param>
        /// <param name="returnview"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ManageWFH(WFHActivityDto wfhModelForm, string returnview)
        {
            var fileLogPath = SiteKey.IsLive ? System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "WFH_Log", "wfh-log.txt") : logPath;
            try
            {                
                var modalStartDate = DateTime.ParseExact(wfhModelForm.StartDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var modalEndDate = DateTime.ParseExact(wfhModelForm.EndDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var leaveActivity = leaveService.GetLeaveByUIdDateRange(wfhModelForm.Uid, modalStartDate, modalEndDate);
                #region "parameters declarations"
                bool returnToListView = (returnview != null ? (returnview.ToLower().Trim() != "calendar") : true);
                bool IsEdit = (wfhModelForm.WFHId > 0) ? true : false;
                bool IsPmHrPmo = (RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId) || CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO
                               || RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
                               || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
                               || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
                               || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
                               || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)
                               || RoleValidator.TL_Sales_DesignationIds.Contains(CurrentUser.DesignationId)
                               || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
                               || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(CurrentUser.DesignationId)
                               || RoleValidator.TL_HR_DesignationIds.Contains(CurrentUser.DesignationId)
                               || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM) ? true : false;
                bool IsRevised = IsEdit && IsPmHrPmo ? true : false;
                int halfValue = 0;
                int halfValues = 0;
               

                #endregion
                if (leaveActivity != null && leaveActivity.Status == (int)Enums.LeaveStatus.Approved && (wfhModelForm.IsCancel == false || wfhModelForm.Status != (int)WFHStatus.Cancelled))
                {
                    var StartDate = leaveActivity.StartDate;
                    var EndDate = leaveActivity.EndDate;

                    if (StartDate >= modalStartDate && EndDate >= modalEndDate && wfhModelForm.IsHalf == false)
                    {

                        ShowErrorMessage("Error !", "You have applied a Leave request for this time period, please ask your manager to Cancel the request and apply WFH request again.", false);
                        return RedirectToAction(returnToListView ? "index" : "ManageWFH");
                    }
                    else if (StartDate >= modalStartDate && EndDate >= modalEndDate)
                    {
                        var FirstHalfvalue = leaveActivity.FirstHalf;
                        var SecondHalfvalue = leaveActivity.SecondHalf;
                        if (FirstHalfvalue == true)
                        {
                            halfValue = 1;
                        }
                        if (SecondHalfvalue == true)
                        {
                            halfValues = 2;
                        }
                        if (halfValue == wfhModelForm.HalfValue || halfValues == wfhModelForm.HalfValue)
                        {

                            ShowErrorMessage("Error !", "You have applied a Leave request for this time period, please ask your manager to Cancel the request and apply WFH request again.", false);
                            return RedirectToAction(returnToListView ? "index" : "ManageWFH");
                        }
                    }

                    else if (StartDate == modalStartDate && EndDate == modalEndDate && wfhModelForm.IsHalf == true)
                    {
                        var FirstHalfvalue = leaveActivity.FirstHalf;
                        var SecondHalfvalue = leaveActivity.SecondHalf;
                        if (FirstHalfvalue == true)
                        {
                            halfValue = 1;
                        }
                        if (SecondHalfvalue == true)
                        {
                            halfValues = 2;
                        }
                        if (halfValue == wfhModelForm.HalfValue || halfValues == wfhModelForm.HalfValue)
                        {

                            ShowErrorMessage("Error !", "You have applied a Leave request for this time period, please ask your manager to Cancel the request and apply WFH request again.", false);
                            return RedirectToAction(returnToListView ? "index" : "ManageWFH");
                        }
                    }
                }

                #region "Save WFH"


                Wfhactivity wfhModelDB = new Wfhactivity();
                wfhModelDB = IsEdit ? WFHService.GetWFHById(wfhModelForm.WFHId) : wfhModelDB;
                DateTime startDatePrevious = new DateTime();
                DateTime endDatePrevious = new DateTime();
                if (IsEdit)
                {
                    startDatePrevious = wfhModelDB.StartDate;
                    endDatePrevious = wfhModelDB.EndDate;
                }
                bool isEditDate = (wfhModelForm.StartDate != wfhModelDB.StartDate.ToString("dd/MM/yyyy") || wfhModelForm.EndDate != wfhModelDB.EndDate.ToString("dd/MM/yyyy"));

                #region "comment>logic for find actual WFH status"
                /*   check for WFH status
                                                        Edit
                                        _________________|________________________
                                  (Yes)|                                          |(No)
                                   IsCancel                          ________ (Is HR/PM/PMO)_________
                          ___________|___________              (yes)|                                | (no)
                    (Yes)|                       |(No)          (from view)                        (Pending)
                 (status= 'CANCEL')             IsPmHrPmo
                                                    |
                                      _____yes_______________No________
                                     |                                |
                               (status from View)                  (status= 'from DB')

                 */

                /*   set proper uid
                                                     Edit
                                     _________________|________________________
                               (Yes)|                                          |(No)
                               Is HR/PM/PMO                          ________ (Is HR/PM/PMO)_________
                       ___________|___________                 (yes)|                                | (no)
                 (Yes)|                       |(No)           ( Uid=  from view)            (Uid= from Current Session)
              (Uid= 'from view')           (Uid= 'from DB')

              */
                #endregion

                if (wfhModelForm.IsSelfWFH)
                {
                    wfhModelForm.Uid = CurrentUser.Uid;
                }
                wfhModelDB.Uid = IsEdit ? (IsPmHrPmo ? wfhModelForm.Uid : wfhModelDB.Uid) : (IsPmHrPmo ? (wfhModelForm.IsSelfWFH ? CurrentUser.Uid : wfhModelForm.Uid) : CurrentUser.Uid);
                wfhModelDB.Status = IsEdit ? (
                                                wfhModelForm.IsCancel ?
                                                (int)Enums.WFHStatus.Cancelled : ((IsPmHrPmo && !wfhModelForm.IsSelfWFH) ? wfhModelForm.Status : wfhModelDB.Status)
                                                ) :
                                               (
                                               (!IsPmHrPmo || wfhModelForm.IsSelfWFH) ? (int)Enums.WFHStatus.Pending : wfhModelForm.Status
                                               );
                if (wfhModelForm.Uid == 0)
                {
                    wfhModelForm.Uid = CurrentUser.Uid;
                }
                

                if (ValidateWFHRquest(wfhModelForm))
                {
                    var userDetail = userLoginService.GetUserInfoByID(wfhModelForm.Uid);
                    wfhModelDB.StartDate = modalStartDate;
                    wfhModelDB.EndDate = modalEndDate;
                    int diffdays = (int)((wfhModelDB.StartDate.Date - DateTime.Now.Date).TotalDays);
                    int wfhDays = (int)((wfhModelDB.EndDate.Date - wfhModelDB.StartDate.Date).TotalDays);
                    wfhModelDB.Wfhcategory = WFHService.GetWFHCategory();
                    if (wfhModelForm.IsSelfWFH && wfhModelForm.Status == (int)WFHStatus.Cancelled)
                    {
                        wfhModelDB.ApprovedById = CurrentUser.Uid;
                    }
                    else if (wfhModelForm.IsCancel == true)
                    {
                        wfhModelDB.ApprovedById = null;
                    }
                    else if (wfhModelForm.IsSelfWFH == false && wfhModelForm.ApprovedById == 0)
                    {
                        wfhModelDB.ApprovedById = PMUserId;
                    }
                    else if (wfhModelForm.Status == (int)WFHStatus.Approved || wfhModelForm.Status == (int)WFHStatus.UnApproved)
                    {
                        wfhModelDB.ApprovedById = CurrentUser.Uid;
                    }
                    wfhModelDB.Comment = wfhModelForm.Comment;
                    wfhModelDB.IsHalf = wfhModelForm.IsHalf;
                    wfhModelDB.DateAdded = wfhModelDB.DateAdded ?? System.DateTime.Now;
                    wfhModelDB.AnyComment = wfhModelForm.AnyComment;
                    wfhModelDB.DateModify = DateTime.Now;
                    wfhModelDB.Ip = GeneralMethods.Getip();
                    if (wfhModelForm.HalfValue.HasValue && wfhModelForm.IsHalf)
                    {
                        if (wfhModelForm.HalfValue.Value == 1)
                        {
                            wfhModelDB.FirstHalf = true;
                            wfhModelDB.SecondHalf = false;
                        }
                        if (wfhModelForm.HalfValue.Value == 2)
                        {
                            wfhModelDB.FirstHalf = false;
                            wfhModelDB.SecondHalf = true;
                        }
                        wfhModelForm.WFHCategory = (int)WFHCategory.Half;
                    }
                    else
                    {
                        wfhModelDB.FirstHalf = true;
                        wfhModelDB.SecondHalf = true;
                        wfhModelForm.WFHCategory = (int)WFHCategory.Full;
                    }
                    if (wfhModelDB.Uid == 0)
                    {
                        wfhModelDB.Uid = CurrentUser.Uid;
                    }
                    WFHService.Save(wfhModelDB);
                    LogPrint.WriteIntoFile(fileLogPath, $"---------------------------WFHId:{wfhModelDB.Wfhid}---------------------------");
                    LogPrint.WriteIntoFile(fileLogPath, $"EMS WFH Data created on:{DateTime.Now}");
                    LogPrint.WriteIntoFile(fileLogPath, $"EmpUid:{wfhModelDB.Uid}, EmpWFHId:{wfhModelDB.Wfhid}, WFHStartDate:{wfhModelDB.StartDate.ToString("dd-MMM-yyyy")}, WFHEndDate:{wfhModelDB.EndDate.ToString("dd-MMM-yyyy")}, HalfDay:{(wfhModelDB.IsHalf.HasValue && wfhModelDB.IsHalf.Value ? true : false)}, FirstHalf:{wfhModelDB.FirstHalf}, SecondHalf:{wfhModelDB.SecondHalf}, WFHCategory:{((Enums.WFHCategory)wfhModelDB.Wfhcategory).GetEnumDescription()}, ModifyDate:{(wfhModelDB.DateModify.HasValue ? (wfhModelDB.DateModify.Value).ToString("dd-MMM-yyyy") : string.Empty)}, Created/ModifyBy:{CurrentUser.Uid}");
                    LogPrint.WriteIntoFile(fileLogPath, "");

                    WFHService.Save(wfhModelDB);

                    if (SiteKey.IsLive)
                    {
                        //***************ExecuteHrmAPI*****************//
                        var emsWFHId = wfhModelDB.Wfhid;
                        var empInfo = userLoginService.GetUserInfoByID(wfhModelDB.Uid);
                        //if (CurrentUser.RoleId != (int)UserRoles.UKBDM && CurrentUser.RoleId != (int)UserRoles.UKPM && CurrentUser.RoleId != (int)UserRoles.AUPM && CurrentUser.RoleId != (int)UserRoles.PMOAU && CurrentUser.RoleId != (int)UserRoles.PMO && SiteKey.IsLive)
                        if (empInfo != null && empInfo.Role != null && empInfo.Role.RoleId != (int)UserRoles.UKBDM && empInfo.Role.RoleId != (int)UserRoles.UKPM && empInfo.Role.RoleId != (int)UserRoles.AUPM && empInfo.Role.RoleId != (int)UserRoles.PMOAU && empInfo.Role.RoleId != (int)UserRoles.PMO && SiteKey.IsLive)
                        {
                            if (wfhModelDB.Status == (int)WFHStatus.Approved)
                            {
                                try
                                {
                                    ExecuteHrmAPI(Convert.ToString(emsWFHId), empInfo.EmailOffice, wfhModelDB.StartDate, wfhModelDB.EndDate, "WFH", (bool)wfhModelDB.IsHalf, "InsertUpdate");
                                    LogPrint.WriteIntoFile(fileLogPath, $"HRM WFH Data (Approved) created on:{DateTime.Now}");
                                    LogPrint.WriteIntoFile(fileLogPath, $"EmpEmail:{empInfo.EmailOffice}, EmpWFHId:{emsWFHId}, WFHStartDate:{wfhModelDB.StartDate.ToString("dd-MMM-yyyy")}, WFHEndDate:{wfhModelDB.EndDate.ToString("dd-MMM-yyyy")}, HalfDay:{(wfhModelDB.IsHalf.HasValue && wfhModelDB.IsHalf.Value ? true : false)}, Status: Approved, ModifyDate:{(wfhModelDB.DateModify.HasValue ? (wfhModelDB.DateModify.Value).ToString("dd-MMM-yyyy") : string.Empty)}, Created/ModifyBy:{CurrentUser.Uid}");
                                    LogPrint.WriteIntoFile(fileLogPath, "");
                                }
                                catch (Exception ex)
                                {
                                    LogPrint.WriteIntoFile(fileLogPath, $"HRM WFH Data (Approved) created on:{DateTime.Now}");
                                    LogPrint.WriteIntoFile(fileLogPath, $"Request => (EmpEmail:{empInfo.EmailOffice}, EmpWFHId:{emsWFHId}, WFHStartDate:{wfhModelDB.StartDate.ToString("dd-MMM-yyyy")}, WFHEndDate:{wfhModelDB.EndDate.ToString("dd-MMM-yyyy")}, HalfDay:{(wfhModelDB.IsHalf.HasValue && wfhModelDB.IsHalf.Value ? true : false)}, Status: Approved, ModifyDate:{(wfhModelDB.DateModify.HasValue ? (wfhModelDB.DateModify.Value).ToString("dd-MMM-yyyy") : string.Empty)}, Created/ModifyBy:{CurrentUser.Uid})");
                                    LogPrint.WriteIntoFile(fileLogPath, $"Exception: {(ex.InnerException.Message ?? ex.Message)}");
                                    LogPrint.WriteIntoFile(fileLogPath, "");
                                }
                            }
                            else if (wfhModelDB.Status == (int)WFHStatus.UnApproved || wfhModelDB.Status == (int)WFHStatus.Cancelled)
                            {
                                try
                                {
                                    ExecuteHrmAPI(Convert.ToString(emsWFHId), empInfo.EmailOffice, wfhModelDB.StartDate, wfhModelDB.EndDate, "WFH", (bool)wfhModelDB.IsHalf, "Delete");
                                    LogPrint.WriteIntoFile(fileLogPath, $"HRM WFH Data (Cancelled/UnApproved) created on:{DateTime.Now}");
                                    LogPrint.WriteIntoFile(fileLogPath, $"EmpEmail:{empInfo.EmailOffice}, EmpWFHId:{emsWFHId}, WFHStartDate:{wfhModelDB.StartDate.ToString("dd-MMM-yyyy")}, WFHEndDate:{wfhModelDB.EndDate.ToString("dd-MMM-yyyy")}, HalfDay:{(wfhModelDB.IsHalf.HasValue && wfhModelDB.IsHalf.Value ? true : false)}, Status: Cancelled/UnApproved, ModifyDate:{(wfhModelDB.DateModify.HasValue ? (wfhModelDB.DateModify.Value).ToString("dd-MMM-yyyy") : string.Empty)}, Created/ModifyBy:{CurrentUser.Uid}");
                                    LogPrint.WriteIntoFile(fileLogPath, "");
                                }
                                catch (Exception ex)
                                {
                                    LogPrint.WriteIntoFile(fileLogPath, $"HRM WFH Data (Cancelled/UnApproved) created on:{DateTime.Now}");
                                    LogPrint.WriteIntoFile(fileLogPath, $"Request => (EmpEmail:{empInfo.EmailOffice}, EmpWFHId:{emsWFHId}, WFHStartDate:{wfhModelDB.StartDate.ToString("dd-MMM-yyyy")}, WFHEndDate:{wfhModelDB.EndDate.ToString("dd-MMM-yyyy")}, HalfDay:{(wfhModelDB.IsHalf.HasValue && wfhModelDB.IsHalf.Value ? true : false)}, Status: Cancelled/UnApproved, ModifyDate:{(wfhModelDB.DateModify.HasValue ? (wfhModelDB.DateModify.Value).ToString("dd-MMM-yyyy") : string.Empty)}, Created/ModifyBy:{CurrentUser.Uid})");
                                    LogPrint.WriteIntoFile(fileLogPath, $"Exception: {(ex.InnerException.Message ?? ex.Message)}");
                                    LogPrint.WriteIntoFile(fileLogPath, "");
                                }
                            }
                        }
                        //****************EndHrmAPI*****************//
                    }

                    wfhModelDB = IsEdit ? WFHService.GetWFHById(wfhModelForm.WFHId) : wfhModelDB;
                    ShowSuccessMessage("Success!", "WFH request has been saved successfully.", false);
                    LogPrint.WriteIntoFile(fileLogPath, "------------------------------------END-------------------------------------");
                    LogPrint.WriteIntoFile(fileLogPath, string.Empty);
                    // get applied  user detail
                    if (wfhModelForm.Uid > 0)
                    {
                        wfhModelDB.U = userLoginService.GetUserInfoByID(wfhModelForm.Uid);
                    }

                    #region "GetEmailIds TO, CC"
                    //get prefernces settings by PM-Id
                    Preference ObjPreference = WFHService.GetPreferecesByPMUid((CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO) ? CurrentUser.Uid : CurrentUser.PMUid);
                    //get PM email by using TL  email
                    string EmailToPM = ""; //!String.IsNullOrEmpty(ObjPreference.EmailPM) ? ObjPreference.EmailPM : "";
                    string emailToTL = string.Empty;
                    if (wfhModelDB.U.TLId.HasValue && wfhModelDB.U.TLId.Value > 0)
                    {
                        emailToTL = userLoginService.GetUserInfoByID(Convert.ToInt32(wfhModelDB.U.TLId))?.EmailOffice;
                    }

                    //Get mail Id of PM and HR to CC
                    string emailCC = "";
                    if (ObjPreference != null)
                    {
                        EmailToPM = !String.IsNullOrEmpty(ObjPreference.EmailPM) ? ObjPreference.EmailPM : "";
                        emailCC = ((!String.IsNullOrEmpty(EmailToPM)) ? EmailToPM + ";" : "") + ((!String.IsNullOrEmpty(ObjPreference.EmailHR)) ? ObjPreference.EmailHR + ";" : "") + emailToTL;
                    }
                    emailCC = emailCC.Trim().TrimEnd(';');
                    #endregion

                    #region "SendEmails & CreateOutLookEvent"
                    if (SiteKey.IsLive)
                    {
                        if (!String.IsNullOrEmpty(CurrentUser.EmailOffice))
                        {
                            if (!IsEdit || isEditDate)
                            {
                                if (wfhModelDB.Status.Value == (int)Enums.WFHStatus.Pending)
                                {
                                    //send WFH request Emails
                                    SendWFHRequestEmail(IsPmHrPmo, wfhModelForm, wfhModelDB, EmailToPM, emailCC);
                                }
                            }

                            // Send Email about WFH status
                            SendWFHStatusEmail(IsPmHrPmo, wfhModelForm, wfhModelDB, EmailToPM, emailCC);
                        }
                    }
                    #endregion
                }
                #endregion

                return RedirectToAction(returnToListView ? "index" : "ManageWFH");
            }
            catch (Exception ex)
            {
                LogPrint.WriteIntoFile(fileLogPath, (ex.InnerException??ex).ToString());
                ShowErrorMessage("Exception!", ex.Message, false);
                return RedirectToAction("manageWFH");
            }
        }
        #endregion

        #region ValidateWFHRquest
        /// <summary>
        /// ValidateWFHRquest method use to validate WFH request send by user.
        /// </summary>
        /// <param name="wfhModelForm"></param>
        /// <returns></returns>
        private bool ValidateWFHRquest(WFHActivityDto wfhModelForm)
        {
            var modalStartDate = DateTime.ParseExact(wfhModelForm.StartDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var modalEndDate = DateTime.ParseExact(wfhModelForm.EndDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var leaveActivity = leaveService.GetLeaveByUIdDateRange(wfhModelForm.Uid, modalStartDate, modalEndDate);
            bool canapplyWFH = WFHService.CanApplyWFH(wfhModelForm.Uid, wfhModelForm.WFHId, modalStartDate, modalEndDate);

            if (leaveActivity != null && leaveActivity.Status == (int)LeaveStatus.Approved)
            {
                if (canapplyWFH == true)
                {
                    if (wfhModelForm.IsHalf == true)
                    {
                        if (leaveActivity.IsHalf == true && wfhModelForm.IsHalf == true && (leaveActivity.StartDate == leaveActivity.EndDate || (leaveActivity.Status == (int)Enums.LeaveStatus.Cancelled || leaveActivity.Status == (int)Enums.LeaveStatus.UnApproved)))
                        {
                            return true;
                        }
                        else if (leaveActivity.IsHalf == false && wfhModelForm.IsHalf == true && (leaveActivity.Status == (int)Enums.LeaveStatus.Cancelled || leaveActivity.Status == (int)Enums.LeaveStatus.UnApproved))
                        {
                            return true;
                        }
                        else
                        {
                            ShowErrorMessage("Error !", "You have already applied a Leave request for this time period, please select a different date range otherwise cancel your Leave request.", false);
                            return false;
                        }
                    }
                    else
                    {
                        if (leaveActivity.Status == (int)Enums.LeaveStatus.Cancelled || leaveActivity.Status == (int)Enums.LeaveStatus.UnApproved || wfhModelForm.IsCancel || wfhModelForm.Status == (int)WFHStatus.Cancelled)
                        {
                            return true;
                        }
                        else
                        {
                            ShowErrorMessage("Error !", "You have already applied a Leave request for this time period, please select a different date range otherwise cancel your Leave request.", false);
                            return false;
                        }
                    }
                }
                else
                {
                    ShowErrorMessage("Error !", "You have already applied a WFH request for this time period, please select a different date range.", false);
                    return false;
                }
            }
            else
            {
                if (canapplyWFH == true)
                {
                    return true;
                }
                else
                {
                    ShowErrorMessage("Error !", "You have already applied a WFH request for this time period, please select a different date range.", false);
                    return false;
                }
            }
        }
        #endregion

        #region SendWFHRequestEmail
        /// <summary>
        /// SendWFHRequestEmail method use to send WFH request email.
        /// </summary>
        /// <param name="IsHrPmPmo"></param>
        /// <param name="wfhModelForm"></param>
        /// <param name="wfhModelDB"></param>
        /// <param name="EmailTo"></param>
        /// <param name="emailCC"></param>
        private void SendWFHRequestEmail(bool IsHrPmPmo, WFHActivityDto wfhModelForm, Wfhactivity wfhModelDB, string EmailTo, string emailCC)
        {
            FlexiMail objSendMail = new FlexiMail();
            string wfhCategory = "WFH";
            string fromemail = CurrentUser.EmailOffice;
            string userName = CurrentUser.Name;
            if (wfhModelDB != null && wfhModelDB.U != null)
            {
                //wfhCategory = wfhModelDB.Wfhcategory.HasValue ? ((Enums.WFHCategory)wfhModelDB.Wfhcategory).GetEnumDescription() : "WFH";
                userName = wfhModelDB.U.Name;
                var pmuid = 0;
                if (wfhModelDB.U.PMUid.HasValue)
                {
                    pmuid = wfhModelDB.U.PMUid.Value;
                }
                else
                {
                    pmuid = CurrentUser.PMUid;
                }
                var preference = WFHService.GetPreferecesByPMUid(pmuid);
                if (preference != null)
                {
                    fromemail = preference.EmailFrom;
                }
            }
            objSendMail.From = fromemail;
            objSendMail.To = EmailTo;
            objSendMail.CC = emailCC;
            objSendMail.BCC = "";
            if (wfhModelDB.Status == (int)WFHStatus.Cancelled)
            {
                objSendMail.Subject = (wfhModelForm.WFHId > 0 ? Enum.GetName(typeof(WFHStatus), wfhModelDB.Status) : Enum.GetName(typeof(WFHStatus), wfhModelForm.Status)) + "WFH Cancel Request From -" + userName;
                objSendMail.MailBody = objSendMail.GetHtml("WFHCancelEmail.html");
                objSendMail.ValueArray = new string[] { CurrentUser.UserName.ToString() };
            }
            else
            {
                objSendMail.ValueArray = new string[] {
                                                        wfhModelForm.IsHalf ? " <b>half day</b>" : "",
                                                        wfhModelForm.WFHId > 0 ? Enum.GetName(typeof(WFHCategory), wfhModelDB.Wfhcategory) : Enum.GetName(typeof(WFHCategory), wfhModelForm.WFHCategory),
                                                        DateTime.ParseExact(wfhModelForm.StartDate,"dd/MM/yyyy",CultureInfo.InvariantCulture).ToString("ddd, MMM dd, yyyy"),
                                                        DateTime.ParseExact(wfhModelForm.EndDate,"dd/MM/yyyy",CultureInfo.InvariantCulture).ToString("ddd, MMM dd, yyyy"),
                                                        wfhModelForm.IsHalf ? "Half" : "Full",
                                                        wfhModelForm.Comment,
                                                        CurrentUser.MobileNumber,
                                                        IsHrPmPmo?userLoginService.GetUserInfoByID(wfhModelDB.Uid).Name :CurrentUser.Name,
                                                        wfhCategory
                                                        };
                objSendMail.MailBody = objSendMail.GetHtml("WFHRequestEmail.html");
                objSendMail.Subject = (wfhModelDB.IsHalf != true ? "Full Day Work From Home" : "Half Day Work From Home") + " Request -" + userName;
            }
            objSendMail.MailBodyManualSupply = true;
            objSendMail.Send();
        }
        #endregion

        #region SendWFHStatusEmail
        /// <summary>
        /// SendWFHStatusEmail method use to send WFH status email.
        /// </summary>
        /// <param name="IsPmHrPmo"></param>
        /// <param name="wfhModelForm"></param>
        /// <param name="wfhModelDB"></param>
        /// <param name="EmailTo"></param>
        /// <param name="emailCC"></param>
        private void SendWFHStatusEmail(bool IsPmHrPmo, WFHActivityDto wfhModelForm, Wfhactivity wfhModelDB, string EmailTo, string emailCC)
        {
            #region "Parameters Declaration"
            string Status = "";
            string wfhCategory = "";
            string ReciepentName = "";
            string AdminName = "";
            UserLogin usr = userLoginService.GetUserInfoByID(wfhModelDB.Uid);
            FlexiMail objSendMail = new FlexiMail();
            #endregion

            #region "Set Common Values"
            string fromemail = usr.EmailOffice;
            var preference = WFHService.GetPreferecesByPMUid(usr.PMUid.HasValue ? usr.PMUid.Value : 0);
            if (preference != null)
            {
                fromemail = preference.EmailFrom;
            }
            Status = Enum.GetName(typeof(WFHStatus), wfhModelDB.Status);
            wfhCategory = wfhModelDB.Wfhcategory.HasValue ? ((Enums.WFHCategory)wfhModelDB.Wfhcategory).GetEnumDescription() : "WFH";
            ReciepentName = usr.Name;
            AdminName = CurrentUser.Name;
            objSendMail.From = fromemail;
            objSendMail.To = usr.EmailOffice;
            objSendMail.CC = emailCC;
            objSendMail.BCC = "";
            objSendMail.MailBodyManualSupply = true;
            #endregion

            #region "Email Status Approved"
            if (Status == Enum.GetName(typeof(WFHStatus), WFHStatus.Approved))
            {
                objSendMail.ValueArray = new string[] { ReciepentName,
                                                      Convert.ToBoolean(wfhModelDB.IsHalf)?"Half":"Full",
                                                      wfhModelDB.StartDate.ToString("ddd, MMM dd, yyyy")+ " to " + wfhModelDB.EndDate.ToString("ddd, MMM dd, yyyy"),
                                                      AdminName,
                                                      wfhModelDB.Comment,
                                                      wfhModelDB.AnyComment==null?string.Empty:wfhModelDB.AnyComment,
                                                     wfhCategory,
                                                     wfhModelDB.AnyComment==null?"":"Approver's Comment: "
                                                      };
                objSendMail.Subject = (wfhModelDB.IsHalf != true ? "Full Day Work From Home Request" : "Half Day Work From Home Request") + " Approved";
                objSendMail.MailBody = objSendMail.GetHtml("WFHStatusApprovedEmail.html");
            }
            #endregion

            #region "Email Status UnApproved"
            else if (Status == Enum.GetName(typeof(WFHStatus), WFHStatus.UnApproved))
            {
                objSendMail.ValueArray = new string[]  {ReciepentName,
                                                        wfhModelDB.Comment,
                                                        AdminName,
                                                       wfhModelDB.StartDate.ToString("ddd, MMM dd, yyyy")+ " to " + wfhModelDB.EndDate.ToString("ddd, MMM dd, yyyy"),
                                                       wfhModelDB.AnyComment==null?string.Empty:wfhModelDB.AnyComment,
                                                      wfhCategory,
                                                      wfhModelDB.AnyComment==null?"":"Approver's Comment: "
                                                            };
                objSendMail.Subject = "Work From Home Request UnApproved";
                objSendMail.MailBody = objSendMail.GetHtml("WFHStatusUnApprovedEmail.html");
            }
            #endregion

            #region "Email Status Cancelled"
            else if (Status == Enum.GetName(typeof(WFHStatus), WFHStatus.Cancelled))
            {
                objSendMail.ValueArray = new string[]  {ReciepentName,
                                                        wfhModelDB.Comment,
                                                        AdminName,
                                                        wfhModelDB.StartDate.ToString("ddd, MMM dd, yyyy")+ " to " + wfhModelDB.EndDate.ToString("ddd, MMM dd, yyyy"),
                                                        wfhModelDB.AnyComment==null?string.Empty:wfhModelDB.AnyComment,
                                                        wfhCategory,
                                                        wfhModelDB.AnyComment==null?"":"Approver's Comment: "
                                                    };
                objSendMail.Subject = "Work From Home Request Cancelled";
                objSendMail.MailBody = objSendMail.GetHtml("WFHStatusCancelledEmail.html");
            }
            #endregion

            #region "Email Status Pending/Others"
            else
            {
                objSendMail.ValueArray = new string[] { ReciepentName,
                    wfhModelDB.StartDate.ToString("ddd, MMM dd, yyyy") + " to " + wfhModelDB.EndDate.ToString("ddd, MMM dd, yyyy"),
                    wfhModelDB.Comment==null?string.Empty:wfhModelDB.Comment,
                    wfhCategory
                };
                objSendMail.Subject = (wfhModelDB.IsHalf != true ? "Full Day Work From Home Request" : "Half Day Work From Home Request") + " Pending";
                objSendMail.MailBody = objSendMail.GetHtml("WFHStatusPendingEmail.html");
            }
            #endregion
            objSendMail.Send();
        }
        #endregion

        #region Index        
        [CustomActionAuthorization]
        public ActionResult Index()
        {            
            bool IsPmHrPmo = (RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId) || CurrentUser.RoleId == (int)Enums.UserRoles.PM ||
                              RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
                           || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
                           || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
                           || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
                           || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)
                           || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
                           || RoleValidator.TL_Sales_DesignationIds.Contains(CurrentUser.DesignationId)
                           || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(CurrentUser.DesignationId)
                ) ? true : false;
            ViewBag.Users = GetEmployees(false);
            ViewBag.Status = typeof(Enums.WFHStatus).EnumToDictionaryWithDescription().Select(v => new SelectListItem { Text = v.Key, Value = v.Value.ToString() }).ToList();
            ViewBag.PM = userLoginService.GetPMAndPMOHRDirectorUsers(true).Select(p => new SelectListItem { Text = p.Name, Value = p.Uid.ToString() }).ToList();
            return View();
        }
        #endregion

        #region GetWFH-List
        /// <summary>
        /// GetWFH method use to get WFH list.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="searchFilter"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult GetWFH(IDataTablesRequest request, WFHSpecialSearchFilterViewModel searchFilter)
        {            
            TempData.Put("searchFilter", searchFilter);
            DateTime dateStart, dateEnd;
            var pagingService = new PagingService<Wfhactivity>(request.Start, request.Length);
            var expr = PredicateBuilder.True<Wfhactivity>();
            int pmId = (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO) ? CurrentUser.Uid : CurrentUser.PMUid;
            if (CurrentUser.Uid > 0)
            {
                if (CurrentUser.RoleId == (int)UserRoles.PM || CurrentUser.RoleId == (int)UserRoles.UKPM || CurrentUser.RoleId == (int)UserRoles.PMO || CurrentUser.RoleId == (int)UserRoles.UKBDM)
                {
                    expr = expr.And(e => e.Uid == CurrentUser.Uid || (e.U.PMUid.HasValue && e.U.PMUid.Value == pmId) || (e.U.TLId.HasValue && e.U.TLId.Value == pmId));
                }
                else if (RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
                      || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
                      || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
                      || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
                      || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)
                      || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)                    
                      || RoleValidator.TL_Sales_DesignationIds.Contains(CurrentUser.DesignationId)                    
                      || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(CurrentUser.DesignationId)                    
                        //|| RoleValidator.DV_RoleIds.Contains(CurrentUser.RoleId)
                        )
                {
                    int[] sdList = userLoginService.GetTLUsers(CurrentUser.Uid).Select(T => T.Uid).ToArray();
                    expr = expr.And(e => e.Uid == CurrentUser.Uid || (e.U.TLId.HasValue && e.U.TLId.Value == CurrentUser.Uid) || (sdList.Contains((int)e.U.TLId))
                    );
                }
                else
                {
                    if (!RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId))
                    {
                        expr = expr.And(e => e.Uid == CurrentUser.Uid);
                    }
                }
            }

            if (searchFilter.user.HasValue && searchFilter.user.Value != 0)
            {
                expr = expr.And(l => l.Uid == searchFilter.user.Value);
            }
            if (searchFilter.status.HasValue && searchFilter.status.Value != 0)
            {
                expr = expr.And(l => l.Status == searchFilter.status.Value);
            }
            //if (searchFilter.WFHcatagory.HasValue && searchFilter.WFHcatagory.Value != 0)
            //{
            //    expr = expr.And(l => l.Wfhcategory == searchFilter.WFHcatagory.Value);
            //}            
            if (searchFilter.pm.HasValue && searchFilter.pm.Value != 0)
            {
                expr = expr.And(l => (l.U.PMUid == searchFilter.pm) || (l.U.TLId == searchFilter.pm)|| (l.U.Uid == searchFilter.pm));
            }
            if (!string.IsNullOrEmpty(searchFilter.startDate) && DateTime.TryParseExact(searchFilter.startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateStart))
            {
                expr = expr.And(l => l.StartDate >= dateStart || l.EndDate >= dateStart);
            }
            if (!string.IsNullOrEmpty(searchFilter.endDate) && DateTime.TryParseExact(searchFilter.endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateEnd))
            {
                expr = expr.And(l => l.EndDate <= dateEnd || l.StartDate <= dateEnd);
            }

            pagingService.Filter = expr;

            pagingService.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    
                    switch (item.Name)
                    {
                        case "UserName":
                            return o.OrderByColumn(item, c => c.U.Name);

                        case "Status":
                            return o.OrderByColumn(item, c => c.Status);

                        case "DateAdded":
                            return o.OrderByColumn(item, c => c.DateAdded);

                        case "StartDate":
                            return o.OrderByColumn(item, c => c.StartDate);
                    }
                }
                return o.OrderByDescending(c => c.DateAdded);
            };

            int totalCount = 0;
            double totalwfh = 0;
            var response = WFHService.GetWFH(out totalCount, pagingService);
            IDictionary<string, object> additionalParameters = new Dictionary<string, object>();
            if (searchFilter.user.HasValue && searchFilter.user.Value != 0)
            {
                totalwfh = WFHService.GetTotalWFH(pagingService);
                String StartDate = "", EndDate = "";
                UserLogin user = userLoginService.GetUserInfoByID(searchFilter.user.Value);
                additionalParameters.Add(new KeyValuePair<string, object>("startDate", StartDate));
                additionalParameters.Add(new KeyValuePair<string, object>("endDate", EndDate));
                if (user.RoleId == (int)Enums.UserRoles.PMO || user.RoleId == (int)Enums.UserRoles.UKPM || user.RoleId == (int)Enums.UserRoles.UKBDM)
                {
                    additionalParameters.Add(new KeyValuePair<string, object>("isIndianUser", "0"));
                }
                else
                {
                    additionalParameters.Add(new KeyValuePair<string, object>("isIndianUser", "1"));
                }
                additionalParameters.Add(new KeyValuePair<string, object>("totalWFH", totalwfh));
            }


            return DataTablesJsonResult(totalCount, request, response.Select((x, index) => new
            {
                rowId = request.Start + index + 1,
                userId = x.U.Uid,
                wfhId = x.Wfhid,
                startDate = x.StartDate.ToString("ddd, MMM dd, yyyy"),
                endDate = x.EndDate.ToString("ddd, MMM dd, yyyy"),
                comment = x.Comment,
                anycomment = x.AnyComment,
                PMName = x.U.Pmu?.Name ?? String.Empty,
                status = x.Status.HasValue ? GetStatus(x.Status.Value) : "",
                ApprovedBy = x.ApprovedById != null ? x.ApprovedBy.Name : String.Empty,
                dateAdded = x.DateAdded?.ToString("ddd, MMM dd, yyyy hh:mm tt"),
                userName = x.U.Name,
                isSelf = (x.U.Uid == CurrentUser.Uid),
                isEdit = x.Status.HasValue ? ((((Enums.WFHStatus)x.Status.Value == Enums.WFHStatus.Pending) ||
                (CurrentUser.RoleId == (int)Enums.UserRoles.PM) ||
                (CurrentUser.RoleId == (int)Enums.UserRoles.PMO) ||
                RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId) || (CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM)
                  ) && ((DateTime.Now.Date <= x.StartDate.Date) || ((CurrentUser.RoleId == (int)Enums.UserRoles.PM) ||
                  (CurrentUser.RoleId == (int)Enums.UserRoles.PMO) ||
                  RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId) || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM)) ||
                  x.U.TLId == CurrentUser.Uid ? true : false) : true
            }), additionalParameters);
        }
        #endregion

        #region GetStatus-WFH
        /// <summary>
        /// GetStatus method use to get WFH status type.
        /// </summary>
        /// <param name="statusId"></param>
        /// <returns></returns>
        public string GetStatus(int statusId)
        {
            string status = string.Empty;
            switch ((Enums.WFHStatus)statusId)
            {
                case Enums.WFHStatus.Approved:
                    status = "<p class='text-green'><b><i class='fa fa-check'></i> Approved</b></p>";
                    break;

                case Enums.WFHStatus.Cancelled:
                    status = "<p class='text-red'><b><i class='fa fa-close'></i> Cancelled</b></p>";
                    break;

                case Enums.WFHStatus.UnApproved:
                    status = "<p class='text-fuchsia'><b><i class='fa fa-remove'></i> UnApproved</b></p>";
                    break;

                case Enums.WFHStatus.Pending:
                    status = "<p class='text-orange'><b><i class='fa fa-warning'></i> Pending</b></p>";
                    break;
            }
            return status;
        }
        #endregion

        #region DownloadWFHdataExcel
        /// <summary>
        /// DownloadWFHdataExcel method use to download WFH data excel file.
        /// </summary>
        /// <returns></returns>
        public FileResult DownloadWFHdataExcel()
        {            
            var searchFilter = TempData.Get<WFHSpecialSearchFilterViewModel>("searchFilter");
            DateTime dateStart, dateEnd;
            var expr = PredicateBuilder.True<Wfhactivity>();
            int pmId = (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO) ? CurrentUser.Uid : CurrentUser.PMUid;
            if (CurrentUser.Uid > 0)
            {
                if (CurrentUser.RoleId == (int)UserRoles.PM || CurrentUser.RoleId == (int)UserRoles.PMO || CurrentUser.RoleId == (int)UserRoles.UKBDM)
                {
                    expr = expr.And(e => e.Uid == CurrentUser.Uid || (e.U.PMUid.HasValue && e.U.PMUid.Value == pmId));
                }
                else if (RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
                      || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
                      || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
                      || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
                      || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)
                      || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
                    //|| RoleValidator.DV_RoleIds.Contains(CurrentUser.RoleId)
                    )
                {
                    int[] sdList = userLoginService.GetTLUsers(CurrentUser.Uid).Select(T => T.Uid).ToArray();
                    expr = expr.And(e => e.Uid == CurrentUser.Uid || (e.U.TLId.HasValue && e.U.TLId.Value == CurrentUser.Uid) || (sdList.Contains((int)e.U.TLId)));
                }
                else
                {
                    if (CurrentUser.RoleId != (int)UserRoles.HRBP)
                    {
                        expr = expr.And(e => e.Uid == CurrentUser.Uid);
                    }
                }
            }
            if (searchFilter.user.HasValue && searchFilter.user.Value != 0)
            {
                expr = expr.And(l => l.Uid == searchFilter.user.Value);
            }
            if (searchFilter.status.HasValue && searchFilter.status.Value != 0)
            {
                expr = expr.And(l => l.Status == searchFilter.status.Value);
            }

            if (searchFilter.pm.HasValue && searchFilter.pm.Value != 0)
            {
                expr = expr.And(l => l.U.PMUid == searchFilter.pm);
            }
            if (!string.IsNullOrEmpty(searchFilter.startDate) && DateTime.TryParseExact(searchFilter.startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateStart))
            {
                expr = expr.And(l => l.StartDate >= dateStart || l.EndDate >= dateStart);
            }
            if (!string.IsNullOrEmpty(searchFilter.endDate) && DateTime.TryParseExact(searchFilter.endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateEnd))
            {
                expr = expr.And(l => l.EndDate <= dateEnd || l.StartDate <= dateEnd);
            }
            var model = WFHService.GetWFHActivity(expr);
            List<WFHReportViewModel> WFHreport = new List<WFHReportViewModel>();
            foreach (var item in model)
            {
                WFHreport.Add(new WFHReportViewModel()
                {
                    EmpName = item.U.Name,
                    StartDate = item.StartDate.ToString("ddd, MMM dd, yyyy"),
                    EndDate = item.EndDate.ToString("ddd, MMM dd, yyyy"),
                    TeamManager = item.U.Pmu?.Name ?? String.Empty,
                    Comment = item.Comment,
                    ApprovedBy = item.ApprovedById != null ? item.ApprovedBy.Name : String.Empty,
                    Status = item.Status.HasValue ? Enum.GetName(typeof(WFHStatus), item.Status.Value) : " ",
                    ApplyDate = item.DateAdded != null ? item.DateAdded.Value.ToString("ddd, MMM dd, yyyy") : " "
                });
            }
            string filename = "WFHDataReport_" + DateTime.Now.Ticks.ToString() + ".xlsx";
            string[] columns = { "EmpName", "StartDate", "EndDate", "TeamManager", "Comment", "ApprovedBy", "Status", "ApplyDate" };
            byte[] filecontent = ExportExcelHelper.ExportExcel(WFHreport, "WFHReport", true, columns);
            string fileName = filename;
            return File(filecontent, ExportExcelHelper.ExcelContentType, fileName);
        }
        #endregion

        #region ExecuteHrmAPI
        /// <summary>
        /// ExecuteHrmAPI method use send WFH request data to HRM API.
        /// </summary>
        /// <param name="emswfh"></param>
        /// <param name="email"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="wfhCategory"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExecuteHrmAPI(string emswfh, string email, DateTime startDate, DateTime endDate, string wfhCategory, bool isHalfDay, string action)
        {
            try
            {
                string startingDate = startDate.ToFormatDateString("yyyy-MM-dd");
                string endingDate = endDate.ToFormatDateString("yyyy-MM-dd");
                PostManager postRequest = new PostManager(SiteKey.HrmServiceURL + "/leaveAddUpdate");
                postRequest.AddHeader("Hrmapikey", SiteKey.HrmApiKey);
                postRequest.AddHeader("Hrmapipassword", SiteKey.HrmApiPassword);
                LeaveHrm data = new LeaveHrm();
                LeaveHrmRequest postData = new LeaveHrmRequest()
                {
                    emsLeaveId = emswfh,
                    emailId = email,
                    startDate = startingDate,
                    endDate = endingDate,
                    leaveType = wfhCategory,
                    isHalf = isHalfDay != true ? "0" : "1",
                    isWfh = "1",
                    actionType = action
                };
                data = postRequest.PostData<LeaveHrmRequest, LeaveHrm>(postData);

                return Json(data);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}

