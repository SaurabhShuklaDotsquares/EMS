using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class DeviceAccessoriesMap
    {
        public int DeviceDetailId { get; set; }
        public int AccessoryId { get; set; }

        public virtual Accessory Accessory { get; set; }
        public virtual DeviceDetail DeviceDetail { get; set; }
    }
}
