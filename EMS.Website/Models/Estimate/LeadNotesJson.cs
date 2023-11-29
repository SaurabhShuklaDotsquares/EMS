using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.Web
{
    public class LeadNotesJson
    {
        public int lead_id { get; set; }
        public string notes_details { get; set; }
        public string notes_time { get; set; }
        public string user_name { get; set; }
    }
}
