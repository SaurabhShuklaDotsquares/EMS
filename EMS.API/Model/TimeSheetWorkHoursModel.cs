using System.Collections.Generic;

namespace EMS.API.Model
{
    public class TimeSheetWorkHoursModel
    {

        public string MemberEmail { get; set; }
        public List<MonthData> MonthData { get; set; }
    }
    public class SearchWorkHoursModel
    {
        public string email { get; set; }
        public string month { get; set; }
        public string year { get; set; }
    }

    public class MonthData
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string Hours { get; set; }
    }

    public class SearchUserLeaveAppraiseModel
    {
        public string email { get; set; }
        public string year { get; set; }
    }

    public class UserLeaveAppriaseModel
    {
        public string year { get; set; }
        public string email { get; set; }
        public decimal totalleavestaken { get; set; }
        public decimal totalleavesinyear { get; set; }
        public int totalappreciationinyear { get; set; }
        public int totalcomplaininyear { get; set; }
    }

    public class SearchLeaveModel
    {
        //email will come in this field
        public string memberid { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
    }

    public class LeaveModel
    {
        public string Date { get; set; }
        public string Status { get; set; }
        public string LeaveType { get; set; }
        public double NoOfDay { get; set; }
    }
    public class TimeSheet
    {
        public WorkingDaysModel timeSheet { get; set; }
    }
    public class WorkingDaysModel
    {
        public string workingDays { get; set; }
        public string fullDayLeaves { get; set; }
        public string halfDayLeaves { get; set; }
    }
    public class ReviewModel
    {
        public ReviewModel()
        {
            ComplainType = new List<ComplainTypeModel>();
            AppraisedType = new List<AppraisedTypeModel>();
        }
        public List<ComplainTypeModel> ComplainType { get; set; }
        public List<AppraisedTypeModel> AppraisedType { get; set; }
    }


    public class ComplainTypeModel
    {
        public string Type { get; set; }
        public string EmsId { get; set; }
        //public string Project { get; set; }
        public string Severioty { get; set; }
        public string CreatedOn { get; set; }
        public string TLPMCoolments { get; set; }


    }
    public class AppraisedTypeModel
    {
        public string Type { get; set; }
        public string EmsId { get; set; }
        //public string Project { get; set; }
        public string CreatedOn { get; set; }
        public string TLPMCoolments { get; set; }
    }
    public class JoiningModel
    {
        public string Email { get; set; }
        public string JoinedDate { get; set; }
    }
    public class LoginUserResponseModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string TeamManager { get; set; }
        public string TeamManagerEmail { get; set; }
        public string LastLoginDate { get; set; }
        public string LastLoginTime { get; set; }
    }
    public class ActiveUserResponseModel
    {
        public int Uid { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public string Role { get; set; }
        public string TL { get; set; }
        public string PM { get; set; }
        public string DOB { get; set; }
        public string JoinedDate { get; set; }
        public string EmailOffice { get; set; }
        public string MobileNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string SkypeId { get; set; }
        public string MarraigeDate { get; set; }
        public string Gender { get; set; }
        public string ProfilePicture { get; set; }
        public int? HRMId { get; set; }
        public int? AttendenceId { get; set; }
        public string EmpCode { get; set; }
    }
}
