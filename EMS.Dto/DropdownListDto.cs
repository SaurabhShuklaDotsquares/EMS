using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Dto
{
 
    /// <summary>
    /// This class can be used by any one how need to fill dynamic dropdown data type
    /// </summary>
    public  class DropdownListDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }          
        public bool Selected { get; set; }

    }
}
