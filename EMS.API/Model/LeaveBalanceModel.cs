namespace EMS.API.Model
{
    public class LeaveBalanceModel
    {
        public int attendanceId { get; set; }
    }
    public class CurrentLeaveModel
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
        public int AttendanceId { get; set; }
        public string Email { get; set; }
    }
}
