using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class MenuAccess
    {
        public int AccessId { get; set; }
        public int RoleId { get; set; }
        public int MenuId { get; set; }

        public int? DesignationId { get; set; }
        public int? RoleCategoryId { get; set; }
        public virtual FrontMenu FrontMenu { get; set; }
        public virtual Role Role { get; set; }
        public virtual Designation Designation { get; set; }
        
    }
}
