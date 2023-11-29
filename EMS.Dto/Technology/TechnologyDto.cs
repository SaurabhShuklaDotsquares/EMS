using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Dto
{
    public class TechnologyDto
    {
        public int TechId { get; set; }
        [DisplayName("Technology Name")]
        public string Title { get; set; }
        public string AddDate { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }
        public string ModifyDate { get; set; }
    }
}
