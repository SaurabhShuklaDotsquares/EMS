using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Dto
{
    public class ManageUserDto
    {
        public int Uid { get; set; }
        public int? AttendanceId { get; set; }
        public string EmployeeCode { get; set; }
        public bool IsActive { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime AddDate { get; set; }
        public string JobTitle { get; set; }
        public Nullable<int> DeptId { get; set; }
        public string DeptName { get; set; }
        public String RoleName { get; set; }
        public String PMName { get; set; }
        public String GroupName { get; set; }
        public string TLName { get; set; }

        public Nullable<int> RoleCateGoryId { get; set; }
        public int? RoleId { get; set; }
       
        public int? DesignationId { get; set; }


        

        public Nullable<int> PMUid { get; set; }
        public Nullable<int> TLId { get; set; }
        public string DOB { get; set; }
        public string JoinedDate { get; set; }
        public string EmailOffice { get; set; }
        public string EmailPersonal { get; set; }
        public string MobileNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternativeNumber { get; set; }
        public string Address { get; set; }
        public string SkypeId { get; set; }
        public string MarraigeDate { get; set; }
        public string Gender { get; set; }
        public string AadharNumber { get; set; }
        public string PanNumber { get; set; }
        public string PassportNumber { get; set; }
        public string OtherTechnology { get; set; }
        public int BloodGroupId { get; set; }

        public bool IsResigned { get; set; }

        public string[] Technology { get; set; }

        public string[] Specialist { get; set; }
        public string PasswordKey { get; set; }

        public List<UserTechnologyDto> TechnologyList { get; set; }

        public List<DomainExpertDto> DomainExpert { get; set; }

        public List<DropdownListDto> SpecTypeList { get; set; }

        public MedicalDataDto EmployeeMedicalData { get; set; }
        public string RoleCategoryName { get; set; }

        public ManageUserDto()
        {
            TechnologyList = new List<UserTechnologyDto>();
            SpecTypeList = new List<DropdownListDto>();
            DomainExpert = new List<DomainExpertDto>();
        }
    }

    public class UserTechnologyDto
    {
        public int TechId { get; set; }
        public string TechName { get; set; }
        public bool Selected { get; set; }

        public byte? SpecTypeId { get; set; }
    }

    public class DomainExpertDto {
        public int DomainId { get; set; }
        public string DomainName { get; set; }
        public bool Selected { get; set; }

    }
    public class RequestUpdateMemberObject
    {
        public string Email { get; set; }
        public int designationId { get; set; }
        public string TeamManagerEmail { get; set; }
        public string ReportsToEmail { get; set; }
    }
    public class RequestUpdateMemberObjectHRM
    {
        public string emailId { get; set; }
        public string ems_designation_id { get; set; }
        public string ems_category_id { get; set; }
        public string ems_role_id { get; set; }
        public string ActionType { get; set; }
    }
}
