using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
   public class TeamStructureDto
    {
        public TeamStructureDto()
        {
            UserslList = new List<SelectListItem>();
        }
        public string Name { get; set; }
        public int UserId { get; set; }

        public bool IsDropDownVisible { get; set; }

        public List<SelectListItem> UserslList { get; set; }
    }
}
