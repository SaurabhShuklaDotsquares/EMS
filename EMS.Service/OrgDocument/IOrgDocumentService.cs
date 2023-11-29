using System;
using System.Collections.Generic;
using EMS.Data;
using EMS.Dto;

namespace EMS.Service
{
    public interface IOrgDocumentService : IDisposable
    {
        OrgDocument GetOrgDocumentById(int id);

       
        OrgDocument GetOrgDocumentByMasterId(int documentMasterId, int? exceptDocId = null);

        OrgDocument GetBaselineOrgDocumentByMasterId(int documentMasterId);

        List<CompanyDocumentDto> GetOrgDocumentHistoryByMasterId(int documentMasterId);

        List<OrgDocument> GetOrgDocuments();

        List<CompanyHeadingDto> GetOrgDocumentsByRoles(bool IsAshishTeamPMUId, int deptId, int roleId, int[] DocumentForAshishTeamOnly);

        List<OrgDocument> GetOrgDocumentsByHeading(string Heading);

        List<OrgDocumentMaster> GetOrgDocumentMasters(byte docType);

        OrgDocumentMaster GetOrgDocumentMasterById(int id);

        OrgDocument Save(OrgDocumentDto model);

        void Update(OrgDocument entity, int[] roleIds, int[] departmentIds);

        OrgDocument UpdateApproveStatus(OrgDocumentApproveDto model);

        List<OrgDocument> GetOrgDocByPaging(out int total, PagingService<OrgDocument> pagingService);
    }
}



