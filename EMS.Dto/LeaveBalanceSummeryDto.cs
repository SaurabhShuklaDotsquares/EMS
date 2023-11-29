using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class LeaveBalanceSummeryDto
    {
        public int? Uid { get; set; }
        public string Gender { get; set; }
        public string Name { get; set; }
        public int? AttendenceId { get; set; }
        public LeaveTypesDto LeaveTypeBalance { get; set; }
        public ApprovedLeaveDto ApprovedLeaveBalance { get; set; }
        public CurrentLeaveDto CurrentLeaveBalance { get; set; }
        public bool isPreviousMonthLeave { get; set; }
        public string MonthName { get; set; }
    }
    public class LeaveTypesDto
    {
        public double? CasualLeave_OB { get; set; } = 0;
        public double? CasualLeave_CB { get; set; } = 0;
        public double? CasualLeave_AB { get; set; } = 0;
        public double? LossPayLeave_OB { get; set; } = 0;
        public double? LossPayLeave_CB { get; set; } = 0;
        public double? LossPayLeave_AB { get; set; } = 0;
        public double? CompensatoryOff_OB { get; set; } = 0;
        public double? CompensatoryOff_CB { get; set; } = 0;
        public double? CompensatoryOff_AB { get; set; } = 0;
        public double? PaternityLeave_OB { get; set; } = 0;
        public double? PaternityLeave_CB { get; set; } = 0;
        public double? PaternityLeave_AB { get; set; } = 0;
        public double? EarnedLeave_OB { get; set; } = 0;
        public double? EarnedLeave_CB { get; set; } = 0;
        public double? EarnedLeave_AB { get; set; } = 0;
        public double? SickLeave_OB { get; set; } = 0;
        public double? SickLeave_CB { get; set; } = 0;
        public double? SickLeave_AB { get; set; } = 0;
        public double? MaternityLeave_OB { get; set; } = 0;
        public double? MaternityLeave_CB { get; set; } = 0;
        public double? MaternityLeave_AB { get; set; } = 0;
        public double? BereavementLeave_OB { get; set; } = 0;
        public double? BereavementLeave_CB { get; set; } = 0;
        public double? BereavementLeave_AB { get; set; } = 0;
        public double? WeddingLeave_OB { get; set; } = 0;
        public double? WeddingLeave_CB { get; set; } = 0;
        public double? WeddingLeave_AB { get; set; } = 0;
        public double? LoyaltyLeave_OB { get; set; } = 0;
        public double? LoyaltyLeave_CB { get; set; } = 0;
        public double? LoyaltyLeave_AB { get; set; } = 0;
    }

    public class ApprovedLeaveDto
    {
        public double? CasualLeave { get; set; }
        public double? LossPayLeave { get; set; }
        public double? CompensatoryOff { get; set; }
        public double? PaternityLeave { get; set; }
        public double? EarnedLeave { get; set; }
        public double? SickLeave { get; set; }
        public double? MaternityLeave { get; set; }
        public double? BereavementLeave { get; set; }
        public double? WeddingLeave { get; set; }
        public double? LoyaltyLeave { get; set; }
    }
    public class CurrentLeaveDto
    {
        public double? CasualLeave { get; set; }
        public double? LossPayLeave { get; set; }
        public double? CompensatoryOff { get; set; }
        public double? PaternityLeave { get; set; }
        public double? EarnedLeave { get; set; }
        public double? SickLeave { get; set; }
        public double? MaternityLeave { get; set; }
        public double? BereavementLeave { get; set; }
        public double? WeddingLeave { get; set; }
        public double? LoyaltyLeave { get; set; }
    }
}
