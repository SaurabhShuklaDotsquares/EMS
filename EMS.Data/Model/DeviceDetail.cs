using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class DeviceDetail
    {
        public DeviceDetail()
        {
            DeviceAccessoriesMap = new HashSet<DeviceAccessoriesMap>();
        }

        public int Id { get; set; }
        public int? DeviceId { get; set; }
        public string Condition { get; set; }
        public int AssignedToUid { get; set; }
        public DateTime AssignedDateTime { get; set; }
        public DateTime? SubmitDateTime { get; set; }
        public int? SubmitToUid { get; set; }
        public int CreateByUid { get; set; }
        public DateTime CreateDate { get; set; }
        public int ModifyByUid { get; set; }
        public DateTime ModifyDate { get; set; }
        public string SerialNumber { get; set; }
        public virtual UserLogin UserLogin  { get; set; }
        public virtual UserLogin UserLogin1 { get; set; }
        public virtual Device Device { get; set; }
        public virtual UserLogin ModifyByU { get; set; }
        public virtual UserLogin SubmitToU { get; set; }
        public virtual ICollection<DeviceAccessoriesMap> DeviceAccessoriesMap { get; set; }
    }
}
