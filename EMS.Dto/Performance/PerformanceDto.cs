using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace EMS.Dto
{
    public class PerformanceDto
    {
        public int Uid { get; set; }
        public string EmployeeCode { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string DateOfJoining { get; set; }
        public string Position { get; set; }
        public string DepartmentName { get; set; }

        public int NoOfWorkingDays { get; set; }
        public int NoOfTotalWorkingDays { get; set; }
        public int NoOfFreeDays { get; set; }
        public int Appreciation { get; set; }
        public int NoOfComplaints { get; set; }
        public string ReportingPerson { get; set; }
        public int NoOfProjectWorking { get; set; }
        public int NoOfUniqueProjectClosed { get; set; }
        //All including unique
        public int NoOfProjectClosed { get; set; }
        public int EstimateGiven { get; set; }
        public int EstimateAwarded { get; set; }
        public int AdditionalSupportReceived { get; set; }
        public double UrgentLeaves { get; set; }
        public double NormalLeaves { get; set; }
        public int OpportunitiesTaken { get; set; }
        public string TentativeArrivalTime { get; set; }
        public string TentativeDepartureTime { get; set; }

        public string YearsInCompany { get; set; }

        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int Assignments { get; set; }

        public int OrgImprovements { get; set; }
        public int IndividualImprovements { get; set; }

    }

    public class PerformanceIndexDto
    {
        public PerformanceIndexDto()
        {
            users = new List<SelectListItem>();
        }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public List<SelectListItem> users { get; set; }
        public int UserId { get; set; }
    }
}
