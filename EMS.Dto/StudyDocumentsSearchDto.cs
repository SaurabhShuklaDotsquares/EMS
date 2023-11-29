using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class StudyDocumentsSearchDto
    {
        public string SearchText { get; set; }
        public int[] TechnologyId { get; set; }
        public int PageNo { get; set; }
        public int DataLength { get; set; }
        public bool IsLoadMore { get; set; }
    }
}
