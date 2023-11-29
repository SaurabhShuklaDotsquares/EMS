using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{

    public class LeaveActivityDto
    {
        public bool IsUKPM { get; set; }
        public int LeaveId { get; set; }
        public int PMid { get; set; }
        public int Uid { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Reason { get; set; }
        public int Status { get; set; }

        public string WorkAlternatorName { get; set; }
        public string WorkAlternator { get; set; }
        public string Remark { get; set; }
        public bool IsHalf { get; set; }
        public bool FirstHalf { get; set; }
        public bool SecondHalf { get; set; }
        public bool IsSelfLeave { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModify { get; set; }
        public int WorkAlterID { get; set; }
        public int LeaveType { get; set; }
        public string IP { get; set; }
        public int AdjustID { get; set; }
        public int? HalfValue { get; set; }

        public bool IsCancel { get; set; }
        public string FullOrHalf { get; set; }
        public int LeaveCategory { get; set; }
        public int userId { get; set; }

        public List<SelectListItem> selectWAList { get; set; }

        public List<SelectListItem> selectEmployeeList { get; set; }


        public int HolidayType { get; set; }
        public List<SelectListItem> HolidayTypeList { get; set; }
        public List<SelectListItem> LeaveCategoryList { get; set; }
        public List<SelectListItem> HalfType { get; set; }
        public LeaveTypesDto LeaveTypeBalance { get; set; }
        public ApprovedLeaveDto ApprovedLeaveBalance { get; set; }
        public CurrentLeaveDto CurrentLeaveBalance { get; set; }
        public bool IsAllowLeave { get; set; }
    }


    public class HolidayTypeDto
    {
        public int UID { get; set; }
        public string UName { get; set; }
        public double Sick { get; set; }
        public double Holiday { get; set; }
    }


    public class LeaveActivityCalenderDto
    {
        public int LeaveId { get; set; }
        public int Uid { get; set; }
        public int? Status { get; set; }
        public int? LeaveType { get; set; }
        public int PmId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime DateModify { get; set; }

        public string Reason { get; set; }
        public string Remark { get; set; }
        public string Name { get; set; }
        public string TLName { get; set; }
        public string WorkAlternator { get; set; }
        public string HandoverTo { get; set; }
        public string LeaveCatagory { get; set; }
        public int? LeaveCatagoryId { get; set; }

        public bool IsHalf { get; set; }
        public bool IsAllowLeave { get; set; }
    }

    public class LeaveCalenderDto
    {
        public int LeaveId { get; set; }
        public int? Status { get; set; }
        public int? LeaveType { get; set; }
        public int? LeaveCategory { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Name { get; set; }
        public bool IsHalf { get; set; }
    }

    public class LeaveActivityAdjustDto
    {

        public int LeaveActivityAdjustId { get; set; }
        public int LeaveId { get; set; }
        public int Adjustid { get; set; }
        public DateTime AddDate { get; set; }

    }
    public class LeaveAdjustDto
    {
        public int AdjustId { get; set; }
        public int Uid { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public bool IsHalf { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModify { get; set; }
        public bool IsCancel { get; set; }
        public string IP { get; set; }
        public bool IsCL { get; set; }
        public bool isadjust { get; set; }
        public bool CLHalfAdjustId { get; set; }

    }


}
