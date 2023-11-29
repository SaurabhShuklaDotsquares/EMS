using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class StudyDocumentsSearchTextResultDto
    {
        public int TotalRecords { get; set; }
        public bool IsNotLastRecords { get; set; }
        public bool IsLoadMore { get; set; }
        public List<StudyDocumentsSearchResultDto> studyDocumentsSearchResultDtos { get; set; } = new List<StudyDocumentsSearchResultDto>();
    }
    public class StudyDocumentsSearchResultDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string KeyId { get; set; } 
        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string AddedDate { get; set; }
        public string UpdatedDate { get; set; }
        public bool IsApproved { get; set; }
        public bool HasPermission { get; set; } // permission
        public int DocumentCount { get; set; }
        public string Technology { get; set; }
        public List<StudyDocumentFilesSearchResultDto> studyDocumentFiles { get; set; } = new List<StudyDocumentFilesSearchResultDto>();
        public List<UserPermissionsSearchResultDto> userPermissions { get; set; } = new List<UserPermissionsSearchResultDto>();
        public bool IsExtend { get; set; } // permission
    }
    public class StudyDocumentFilesSearchResultDto
    {
        public string KeyId { get; set; } 
        public string FileName { get; set; }
        public string DisplayName { get; set; }
    }
    public class UserPermissionsSearchResultDto
    {
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
