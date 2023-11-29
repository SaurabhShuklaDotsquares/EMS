using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace EMS.Dto
{
    public class ShowPasswordDto
    {
        public int UserId { get; set; }
        public string OriginalPassword { get; set; }
    }
}
