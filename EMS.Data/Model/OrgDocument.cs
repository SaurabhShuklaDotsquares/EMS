using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class OrgDocument
    {
        public OrgDocument()
        {
            OrgDocumentApproves = new HashSet<OrgDocumentApprove>();
            OrgDocumentDepartment = new HashSet<OrgDocumentDepartment>();
            OrgDocumentRole = new HashSet<OrgDocumentRole>();
        }

        public int Id { get; set; }
        public int OrgDocumentMasterId { get; set; }
        public string DocumentPath { get; set; }
        public string Ver { get; set; }
        public bool IsMajorVer { get; set; }
        public bool IsBaseline { get; set; }
        public bool IsApproved { get; set; }
        public bool IsSendEmail { get; set; }
        public string HighLevelChanges { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateByUid { get; set; }
        public DateTime ModifyDate { get; set; }
        public int ModifyByUid { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public virtual UserLogin UserLogin { get; set; }
        public virtual UserLogin ModifyByU { get; set; }
        public virtual OrgDocumentMaster OrgDocumentMaster { get; set; }
        public virtual ICollection<OrgDocumentApprove> OrgDocumentApproves { get; set; }
        public virtual ICollection<OrgDocumentDepartment> OrgDocumentDepartment { get; set; }
        public virtual ICollection<OrgDocumentRole> OrgDocumentRole { get; set; }
    }
}
