using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class StudyDocumentsUnapprovedReasonDto
    {
        public string StudyDocumentIds { get; set; }
        public string UnapprovedReason { get; set; }
        public bool IsApproved { get; set; }
        public string Ip { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; } 
    }
}
