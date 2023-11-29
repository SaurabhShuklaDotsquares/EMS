using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LibraryFileType
    {
        public LibraryFileType()
        {
            LibraryFile = new HashSet<LibraryFile>();
            LibraryDownloadHistory = new HashSet<LibraryDownloadHistory>();
            LibraryDownloadPermission = new HashSet<LibraryDownloadPermission>();
            LibraryComponentFile = new HashSet<LibraryComponentFile>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public bool IsActive { get; set; }
        public string Extension { get; set; }
        public string Icon { get; set; }

        public virtual ICollection<LibraryDownloadHistory> LibraryDownloadHistory { get; set; }
        public virtual ICollection<LibraryDownloadPermission> LibraryDownloadPermission { get; set; }
        public virtual ICollection<LibraryFile> LibraryFile { get; set; }
        public virtual ICollection<LibraryComponentFile> LibraryComponentFile { get; set; }
    }
}
