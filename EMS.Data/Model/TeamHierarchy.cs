using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class TeamHierarchy
    {
        public int Id { get; set; }
        public int TlId { get; set; }
        public int TotalEmployees { get; set; }
        public int? Pmuid { get; set; }
        public bool? IsAllTeam { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual UserLogin UserLoginDetails { get; set; }
    }
}
