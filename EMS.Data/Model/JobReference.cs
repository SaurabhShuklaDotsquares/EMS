using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class JobReference
    {
        public int Id { get; set; }
        public int? CurrentOpeningId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string Small_Desc { get; set; }
        public string Attacchment { get; set; }
        public int? ReferBy_UserLoginId { get; set; }
        public int Status { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string IP { get; set; }

        public virtual CurrentOpening CurrentOpening { get; set; }
        public virtual UserLogin UserLogin { get; set; }
    }
}
