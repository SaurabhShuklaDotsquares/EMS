using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class LibraryComponentFile
    {
        public long Id { get; set; }
        public long LibraryId { get; set; }
        public int LibraryComponentFileTypeId { get; set; }
        public string FilePath { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public long SrNo { get; set; }
        public string FileId { get; set; }
        public Guid KeyId { get; set; }
        public string PsdfilePath { get; set; }
        public bool? DesignUnitType { get; set; }

        public virtual Library Library { get; set; }
        public virtual LibraryFileType LibraryFileType { get; set; }
    }
}
