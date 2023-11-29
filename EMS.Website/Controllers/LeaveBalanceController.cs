using EMS.Core;
using EMS.Dto;
using EMS.Dto.SARAL;
using EMS.Service;
using EMS.Service.SARAL;
using EMS.Service.SARALDT;
using EMS.Web.Code.Attributes;
using EMS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static EMS.Core.Enums;

namespace EMS.Website.Controllers
{
    [CustomAuthorization()]
    public class LeaveBalanceController : BaseController
    {
        #region Fileds and Constructor
        private readonly IUserLoginService userLoginService;
        private readonly ILeaveService leaveService;
        private readonly ILevDetailsService levDetailsService;
        private readonly ILevDetailsDTService levDetailsDTService;
        public bool isPreviousMonth = false;

        public LeaveBalanceController(IUserLoginService userLoginService, ILeaveService leaveService, ILevDetailsService levDetailsService, ILevDetailsDTService levDetailsDTService)
        {
            this.userLoginService = userLoginService;
            this.leaveService = leaveService;
            this.levDetailsService = levDetailsService;
            this.levDetailsDTService = levDetailsDTService;
        }
        #endregion

        public ActionResult Index()
        {
            LeaveBalanceSummeryDto model = new LeaveBalanceSummeryDto();

            try
            {
                if (CurrentUser.RoleId != (int)UserRoles.UKBDM && CurrentUser.RoleId != (int)UserRoles.UKPM && CurrentUser.RoleId != (int)UserRoles.AUPM && CurrentUser.RoleId != (int)UserRoles.PMOAU && CurrentUser.RoleId != (int)UserRoles.PMO)
                {
                    model.LeaveTypeBalance = GetAllLeaveBalance();
                    model.ApprovedLeaveBalance = GetApprovedLeave();
                    double sickLeave = leaveService.GetAllSickLeavesForYear(CurrentUser.Uid, (int)LeaveCategory.SickLeave);
                    model.CurrentLeaveBalance = GetCurrentLeaveBalance(model.LeaveTypeBalance, model.ApprovedLeaveBalance, sickLeave, isPreviousMonth);
                }
                model.isPreviousMonthLeave = isPreviousMonth;
                if (isPreviousMonth)
                {
                    model.MonthName = DateTime.Now.AddMonths(-1).ToString("MMMM");
                }

                ViewBag.AttendenceId = TempData["AttendenceId"];
                return PartialView("_LeaveBalance", model);
            }
            catch (Exception ex)
            {
                return PartialView();
            }
        }

        public ActionResult ViewUserLeaveBalance(int id)
        {
            LeaveBalanceSummeryDto model = new LeaveBalanceSummeryDto();
            if (CurrentUser.RoleId != (int)UserRoles.UKBDM && CurrentUser.RoleId != (int)UserRoles.UKPM && CurrentUser.RoleId != (int)UserRoles.AUPM && CurrentUser.RoleId != (int)UserRoles.PMOAU)
            {
                var userinfo = id > 0 ? userLoginService.GetUserInfoByID(Convert.ToInt32(id)) : userLoginService.GetUserInfoByID(CurrentUser.Uid);
                if (id > 0)
                {
                    model.LeaveTypeBalance = GetAllLeaveBalance(Convert.ToInt32(id));
                }
                else
                {
                    model.LeaveTypeBalance = GetAllLeaveBalance();
                }
                double sickLeave = leaveService.GetAllSickLeavesForYear(userinfo.Uid, (int)LeaveCategory.SickLeave);
                model.ApprovedLeaveBalance = GetCurrentMonthLeaveBalance(GetLeaveBalance(userinfo.Uid), GetApprovedLeave(userinfo.Uid));
                model.CurrentLeaveBalance = GetCurrentLeaveBalance(model.LeaveTypeBalance, model.ApprovedLeaveBalance, sickLeave, isPreviousMonth);
                model.Gender = userinfo != null ? !String.IsNullOrWhiteSpace(userinfo.Gender) ? userinfo.Gender : "" : "";
                model.Name = userinfo != null ? userinfo.Name : "";
            }
            ViewBag.AttendenceId = TempData["AttendenceId"];
            return PartialView("_UserLeaveBalance", model);
        }

        #region Leave Balance Calculation
        public ApprovedLeaveDto GetApprovedLeave(int uid = 0)
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
                dto.CasualLeave = leaveService.GetApprovedLeaves(id, (int)LeaveCategory.CasualLeave, startDate, endDate, isPreviousMonth);
                dto.EarnedLeave = leaveService.GetApprovedLeaves(id, (int)LeaveCategory.EarnedLeave, startDate, endDate, isPreviousMonth);
                dto.LossPayLeave = leaveService.GetApprovedLeaves(id, (int)LeaveCategory.UnpaidLeave, startDate, endDate, isPreviousMonth);
                dto.SickLeave = leaveService.GetApprovedLeaves(id, (int)LeaveCategory.SickLeave, startDate, endDate, isPreviousMonth);
                dto.BereavementLeave = leaveService.GetApprovedLeaves(id, (int)LeaveCategory.BereavementLeave, startDate, endDate, isPreviousMonth);
                dto.WeddingLeave = leaveService.GetApprovedLeaves(id, (int)LeaveCategory.WeddingLeave, startDate, endDate, isPreviousMonth);
                dto.CompensatoryOff = leaveService.GetApprovedLeaves(id, (int)LeaveCategory.CompensatoryOff, startDate, endDate, isPreviousMonth);
                //dto.LoyaltyLeave = leaveService.GetApprovedLeaves(id, (int)LeaveCategory.LoyaltyLeave, startDate, endDate);
                dto.MaternityLeave = leaveService.GetApprovedLeaves(id, (int)LeaveCategory.MaternityLeave, startDate, endDate, isPreviousMonth);
                dto.PaternityLeave = leaveService.GetApprovedLeaves(id, (int)LeaveCategory.PaternityLeave, startDate, endDate, isPreviousMonth);

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
                //DateTime now = DateTime.Now;
                //var startDate = new DateTime(now.Year, now.Month, 1);
                //var endDate = startDate.AddMonths(1).AddDays(-1);
                dto.CasualLeave = leaveService.GetPendingLeaves(id, (int)Enums.LeaveCategory.CasualLeave);
                dto.EarnedLeave = leaveService.GetPendingLeaves(id, (int)Enums.LeaveCategory.EarnedLeave);
                dto.LossPayLeave = leaveService.GetPendingLeaves(id, (int)Enums.LeaveCategory.UnpaidLeave);
                dto.SickLeave = leaveService.GetPendingLeaves(id, (int)Enums.LeaveCategory.SickLeave);
                dto.BereavementLeave = leaveService.GetPendingLeaves(id, (int)Enums.LeaveCategory.BereavementLeave);
                dto.WeddingLeave = leaveService.GetPendingLeaves(id, (int)Enums.LeaveCategory.WeddingLeave);
                dto.CompensatoryOff = leaveService.GetPendingLeaves(id, (int)Enums.LeaveCategory.CompensatoryOff);
                //dto.LoyaltyLeave = leaveService.GetPendingLeaves(id,(int)Enums.LeaveCategory.LoyaltyLeave);
                dto.MaternityLeave = leaveService.GetPendingLeaves(id, (int)Enums.LeaveCategory.MaternityLeave);
                dto.PaternityLeave = leaveService.GetPendingLeaves(id, (int)Enums.LeaveCategory.PaternityLeave);

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

                var empInfo = uid > 0 ? userLoginService.GetUserInfoByID(uid) : userLoginService.GetUserInfoByID(CurrentUser.Uid);
                int? empAttendanceId = empInfo != null ? empInfo.AttendenceId.HasValue ? empInfo.AttendenceId.Value : 0 : 0;
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
                TempData["AttendenceId"] = empAttendanceId;
                TempData["Gender"] = empInfo.Gender;
                return dto;
            }
            catch (Exception ex)
            {
                return dto;
            }
        }
        public CurrentLeaveDto GetCurrentLeaveBalance(LeaveTypesDto leaveTypes, ApprovedLeaveDto approvedLeave, double sickLeave, bool isPrev = false)
        {
            CurrentLeaveDto dto = new CurrentLeaveDto();
            try
            {
                if (!isPrev)
                {
                    dto.CasualLeave = Convert.ToDouble(leaveTypes.CasualLeave_OB)- approvedLeave.CasualLeave;
                    dto.LossPayLeave = Convert.ToDouble(leaveTypes.LossPayLeave_OB) - approvedLeave.LossPayLeave;
                    dto.CompensatoryOff = Convert.ToDouble(leaveTypes.CompensatoryOff_OB) - approvedLeave.CompensatoryOff;
                    dto.PaternityLeave = Convert.ToDouble(leaveTypes.PaternityLeave_OB) - approvedLeave.PaternityLeave;
                    dto.SickLeave = Convert.ToDouble(leaveTypes.SickLeave_OB) - approvedLeave.SickLeave;
                    dto.MaternityLeave = Convert.ToDouble(leaveTypes.MaternityLeave_OB) - approvedLeave.MaternityLeave;
                    dto.EarnedLeave = Convert.ToDouble(leaveTypes.EarnedLeave_OB) - approvedLeave.EarnedLeave;
                    dto.BereavementLeave = Convert.ToDouble(leaveTypes.BereavementLeave_OB) - approvedLeave.BereavementLeave;
                    dto.WeddingLeave = Convert.ToDouble(leaveTypes.WeddingLeave_OB) - approvedLeave.WeddingLeave;
                                                                                    //dto.LoyaltyLeave = Convert.ToDouble(leaveTypes.LoyaltyLeave_CB) - approvedLeave.LoyaltyLeave;

                }
                else
                {
                    dto.CasualLeave = Convert.ToDouble(leaveTypes.CasualLeave_CB - approvedLeave.CasualLeave);//- approvedLeave.CasualLeave;
                    dto.LossPayLeave = Convert.ToDouble(leaveTypes.LossPayLeave_CB - approvedLeave.LossPayLeave);//;
                    dto.CompensatoryOff = Convert.ToDouble(leaveTypes.CompensatoryOff_CB - approvedLeave.CompensatoryOff);//;
                    dto.PaternityLeave = Convert.ToDouble(leaveTypes.PaternityLeave_CB - approvedLeave.PaternityLeave);//;
                    dto.SickLeave = Convert.ToDouble(leaveTypes.SickLeave_CB - approvedLeave.SickLeave);
                    dto.MaternityLeave = Convert.ToDouble(leaveTypes.MaternityLeave_CB - approvedLeave.MaternityLeave);// - approvedLeave.MaternityLeave;
                    dto.EarnedLeave = Convert.ToDouble(leaveTypes.EarnedLeave_CB - approvedLeave.EarnedLeave);// - approvedLeave.EarnedLeave;
                    dto.BereavementLeave = Convert.ToDouble(leaveTypes.BereavementLeave_CB - approvedLeave.BereavementLeave);// - approvedLeave.BereavementLeave;
                    dto.WeddingLeave = Convert.ToDouble(leaveTypes.WeddingLeave_CB - approvedLeave.WeddingLeave);// - approvedLeave.WeddingLeave;
                                                                                                                 //dto.LoyaltyLeave = Convert.ToDouble(leaveTypes.LoyaltyLeave_CB) - approvedLeave.LoyaltyLeave;
                }
                return dto;
            }
            catch (Exception ex)
            {
                return dto;
            }
        }
        public ApprovedLeaveDto GetCurrentMonthLeaveBalance(ApprovedLeaveDto leaveTypes, ApprovedLeaveDto approvedLeave)
        {
            ApprovedLeaveDto dto = new ApprovedLeaveDto();
            try
            {
                if (isPreviousMonth)
                {

                    dto.CasualLeave = Convert.ToDouble(approvedLeave.CasualLeave);
                    dto.LossPayLeave = Convert.ToDouble(approvedLeave.LossPayLeave);
                    dto.CompensatoryOff = Convert.ToDouble(approvedLeave.CompensatoryOff);
                    dto.PaternityLeave = Convert.ToDouble(approvedLeave.PaternityLeave);
                    dto.SickLeave = Convert.ToDouble(approvedLeave.SickLeave);
                    dto.MaternityLeave = Convert.ToDouble(approvedLeave.MaternityLeave);
                    dto.EarnedLeave = Convert.ToDouble(approvedLeave.EarnedLeave);
                    dto.BereavementLeave = Convert.ToDouble(approvedLeave.BereavementLeave);
                    dto.WeddingLeave = Convert.ToDouble(approvedLeave.WeddingLeave);
                }
                else
                {
                    dto.CasualLeave = Convert.ToDouble(approvedLeave.CasualLeave);// + leaveTypes.CasualLeave;
                    dto.LossPayLeave = Convert.ToDouble(approvedLeave.LossPayLeave);// + leaveTypes.LossPayLeave;
                    dto.CompensatoryOff = Convert.ToDouble(approvedLeave.CompensatoryOff);// + leaveTypes.CompensatoryOff;
                    dto.PaternityLeave = Convert.ToDouble(approvedLeave.PaternityLeave);// + leaveTypes.PaternityLeave;
                    dto.SickLeave = Convert.ToDouble(approvedLeave.SickLeave);// + leaveTypes.SickLeave;
                    dto.MaternityLeave = Convert.ToDouble(approvedLeave.MaternityLeave);// + leaveTypes.MaternityLeave;
                    dto.EarnedLeave = Convert.ToDouble(approvedLeave.EarnedLeave);// + leaveTypes.EarnedLeave;
                    dto.BereavementLeave = Convert.ToDouble(approvedLeave.BereavementLeave);// + leaveTypes.BereavementLeave;
                    dto.WeddingLeave = Convert.ToDouble(approvedLeave.WeddingLeave);// + leaveTypes.WeddingLeave;
                }
                //dto.LoyaltyLeave = Convert.ToDouble(leaveTypes.LoyaltyLeave_CB) + approvedLeave.LoyaltyLeave;
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
        #endregion
    }
}
