using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Data
{
   public  class GetMonthTimesheets_Result
    {
        public decimal UserTimeSheetId { get; set; }
        public System.DateTime AddDate { get; set; }
        public System.TimeSpan WorkHours { get; set; }
        public int CRMProjectId { get; set; }
        public Nullable<int> RoleId { get; set; }
    }
    public class GetMonthTimesheetsNew_Result
    {
        public decimal UserTimeSheetId { get; set; }
        public System.DateTime AddDate { get; set; }
        public System.TimeSpan WorkHours { get; set; }
        public int CRMProjectId { get; set; }
        public Nullable<int> RoleId { get; set; }
        public Nullable<int> DesignationId { get; set; }
    }
}
