using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Wfhactivity
    {
        public int Wfhid { get; set; }
        public int Uid { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Comment { get; set; }
        public int? Status { get; set; }
        public bool? IsHalf { get; set; }
        public bool? FirstHalf { get; set; }
        public bool? SecondHalf { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateModify { get; set; }
        public string Ip { get; set; }
        public int? Wfhcategory { get; set; }
        public int? ApprovedById { get; set; }
        public string AnyComment { get; set; }

        public virtual UserLogin ApprovedBy { get; set; }
        public virtual UserLogin U { get; set; }
        public virtual WfhtypesMaster WfhcategoryNavigation { get; set; }
    }
}
