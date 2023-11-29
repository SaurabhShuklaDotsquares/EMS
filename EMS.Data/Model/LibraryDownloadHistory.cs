using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LibraryDownloadHistory
    {
        public long Id { get; set; }
        public long? LibraryId { get; set; }
        public int LibraryFileTypeId { get; set; }
        public long LibraryFileId { get; set; }
        public int DownloadBy { get; set; }
        public DateTime DownloadOn { get; set; }
        public string Ip { get; set; }

        public virtual UserLogin DownloadByNavigation { get; set; }
        public virtual Library Library { get; set; }
        public virtual LibraryFile LibraryFile { get; set; }
        public virtual LibraryFileType LibraryFileType { get; set; }
    }
}
