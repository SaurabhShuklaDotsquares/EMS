using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using static EMS.Core.Enums;

namespace EMS.Dto
{
    public class AdjustHoursDto
    {
        public AdjustHoursDto()
        {
            EmployeeList = new List<EmployeeListDto>();
        }
        public string Modified { get; set; }
        public List<EmployeeListDto> EmployeeList { get; set; }
    }
    public class EmployeeListDto
    {
        public EmployeeListDto()
        {
            SelectEmployeeList = new List<SelectListItem>();
        }
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public List<SelectListItem> SelectEmployeeList { get; set; }
        public int Id { get; set; }
        public int Uid { get; set; }
        public string LateStartTime { get; set; }
        public string EarlyLeaveTime { get; set; }
        public string WorkAtHome { get; set; }
        public string WorkFromHome { get; set; }
        public string LateReason { get; set; }
        public string EarlyReason { get; set; }
        public int LeaveType { get; set; }
    }
}
