using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class MeetingMinutes
    {
        public int Id { get; set; }
        public string MeetingSubject { get; set; }
        public DateTime? MeetingDate { get; set; }
        public string PmandTl { get; set; }
        public string Discussed { get; set; }
        public string ActionPoint { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? AddedDate { get; set; }
    }
}
