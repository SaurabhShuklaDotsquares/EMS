using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EMS.Dto
{
    public class CVBuilderDto
    {
        public CVBuilderDto()
        {            
            UserList = new List<SelectListItem>();
            Industries = new List<SelectListItem>();
            TechnologyParents = new List<SelectListItem>();
            Technologies = new List<SelectListItem>();            
        }
        public long Id { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }        
        public List<SelectListItem> UserList { get; set; }
       

        public int Uid_User { get; set; }        
        public string UserName { get; set; }        
        public string Designation { get; set; }        
        public string Email { get; set; }        
        public string Languages { get; set; }        
        public string Linkedin { get; set; }        
        public string Phone { get; set; }        
        public string Address { get; set; }                
        public DateTime Created { get; set; }
        public int Status { get; set; }
        //[Required(ErrorMessage = "Experience is required")]
        public int ExperienceType { get; set; }

        //[AllowHtml]
        public string Title { get; set; }
        public string ProfileSummary { get; set; }
        public string TechnicalSkills { get; set; }
        public string WorkExperience { get; set; }
        public string RolesAcross { get; set; }
        public bool isAshishTeamMember { get; set; }
        public byte LibraryTypeId { get; set; }
        
        public string Value { get; set; }
        //public string[] Industry { get; set; }
        //public string[] Technology { get; set; }
        public List<int> Industry { get; set; }
        public List<CVTechData> Technology { get; set; }
        public string TechnologyJson { get; set; }
        public string IndustryJson { get; set; }
        public string OtherIndustry { get; set; }
        public string OtherTechnologyParent { get; set; }
        public string OtherTechnology { get; set; }
        public string dataListJson { get; set; }
        public string EducationJson { get; set; }
        public string CertificationsJson { get; set; }
        public string PreviousExperienceJson { get; set; }
        public string UserDomainJson { get; set; }
        public string EncryptUid { get; set; }
        public IFormFile ProfileImage { get; set; }
        public IFormFile CertificationImage { get; set; }
        public string ProfilePicture { get; set; }
        [DisplayName("I acknowledge that the above information is true and valid to the best of my knowledge.")]
        public bool IsAgree { get; set; }
        [DisplayName("Industry")]
        public List<SelectListItem> Industries { get; set; }
        [DisplayName("Technology Category")]
        public List<SelectListItem> TechnologyParents { get; set; }
        [DisplayName("Technology")]
        public List<SelectListItem> Technologies { get; set; }
        public List<CVBuilderData> dataList { get; set; }
        public List<EducationData> Education { get; set; }
        public List<CVBuilderCertificationsData> Certifications { get; set; }
        public List<IFormFile> CertificationIMG { get; set; }
        public List<PreviousExperienceData> PreviousExperience { get; set; }
        public List<UserTechnologyDto> TechnologyList { get; set; }
        public List<DropdownListDto> SpecTypeList { get; set; }
        public List<DomainExpertDto> DomainExpert { get; set; }
    }    
    public class CVBuilder_Dto
    {
        public CVBuilder_Dto()
        {
            UserList = new List<SelectListItem>();
            Industries = new List<SelectListItem>();
            TechnologyParents = new List<SelectListItem>();
            Technologies = new List<SelectListItem>();
        }
        public long Id { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<SelectListItem> UserList { get; set; }


        public int Uid_User { get; set; }
        public string UserName { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string Languages { get; set; }
        public string Linkedin { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime Created { get; set; }
        public int Status { get; set; }
        //[Required(ErrorMessage = "Experience is required")]
        public int ExperienceType { get; set; }

        //[AllowHtml]
        public string Title { get; set; }
        public string ProfileSummary { get; set; }
        public string TechnicalSkills { get; set; }
        public string WorkExperience { get; set; }
        public string RolesAcross { get; set; }
        public bool isAshishTeamMember { get; set; }
        public byte LibraryTypeId { get; set; }

        public string Value { get; set; }
        public string[] Industry { get; set; }
        public string[] Technology { get; set; }
        public string OtherIndustry { get; set; }
        public string OtherTechnologyParent { get; set; }
        public string OtherTechnology { get; set; }
        public string dataListJson { get; set; }
        public string EducationJson { get; set; }
        public string CertificationsJson { get; set; }
        public string PreviousExperienceJson { get; set; }
        
        [DisplayName("Industry")]
        public List<SelectListItem> Industries { get; set; }
        [DisplayName("Technology Category")]
        public List<SelectListItem> TechnologyParents { get; set; }
        [DisplayName("Technology")]
        public List<SelectListItem> Technologies { get; set; }
        public List<CVBuilderData> dataList { get; set; }
        public List<EducationData> Education { get; set; }
        public List<CVBuilderData> Certifications { get; set; }
        public List<PreviousExperienceData> PreviousExperience { get; set; }
        public List<UserTechnologyDto> TechnologyList { get; set; }
    }
    public class CVTechData
    {
        public byte? SpecTypeId { get; set; }
        public int TechId { get; set; }
    }
    public class CVBuilderData
    {
        public long Id { get; set; } = 0;
        public string Title { get; set; }
        public string KRAOrderno { get; set; }
    }
    public class CVBuilderCertificationsData
    {
        public long Id { get; set; } = 0;
        public string Title { get; set; }
        public string CertificationsNumber { get; set; }
        public string CertificationsURL { get; set; }
        public string ImageIndex { get; set; }
    }
    public class EducationData
    {
        public string Title { get; set; }
        public string University { get; set; }
    }
    public class PreviousExperienceData
    {
        public string OrganizationName { get; set; }
        public string Designation { get; set; }
        public string FromMonth { get; set; }
        public string FromDate { get; set; }
        public string ToMonth { get; set; }
        public string ToDate { get; set; }
    }
    public class ProjectReviewCommentsAddDto
    {
        public ProjectReviewCommentsAddDto()
        {
            PaticipantList = new List<SelectListItem>();
        }
        public int Id { get; set; }
        public int ProjectReviewId { get; set; }
        public int ActionById { get; set; }
        public int ActionFromId { get; set; }
        public int Uid_Reviewer { get; set; }
        public string PaticipantsList { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public bool Status { get; set; }
        public int[] Paticipants { get; set; }
        public string ActionByName { get; set; }
        public string ActionFromName { get; set; }
        public List<SelectListItem> PaticipantList { get; set; }
        public List<ProjectReviewCommentsAddDto> ProjectReviewCommentHistory { get; set; }
    }
    public class ProjectReviewRewardDto
    {
        public ProjectReviewRewardDto()
        {
            UserList = new List<SelectListItem>();
        }
        public int ProjectReviewId { get; set; }
        public int RewardUserId { get; set; }
        public bool RewardStatus { get; set; }
        public decimal TotalDay { get; set; }
        public DateTime Date { get; set; }
        public string ProjectConvertDate { get; set; }
        public List<SelectListItem> UserList { get; set; }
    }
    public class CVBuilderIndexDto
    {
        public CVBuilderIndexDto()
        {
            UserList = new List<SelectListItem>();
            BAList = new List<SelectListItem>();
            TLList = new List<SelectListItem>();
            ReviewerList = new List<SelectListItem>();
        }

        public int Uid_BA { get; set; }
        public int Uid_TL { get; set; }
        public int Uid_Reviewer { get; set; }
        public int Uid_User { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public bool IsCVExists { get; set; }
        public List<SelectListItem> UserList { get; set; }
        public List<SelectListItem> BAList { get; set; }
        public List<SelectListItem> TLList { get; set; }
        public List<SelectListItem> ReviewerList { get; set; }
    }
    public class CVBuilderSearchFilter
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int? Uid_User { get; set; }
        public int? department { get; set; }
        public int? pm { get; set; }
        public int[] ExperienceType { get; set; }
        public int? DomainType { get; set; }
        public int? Technology { get; set; }
        public int[] Domains { get; set; }
        public int[] Technologies { get; set; }
        public int? SpecType { get; set; }
        public int? EmpStatusType { get; set; }
        public bool EmpStatusTypeCheck { get; set; }
        public bool TechnologyrdAnd { get; set; }
        public bool TrainingCheck { get; set; }
        public bool PMReviewCheck { get; set; }
        public int TemplateId { get; set; }
    }
    public class CVViewDto
    {
        public long Id { get; set; }        
        public string UserName { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string Languages { get; set; }
        public string Linkedin { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string DateOfBirth { get; set; }
        public string ProfilePicture { get; set; }
        

        //[AllowHtml]
        public string Title { get; set; }
        public string ProfileSummary { get; set; }
        public string TechnicalSkills { get; set; }
        public string WorkExperience { get; set; }
        public string RolesAcross { get; set; }

        public string CoreCompetencies { get; set; }
        public string Certifications { get; set; }
        public string PreviousExperience { get; set; }
        public string CAREERTIMELINE { get; set; }
        public string technology { get; set; }
        public string Education { get; set; }
    }

    public class CVSpResponse
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public bool IsTraining { get; set; }
        public bool IsApproved { get; set; }
        public string Name { get; set; }
        public string EmailOffice { get; set; }
        public string MobileNumber { get; set; }
        public string Experience { get; set; }
        public string TechTitle { get; set; }
        public string DomainName { get; set; }
        public int ExperienceId { get; set; }
        public int RoleId { get; set; }
        public int[] DomainId { get; set; }
        
    }
    public class CVSearchRequest
    {
        public int? pm { get; set; }
        public int UserId { get; set; } = 0;
        public string ExperienceType { get; set; } = "";
        public string Domains { get; set; } = "";
        public string Technologies { get; set; } = "";
        public int SpecType { get; set; } = 0;
        public bool EmpStatusTypeCheck { get; set; }
        public bool TechnologyrdAnd { get; set; }
        public bool TrainingCheck { get; set; }
        public bool PMReviewCheck { get; set; }
        public string BucketProjectList { get; set; } = "";
        public int IsPm { get; set; } = 0;
    }
    public class PrintCVDto
    {
        public string htmlString { get; set; }
        public string Username { get; set; }
    }
}