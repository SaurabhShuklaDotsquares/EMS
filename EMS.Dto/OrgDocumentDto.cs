using EMS.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace EMS.Dto
{
    public class OrgDocumentDto
    {
        public OrgDocumentDto()
        {
            DocumentTypeList = new List<SelectListItem>();
            OrgDocumentMasterList = new List<SelectListItem>();
        }

        public int Id { get; set; }

        [DisplayName("Document Type")]
        public byte DocType { get; set; }

        public string DocTypeName { get; set; }

        [DisplayName("Document")]
        public int OrgDocumentMasterId { get; set; }

        public string OrgDocumentMasterName { get; set; }

        public string OrgDocumentName { get; set; }

        [DisplayName("Upload Document")]
        public IFormFile Document { get; set; }
        public string DocumentPath { get; set; }
        
        public string Ver { get; set; }

        [DisplayName("Is Major changes in Doc?")]
        public bool IsMajorVer { get; set; }

        public bool IsBaseline { get; set; }
        public bool IsApproved { get; set; }

        [DisplayName("Send Email?")]
        public bool IsSendEmail { get; set; }

        [DisplayName("High level changes (Optional)")]
        // [AllowHtml] ** no need ot AllowHTML in .net Core https://github.com/aspnet/Mvc/issues/324#issuecomment-50082022**
        public string HighLevelChanges { get; set; }

        public bool AllowEdit { get; set; }

        public string CreateDate { get; set; }
        public string ApprovedDate { get; set; }

        public string CreateBy { get; set; }
        public int CreateByUid { get; set; }

        public int[] RoleIds { get; set; }
        public int[] DepartmentIds { get; set; }
        
        public List<SelectListItem> DocumentTypeList { get; set; }
        public List<SelectListItem> OrgDocumentMasterList { get; set; }
        public List<SelectListItem> DepartmentList { get; set; }
        public List<SelectListItem> RoleList { get; set; }
    }

    public class OrgDocumentApproveDto
    {
        public int OrgDocId { get; set; }
        public DateTime ApprovedDate { get; set; }
        public int ApproverUid { get; set; }
        public bool isApproved { get; set; }
        public string Comments { get; set; }
    }
}
