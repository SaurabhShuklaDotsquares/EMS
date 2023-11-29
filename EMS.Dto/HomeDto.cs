using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
    public class HomeDto
    {
        public string UserBirthday { get; set; }
        public string UserMarriage { get; set; }
        public string UserWorkanniversary { get; set; }
        public string UserTimeSheet { get; set; }

        public string DailyThought1 { get; set; }
        public string DailyThought2 { get; set; }

        public bool IsPMUser { get; set; }

        public bool IsDirector { get; set; }

        public bool IsAshishTeamMember { get; set; }
        public bool IsUKAUUserIDToShowAshishTeamActivity { get; set; }

        public string PlanHours { get; set; }
        public string ActualHours { get; set; }

        public List<SelectListItem> BAList { get; set; }
        public HomeDto()
        {
            BAList = new List<SelectListItem>();
        }
        //public LeaveTypesDto LeaveTypeBalance { get; set; }
        //public ApprovedLeaveDto ApprovedLeaveBalance { get; set; }
        //public CurrentLeaveDto CurrentLeaveBalance { get; set; }
    }
    //public class LeaveTypesDto
    //{
    //    public double? CasualLeave_OB { get; set; }
    //    public double? CasualLeave_CB { get; set; }
    //    public double? LossPayLeave_OB { get; set; }
    //    public double? LossPayLeave_CB { get; set; }
    //    public double? CompensatoryOff_OB { get; set; }
    //    public double? CompensatoryOff_CB { get; set; }
    //    public double? PaternityLeave_OB { get; set; }
    //    public double? PaternityLeave_CB { get; set; }
    //    public double? EarnedLeave_OB { get; set; }
    //    public double? EarnedLeave_CB { get; set; }
    //    public double? SickLeave_OB { get; set; }
    //    public double? SickLeave_CB { get; set; }
    //    public double? MaternityLeave_OB { get; set; }
    //    public double? MaternityLeave_CB { get; set; }
    //    public double? BereavementLeave_OB { get; set; }
    //    public double? BereavementLeave_CB { get; set; }
    //    public double? WeddingLeave_OB { get; set; }
    //    public double? WeddingLeave_CB { get; set; }
    //    public double? LoyaltyLeave_OB { get; set; }
    //    public double? LoyaltyLeave_CB { get; set; }
    //}

    //public class ApprovedLeaveDto
    //{
    //    public double? CasualLeave { get; set; }
    //    public double? LossPayLeave { get; set; }
    //    public double? CompensatoryOff { get; set; }
    //    public double? PaternityLeave { get; set; }
    //    public double? EarnedLeave { get; set; }
    //    public double? SickLeave { get; set; }
    //    public double? MaternityLeave { get; set; }
    //    public double? BereavementLeave { get; set; }
    //    public double? WeddingLeave { get; set; }
    //    public double? LoyaltyLeave { get; set; }
    //}
    //public class CurrentLeaveDto
    //{
    //    public double? CasualLeave { get; set; }
    //    public double? LossPayLeave { get; set; }
    //    public double? CompensatoryOff { get; set; }
    //    public double? PaternityLeave { get; set; }
    //    public double? EarnedLeave { get; set; }
    //    public double? SickLeave { get; set; }
    //    public double? MaternityLeave { get; set; }
    //    public double? BereavementLeave { get; set; }
    //    public double? WeddingLeave { get; set; }
    //    public double? LoyaltyLeave { get; set; }
    //}
}
