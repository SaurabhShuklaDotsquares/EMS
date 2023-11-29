using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace EMS.Dto
{
    public class VirtualDeveloperDto
    {
        public int VirtualDeveloper_Id { get; set; }

        [DisplayName("Developer Name*")]
        public string VirtualDeveloper_Name { get; set; }

        [DisplayName("Skype Id")]
        public string Skype_Id { get; set; }

        [DisplayName("Active")]
        public bool IsActive { get; set; }

        [DisplayName("Email Address*")]
        public string Email { get; set; }
        public bool IsMain { get; set; }

        [DisplayName("Select PM*")]
        public int PMUid { get; set; }

        public List<SelectListItem> PMList { get; set; }

        public VirtualDeveloperDto()
        {
            PMList = new List<SelectListItem>();
        }
    }
}
