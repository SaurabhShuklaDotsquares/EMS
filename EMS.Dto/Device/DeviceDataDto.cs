using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
    public class DeviceDataDto
    {
        public DeviceDataDto()
        {
            DeviceTypeList = new List<SelectListItem>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public short? Quantity { get; set; }

        [DisplayName("Description")]
        public string Condition { get; set; }     
        
        [DisplayName("Type")]
        public byte DeviceType { get; set; }

        [DisplayName("SIM Number")]
        public string SimNumber { get; set; }

        [DisplayName("SIM Network")]
        public string SimNetwork { get; set; }

        public int CurrentUserId { get; set; }
        public int PMUid { get; set; }

        public int AssignedQuantity { get; set; }

        public List<SelectListItem> DeviceTypeList { get; set; }
        
    }
}
