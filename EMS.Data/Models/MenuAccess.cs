using System;
using System.Collections.Generic;

namespace EMS.Data.Models
{
    public partial class MenuAccess
    {
        public int AccessId { get; set; }
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public int? DesignationId { get; set; }
        public int? RoleCategoryId { get; set; }
    }
}
