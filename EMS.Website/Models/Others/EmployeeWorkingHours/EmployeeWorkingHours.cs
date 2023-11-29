using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.Website.Models.Others
{
    public class RequestObject
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string emailid { get; set; }
    }
    public class ResponseRoot
    {
        public List<ResponseObject> ResponsePacket { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }
    public class ResponseObject
    {
        public int? CRMID { get; set; }
        public string ProjectName { get; set; }
        public string MemberEmail { get; set; }
        public string MemberName { get; set; }
        public double ActualHours { get; set; }
        public double PlanHour { get; set; }
        public string TaskName { get; set; }
        public string ActualHoursEMS { get; set; }
        public string PlanHourEMS { get; set; }
    }
    public class ResponseSummary
    {
        public int? CRMID { get; set; }
        public string ProjectName { get; set; }
        public string MemberEmail { get; set; }
        public string MemberName { get; set; }
        public double ActualHours { get; set; }
        public double PlanHour { get; set; }
        public string TaskName { get; set; }
        public string ActualHoursEMS { get; set; }
        public string PlanHourEMS { get; set; }
        public string TLEmail { get; set; }
        public string TLName { get; set; }
        public string MobileNumber { get; set; }
        public string LeaveActivity { get; set; }
    }

    public class WorkingHoursSummaryObject
    {
        public int rowIndex { get; set; }
        public string MemberName { get; set; }
        public string MobileNumber { get; set; }
        public string MemberEmail { get; set; }
        public string DepartmentName { get; set; }
        public string TLName { get; set; }

        public string ProjectHtml { get; set; }
        public double TotalActualHours { get; set; }
        public double TotalPlanHours { get; set; }
        public string ColorCode { get; set; }
        public string LeaveStatus { get; set; }
    }

}
