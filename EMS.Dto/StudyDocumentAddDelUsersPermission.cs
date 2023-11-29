using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class StudyDocumentAddDelUsersPermission
    {
        public string StudyDocumentIds { get; set; }
        public int UpdatedBy { get; set; }
        public string Ip { get; set; }
        public int[] UserId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool AddDelPermission { get; set; }
    }
}
