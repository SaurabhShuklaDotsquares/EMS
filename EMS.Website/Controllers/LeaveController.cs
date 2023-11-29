using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Dto.SARAL;
using EMS.Service;
using EMS.Service.SARAL;
using EMS.Service.SARALDT;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.Controllers;
using EMS.Web.Models;
using EMS.Web.Models.Calendar;
using EMS.Website.Code.LIBS;
using EMS.Website.LIBS;
using EMS.Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using static EMS.Core.Enums;

namespace EMS.Website.Controllers
{
    public class LeaveController : BaseController
    {
        #region "Fields"

        private IUserLoginService userLoginService;
        private ILeaveService leaveService;
        private ILevAllotmentService levAllotmentService;
        private ILevDetailsService levDetailsService;
        private ILevMonthdetService levMonthdetService;
        private ILevAllotmentDTService levAllotmentDTService;
        private ILevDetailsDTService levDetailsDTService;
        private ILevMonthdetDTService levMonthdetDTService;
        private IWFHService wFHService;
        public int leaveadlust = 0;
        public int leaveadlustid = 0;
        string logPath = "D:/local/EMSWebCore/EMS.Website/wwwroot/Leave_Log/leave-log.txt";
        public bool isPreviousMonth = false;
        #endregion

        #region "Constructor"

        public LeaveController(IWFHService _wFHService, IUserLoginService _userLoginService, ILeaveService _manageLeaveService, ILevAllotmentService _levAllotmentService, ILevDetailsService _levDetailsService, ILevMonthdetService _levMonthdetService, ILevAllotmentDTService _levAllotmentDTService, ILevDetailsDTService _levDetailsDTService, ILevMonthdetDTService _levMonthdetDTService)
        {
            this.userLoginService = _userLoginService;
            this.leaveService = _manageLeaveService;
            this.levAllotmentService = _levAllotmentService;
            this.levDetailsService = _levDetailsService;
            this.levMonthdetService = _levMonthdetService;
            this.levAllotmentDTService = _levAllotmentDTService;
            this.levDetailsDTService = _levDetailsDTService;
            this.levMonthdetDTService = _levMonthdetDTService;
            this.wFHService = _wFHService;
        }

        #endregion
            
        [CustomActionAuthorization]
        public ActionResult Index()
        {
            bool IsPmHrPmo = (RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId) || CurrentUser.RoleId == (int)Enums.UserRoles.PM
                || RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_Sales_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_HR_DesignationIds.Contains(CurrentUser.DesignationId)
                //  || RoleValidator.DV_RoleIds.Contains(CurrentUser.RoleId)
                ) ? true : false;
            ViewBag.Users = GetEmployees(false);
            if (!IsPmHrPmo)
            {
                ViewBag.LeaveCategory = GetLeaveCategoryByGender(CurrentUser.Gender, false);
            }
            else
            {
                ViewBag.LeaveCategory = GetLeaveCategory(false);
            }
            //ViewBag.LeaveCategory = GetLeaveCategory(false);
            ViewBag.Status = typeof(Enums.LeaveStatus).EnumToDictionaryWithDescription().Select(v => new SelectListItem { Text = v.Key, Value = v.Value.ToString() }).ToList();
            ViewBag.LeaveType = typeof(Enums.LeaveType).EnumToDictionaryWithDescription().Where(v => v.Value == 15 || v.Value == 16).Select(v => new SelectListItem { Text = v.Key, Value = v.Value.ToString() }).ToList();
            ViewBag.PM = userLoginService.GetPMAndPMOHRDirectorUsers(true).Select(p => new SelectListItem { Text = p.Name, Value = p.Uid.ToString() }).ToList();
            //ViewBag.PM = userLoginService.GetPMAndPMOUsers(true).Select(p => new SelectListItem { Text = p.Name, Value = p.Uid.ToString() }).ToList();

            return View();
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult GetLeaves(IDataTablesRequest request, SpecialSearchFilterViewModel searchFilter)
        {
            TempData.Put("searchFilter", searchFilter);

            DateTime dateStart, dateEnd;

            var pagingService = new PagingService<LeaveActivity>(request.Start, request.Length);

            var expr = PredicateBuilder.True<LeaveActivity>();

            int pmId = (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO) ? CurrentUser.Uid : CurrentUser.PMUid;
            if (CurrentUser.Uid > 0)
            {
                if (CurrentUser.RoleId == (int)UserRoles.PM || CurrentUser.RoleId == (int)UserRoles.UKPM || CurrentUser.RoleId == (int)UserRoles.PMO || CurrentUser.RoleId == (int)UserRoles.UKBDM)
                {
                    expr = expr.And(e => e.Uid == CurrentUser.Uid || (e.UserLogin1.PMUid.HasValue && e.UserLogin1.PMUid.Value == pmId) || (e.UserLogin1.TLId.HasValue && e.UserLogin1.TLId.Value == pmId));
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
                    expr = expr.And(e => e.Uid == CurrentUser.Uid || (e.UserLogin1.TLId.HasValue && e.UserLogin1.TLId.Value == CurrentUser.Uid) || (sdList.Contains((int)e.UserLogin1.TLId))
                    );
                }
                //else if (RoleValidator.TL_RoleIds.Contains(CurrentUser.RoleId) || RoleValidator.DV_RoleIds.Contains(CurrentUser.RoleId))
                //{
                //    int[] sdList = userLoginService.GetTLUsers(CurrentUser.Uid).Select(T => T.Uid).ToArray();
                //    expr = expr.And(e => e.Uid == CurrentUser.Uid || (e.UserLogin1.TLId.HasValue && e.UserLogin1.TLId.Value == CurrentUser.Uid) || (sdList.Contains((int)e.UserLogin1.TLId))
                //    );
                //}
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
            if (searchFilter.leavecatagory.HasValue && searchFilter.leavecatagory.Value != 0)
            {
                expr = expr.And(l => l.LeaveCategory == searchFilter.leavecatagory.Value);
            }
            if (searchFilter.leavetype.HasValue && searchFilter.leavetype.Value != 0)
            {
                expr = expr.And(l => l.LeaveType == searchFilter.leavetype);
            }
            if (searchFilter.pm.HasValue && searchFilter.pm.Value != 0)
            {
                expr = expr.And(l => (l.UserLogin1.PMUid == searchFilter.pm) || (l.UserLogin1.TLId == searchFilter.pm));
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
                            return o.OrderByColumn(item, c => c.UserLogin1.Name);

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
            double totalLeaves = 0;
            var response = leaveService.GetLeaves(out totalCount, pagingService);

            IDictionary<string, object> additionalParameters = new Dictionary<string, object>();
            if (searchFilter.user.HasValue && searchFilter.user.Value != 0)
            {
                double sickLeaveCount = 0;
                double holidayLeaveCount = 0;
                totalLeaves = leaveService.GetTotalLeaves(pagingService);
                //if (totalLeaves>0)
                //{

                String StartDate = "", EndDate = "";
                //HolidayEdjustment(ref totalLeaves, searchFilter.user.Value, response, searchFilter.startDate, searchFilter.endDate);
                HolidayEdjustmentAndHolidayCount(ref totalLeaves, searchFilter.user.Value, response, searchFilter.startDate, searchFilter.endDate, ref sickLeaveCount, ref holidayLeaveCount, ref StartDate, ref EndDate);
                UserLogin user = userLoginService.GetUserInfoByID(searchFilter.user.Value);

                additionalParameters.Add(new KeyValuePair<string, object>("holidayLeaveUserName", user.Name));
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
                additionalParameters.Add(new KeyValuePair<string, object>("holidayLeaveCount", holidayLeaveCount));
                additionalParameters.Add(new KeyValuePair<string, object>("sickLeaveCount", sickLeaveCount));
                additionalParameters.Add(new KeyValuePair<string, object>("totalLeave", totalLeaves));
                //}
            }


            return DataTablesJsonResult(totalCount, request, response.Select((x, index) => new
            {
                rowId = request.Start + index + 1,
                userId = x.UserLogin1.Uid, //IsHrPmPmo ? x.UserLogin1.Uid : x.Uid,
                leaveId = x.LeaveId,
                startDate = x.StartDate.ToString("ddd, MMM dd, yyyy"),
                endDate = x.EndDate.ToString("ddd, MMM dd, yyyy"),
                reason = x.Reason,
                Remark = x.Remark,
                isHalf = x.IsHalf == true ? "<p class='text-blue'><b>Half " + (x.TypeMaster != null && x.TypeMaster.TypeName.ToLower().Trim() != "normal" ? "(" + x.TypeMaster.TypeName + ")" : "") + "</b></p>" : "<p class='text-red'><b>Full " + (x.TypeMaster != null && x.TypeMaster.TypeName.ToLower().Trim() != "normal" ? "(" + x.TypeMaster.TypeName + ")" : "") + "</b></p>",
                status = x.Status.HasValue ? GetStatus(x.Status.Value) : "",     //Enum.GetName(typeof(Enums.LeaveStatus), x.Status) : "",
                //typeName = x.TypeMaster != null ? x.TypeMaster.TypeName : string.Empty,
                dateAdded = x.DateAdded?.ToString("ddd, MMM dd, yyyy hh:mm tt"),
                userName = x.UserLogin1.Name, //IsHrPmPmo ? x.UserLogin1.Name : CurrentUser.Name
                leaveCategory = x.LeaveCategoryNavigation != null ? x.LeaveCategoryNavigation.Name : string.Empty,
                isSelf = (x.UserLogin.Uid == CurrentUser.Uid),
                isEdit = x.Status.HasValue ? ((((Enums.LeaveStatus)x.Status.Value == Enums.LeaveStatus.Pending) ||
                (CurrentUser.RoleId == (int)Enums.UserRoles.PM) ||
                (CurrentUser.RoleId == (int)Enums.UserRoles.PMO) ||
                RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId) || (CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM)
                  ) && ((DateTime.Now.Date <= x.StartDate.Date) || ((CurrentUser.RoleId == (int)Enums.UserRoles.PM) ||
                  (CurrentUser.RoleId == (int)Enums.UserRoles.PMO) ||
                  RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId) || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM)) ||
                  x.UserLogin1.TLId == CurrentUser.Uid ? true : false) : true,
                modifyBy = x.ModifyBy.HasValue && x.ModifyByNavigation != null ? x.ModifyByNavigation.Name : string.Empty,
                modifyDate = x.DateModify?.ToString("ddd, MMM dd, yyyy hh:mm tt")
            }), additionalParameters);
        }

        public void HolidayEdjustmentAndHolidayCount(ref double totalLeavesForDdj, int uid, List<LeaveActivity> leaveActivitys, String StartDate, String EndDate, ref double sickLeaveCount, ref double holidayLeaveCount, ref String StartDateFilter, ref String EndDateFilter)
        {
            UserLogin user = userLoginService.GetUserInfoByID(uid);
            //if (user.RoleId == (int)Enums.UserRoles.PMO || user.RoleId == (int)Enums.UserRoles.UKPM || user.RoleId == (int)Enums.UserRoles.UKBDM)
            //{

            DateTime StartDateDate = DateTime.Now;
            DateTime EndDateDate = DateTime.Now;
            double leavesCount = 0;
            int countryId = 2;
            List<OfficialLeave> _leaveList = leaveService.GetOfficialLeavesList(countryId);
            if (leaveActivitys.Count > 0)
            {
                foreach (LeaveActivity leaveActivity in leaveActivitys)
                {
                    DateTime dateStart, dateEnd;
                    if (!string.IsNullOrEmpty(StartDate) && DateTime.TryParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateStart))
                    {
                        if (dateStart < leaveActivity.StartDate)
                        {
                            dateStart = leaveActivity.StartDate;
                        }
                    }
                    else
                    {
                        dateStart = leaveActivity.StartDate;
                    }

                    if (!string.IsNullOrEmpty(EndDate) && DateTime.TryParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateEnd))
                    {
                        if (dateEnd > leaveActivity.EndDate)
                        {
                            dateEnd = leaveActivity.EndDate;
                        }
                    }
                    else
                    {
                        dateEnd = leaveActivity.EndDate;
                    }



                    //foreach (DateTime day in Common.EachDay(leaveActivity.StartDate, leaveActivity.EndDate))
                    foreach (DateTime day in Common.EachDay(dateStart, dateEnd))
                    {
                        if (leaveActivity.Status == (int)Enums.LeaveStatus.Approved || leaveActivity.Status == (int)Enums.LeaveStatus.Pending)
                        {
                            if ((day.DayOfWeek.Equals(DayOfWeek.Sunday) ||
                           day.DayOfWeek.Equals(DayOfWeek.Saturday) ||
                           _leaveList.Where(l => l.LeaveDate.Equals(day)).Count() > 0) && (user.RoleId == (int)Enums.UserRoles.PMO || user.RoleId == (int)Enums.UserRoles.UKPM || user.RoleId == (int)Enums.UserRoles.UKBDM))
                            {
                                totalLeavesForDdj -= 1;
                            }
                            else
                            {
                                if (leaveActivity.Status == (int)Enums.LeaveStatus.Pending)
                                {
                                    if (leaveActivity.IsHalf == true)
                                    {
                                        leavesCount += .5;
                                    }
                                    else
                                    {
                                        leavesCount++;
                                    }
                                }

                                if (leaveActivity.Status == (int)Enums.LeaveStatus.Approved)
                                {
                                    if (leaveActivity.HolidayType == (int)Enums.HolidayType.Holiday
                                        || leaveActivity.HolidayType == 0)
                                    {
                                        holidayLeaveCount += (leaveActivity.IsHalf == true ? 0.5 : 1);
                                    }
                                    else if (leaveActivity.HolidayType == (int)Enums.HolidayType.Sick)
                                    {
                                        sickLeaveCount += (leaveActivity.IsHalf == true ? 0.5 : 1);
                                    }
                                }
                            }
                        }
                    }

                    if (String.IsNullOrEmpty(StartDateFilter))
                    {
                        StartDateDate = dateStart;
                    }
                    else
                    {
                        if (StartDateDate > dateStart)
                        {
                            StartDateDate = dateStart;
                        }
                    }
                    if (String.IsNullOrEmpty(EndDateFilter))
                    {
                        EndDateDate = dateEnd;
                    }
                    else
                    {
                        if (EndDateDate < dateEnd)
                        {
                            EndDateDate = dateEnd;
                        }
                    }


                    StartDateFilter = StartDateDate.ToString("dd/MM/yyyy").Replace("-", "/");
                    EndDateFilter = EndDateDate.ToString("dd/MM/yyyy").Replace("-", "/");
                }
            }
            else
            {
                StartDateFilter = StartDate;
                EndDateFilter = EndDate;
            }
            totalLeavesForDdj = leavesCount;
            //}
        }

        public void HolidayEdjustmentOLD(ref double totalLeavesForDdj, int uid, List<LeaveActivity> leaveActivitys, String StartDate, String EndDate)
        {
            UserLogin user = userLoginService.GetUserInfoByID(uid);
            if (user.RoleId == (int)Enums.UserRoles.PMO || user.RoleId == (int)Enums.UserRoles.UKPM || user.RoleId == (int)Enums.UserRoles.UKBDM)
            {
                double leavesCount = 0;
                int countryId = 2;
                List<OfficialLeave> _leaveList = leaveService.GetOfficialLeavesList(countryId);
                foreach (LeaveActivity leaveActivity in leaveActivitys)
                {
                    DateTime dateStart, dateEnd;
                    if (!string.IsNullOrEmpty(StartDate) && DateTime.TryParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateStart))
                    {
                        if (dateStart < leaveActivity.StartDate)
                        {
                            dateStart = leaveActivity.StartDate;
                        }
                    }
                    else
                    {
                        dateStart = leaveActivity.StartDate;
                    }

                    if (!string.IsNullOrEmpty(EndDate) && DateTime.TryParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateEnd))
                    {
                        if (dateEnd > leaveActivity.EndDate)
                        {
                            dateEnd = leaveActivity.EndDate;
                        }
                    }
                    else
                    {
                        dateEnd = leaveActivity.EndDate;
                    }


                    //foreach (DateTime day in Common.EachDay(leaveActivity.StartDate, leaveActivity.EndDate))
                    foreach (DateTime day in Common.EachDay(dateStart, dateEnd))
                    {
                        if (day.DayOfWeek.Equals(DayOfWeek.Sunday) ||
                            day.DayOfWeek.Equals(DayOfWeek.Saturday) ||
                            _leaveList.Where(l => l.LeaveDate.Equals(day)).Count() > 0)
                        {
                            totalLeavesForDdj -= 1; //This logic will not used.
                        }
                        else
                        {
                            if (leaveActivity.IsHalf == true)
                            {
                                leavesCount += .5;
                            }
                            else
                            {
                                leavesCount++;
                            }
                        }
                    }
                }

                totalLeavesForDdj = leavesCount;
            }
        }


        public string GetStatus(int statusId)
        {
            string status = string.Empty;
            switch ((Enums.LeaveStatus)statusId)
            {
                case Enums.LeaveStatus.Approved:
                    status = "<p class='text-green'><b><i class='fa fa-check'></i> Approved</b></p>";
                    break;

                case Enums.LeaveStatus.Cancelled:
                    status = "<p class='text-red'><b><i class='fa fa-close'></i> Cancelled</b></p>";
                    break;

                case Enums.LeaveStatus.UnApproved:
                    status = "<p class='text-fuchsia'><b><i class='fa fa-remove'></i> UnApproved</b></p>";
                    break;

                case Enums.LeaveStatus.Pending:
                    status = "<p class='text-orange'><b><i class='fa fa-warning'></i> Pending</b></p>";
                    break;
            }
            return status;
        }

        //private Expression<Func<LeaveActivity, bool>> GetLeaveFilterExpersion(SpecialSearchFilterViewModel searchFilter)
        //{
        //    var expr = PredicateBuilder.True<LeaveActivity>();
        //    DateTime dateStart, dateEnd;
        //    int pmId = (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO) ? CurrentUser.Uid : CurrentUser.PMUid;
        //    if (CurrentUser.Uid > 0)
        //    {
        //        if (CurrentUser.RoleId == (int)UserRoles.PM || CurrentUser.RoleId == (int)UserRoles.UKPM || CurrentUser.RoleId == (int)UserRoles.PMO || CurrentUser.RoleId == (int)UserRoles.UKBDM)
        //        {
        //            expr = expr.And(e => e.Uid == CurrentUser.Uid || (e.UserLogin1.PMUid.HasValue && e.UserLogin1.PMUid.Value == pmId) || (e.UserLogin1.TLId.HasValue && e.UserLogin1.TLId.Value == pmId));
        //        }
        //        else if (CurrentUser.RoleId == (int)UserRoles.TL || CurrentUser.RoleId == (int)UserRoles.SD)
        //        {
        //            int[] sdList = userLoginService.GetTLUsers(CurrentUser.Uid).Select(T => T.Uid).ToArray();
        //            expr = expr.And(e => e.Uid == CurrentUser.Uid || (e.UserLogin1.TLId.HasValue && e.UserLogin1.TLId.Value == CurrentUser.Uid) || (sdList.Contains((int)e.UserLogin1.TLId))
        //            );
        //        }
        //        else
        //        {
        //            if (CurrentUser.RoleId != (int)UserRoles.HR && CurrentUser.RoleId != (int)UserRoles.OP)
        //            {
        //                expr = expr.And(e => e.Uid == CurrentUser.Uid);
        //            }
        //        }
        //    }

        //    if (searchFilter.user.HasValue && searchFilter.user.Value != 0)
        //    {
        //        expr = expr.And(l => l.Uid == searchFilter.user.Value);
        //    }
        //    if (searchFilter.status.HasValue && searchFilter.status.Value != 0)
        //    {
        //        expr = expr.And(l => l.Status == searchFilter.status.Value);
        //    }
        //    if (searchFilter.leavetype.HasValue && searchFilter.leavetype.Value != 0)
        //    {
        //        expr = expr.And(l => l.LeaveType == searchFilter.leavetype);
        //    }
        //    if (searchFilter.pm.HasValue && searchFilter.pm.Value != 0)
        //    {
        //        expr = expr.And(l => (l.UserLogin1.PMUid == searchFilter.pm) || (l.UserLogin1.TLId == searchFilter.pm));
        //    }
        //    if (!string.IsNullOrEmpty(searchFilter.startDate) && DateTime.TryParseExact(searchFilter.startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateStart))
        //    {
        //        expr = expr.And(l => l.StartDate >= dateStart || l.EndDate >= dateStart);
        //    }
        //    if (!string.IsNullOrEmpty(searchFilter.endDate) && DateTime.TryParseExact(searchFilter.endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateEnd))
        //    {
        //        expr = expr.And(l => l.EndDate <= dateEnd || l.StartDate <= dateEnd);
        //    }

        //    return expr;
        //}
        //private void HolidayType(ref double sickLeaveCount, ref double holidayLeaveCount, int uid, List<LeaveActivity> leaveActivitys)
        //{
        //    UserLogin user = userLoginService.GetUserInfoByID(uid);
        //    if (user.RoleId == (int)Enums.UserRoles.PMO || user.RoleId == (int)Enums.UserRoles.UKPM || user.RoleId == (int)Enums.UserRoles.UKBDM)
        //    {
        //        int countryId = 2;
        //        List<OfficialLeave> _leaveList = leaveService.GetOfficialLeavesList(countryId);
        //        foreach (LeaveActivity leaveActivity in _leaveList)
        //        {
        //            foreach (DateTime day in Common.EachDay(leaveActivity.StartDate, leaveActivity.EndDate))
        //            {
        //                if (day.DayOfWeek.Equals(DayOfWeek.Sunday) ||
        //                    day.DayOfWeek.Equals(DayOfWeek.Saturday) ||
        //                    _leaveList.Where(l => l.LeaveDate.Equals(day)).Count() > 0)
        //                {
        //                    totalLeavesForDdj -= 1;
        //                }
        //            }
        //        }
        //    }


        //    var searchFilter = TempData.Get<SpecialSearchFilterViewModel>("searchFilter");
        //    var expr = GetLeaveFilterExpersion(searchFilter);

        //    var pagingService = new PagingService<LeaveActivity>();
        //    pagingService.Filter = expr;

        //    int totalCount = 0;
        //    //double totalLeaves = 0;
        //    var leaveActivitys = leaveService.GetLeaves(out totalCount, pagingService);

        //    List<HolidayTypeDto> holidayTypeList = new List<HolidayTypeDto>();

        //    if (searchFilter.user.HasValue && searchFilter.user.Value != 0)
        //    {
        //        UserLogin user = userLoginService.GetUserInfoByID(searchFilter.user.Value);
        //        double sickLeaveCount = 0;
        //        double holidayLeaveCount = 0;
        //        foreach (LeaveActivity leaveActivity in leaveActivitys)
        //        {
        //            foreach (DateTime day in Common.EachDay(leaveActivity.StartDate, leaveActivity.EndDate))
        //            {
        //                if (leaveActivity.HolidayType == (int)Enums.HolidayType.Holiday)
        //                {
        //                    holidayLeaveCount += (leaveActivity.IsHalf == true ? 0.5 : 1);
        //                }
        //                else if (leaveActivity.HolidayType == (int)Enums.HolidayType.Sick)
        //                {
        //                    sickLeaveCount += (leaveActivity.IsHalf == true ? 0.5 : 1);
        //                }
        //            }
        //        }
        //        holidayTypeList.Add(new HolidayTypeDto
        //        {
        //            UID = user.Uid,
        //            UName = user.Name,
        //            Sick = sickLeaveCount,
        //            Holiday = holidayLeaveCount
        //        });
        //    }

        //    return holidayTypeList;
        //}


        //[HttpPost]
        //public PartialViewResult HolidayType(SpecialSearchFilterViewModel searchFilter)
        //{
        //    //var searchFilter = TempData.Get<SpecialSearchFilterViewModel>("searchFilter");
        //    var expr = GetLeaveFilterExpersion(searchFilter);

        //    var pagingService = new PagingService<LeaveActivity>();
        //    pagingService.Filter = expr;

        //    int totalCount = 0;
        //    //double totalLeaves = 0;
        //    var leaveActivitys = leaveService.GetLeaves(out totalCount, pagingService);

        //    List<HolidayTypeDto> holidayTypeList = new List<HolidayTypeDto>();

        //    if (searchFilter.user.HasValue && searchFilter.user.Value != 0)
        //    {
        //        UserLogin user = userLoginService.GetUserInfoByID(searchFilter.user.Value);
        //        double sickLeaveCount = 0;
        //        double holidayLeaveCount = 0;
        //        foreach (LeaveActivity leaveActivity in leaveActivitys)
        //        {
        //            foreach (DateTime day in Common.EachDay(leaveActivity.StartDate, leaveActivity.EndDate))
        //            {
        //                if (leaveActivity.HolidayType == (int)Enums.HolidayType.Holiday)
        //                {
        //                    holidayLeaveCount += (leaveActivity.IsHalf == true ? 0.5 : 1);
        //                }
        //                else if (leaveActivity.HolidayType == (int)Enums.HolidayType.Sick)
        //                {
        //                    sickLeaveCount += (leaveActivity.IsHalf == true ? 0.5 : 1);
        //                }
        //            }
        //        }
        //        holidayTypeList.Add(new HolidayTypeDto {
        //            UID = user.Uid,
        //            UName = user.Name,
        //            Sick = sickLeaveCount,
        //            Holiday = holidayLeaveCount
        //        });
        //    }


        //    return PartialView("_HolidayType", holidayTypeList);
        //}



        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Calendar()
        {
            if (RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId))
            {
                //ViewBag.PmList = userLoginService.GetPMAndPMOUsers(true);
                ViewBag.PmList = userLoginService.GetPMAndPMOHRDirectorUsers(true);
            }
            else
            {
                ViewBag.PmId = (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO) ? CurrentUser.Uid : CurrentUser.PMUid;
            }
            //var users = leaveService.GetLeavesEmployeeListByUid(CurrentUser.Uid, CurrentUser.RoleId);
            ViewBag.UnderEmployeeList = GetEmployees(false); //users.Count > 0 ? users.Select(n => new KeyValuePair<string, int>(n.UserLogin1.Name, n.Uid)).ToList().Distinct() : null;
            if (!(CurrentUser.RoleId == (int)Enums.UserRoles.HRBP || CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO
                //|| RoleValidator.DV_RoleIds.Contains(CurrentUser.RoleId) 
                || RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_Sales_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_HR_DesignationIds.Contains(CurrentUser.DesignationId)
                || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM) && (dynamic)ViewBag.UnderEmployeeList.Count <= 0)
            {
                ViewBag.UnderEmployeeList = null;
            }
            bool IsPmHrPmo = (RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId) || CurrentUser.RoleId == (int)Enums.UserRoles.PM
                || RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_Sales_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_HR_DesignationIds.Contains(CurrentUser.DesignationId)) ? true : false;
            ViewBag.Users = GetEmployees(false);
            if (!IsPmHrPmo)
            {
                ViewBag.LeaveCategory = GetLeaveCategoryByGender(CurrentUser.Gender, false);
            }
            else
            {
                ViewBag.LeaveCategory = GetLeaveCategory(false);
            }
            return View();
        }

        public ActionResult LeaveCalender(string steps, int leaveType, string pmid, string uid, int leaveCategory)
        {
            int month = DateTime.UtcNow.Month;
            int year = DateTime.UtcNow.Year;
            DateTime startDate = new DateTime(year, month, 1);

            if (TempData["CalendarMonth"] == null || TempData["CalendarYear"] == null)
            {
                TempData["CalendarMonth"] = startDate.Month;
                TempData["CalendarYear"] = startDate.Year;
            }

            if (!string.IsNullOrEmpty(steps) && steps.ToLower() == "next")
            {
                int? lastmonth = (int)TempData["CalendarMonth"];
                int? lastyear = (int)TempData["CalendarYear"];
                DateTime lastDate = new DateTime(lastyear.HasValue ? lastyear.Value : year, lastmonth.HasValue ? lastmonth.Value : month, 1);
                startDate = lastDate.AddMonths(1);
                month = startDate.Month;
                year = startDate.Year;
            }
            else if (!string.IsNullOrEmpty(steps) && steps.ToLower() == "previous")
            {
                month = (int)TempData["CalendarMonth"];
                year = (int)TempData["CalendarYear"];
                DateTime lastDate = new DateTime(year, month, 1);
                startDate = lastDate.AddMonths(-1);
                month = startDate.Month;
                year = startDate.Year;
            }
            else if ((TempData["PageReloaded"] != null && (bool)TempData["PageReloaded"]) || (!string.IsNullOrEmpty(steps) && steps.ToLower() == "reload"))
            {
                month = (int)TempData["CalendarMonth"];
                year = (int)TempData["CalendarYear"];
                startDate = new DateTime(year, month, 1);
                TempData["PageReloaded"] = false;
            }

            //var isPmOrHr = false;
            //isPmOrHr = (CurrentUser.RoleId == (int)Enum.Parse(typeof(Core.Enums.UserRoles), Core.Enums.UserRoles.PM.ToString())) ? true :

            //    (CurrentUser.RoleId == (int)Enum.Parse(typeof(Core.Enums.UserRoles), Core.Enums.UserRoles.PMO.ToString())) ? true :

            //    (CurrentUser.RoleId == (int)Enum.Parse(typeof(Core.Enums.UserRoles), Core.Enums.UserRoles.HR.ToString())) ? true :
            //    false;
            ////var leaves = leaveActivityService.GetLeaveActivityByUidAndMonth(year, month, CurrentUser.Uid, isPmOrHr, leaveType: leaveType);
            int uidFilter = 0, pmidFilter = 0;
            int.TryParse(uid, out uidFilter);
            int.TryParse(pmid, out pmidFilter);

            if (CurrentUser.RoleId == (int)UserRoles.UKBDM && pmidFilter == 0)
            {
                pmidFilter = CurrentUser.PMUid;
            }

            var pagingService = new PagingService<LeaveActivity>();
            var pagingService1 = new PagingService<LateHour>();

            var expr = PredicateBuilder.True<LeaveActivity>();
            var expr1 = PredicateBuilder.True<LateHour>();

            expr = expr.And(e => (e.StartDate.Year == year || e.EndDate.Year == year));
            expr1 = expr1.And(e => (e.DayOfDate.Year == year));

            expr = expr.And(e => (e.StartDate.Month == month || e.EndDate.Month == month));
            expr1 = expr1.And(e => (e.DayOfDate.Month == month));

            int pmId = (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO) ? CurrentUser.Uid : CurrentUser.PMUid;

            if (CurrentUser.RoleId == (int)UserRoles.PM || CurrentUser.RoleId == (int)UserRoles.PMO || CurrentUser.RoleId == (int)UserRoles.UKPM)
            {
                expr = expr.And(e => e.Uid == CurrentUser.Uid || (e.UserLogin1.PMUid.HasValue && e.UserLogin1.PMUid.Value == pmId) || (e.UserLogin1.TLId.HasValue && e.UserLogin1.TLId.Value == pmId));
                expr1 = expr1.And(e => e.Uid == CurrentUser.Uid || (e.UserLogin1.PMUid.HasValue && e.UserLogin1.PMUid.Value == pmId) || (e.UserLogin1.TLId.HasValue && e.UserLogin1.TLId.Value == pmId));
            }
            else if (RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_Sales_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
              //|| RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(CurrentUser.DesignationId)
              //|| RoleValidator.TL_HR_DesignationIds.Contains(CurrentUser.DesignationId)
              //|| RoleValidator.DV_RoleIds.Contains(CurrentUser.RoleId)
              )
            {
                int[] sdList = userLoginService.GetTLUsers(CurrentUser.Uid).Select(T => T.Uid).ToArray();
                expr = expr.And(e => e.Uid == CurrentUser.Uid || (e.UserLogin1.TLId.HasValue && e.UserLogin1.TLId.Value == CurrentUser.Uid) || (sdList.Contains((int)e.UserLogin1.TLId)));
                expr1 = expr1.And(e => e.Uid == CurrentUser.Uid || (e.UserLogin1.TLId.HasValue && e.UserLogin1.TLId.Value == CurrentUser.Uid) || (sdList.Contains((int)e.UserLogin.TLId)));
            }
            else if (CurrentUser.RoleId == (int)UserRoles.UKBDM)
            {
                expr = expr.And(e => e.UserLogin1.PMUid.HasValue && e.UserLogin1.PMUid.Value == pmidFilter);
                expr1 = expr1.And(e => e.UserLogin1.PMUid.HasValue && e.UserLogin1.PMUid.Value == pmidFilter);
            }
            else
            {
                if (!RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId))
                {
                    expr = expr.And(e => e.Uid == CurrentUser.Uid);
                    expr1 = expr1.And(e => e.Uid == CurrentUser.Uid);
                }
            }



            expr = expr.And(l => l.UserLogin1.IsActive == true);
            expr1 = expr1.And(l => l.UserLogin1.IsActive == true);

            if (leaveType > 0)
            {
                expr = expr.And(l => l.LeaveType == leaveType);
                expr1 = expr1.And(l => l.LeaveType == leaveType);
            }
            if (leaveCategory > 0)
            {
                expr = expr.And(l => l.LeaveCategory == leaveCategory);
            }
            if (uidFilter > 0)
            {
                expr = expr.And(l => l.Uid == uidFilter);
                expr1 = expr1.And(l => l.Uid == uidFilter);
            }
            else if (pmidFilter > 0)
            {
                expr = expr.And(l => (l.UserLogin1.PMUid == pmidFilter) || (l.UserLogin1.TLId == pmidFilter) || (l.Uid == pmidFilter));
                expr1 = expr1.And(l => (l.UserLogin1.PMUid == pmidFilter) || (l.UserLogin1.TLId == pmidFilter) || (l.Uid == pmidFilter));
            }
            //Uk role leave status
            if (CurrentUser.RoleId == (int)Enums.UserRoles.PMO || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM)
            {
                pagingService = new PagingService<LeaveActivity>();
                expr = PredicateBuilder.True<LeaveActivity>();

                List<int?> lRoles = new List<int?>();
                lRoles.Add((int)Enums.UserRoles.PMO);
                lRoles.Add((int)Enums.UserRoles.UKPM);
                lRoles.Add((int)Enums.UserRoles.UKBDM);
                expr = expr.And(e => (e.StartDate.Year == year || e.EndDate.Year == year));
                expr = expr.And(e => (e.StartDate.Month == month || e.EndDate.Month == month));
                expr = expr.And(l => lRoles.Contains(l.UserLogin1.RoleId));
                expr = expr.And(l => l.UserLogin1.IsActive == true);
            }
            pagingService.Filter = expr;
            pagingService1.Filter = expr1;
            var leaves = leaveService.GetLeaveActivityByUidAndMonth(pagingService).Select(l => new LeaveCalenderDto
            {
                Name = l.UserLogin1.Name,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                IsHalf = (l.IsHalf.HasValue && l.IsHalf.Value),
                LeaveId = l.LeaveId,
                LeaveType = l.LeaveType,
                Status = l.Status,
                LeaveCategory = l.LeaveCategory.HasValue ? l.LeaveCategory : (int)Enums.LeaveCategory.UnpaidLeave
            }).ToList();

            var lateHourList = new List<LateHour>();
            if (CurrentUser.RoleId == (int)UserRoles.PMO || CurrentUser.RoleId == (int)UserRoles.UKBDM)
            {
                lateHourList = leaveService.GetFilterLateHourList(pagingService1).ToList();
            }

            int daysInMonth = DateTime.DaysInMonth(year, month);

            CalendarMonthWithWeek model = GetLeavesInfoOfMonth(leaves, lateHourList, month, year);

            if (DateTime.Today.Month == month && DateTime.Today.Year == year)
            {
                model.DisablePrevious = true;
            }

            TempData["CalendarMonth"] = startDate.Month;
            TempData["CalendarYear"] = startDate.Year;



            if (CurrentUser.RoleId == (int)UserRoles.UKPM || CurrentUser.RoleId == (int)UserRoles.UKBDM || CurrentUser.RoleId == (int)UserRoles.PMO)
            {
                ViewBag.OfficialLeave = leaveService.GetOfficialEventLeaveList((byte)Country.UK).Select(l => new OfficialLeaveDto { CountryId = l.CountryId, IsActive = l.IsActive, LeaveDate = l.LeaveDate, LeaveId = l.LeaveId, Title = l.Title, LeaveType = l.LeaveType }).ToList();
                ViewBag.IsUKTeam = true;
            }
            else
            {
                ViewBag.OfficialLeave = leaveService.GetOfficialLeaves((byte)Country.India, true).Select(l => new OfficialLeaveDto { CountryId = l.CountryId, IsActive = l.IsActive, LeaveDate = l.LeaveDate, LeaveId = l.LeaveId, Title = l.Title, LeaveType = l.LeaveType }).ToList();
            }

            ViewBag.totalLeave = "";
            if (uidFilter > 0)
            {
                //UserLogin user = userLoginService.GetUserInfoByID(uidFilter);
                //if (user.RoleId == (int)Enums.UserRoles.PMO || user.RoleId == (int)Enums.UserRoles.UKPM || user.RoleId == (int)Enums.UserRoles.UKBDM)
                //{
                int totalCount = 0;
                List<LeaveActivity> leaveActivitys = leaveService.GetLeaves(out totalCount, pagingService);
                double totalLeaves = leaveService.GetTotalLeaves(pagingService);
                //if (totalLeaves > 0)
                //{
                //HolidayEdjustment(ref totalLeaves, uidFilter, leaveActivitys);
                double sickLeaveCount = 0;
                double holidayLeaveCount = 0;
                String StartDate = "", EndDate = "";
                HolidayEdjustmentAndHolidayCount(ref totalLeaves, uidFilter, leaveActivitys, (new DateTime(year, month, 1)).ToString("dd/MM/yyyy").Replace("-", "/"), (new DateTime(year, month, 1).AddMonths(1).AddDays(-1)).ToString("dd/MM/yyyy").Replace("-", "/"), ref sickLeaveCount, ref holidayLeaveCount, ref StartDate, ref EndDate);

                ViewBag.totalLeave = totalLeaves + ""; // + (totalLeaves == 1 ? " Day" : " Days");
                ViewBag.holidayLeaveCount = holidayLeaveCount;
                ViewBag.sickLeaveCount = sickLeaveCount;
                UserLogin user = userLoginService.GetUserInfoByID(uidFilter);
                ViewBag.holidayLeaveUserName = user.Name;
                ViewBag.startDate = StartDate;
                ViewBag.endDate = EndDate;
                if (user.RoleId == (int)Enums.UserRoles.PMO || user.RoleId == (int)Enums.UserRoles.UKPM || user.RoleId == (int)Enums.UserRoles.UKBDM)
                {
                    ViewBag.isIndianUser = "0";
                }
                else
                {
                    ViewBag.isIndianUser = "1";
                }
                //}
            }

            return PartialView("_LeaveCalender", model);
        }

        private CalendarMonthWithWeek GetLeavesInfoOfMonth(List<LeaveCalenderDto> leaves, List<LateHour> lateHourList, int month, int year)
        {
            CalendarMonthWithWeek calendarMonth = new CalendarMonthWithWeek();
            Calendar geoCalendar = CultureInfo.CurrentCulture.Calendar;
            calendarMonth.MonthName = String.Format("{0} {1}", ((EMS.Core.Enums.Month)month).ToString().Substring(0, 3), year.ToString());
            IEnumerable<int> daysInMonth = Enumerable.Range(1, geoCalendar.GetDaysInMonth(year, month));

            List<Tuple<DateTime, DateTime>> weeks = daysInMonth.Select(day => new DateTime(year, month, day))
                .GroupBy(d => geoCalendar.GetWeekOfYear(d, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday))
                .Select(g => new Tuple<DateTime, DateTime>(g.First(), g.Last()))
                .ToList();

            weeks.ForEach(w =>
            {
                MonthWeek monthweek = new MonthWeek();

                if (w.Item1.Day == 1)
                {
                    for (int i = (w.Item2.Day - w.Item1.Day + 1); i < 7; i++)
                    {
                        monthweek.calendarDays.Add(new CalendarDay { Day = "" });
                    }
                }
                for (var monthDate = w.Item1; monthDate <= w.Item2; monthDate = monthDate.AddDays(1))
                {
                    CalendarDay calendarDay = new CalendarDay();
                    calendarDay.Date = monthDate.Date;
                    calendarDay.Day = monthDate.Day.ToString();
                    int dayOfWeek = (int)monthDate.DayOfWeek;

                    var selectedLeaves = leaves.Where(m => m.StartDate <= monthDate && m.EndDate >= monthDate).ToList();
                    if (selectedLeaves != null && selectedLeaves.Any())
                    {
                        calendarDay.EmployeeWithLeaveId = selectedLeaves.Select(n => new KeyValuePair<string, int>(n.Name, n.LeaveId)).ToList();

                        calendarDay.CalenderDayList = selectedLeaves.Select(n => new Tuple<string, int, int, int>(n.Name,
                            n.LeaveId,
                            n.LeaveType.HasValue ? n.LeaveType.Value : 0,
                            n.Status.HasValue ? n.Status.Value : 0)).ToList();
                    }

                    //Late hour show in calender
                    var selectedadjusthour = lateHourList.Where(m => m.DayOfDate <= monthDate && m.DayOfDate >= monthDate).ToList();
                    if (selectedadjusthour != null && selectedadjusthour.Any())
                    {
                        calendarDay.CalenderDayAdjustHourList = selectedadjusthour.Select(n => new Tuple<string, int, string, string, string>(
                            n.UserLogin1.Name,
                            n.Id,
                            n.LateStartTimeDiff != null ? (n.LateStartTimeDiff.Value.Hours > 0 ? n.LateStartTimeDiff.Value.Hours.ToString() + " hr " : "") + (n.LateStartTimeDiff.Value.Minutes > 0 ? n.LateStartTimeDiff.Value.Minutes.ToString() + " min" : "") : "",
                            n.EarlyLeaveTimeDiff != null ? (n.EarlyLeaveTimeDiff.Value.Hours > 0 ? n.EarlyLeaveTimeDiff.Value.Hours.ToString() + " hr " : "") + (n.EarlyLeaveTimeDiff.Value.Minutes > 0 ? n.EarlyLeaveTimeDiff.Value.Minutes.ToString() + " min" : "") : "",
                            n.WorkAtHome != null ? n.WorkAtHome : null
                            )).ToList();
                    }

                    monthweek.calendarDays.Add(calendarDay);
                }
                if (monthweek.calendarDays.Count < 7)
                {
                    for (int i = (w.Item2.Day - w.Item1.Day + 1); i < 7; i++)
                    {
                        monthweek.calendarDays.Add(new CalendarDay { Day = "" });
                    }
                }
                calendarMonth.calendarWeeks.Add(monthweek);
            });
            return calendarMonth;
        }

        [HttpGet]
        public ActionResult ManageLeave(string returnview, int? id)
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

            //Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(string.Format("en-GB"));

            Dictionary<string, string> hdnFields = new Dictionary<string, string>();
            LeaveActivity leaveDB = new LeaveActivity();
            LeaveActivityDto leaveDto = new LeaveActivityDto();

            hdnFields.Add("returnview", returnview ?? "");
            Preference objPreference = leaveService.GetPreferecesByPMUid((CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO) ? CurrentUser.Uid : CurrentUser.PMUid);
            if (objPreference != null)
            {
                hdnFields.Add("hdnDays", objPreference.PriorLeaveDay != null ? objPreference.PriorLeaveDay.ToString() : "5");
            }
            else
            {
                hdnFields.Add("hdnDays", "5");
            }
            if (RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId))
            {
                //ViewBag.PMList = userLoginService.GetPMAndPMOHRUsers().Select(p => new SelectListItem { Text = p.Name, Value = p.Uid.ToString() }).ToList();
                ViewBag.PMList = userLoginService.GetPMAndPMOHRDirectorUsers(true).Select(p => new SelectListItem { Text = p.Name, Value = p.Uid.ToString() }).ToList();
            }


            try
            {
                if (IsEdit)
                {
                    #region "Binding Data Model to View Model"

                    leaveDB = leaveService.GetLeaveById(id.Value);

                    leaveDto.Uid = leaveDB.Uid;
                    leaveDto.PMid = leaveDB.UserLogin.PMUid.HasValue ? leaveDB.UserLogin.PMUid.Value : 0;
                    leaveDto.userId = leaveDB.Uid;
                    leaveDto.LeaveId = leaveDB.LeaveId;
                    leaveDto.StartDate = leaveDB.StartDate != null ? leaveDB.StartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
                    leaveDto.EndDate = leaveDB.EndDate != null ? leaveDB.EndDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
                    leaveDto.WorkAlterID = Convert.ToInt32(leaveDB.WorkAlterId);
                    leaveDto.WorkAlternatorName = leaveDB.UserLogin.Name;
                    leaveDto.WorkAlternator = leaveDB.WorkAlternator;
                    leaveDto.Reason = leaveDB.Reason;
                    leaveDto.Remark = leaveDB.Remark;
                    leaveDto.IsHalf = leaveDB.IsHalf != null ? leaveDB.IsHalf.Value : false;
                    leaveDto.LeaveType = leaveDB.LeaveType ?? 0;
                    leaveDto.DateAdded = leaveDB.DateAdded ?? DateTime.Now;
                    leaveDto.DateModify = leaveDB.DateModify ?? DateTime.Now;
                    leaveDto.Status = leaveDB.Status ?? 0;
                    leaveDto.IP = GeneralMethods.Getip();
                    leaveDto.IsCancel = (leaveDto.Status == (int)LeaveStatus.Cancelled);
                    leaveDto.IsSelfLeave = leaveDB.Uid == CurrentUser.Uid;
                    if (CurrentUser.RoleId != (int)UserRoles.UKBDM && CurrentUser.RoleId != (int)UserRoles.UKPM && CurrentUser.RoleId != (int)UserRoles.AUPM && CurrentUser.RoleId != (int)UserRoles.PMOAU)
                    {
                        leaveDto.LeaveCategory = leaveDB.LeaveCategory.HasValue && (int)leaveDB.LeaveCategory.Value > 0 ? (int)leaveDB.LeaveCategory : (int)LeaveCategory.CasualLeave;
                    }
                    else
                    {
                        leaveDto.LeaveCategory = (int)LeaveCategory.NA;
                    }
                    leaveDto.FirstHalf = leaveDB.FirstHalf != null ? (bool)leaveDB.FirstHalf ? true : false : false;
                    leaveDto.SecondHalf = leaveDB.SecondHalf != null ? (bool)leaveDB.SecondHalf ? true : false : false;
                    leaveDto.HalfValue = leaveDto.FirstHalf ? 1 : leaveDto.SecondHalf ? 2 : 0;
                    hdnFields.Add("hdnTLId", leaveDB.UserLogin1.TLId.ToString());
                    hdnFields.Add("hdnAddDT", leaveDB.DateAdded.ToString() ?? DateTime.Now.ToString("dd/MM/yyyy"));

                    leaveDto.HolidayType = leaveDB.HolidayType ?? (int)Enums.HolidayType.Holiday;

                    #endregion
                }
                else
                {
                    leaveDto.LeaveType = (int)Enums.LeaveType.Normal;
                    leaveDto.Status = (int)LeaveStatus.Pending;
                    hdnFields.Add("hdnTLId", CurrentUser.TLId.ToString());
                    hdnFields.Add("hdnAddDT", DateTime.Now.ToString("dd/MM/yyyy"));
                    leaveDto.userId = 0;
                    leaveDto.HolidayType = (int)Enums.HolidayType.Holiday;
                }

                #region  fill ddlWorkAlternator

                leaveDto.HolidayTypeList = WebExtensions.GetSelectList<Enums.HolidayType>();
                var halfType = new List<SelectListItem>() { new SelectListItem { Text = "First Half", Value = "1" }, new SelectListItem { Text = "Second Half", Value = "2" } };
                halfType.Insert(0, new SelectListItem() { Text = "-Select-", Value = "" });
                leaveDto.HalfType = halfType;
                ViewBag.HdnFields = hdnFields;
                //leaveDto.selectWAList = GetWorkAltoernators();
                if (!IsPmHrPmo)
                {
                    leaveDto.selectWAList = GetWorkAltoernators();
                    leaveDto.LeaveCategoryList = GetLeaveCategoryByGender(CurrentUser.Gender);
                }
                else
                {
                    leaveDto.selectWAList = GetWorkAltoernators(leaveDto.PMid);
                    leaveDto.LeaveCategoryList = GetLeaveCategory(true);
                }
                leaveDto.selectEmployeeList = IsPmHrPmo ? GetEmployees() : null;

                #endregion

                leaveDto.IsAllowLeave = true;

                var pmId = (CurrentUser.RoleId == (int)Enums.UserRoles.PM ? CurrentUser.Uid : CurrentUser.PMUid);

                if (pmId > 0)
                {
                    var _prefereces = leaveService.GetPreferecesByPMUid(pmId);
                    if (_prefereces != null)
                    {
                        if (_prefereces.IsAllowLeaveByTL && CurrentUser.RoleId != (int)Enums.UserRoles.PM)
                        {
                            leaveDto.IsAllowLeave = false;
                        }

                    }
                }
                //if (!IsPmHrPmo)
                //{
                CurrentLeaveDto leaveData = new CurrentLeaveDto();


                if (IsEdit)
                {
                    leaveData = GetCurrentLeaveBalance(GetAllLeaveBalance(!IsPmHrPmo ? CurrentUser.Uid : leaveDto.Uid), GetLeaveBalance(!IsPmHrPmo ? CurrentUser.Uid : leaveDto.Uid), 0, IsEdit);
                    var leave = leaveService.GetLeaveActivityById(id.Value);
                    if (leave != null)
                    {
                        var totalDays = leave.IsHalf == true ? ((leave.EndDate - leave.StartDate).TotalDays + 1) * 0.5 : (leave.EndDate - leave.StartDate).TotalDays + 1;

                        if (leave.LeaveCategory.HasValue && leave.LeaveCategory > 0)
                        {
                            switch (leave.LeaveCategory.Value)
                            {
                                case (int)LeaveCategory.CompensatoryOff:
                                    leaveData.CompensatoryOff = leaveData.CompensatoryOff + totalDays;
                                    break;
                                case (int)LeaveCategory.CasualLeave:
                                    leaveData.CasualLeave = leaveData.CasualLeave + totalDays;
                                    break;
                                case (int)LeaveCategory.PaternityLeave:
                                    leaveData.PaternityLeave = leaveData.PaternityLeave + totalDays;
                                    break;
                                case (int)LeaveCategory.MaternityLeave:
                                    leaveData.MaternityLeave = leaveData.MaternityLeave + totalDays;
                                    break;
                                case (int)LeaveCategory.EarnedLeave:
                                    leaveData.EarnedLeave = leaveData.EarnedLeave + totalDays;
                                    break;
                                case (int)LeaveCategory.SickLeave:
                                    leaveData.SickLeave = leaveData.SickLeave + totalDays;
                                    break;
                                case (int)LeaveCategory.BereavementLeave:
                                    leaveData.BereavementLeave = leaveData.BereavementLeave + totalDays;
                                    break;
                                case (int)LeaveCategory.WeddingLeave:
                                    leaveData.WeddingLeave = leaveData.WeddingLeave + totalDays;
                                    break;
                                case (int)LeaveCategory.UnpaidLeave:
                                    leaveData.LossPayLeave = leaveData.LossPayLeave + totalDays;
                                    break;

                            }
                        }

                    }
                }
                else
                {
                    leaveData = GetCurrentLeaveBalance(GetAllLeaveBalance(!IsPmHrPmo ? CurrentUser.Uid : leaveDto.Uid), GetCurrentMonthLeaveBalance(GetLeaveBalance(!IsPmHrPmo ? CurrentUser.Uid : leaveDto.Uid), GetPendingLeave(!IsPmHrPmo ? CurrentUser.Uid : leaveDto.Uid)), 0);
                }
                hdnFields.Add("hdnLeavesCL", (leaveData.CasualLeave ?? 0).ToString());
                hdnFields.Add("hdnLeavesEL", (leaveData.EarnedLeave ?? 0).ToString());
                hdnFields.Add("hdnLeavesAL", (leaveData.LossPayLeave ?? 0).ToString());
                hdnFields.Add("hdnLeavesSL", (leaveData.SickLeave ?? 0).ToString());
                hdnFields.Add("hdnLeavesBL", (leaveData.BereavementLeave ?? 0).ToString());
                hdnFields.Add("hdnLeavesWL", (leaveData.WeddingLeave ?? 0).ToString());
                hdnFields.Add("hdnLeavesLL", (leaveData.LoyaltyLeave ?? 0).ToString());
                hdnFields.Add("hdnLeavesPL", (leaveData.PaternityLeave ?? 0).ToString());
                hdnFields.Add("hdnLeavesML", (leaveData.MaternityLeave ?? 0).ToString());
                hdnFields.Add("hdnLeavesCO", (leaveData.CompensatoryOff ?? 0).ToString());


            }
            catch (Exception ex)
            {
            }
            return View(leaveDto);
        }

        public ActionResult CheckforOtherLeave(int uid, string StartDate, string EndDate)
        {
            var result = new { needToShowMessage = false, message = "" };
            var user = userLoginService.GetUserInfoByID(uid);
            if (user.RoleId == (int)Enums.UserRoles.UKPM || user.RoleId == (int)Enums.UserRoles.UKBDM)
            {
                DateTime startdate, enddate;
                startdate = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).Date;
                enddate = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).Date;
                var results = leaveService.GetLeaveActivity(l => l.UserLogin1.RoleId == user.RoleId && l.Uid != uid && ((startdate >= l.StartDate && startdate <= l.EndDate) || (enddate >= l.StartDate && enddate <= l.EndDate) || (startdate <= l.StartDate && enddate >= l.EndDate)));
                if (results.Count() > 0)
                {
                    var Names = string.Join(", ", results.Select(a => a.UserLogin1.Name).Distinct().ToList());
                    result = new { needToShowMessage = true, message = Names + " already applied the leave for same time period, are you sure you want to apply the leave?" };
                }
            }
            return Json(result);
        }

        private List<SelectListItem> GetWorkAltoernators(int pmUid = 0)
        {
            int pmId = (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO || RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId)) ? CurrentUser.Uid : CurrentUser.PMUid;
            int roleId = CurrentUser.RoleId;
            var WorkAlterList = pmUid > 0 ? userLoginService.GetWorkAlternators(pmUid, CurrentUser.Uid, roleId).OrderBy(x => x.Name) : userLoginService.GetWorkAlternators(pmId, CurrentUser.Uid, roleId).OrderBy(x => x.Name);
            var selectWAList = WorkAlterList.Select(x => new SelectListItem { Text = x.EmpCode != null ? x.Name.ToString() + " [" + x.EmpCode + "]" : x.Name, Value = x.Uid.ToString(), Selected = (x.Uid == WorkAlterList.FirstOrDefault().Uid ? true : false) }).ToList();
            selectWAList.Insert(0, new SelectListItem() { Text = "-Select-", Value = "" });
            return selectWAList;
        }
        private List<SelectListItem> GetLeaveCategory(bool selectDefault = true)
        {
            //var leaveCategory = leaveService.GetLeaveCategoryList();
            var leaveCategory = levDetailsService.GetLeaveTypeList();
            //var leaveCategorylist = leaveCategory.Select(x => new SelectListItem { Text = x.Name != null ? x.Name.ToString() + " [" + x.ShortName + "]" : x.Name, Value = x.Id.ToString(), Selected = selectDefault ? (x.Id == leaveCategory.FirstOrDefault().Id ? true : false) : false }).ToList();
            var leaveCategorylist = leaveCategory.Select(x => new SelectListItem { Text = x.Levname != null ? x.Levname.ToString() + " [" + x.Levshort + "]" : x.Levname, Value = x.Levid.ToString(), Selected = selectDefault ? (x.Levid == leaveCategory.FirstOrDefault().Levid ? true : false) : false }).ToList();
            if (selectDefault)
            {
                leaveCategorylist.Insert(0, new SelectListItem() { Text = "-Select-", Value = "" });
            }
            return leaveCategorylist;
        }
        private List<SelectListItem> GetLeaveCategoryByGender(string gender, bool selectDefault = true)
        {
            //var leaveCategory = leaveService.GetLeaveCategoryList();
            var leaveCategory = levDetailsService.GetLeaveTypeListByGender(gender);
            //var leaveCategorylist = leaveCategory.Select(x => new SelectListItem { Text = x.Name != null ? x.Name.ToString() + " [" + x.ShortName + "]" : x.Name, Value = x.Id.ToString(), Selected = selectDefault ? (x.Id == leaveCategory.FirstOrDefault().Id ? true : false) : false }).ToList();
            var leaveCategorylist = leaveCategory.Select(x => new SelectListItem { Text = x.Levname != null ? x.Levname.ToString() + " [" + x.Levshort + "]" : x.Levname, Value = x.Levid.ToString(), Selected = selectDefault ? (x.Levid == leaveCategory.FirstOrDefault().Levid ? true : false) : false }).ToList();
            if (selectDefault)
            {
                leaveCategorylist.Insert(0, new SelectListItem() { Text = "-Select-", Value = "" });
            }
            return leaveCategorylist;
        }

        private List<SelectListItem> GetEmployees(bool selectDefault = true)
        {
            var EmployeeList = userLoginService.GetUsersListByAllDesignation(CurrentUser.Uid, CurrentUser.RoleId, CurrentUser.DesignationId);
            var selectEmpList = EmployeeList.Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.Uid.ToString(), Selected = selectDefault ? (x.Uid == EmployeeList.FirstOrDefault().Uid ? true : false) : false }).ToList();
            if (selectDefault)
                selectEmpList.Insert(0, new SelectListItem() { Text = "-Select-", Value = "" });
            return selectEmpList;
        }

        public LeaveActivityDto FillData(LeaveActivityDto leaveModelForm, string returnview)
        {
            bool IsEdit = leaveModelForm.LeaveId > 0 ? true : false;
            //bool IsPmHrPmo = (CurrentUser.RoleId == (int)Enums.UserRoles.OP || CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO || CurrentUser.RoleId == (int)Enums.UserRoles.TL) ? true : false;

            //UK PM Leave Apply Issue for date 27-Nov-2019 : Issue resolved date 21-Nov-2019.
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

            LeaveActivity leaveDB = leaveService.GetLeaveById(leaveModelForm.LeaveId);
            Dictionary<string, string> hdnFields = new Dictionary<string, string>();
            if (!IsPmHrPmo)
            {
                CurrentLeaveDto leaveData = new CurrentLeaveDto();
                if (IsEdit)
                {
                    leaveData = GetCurrentLeaveBalance(GetAllLeaveBalance(CurrentUser.Uid), GetLeaveBalance(CurrentUser.Uid), 0, IsEdit);
                    var leave = leaveService.GetLeaveActivityById(leaveModelForm.LeaveId);
                    if (leave != null)
                    {
                        var totalDays = leave.IsHalf == true ? ((leave.EndDate - leave.StartDate).TotalDays + 1) * 0.5 : (leave.EndDate - leave.StartDate).TotalDays + 1;
                        if (leave.LeaveCategory.HasValue && leave.LeaveCategory > 0)
                        {
                            switch (leave.LeaveCategory.Value)
                            {
                                case (int)LeaveCategory.CompensatoryOff:
                                    leaveData.CompensatoryOff = leaveData.CompensatoryOff + totalDays;
                                    break;
                                case (int)LeaveCategory.CasualLeave:
                                    leaveData.CasualLeave = leaveData.CasualLeave + totalDays;
                                    break;
                                case (int)LeaveCategory.PaternityLeave:
                                    leaveData.PaternityLeave = leaveData.PaternityLeave + totalDays;
                                    break;
                                case (int)LeaveCategory.MaternityLeave:
                                    leaveData.MaternityLeave = leaveData.MaternityLeave + totalDays;
                                    break;
                                case (int)LeaveCategory.EarnedLeave:
                                    leaveData.EarnedLeave = leaveData.EarnedLeave + totalDays;
                                    break;
                                case (int)LeaveCategory.SickLeave:
                                    leaveData.SickLeave = leaveData.SickLeave + totalDays;
                                    break;
                                case (int)LeaveCategory.BereavementLeave:
                                    leaveData.BereavementLeave = leaveData.BereavementLeave + totalDays;
                                    break;
                                case (int)LeaveCategory.WeddingLeave:
                                    leaveData.WeddingLeave = leaveData.WeddingLeave + totalDays;
                                    break;
                                case (int)LeaveCategory.UnpaidLeave:
                                    leaveData.LossPayLeave = leaveData.LossPayLeave + totalDays;
                                    break;

                            }
                        }
                    }
                }
                else
                {
                    leaveData = GetCurrentLeaveBalance(GetAllLeaveBalance(CurrentUser.Uid), GetCurrentMonthLeaveBalance(GetLeaveBalance(CurrentUser.Uid), GetPendingLeave(CurrentUser.Uid)), 0);
                }
                hdnFields.Add("hdnLeavesCL", (leaveData.CasualLeave ?? 0).ToString());
                hdnFields.Add("hdnLeavesEL", (leaveData.EarnedLeave ?? 0).ToString());
                hdnFields.Add("hdnLeavesAL", (leaveData.LossPayLeave ?? 0).ToString());
                hdnFields.Add("hdnLeavesSL", (leaveData.SickLeave ?? 0).ToString());
                hdnFields.Add("hdnLeavesBL", (leaveData.BereavementLeave ?? 0).ToString());
                hdnFields.Add("hdnLeavesWL", (leaveData.WeddingLeave ?? 0).ToString());
                hdnFields.Add("hdnLeavesLL", (leaveData.LoyaltyLeave ?? 0).ToString());
                hdnFields.Add("hdnLeavesPL", (leaveData.PaternityLeave ?? 0).ToString());
                hdnFields.Add("hdnLeavesML", (leaveData.MaternityLeave ?? 0).ToString());
                hdnFields.Add("hdnLeavesCO", (leaveData.CompensatoryOff ?? 0).ToString());
            }
            else
            {
                hdnFields.Add("hdnLeavesCL", "0");
                hdnFields.Add("hdnLeavesEL", "0");
                hdnFields.Add("hdnLeavesAL", "0");
                hdnFields.Add("hdnLeavesSL", "0");
                hdnFields.Add("hdnLeavesBL", "0");
                hdnFields.Add("hdnLeavesWL", "0");
                hdnFields.Add("hdnLeavesLL", "0");
                hdnFields.Add("hdnLeavesPL", "0");
                hdnFields.Add("hdnLeavesML", "0");
                hdnFields.Add("hdnLeavesCO", "0");
            }

            if (RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId))
            {
                //ViewBag.PMList = userLoginService.GetPMAndPMOUsers().Select(p => new SelectListItem { Text = p.Name, Value = p.Uid.ToString() }).ToList();
                ViewBag.PMList = userLoginService.GetPMAndPMOHRDirectorUsers(true).Select(p => new SelectListItem { Text = p.Name, Value = p.Uid.ToString() }).ToList();
            }
            Preference objPreference = leaveService.GetPreferecesByPMUid((CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO) ? CurrentUser.Uid : CurrentUser.PMUid);
            if (objPreference != null)
            {
                hdnFields.Add("hdnDays", objPreference.PriorLeaveDay != null ? objPreference.PriorLeaveDay.ToString() : "5");
            }
            else
            {
                hdnFields.Add("hdnDays", "5");
            }
            hdnFields.Add("returnview", returnview ?? "");
            if (IsEdit)
            {
                #region "Binding Data Model to View Model"

                hdnFields.Add("hdnTLId", leaveDB.UserLogin1.TLId.ToString());
                hdnFields.Add("hdnAddDT", leaveDB.DateAdded.ToString() ?? DateTime.Now.ToString("dd/MM/yyyy"));

                #endregion
            }
            else
            {
                leaveModelForm.LeaveType = (int)Enums.LeaveType.Normal;
                leaveModelForm.Status = (int)LeaveStatus.Pending;
                hdnFields.Add("hdnTLId", CurrentUser.TLId.ToString());
                hdnFields.Add("hdnAddDT", DateTime.Now.ToString("dd/MM/yyyy"));
            }
            leaveModelForm.FirstHalf = false;
            leaveModelForm.SecondHalf = false;
            leaveModelForm.HalfValue = 1;
            var halfType = new List<SelectListItem>() { new SelectListItem { Text = "First Half", Value = "1" }, new SelectListItem { Text = "Second Half", Value = "2" } };
            halfType.Insert(0, new SelectListItem() { Text = "-Select-", Value = "" });
            leaveModelForm.HalfType = halfType;
            leaveModelForm.HolidayTypeList = WebExtensions.GetSelectList<Enums.HolidayType>();
            ViewBag.HdnFields = hdnFields;
            leaveModelForm.selectWAList = GetWorkAltoernators();
            if (!IsPmHrPmo)
            {
                leaveModelForm.LeaveCategoryList = GetLeaveCategoryByGender(CurrentUser.Gender);
            }
            else
            {
                leaveModelForm.LeaveCategoryList = GetLeaveCategory(true);
            }
            leaveModelForm.selectEmployeeList = IsPmHrPmo ? GetEmployees() : null;

            return leaveModelForm;
        }

        [HttpPost]
        public ActionResult ManageLeave(LeaveActivityDto leaveModelForm, string returnview)
        {
            var fileLogPath = SiteKey.IsLive ? System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Leave_Log", "leave-log.txt") : logPath;
            try
            {
                #region "parameters declarations"
                var modalStartDate = DateTime.ParseExact(leaveModelForm.StartDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var modalEndDate = DateTime.ParseExact(leaveModelForm.EndDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                bool returnToListView = (returnview != null ? (returnview.ToLower().Trim() != "calendar") : true);
                bool IsEdit = (leaveModelForm.LeaveId > 0) ? true : false;
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
                LeaveActivity LeaveModelDB = new LeaveActivity();

                #endregion
                #region [Check Existing WFH]
                //var leaveActivity = wFHService.GetWFHByUId(leaveModelForm.Uid > 0 ? leaveModelForm.Uid : CurrentUser.Uid);
                //if (leaveActivity != null && (!leaveModelForm.IsCancel || leaveModelForm.Status != (int)LeaveStatus.Cancelled))
                //{
                //    var halfValue = 0;
                //    var halfValues = 0;
                //    foreach (var item in leaveActivity.Where(l => l.Status == (int)Enums.LeaveStatus.Approved))
                //    {

                //        if (modalStartDate >= item.StartDate && item.EndDate >= modalEndDate && leaveModelForm.IsHalf == false)
                //        {

                //            ShowErrorMessage("Error !", "You have applied a WFH request for this time period, please Contact Your Manager.", false);

                //            return RedirectToAction(returnToListView ? "index" : "calendar");
                //        }
                //        //&& item.EndDate == Convert.ToDateTime(leaveModelForm.EndDate) && leaveModelForm.IsHalf == false
                //        else if (modalStartDate >= item.StartDate && item.EndDate >= modalEndDate)
                //        {
                //            var FirstHalfvalue = item.FirstHalf;
                //            var SecondHalfvalue = item.SecondHalf;
                //            if (FirstHalfvalue == true)
                //            {
                //                halfValue = 1;
                //            }
                //            if (SecondHalfvalue == true)
                //            {
                //                halfValues = 2;
                //            }
                //            if (halfValue == leaveModelForm.HalfValue || halfValues == leaveModelForm.HalfValue)
                //            {

                //                ShowErrorMessage("Error !", "You have applied a WFH request for this time period, please Contact Your Manager.", false);
                //                return RedirectToAction(returnToListView ? "index" : "calendar");
                //            }
                //        }

                //        else if (modalStartDate >= item.StartDate && item.EndDate >= modalEndDate && item.IsHalf == true)
                //        {
                //            var FirstHalfvalue = item.FirstHalf;
                //            var SecondHalfvalue = item.SecondHalf;
                //            if (FirstHalfvalue == true)
                //            {
                //                halfValue = 1;
                //            }
                //            if (SecondHalfvalue == true)
                //            {
                //                halfValues = 2;
                //            }
                //            if (halfValue == leaveModelForm.HalfValue || halfValues == leaveModelForm.HalfValue)
                //            {

                //                ShowErrorMessage("Error !", "You have applied a WFH request for this time period, please Contact Your Manager.", false);
                //                return RedirectToAction(returnToListView ? "index" : "calendar");
                //            }
                //        }
                //    }
                //}
                #endregion
                #region "Save Leave"

                LeaveModelDB = IsEdit ? leaveService.GetLeaveById(leaveModelForm.LeaveId) : LeaveModelDB;
                if (IsEdit && LeaveModelDB.LeaveCategory.HasValue && LeaveModelDB.LeaveCategory.Value > 0)
                {
                    leaveModelForm.LeaveCategory = LeaveModelDB.LeaveCategory.Value;
                }
                DateTime startDatePrevious = new DateTime();
                DateTime endDatePrevious = new DateTime();
                if (IsEdit)
                {
                    startDatePrevious = LeaveModelDB.StartDate;
                    endDatePrevious = LeaveModelDB.EndDate;
                }
                if (leaveModelForm.LeaveCategory == (int)LeaveCategory.SickLeave)
                {
                    double sickLeave = leaveService.GetAllSickLeavesForYear(leaveModelForm.Uid, (int)LeaveCategory.SickLeave);
                    if (IsEdit)
                    {
                        sickLeave = (endDatePrevious - startDatePrevious).TotalDays + 1 - sickLeave;
                    }
                    if (sickLeave >= 3)
                    {
                        ShowErrorMessage("Error!", "You have already consumed limited sick leaves.", true);
                        leaveModelForm = FillData(leaveModelForm, returnview);
                        ViewBag.ReturntoListView = (returnview != null ? (returnview.ToLower().Trim() != "calendar") : true);
                        return View(leaveModelForm);
                    }

                }
                bool isEditDate = (leaveModelForm.StartDate != LeaveModelDB.StartDate.ToString("dd/MM/yyyy") || leaveModelForm.EndDate != LeaveModelDB.EndDate.ToString("dd/MM/yyyy"));

                #region "comment>logic for find actual leave status"
                /*   check for leave status
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

                if (leaveModelForm.IsSelfLeave)
                {
                    leaveModelForm.Uid = CurrentUser.Uid;
                }


                LeaveModelDB.Uid = IsEdit ? (IsPmHrPmo ? leaveModelForm.Uid : LeaveModelDB.Uid) : (IsPmHrPmo ? (leaveModelForm.IsSelfLeave ? CurrentUser.Uid : leaveModelForm.Uid) : CurrentUser.Uid);

                LeaveModelDB.Status = IsEdit ? (
                                                leaveModelForm.IsCancel ?
                                                (int)Enums.LeaveStatus.Cancelled : ((IsPmHrPmo && !leaveModelForm.IsSelfLeave) ? leaveModelForm.Status : LeaveModelDB.Status)
                                                ) :
                                               (
                                               (!IsPmHrPmo || leaveModelForm.IsSelfLeave) ? (int)Enums.LeaveStatus.Pending : leaveModelForm.Status
                                               );
                if (leaveModelForm.Uid == 0)
                {
                    leaveModelForm.Uid = CurrentUser.Uid;
                }
                if (leaveModelForm.WorkAlterID == leaveModelForm.Uid)
                {
                    ShowErrorMessage("Error!", "The handover person can not be the same, as he/she will take care of the work in absence.", true);
                    leaveModelForm = FillData(leaveModelForm, returnview);
                    ViewBag.ReturntoListView = (returnview != null ? (returnview.ToLower().Trim() != "calendar") : true);
                    return View(leaveModelForm);
                }
                var userDetail = userLoginService.GetUserInfoByID(leaveModelForm.Uid);
                if (leaveModelForm.LeaveCategory == (int)LeaveCategory.MaternityLeave || leaveModelForm.LeaveCategory == (int)LeaveCategory.PaternityLeave)
                {
                    if (userDetail != null)
                    {
                        if ((userDetail.Gender.Equals("M") && leaveModelForm.LeaveCategory == (int)LeaveCategory.MaternityLeave) || (userDetail.Gender.Equals("F") && leaveModelForm.LeaveCategory == (int)LeaveCategory.PaternityLeave))
                        {
                            ShowErrorMessage("Error!", "Select leave type is not allowed as per the employee's gender.", true);
                            leaveModelForm = FillData(leaveModelForm, returnview);
                            ViewBag.ReturntoListView = (returnview != null ? (returnview.ToLower().Trim() != "calendar") : true);
                            return View(leaveModelForm);
                        }
                    }
                }
                //if(leaveModelForm.Uid==CurrentUser.Uid)
                //{
                //    leaveModelForm.IsSelfLeave = true;
                //}
                if ((RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId) || CurrentUser.RoleId == (int)Enums.UserRoles.PM
                    || RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
                    || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
                    || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
                    || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
                    || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)
                    || RoleValidator.TL_Sales_DesignationIds.Contains(CurrentUser.DesignationId)
                    || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
                    || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(CurrentUser.DesignationId)
                    || RoleValidator.TL_HR_DesignationIds.Contains(CurrentUser.DesignationId))
                    && leaveModelForm.IsSelfLeave && (!leaveModelForm.IsCancel || leaveModelForm.Status != (int)LeaveStatus.Cancelled))
                {
                    var SelectedStartdate = DateTime.ParseExact(leaveModelForm.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var SelectedEnddate = DateTime.ParseExact(leaveModelForm.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var totalDays = leaveModelForm.IsHalf ? ((SelectedEnddate - SelectedStartdate).TotalDays + 1) * 0.5 : ((SelectedEnddate - SelectedStartdate).TotalDays + 1) * 1;
                    if (totalDays <= 5)
                    {
                        if (IsEdit)
                        {
                            var previousDay = (endDatePrevious - startDatePrevious).TotalDays + 1;
                            var pendingLeaveInfo = leaveService.GetPendingLeaves(LeaveModelDB.Uid);
                            if (((double)totalDays + pendingLeaveInfo - previousDay) > 5)
                            {
                                if (((double)totalDays + pendingLeaveInfo - previousDay) > 5)
                                {
                                    ShowErrorMessage("Error!", $"Your have consumed 5 days leaves limit(In pending), either wait for the approval or for more leaves please contact to HR department.", true);

                                    leaveModelForm = FillData(leaveModelForm, returnview);
                                    ViewBag.ReturntoListView = (returnview != null ? (returnview.ToLower().Trim() != "calendar") : true);
                                    return View(leaveModelForm);
                                }
                            }
                        }
                        else
                        {
                            var pendingLeaveInfo = leaveService.GetPendingLeaves(LeaveModelDB.Uid);
                            if (((double)totalDays + pendingLeaveInfo) > 5 /*|| pendingLeaveInfo > 0*/)
                            {
                                if (pendingLeaveInfo > 5)
                                {
                                    ShowErrorMessage("Error!", $"Your have consumed 5 days leaves limit(In pending), either wait for the approval or for more leaves please contact to HR department.", true);
                                }
                                else
                                {
                                    ShowErrorMessage("Error!", $"Your previous leave is still pending you can't apply for leave for the selected leave category.", true);
                                }
                                leaveModelForm = FillData(leaveModelForm, returnview);
                                ViewBag.ReturntoListView = (returnview != null ? (returnview.ToLower().Trim() != "calendar") : true);
                                return View(leaveModelForm);
                            }
                        }
                    }
                    else
                    {
                        ShowErrorMessage("Error!", "You can apply a maximum of 5 days of these leave category.", true);
                        leaveModelForm = FillData(leaveModelForm, returnview);
                        ViewBag.ReturntoListView = (returnview != null ? (returnview.ToLower().Trim() != "calendar") : true);
                        return View(leaveModelForm);
                    }
                }
                else if ((!RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId) && CurrentUser.RoleId != (int)Enums.UserRoles.PM
                    //  &&
                    //(  (RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
                    //|| RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
                    //|| RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
                    //|| RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
                    //|| RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)
                    //|| RoleValidator.TL_Sales_DesignationIds.Contains(CurrentUser.DesignationId)
                    //|| RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
                    //|| RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(CurrentUser.DesignationId)
                    //||
                    //RoleValidator.TL_HR_DesignationIds.Contains(CurrentUser.DesignationId))
                    && CurrentUser.RoleId != (int)UserRoles.UKBDM && CurrentUser.RoleId != (int)UserRoles.UKPM && CurrentUser.RoleId != (int)UserRoles.AUPM && CurrentUser.RoleId != (int)UserRoles.PMOAU && CurrentUser.RoleId != (int)UserRoles.PMO) && (!leaveModelForm.IsCancel || leaveModelForm.Status != (int)LeaveStatus.Cancelled))
                {
                    var SelectedStartdate = DateTime.ParseExact(leaveModelForm.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var SelectedEnddate = DateTime.ParseExact(leaveModelForm.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var totalDays = leaveModelForm.IsHalf ? ((SelectedEnddate - SelectedStartdate).TotalDays + 1) * 0.5 : ((SelectedEnddate - SelectedStartdate).TotalDays + 1) * 1;
                    if (totalDays <= 5)
                    {
                        if (IsEdit)
                        {
                            var previousDay = (endDatePrevious - startDatePrevious).TotalDays + 1;
                            var pendingLeaveInfo = leaveService.GetPendingLeaves(LeaveModelDB.Uid);
                            if (((double)totalDays + pendingLeaveInfo - previousDay) > 5)
                            {
                                if (((double)totalDays + pendingLeaveInfo - previousDay) > 5)
                                {
                                    ShowErrorMessage("Error!", $"Your have consumed 5 days leaves limit(In pending), either wait for the approval or for more leaves please contact to HR department.", true);

                                    leaveModelForm = FillData(leaveModelForm, returnview);
                                    ViewBag.ReturntoListView = (returnview != null ? (returnview.ToLower().Trim() != "calendar") : true);
                                    return View(leaveModelForm);
                                }
                            }
                        }
                        else
                        {
                            var pendingLeaveInfo = leaveService.GetPendingLeaves(LeaveModelDB.Uid);
                            if (((double)totalDays + pendingLeaveInfo) > 5 /*|| pendingLeaveInfo > 0*/)
                            {
                                if (pendingLeaveInfo > 5)
                                {
                                    ShowErrorMessage("Error!", $"Your have consumed 5 days leaves limit(In pending), either wait for the approval or for more leaves please contact to HR department.", true);
                                }
                                else
                                {
                                    ShowErrorMessage("Error!", $"Your previous leave is still pending you can't apply for leave for the selected leave category.", true);
                                }
                                leaveModelForm = FillData(leaveModelForm, returnview);
                                ViewBag.ReturntoListView = (returnview != null ? (returnview.ToLower().Trim() != "calendar") : true);
                                return View(leaveModelForm);
                            }
                        }
                    }
                    else
                    {
                        ShowErrorMessage("Error!", "You can apply a maximum of 5 days of these leave category.", true);
                        leaveModelForm = FillData(leaveModelForm, returnview);
                        ViewBag.ReturntoListView = (returnview != null ? (returnview.ToLower().Trim() != "calendar") : true);
                        return View(leaveModelForm);
                    }
                }
                bool canapplyleave = leaveService.CanApplyLeave(leaveModelForm.Uid, leaveModelForm.LeaveId, DateTime.ParseExact(leaveModelForm.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.ParseExact(leaveModelForm.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                if (!canapplyleave)
                {
                    ShowErrorMessage("Error!", "You have already applied a leave for this time period, please select a different date range.", true);
                    leaveModelForm = FillData(leaveModelForm, returnview);
                    ViewBag.ReturntoListView = (returnview != null ? (returnview.ToLower().Trim() != "calendar") : true);
                    return View(leaveModelForm);
                }

                if ((RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
                  || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
                  || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
                  || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
                  || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)
                  || RoleValidator.TL_Sales_DesignationIds.Contains(CurrentUser.DesignationId)
                  || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
                  || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(CurrentUser.DesignationId)
                  || RoleValidator.TL_HR_DesignationIds.Contains(CurrentUser.DesignationId)) && LeaveModelDB.Status == 0)
                {
                    LeaveModelDB.Status = (int)Enums.LeaveStatus.Pending;
                }

                int leavepriordays = 5;
                if (RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId))
                {
                    Preference objPreference = leaveService.GetPreferecesByPMUid(leaveModelForm.PMid);

                    if (objPreference != null && objPreference.PriorLeaveDay.HasValue)
                    {
                        leavepriordays = objPreference.PriorLeaveDay.Value;
                    }
                }
                else
                {
                    Preference objPreference = leaveService.GetPreferecesByPMUid((CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO) ? CurrentUser.Uid : CurrentUser.PMUid);
                    if (objPreference != null && objPreference.PriorLeaveDay.HasValue)
                    {
                        leavepriordays = objPreference.PriorLeaveDay.Value;
                    }
                }
                if (LeaveModelDB.Uid == 0)
                {
                    LeaveModelDB.Uid = CurrentUser.Uid;
                }
                LeaveModelDB.StartDate = DateTime.ParseExact(leaveModelForm.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                LeaveModelDB.EndDate = DateTime.ParseExact(leaveModelForm.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                int diffdays = (int)((LeaveModelDB.StartDate.Date - DateTime.Now.Date).TotalDays);
                int leaveDays = (int)((LeaveModelDB.EndDate.Date - LeaveModelDB.StartDate.Date).TotalDays);
                LeaveModelDB.LeaveType = !IsEdit ? (diffdays < leavepriordays ? (int)Enums.LeaveType.Urgent : (int)Enums.LeaveType.Normal) : LeaveModelDB.LeaveType;
                leaveModelForm.LeaveType = LeaveModelDB.LeaveType.Value;
                LeaveModelDB.WorkAlterId = Convert.ToInt32(leaveModelForm.WorkAlterID);
                LeaveModelDB.WorkAlternator = leaveModelForm.WorkAlternator;
                LeaveModelDB.Reason = leaveModelForm.Reason;
                LeaveModelDB.IsHalf = leaveModelForm.IsHalf;
                LeaveModelDB.DateAdded = LeaveModelDB.DateAdded ?? System.DateTime.Now;
                LeaveModelDB.Remark = leaveModelForm.Remark;
                LeaveModelDB.ModifyBy = CurrentUser.Uid;
                LeaveModelDB.DateModify = DateTime.Now;
                LeaveModelDB.Ip = GeneralMethods.Getip();
                string leaveReason = leaveModelForm.Reason.Length > 100 ? leaveModelForm.Reason.Substring(0, 100) : leaveModelForm.Reason;
                if (CurrentUser.RoleId != (int)UserRoles.UKBDM && CurrentUser.RoleId != (int)UserRoles.UKPM && CurrentUser.RoleId != (int)UserRoles.AUPM && CurrentUser.RoleId != (int)UserRoles.PMOAU && CurrentUser.RoleId != (int)UserRoles.PMO)
                {
                    if (leaveModelForm.LeaveCategory != 0 && !IsEdit)
                    {
                        bool hasLeaveBalance = true;
                        var userLeavebalance = GetAllLeaveBalance(LeaveModelDB.Uid);
                        switch (leaveModelForm.LeaveCategory)
                        {
                            case (int)LeaveCategory.CompensatoryOff:
                                if (userLeavebalance != null && userLeavebalance.CompensatoryOff_CB == 0)
                                {
                                    hasLeaveBalance = false;
                                    goto NotSufficientLeaveError;
                                }
                                break;
                            case (int)LeaveCategory.CasualLeave:
                                if (userLeavebalance != null && userLeavebalance.CasualLeave_CB == 0)
                                {
                                    hasLeaveBalance = false;
                                    goto NotSufficientLeaveError;
                                }
                                break;
                            case (int)LeaveCategory.EarnedLeave:
                                if (userLeavebalance != null && userLeavebalance.EarnedLeave_CB == 0)
                                {
                                    hasLeaveBalance = false;
                                    goto NotSufficientLeaveError;
                                }
                                break;
                            case (int)LeaveCategory.PaternityLeave:
                                if (userLeavebalance != null && userLeavebalance.PaternityLeave_CB == 0)
                                {
                                    hasLeaveBalance = false;
                                    goto NotSufficientLeaveError;
                                }
                                break;
                            case (int)LeaveCategory.MaternityLeave:
                                if (userLeavebalance != null && userLeavebalance.MaternityLeave_CB == 0)
                                {
                                    hasLeaveBalance = false;
                                    goto NotSufficientLeaveError;
                                }
                                break;
                            //case (int)LeaveCategory.SickLeave:
                            //    if (userLeavebalance != null && userLeavebalance.SickLeave_CB == 0)
                            //         {
                            //hasLeaveBalance = false;
                            //goto NotSufficientLeaveError;
                            //}
                            //    break;
                            case (int)LeaveCategory.WeddingLeave:
                                if (userLeavebalance != null && userLeavebalance.WeddingLeave_CB == 0)
                                {
                                    hasLeaveBalance = false;
                                    goto NotSufficientLeaveError;
                                }
                                break;
                            case (int)LeaveCategory.BereavementLeave:
                                if (userLeavebalance != null && userLeavebalance.BereavementLeave_CB == 0)
                                {
                                    hasLeaveBalance = false;
                                    goto NotSufficientLeaveError;
                                }
                                break;
                        }
                    NotSufficientLeaveError:
                        {
                            if (!hasLeaveBalance)
                            {
                                ShowErrorMessage("Error!", "Not sufficient selected leave balance. please try with a different leave type.", true);
                                leaveModelForm = FillData(leaveModelForm, returnview);
                                ViewBag.ReturntoListView = (returnview != null ? (returnview.ToLower().Trim() != "calendar") : true);
                                return View(leaveModelForm);
                            }
                        }
                    }
                    LeaveModelDB.LeaveCategory = leaveModelForm.LeaveCategory > 0 ? leaveModelForm.LeaveCategory : (int)LeaveCategory.CasualLeave;
                }
                else
                {
                    LeaveModelDB.LeaveCategory = null;
                }
                if (leaveModelForm.HalfValue.HasValue && leaveModelForm.IsHalf)
                {
                    if (leaveModelForm.HalfValue.Value == 1)
                    {
                        LeaveModelDB.FirstHalf = true;
                        LeaveModelDB.SecondHalf = false;
                    }
                    else if (leaveModelForm.HalfValue.Value == 2)
                    {
                        LeaveModelDB.FirstHalf = false;
                        LeaveModelDB.SecondHalf = true;
                    }
                }
                else
                {
                    LeaveModelDB.FirstHalf = true;
                    LeaveModelDB.SecondHalf = true;
                }
                LeaveModelDB.HolidayType = Convert.ToInt32(leaveModelForm.HolidayType);

                LogPrint.WriteIntoFile(fileLogPath, $"---------------------------LeaveId:{LeaveModelDB.LeaveId}---------------------------");
                LogPrint.WriteIntoFile(fileLogPath, $"EMS Data created on:{DateTime.Now}");
                if (LeaveModelDB.LeaveCategory != null)
                {
                    LogPrint.WriteIntoFile(fileLogPath, $"EmpUid:{LeaveModelDB.Uid},AttendanceId:{"NA"}, EmpLeaveId:{LeaveModelDB.LeaveId}, LeaveStartDate:{LeaveModelDB.StartDate.ToString("dd-MMM-yyyy")}, LeaveEndDate:{LeaveModelDB.EndDate.ToString("dd-MMM-yyyy")}, HalfDay:{(LeaveModelDB.IsHalf.HasValue && LeaveModelDB.IsHalf.Value ? true : false)}, LeaveType:{LeaveModelDB.LeaveCategory}, FirstHalf:{LeaveModelDB.FirstHalf}, SecondHalf:{LeaveModelDB.SecondHalf}, LeaveCategory:{((Enums.LeaveCategory)LeaveModelDB.LeaveCategory).GetEnumDescription()}, ModifyDate:{(LeaveModelDB.DateModify.HasValue ? (LeaveModelDB.DateModify.Value).ToString("dd-MMM-yyyy") : string.Empty)}, Created/ModifyBy:{CurrentUser.Uid}({CurrentUser.Name})");
                }
                else
                {
                    LogPrint.WriteIntoFile(fileLogPath, $"EmpUid:{LeaveModelDB.Uid},AttendanceId:{"NA"}, EmpLeaveId:{LeaveModelDB.LeaveId}, LeaveStartDate:{LeaveModelDB.StartDate.ToString("dd-MMM-yyyy")}, LeaveEndDate:{LeaveModelDB.EndDate.ToString("dd-MMM-yyyy")}, HalfDay:{(LeaveModelDB.IsHalf.HasValue && LeaveModelDB.IsHalf.Value ? true : false)}, FirstHalf:{LeaveModelDB.FirstHalf}, SecondHalf:{LeaveModelDB.SecondHalf}, ModifyDate:{(LeaveModelDB.DateModify.HasValue ? (LeaveModelDB.DateModify.Value).ToString("dd-MMM-yyyy") : string.Empty)}, Created/ModifyBy:{CurrentUser.Uid}({CurrentUser.Name})");
                }
                LogPrint.WriteIntoFile(fileLogPath, "");
                leaveService.Save(LeaveModelDB);



                var emsLeaveId = LeaveModelDB.LeaveId;
                var empInfo = userLoginService.GetUserInfoByID(LeaveModelDB.Uid);
                int? empAttendanceId = empInfo != null && empInfo.AttendenceId.HasValue ? empInfo.AttendenceId.Value : 0;
                if (empAttendanceId.Value == 0)
                {
                    if (empInfo != null)
                    {

                        var saralUserInfo = levDetailsService.GetEmployeeInfo(empInfo.EmailOffice);
                        if (saralUserInfo != null)
                        {
                            empAttendanceId = saralUserInfo != null ? saralUserInfo.Empid > 0 ? saralUserInfo.Empid : 0 : 0;
                        }
                        else if (saralUserInfo == null)
                        {
                            var saralDTUserInfo = levDetailsDTService.GetDTEmployeeInfo(empInfo.EmailOffice);
                            empAttendanceId = saralDTUserInfo != null ? saralDTUserInfo.Empid > 0 ? saralDTUserInfo.Empid : 0 : 0;
                        }
                    }
                }
                LogPrint.WriteIntoFile(fileLogPath, $"---------------------------LeaveId:{LeaveModelDB.LeaveId}---------------------------");
                LogPrint.WriteIntoFile(fileLogPath, $"EMS Data created on:{DateTime.Now}");
                if (LeaveModelDB.LeaveCategory != null)
                {
                    LogPrint.WriteIntoFile(fileLogPath, $"EmpUid:{LeaveModelDB.Uid},AttendanceId:{empAttendanceId.Value}, EmpLeaveId:{LeaveModelDB.LeaveId}, LeaveStartDate:{LeaveModelDB.StartDate.ToString("dd-MMM-yyyy")}, LeaveEndDate:{LeaveModelDB.EndDate.ToString("dd-MMM-yyyy")}, HalfDay:{(LeaveModelDB.IsHalf.HasValue && LeaveModelDB.IsHalf.Value ? true : false)}, FirstHalf:{LeaveModelDB.FirstHalf}, SecondHalf:{LeaveModelDB.SecondHalf},LeaveType:{LeaveModelDB.LeaveCategory}, LeaveCategory:{((Enums.LeaveCategory)LeaveModelDB.LeaveCategory).GetEnumDescription()}, ModifyDate:{(LeaveModelDB.DateModify.HasValue ? (LeaveModelDB.DateModify.Value).ToString("dd-MMM-yyyy") : string.Empty)}, Created/ModifyBy:{CurrentUser.Uid}({CurrentUser.Name})");
                }
                else
                {
                    LogPrint.WriteIntoFile(fileLogPath, $"EmpUid:{LeaveModelDB.Uid},AttendanceId:{empAttendanceId.Value}, EmpLeaveId:{LeaveModelDB.LeaveId}, LeaveStartDate:{LeaveModelDB.StartDate.ToString("dd-MMM-yyyy")}, LeaveEndDate:{LeaveModelDB.EndDate.ToString("dd-MMM-yyyy")}, HalfDay:{(LeaveModelDB.IsHalf.HasValue && LeaveModelDB.IsHalf.Value ? true : false)}, FirstHalf:{LeaveModelDB.FirstHalf}, SecondHalf:{LeaveModelDB.SecondHalf}, ModifyDate:{(LeaveModelDB.DateModify.HasValue ? (LeaveModelDB.DateModify.Value).ToString("dd-MMM-yyyy") : string.Empty)}, Created/ModifyBy:{CurrentUser.Uid}({CurrentUser.Name})");
                }
                LogPrint.WriteIntoFile(fileLogPath, "");

                #region [Saral and HRM DB Entry]

                //if (CurrentUser.RoleId != (int)UserRoles.UKBDM && CurrentUser.RoleId != (int)UserRoles.UKPM && CurrentUser.RoleId != (int)UserRoles.AUPM && CurrentUser.RoleId != (int)UserRoles.PMOAU && CurrentUser.RoleId != (int)UserRoles.PMO && empAttendanceId.Value > 0 && SiteKey.IsLive) 
                if (empInfo != null && empInfo.Role != null && empInfo.Role.RoleId != (int)UserRoles.UKBDM && empInfo.Role.RoleId != (int)UserRoles.UKPM && empInfo.Role.RoleId != (int)UserRoles.AUPM && empInfo.Role.RoleId != (int)UserRoles.PMOAU && empInfo.Role.RoleId != (int)UserRoles.PMO && empAttendanceId.HasValue && empAttendanceId.Value > 0 && SiteKey.IsLive)
                {
                    int empMaxDetailId = 0;
                    try
                    {

                        empMaxDetailId = levAllotmentService.GetMaxAllotmentValue(empAttendanceId.Value);
                        if (empMaxDetailId == 0)
                        {
                            empMaxDetailId = levAllotmentDTService.GetDTMaxAllotmentValue(empAttendanceId.Value);
                        }
                    }
                    catch (Exception)
                    {
                        empMaxDetailId = 0;
                    }
                    var monthYearStart = ((LeaveModelDB.StartDate.Year) * 12) + (LeaveModelDB.StartDate.Month);
                    var monthYearEnd = ((LeaveModelDB.EndDate.Year) * 12) + (LeaveModelDB.EndDate.Month);
                    if (LeaveModelDB.Status == (int)LeaveStatus.Pending || LeaveModelDB.Status == (int)LeaveStatus.Approved)
                    {
                        string _statusType = LeaveModelDB.Status == (int)LeaveStatus.Pending ? "Pending" : "Approved";
                        var date = LeaveModelDB.StartDate.Date;
                        if (IsEdit)
                        {
                            var levDetailsDT = levDetailsDTService.GetDTLeaveDetailsByLeaveDate(empAttendanceId, startDatePrevious, endDatePrevious);
                            if (levDetailsDT.Count > 0)
                            {
                                foreach (var item in levDetailsDT)
                                {
                                    levDetailsDTService.Delete(item);
                                }
                            }
                            else
                            {
                                var levDetails = levDetailsService.GetLeaveDetailsByLeaveDate(empAttendanceId, startDatePrevious, endDatePrevious);
                                if (levDetails.Count > 0)
                                {
                                    foreach (var item in levDetails)
                                    {
                                        levDetailsService.Delete(item);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i <= leaveDays; i++)
                        {
                            if (empInfo.IsFromDbdt == false || empInfo.IsFromDbdt == null)
                            {
                                Data.saral.LevDetails details = new Data.saral.LevDetails()
                                {
                                    Empid = (int)empAttendanceId,
                                    Empdetid = empMaxDetailId,
                                    Levid = (int)LeaveModelDB.LeaveCategory,
                                    Leavedate = date,
                                    Reason = leaveReason,
                                    Firsthalfyn = Convert.ToByte(LeaveModelDB.FirstHalf),
                                    Secondhalfyn = Convert.ToByte(LeaveModelDB.SecondHalf)
                                };
                                levDetailsService.Save(details);

                                LogPrint.WriteIntoFile(fileLogPath, $"SARAL Data created on:{DateTime.Now}");
                                LogPrint.WriteIntoFile(fileLogPath, $"EmpUid:{LeaveModelDB.Uid}, AttendanceId:{empAttendanceId.Value}, EmpLeaveId:{LeaveModelDB.LeaveId}, LeaveStartDate:{date.ToString("dd-MMM-yyyy")}, LeaveEndDate:{date.ToString("dd-MMM-yyyy")}, HalfDay:{(LeaveModelDB.IsHalf.HasValue && LeaveModelDB.IsHalf.Value ? true : false)}, FirstHalf:{LeaveModelDB.FirstHalf}, SecondHalf:{LeaveModelDB.SecondHalf}, LeaveType:{LeaveModelDB.LeaveCategory}, LeaveCategory:{((Enums.LeaveCategory)LeaveModelDB.LeaveCategory).GetEnumDescription()}, ModifyDate:{(LeaveModelDB.DateModify.HasValue ? (LeaveModelDB.DateModify.Value).ToString("dd-MMM-yyyy") : string.Empty)}, Created/ModifyBy:{CurrentUser.Uid}({CurrentUser.Name}),Status: " + _statusType);
                                LogPrint.WriteIntoFile(fileLogPath, "");

                            }
                            else
                            {
                                Data.saralDT.LevDetails details = new Data.saralDT.LevDetails()
                                {
                                    Empid = (int)empAttendanceId,
                                    Empdetid = empMaxDetailId,
                                    Levid = (int)LeaveModelDB.LeaveCategory,
                                    Leavedate = date,
                                    Reason = leaveReason,
                                    Firsthalfyn = Convert.ToByte(LeaveModelDB.FirstHalf),
                                    Secondhalfyn = Convert.ToByte(LeaveModelDB.SecondHalf)
                                };
                                levDetailsDTService.Save(details);

                                LogPrint.WriteIntoFile(fileLogPath, $"SARAL Data created on:{DateTime.Now}");
                                LogPrint.WriteIntoFile(fileLogPath, $"EmpUid:{LeaveModelDB.Uid}, AttendanceId:{empAttendanceId.Value}, EmpLeaveId:{LeaveModelDB.LeaveId}, LeaveStartDate:{date.ToString("dd-MMM-yyyy")}, LeaveEndDate:{date.ToString("dd-MMM-yyyy")}, HalfDay:{(LeaveModelDB.IsHalf.HasValue && LeaveModelDB.IsHalf.Value ? true : false)}, FirstHalf:{LeaveModelDB.FirstHalf}, SecondHalf:{LeaveModelDB.SecondHalf}, LeaveType:{LeaveModelDB.LeaveCategory}, LeaveCategory:{((Enums.LeaveCategory)LeaveModelDB.LeaveCategory).GetEnumDescription()}, ModifyDate:{(LeaveModelDB.DateModify.HasValue ? (LeaveModelDB.DateModify.Value).ToString("dd-MMM-yyyy") : string.Empty)}, Created/ModifyBy:{CurrentUser.Uid}({CurrentUser.Name}), Status: " + _statusType);
                                LogPrint.WriteIntoFile(fileLogPath, "");

                            }
                            date = date.AddDays(1);
                        }
                        try
                        {
                            ExecuteHrmAPI(Convert.ToString(emsLeaveId), empInfo.EmailOffice, LeaveModelDB.StartDate, LeaveModelDB.EndDate, (int)LeaveModelDB.LeaveCategory, leaveModelForm.IsHalf, "InsertUpdate");

                            LogPrint.WriteIntoFile(fileLogPath, $"HRM Data (Insert/Update) created on:{DateTime.Now}");
                            LogPrint.WriteIntoFile(fileLogPath, $"EmpEmail:{empInfo.EmailOffice}, AttendanceId:{empAttendanceId.Value}, EmpLeaveId:{emsLeaveId}, LeaveStartDate:{LeaveModelDB.StartDate.ToString("dd-MMM-yyyy")}, LeaveEndDate:{LeaveModelDB.EndDate.ToString("dd-MMM-yyyy")}, HalfDay:{(leaveModelForm.IsHalf ? true : false)}, LeaveType:{LeaveModelDB.LeaveCategory}, LeaveCategory:{((Enums.LeaveCategory)LeaveModelDB.LeaveCategory).GetEnumDescription()},LeaveCategoryShortName:{((LeaveCategory)LeaveModelDB.LeaveCategory).GetEnumDisplayShortName()}, Created/ModifyBy:{CurrentUser.Uid}({CurrentUser.Name}),Status:" + _statusType);
                            LogPrint.WriteIntoFile(fileLogPath, "");
                        }
                        catch (Exception ex)
                        {
                            LogPrint.WriteIntoFile(fileLogPath, $"HRM Data (Insert/Update) created on:{DateTime.Now}");
                            LogPrint.WriteIntoFile(fileLogPath, $"EmpEmail:{empInfo.EmailOffice}, AttendanceId:{empAttendanceId.Value}, EmpLeaveId:{emsLeaveId}, LeaveStartDate:{LeaveModelDB.StartDate.ToString("dd-MMM-yyyy")}, LeaveEndDate:{LeaveModelDB.EndDate.ToString("dd-MMM-yyyy")}, HalfDay:{(leaveModelForm.IsHalf ? true : false)}, LeaveType:{LeaveModelDB.LeaveCategory}, LeaveCategory:{((Enums.LeaveCategory)LeaveModelDB.LeaveCategory).GetEnumDescription()},LeaveCategoryShortName:{((LeaveCategory)LeaveModelDB.LeaveCategory).GetEnumDisplayShortName()}, Created/ModifyBy:{CurrentUser.Uid}({CurrentUser.Name}),Status:" + _statusType);
                            LogPrint.WriteIntoFile(fileLogPath, "");
                        }
                        if (monthYearStart == monthYearEnd)
                        {
                            var leaveMonth = levMonthdetService.GetLeaveMonthDetail(empAttendanceId, monthYearStart);
                            if (leaveMonth != null)
                            {
                                leaveMonth.Editmodeyn = 1;
                                levMonthdetService.Save(leaveMonth);
                            }
                            else
                            {
                                var leaveMonthDT = levMonthdetDTService.GetLeaveMonthDetail(empAttendanceId, monthYearStart);
                                if (leaveMonthDT != null)
                                {
                                    leaveMonthDT.Editmodeyn = 1;
                                    levMonthdetDTService.Save(leaveMonthDT);
                                }
                            }
                        }
                        else
                        {
                            for (int i = monthYearStart; i <= monthYearEnd; i++)
                            {
                                var leaveMonth = levMonthdetService.GetLeaveMonthDetail(empAttendanceId, monthYearStart);
                                if (leaveMonth != null)
                                {
                                    leaveMonth.Editmodeyn = 1;
                                    levMonthdetService.Save(leaveMonth);
                                }
                                else
                                {
                                    var leaveMonthDT = levMonthdetDTService.GetLeaveMonthDetail(empAttendanceId, monthYearStart);
                                    if (leaveMonthDT != null)
                                    {
                                        leaveMonthDT.Editmodeyn = 1;
                                        levMonthdetDTService.Save(leaveMonthDT);
                                    }
                                }
                            }
                        }
                    }
                    else if (LeaveModelDB.Status == (int)LeaveStatus.UnApproved || LeaveModelDB.Status == (int)LeaveStatus.Cancelled)
                    {
                        string _statusType = LeaveModelDB.Status == (int)LeaveStatus.UnApproved ? "UnApproved" : "Cancelled";
                        var levDetails = levDetailsService.GetLeaveDetailsByLeaveDate(empAttendanceId, LeaveModelDB.StartDate, LeaveModelDB.EndDate);
                        if (levDetails.Count > 0)
                        {
                            foreach (var item in levDetails)
                            {
                                levDetailsService.Delete(item);
                                int month = (item.Leavedate.Year * 12) + item.Leavedate.Month;
                                var leaveMonth = levMonthdetService.GetLeaveMonthDetail(empAttendanceId, month);
                                if (leaveMonth != null)
                                {
                                    leaveMonth.Editmodeyn = 1;
                                    levMonthdetService.Save(leaveMonth);
                                }
                            }
                        }
                        else
                        {
                            var levDetailsDT = levDetailsDTService.GetDTLeaveDetailsByLeaveDate(empAttendanceId, LeaveModelDB.StartDate, LeaveModelDB.EndDate);
                            if (levDetailsDT.Count > 0)
                            {
                                foreach (var item in levDetailsDT)
                                {
                                    levDetailsDTService.Delete(item);
                                    int month = (item.Leavedate.Year * 12) + item.Leavedate.Month;
                                    var leaveMonth = levMonthdetDTService.GetLeaveMonthDetail(empAttendanceId, month);
                                    if (leaveMonth != null)
                                    {
                                        leaveMonth.Editmodeyn = 1;
                                        levMonthdetDTService.Save(leaveMonth);
                                    }
                                }
                            }
                        }

                        try
                        {
                            ExecuteHrmAPI(Convert.ToString(emsLeaveId), empInfo.EmailOffice, LeaveModelDB.StartDate, LeaveModelDB.EndDate, (int)LeaveModelDB.LeaveCategory, leaveModelForm.IsHalf, "Delete");

                            LogPrint.WriteIntoFile(fileLogPath, $"HRM Data (Delete) created on:{DateTime.Now}");
                            LogPrint.WriteIntoFile(fileLogPath, $"EmpEmail:{empInfo.EmailOffice}, AttendanceId:{empAttendanceId.Value}, EmpLeaveId:{emsLeaveId}, LeaveStartDate:{LeaveModelDB.StartDate.ToString("dd-MMM-yyyy")}, LeaveEndDate:{LeaveModelDB.EndDate.ToString("dd-MMM-yyyy")}, HalfDay:{(leaveModelForm.IsHalf ? true : false)}, LeaveType:{LeaveModelDB.LeaveCategory}, LeaveCategory:{((Enums.LeaveCategory)LeaveModelDB.LeaveCategory).GetEnumDescription()},LeaveCategoryShortName:{((LeaveCategory)LeaveModelDB.LeaveCategory).GetEnumDisplayShortName()}, Created/ModifyBy:{CurrentUser.Uid}({CurrentUser.Name}),Status:" + _statusType);
                            LogPrint.WriteIntoFile(fileLogPath, "");
                            if (LeaveModelDB.Status == (int)LeaveStatus.UnApproved)
                            {
                                var date = LeaveModelDB.StartDate.Date;
                                for (int i = 0; i <= leaveDays; i++)
                                {
                                    if (empInfo.IsFromDbdt == false || empInfo.IsFromDbdt == null)
                                    {
                                        Data.saral.LevDetails details = new Data.saral.LevDetails()
                                        {
                                            Empid = (int)empAttendanceId,
                                            Empdetid = empMaxDetailId,
                                            Levid = (int)LeaveCategory.UnpaidLeave,
                                            Leavedate = date,
                                            Reason = leaveReason,
                                            Firsthalfyn = Convert.ToByte(LeaveModelDB.FirstHalf),
                                            Secondhalfyn = Convert.ToByte(LeaveModelDB.SecondHalf)
                                        };
                                        levDetailsService.Save(details);

                                        LogPrint.WriteIntoFile(fileLogPath, $"SARAL UnApproved Data created on:{DateTime.Now}");
                                        LogPrint.WriteIntoFile(fileLogPath, $"EmpUid:{LeaveModelDB.Uid}, AttendanceId:{empAttendanceId.Value}, EmpLeaveId:{LeaveModelDB.LeaveId}, LeaveStartDate:{date.ToString("dd-MMM-yyyy")}, LeaveEndDate:{date.ToString("dd-MMM-yyyy")}, HalfDay:{(LeaveModelDB.IsHalf.HasValue && LeaveModelDB.IsHalf.Value ? true : false)}, FirstHalf:{LeaveModelDB.FirstHalf}, SecondHalf:{LeaveModelDB.SecondHalf}, LeaveType:{LeaveModelDB.LeaveCategory}, LeaveCategory:{((Enums.LeaveCategory)LeaveModelDB.LeaveCategory).GetEnumDescription()}, ModifyDate:{(LeaveModelDB.DateModify.HasValue ? (LeaveModelDB.DateModify.Value).ToString("dd-MMM-yyyy") : string.Empty)}, Created/ModifyBy:{CurrentUser.Uid}({CurrentUser.Name}),Status: " + _statusType);
                                        LogPrint.WriteIntoFile(fileLogPath, "");

                                    }
                                    else
                                    {
                                        Data.saralDT.LevDetails details = new Data.saralDT.LevDetails()
                                        {
                                            Empid = (int)empAttendanceId,
                                            Empdetid = empMaxDetailId,
                                            Levid = (int)LeaveCategory.UnpaidLeave,
                                            Leavedate = date,
                                            Reason = leaveReason,
                                            Firsthalfyn = Convert.ToByte(LeaveModelDB.FirstHalf),
                                            Secondhalfyn = Convert.ToByte(LeaveModelDB.SecondHalf)
                                        };
                                        levDetailsDTService.Save(details);

                                        LogPrint.WriteIntoFile(fileLogPath, $"SARAL UnApproved Data created on:{DateTime.Now}");
                                        LogPrint.WriteIntoFile(fileLogPath, $"EmpUid:{LeaveModelDB.Uid}, AttendanceId:{empAttendanceId.Value}, EmpLeaveId:{LeaveModelDB.LeaveId}, LeaveStartDate:{date.ToString("dd-MMM-yyyy")}, LeaveEndDate:{date.ToString("dd-MMM-yyyy")}, HalfDay:{(LeaveModelDB.IsHalf.HasValue && LeaveModelDB.IsHalf.Value ? true : false)}, FirstHalf:{LeaveModelDB.FirstHalf}, SecondHalf:{LeaveModelDB.SecondHalf}, LeaveType:{LeaveModelDB.LeaveCategory}, LeaveCategory:{((Enums.LeaveCategory)LeaveModelDB.LeaveCategory).GetEnumDescription()}, ModifyDate:{(LeaveModelDB.DateModify.HasValue ? (LeaveModelDB.DateModify.Value).ToString("dd-MMM-yyyy") : string.Empty)}, Created/ModifyBy:{CurrentUser.Uid}({CurrentUser.Name}), Status: " + _statusType);
                                        LogPrint.WriteIntoFile(fileLogPath, "");

                                    }
                                    date = date.AddDays(1);
                                }
                                ExecuteHrmAPI(Convert.ToString(emsLeaveId), empInfo.EmailOffice, LeaveModelDB.StartDate, LeaveModelDB.EndDate, (int)LeaveCategory.UnpaidLeave, leaveModelForm.IsHalf, "InsertUpdate");
                                LogPrint.WriteIntoFile(fileLogPath, $"HRM UnApproved Data created on:{DateTime.Now}");
                                LogPrint.WriteIntoFile(fileLogPath, $"EmpEmail:{empInfo.EmailOffice}, AttendanceId:{empAttendanceId.Value}, EmpLeaveId:{emsLeaveId}, LeaveStartDate:{LeaveModelDB.StartDate.ToString("dd-MMM-yyyy")}, LeaveEndDate:{LeaveModelDB.EndDate.ToString("dd-MMM-yyyy")}, HalfDay:{(leaveModelForm.IsHalf ? true : false)}, LeaveType:{LeaveCategory.UnpaidLeave}, LeaveCategory:{(LeaveCategory.UnpaidLeave).GetEnumDescription()},LeaveCategoryShortName:{(LeaveCategory.UnpaidLeave).GetEnumDisplayShortName()}, Created/ModifyBy:{CurrentUser.Uid}({CurrentUser.Name}),Status: " + _statusType);
                                LogPrint.WriteIntoFile(fileLogPath, "");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogPrint.WriteIntoFile(fileLogPath, $"HRM Data (Delete) Exception created on:{DateTime.Now}");
                            LogPrint.WriteIntoFile(fileLogPath, $"EmpEmail:{empInfo.EmailOffice}, AttendanceId:{empAttendanceId.Value}, EmpLeaveId:{emsLeaveId}, LeaveStartDate:{LeaveModelDB.StartDate.ToString("dd-MMM-yyyy")}, LeaveEndDate:{LeaveModelDB.EndDate.ToString("dd-MMM-yyyy")}, HalfDay:{(leaveModelForm.IsHalf ? true : false)}, LeaveType:{LeaveModelDB.LeaveCategory}, LeaveCategory:{((Enums.LeaveCategory)LeaveModelDB.LeaveCategory).GetEnumDescription()},LeaveCategoryShortName:{((LeaveCategory)LeaveModelDB.LeaveCategory).GetEnumDisplayShortName()}, Created/ModifyBy:{CurrentUser.Uid}({CurrentUser.Name}),Status: " + _statusType);
                            LogPrint.WriteIntoFile(fileLogPath, "");
                        }
                    }
                }
                #endregion

                LogPrint.WriteIntoFile(fileLogPath, $"-------------------------------End-----------------------------");
                LogPrint.WriteIntoFile(fileLogPath, string.Empty);
                LeaveModelDB = IsEdit ? leaveService.GetLeaveById(leaveModelForm.LeaveId) : LeaveModelDB;

                ShowSuccessMessage("Success!", "Leave has been saved successfully", false);

                // get applied  user detail
                if (leaveModelForm.Uid > 0)
                {
                    LeaveModelDB.UserLogin1 = userLoginService.GetUserInfoByID(leaveModelForm.Uid);
                }

                #endregion

                #region "GetEmailIds TO, CC, HandOver"

                //get prefernces settings by PM-Id
                Preference ObjPreference = leaveService.GetPreferecesByPMUid((CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO) ? CurrentUser.Uid : CurrentUser.PMUid);

                //get PM email by using TL  email
                string EmailToPM = ""; //!String.IsNullOrEmpty(ObjPreference.EmailPM) ? ObjPreference.EmailPM : "";
                string emailToTL = string.Empty;
                if (LeaveModelDB.UserLogin1.TLId.HasValue && LeaveModelDB.UserLogin1.TLId.Value > 0)
                {
                    emailToTL = userLoginService.GetUserInfoByID(Convert.ToInt32(LeaveModelDB.UserLogin1.TLId))?.EmailOffice;
                }

                //Get mail Id of PM and HR to CC
                string emailCC = "";

                if (ObjPreference != null)
                {
                    EmailToPM = !String.IsNullOrEmpty(ObjPreference.EmailPM) ? ObjPreference.EmailPM : "";
                    emailCC = ((!String.IsNullOrEmpty(EmailToPM)) ? EmailToPM + ";" : "") + ((!String.IsNullOrEmpty(ObjPreference.EmailHR)) ? ObjPreference.EmailHR + ";" : "") + emailToTL;
                }

                emailCC = emailCC.Trim().TrimEnd(';');

                //Get work alternator's Name & EmailId
                string EmailHandoverTo = leaveModelForm.WorkAlterID != 0 ? userLoginService.GetUserInfoByID(leaveModelForm.WorkAlterID).EmailOffice : "";
                string HandoverName = leaveModelForm.WorkAlterID != 0 ? leaveModelForm.WorkAlternatorName : "";

                #endregion

                #region "SendEmails & CreateOutLookEvent"
                if (SiteKey.IsLive)
                {
                    if (!String.IsNullOrEmpty(CurrentUser.EmailOffice))
                    {
                        //Create OUTLOOK Calendar event
                        //string filepath = CreateOutlookCalendar.CreateEventICSFile(CurrentUser.Name, CurrentUser.EmailOffice, "Leave " + CurrentUser.Name
                        //                                                             , "Jaipur", LeaveModelDB.StartDate.AddDays(-1).AddHours(11)
                        //                                                             , LeaveModelDB.EndDate.AddHours(16)
                        //                                                             , leaveModelForm.Reason);

                        if (!IsEdit || isEditDate)
                        {
                            if (LeaveModelDB.Status.Value == (int)Enums.LeaveStatus.Pending)
                            {
                                //send Leave request Emails
                                SendLeaveRequestEmail(IsPmHrPmo, leaveModelForm, LeaveModelDB, EmailToPM, emailCC);

                                // Send Handover Email
                                SendLeaveHandOverEmail(IsPmHrPmo, leaveModelForm, LeaveModelDB, EmailHandoverTo);
                            }
                        }

                        // Send Email about Leave status
                        SendLeaveStatusEmail(IsPmHrPmo, leaveModelForm, LeaveModelDB, EmailToPM, emailCC);
                    }

                    #endregion

                    //return RedirectToAction(returnToListView ? "index" : "calendar");
                }
                return RedirectToAction(returnToListView ? "index" : "calendar");
            }
            catch (Exception ex)
            {
                LogPrint.WriteIntoFile(fileLogPath, (ex.InnerException ?? ex).ToString());
                ShowErrorMessage("Exception!", ex.Message, false);
                return RedirectToAction("manageleave");
            }
        }

        private void SendLeaveHandOverEmail(bool IsHrPmPmo, LeaveActivityDto leaveModelForm, LeaveActivity LeaveModelDB, string EmailTo)
        {
            #region

            // v0 = HandoverPersonName  v1 = SenderName  v2 = from Date & WeekDay v3 = to Date & WeekDay
            //v4 =HandoverPersonName v5 =  WorkDescription
            string leaveCategory = "";
            FlexiMail objSendMail = new FlexiMail();

            objSendMail.ValueArray = new string[] {
                                                    leaveModelForm.WorkAlternatorName,
                                                    IsHrPmPmo ? userLoginService.GetUserInfoByID(leaveModelForm.Uid).Name : CurrentUser.Name,
                                                    DateTime.ParseExact(leaveModelForm.StartDate,"dd/MM/yyyy",CultureInfo.InvariantCulture).ToString("ddd, MMM dd, yyyy"),
                                                    DateTime.ParseExact(leaveModelForm.EndDate,"dd/MM/yyyy",CultureInfo.InvariantCulture).ToString("ddd, MMM dd, yyyy"),
                                                    leaveModelForm.WorkAlternatorName,
                                                    leaveModelForm.WorkAlternator,
                                                    LeaveModelDB.LeaveCategoryNavigation !=null ? LeaveModelDB.LeaveCategoryNavigation.Name.ToString():"leave"
                                                    };
            string fromemail = CurrentUser.EmailOffice;
            string userName = CurrentUser.Name;
            if (LeaveModelDB != null && LeaveModelDB.UserLogin1 != null)
            {
                leaveCategory = LeaveModelDB.LeaveCategory.HasValue ? ((Enums.LeaveCategory)LeaveModelDB.LeaveCategory).GetEnumDescription() : "Leave";
                userName = LeaveModelDB.UserLogin1.Name;
                var pmuid = 0;
                if (LeaveModelDB.UserLogin1.PMUid.HasValue)
                {
                    pmuid = LeaveModelDB.UserLogin1.PMUid.Value;
                }
                else
                {
                    pmuid = CurrentUser.PMUid;
                }
                var preference = leaveService.GetPreferecesByPMUid(pmuid);
                if (preference != null)
                {
                    fromemail = preference.EmailFrom;
                }
            }

            objSendMail.From = fromemail;
            objSendMail.To = userLoginService.GetUserInfoByID(leaveModelForm.WorkAlterID).EmailOffice; //handover person email
            objSendMail.CC = "";
            objSendMail.BCC = "";
            objSendMail.Subject = (leaveModelForm.LeaveId > 0 ? Enum.GetName(typeof(LeaveStatus), LeaveModelDB.Status) + " " + leaveCategory + "- " : "") + " Work handover details by " + userName;

            objSendMail.MailBodyManualSupply = true;
            objSendMail.MailBody = objSendMail.GetHtml("LeaveHandOverEmail.html"); ;
            objSendMail.Send();

            #endregion
        }

        private void SendLeaveRequestEmail(bool IsHrPmPmo, LeaveActivityDto leaveModelForm, LeaveActivity LeaveModelDB, string EmailTo, string emailCC)
        {
            #region

            //   v0 = ""; // is half  v1 =eave status v2 =from Date & WeekDay, v3 = to Date & WeekDay
            //  v4 = Half/Full day leave, v5 =reason of leave,  v6=handover name, v7=handover desc
            // v7 = contact  v8 = regards name

            FlexiMail objSendMail = new FlexiMail();
            string leaveCategory = "";
            string fromemail = CurrentUser.EmailOffice;
            string userName = CurrentUser.Name;
            if (LeaveModelDB != null && LeaveModelDB.UserLogin1 != null)
            {
                leaveCategory = LeaveModelDB.LeaveCategory.HasValue ? ((Enums.LeaveCategory)LeaveModelDB.LeaveCategory).GetEnumDescription() : "Leave";
                userName = LeaveModelDB.UserLogin1.Name;
                var pmuid = 0;
                if (LeaveModelDB.UserLogin1.PMUid.HasValue)
                {
                    pmuid = LeaveModelDB.UserLogin1.PMUid.Value;
                }
                else
                {
                    pmuid = CurrentUser.PMUid;
                }
                var preference = leaveService.GetPreferecesByPMUid(pmuid);
                if (preference != null)
                {
                    fromemail = preference.EmailFrom;
                }
            }

            objSendMail.From = fromemail;

            //objSendMail.From = "info@dotsquares.com";//CurrentUser.EmailOffice;
            objSendMail.To = EmailTo;  //bhanwar.mali@dotsquares.com
            objSendMail.CC = emailCC;
            objSendMail.BCC = "";
            if (LeaveModelDB.Status == (int)LeaveStatus.Cancelled)
            {
                objSendMail.Subject = (leaveModelForm.LeaveId > 0 ? Enum.GetName(typeof(LeaveStatus), LeaveModelDB.Status) : Enum.GetName(typeof(LeaveStatus), leaveModelForm.Status)) + "Leave Cancel Request From -" + userName;
                objSendMail.MailBody = objSendMail.GetHtml("LeaveCancelEmail.html");
                objSendMail.ValueArray = new string[] { IsHrPmPmo ? userLoginService.GetUserInfoByID(LeaveModelDB.Uid).UserName: CurrentUser.UserName.ToString() };
            }
            else
            {
                objSendMail.ValueArray = new string[] {
                                                        leaveModelForm.IsHalf ? " <b>half day</b>" : "",
                                                        leaveModelForm.LeaveId > 0 ? Enum.GetName(typeof(LeaveType), LeaveModelDB.LeaveType) : Enum.GetName(typeof(LeaveType), leaveModelForm.LeaveType),
                                                        DateTime.ParseExact(leaveModelForm.StartDate,"dd/MM/yyyy",CultureInfo.InvariantCulture).ToString("ddd, MMM dd, yyyy"),
                                                        DateTime.ParseExact(leaveModelForm.EndDate,"dd/MM/yyyy",CultureInfo.InvariantCulture).ToString("ddd, MMM dd, yyyy"),
                                                        leaveModelForm.IsHalf ? "Half" : "Full",
                                                        leaveModelForm.Reason,
                                                        leaveModelForm.WorkAlternatorName,
                                                        leaveModelForm.WorkAlternator,
                                                        IsHrPmPmo?userLoginService.GetUserInfoByID(LeaveModelDB.Uid).MobileNumber :CurrentUser.MobileNumber,
                                                        IsHrPmPmo?userLoginService.GetUserInfoByID(LeaveModelDB.Uid).Name :CurrentUser.Name,
                                                        leaveCategory
                                                        };
                objSendMail.MailBody = objSendMail.GetHtml("LeaveRequestEmail.html");
                objSendMail.Subject = (leaveModelForm.LeaveType == (int)LeaveType.Urgent ? "Urgent " : "") + (leaveModelForm.IsHalf ? "Half Day Leave" : leaveCategory) + " Request -" + userName;
            }
            objSendMail.MailBodyManualSupply = true;

            objSendMail.Send();

            #endregion
        }

        private void SendLeaveStatusEmail(bool IsPmHrPmo, LeaveActivityDto leaveModelForm, LeaveActivity LeaveModelDB, string EmailTo, string emailCC)
        {
            //if (IsPmHrPmo)
            //{
            #region "Parameters Declaration"

            string Status = "";
            string leaveCategory = "";
            string ReciepentName = "";
            string AdminName = "";
            UserLogin usr = userLoginService.GetUserInfoByID(LeaveModelDB.Uid);
            FlexiMail objSendMail = new FlexiMail();

            #endregion

            #region "Set Common Values"
            string fromemail = usr.EmailOffice;
            var preference = leaveService.GetPreferecesByPMUid(usr.PMUid.HasValue ? usr.PMUid.Value : 0);
            if (preference != null)
            {
                fromemail = preference.EmailFrom;
            }
            Status = Enum.GetName(typeof(LeaveStatus), LeaveModelDB.Status);
            leaveCategory = LeaveModelDB.LeaveCategory.HasValue ? ((Enums.LeaveCategory)LeaveModelDB.LeaveCategory).GetEnumDescription() : "Leave";
            ReciepentName = usr.Name;
            AdminName = CurrentUser.Name; //CurrentUser.EmailOffice;
            objSendMail.From = fromemail;   //!String.IsNullOrEmpty(SiteKey.From) ? SiteKey.From.Trim(';') : "info@dotsquares.com";
            objSendMail.To = usr.EmailOffice;
            objSendMail.CC = emailCC;
            objSendMail.BCC = "";
            objSendMail.MailBodyManualSupply = true;
            #endregion

            #region "Email Status Approved"
            if (Status == Enum.GetName(typeof(LeaveStatus), LeaveStatus.Approved))
            {
                // v0=recipent name ,v1=  half or full ,v2=leave period, v3 =admin name, v4=remark by admin
                objSendMail.ValueArray = new string[] { ReciepentName,
                                                      Convert.ToBoolean(LeaveModelDB.IsHalf)?"Half":"Full",
                                                      LeaveModelDB.StartDate.ToString("ddd, MMM dd, yyyy")+ " to " + LeaveModelDB.EndDate.ToString("ddd, MMM dd, yyyy"),
                                                      AdminName,
                                                      LeaveModelDB.Remark==null?"":LeaveModelDB.Remark,
                                                      leaveModelForm==null?string.Empty:leaveModelForm.WorkAlternatorName,
                                                      leaveModelForm==null?string.Empty:leaveModelForm.WorkAlternator,
                                                      LeaveModelDB.Reason==null?string.Empty:LeaveModelDB.Reason,
                                                      leaveCategory,
                                                       LeaveModelDB.Remark==null?"":"Approver's Comment: "
                                                      };
                //objSendMail.Subject = (LeaveModelDB.LeaveType == (int)LeaveType.Urgent ? "Urgent " : "Normal") + " Leave Approved";
                objSendMail.Subject = (LeaveModelDB.LeaveType == (int)LeaveType.Urgent ? "Urgent " : "") + (leaveCategory != null ? leaveCategory : "") + " Approved";
                objSendMail.MailBody = objSendMail.GetHtml("LeaveStatusApprovedEmail.html");
            }
            #endregion

            #region "Email Status UnApproved"
            else if (Status == Enum.GetName(typeof(LeaveStatus), LeaveStatus.UnApproved))
            {
                // v0 =reciepent, v1 =remark by admin,v2= admin name
                objSendMail.ValueArray = new string[]  {ReciepentName,
                                                        LeaveModelDB.Remark==null?"":"Approver's Comment: "+LeaveModelDB.Remark,
                                                        AdminName,
                                                       LeaveModelDB.StartDate.ToString("ddd, MMM dd, yyyy")+ " to " + LeaveModelDB.EndDate.ToString("ddd, MMM dd, yyyy"),
                                                       LeaveModelDB.Reason==null?string.Empty:LeaveModelDB.Reason,
                                                      leaveCategory,
                                                      LeaveModelDB.Remark==null?"":"Because, the reason is: "
                                                            };
                objSendMail.Subject = (LeaveModelDB.LeaveType == (int)LeaveType.Urgent ? "Urgent " : "") + (leaveCategory != null ? leaveCategory : "") + " UnApproved";
                objSendMail.MailBody = objSendMail.GetHtml("LeaveStatusUnApprovedEmail.html");
            }
            #endregion

            #region "Email Status Cancelled"
            else if (Status == Enum.GetName(typeof(LeaveStatus), LeaveStatus.Cancelled))
            {
                // v0 =reciepent, v1 =remark  by admin,v2= admin name
                objSendMail.ValueArray = new string[]  {ReciepentName,
                                                        LeaveModelDB.Remark==null?"":"Approver's Comment: "+LeaveModelDB.Remark,
                                                        AdminName,
                                                        LeaveModelDB.StartDate.ToString("ddd, MMM dd, yyyy")+ " to " + LeaveModelDB.EndDate.ToString("ddd, MMM dd, yyyy"),
                                                        LeaveModelDB.Reason==null?string.Empty:LeaveModelDB.Reason,
                                                        leaveCategory,
                                                        LeaveModelDB.Remark==null?"":"Because, the reason is: "
                                                    };
                objSendMail.Subject = (LeaveModelDB.LeaveType == (int)LeaveType.Urgent ? "Urgent " : "") + (leaveCategory != null ? leaveCategory : "") + " Cancelled";
                objSendMail.MailBody = objSendMail.GetHtml("LeaveStatusCancelledEmail.html");
            }
            #endregion

            #region "Email Status Pending/Others"
            else
            {
                // v0 =reciepent
                objSendMail.ValueArray = new string[] { ReciepentName,
                    LeaveModelDB.StartDate.ToString("ddd, MMM dd, yyyy") + " to " + LeaveModelDB.EndDate.ToString("ddd, MMM dd, yyyy"),
                    leaveModelForm==null?string.Empty:leaveModelForm.WorkAlternatorName,
                    leaveModelForm==null?string.Empty:leaveModelForm.WorkAlternator,
                    LeaveModelDB.Reason==null?string.Empty:LeaveModelDB.Reason,
                    leaveCategory
                };
                objSendMail.Subject = (LeaveModelDB.LeaveType == (int)LeaveType.Urgent ? "Urgent " : "") + (leaveCategory != null ? leaveCategory : "") + " Pending";
                objSendMail.MailBody = objSendMail.GetHtml("LeaveStatusPendingEmail.html");
            }

            #endregion

            objSendMail.Send();

            //}
        }

        [HttpGet]
        public ActionResult LeaveInfo(int id)
        {
            var model = new LeaveActivityCalenderDto();
            try
            {
                var leave = leaveService.GetLeaveActivityById(id);
                if (leave != null)
                {
                    model.LeaveId = leave.LeaveId;
                    model.Name = userLoginService.GetUserInfoByID(leave.Uid).Name;
                    int tlid = userLoginService.GetUserInfoByID(leave.Uid).TLId ?? 0;
                    model.TLName = userLoginService.GetUserInfoByID(tlid)?.Name;
                    model.StartDate = leave.StartDate;
                    model.EndDate = leave.EndDate;
                    model.Uid = leave.Uid;
                    model.Reason = leave.Reason;
                    model.Remark = leave.Remark;
                    model.DateAdded = leave.DateAdded;
                    model.IsHalf = leave.IsHalf.HasValue ? leave.IsHalf.Value : false;
                    model.WorkAlternator = leave.WorkAlternator;
                    model.Status = leave.Status;
                    model.LeaveType = leave.LeaveType.HasValue ? leave.LeaveType.Value : (int)LeaveType.Normal;
                    model.HandoverTo = leave.WorkAlterId.HasValue ? userLoginService.GetUserInfoByID(leave.WorkAlterId.Value)?.Name : "";
                    model.LeaveCatagory = leave.LeaveCategoryNavigation != null ? leave.LeaveCategoryNavigation.Name : string.Empty;
                    model.LeaveCatagoryId = leave.LeaveCategory != null ? leave.LeaveCategory : (int)Enums.LeaveCategory.UnpaidLeave;
                }
            }
            catch (Exception)
            {
            }
            ViewBag.LeaveStatus = SelectListHelper.GetLeaveStatus();
            model.IsAllowLeave = true;

            var pmId = (CurrentUser.RoleId == (int)Enums.UserRoles.PM ? CurrentUser.Uid : CurrentUser.PMUid);

            if (pmId > 0)
            {
                var _prefereces = leaveService.GetPreferecesByPMUid(pmId);
                if (_prefereces != null)
                {
                    if (_prefereces.IsAllowLeaveByTL && CurrentUser.RoleId != (int)Enums.UserRoles.PM)
                    {
                        model.IsAllowLeave = false;
                    }

                }
            }

            return PartialView("_LeaveInfo", model);
        }

        [HttpPost]
        public ActionResult LeaveInfo_Old(LeaveActivityDto model)
        {
            try
            {
                var leave = leaveService.GetLeaveActivityById(model.LeaveId);
                if (leave != null)
                {
                    leave.Status = model.Status;
                    leaveService.Save(leave);
                }
                ShowSuccessMessage("Success", "Leave status has been successfully updated", false);
            }
            catch (Exception)
            {
                ShowWarningMessage("Warning", "Leave status cannot be updated, please contact to admin.", false);
            }

            return Json(new { isSuccess = true, redirectUrl = Url.Action("index", "Leave", new { steps = "reload" }) });
        }

        [HttpPost]
        public ActionResult LeaveInfo(LeaveActivityCalenderDto model)
        {
            try
            {
                var leave = leaveService.GetLeaveActivityById(model.LeaveId);
                if (leave != null)
                {
                    var leaveData = leave;
                    leave.Status = model.Status;
                    leave.Remark = model.Remark;
                    leave.ModifyBy = CurrentUser.Uid;
                    leave.DateModify = DateTime.Now;
                    string leaveReason = leave.Reason.Length > 100 ? leave.Reason.Substring(0, 100) : leave.Reason;
                    if (model.Status == (int)Enums.LeaveStatus.Cancelled)
                    {
                        ExecuteHrmAPI(Convert.ToString(model.LeaveId), leave.UserLogin1.EmailOffice, leave.StartDate, leave.EndDate, (int)leave.LeaveCategory, leave.IsHalf.Value, "Delete");
                    }
                    else if (model.Status == (int)Enums.LeaveStatus.UnApproved)
                    {
                        ExecuteHrmAPI(Convert.ToString(model.LeaveId), leave.UserLogin1.EmailOffice, leave.StartDate, leave.EndDate, (int)leave.LeaveCategory, leave.IsHalf.Value, "Delete");
                    }
                    leaveService.Save(leave);
                    var emsLeaveId = leave.LeaveId;
                    var empInfo = userLoginService.GetUserInfoByID(leave.Uid);
                    int? empAttendanceId = empInfo.AttendenceId.HasValue ? empInfo.AttendenceId.Value : 0;
                    if (empAttendanceId.Value == 0)
                    {
                        if (empInfo != null)
                        {
                            if (empInfo.IsFromDbdt == false || empInfo.IsFromDbdt == null)
                            {
                                var saralUserInfo = levDetailsService.GetEmployeeInfo(empInfo.EmailOffice);
                                empAttendanceId = saralUserInfo != null ? saralUserInfo.Empid > 0 ? saralUserInfo.Empid : 0 : 0;
                            }
                            else
                            {
                                var saralDTUserInfo = levDetailsDTService.GetDTEmployeeInfo(empInfo.EmailOffice);
                                empAttendanceId = saralDTUserInfo != null ? saralDTUserInfo.Empid > 0 ? saralDTUserInfo.Empid : 0 : 0;
                            }
                        }
                    }
                    var attendId = SiteKey.AttendenceId.Split(',');
                    if (CurrentUser.RoleId != (int)UserRoles.UKBDM && CurrentUser.RoleId != (int)UserRoles.UKPM && CurrentUser.RoleId != (int)UserRoles.AUPM && CurrentUser.RoleId != (int)UserRoles.PMOAU && CurrentUser.RoleId != (int)UserRoles.PMO && empAttendanceId.HasValue && empAttendanceId.Value > 0)
                    {
                        int empMaxDetailId = 0;
                        if (empInfo.IsFromDbdt == true)
                        {
                            empMaxDetailId = levAllotmentDTService.GetDTMaxAllotmentValue(empAttendanceId.Value);
                        }
                        else
                        {
                            empMaxDetailId = levAllotmentService.GetMaxAllotmentValue(empAttendanceId.Value);
                        }
                        var monthYearStart = ((leaveData.StartDate.Year) * 12) + (leaveData.StartDate.Month);
                        var monthYearEnd = ((leaveData.EndDate.Year) * 12) + (leaveData.EndDate.Month);
                        #region ---- pending status commented code ----
                        int leaveDays = (int)((leave.EndDate.Date - leave.StartDate.Date).TotalDays);
                        if (leaveData.Status == (int)LeaveStatus.Pending)
                        {
                            var date = leaveData.StartDate.Date;

                            var levDetailsDT = levDetailsDTService.GetDTLeaveDetailsByLeaveDate(empAttendanceId, leave.StartDate, leave.EndDate);
                            if (levDetailsDT.Count > 0)
                            {
                                foreach (var item in levDetailsDT)
                                {
                                    levDetailsDTService.Delete(item);
                                }
                            }
                            else
                            {
                                var levDetails = levDetailsService.GetLeaveDetailsByLeaveDate(empAttendanceId, leave.StartDate, leave.EndDate);
                                if (levDetails.Count > 0)
                                {
                                    foreach (var item in levDetails)
                                    {
                                        levDetailsService.Delete(item);
                                    }
                                }
                            }
                            for (int i = 0; i <= leaveDays; i++)
                            {
                                if (empInfo.IsFromDbdt == false || empInfo.IsFromDbdt == null)
                                {
                                    Data.saral.LevDetails details = new Data.saral.LevDetails()
                                    {
                                        Empid = (int)empAttendanceId,
                                        Empdetid = empMaxDetailId,
                                        Levid = (int)leave.LeaveCategory,
                                        Leavedate = date,
                                        Reason = leaveReason,
                                        Firsthalfyn = Convert.ToByte(leave.FirstHalf),
                                        Secondhalfyn = Convert.ToByte(leave.SecondHalf)
                                    };
                                    levDetailsService.Save(details);
                                }
                                else
                                {
                                    Data.saralDT.LevDetails detailsDT = new Data.saralDT.LevDetails()
                                    {
                                        Empid = (int)empAttendanceId,
                                        Empdetid = empMaxDetailId,
                                        Levid = (int)leave.LeaveCategory,
                                        Leavedate = date,
                                        Reason = leaveReason,
                                        Firsthalfyn = Convert.ToByte(leave.FirstHalf),
                                        Secondhalfyn = Convert.ToByte(leave.SecondHalf)
                                    };
                                    levDetailsDTService.Save(detailsDT);
                                }
                                date = date.AddDays(1);
                            }

                            if (monthYearStart == monthYearEnd)
                            {

                                var leaveMonth = levMonthdetService.GetLeaveMonthDetail(empAttendanceId, monthYearStart);
                                if (leaveMonth != null)
                                {
                                    leaveMonth.Editmodeyn = 1;
                                    levMonthdetService.Save(leaveMonth);
                                }
                                else
                                {
                                    var leaveMonthDT = levMonthdetDTService.GetLeaveMonthDetail(empAttendanceId, monthYearStart);
                                    if (leaveMonthDT != null)
                                    {
                                        leaveMonthDT.Editmodeyn = 1;
                                        levMonthdetDTService.Save(leaveMonthDT);
                                    }
                                }
                            }
                            else
                            {
                                for (int i = monthYearStart; i <= monthYearEnd; i++)
                                {
                                    var leaveMonth = levMonthdetService.GetLeaveMonthDetail(empAttendanceId, monthYearStart);
                                    if (leaveMonth != null)
                                    {
                                        leaveMonth.Editmodeyn = 1;
                                        levMonthdetService.Save(leaveMonth);
                                    }
                                    else
                                    {
                                        var leaveMonthDT = levMonthdetDTService.GetLeaveMonthDetail(empAttendanceId, monthYearStart);
                                        if (leaveMonthDT != null)
                                        {
                                            leaveMonthDT.Editmodeyn = 1;
                                            levMonthdetDTService.Save(leaveMonthDT);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        #endregion

                        if (leaveData.Status == (int)LeaveStatus.UnApproved || leaveData.Status == (int)LeaveStatus.Cancelled)
                        {

                            var levDetails = levDetailsService.GetLeaveDetailsByLeaveDate(empAttendanceId, leaveData.StartDate, leaveData.EndDate);
                            if (levDetails.Count > 0)
                            {
                                foreach (var item in levDetails)
                                {
                                    levDetailsService.Delete(item);
                                    int month = (item.Leavedate.Year * 12) + item.Leavedate.Month;
                                    var leaveMonth = levMonthdetService.GetLeaveMonthDetail(empAttendanceId, month);
                                    if (leaveMonth != null)
                                    {
                                        leaveMonth.Editmodeyn = 1;
                                        levMonthdetService.Save(leaveMonth);
                                    }
                                }
                            }
                            else
                            {
                                var levDetailsDT = levDetailsDTService.GetDTLeaveDetailsByLeaveDate(empAttendanceId, leaveData.StartDate, leaveData.EndDate);
                                if (levDetailsDT.Count > 0)
                                {
                                    foreach (var item in levDetailsDT)
                                    {
                                        levDetailsDTService.Delete(item);
                                        int month = (item.Leavedate.Year * 12) + item.Leavedate.Month;
                                        var leaveMonth = levMonthdetDTService.GetLeaveMonthDetail(empAttendanceId, month);
                                        if (leaveMonth != null)
                                        {
                                            leaveMonth.Editmodeyn = 1;
                                            levMonthdetDTService.Save(leaveMonth);
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                //--------------------------------
                int? LeadId = leave.UserLogin1.TLId;
                string EmailTo = leave.UserLogin1.EmailOffice;
                bool isApproved = model.Status == (int)Enums.LeaveStatus.Approved ? true : false;

                if (!String.IsNullOrEmpty(EmailTo) &&
                    (isApproved || model.Status == (int)Enums.LeaveStatus.Cancelled))
                {
                    string EmailFrom = !String.IsNullOrEmpty(SiteKey.From) ? SiteKey.From.Trim(';') : "info@dotsquares.com";
                    string EmailLeadCC = LeadId.HasValue ? userLoginService.GetUserInfoByID(LeadId.Value).EmailOffice : string.Empty;
                    string EmailCC = EmailLeadCC, subject = string.Empty;

                    subject = (leave.LeaveType == (int)Enums.LeaveType.Urgent ? Enums.LeaveType.Urgent.ToString() : Enums.LeaveType.Normal.ToString());
                    subject += " Leave " + (isApproved ? Enums.LeaveStatus.Approved.ToString() : Enums.LeaveStatus.Cancelled.ToString());

                    //get prefernces settings by PM-Id
                    Preference ObjPreference = leaveService.GetPreferecesByPMUid((CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO) ? CurrentUser.Uid : CurrentUser.PMUid);

                    //get PM email by using TL  email
                    string EmailToPM = !String.IsNullOrEmpty(ObjPreference.EmailPM) ? ObjPreference.EmailPM : "";

                    //Get mail Id of PM and HR to CC
                    string emailCC = "";
                    if (ObjPreference != null)
                    {
                        emailCC = ((!String.IsNullOrEmpty(EmailToPM)) ? EmailToPM + ";" : "") + ((!String.IsNullOrEmpty(ObjPreference.EmailHR)) ? ObjPreference.EmailHR : "");
                    }
                    EmailCC = !string.IsNullOrEmpty(EmailLeadCC) ? EmailLeadCC + (!String.IsNullOrEmpty(EmailCC) ? (";" + EmailCC.Trim(';')) : "") : !String.IsNullOrEmpty(EmailCC) ? EmailCC.Trim(';') : "";

                    bool IsPmHrPmo = (RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId) || CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO
                        || RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_Sales_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_HR_DesignationIds.Contains(CurrentUser.DesignationId)
                        ) ? true : false;
                    if (SiteKey.IsLive)
                    {
                        SendLeaveStatusEmail(IsPmHrPmo, null, leave, EmailTo, emailCC);
                    }
                }

                ShowSuccessMessage("Success", "Leave status has been successfully updated", false);
            }
            catch (Exception ex)
            {
                ShowWarningMessage("Warning", "Leave status cannot be updated, please contact to admin.", false);
            }

            return Json(new { isSuccess = true, redirectUrl = Url.Action("calendar", "Leave", new { steps = "reload" }) });
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

        public ActionResult GetPriorLeaveByPM(int pmid)
        {
            int priorday = 5;
            try
            {
                Preference objPreference = leaveService.GetPreferecesByPMUid(pmid);

                if (objPreference != null && objPreference.PriorLeaveDay.HasValue)
                {
                    priorday = objPreference.PriorLeaveDay.Value;
                }
            }
            catch
            {
                priorday = 5;
            }
            return Json(new { leavepriordays = priorday });
        }

        public ActionResult GetEmployeeList(string pmid)
        {
            var users = Convert.ToInt32(pmid) > 0 ? leaveService.GetLeavesEmployeeListByUid(Convert.ToInt32(pmid), (int)Enums.UserRoles.PM) :
                leaveService.GetLeavesEmployeeListByUidDesignation(CurrentUser.Uid, CurrentUser.RoleId, CurrentUser.DesignationId);

            var leavesEmployee = users.Count > 0 ? users.Select(n => new KeyValuePair<int, string>(n.Uid, n.UserLogin1.Name)).ToList().Distinct() : null;

            SelectList items = new SelectList(
            leavesEmployee?.Select(x => new { Value = x.Key, Text = x.Value }),
            "Value",
            "Text");
            return Json(items);
        }

        //[CustomActionAuthorization]
        public ActionResult OfficialLeave()
        {
            int countryId = 0;

            //if (CurrentUser.RoleId == (int)UserRoles.UKPM || CurrentUser.RoleId == (int)UserRoles.UKBDM || CurrentUser.RoleId == (int)UserRoles.PMO)
            //{
            //    countryId = (byte)Country.UK;
            //}
            //else
            if (CurrentUser.RoleId != (int)UserRoles.HRBP && CurrentUser.RoleId != (int)UserRoles.PM && CurrentUser.RoleId != (int)UserRoles.UKPM && CurrentUser.RoleId != (int)UserRoles.UKBDM && CurrentUser.RoleId != (int)UserRoles.PMO)
            {
                countryId = (byte)Country.India;
            }
            List<OfficialLeave> _leaveList = leaveService.GetOfficialLeavesListNew(countryId);
            var officialLeave = _leaveList.Select(x => new OfficialLeaveDto
            {
                Title = x.Title,
                LeaveDate = x.LeaveDate,
                LeaveType = x.LeaveType,
                CountryId = x.CountryId
            }).ToList();
            return View(officialLeave);
        }

        public FileResult DownloadLeavedataExcel()
        {
            var searchFilter = TempData.Get<SpecialSearchFilterViewModel>("searchFilter");
            DateTime dateStart, dateEnd;
            var expr = PredicateBuilder.True<LeaveActivity>();
            int pmId = (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO) ? CurrentUser.Uid : CurrentUser.PMUid;
            if (CurrentUser.Uid > 0)
            {
                if (CurrentUser.RoleId == (int)UserRoles.PM || CurrentUser.RoleId == (int)UserRoles.PMO || CurrentUser.RoleId == (int)UserRoles.UKBDM)
                {
                    expr = expr.And(e => e.Uid == CurrentUser.Uid || (e.UserLogin1.PMUid.HasValue && e.UserLogin1.PMUid.Value == pmId));
                }
                else if (RoleValidator.TL_Technical_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_Sales_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(CurrentUser.DesignationId)
            || RoleValidator.TL_HR_DesignationIds.Contains(CurrentUser.DesignationId)
                        //     || RoleValidator.DV_RoleIds.Contains(CurrentUser.RoleId)
                        )
                {
                    int[] sdList = userLoginService.GetTLUsers(CurrentUser.Uid).Select(T => T.Uid).ToArray();
                    expr = expr.And(e => e.Uid == CurrentUser.Uid || (e.UserLogin1.TLId.HasValue && e.UserLogin1.TLId.Value == CurrentUser.Uid) || (sdList.Contains((int)e.UserLogin1.TLId))
                    );
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
            if (searchFilter.leavetype.HasValue && searchFilter.leavetype.Value != 0)
            {
                expr = expr.And(l => l.LeaveType == searchFilter.leavetype);
            }
            if (searchFilter.leavecatagory.HasValue && searchFilter.leavecatagory.Value != 0)
            {
                expr = expr.And(l => l.LeaveCategory == searchFilter.leavecatagory);
            }
            if (searchFilter.pm.HasValue && searchFilter.pm.Value != 0)
            {
                expr = expr.And(l => l.UserLogin1.PMUid == searchFilter.pm);
            }
            if (!string.IsNullOrEmpty(searchFilter.startDate) && DateTime.TryParseExact(searchFilter.startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateStart))
            {
                expr = expr.And(l => l.StartDate >= dateStart || l.EndDate >= dateStart);
            }
            if (!string.IsNullOrEmpty(searchFilter.endDate) && DateTime.TryParseExact(searchFilter.endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateEnd))
            {
                expr = expr.And(l => l.EndDate <= dateEnd || l.StartDate <= dateEnd);
            }
            var model = leaveService.GetLeaveActivity(expr);
            List<LeaveReportViewModel> leavereport = new List<LeaveReportViewModel>();
            foreach (var item in model)
            {
                leavereport.Add(new LeaveReportViewModel()
                {
                    EmpName = item.UserLogin1.Name,
                    StartDate = item.StartDate.ToString("ddd, MMM dd, yyyy"),
                    EndDate = item.EndDate.ToString("ddd, MMM dd, yyyy"),
                    LeaveType = item.LeaveType != null ? (item.IsHalf == true ? "Half" + (item.TypeMaster != null && item.TypeMaster.TypeName.ToLower().Trim() != "normal" ? "(" + item.TypeMaster.TypeName + ")" : " ") : "Full") : (item.TypeMaster != null && item.TypeMaster.TypeName.ToLower().Trim() != "normal" ? "(" + item.TypeMaster.TypeName + ")" : " "),
                    LeaveCatagory = item.LeaveCategoryNavigation != null ? item.LeaveCategoryNavigation.Name : " ",
                    Reason = item.Reason,
                    Status = item.Status.HasValue ? Enum.GetName(typeof(LeaveStatus), item.Status.Value) : " ",
                    ApplyDate = item.DateAdded != null ? item.DateAdded.Value.ToString("ddd, MMM dd, yyyy") : " "
                });
            }
            string filename = "LeaveDataReport_" + DateTime.Now.Ticks.ToString() + ".xlsx";
            string[] columns = { "EmpName", "StartDate", "EndDate", "LeaveType", "LeaveCatagory", "Reason", "Status", "ApplyDate" };
            byte[] filecontent = ExportExcelHelper.ExportExcel(leavereport, "LeaveReport", true, columns);
            string fileName = filename;
            return File(filecontent, ExportExcelHelper.ExcelContentType, fileName);
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult AdjustHours()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdjustHours(AdjustHoursDto model)
        {
            List<LateHour> lateHour = new List<LateHour>();
            if (model.EmployeeList != null)
            {
                foreach (var item in model.EmployeeList.Where(s => !string.IsNullOrWhiteSpace(s.LateStartTime) || !string.IsNullOrWhiteSpace(s.EarlyLeaveTime) || !string.IsNullOrWhiteSpace(s.WorkAtHome) || !string.IsNullOrWhiteSpace(s.WorkFromHome) || s.Id > 0))
                {
                    TimeSpan? timeLateStart = null;
                    TimeSpan? timeEarlyLeave = null;
                    string WorkAtHome = null;
                    string WorkFromHome = null;
                    int LeaveType = 1;
                    if (!string.IsNullOrWhiteSpace(item.LateStartTime) && item.LateStartTime != "00:00")
                    {
                        timeLateStart = TimeSpan.Parse(item.LateStartTime);
                    }
                    if (!string.IsNullOrWhiteSpace(item.EarlyLeaveTime) && item.EarlyLeaveTime != "00:00")
                    {
                        timeEarlyLeave = TimeSpan.Parse(item.EarlyLeaveTime);
                    }
                    if (!string.IsNullOrWhiteSpace(item.WorkAtHome))
                    {
                        LeaveType = 2;
                        WorkAtHome = item.WorkAtHome;
                    }
                    if (!string.IsNullOrWhiteSpace(item.WorkFromHome))
                    {
                        LeaveType = 3;
                        WorkFromHome = item.WorkFromHome;
                    }
                    lateHour.Add(new LateHour
                    {
                        DayOfDate = DateTime.ParseExact(model.Modified.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Uid = item.Uid,
                        LateStartTimeDiff = timeLateStart,
                        EarlyLeaveTimeDiff = timeEarlyLeave,
                        Modified = DateTime.Now,
                        ModifiedByUid = CurrentUser.Uid,
                        Id = item.Id,
                        LeaveType = LeaveType,
                        WorkAtHome = WorkAtHome,
                        WorkFromHome = WorkFromHome,
                        EarlyReason = item.EarlyReason,
                        LateReason = item.LateReason
                    });
                }
                DateTime modifiedDate = DateTime.ParseExact(model.Modified.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                leaveService.SaveAdjustHour(lateHour);
            }
            return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record has been saved Successfully.", IsSuccess = true, RedirectUrl = Url.Action("AdjustHours", "Leave"), Data = model.Modified });
        }

        [CustomActionAuthorization]
        public ActionResult GetAdjustHours(string modifiedDate)
        {
            AdjustHoursDto model = new AdjustHoursDto();
            if (!String.IsNullOrEmpty(modifiedDate))
            {
                List<LateHour> data = new List<LateHour>();
                DateTime modified = DateTime.ParseExact(modifiedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var EmployeeList = userLoginService.GetUsersListByAllDesignation(CurrentUser.Uid, CurrentUser.RoleId, CurrentUser.DesignationId);
                model.EmployeeList = EmployeeList.Select(x => new EmployeeListDto { Name = x.Name.ToString(), Uid = x.Uid }).ToList();
                data = leaveService.getLateHour(modified);
                foreach (var item in data)
                {
                    var emp = model.EmployeeList.FirstOrDefault(x => x.Uid == item.Uid);
                    if (emp != null)
                    {
                        emp.LateStartTime = item.LateStartTimeDiff != null ? item.LateStartTimeDiff.Value.ToString(@"hh\:mm") : null;
                        emp.EarlyLeaveTime = item.EarlyLeaveTimeDiff != null ? item.EarlyLeaveTimeDiff.Value.ToString(@"hh\:mm") : null;
                        emp.Id = item.Id;
                        emp.WorkAtHome = item.WorkAtHome;
                        emp.LeaveType = item.LeaveType;
                        emp.LateReason = item.LateReason;
                        emp.EarlyReason = item.EarlyReason;
                        emp.WorkFromHome = item.WorkFromHome;

                    }
                }
            }
            return PartialView("_GetAdjustHours", model);
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult LateHour()
        {
            EmployeeListDto model = new EmployeeListDto();
            var EmployeeList = userLoginService.GetUsersListByAllDesignation(CurrentUser.Uid, CurrentUser.RoleId, CurrentUser.DesignationId);
            model.SelectEmployeeList = EmployeeList.Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.Uid.ToString() }).ToList();
            return View(model);
        }

        [HttpPost]
        public DataTablesJsonResult LateHour(IDataTablesRequest request, SpacialSearch filter)
        {
            TempData.Put("filter", filter);

            DateTime dateStart, dateEnd;

            var pagingServices = new PagingService<LateHour>(request.Start, request.Length);
            var expr = PredicateBuilder.True<LateHour>();

            if (!string.IsNullOrEmpty(filter.dateFrom) && DateTime.TryParseExact(filter.dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateStart))
            {
                expr = expr.And(l => l.DayOfDate >= dateStart);
            }
            if (!string.IsNullOrEmpty(filter.dateTo) && DateTime.TryParseExact(filter.dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateEnd))
            {
                expr = expr.And(l => l.DayOfDate <= dateEnd);
            }

            if (filter.search_text != null && filter.search_text > 0)
            {
                expr = expr.And(x => x.LeaveType == filter.search_text);
            }
            if (filter.EmployeeId != null && filter.EmployeeId > 0)
            {
                expr = expr.And(x => x.UserLogin1.Uid == filter.EmployeeId);
            }
            int pmuid = CurrentUser.RoleId == (int)Enums.UserRoles.PM ? CurrentUser.RoleId : CurrentUser.PMUid;

            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "Name":
                            return o.OrderByColumn(item, c => c.Modified);

                        default:
                            return o.OrderByColumn(item, c => c.Id);
                    }
                }
                return o.OrderByDescending(c => c.Modified);
            };
            int totalCount = 0;

            var response = leaveService.GetLateHour(out totalCount, pagingServices);

            Dictionary<string, Object> additionalParameters = new Dictionary<string, Object>();

            if (filter.EmployeeId != null && filter.EmployeeId > 0)
            {
                var model = leaveService.GetLateHourList(expr);
                string TotalLateTime = CalculateTotalLateTime(model);
                if (!string.IsNullOrEmpty(TotalLateTime))
                {
                    additionalParameters.Add("TotalLateTime", TotalLateTime);
                }
                else { additionalParameters = null; }
            }
            else { additionalParameters = null; }


            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                id = r.Id,
                rowIndex = index + 1 + (request.Start),
                empName = r.UserLogin1.Name,
                dayOfDate = r.DayOfDate.ToString("ddd, MMM dd, yyyy"),
                lateHour = r.WorkFromHome != null ? "<a href='javascript:void(0)' style='cursor:pointer' title='" + r.WorkFromHome + "'> Work From Home </a>" : r.WorkAtHome != null ? "<a href='javascript:void(0)' style='cursor:pointer' title='" + r.WorkAtHome + "'> UnAuthorized </a>" : r.LateStartTimeDiff != null ? "<b>Late Hour</b> (<span style=color:red; >" + (r.LateStartTimeDiff.Value.Hours > 0 ? r.LateStartTimeDiff.Value.Hours.ToString() + " hr " : "")
                + (r.LateStartTimeDiff.Value.Minutes > 0 ? r.LateStartTimeDiff.Value.Minutes.ToString() + " min" : "") + "</span>)"
                + (r.EarlyLeaveTimeDiff != null ? "&nbsp;<b>Early Leave</b> (<span style=color:red; >" + (r.EarlyLeaveTimeDiff.Value.Hours > 0 ? r.EarlyLeaveTimeDiff.Value.Hours.ToString() + " hr " : "")
                + (r.EarlyLeaveTimeDiff.Value.Minutes > 0 ? r.EarlyLeaveTimeDiff.Value.Minutes.ToString() + " min" : "") + "</span>)" : "") : (r.EarlyLeaveTimeDiff != null ? "<b>Early Leave</b> (<span style=color:red; >" + (r.EarlyLeaveTimeDiff.Value.Hours > 0 ? r.EarlyLeaveTimeDiff.Value.Hours.ToString() + " hr " : "")
                + (r.EarlyLeaveTimeDiff.Value.Minutes > 0 ? r.EarlyLeaveTimeDiff.Value.Minutes.ToString() + " min" : "") + "</span>)" : ""),
                reason = r.LeaveType == 3 ? "&nbsp;" + r.WorkFromHome : r.LeaveType == 2 ? "&nbsp;" + r.WorkAtHome : r.LateReason != null ? "&nbsp;<b>Late Reason</b> (" + r.LateReason + ")" + (r.EarlyReason != null ? ",<b> Early Reason</b> (" + r.EarlyReason + ")" : "") : r.EarlyReason != null ? "<b> Early Reason</b>(" + r.EarlyReason + ")" : ""
            }), additionalParameters);



            //return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            //{
            //    id = r.Id,
            //    rowIndex = index + 1 + (request.Start),
            //    empName = r.UserLogin1.Name,
            //    dayOfDate = r.DayOfDate.ToString("ddd, MMM dd, yyyy"),
            //    lateHour = r.WorkAtHome != null ? "<a href='javascript:void(0)' style='cursor:pointer' title='" + r.WorkAtHome + "'> UnAuthorized </a>" :
            //    r.LateStartTimeDiff != null ? "<b>Late Hour</b> (<span style=color:red; >" + (r.LateStartTimeDiff.Value.Hours > 0 ? r.LateStartTimeDiff.Value.Hours.ToString() + " hr " : "") + (r.LateStartTimeDiff.Value.Minutes > 0 ? r.LateStartTimeDiff.Value.Minutes.ToString() : "") + " min" + "</span>)"
            //    + (r.EarlyLeaveTimeDiff != null ? "&nbsp;<b>Early Leave</b> (<span style=color:red; >" + (r.EarlyLeaveTimeDiff.Value.Hours > 0 ? r.EarlyLeaveTimeDiff.Value.Hours.ToString() + " hr " : "") + (r.EarlyLeaveTimeDiff.Value.Minutes > 0 ? r.EarlyLeaveTimeDiff.Value.Minutes.ToString() : "") + " min" + "</span>)" : "") : (r.EarlyLeaveTimeDiff != null ? "<b>Early Leave</b> (<span style=color:red; >" + (r.EarlyLeaveTimeDiff.Value.Hours > 0 ? r.EarlyLeaveTimeDiff.Value.Hours.ToString() + " hr " : "") + (r.EarlyLeaveTimeDiff.Value.Minutes > 0 ? r.EarlyLeaveTimeDiff.Value.Minutes.ToString() : "") + " min" + "</span>)" : ""),
            //    reason = r.LeaveType == 2 ? "&nbsp;" + r.WorkAtHome : r.LateReason != null ? "&nbsp;<b>Late Reason</b> (" + r.LateReason + ")" + (r.EarlyReason != null ? ",<b> Early Reason</b> (" + r.EarlyReason + ")" : "") : r.EarlyReason != null ? "<b> Early Reason</b>(" + r.EarlyReason + ")" : ""
            //}), additionalParameters);
        }


        [NonAction]
        private string CalculateTotalLateTime(List<LateHour> lateHours, bool withHtmlTag = true)
        {
            string result = String.Empty;
            double totalLateMin = 0;
            foreach (LateHour lateHour in lateHours)
            {
                if (lateHour.LateStartTimeDiff.HasValue)
                {
                    totalLateMin += lateHour.LateStartTimeDiff.Value.TotalMinutes;
                }
                if (lateHour.EarlyLeaveTimeDiff.HasValue)
                {
                    totalLateMin += lateHour.EarlyLeaveTimeDiff.Value.TotalMinutes;
                }
                if (lateHour.WorkAtHome != null)
                {
                    totalLateMin += (8 * 60);
                }
            }
            if (totalLateMin > 0)
            {
                if (Math.Truncate(totalLateMin / (8 * 60)) > 0)
                {
                    result += Math.Truncate(totalLateMin / (8 * 60)).ToString() + (Math.Truncate(totalLateMin / (8 * 60)) > 1 ? " Days " : " Day ");
                    totalLateMin = (totalLateMin % (8 * 60));
                }
                if (Math.Truncate(totalLateMin / 60) > 0)
                {
                    result += Math.Truncate(totalLateMin / 60).ToString() + (Math.Truncate(totalLateMin / 60) > 1 ? " Hrs " : " Hr ");
                    totalLateMin = (totalLateMin % 60);
                }
                if (totalLateMin > 0)
                {
                    result += (totalLateMin).ToString() + " Min";
                }
                if (withHtmlTag)
                {
                    result = "<span>" + result + "</span>";
                }
                else { result = "(" + result + ")"; }
            }
            return result;
        }

        public ActionResult DownloadLatehourExcel()
        {
            try
            {
                var filter = TempData.Get<SpacialSearch>("filter");
                var expr = PredicateBuilder.True<LateHour>();
                DateTime dateStart, dateEnd;
                if (filter.search_text > 0)
                {
                    expr = expr.And(x => x.LeaveType == filter.search_text);
                }
                if (filter.EmployeeId > 0)
                {
                    expr = expr.And(x => x.UserLogin1.Uid == filter.EmployeeId);
                }

                if (!string.IsNullOrEmpty(filter.dateFrom) && DateTime.TryParseExact(filter.dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateStart))
                {
                    expr = expr.And(l => l.DayOfDate >= dateStart);
                }
                if (!string.IsNullOrEmpty(filter.dateTo) && DateTime.TryParseExact(filter.dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateEnd))
                {
                    expr = expr.And(l => l.DayOfDate <= dateEnd);
                }
                var model = leaveService.GetLateHourList(expr);
                List<LateHourReportViewModel> leavereport = new List<LateHourReportViewModel>();
                foreach (var item in model)
                {
                    leavereport.Add(new LateHourReportViewModel()
                    {
                        EmpName = item.UserLogin1.Name,
                        StartDate = item.DayOfDate.ToString("ddd, MMM dd, yyyy"),
                        LeaveType = item.LeaveType == 3 ? "Work From Home" : item.LeaveType == 2 ? "UnAuthorized" :
                        item.LateStartTimeDiff != null ? "Late Hour (" + (item.LateStartTimeDiff.Value.Hours > 0 ? item.LateStartTimeDiff.Value.Hours.ToString() + " hr " : "") + (item.LateStartTimeDiff.Value.Minutes > 0 ? item.LateStartTimeDiff.Value.Minutes.ToString() + " min" : "") + ")"
                        + (item.EarlyLeaveTimeDiff != null ? "Early Leave (" + (item.EarlyLeaveTimeDiff.Value.Hours > 0 ? item.EarlyLeaveTimeDiff.Value.Hours.ToString() + " hr " : "")
                        + (item.EarlyLeaveTimeDiff.Value.Minutes > 0 ? item.EarlyLeaveTimeDiff.Value.Minutes.ToString() + " min" : "") + ")" : "") : (item.EarlyLeaveTimeDiff != null ? "Early Leave (" + (item.EarlyLeaveTimeDiff.Value.Hours > 0 ? item.EarlyLeaveTimeDiff.Value.Hours.ToString() + " hr " : "")
                        + (item.EarlyLeaveTimeDiff.Value.Minutes > 0 ? item.EarlyLeaveTimeDiff.Value.Minutes.ToString() + " min" : "") + ")" : ""),
                        Reason = item.LeaveType == 3 ? item.WorkFromHome : item.LeaveType == 2 ? item.WorkAtHome : item.LateReason != null ? "Late Reason(" + item.LateReason + ")" + (item.EarlyReason != null ? ", Early Reason (" + item.EarlyReason + ")" : " ") : item.EarlyReason != null ? "Early Reason(" + item.EarlyReason + ")" : " "
                    });
                }

                string TotalLateTime = String.Empty;
                if (filter.EmployeeId != null && filter.EmployeeId > 0)
                {
                    TotalLateTime = CalculateTotalLateTime(model, false);
                }

                if (leavereport.Count > 0)
                {
                    string filename = "LeaveReport_" + DateTime.Now.Ticks.ToString() + ".xlsx";
                    string[] columns = { "EmpName", "StartDate", "LeaveType", "Reason" };
                    byte[] filecontent = ExportExcelHelper.ExportExcel(leavereport, "Late Hour Report " + TotalLateTime, true, columns);
                    string fileName = filename;
                    return File(filecontent, ExportExcelHelper.ExcelContentType, fileName);
                }
            }
            catch (Exception e)
            {
            }
            return CreateModelStateErrors();
        }

        public ActionResult SendAppointmentEmail()
        {
            //Create OUTLOOK Calendar event
            string filepath = CreateOutlookCalendar.CreateEventICSFile(
                                CurrentUser.Name,
                                CurrentUser.EmailOffice,
                                "Test " + CurrentUser.Name,
                                "Jaipur",
                                DateTime.Now.Date.AddDays(10).AddHours(10),
                                DateTime.Now.Date.AddDays(10).AddHours(11.00),
                                "Test");
            #region
            FlexiMail objSendMail = new FlexiMail();

            string fromemail = "hiteshkumar.aylani@dotsquares.com";//CurrentUser.EmailOffice;

            objSendMail.From = fromemail;
            objSendMail.To = fromemail;// "naveen.khandelwal@dotsquares.com";
            objSendMail.CC = "";
            objSendMail.BCC = "";
            objSendMail.Subject = "Email Test";

            objSendMail.MailBodyManualSupply = true;
            objSendMail.MailBody = "test";
            string eventString = System.IO.File.ReadAllText(filepath);
            objSendMail.SendCalendarEvent(eventString);

            #endregion

            return File(filepath, "text/calendar", "event.ics");
        }

        [HttpPost]
        public ActionResult GetUserLeaves(int uid, bool edit, int id)
        {
            if (uid == 0)
            {
                uid = CurrentUser.Uid;
            }
            double sickLeave = leaveService.GetAllSickLeavesForYear(uid, (int)LeaveCategory.SickLeave);
            var leaveActivity = leaveService.GetLeaveActivityById(id);
            CurrentLeaveDto dto = new CurrentLeaveDto();
            //var leaveData = GetCurrentLeaveBalance(GetAllLeaveBalance(uid), GetLeave(uid));
            //var leaveData = edit ? GetCurrentLeaveBalance(GetAllLeaveBalance(uid), GetLeaveBalance(uid)) : GetCurrentLeaveBalance(GetAllLeaveBalance(uid), GetCurrentMonthLeaveBalance(GetLeaveBalance(uid), GetPendingLeave(uid)));
            var leaveData = edit ? GetCurrentLeaveBalance(GetAllLeaveBalance(uid), GetPendingLeaves(uid), (leaveActivity != null && leaveActivity.Status.HasValue ? leaveActivity.Status.Value : 0), edit, isPreviousMonth)
                : GetCurrentLeaveBalanceHidden(GetAllLeaveBalance(uid), GetCurrentMonthLeaveBalance(GetLeaveBalance(uid), GetPendingLeave(uid)), edit, isPreviousMonth);
            //var leaveData = GetCurrentLeaveBalance(GetAllLeaveBalance(uid), GetLeaveBalance(uid));
            dto.CasualLeave = leaveData.CasualLeave;
            dto.LossPayLeave = leaveData.LossPayLeave;
            dto.CompensatoryOff = leaveData.CompensatoryOff;
            dto.PaternityLeave = leaveData.PaternityLeave;
            //dto.SickLeave = sickLeave >= 3 ? 0 :
            //    (sickLeave < 3 && sickLeave > 0 ? (leaveData.SickLeave > 0 && leaveData.SickLeave <= 3 ? leaveData.SickLeave : 0)
            //    : leaveData.SickLeave - sickLeave);
            //double sl = 0;
            //if (sickLeave >= 3)
            //{
            //    sl = 0;
            //}
            //else if (sickLeave >= 0 && sickLeave < 3)
            //{
            //    if (leaveData.SickLeave >= 3)
            //    {
            //        sl = 3 - sickLeave;
            //    }
            //    else if (leaveData.SickLeave > 0 && leaveData.SickLeave < 3)
            //    {
            //        sl = leaveData.SickLeave.Value;
            //    }
            //    else
            //    {
            //        sl = 0;
            //    }
            //}
            //else
            //{
            //    sl = 0;
            //}
            dto.SickLeave = leaveData.SickLeave;
            dto.MaternityLeave = leaveData.MaternityLeave;
            dto.EarnedLeave = leaveData.EarnedLeave;
            dto.BereavementLeave = leaveData.BereavementLeave;
            dto.WeddingLeave = leaveData.WeddingLeave;
            //dto.LoyaltyLeave = leaveData.LoyaltyLeave;

            if (edit && leaveActivity != null && leaveActivity.LeaveCategory.HasValue)
            {
                var totalDays = leaveActivity.IsHalf == true ? ((leaveActivity.EndDate - leaveActivity.StartDate).TotalDays + 1) * 0.5 : (leaveActivity.EndDate - leaveActivity.StartDate).TotalDays + 1;
                switch (leaveActivity.LeaveCategory.Value)
                {
                    case (int)LeaveCategory.CompensatoryOff:
                        dto.CompensatoryOff = dto.CompensatoryOff + totalDays;
                        break;
                    case (int)LeaveCategory.CasualLeave:
                        dto.CasualLeave = dto.CasualLeave + totalDays;
                        break;
                    case (int)LeaveCategory.PaternityLeave:
                        dto.PaternityLeave = dto.PaternityLeave + totalDays;
                        break;
                    case (int)LeaveCategory.MaternityLeave:
                        dto.MaternityLeave = dto.MaternityLeave + totalDays;
                        break;
                    case (int)LeaveCategory.EarnedLeave:
                        dto.EarnedLeave = dto.EarnedLeave + totalDays;
                        break;
                    case (int)LeaveCategory.SickLeave:
                        dto.SickLeave = dto.SickLeave + totalDays;
                        break;
                    case (int)LeaveCategory.BereavementLeave:
                        dto.BereavementLeave = dto.BereavementLeave + totalDays;
                        break;
                    case (int)LeaveCategory.WeddingLeave:
                        dto.WeddingLeave = dto.WeddingLeave + totalDays;
                        break;
                    case (int)LeaveCategory.UnpaidLeave:
                        dto.LossPayLeave = dto.LossPayLeave + totalDays;
                        break;

                }
            }

            return Json(dto);
        }

        #region Leave Balance Calculation
        public ApprovedLeaveDto GetLeave(int Uid)
        {
            ApprovedLeaveDto dto = new ApprovedLeaveDto();
            try
            {
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                dto.CasualLeave = leaveService.GetLeaves(Uid, (int)LeaveCategory.CasualLeave, startDate, endDate);
                dto.EarnedLeave = leaveService.GetLeaves(Uid, (int)LeaveCategory.EarnedLeave, startDate, endDate);
                dto.LossPayLeave = leaveService.GetLeaves(Uid, (int)LeaveCategory.UnpaidLeave, startDate, endDate);
                dto.SickLeave = leaveService.GetLeaves(Uid, (int)LeaveCategory.SickLeave, startDate, endDate);
                dto.BereavementLeave = leaveService.GetLeaves(Uid, (int)LeaveCategory.BereavementLeave, startDate, endDate);
                dto.WeddingLeave = leaveService.GetLeaves(Uid, (int)LeaveCategory.WeddingLeave, startDate, endDate);
                dto.CompensatoryOff = leaveService.GetLeaves(Uid, (int)LeaveCategory.CompensatoryOff, startDate, endDate);
                //dto.LoyaltyLeave = leaveService.GetLeaves(Uid, (int)LeaveCategory.LoyaltyLeave, startDate, endDate);
                dto.MaternityLeave = leaveService.GetLeaves(Uid, (int)LeaveCategory.MaternityLeave, startDate, endDate);
                dto.PaternityLeave = leaveService.GetLeaves(Uid, (int)LeaveCategory.PaternityLeave, startDate, endDate);

                return dto;
            }
            catch (Exception ex)
            {
                return dto;
            }
        }
        public ApprovedLeaveDto GetLeaveBalance(int uid = 0)
        {
            ApprovedLeaveDto dto = new ApprovedLeaveDto();
            try
            {
                int id = uid;
                if (uid == 0)
                {
                    id = CurrentUser.Uid;
                }
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                dto.CasualLeave = leaveService.GetApprovedLeaves(id, (int)Enums.LeaveCategory.CasualLeave, startDate, endDate, isPreviousMonth);
                dto.EarnedLeave = leaveService.GetApprovedLeaves(id, (int)Enums.LeaveCategory.EarnedLeave, startDate, endDate, isPreviousMonth);
                dto.LossPayLeave = leaveService.GetApprovedLeaves(id, (int)Enums.LeaveCategory.UnpaidLeave, startDate, endDate, isPreviousMonth);
                dto.SickLeave = leaveService.GetApprovedLeaves(id, (int)Enums.LeaveCategory.SickLeave, startDate, endDate, isPreviousMonth);
                dto.BereavementLeave = leaveService.GetApprovedLeaves(id, (int)Enums.LeaveCategory.BereavementLeave, startDate, endDate, isPreviousMonth);
                dto.WeddingLeave = leaveService.GetApprovedLeaves(id, (int)Enums.LeaveCategory.WeddingLeave, startDate, endDate, isPreviousMonth);
                dto.CompensatoryOff = leaveService.GetApprovedLeaves(id, (int)Enums.LeaveCategory.CompensatoryOff, startDate, endDate, isPreviousMonth);
                //dto.LoyaltyLeave = leaveService.GetApprovedLeaves(id,(int)Enums.LeaveCategory.LoyaltyLeave, startDate,endDate);
                dto.MaternityLeave = leaveService.GetApprovedLeaves(id, (int)Enums.LeaveCategory.MaternityLeave, startDate, endDate, isPreviousMonth);
                dto.PaternityLeave = leaveService.GetApprovedLeaves(id, (int)Enums.LeaveCategory.PaternityLeave, startDate, endDate, isPreviousMonth);

                return dto;
            }
            catch (Exception ex)
            {
                return dto;
            }
        }
        public ApprovedLeaveDto GetAllApprovedLeave(int uid = 0)
        {
            ApprovedLeaveDto dto = new ApprovedLeaveDto();
            try
            {
                int id = uid;
                if (uid == 0)
                {
                    id = CurrentUser.Uid;
                }
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                dto.CasualLeave = leaveService.GetAllApprovedLeaves(id, (int)LeaveCategory.CasualLeave);
                dto.EarnedLeave = leaveService.GetAllApprovedLeaves(id, (int)LeaveCategory.EarnedLeave);
                dto.LossPayLeave = leaveService.GetAllApprovedLeaves(id, (int)LeaveCategory.UnpaidLeave);
                dto.SickLeave = leaveService.GetAllApprovedLeaves(id, (int)LeaveCategory.SickLeave);
                dto.BereavementLeave = leaveService.GetAllApprovedLeaves(id, (int)LeaveCategory.BereavementLeave);
                dto.WeddingLeave = leaveService.GetAllApprovedLeaves(id, (int)LeaveCategory.WeddingLeave);
                dto.CompensatoryOff = leaveService.GetAllApprovedLeaves(id, (int)LeaveCategory.CompensatoryOff);
                //dto.LoyaltyLeave = leaveService.GetAllApprovedLeaves(id, (int)LeaveCategory.LoyaltyLeave);
                dto.MaternityLeave = leaveService.GetAllApprovedLeaves(id, (int)LeaveCategory.MaternityLeave);
                dto.PaternityLeave = leaveService.GetAllApprovedLeaves(id, (int)LeaveCategory.PaternityLeave);

                return dto;
            }
            catch (Exception ex)
            {
                return dto;
            }
        }
        public CurrentLeaveDto GetPendingLeave(int Uid)
        {
            CurrentLeaveDto dto = new CurrentLeaveDto();
            try
            {
                dto.CasualLeave = leaveService.GetPendingLeaves(Uid, (int)LeaveCategory.CasualLeave);
                dto.EarnedLeave = leaveService.GetPendingLeaves(Uid, (int)LeaveCategory.EarnedLeave);
                dto.LossPayLeave = leaveService.GetPendingLeaves(Uid, (int)LeaveCategory.UnpaidLeave);
                dto.SickLeave = leaveService.GetPendingLeaves(Uid, (int)LeaveCategory.SickLeave);
                dto.BereavementLeave = leaveService.GetPendingLeaves(Uid, (int)LeaveCategory.BereavementLeave);
                dto.WeddingLeave = leaveService.GetPendingLeaves(Uid, (int)LeaveCategory.WeddingLeave);
                dto.CompensatoryOff = leaveService.GetPendingLeaves(Uid, (int)LeaveCategory.CompensatoryOff);
                //dto.LoyaltyLeave = leaveService.GetPendingLeaves(Uid, (int)LeaveCategory.LoyaltyLeave);
                dto.MaternityLeave = leaveService.GetPendingLeaves(Uid, (int)LeaveCategory.MaternityLeave);
                dto.PaternityLeave = leaveService.GetPendingLeaves(Uid, (int)LeaveCategory.PaternityLeave);

                return dto;
            }
            catch (Exception ex)
            {
                return dto;
            }
        }

        public ApprovedLeaveDto GetPendingLeaves(int Uid)
        {
            ApprovedLeaveDto dto = new ApprovedLeaveDto();
            try
            {
                dto.CasualLeave = leaveService.GetPendingLeaves(Uid, (int)LeaveCategory.CasualLeave, isPreviousMonth);
                dto.EarnedLeave = leaveService.GetPendingLeaves(Uid, (int)LeaveCategory.EarnedLeave, isPreviousMonth);
                dto.LossPayLeave = leaveService.GetPendingLeaves(Uid, (int)LeaveCategory.UnpaidLeave, isPreviousMonth);
                dto.SickLeave = leaveService.GetPendingLeaves(Uid, (int)LeaveCategory.SickLeave, isPreviousMonth);
                dto.BereavementLeave = leaveService.GetPendingLeaves(Uid, (int)LeaveCategory.BereavementLeave, isPreviousMonth);
                dto.WeddingLeave = leaveService.GetPendingLeaves(Uid, (int)LeaveCategory.WeddingLeave, isPreviousMonth);
                dto.CompensatoryOff = leaveService.GetPendingLeaves(Uid, (int)LeaveCategory.CompensatoryOff, isPreviousMonth);
                //dto.LoyaltyLeave = leaveService.GetPendingLeaves(Uid, (int)LeaveCategory.LoyaltyLeave);
                dto.MaternityLeave = leaveService.GetPendingLeaves(Uid, (int)LeaveCategory.MaternityLeave, isPreviousMonth);
                dto.PaternityLeave = leaveService.GetPendingLeaves(Uid, (int)LeaveCategory.PaternityLeave, isPreviousMonth);

                return dto;
            }
            catch (Exception ex)
            {
                return dto;
            }
        }
        public LeaveTypesDto GetAllLeaveBalance(int uid = 0)
        {
            LeaveTypesDto dto = new LeaveTypesDto();
            try
            {
                var empInfo = userLoginService.GetUserInfoByID(uid);
                int? empAttendanceId = empInfo != null ? empInfo.AttendenceId.HasValue ? empInfo.AttendenceId.Value : 0 : 0;
                if (empAttendanceId.Value == 0)
                {
                    if (empInfo != null)
                    {
                        var saralUserInfo = levDetailsService.GetEmployeeInfo(empInfo.EmailOffice);
                        empAttendanceId = saralUserInfo != null ? saralUserInfo.Empid > 0 ? saralUserInfo.Empid : 0 : 0;
                        if (empAttendanceId == 0)
                        {
                            var saralDTUserInfo = levDetailsDTService.GetDTEmployeeInfo(empInfo.EmailOffice);
                            empAttendanceId = saralDTUserInfo != null ? saralDTUserInfo.Empid > 0 ? saralDTUserInfo.Empid : 0 : 0;
                        }
                    }
                }
                if (empAttendanceId.Value > 0)
                {
                    var monthYearValue = (DateTime.Now.Year * 12) + DateTime.Now.Month;
                    DataTable leaveBalance = null;
                    List<LeaveBalanceDetailsDto> leaveBalanceList = new List<LeaveBalanceDetailsDto>();

                    leaveBalance = levDetailsService.GetLeaveBalance(empAttendanceId, monthYearValue);
                    if (leaveBalance.Rows.Count > 0 == false)
                    {
                        isPreviousMonth = true;
                        var years = DateTime.Now.Month != 1 ? DateTime.Now.Year : DateTime.Now.AddYears(-1).Year;
                        monthYearValue = (years * 12) + DateTime.Now.AddMonths(-1).Month;
                        leaveBalance = levDetailsService.GetLeaveBalance(empAttendanceId, monthYearValue);
                    }

                    leaveBalanceList = DtoBinder(leaveBalance);
                    monthYearValue = (DateTime.Now.Year * 12) + DateTime.Now.Month;
                    if (leaveBalanceList.Count() == 0)
                    {
                        isPreviousMonth = false;
                        leaveBalance = levDetailsDTService.GetDTLeaveBalance(empAttendanceId, monthYearValue);
                        if (leaveBalance.Rows.Count > 0 == false)
                        {
                            isPreviousMonth = true;
                            var years = DateTime.Now.Month != 1 ? DateTime.Now.Year : DateTime.Now.AddYears(-1).Year;
                            monthYearValue = (years * 12) + DateTime.Now.AddMonths(-1).Month;
                            leaveBalance = levDetailsDTService.GetDTLeaveBalance(empAttendanceId, monthYearValue);
                        }
                        leaveBalanceList = DtoBinder(leaveBalance);
                    }

                    foreach (var item in leaveBalanceList)
                    {
                        switch (item.LeaveName)
                        {
                            case "Loss Of Pay":
                                dto.LossPayLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.UnpaidLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
                                dto.LossPayLeave_AB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.UnpaidLeave.GetEnumDisplayName())).Select(x => x.Allotted).FirstOrDefault());
                                dto.LossPayLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.UnpaidLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
                                dto.LossPayLeave_OB = isPreviousMonth ? dto.LossPayLeave_CB : dto.LossPayLeave_OB + dto.LossPayLeave_AB;
                                break;
                            case "CL":
                                dto.CasualLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.CasualLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
                                dto.CasualLeave_AB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.CasualLeave.GetEnumDisplayName())).Select(x => x.Allotted).FirstOrDefault());
                                dto.CasualLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.CasualLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
                                dto.CasualLeave_OB = isPreviousMonth ? dto.CasualLeave_CB : dto.CasualLeave_OB + dto.CasualLeave_AB;
                                break;
                            case "Compensatory Off":
                                dto.CompensatoryOff_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.CompensatoryOff.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
                                dto.CompensatoryOff_AB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.CompensatoryOff.GetEnumDisplayName())).Select(x => x.Allotted).FirstOrDefault());
                                dto.CompensatoryOff_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.CompensatoryOff.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
                                dto.CompensatoryOff_OB = isPreviousMonth ? dto.CompensatoryOff_CB : dto.CompensatoryOff_OB + dto.CompensatoryOff_AB;
                                break;
                            case "Paternity Leave":
                                dto.PaternityLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.PaternityLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
                                dto.PaternityLeave_AB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.PaternityLeave.GetEnumDisplayName())).Select(x => x.Allotted).FirstOrDefault());
                                dto.PaternityLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.PaternityLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
                                dto.PaternityLeave_OB = isPreviousMonth ? dto.PaternityLeave_CB : dto.PaternityLeave_OB + dto.PaternityLeave_AB;
                                break;
                            case "Sick Leave":
                                dto.SickLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.SickLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
                                dto.SickLeave_AB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.SickLeave.GetEnumDisplayName())).Select(x => x.Allotted).FirstOrDefault());
                                dto.SickLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.SickLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
                                dto.SickLeave_OB = isPreviousMonth ? dto.SickLeave_CB : dto.SickLeave_OB + dto.SickLeave_AB;
                                break;
                            case "Maternity Leave":
                                dto.MaternityLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.MaternityLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
                                dto.MaternityLeave_AB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.MaternityLeave.GetEnumDisplayName())).Select(x => x.Allotted).FirstOrDefault());
                                dto.MaternityLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.MaternityLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
                                dto.MaternityLeave_OB = isPreviousMonth ? dto.MaternityLeave_CB : dto.MaternityLeave_OB + dto.MaternityLeave_AB;
                                break;
                            case "Earned Leave":
                                dto.EarnedLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.EarnedLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
                                dto.EarnedLeave_AB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.EarnedLeave.GetEnumDisplayName())).Select(x => x.Allotted).FirstOrDefault());
                                dto.EarnedLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.EarnedLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
                                dto.EarnedLeave_OB = isPreviousMonth ? dto.EarnedLeave_CB : dto.EarnedLeave_OB + dto.EarnedLeave_AB;
                                break;
                            case "Bereavement Leave":
                                dto.BereavementLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.BereavementLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
                                dto.BereavementLeave_AB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.BereavementLeave.GetEnumDisplayName())).Select(x => x.Allotted).FirstOrDefault());
                                dto.BereavementLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.BereavementLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
                                dto.BereavementLeave_OB = isPreviousMonth ? dto.BereavementLeave_CB : dto.BereavementLeave_OB + dto.BereavementLeave_AB;
                                break;
                            case "Wedding Leave":
                                dto.WeddingLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.WeddingLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
                                dto.WeddingLeave_AB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.WeddingLeave.GetEnumDisplayName())).Select(x => x.Allotted).FirstOrDefault());
                                dto.WeddingLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.WeddingLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
                                dto.WeddingLeave_OB = isPreviousMonth ? dto.WeddingLeave_CB : dto.WeddingLeave_OB + dto.WeddingLeave_AB;
                                break;
                                //case "Loyalty Leave":
                                //    dto.LoyaltyLeave_OB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.LoyaltyLeave.GetEnumDisplayName())).Select(x => x.OpeningBalance).FirstOrDefault());
                                //    dto.LoyaltyLeave_CB = Convert.ToDouble(leaveBalanceList.Where(x => x.LeaveName.Contains(LeaveCategory.LoyaltyLeave.GetEnumDisplayName())).Select(x => x.ClosingBalance).FirstOrDefault());
                                //    break;
                        }
                    }

                }
                return dto;
            }
            catch (Exception ex)
            {
                return dto;
            }
        }
        public ApprovedLeaveDto GetCurrentMonthLeaveBalance(ApprovedLeaveDto leaveTypes, CurrentLeaveDto approvedLeave)
        {
            ApprovedLeaveDto dto = new ApprovedLeaveDto();
            try
            {
                //if (isPreviousMonth)
                //{
                dto.CasualLeave = Convert.ToDouble(leaveTypes.CasualLeave);
                dto.LossPayLeave = Convert.ToDouble(leaveTypes.LossPayLeave);
                dto.CompensatoryOff = Convert.ToDouble(leaveTypes.CompensatoryOff);
                dto.PaternityLeave = Convert.ToDouble(leaveTypes.PaternityLeave);
                dto.SickLeave = Convert.ToDouble(leaveTypes.SickLeave);
                dto.MaternityLeave = Convert.ToDouble(leaveTypes.MaternityLeave);
                dto.EarnedLeave = Convert.ToDouble(leaveTypes.EarnedLeave);
                dto.BereavementLeave = Convert.ToDouble(leaveTypes.BereavementLeave);
                dto.WeddingLeave = Convert.ToDouble(leaveTypes.WeddingLeave);
                //dto.LoyaltyLeave = Convert.ToDouble(leaveTypes.LoyaltyLeave) + approvedLeave.LoyaltyLeave;
                //}
                //else
                //{
                //    dto.CasualLeave = Convert.ToDouble(leaveTypes.CasualLeave) + approvedLeave.CasualLeave;
                //    dto.LossPayLeave = Convert.ToDouble(leaveTypes.LossPayLeave) + approvedLeave.LossPayLeave;
                //    dto.CompensatoryOff = Convert.ToDouble(leaveTypes.CompensatoryOff) + approvedLeave.CompensatoryOff;
                //    dto.PaternityLeave = Convert.ToDouble(leaveTypes.PaternityLeave) + approvedLeave.PaternityLeave;
                //    dto.SickLeave = Convert.ToDouble(leaveTypes.SickLeave) + approvedLeave.SickLeave;
                //    dto.MaternityLeave = Convert.ToDouble(leaveTypes.MaternityLeave) + approvedLeave.MaternityLeave;
                //    dto.EarnedLeave = Convert.ToDouble(leaveTypes.EarnedLeave) + approvedLeave.EarnedLeave;
                //    dto.BereavementLeave = Convert.ToDouble(leaveTypes.BereavementLeave) + approvedLeave.BereavementLeave;
                //    dto.WeddingLeave = Convert.ToDouble(leaveTypes.WeddingLeave) + approvedLeave.WeddingLeave;
                //}
                return dto;
            }
            catch (Exception ex)
            {
                return dto;
            }
        }

        public ApprovedLeaveDto GetCurrentMonthLeaveBalanceHidden(ApprovedLeaveDto leaveTypes, CurrentLeaveDto approvedLeave)
        {
            ApprovedLeaveDto dto = new ApprovedLeaveDto();
            try
            {
                if (isPreviousMonth)
                {
                    dto.CasualLeave = Convert.ToDouble(leaveTypes.CasualLeave);
                    dto.LossPayLeave = Convert.ToDouble(leaveTypes.LossPayLeave);
                    dto.CompensatoryOff = Convert.ToDouble(leaveTypes.CompensatoryOff);
                    dto.PaternityLeave = Convert.ToDouble(leaveTypes.PaternityLeave);
                    dto.SickLeave = Convert.ToDouble(leaveTypes.SickLeave);
                    dto.MaternityLeave = Convert.ToDouble(leaveTypes.MaternityLeave);
                    dto.EarnedLeave = Convert.ToDouble(leaveTypes.EarnedLeave);
                    dto.BereavementLeave = Convert.ToDouble(leaveTypes.BereavementLeave);
                    dto.WeddingLeave = Convert.ToDouble(leaveTypes.WeddingLeave);
                    dto.LoyaltyLeave = Convert.ToDouble(leaveTypes.LoyaltyLeave) + approvedLeave.LoyaltyLeave;
                }
                else
                {
                    dto.CasualLeave = Convert.ToDouble(leaveTypes.CasualLeave) - approvedLeave.CasualLeave;
                    dto.LossPayLeave = Convert.ToDouble(leaveTypes.LossPayLeave) - approvedLeave.LossPayLeave;
                    dto.CompensatoryOff = Convert.ToDouble(leaveTypes.CompensatoryOff) - approvedLeave.CompensatoryOff;
                    dto.PaternityLeave = Convert.ToDouble(leaveTypes.PaternityLeave) - approvedLeave.PaternityLeave;
                    dto.SickLeave = Convert.ToDouble(leaveTypes.SickLeave) - approvedLeave.SickLeave;
                    dto.MaternityLeave = Convert.ToDouble(leaveTypes.MaternityLeave) - approvedLeave.MaternityLeave;
                    dto.EarnedLeave = Convert.ToDouble(leaveTypes.EarnedLeave) - approvedLeave.EarnedLeave;
                    dto.BereavementLeave = Convert.ToDouble(leaveTypes.BereavementLeave) - approvedLeave.BereavementLeave;
                    dto.WeddingLeave = Convert.ToDouble(leaveTypes.WeddingLeave) - approvedLeave.WeddingLeave;
                }
                return dto;
            }
            catch (Exception ex)
            {
                return dto;
            }
        }

        public CurrentLeaveDto GetCurrentLeaveBalance(LeaveTypesDto leaveTypes, ApprovedLeaveDto approvedLeave, int status, bool isEdit = false, bool isPrev = false)
        {
            CurrentLeaveDto dto = new CurrentLeaveDto();
            try
            {
                if (isEdit)
                {
                    if (isPrev)
                    {
                        dto.CasualLeave = Convert.ToDouble(leaveTypes.CasualLeave_CB) - (status == (int)LeaveStatus.Pending ? 0 : approvedLeave.CasualLeave);
                        dto.LossPayLeave = Convert.ToDouble(leaveTypes.LossPayLeave_CB) - (status == (int)LeaveStatus.Pending ? 0 : approvedLeave.LossPayLeave);
                        dto.CompensatoryOff = Convert.ToDouble(leaveTypes.CompensatoryOff_CB) - (status == (int)LeaveStatus.Pending ? 0 : approvedLeave.CompensatoryOff);
                        dto.PaternityLeave = Convert.ToDouble(leaveTypes.PaternityLeave_CB) - (status == (int)LeaveStatus.Pending ? 0 : approvedLeave.PaternityLeave);
                        dto.SickLeave = Convert.ToDouble(leaveTypes.SickLeave_CB) - (status == (int)LeaveStatus.Pending ? 0 : approvedLeave.SickLeave);
                        dto.MaternityLeave = Convert.ToDouble(leaveTypes.MaternityLeave_CB) - (status == (int)LeaveStatus.Pending ? 0 : approvedLeave.MaternityLeave);
                        dto.EarnedLeave = Convert.ToDouble(leaveTypes.EarnedLeave_CB) - (status == (int)LeaveStatus.Pending ? 0 : approvedLeave.EarnedLeave);
                        dto.BereavementLeave = Convert.ToDouble(leaveTypes.BereavementLeave_CB) - (status == (int)LeaveStatus.Pending ? 0 : approvedLeave.BereavementLeave);
                        dto.WeddingLeave = Convert.ToDouble(leaveTypes.WeddingLeave_CB) - (status == (int)LeaveStatus.Pending ? 0 : approvedLeave.WeddingLeave);
                        //dto.LoyaltyLeave = Convert.ToDouble(leaveTypes.LoyaltyLeave_CB) + approvedLeave.LoyaltyLeave;
                    }
                    else
                    {

                        dto.CasualLeave = Convert.ToDouble(leaveTypes.CasualLeave_CB);// + approvedLeave.CasualLeave;
                        dto.LossPayLeave = Convert.ToDouble(leaveTypes.LossPayLeave_CB);// + approvedLeave.LossPayLeave;
                        dto.CompensatoryOff = Convert.ToDouble(leaveTypes.CompensatoryOff_CB);// + approvedLeave.CompensatoryOff;
                        dto.PaternityLeave = Convert.ToDouble(leaveTypes.PaternityLeave_CB);// + approvedLeave.PaternityLeave;
                        dto.SickLeave = Convert.ToDouble(leaveTypes.SickLeave_CB);// + approvedLeave.SickLeave;
                        dto.MaternityLeave = Convert.ToDouble(leaveTypes.MaternityLeave_CB);// + approvedLeave.MaternityLeave;
                        dto.EarnedLeave = Convert.ToDouble(leaveTypes.EarnedLeave_CB);// + approvedLeave.EarnedLeave;
                        dto.BereavementLeave = Convert.ToDouble(leaveTypes.BereavementLeave_CB);// + approvedLeave.BereavementLeave;
                        dto.WeddingLeave = Convert.ToDouble(leaveTypes.WeddingLeave_CB);// + approvedLeave.WeddingLeave;
                        //dto.LoyaltyLeave = Convert.ToDouble(leaveTypes.LoyaltyLeave_CB) + approvedLeave.LoyaltyLeave;
                    }

                }
                else
                {
                    dto.CasualLeave = Convert.ToDouble(leaveTypes.CasualLeave_CB)/* - approvedLeave.CasualLeave;
                    dto.LossPayLeave = Convert.ToDouble(leaveTypes.LossPayLeave_CB)/* - approvedLeave.LossPayLeave*/;
                    dto.CompensatoryOff = Convert.ToDouble(leaveTypes.CompensatoryOff_CB)/* - approvedLeave.CompensatoryOff*/;
                    dto.PaternityLeave = Convert.ToDouble(leaveTypes.PaternityLeave_CB)/* - approvedLeave.PaternityLeave*/;
                    dto.SickLeave = Convert.ToDouble(leaveTypes.SickLeave_CB)/* - approvedLeave.SickLeave*/;
                    dto.MaternityLeave = Convert.ToDouble(leaveTypes.MaternityLeave_CB)/* - approvedLeave.MaternityLeave*/;
                    dto.EarnedLeave = Convert.ToDouble(leaveTypes.EarnedLeave_CB)/* - approvedLeave.EarnedLeave*/;
                    dto.BereavementLeave = Convert.ToDouble(leaveTypes.BereavementLeave_CB)/* - approvedLeave.BereavementLeave*/;
                    dto.WeddingLeave = Convert.ToDouble(leaveTypes.WeddingLeave_CB)/* - approvedLeave.WeddingLeave*/;
                    //dto.LoyaltyLeave = Convert.ToDouble(leaveTypes.LoyaltyLeave_CB) - approvedLeave.LoyaltyLeave;
                }
                return dto;
            }
            catch (Exception ex)
            {
                return dto;
            }
        }

        public CurrentLeaveDto GetCurrentLeaveBalanceHidden(LeaveTypesDto leaveTypes, ApprovedLeaveDto approvedLeave, bool isEdit = false, bool isPrev = false)
        {
            CurrentLeaveDto dto = new CurrentLeaveDto();
            try
            {
                if (isPrev)
                {
                    dto.CasualLeave = Convert.ToDouble(leaveTypes.CasualLeave_CB) - approvedLeave.CasualLeave;
                    dto.LossPayLeave = Convert.ToDouble(leaveTypes.LossPayLeave_CB) - approvedLeave.LossPayLeave;
                    dto.CompensatoryOff = Convert.ToDouble(leaveTypes.CompensatoryOff_CB) - approvedLeave.CompensatoryOff;
                    dto.PaternityLeave = Convert.ToDouble(leaveTypes.PaternityLeave_CB) - approvedLeave.PaternityLeave;
                    dto.SickLeave = Convert.ToDouble(leaveTypes.SickLeave_CB) - approvedLeave.SickLeave;
                    dto.MaternityLeave = Convert.ToDouble(leaveTypes.MaternityLeave_CB) - approvedLeave.MaternityLeave;
                    dto.EarnedLeave = Convert.ToDouble(leaveTypes.EarnedLeave_CB) - approvedLeave.EarnedLeave;
                    dto.BereavementLeave = Convert.ToDouble(leaveTypes.BereavementLeave_CB) - approvedLeave.BereavementLeave;
                    dto.WeddingLeave = Convert.ToDouble(leaveTypes.WeddingLeave_CB) - approvedLeave.WeddingLeave;
                    dto.LoyaltyLeave = Convert.ToDouble(leaveTypes.LoyaltyLeave_CB) - approvedLeave.LoyaltyLeave;
                }
                else
                {
                    dto.CasualLeave = Convert.ToDouble(leaveTypes.CasualLeave_OB) - approvedLeave.CasualLeave;
                    dto.LossPayLeave = Convert.ToDouble(leaveTypes.LossPayLeave_OB) - approvedLeave.LossPayLeave;
                    dto.CompensatoryOff = Convert.ToDouble(leaveTypes.CompensatoryOff_OB) - approvedLeave.CompensatoryOff;
                    dto.PaternityLeave = Convert.ToDouble(leaveTypes.PaternityLeave_OB) - approvedLeave.PaternityLeave;
                    dto.SickLeave = Convert.ToDouble(leaveTypes.SickLeave_OB) - approvedLeave.SickLeave;
                    dto.MaternityLeave = Convert.ToDouble(leaveTypes.MaternityLeave_OB) - approvedLeave.MaternityLeave;
                    dto.EarnedLeave = Convert.ToDouble(leaveTypes.EarnedLeave_OB) - approvedLeave.EarnedLeave;
                    dto.BereavementLeave = Convert.ToDouble(leaveTypes.BereavementLeave_OB) - approvedLeave.BereavementLeave;
                    dto.WeddingLeave = Convert.ToDouble(leaveTypes.WeddingLeave_OB) - approvedLeave.WeddingLeave;
                    dto.LoyaltyLeave = Convert.ToDouble(leaveTypes.LoyaltyLeave_OB) - approvedLeave.LoyaltyLeave;
                }
                return dto;
            }
            catch (Exception ex)
            {
                return dto;
            }
        }

        public List<LeaveBalanceDetailsDto> DtoBinder(System.Data.DataTable data)
        {
            List<LeaveBalanceDetailsDto> leaveBalancelist = new List<LeaveBalanceDetailsDto>();
            if (data.Rows.Count > 0)
            {
                foreach (DataRow dr in data.Rows)
                {
                    leaveBalancelist.Add(new LeaveBalanceDetailsDto
                    {
                        EmpId = Convert.ToInt32(dr["EMPID"]),
                        EmpName = dr["EMPNAME"].ToString(),
                        LeaveName = dr["LEVNAME"].ToString(),
                        OpeningBalance = Convert.ToDecimal(dr["OPENING_BALANCE"]),
                        Allotted = Convert.ToDecimal(dr["ALLOTED"]),
                        Lapsed = Convert.ToDecimal(dr["LAPSE"]),
                        EnchaseDays = Convert.ToDecimal(dr["ENCHASEDAYS"]),
                        LeaveAvailed = Convert.ToDecimal(dr["LEAVEAVAILED"]),
                        ClosingBalance = Convert.ToDecimal(dr["CB"])
                    });
                }
            }
            return leaveBalancelist;
        }
        [HttpPost]
        public ActionResult ExecuteHrmAPI(string emsLeave, string email, DateTime startDate, DateTime endDate, int leaveCategory, bool isHalfDay, string action)
        {
            try
            {
                string startingDate = startDate.ToFormatDateString("yyyy-MM-dd");
                string endingDate = endDate.ToFormatDateString("yyyy-MM-dd");
                string leaveType = ((LeaveCategory)leaveCategory).GetEnumDisplayShortName();
                PostManager postRequest = new PostManager(SiteKey.HrmServiceURL + "/leaveAddUpdate");
                postRequest.AddHeader("Hrmapikey", SiteKey.HrmApiKey);
                postRequest.AddHeader("Hrmapipassword", SiteKey.HrmApiPassword);
                LeaveHrm data = new LeaveHrm();
                LeaveHrmRequest postData = new LeaveHrmRequest()
                {
                    emsLeaveId = emsLeave,
                    emailId = email,
                    startDate = startingDate,
                    endDate = endingDate,
                    leaveType = leaveType,
                    isHalf = isHalfDay != true ? "0" : "1",
                    isWfh = "0",
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