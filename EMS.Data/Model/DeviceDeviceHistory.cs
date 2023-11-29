using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class DeviceDeviceHistory
    {
        public int DeviceDeviceHistoryId { get; set; }
        public int? DeviceDeviceInfoId { get; set; }
        public int? Uid { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int SubmitedBy { get; set; }
        public int Status { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? SubmitedTo { get; set; }
        public bool? SubmitApproved { get; set; }

        public virtual DeviceDeviceInfo DeviceDeviceInfo { get; set; }
        public virtual UserLogin U { get; set; }
    }
}
