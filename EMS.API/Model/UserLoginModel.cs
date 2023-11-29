using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.API.Model
{
    public class UserLoginModel
    {
        public UserLoginModel()
        {
            TechDetails = new List<TechDetail>();
        }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }  // dd/mm/yyyy
        public string PMEmailId { get; set; }
        public string TLEmailId { get; set; }
        public string EmailOffice { get; set; }
        public string EmailPersonal { get; set; }
        //public string UserName { get; set; }
        public string JobTitle { get; set; }
        public string DepartmentCode { get; set; }  // Department Code
        public string Role { get; set; }    // Role Code
        public string JoinedDate { get; set; }  // dd/mm/yyyy
        public string MobileNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternativeNumber { get; set; }
        public string Address { get; set; }
        public string MarriageDate { get; set; }   // dd/mm/yyyy
        public string AadhaarNumber { get; set; }
        public string PanNumber { get; set; }
        public string PassportNumber { get; set; }
        public string BloodGroup { get; set; }
        public string ActionType { get; set; }   // add, update
        public int HRMId { get; set; }
        public string EmpCode { get; set; }
        public string AttendenceId { get; set; }
        public string RelievingDate { get; set; }
        public List<TechDetail> TechDetails { get; set; }
        public int DesignationId { get; set; }
    }

    public class TechDetail
    {
        public string Technology { get; set; }
        public bool Specialist { get; set; }
    }
}
