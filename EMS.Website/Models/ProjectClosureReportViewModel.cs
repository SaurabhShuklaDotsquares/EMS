using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Web.Modals
{
    public class ProjectClosureReportViewModel
    {
        public int CRMId { get; set; }
        public string ProjectName { get; set; }
        public string ClientName { get; set; }
        public string Status { get; set; }
        public string ProjectStatus { get; set; }
        public string Quality { get; set; }
        public string Technology { get; set; }
        public string DateOfChasing { get; set; }
        public string LatestConversation { get; set; }
        public string NextDate { get; set; }
        public string BAName { get; set; }
        public string PMName { get; set; }
        public string DateOfClosing { get; set; }
        public string TLName { get; set; }
        public string ActualDeveloper { get; set; }
        public string ChaseHistory { get; set; }
    }
}