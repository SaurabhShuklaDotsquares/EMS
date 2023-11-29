using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
    public class ReturnDeviceDto
    {
        public ReturnDeviceDto()
        {
            ReturnedUserList = new List<SelectListItem>();
        }

        public int DeviceDetailId { get; set; }

        [DisplayName("Received By")]
        public int ReturnToUid { get; set; }

        [DisplayName("Received Date")]
        public string ReturnDate { get; set; }

        public string AssignedDate { get; set; }

        public List<SelectListItem> ReturnedUserList { get; set; }
    }
}
