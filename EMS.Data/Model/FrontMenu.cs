using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class FrontMenu
    {
        public FrontMenu()
        {
            MenuAccesses = new HashSet<MenuAccess>();
        }

        public int MenuId { get; set; }
        public int? ParentId { get; set; }
        public string MenuName { get; set; }
        public string MenuDisplayName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateModify { get; set; }
        public string Ip { get; set; }
        public string PageName { get; set; }
        public string ChildPages { get; set; }
        public int MenuOrder { get; set; }
        public byte? NotificationFor { get; set; }
        public string UserIds { get; set; }

        public virtual ICollection<MenuAccess> MenuAccesses { get; set; }
    }
}
