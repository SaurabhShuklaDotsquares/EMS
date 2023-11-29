using System;
using System.Collections.Generic;
using EMS.Data;
using EMS.Data.Model;
using EMS.Dto;

namespace EMS.Service
{
    public interface IStudyDocumentsService : IDisposable
    {
        StudyDocumentsDto GetStudyDocumentsById(int id);
        List<StudyDocumentsDto> GetStudyDocuments();
        void SaveStudyDocuments(StudyDocumentsDto model);
        List<StudyDocumentFiles> UpdateStudyDocuments(StudyDocumentsDto model);

        StudyDocumentFilesDto GetStudyDocumentFilesById(int id);
        List<StudyDocumentFilesDto> GetStudyDocumentFilesByStudyDocumentsId(int id);
        List<StudyDocumentFilesDto> GetStudyDocumentFiles();
        void SaveStudyDocumentFiles(StudyDocumentFilesDto model);
        void UpdateStudyDocumentFiles(StudyDocumentFilesDto model);
        void SaveStudyDocumentFiles(List<StudyDocumentFilesDto> model);
        void ApproveStudyDocumentBySDId(int id, int uid, bool val);
        List<StudyDocuments> GetStudyDocuments(PagingService<StudyDocuments> pagingService, out int total);
        void DeleteStudyDocuments(string id, int uid);
        List<StudyDocuments> GetStudyDocumentsBySearchText(PagingService<StudyDocuments> pagingService, out int total);
        StudyDocuments GetStudyDocumentsByKeyId(string id);
        List<StudyDocuments> GetAllStudyDocumentsByKeyId(string id);
        StudyDocumentsPermissions GetStudyDocumentsPermissionsBySDId(int id, int uid);
        void SaveStudyDocumentsPermission(StudyDocumentsPermissions entity);
        void UpdateStudyDocumentsPermission(StudyDocumentsPermissions entity);
        void DeleteStudyDocumentsPermission(StudyDocumentsPermissions entity);
        List<StudyDocumentsPermissions> GetAllStudyDocumentsPermissionsBySDId(int id);
        void ApproveStudyDocumentsBySDIds(StudyDocumentsUnapprovedReasonDto model);
        void UpdateStudyDocumentsOnly(StudyDocumentsDto model);
        void UpdateStudyDocumentsUserPermission(StudyDocumentAddDelUsersPermission model);
        StudyDocuments UpdateStudyDocumentsOnlyByKeyId(StudyDocumentsDto model);
        StudyDocumentFiles DeleteStudyDocumentFile(string fileKeyId);
        StudyDocumentsDto GetStudyDocumentsAndFilesByKeyId(string keyId);
        List<int> GetUnApprovedStudyDocIds(string id);
        void InsertRequestedStudyDocument(RequestedStudyDocuments entity);
    }
}



