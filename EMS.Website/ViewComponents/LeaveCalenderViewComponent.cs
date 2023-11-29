using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Service.SARAL;
using EMS.Service.SARALDT;
using EMS.Web.Code.LIBS;
using EMS.Web.Models.Calendar;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static EMS.Core.Enums;

namespace EMS.Website.ViewComponents
{
    public class LeaveCalenderViewComponent : ViewComponent
    {
        private readonly IUserLoginService userLoginService;
        private readonly ILeaveService leaveService;
        private readonly ILevAllotmentService levAllotmentService;
        private readonly ILevDetailsService levDetailsService;
        private readonly ILevMonthdetService levMonthdetService;
        private readonly ILevAllotmentDTService levAllotmentDTService;
        private readonly ILevDetailsDTService levDetailsDTService;
        private readonly ILevMonthdetDTService levMonthdetDTService;
        private readonly IWFHService wFHService;

        public LeaveCalenderViewComponent(IUserLoginService _userLoginService, ILeaveService _manageLeaveService, ILevAllotmentService _levAllotmentService, ILevDetailsService _levDetailsService, ILevMonthdetService _levMonthdetService, ILevAllotmentDTService _levAllotmentDTService, ILevDetailsDTService _levDetailsDTService, ILevMonthdetDTService _levMonthdetDTService,IWFHService _wFHService)
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

        public async Task<IViewComponentResult> InvokeAsync(string steps, int leaveType, string pmid, string uid)
        {
            var user = new CustomPrincipal(HttpContext.User);

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

            var isPmOrHr = false;
            isPmOrHr = (user.RoleId == (int)Enum.Parse(typeof(Core.Enums.UserRoles), Core.Enums.UserRoles.PM.ToString())) ? true :

                 (user.RoleId == (int)Enum.Parse(typeof(Core.Enums.UserRoles), Core.Enums.UserRoles.UKPM.ToString())) ? true :

                (user.RoleId == (int)Enum.Parse(typeof(Core.Enums.UserRoles), Core.Enums.UserRoles.PMO.ToString())) ? true :

                (RoleValidator.HR_RoleIds.Contains(user.RoleId)) ? true :
                false;
            ////var leaves = leaveActivityService.GetLeaveActivityByUidAndMonth(year, month, CurrentUser.Uid, isPmOrHr, leaveType: leaveType);
            int uidFilter = 0, pmidFilter = 0;
            int.TryParse(uid, out uidFilter);
            int.TryParse(pmid, out pmidFilter);

            if ((user.RoleId == (int)UserRoles.UKBDM) && pmidFilter == 0)
            {
                pmidFilter = user.PMUid;
            }

            var pagingService = new PagingService<LeaveActivity>();
            var pagingService1 = new PagingService<LateHour>();

            var expr = PredicateBuilder.True<LeaveActivity>();
            var expr1 = PredicateBuilder.True<LateHour>();

            expr = expr.And(e => (e.StartDate.Year == year || e.EndDate.Year == year));
            expr1 = expr1.And(e => (e.DayOfDate.Year == year));

            expr = expr.And(e => (e.StartDate.Month == month || e.EndDate.Month == month));
            expr1 = expr1.And(e => (e.DayOfDate.Month == month));

            int pmId = (user.RoleId == (int)Enums.UserRoles.PM || user.RoleId == (int)UserRoles.UKPM || user.RoleId == (int)Enums.UserRoles.PMO) ? user.Uid : user.PMUid;
            

            if (user.RoleId == (int)UserRoles.PM || user.RoleId == (int)UserRoles.UKPM || user.RoleId == (int)UserRoles.PMO)
            {
                expr = expr.And(e => e.Uid == user.Uid || (e.UserLogin1.PMUid.HasValue && e.UserLogin1.PMUid.Value == pmId) || (e.UserLogin1.TLId.HasValue && e.UserLogin1.TLId.Value == pmId));
                expr1 = expr1.And(e => e.Uid == user.Uid || (e.UserLogin1.PMUid.HasValue && e.UserLogin1.PMUid.Value == pmId) || (e.UserLogin1.TLId.HasValue && e.UserLogin1.TLId.Value == pmId));
            }
            else if (RoleValidator.TL_Technical_DesignationIds.Contains(user.DesignationId)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(user.DesignationId)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(user.DesignationId)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(user.DesignationId)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(user.DesignationId)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(user.DesignationId)
            || RoleValidator.TL_Sales_DesignationIds.Contains(user.DesignationId)
            || RoleValidator.TL_AccountsandAdmin_DesignationIds.Contains(user.DesignationId)
                //|| RoleValidator.DV_RoleIds.Contains(user.RoleId)
                )
            {
                int[] sdList = userLoginService.GetTLUsers(user.Uid).Select(T => T.Uid).ToArray();
                expr = expr.And(e => e.Uid == user.Uid || (e.UserLogin1.TLId.HasValue && e.UserLogin1.TLId.Value == user.Uid) || (sdList.Contains((int)e.UserLogin1.TLId)));
                expr1 = expr1.And(e => e.Uid == user.Uid || (e.UserLogin1.TLId.HasValue && e.UserLogin1.TLId.Value == user.Uid) || (sdList.Contains((int)e.UserLogin.TLId)));
            }
            else if (user.RoleId == (int)UserRoles.UKBDM)
            {
                expr = expr.And(e => e.UserLogin1.PMUid.HasValue && e.UserLogin1.PMUid.Value == pmidFilter);
                expr1 = expr1.And(e => e.UserLogin1.PMUid.HasValue && e.UserLogin1.PMUid.Value == pmidFilter);
            }
            else
            {
                if (!RoleValidator.HR_RoleIds.Contains(user.RoleId))
                {
                    expr = expr.And(e => e.Uid == user.Uid);
                    expr1 = expr1.And(e => e.Uid == user.Uid);
                }
            }

            if (uidFilter > 0)
            {
                expr = expr.And(l => l.Uid == uidFilter);
                expr1 = expr1.And(l => l.Uid == uidFilter);
            }

            expr = expr.And(l => l.UserLogin1.IsActive == true);
            expr1 = expr1.And(l => l.UserLogin1.IsActive == true);

            if (leaveType > 0)
            {
                expr = expr.And(l => l.LeaveType == leaveType);
                expr1 = expr1.And(l => l.LeaveType == leaveType);
            }
            if (pmidFilter > 0)
            {
                expr = expr.And(l => l.UserLogin1.PMUid == pmidFilter);
                expr1 = expr1.And(l => l.UserLogin1.PMUid == pmidFilter);
            }
            //Uk role leave status
            if (user.RoleId == (int)Enums.UserRoles.PMO || user.RoleId == (int)Enums.UserRoles.UKPM || user.RoleId == (int)Enums.UserRoles.UKBDM)
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
                Status = l.Status
            }).ToList();

            var lateHourList = new List<LateHour>();

            if (user.RoleId == (int)UserRoles.PMO || user.RoleId == (int)UserRoles.UKBDM)
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
            if (user.RoleId == (int)UserRoles.UKPM || user.RoleId == (int)UserRoles.UKBDM || user.RoleId == (int)UserRoles.PMO)
            {
                ViewBag.OfficialLeave = leaveService.GetOfficialEventLeaveList((byte)Country.UK).Select(l => new OfficialLeaveDto { CountryId = l.CountryId, IsActive = l.IsActive, LeaveDate = l.LeaveDate, LeaveId = l.LeaveId, Title = l.Title, LeaveType = l.LeaveType }).ToList();
                //ViewBag.OfficialLeave = leaveService.GetOfficialLeaves((byte)Country.UK, true).Select(l => new OfficialLeaveDto { CountryId = l.CountryId, IsActive = l.IsActive, LeaveDate = l.LeaveDate, LeaveId = l.LeaveId, Title = l.Title }).ToList();
                ViewBag.IsUKTeam = true;
            }
            else
            {
                ViewBag.OfficialLeave = leaveService.GetOfficialLeaves((byte)Country.India, true).Select(l => new OfficialLeaveDto { CountryId = l.CountryId, IsActive = l.IsActive, LeaveDate = l.LeaveDate, LeaveId = l.LeaveId, Title = l.Title, LeaveType = l.LeaveType }).ToList();
            }



            if (ViewBag.UnderEmployeeList == null && user.Uid > 0)
            {
                ViewBag.totalLeave = "";
                int totalCount = 0;
                List<LeaveActivity> leaveActivitys = leaveService.GetLeaves(out totalCount, pagingService);
                double totalLeaves = leaveService.GetTotalLeaves(pagingService);
                //if (totalLeaves > 0)
                //{
                //totalLeaves = 25;
                double sickLeaveCount = 0;
                double holidayLeaveCount = 0;
                String StartDate = "", EndDate = "";
                EMS.Website.Controllers.LeaveController cc = new Controllers.LeaveController(this.wFHService,this.userLoginService, this.leaveService, this.levAllotmentService, this.levDetailsService, this.levMonthdetService, this.levAllotmentDTService, this.levDetailsDTService, this.levMonthdetDTService);

                cc.HolidayEdjustmentAndHolidayCount(ref totalLeaves, user.Uid, leaveActivitys, (new DateTime(year, month, 1)).ToString("dd/MM/yyyy").Replace("-", "/"), (new DateTime(year, month, 1).AddMonths(1).AddDays(-1)).ToString("dd/MM/yyyy").Replace("-", "/"), ref sickLeaveCount, ref holidayLeaveCount, ref StartDate, ref EndDate);

                ViewBag.totalLeave = totalLeaves + (totalLeaves == 1 ? " Day" : " Days");
                ViewBag.holidayLeaveCount = holidayLeaveCount;
                ViewBag.sickLeaveCount = sickLeaveCount;
                //UserLogin user = userLoginService.GetUserInfoByID(uidFilter);
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


                //HolidayEdjustment(ref totalLeaves, uidFilter, leaveActivitys, (new DateTime(year, month, 1)).ToString("dd/MM/yyyy").Replace("-", "/"), (new DateTime(year, month, 1).AddMonths(1).AddDays(-1)).ToString("dd/MM/yyyy").Replace("-", "/"));

                //}
                //if (totalLeaves > 0)
                //{
                //    ViewBag.totalLeave = totalLeaves + (totalLeaves == 1 ? " Day" : " Days");
                //}
            }

            return View("~/Views/Leave/_LeaveCalender.cshtml", model);
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
    }
}