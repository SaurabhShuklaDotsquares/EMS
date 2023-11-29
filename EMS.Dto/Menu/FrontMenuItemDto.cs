using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Dto
{
   public class FrontMenuItemDto
    {
        public int MenuId { get; set; }
        public int? ParentId { get; set; }
        public string MenuDisplayName { get; set; }
        public string PageName { get; set; }
        public byte ? NotificationFor { get; set; }
        public int NotificationCount { get; set; }
    }
}
