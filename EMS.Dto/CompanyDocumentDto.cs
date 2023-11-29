using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Dto
{
    public class CompanyHeadingDto
    {
        public string Heading { get; set; }
        public int DocumentMasterId { get; set; }
        public List<CompanyDocumentDto> Documents { get; set; }
    }

    public class CompanyDocumentDto
    {
        public int Id { get; set; }
        public string Heading { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public string Department { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime Modified { get; set; }
        public string DownloadLink { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovedOn { get; set; } 
        public string Roles { get; set; }
        public int DocumentMasterId { get; set; }
        public bool HasHistory { get; set; }
    }

    
}
