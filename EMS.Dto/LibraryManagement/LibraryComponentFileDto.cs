using EMS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class LibraryComponentFileDto
    {
        public long Id { get; set; }
        public string PSDFilePath { get; set; }
        public long LibraryId { get; set; }
        public int LibraryComponentFileTypeId { get; set; }
        public string FileName { get; set; }
        public string FileShortName { get; set; }
        public string PsdFileShortName { get; set; }
        public bool DesignUnitType { get; set; }        
        public string FilePath { get; set; }
        public string FileImage { get; set; }
        public string FileTypeName { get; set; }

    }
}
