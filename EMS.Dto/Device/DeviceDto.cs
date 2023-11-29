using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
    public class DeviceDto
    {
        public DeviceDto()
        {           
            DeviceStatus = new List<SelectListItem>();
            UserList = new List<SelectListItem>();
            DeviceList = new List<SelectListItem>();
            AccessoryList = new List<SelectListItem>();
            SimList = new List<SelectListItem>();
        }
        [DisplayName("Device")]
        public int? DeviceId { get; set; }
        public List<SelectListItem> DeviceList { get; set; }
        public int[] Accessory { get; set; }
        public List<SelectListItem> AccessoryList { get; set; }
        public int? SimId { get; set; }
        public List<SelectListItem> SimList { get; set; }
        public byte DeviceStatusId { get; set; }
        public List<SelectListItem> DeviceStatus { get; set; }
        public int AssignBy { get; set; }
        
        public string PreviousUser { get; set; }
        public string Description { get; set; }
        public int AssignUid { get; set; }
        public int Id { get; set; }
        public string StartTime { get; set; }

        [DisplayName("User")]
        public List<SelectListItem> UserList { get; set; }
    }

    public class DeviceStatusDto
    {
        public DeviceStatusDto()
        {
            UserList = new List<SelectListItem>();
        }
        public int Id { get; set; }
        public int SubmitedUserId { get; set; }
        public string Device { get; set; }
        public string UserName { get; set; }
        public List<SelectListItem> UserList { get; set; }
        public int SubmittedBy { get; set; }
    }
}
