using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto.SARAL
{
    public class LeaveBalanceDetailsDto
    {
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string LeaveName { get; set; }
        public string LeaveCode { get; set; }
        public decimal? OpeningBalance { get; set; }
        public decimal? Allotted { get; set; }
        public decimal? Lapsed { get; set; }
        public decimal? EnchaseDays { get; set; }
        public decimal? LeaveAvailed { get; set; }
        public decimal? ClosingBalance { get; set; }
    }
    //public class LeaveTypesDto
    //{
    //    public decimal? CasualLeave_OB { get; set; }
    //    public decimal? CasualLeave_CB { get; set; }
    //    public decimal? LossPayLeave_OB { get; set; }
    //    public decimal? LossPayLeave_CB { get; set; }
    //    public decimal? CompensatoryOff_OB { get; set; }
    //    public decimal? CompensatoryOff_CB { get; set; }
    //    public decimal? PaternityLeave_OB { get; set; }
    //    public decimal? PaternityLeave_CB { get; set; }
    //    public decimal? EarnedLeave_OB { get; set; }
    //    public decimal? EarnedLeave_CB { get; set; }
    //    public decimal? SickLeave_OB { get; set; }
    //    public decimal? SickLeave_CB { get; set; }
    //    public decimal? MaternityLeave_OB { get; set; }
    //    public decimal? MaternityLeave_CB { get; set; }
    //    public decimal? BereavementLeave_OB { get; set; }
    //    public decimal? BereavementLeave_CB { get; set; }
    //    public decimal? WeddingLeave_OB { get; set; }
    //    public decimal? WeddingLeave_CB { get; set; }
    //    public decimal? LoyaltyLeave_OB { get; set; }
    //    public decimal? LoyaltyLeave_CB { get; set; }
    //}
}
