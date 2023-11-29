using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class StudyDocumentsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;
        public bool IsApproved { get; set; }
        public string Ip { get; set; }
        public string KeyId { get; set; } = Guid.NewGuid().ToString();
        public int AddedBy { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public int TechnologyId { get; set; }
        public List<StudyDocumentFilesDto> studyDocumentFiles { get; set; } = new List<StudyDocumentFilesDto>();
    }
}
