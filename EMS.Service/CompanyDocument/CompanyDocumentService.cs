using System.Collections.Generic;
using System.Linq;
using EMS.Data;
using EMS.Repo;
using EMS.Dto;

namespace EMS.Service
{
    public class CompanyDocumentService : ICompanyDocumentService
    {
        #region "Fields"
        private IRepository<CompanyDocument> repoCompanyDocument;
        #endregion

        #region "Cosntructor"
        public CompanyDocumentService(IRepository<CompanyDocument> _repoCompanyDocument)
        {
            this.repoCompanyDocument = _repoCompanyDocument;
        }
        #endregion
        public CompanyDocument GetCompanyDocumentsByID(int CompanyDocumentId)
        {
            return repoCompanyDocument.Query().Filter(x => x.Id == CompanyDocumentId).Get().FirstOrDefault();
        }

        public List<CompanyDocument> GetCompanyDocuments()
        {
            return repoCompanyDocument.Query().Get().OrderBy(T => T.Heading).ThenBy(S => S.DocumentName).ToList();
        }

        public List<CompanyHeadingDto> GetCompanyDocumentsByRoles(bool isPM, int deptId)
        {
            if (isPM)
            {
                return repoCompanyDocument.Query().Get().GroupBy(x => x.Heading).Select(y => new CompanyHeadingDto
                {
                    Heading = y.Key,
                    Documents = y.Select(d => new CompanyDocumentDto
                    {
                        Id = d.Id,
                        DocumentName = d.DocumentName,
                        DocumentPath = d.DocumentPath,
                        DepartmentId = d.DepartmentId,
                        Department = d.Department !=null ? d.Department.Name : "",
                        DownloadLink=d.DocumentPath
                    }).OrderBy(T => T.Heading).ThenBy(x => x.DocumentName).ToList()
                }).ToList();
            }
            else
            {
                return repoCompanyDocument.Query().Filter(x => x.DepartmentId == deptId).Get().GroupBy(x => x.Heading).Select(y => new CompanyHeadingDto
                {
                    Heading = y.Key,
                    Documents = y.Select(d => new CompanyDocumentDto
                    {
                        Id = d.Id,
                        DocumentName = d.DocumentName,
                        DocumentPath = d.DocumentPath,
                        DepartmentId = d.DepartmentId,
                        Department=d.Department.Name,
                        DownloadLink = d.DocumentPath
                    }).ToList()
                }).ToList();
            }

            //if (isPM)
            //    return repoCompanyDocument.Query().Get().OrderBy(T => T.Heading).ThenBy(S => S.DocumentName).ToList();
            //else
            //    return repoCompanyDocument.Query().Filter(x => x.DepartmentId == deptId).Get().OrderBy(T => T.Heading).ThenBy(S => S.DocumentName).ToList();
        }
        public List<CompanyDocument> GetCompanyDocumentsByHeading(string heading)
        {
            return repoCompanyDocument.Query().Filter(x => x.Heading == heading).Get().OrderBy(S => S.Heading).ThenBy(x => x.DocumentName).ToList();
        }

        #region "Dispose"
        public void Dispose()
        {
            if (repoCompanyDocument != null)
            {
                repoCompanyDocument.Dispose();
                repoCompanyDocument = null;
            }
        }
        #endregion

    }
}
