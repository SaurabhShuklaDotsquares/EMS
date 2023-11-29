using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Device
    {
        public Device()
        {
            DeviceDetails = new HashSet<DeviceDetail>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public short Quantity { get; set; }
        public string Condition { get; set; }
        public bool IsActive { get; set; }
        public int CreateByUid { get; set; }
        public int ModifyByUid { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte DeviceType { get; set; }
        public string SimNumber { get; set; }
        public string SimNetwork { get; set; }  
        public int PMUid { get; set; }

        public virtual UserLogin UserLogin{ get; set; }
        public virtual UserLogin UserLogin1 { get; set; }
        public virtual UserLogin Pmu { get; set; }
        public virtual ICollection<DeviceDetail> DeviceDetails { get; set; }
    }
}
