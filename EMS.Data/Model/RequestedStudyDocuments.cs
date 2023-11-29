using EMS.Data.Model;
using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class RequestedStudyDocuments
    {
        public int Id { get; set; }
        public int Studydocumentid { get; set; }
        public int Requestedby { get; set; }
        public DateTime? Addeddate { get; set; }
        public string Ip { get; set; }

        public virtual UserLogin RequestedbyNavigation { get; set; }
        public virtual StudyDocuments Studydocument { get; set; }
    }
}
