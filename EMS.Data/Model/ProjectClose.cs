using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ProjectClose
    {
        public int Id { get; set; }
        public string ClosureId { get; set; }
        public string Crmid { get; set; }
        public string ClientName { get; set; }
        public string Reason { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
