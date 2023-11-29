using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class StudyDocumentsPermissionDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string KeyId { get; set; }        
        public string Technology { get; set; }        
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public bool AddDelPermission { get; set; }
        public List<FilesPermissionDto> studyDocumentFiles { get; set; } = new List<FilesPermissionDto>();
        public UserPermissionsDto userPermission { get; set; } = new UserPermissionsDto();
    }
    public class FilesPermissionDto
    {
        public string FileName { get; set; }
        public string DisplayName { get; set; }
    }
    public class UserPermissionsDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string EncryptedUserId { get; set; }
        public string RequestedUser { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
