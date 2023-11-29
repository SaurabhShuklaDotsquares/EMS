using System;
using System.Collections.Generic;

namespace EMS.Data.Models
{
    public partial class UserLogin
    {
        public UserLogin()
        {
            InversePmu = new HashSet<UserLogin>();
        }

        public int Uid { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string JobTitle { get; set; }
        public int? DeptId { get; set; }
        public int? RoleId { get; set; }
        public int? Tlid { get; set; }
        public DateTime? Dob { get; set; }
        public DateTime? JoinedDate { get; set; }
        public DateTime? AddDate { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Ip { get; set; }
        public string EmailOffice { get; set; }
        public string EmailPersonal { get; set; }
        public string MobileNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternativeNumber { get; set; }
        public string Address { get; set; }
        public string SkypeId { get; set; }
        public DateTime? MarraigeDate { get; set; }
        public string Gender { get; set; }
        public int? Pmuid { get; set; }
        public bool? IsSuperAdmin { get; set; }
        public int? CrmuserId { get; set; }
        public string ApiPassword { get; set; }
        public string PanNumber { get; set; }
        public string AadharNumber { get; set; }
        public string PassportNumber { get; set; }
        public int? BloodGroupId { get; set; }
        public int? Hrmid { get; set; }
        public string EmpCode { get; set; }
        public bool IsResigned { get; set; }
        public bool IsSpeg { get; set; }
        public string OtherTechnology { get; set; }
        public string PasswordKey { get; set; }
        public int? AttendenceId { get; set; }
        public DateTime? ResignationDate { get; set; }
        public DateTime? RelievingDate { get; set; }
        public bool? IsInterestedPffaccount { get; set; }
        public string Uannumber { get; set; }
        public string ProfilePicture { get; set; }
        public bool? IsFromDbdt { get; set; }
        public bool? IsExpensesAllowed { get; set; }
        public string PasswordBackUp { get; set; }
        public int? DesignationId { get; set; }
        public int? RoleCategoryId { get; set; }

        public virtual Designation Designation { get; set; }
        public virtual UserLogin Pmu { get; set; }
        public virtual ICollection<UserLogin> InversePmu { get; set; }
    }
}
