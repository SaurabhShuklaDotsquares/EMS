using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class DeviceCategory
    {
        public DeviceCategory()
        {
            DeviceDeviceInfo = new HashSet<DeviceDeviceInfo>();
        }

        public int DeviceCategoryId { get; set; }
        public string DeviceCategoryname { get; set; }

        public virtual ICollection<DeviceDeviceInfo> DeviceDeviceInfo { get; set; }
    }
}
