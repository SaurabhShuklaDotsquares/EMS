using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class StudyDocumentsQuickViewDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string KeyId { get; set; }        
        public string Technology { get; set; }        
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public bool AddDelPermission { get; set; }
        public List<FilesQuickViewDto> studyDocumentFiles { get; set; } = new List<FilesQuickViewDto>();
        public List<UserPermissionsQuickViewDto> userPermissions { get; set; } = new List<UserPermissionsQuickViewDto>();
    }
    public class FilesQuickViewDto
    {
        public string KeyId { get; set; }
        public string FileName { get; set; }
        public string DisplayName { get; set; }
    }
    public class UserPermissionsQuickViewDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string EncryptedUserId { get; set; }
        public string UserName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
