using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Data.Model
{
   public class TeamStatusReportGraph_Result
    {
        public Int32 NoOfEmployee { get; set; }
        public DateTime Date { get; set; }
        public Int32 MemberOfTeam { get; set; }
        public string TlName { get; set; }
    }
    public class AllTeamStatusReportGraph_Result
    {
        public Int32 NoOfEmployee { get; set; }
        public DateTime Date { get; set; }
        public Int32 MemberOfTeam { get; set; }
        public string PMName { get; set; }
    }

    public class TeamStatusReportGraphDetail_Result
    {      
        public DateTime Date { get; set; }
        public string Employee { get; set; }
        public string ProjectName { get; set; }
        public string Status { get; set; }

    }

}
