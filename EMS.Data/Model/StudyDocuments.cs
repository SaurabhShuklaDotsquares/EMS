using EMS.Data;
using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class StudyDocuments
    {
        public StudyDocuments()
        {
            StudyDocumentFiles = new HashSet<StudyDocumentFiles>();
            StudyDocumentsPermissions = new HashSet<StudyDocumentsPermissions>();
            RequestedStudyDocuments = new HashSet<RequestedStudyDocuments>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Addedby { get; set; }
        public int Updatedby { get; set; }
        public DateTime? Addeddate { get; set; }
        public DateTime? Updateddate { get; set; }
        public bool? Isactive { get; set; }
        public bool? Isdelete { get; set; }
        public bool? Isapproved { get; set; }
        public string Ip { get; set; }
        public string Keyid { get; set; }
        public string Unapprovedreson { get; set; }
        public int? Technologyid { get; set; }

        public virtual UserLogin AddedbyNavigation { get; set; }
        public virtual UserLogin UpdatedbyNavigation { get; set; }
        public virtual Technology Technology { get; set; }
        public virtual ICollection<StudyDocumentFiles> StudyDocumentFiles { get; set; }
        public virtual ICollection<StudyDocumentsPermissions> StudyDocumentsPermissions { get; set; }
        public virtual ICollection<RequestedStudyDocuments> RequestedStudyDocuments { get; set; }
    }
}
