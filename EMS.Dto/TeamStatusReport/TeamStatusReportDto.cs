using EMS.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class TeamStatusReportDto
    {      
        public List<SelectListItem> TlList { get; set; }      
        public string Uid { get; set; }
        public string PMUid { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public bool IsCurrentWeek { get; set; }
        public bool IsCurrentMonth { get; set; }
    }

    public class TeamStatusReportGraphDto
    {      
        public int NoOfEmployee { get; set; }

        public string Date { get; set; }

        public string TlName { get; set; }
        public int MemberOfTeam { get; set; }
    }
    public class TeamStatusReportGraphDetailsDto
    {
        public string Date { get; set; }
        public string Employee { get; set; }
        public string ProjectName { get; set; }
        public string Status { get; set; }
    }

}
