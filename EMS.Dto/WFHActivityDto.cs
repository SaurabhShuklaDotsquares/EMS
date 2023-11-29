using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
    public class WFHActivityDto
    {
        public bool IsUKPM { get; set; }
        public int WFHId { get; set; }
        public int PMid { get; set; }
        public int Uid { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int Status { get; set; }
        public string Comment { get; set; }
        public bool IsHalf { get; set; }
        public bool FirstHalf { get; set; }
        public bool SecondHalf { get; set; }
        public bool IsSelfWFH { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModify { get; set; }
        public string IP { get; set; }
        public int? HalfValue { get; set; }
        public bool IsCancel { get; set; }
        public string FullOrHalf { get; set; }
        public int WFHCategory { get; set; }
        public string AnyComment { get; set; }
        public int ? ApprovedById { get; set; }
        public int userId { get; set; }

        public List<SelectListItem> selectWAList { get; set; }
        public List<SelectListItem> selectEmployeeList { get; set; }
        public List<SelectListItem> WFHCategoryList { get; set; }
        public List<SelectListItem> HalfType { get; set; }

        public bool IsAllowWFH { get; set; }
    }

    public class WFHActivityCalenderDto
    {
        public int WFHId { get; set; }
        public int Uid { get; set; }
        public int? Status { get; set; }
        public int PmId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime DateModify { get; set; }
        public string Comment { get; set; }
        public string Name { get; set; }
        public string TLName { get; set; }
        public string WFHCatagory { get; set; }
        public int? WFHCatagoryId { get; set; }
        public bool IsHalf { get; set; }
    }

    public class WFHCalenderDto
    {
        public int WFHId { get; set; }
        public int? Status { get; set; }
        public int? WFHCategory { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Name { get; set; }
        public bool IsHalf { get; set; }
    }
}
