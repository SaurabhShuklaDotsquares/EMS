using System;


namespace EMS.Data
{
    public partial class GetTimesheetsByTimesheetIds_Result
    {
        public Nullable<long> Row_Number { get; set; }
        public Nullable<int> TotalCount { get; set; }
        public decimal UserTimeSheetID { get; set; }
        public System.DateTime AddDate { get; set; }
        public string Description { get; set; }
        public System.TimeSpan WorkHours { get; set; }
        public string ProjectName { get; set; }
        public string Name { get; set; }
        public string VirtualDeveloper { get; set; }
    }
}
