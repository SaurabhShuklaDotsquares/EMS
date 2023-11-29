using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class StudyDocumentFiles
    {
        public int Id { get; set; }
        public string Filename { get; set; }        
        public string Displayname { get; set; }        
        public int? Studydocumentid { get; set; }
        public string Keyid { get; set; }

        public virtual StudyDocuments Studydocument { get; set; }
    }
}
