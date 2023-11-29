using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Dto
{
    public class AssignDeviceDto
    {
        public AssignDeviceDto()
        {
            DeviceTypeList = new List<SelectListItem>();
            DeviceNameList = new List<SelectListItem>();
            AssignedUserList = new List<SelectListItem>();
        }
        
        public int Id { get; set; }

        [DisplayName("Device")]
        public int DeviceId { get; set; }

        public string Condition { get; set; }

        [DisplayName("Assigned To")]
        public int AssignedToUid { get; set; }

        [DisplayName("Assigned Date")]
        public string AssignedDateTime { get; set; }
        
        public int CreateByUid { get; set; }
        
        public int ModifyByUid { get; set; }

        [DisplayName("Serial Number(S/N:)")]
        public string SerialNumber { get; set; }        

        [DisplayName("Device Type")]
        public byte DeviceType { get; set; }

        public List<SelectListItem> DeviceTypeList { get; set; }

        public List<SelectListItem> DeviceNameList { get; set; }

        public List<SelectListItem> AssignedUserList { get; set; }
    }

    public class AssignedHistoryIndexDto
    {
        public AssignedHistoryIndexDto()
        {
            DeviceTypeList = new List<SelectListItem>();
            UserList = new List<SelectListItem>();
        }
        public List<SelectListItem> UserList { get; set; }
        public List<SelectListItem> DeviceTypeList { get; set; }
        public bool AllowManage { get; set; }

    }
}
