using System;
using System.Collections.Generic;
using EMS.Data;
using EMS.Dto;

namespace EMS.Service
{
    public interface ICompanyDocumentService : IDisposable
    {
        CompanyDocument GetCompanyDocumentsByID(int Id);
        List<CompanyDocument> GetCompanyDocuments();
        List<CompanyHeadingDto> GetCompanyDocumentsByRoles(bool isPM, int deptId);

        //List<CompanyDocument> GetCompanyDocumentsByID(int ID);

        List<CompanyDocument> GetCompanyDocumentsByHeading(string Heading);
    }
}



