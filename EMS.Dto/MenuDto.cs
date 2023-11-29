using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Dto
{
    public class MenuDto
    {
        public int MenuId { get; set; }
        public int ParentId { get; set; }
        public string MenuName { get; set; }
        public string MenuDisplayName { get; set; }
        public bool IsActive { get; set; }
        public string PageName { get; set; }
    }
}
