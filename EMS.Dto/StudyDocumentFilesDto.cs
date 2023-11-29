using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class StudyDocumentFilesDto
    {
        public int Id { get; set; }
        public int StudyDodumentId { get; set; }
        public string KeyId { get; set; } = Guid.NewGuid().ToString();
        public string FileName { get; set; }
        public string DisplayName { get; set; }
    }
}
