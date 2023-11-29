using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
    public class BugStatusDto : BugReportDto
    {        
        public string Comment { get; set; }

        [DisplayName("Status")]
        public int StatusId { get; set; }
       
        public BugStatusDto()
        {
            
        }
    }
}
