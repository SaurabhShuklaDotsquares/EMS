using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Momdocument
    {
        public int Id { get; set; }
        public string DocumentPath { get; set; }
        public DateTime AddedDate { get; set; }
        public int? MomMeetingId { get; set; }
    }
}
