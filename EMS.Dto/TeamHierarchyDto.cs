using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class TeamHierarchyDto
    {
        public int Id { get; set; }
        public int TlId { get; set; }
        public int TotalEmployees { get; set; }
        public int? Pmuid { get; set; }
        public bool IsAllTeam { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
