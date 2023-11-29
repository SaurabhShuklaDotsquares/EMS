using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ReportBug
    {
        public int ReportId { get; set; }
        public string SectionName { get; set; }
        public string SectionDescription { get; set; }
        public string ImageName { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string IP { get; set; }
        public int? UserId { get; set; }
        public bool? IsClosed { get; set; }
        public string PagePath { get; set; }
        public string Remark { get; set; }
        public bool? IsApproved { get; set; }
        public byte Status { get; set; }

        public virtual UserLogin UserLogin { get; set; }
    }
}
