using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EMS.Data
{
    public partial class LibraryDownloadPermission
    {
        public long Id { get; set; }
        public int? RoleId { get; set; }
        public int LibraryFileTypeId { get; set; }
        public int? MaximumDownloadInDay { get; set; }
        public int? MaximumDownloadInMonth { get; set; }
        public int? UserLoginId { get; set; }
        public int AllowedDownloadBy { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual UserLogin AllowedDownloadByNavigation { get; set; }
        public virtual LibraryFileType LibraryFileType { get; set; }
        public virtual Role Role { get; set; }
        public virtual UserLogin UserLogin { get; set; }
    }
}
