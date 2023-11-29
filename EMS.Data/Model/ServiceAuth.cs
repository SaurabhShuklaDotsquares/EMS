using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ServiceAuth
    {
        public int Id { get; set; }
        public string Apikey { get; set; }
        public string ApiPass { get; set; }
        public DateTime Modified { get; set; }
    }
}
