using EMS.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class SmeDto
    {
        public int Id { get; set; }
        public string SubjectMatterExpert { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public int? Level1 { get; set; }
        public int? Level2 { get; set; }
        public int? Level3 { get; set; }
        public int? Level4 { get; set; }
        public int? Level5 { get; set; }


        public List<SelectListItem> Level1Options { get; set; }


        public List<SelectListItem> Level2Options { get; set; }
        public List<SelectListItem> Level3Options { get; set; }
        public List<SelectListItem> Level4Options { get; set; }
        public List<SelectListItem> Level5Options { get; set; }
        public List<string> datalist { get; set; }
    }
}
