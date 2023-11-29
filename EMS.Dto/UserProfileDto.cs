using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Dto
{
    public class UserProfileDto
    {

        public int Uid { get; set; }
        public string EmployeeCode { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public DateTime AddDate { get; set; }
        public string JobTitle { get; set; }
        public Nullable<int> DeptId { get; set; }
        public string DeptName { get; set; }
        public String RoleName { get; set; }
        public string RoleCategoryName { get; set; }

        
        public String PMName { get; set; }
        public String GroupName { get; set; }
        public string TLName { get; set; }
        public Nullable<int> RoleId { get; set; }
        public Nullable<int> PMUid { get; set; }
        public Nullable<int> TLId { get; set; }
        public string DOB { get; set; }
        //public Nullable<System.DateTime> DOB { get; set; }
        public string JoinedDate { get; set; }
        //public Nullable<System.DateTime> JoinedDate { get; set; }                
        public string EmailOffice { get; set; }
        public string EmailPersonal { get; set; }
        public string MobileNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternativeNumber { get; set; }
        public string Address { get; set; }
        public string SkypeId { get; set; }
        public string MarraigeDate { get; set; }
        //public Nullable<System.DateTime> MarraigeDate { get; set; }        
        public string Gender { get; set; }

        public string ProfilePicture { get; set; }//db col
        public IFormFile ProfileImage { get; set; }

        public string AadharNumber { get; set; }
        public string PanNumber { get; set; }
        public string PassportNumber { get; set; }
        public string OtherTechnology { get; set; }
        public int BloodGroupId { get; set; }
        public bool? IsInterestedPffaccount { get; set; }
        public bool? IsFromDbdt { get; set; }
        public bool? IsExpensesAllowed { get; set; }
        public string UANNumber { get; set; }

        public string[] Technology { get; set; }

        public string[] Specialist { get; set; }

        public List<UserTechnologyDto> TechnologyList { get; set; }
        public List<DropdownListDto> SpecTypeList { get; set; }

        public List<DomainExpertDto> DomainExpert { get; set; }

        public List<UserProfileDocumentDto> ProfileDocumentsList { get; set; }
        public string DesignationName { get; set; }

        public UserProfileDto()
        {
            TechnologyList = new List<UserTechnologyDto>();
            SpecTypeList = new List<DropdownListDto>();
            DomainExpert = new List<DomainExpertDto>();
            ProfileDocumentsList = new List<UserProfileDocumentDto>();
        }


    }


    public class UserProfileDocumentDto
    {
        public int Id { get; set; }
        public int UId { get; set; }
        public string DocumentPath { get; set; }
    }
}
