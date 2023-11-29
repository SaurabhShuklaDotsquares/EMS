using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.API.Model
{
    public class ProjectStatusDto
    {
        public int CRMID { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
    }
}
