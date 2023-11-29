using System.Collections.Generic;
using System.Linq;
using EMS.Data;
using EMS.Repo;
using EMS.Dto;
using System;
using EMS.Core;

namespace EMS.Service
{
    public class OrgDocumentService : IOrgDocumentService
    {
        #region "Fields"

        private readonly IRepository<OrgDocument> repoOrgDocument;
        private readonly IRepository<OrgDocumentMaster> repoOrgDocumentMaster;
        private readonly IRepository<UserLogin> repoUserLogin;
        private readonly IRepository<Department> repoDepartment;
        private readonly IRepository<Role> repoRole;

        #endregion

        #region "Cosntructor"

        public OrgDocumentService(IRepository<OrgDocument> _repoOrgDocument,
            IRepository<OrgDocumentMaster> _repoOrgDocumentMaster,
            IRepository<UserLogin> _repoUserLogin,
            IRepository<Department> _repoDepartment,
            IRepository<Role> _repoRole)
        {
            repoOrgDocument = _repoOrgDocument;
            repoOrgDocumentMaster = _repoOrgDocumentMaster;
            repoUserLogin = _repoUserLogin;
            repoDepartment = _repoDepartment;
            repoRole = _repoRole;
        }

        #endregion

        public OrgDocument GetOrgDocumentById(int id)
        {
            return repoOrgDocument.FindById(id);
        }

        private OrgDocument GetOrgDocById(int id)
        {
            return repoOrgDocument.Query().Filter(x => x.Id == id).GetQuerable().FirstOrDefault();
        }

        public OrgDocument GetOrgDocumentByMasterId(int documentMasterId, int? exceptDocId = null)
        {
            return repoOrgDocument.Query()
                    .Filter(x => x.OrgDocumentMasterId == documentMasterId && (!exceptDocId.HasValue || exceptDocId.Value == 0 || x.Id != exceptDocId.Value))
                    .OrderBy(o => o.OrderByDescending(x => x.Id))
                    .GetQuerable().FirstOrDefault();
        }

        public OrgDocument GetBaselineOrgDocumentByMasterId(int documentMasterId)
        {
            return repoOrgDocument.Query().AsTracking()
                    .Filter(x => x.OrgDocumentMasterId == documentMasterId && x.IsApproved && x.IsBaseline)
                    .GetQuerable().FirstOrDefault();
        }

        public List<CompanyDocumentDto> GetOrgDocumentHistoryByMasterId(int documentMasterId)
        {
            return repoOrgDocument.Query()
                    .Filter(x => x.OrgDocumentMasterId == documentMasterId && x.IsApproved && !x.IsBaseline)
                    .OrderBy(o => o.OrderByDescending(x => x.Id))
                    .Get().Select(d => new CompanyDocumentDto
                    {
                        Id = d.Id,
                        DocumentName = string.Format("{0} v{1}", d.OrgDocumentMaster.Name, d.Ver),
                        DocumentPath = d.DocumentPath,
                        ApprovedOn = d.ApprovedDate.ToFormatDateString("dd MMM yyyy, hh:mm tt")
                    }).OrderBy(T => T.Heading).ThenBy(x => x.DocumentName).ToList();
        }

        public List<OrgDocument> GetOrgDocuments()
        {
            return repoOrgDocument.Query().Get().ToList();
        }

        public List<CompanyHeadingDto> GetOrgDocumentsByRoles(bool IsAshishTeamPMUId, int deptId, int roleId, int[] DocumentForAshishTeamOnly)
        {
            //Enums.PMPrefernceKeys.IsHandoverDocumentTemplateVisible.GetDescription()
            return repoOrgDocument.Query().AsTracking().Filter(x => x.IsBaseline && x.IsApproved &&
                    (x.OrgDocumentDepartment.Any(m => m.DepartmentId == deptId) || x.OrgDocumentRole.Any(m => m.RoleId == roleId))
                    &&
                    ( !DocumentForAshishTeamOnly.Contains(x.OrgDocumentMasterId) ||
                        (DocumentForAshishTeamOnly.Contains(x.OrgDocumentMasterId) && IsAshishTeamPMUId))
                    )
                .Get().GroupBy(x => x.OrgDocumentMaster.DocType)
                .Select(y => new CompanyHeadingDto
                {
                    Heading = ((Enums.OrgDocumentType)y.Key).GetDescription(),
                    Documents = y.Select(d => new CompanyDocumentDto
                    {
                        Id = d.Id,
                        DocumentName = string.Format("{0} v{1}", d.OrgDocumentMaster.Name, d.Ver),
                        DocumentPath = d.DocumentPath,
                        DocumentMasterId = d.OrgDocumentMasterId,
                        HasHistory = d.OrgDocumentMaster.OrgDocuments.Any(x=> x.IsApproved && !x.IsBaseline),
                        /*Changed By Tabassum */
                        //Department = d.Departments.Any() ? string.Join(", ", d.Departments.Select(dp => dp.Name)) : "",
                        Department = d.OrgDocumentDepartment.Any() ? string.Join(", ", d.OrgDocumentDepartment.Select(dp => dp.Department.Name)) : "",
                        /*END*/
                        DownloadLink = d.DocumentPath,
                        ApprovedOn = d.ApprovedDate.ToFormatDateString("dd MMM yyyy, hh:mm tt"),
                        ApprovedBy = "SEPG Team",//d.OrgDocumentApproves.Any() ? string.Join(", ", d.OrgDocumentApproves.Select(o => o.UserLogin.Name)) : "",
                         /*Changed By Tabassum */
                        //Roles = d.Roles.Any() ? string.Join(", ", d.Roles.Select(r => r.RoleName)) : ""
                        Roles = d.OrgDocumentRole.Any() ? string.Join(", ", d.OrgDocumentRole.Select(r => r.Role.RoleName)) : ""
                        /*ENd*/

                    }).OrderBy(T => T.Heading).ThenBy(x => x.DocumentName).ToList()
                }).ToList();
        }

        public List<OrgDocument> GetOrgDocumentsByHeading(string Heading)
        {
            throw new NotImplementedException();
        }

        public List<OrgDocumentMaster> GetOrgDocumentMasters(byte docType)
        {
            return repoOrgDocumentMaster.Query()
                .Filter(x => x.DocType == docType && x.IsActive)
                .OrderBy(o => o.OrderBy(x => x.Name))
                .Get().ToList();
        }

        public OrgDocumentMaster GetOrgDocumentMasterById(int id)
        {
            return repoOrgDocumentMaster.FindById(id);
        }

        public OrgDocument Save(OrgDocumentDto model)
        {
            OrgDocument docEntity = null;
            var currentDateTime = DateTime.Now;
            if (model.Id == 0)
            {
                docEntity = new OrgDocument
                {
                    OrgDocumentMasterId = model.OrgDocumentMasterId,
                    DocumentPath = model.DocumentPath,
                    Ver = model.Ver,
                    IsSendEmail = model.IsSendEmail,
                    HighLevelChanges = model.HighLevelChanges,
                    CreateByUid = model.CreateByUid,
                    ModifyByUid = model.CreateByUid,
                    CreateDate = currentDateTime,
                    ModifyDate = currentDateTime
                };

                var approvers = repoUserLogin.Query().Filter(x => x.IsActive == true && x.IsSPEG).GetQuerable().Select(x => x.Uid);

                if (approvers.Any())
                {
                    foreach (var uid in approvers)
                    {
                        docEntity.OrgDocumentApproves.Add(new OrgDocumentApprove
                        {
                            ApproverUid = uid
                        });
                    }
                }

                if (model.DepartmentIds != null && model.DepartmentIds.Any())
                {
                    /*Changed By Tabassum */
                    // Array.ForEach(model.DepartmentIds, x => docEntity.Departments.Add(repoDepartment.FindById(x)));
                    Array.ForEach(model.DepartmentIds, x => docEntity.OrgDocumentDepartment.Add(new OrgDocumentDepartment { Department= repoDepartment.FindById(x)}));
                    /*End*/
                }

                if (model.RoleIds != null && model.RoleIds.Any())
                {
                    /*Changed By Tabassum */
                    //Array.ForEach(model.RoleIds, x => docEntity.Roles.Add(repoRole.FindById(x)));
                    Array.ForEach(model.RoleIds, x => docEntity.OrgDocumentRole.Add(new OrgDocumentRole{ Role  =repoRole.FindById(x)}));
                    /*End*/
                }

                repoOrgDocument.InsertGraph(docEntity);

                return GetOrgDocById(docEntity.Id);
            }
            else
            {
                docEntity = repoOrgDocument.FindById(model.Id);

                if (docEntity != null)
                {
                    /*Changed By Tabassum */
                    //docEntity.Departments.Clear();
                    //docEntity.Roles.Clear();
                    docEntity.OrgDocumentDepartment.Clear();
                    docEntity.OrgDocumentRole.Clear();
                    /*End*/
                    if (model.DepartmentIds != null && model.DepartmentIds.Any())
                    { 
                        /*Changed By Tabassum */
                        // Array.ForEach(model.DepartmentIds, x => docEntity.Departments.Add(repoDepartment.FindById(x)));
                        Array.ForEach(model.DepartmentIds, x => docEntity.OrgDocumentDepartment.Add(new OrgDocumentDepartment{Department= repoDepartment.FindById(x)}));
                        /*End*/
                    }

                    if (model.RoleIds != null && model.RoleIds.Any())
                    {
                        /*Changed By Tabassum */
                        //Array.ForEach(model.RoleIds, x => docEntity.Roles.Add(repoRole.FindById(x)));
                        Array.ForEach(model.RoleIds, x => docEntity.OrgDocumentRole.Add(new OrgDocumentRole{Role= repoRole.FindById(x)}));
                        /*End*/
                    }

                    repoOrgDocument.SaveChanges();
                }
            }

            return docEntity;
        }

        public void Update(OrgDocument entity, int[] roleIds, int[] departmentIds)
        {
            /*Changed By Tabassum */
            //entity.Departments.Clear();
            //entity.Roles.Clear();
            entity.OrgDocumentDepartment.Clear();
            entity.OrgDocumentRole.Clear();
            /*End*/

            if (departmentIds != null && departmentIds.Any())
            {
                /*Changed By Tabassum */
                // Array.ForEach(departmentIds, x => entity.Departments.Add(repoDepartment.FindById(x)));
                Array.ForEach(departmentIds, x => entity.OrgDocumentDepartment.Add(new OrgDocumentDepartment { Department= repoDepartment.FindById(x) }));
                /*End*/
            }

            if (roleIds != null && roleIds.Any())
            {
                /*Changed By Tabassum */
                //Array.ForEach(roleIds, x => entity.Roles.Add(repoRole.FindById(x)));
                Array.ForEach(roleIds, x => entity.OrgDocumentRole.Add(new OrgDocumentRole {Role=repoRole.FindById(x) }));
                /*End*/
            }

            repoOrgDocument.Update(entity);
        }

        public OrgDocument UpdateApproveStatus(OrgDocumentApproveDto model)
        {
            var docEntity = GetOrgDocumentById(model.OrgDocId);
            if (docEntity != null && docEntity.OrgDocumentApproves.Any(x => x.ApproverUid == model.ApproverUid && !x.IsApproved))
            {
                var currentDateTime = DateTime.Now;
                var approver = docEntity.OrgDocumentApproves.FirstOrDefault(x => x.ApproverUid == model.ApproverUid);
                approver.Comments = model.Comments;
                approver.ApprovedDate = currentDateTime;
                approver.IsApproved = true;

                if (!docEntity.OrgDocumentApproves.Any(x => !x.IsApproved))
                {
                    var baselineDoc = GetBaselineOrgDocumentByMasterId(docEntity.OrgDocumentMasterId);

                    if (baselineDoc != null)
                    {
                        baselineDoc.IsBaseline = false;
                    }

                    docEntity.IsBaseline = true;
                    docEntity.IsApproved = true;
                    docEntity.ApprovedDate = currentDateTime;
                }

                repoOrgDocument.SaveChanges();
            }
            else
            {
                return null;
            }
            return docEntity;
        }

        public List<OrgDocument> GetOrgDocByPaging(out int total, PagingService<OrgDocument> pagingService)
        {
            return repoOrgDocument.Query().Filter(pagingService.Filter).
                OrderBy(pagingService.Sort).
                GetPage(pagingService.Start, pagingService.Length, out total).
                ToList();
        }

        #region "Dispose"

        public void Dispose()
        {
            if (repoOrgDocument != null)
            {
                repoOrgDocument.Dispose();
            }
            if (repoOrgDocumentMaster != null)
            {
                repoOrgDocumentMaster.Dispose();
            }
        }


        #endregion
    }
}
