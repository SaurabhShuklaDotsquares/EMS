using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto.SME
{
    public class SMEReportModel
    {
        public string Subject { get; set; }
        public bool IsActive { get; set; }
        public int? Expert1 { get; set; }
        public int? Expert2 { get; set; }
        public int? Expert3 { get; set; }
        public int? Expert4 { get; set; }
        public int? Expert5 { get; set; }
    }
}
