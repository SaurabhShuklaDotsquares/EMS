using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class StudyDocumentsPermissions
    {
        public int Id { get; set; }
        public int Studydocumentsid { get; set; }
        public int Userid { get; set; }
        public DateTime? Startdate { get; set; }
        public DateTime? Enddate { get; set; }

        public virtual StudyDocuments Studydocuments { get; set; }
        public virtual UserLogin User { get; set; }
    }
}
