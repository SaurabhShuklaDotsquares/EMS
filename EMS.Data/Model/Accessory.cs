using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Accessory
    {
        public Accessory()
        {
            DeviceAccessoriesMap = new HashSet<DeviceAccessoriesMap>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public short Quantity { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int CreateByUid { get; set; }
        public int ModifyByUid { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }

        public virtual UserLogin CreateByU { get; set; }
        public virtual UserLogin ModifyByU { get; set; }
        public virtual ICollection<DeviceAccessoriesMap> DeviceAccessoriesMap { get; set; }
    }
}
