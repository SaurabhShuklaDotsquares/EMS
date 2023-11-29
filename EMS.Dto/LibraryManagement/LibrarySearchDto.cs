using System;
using System.Collections.Generic;
using System.Text;
using EMS.Core;

namespace EMS.Dto
{
    public class LibrarySearchDto
    {
        public LibrarySearchDto()
        {
               SearchId= Guid.NewGuid();
        }
        //public string SearchText { get; set; }
        public string Keyword { get; set; }
        public Enums.LibraryType LibraryType { get; set; }

        public int[] Technologies { get; set; }
        public int[] Domains { get; set; }
        public bool IsAdvanceSearch { get; set; }
        public bool? IsNDA { get; set; }
        public bool? Featured { get; set; }
        public bool? IsReadyToUse { get; set; }

        public string IP { get; set; }

        public Guid SearchId { get; set; }
        public Guid ?KeyId { get; set; }
        public Enums.DesignType? DesignType { get; set; }
        public int[] Layouts { get; set; }
        public int[] Components { get; set; }
        public int[] Templates { get; set; }
    }
}
