using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class DeviceDeviceInfo
    {
        public DeviceDeviceInfo()
        {
            DeviceDeviceHistory = new HashSet<DeviceDeviceHistory>();
        }

        public int DeviceDeviceInfoId { get; set; }
        public int? DeviceCategoryId { get; set; }
        public string DeviceDeviceInfoName { get; set; }
        public int? Quantity { get; set; }
        public int PmId { get; set; }

        public virtual DeviceCategory DeviceCategory { get; set; }
        public virtual UserLogin Pm { get; set; }
        public virtual ICollection<DeviceDeviceHistory> DeviceDeviceHistory { get; set; }
    }
}
