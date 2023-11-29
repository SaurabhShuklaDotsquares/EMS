using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Dto
{
    public class UserSessionDto
    {
        public int Uid { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string JobTitle { get; set; }
        public int DeptId { get; set; }
        public int RoleId { get; set; }
        public int TLId { get; set; }
        public int DesignationId { get; set; }
        public string DesignationName { get; set; }
        public string EmailOffice { get; set; }
        public string MobileNumber { get; set; }
        public int PMUid { get; set; }
        public Boolean IsSuperAdmin { get; set; }
        public Boolean IsSPEG { get; set; }
        public int CRMUserId { get; set; }
        public int AttendenceId { get; set; }
        public string ApiPassword { get; set; }
        public string Gender { get; set; }
        public string MarriageDate { get; set; }
        public bool IsAllowLeave { get; set; }
        public bool IsAllowWFH { get; set; }
        public double NotificationMinute { get; set; }
    }
}
