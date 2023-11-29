using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ReadMessage
    {
        public int MessageId { get; set; }
        public int? ForumId { get; set; }
        public int? Uid { get; set; }
        public bool? IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public string Ip { get; set; }

        public virtual Forums Forum { get; set; }
        public virtual UserLogin U { get; set; }
    }
}
