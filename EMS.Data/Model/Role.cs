using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Role
    {
        public Role()
        {
            EmployeeProject = new HashSet<EmployeeProject>();
            MenuAccessNavigation = new HashSet<MenuAccess>();
            OrgDocumentRole = new HashSet<OrgDocumentRole>();
            UserLogins = new HashSet<UserLogin>();
            LibraryDownloadPermission = new HashSet<LibraryDownloadPermission>();
            Designation = new HashSet<Designation>();
        }

        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string MenuAccess { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? AddDate { get; set; }

        public int? RoleCategoryId { get; set; }

        public virtual ICollection<EmployeeProject> EmployeeProject { get; set; }
        public virtual ICollection<MenuAccess> MenuAccessNavigation { get; set; }
        public virtual ICollection<OrgDocumentRole> OrgDocumentRole { get; set; }
        public virtual ICollection<UserLogin> UserLogins { get; set; }
        public virtual ICollection<LibraryDownloadPermission> LibraryDownloadPermission { get; set; }

        public virtual RoleCategory RoleCategory { get; set; }
        public virtual ICollection<Designation> Designation { get; set; }
       
    }
}
