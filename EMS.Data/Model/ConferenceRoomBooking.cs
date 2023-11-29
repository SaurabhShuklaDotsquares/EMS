using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ConferenceRoomBooking
    {
        public long Id { get; set; }
        public int? ConferenceRoomId { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string AttendeeName { get; set; }
        public int? BookedByUid { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual UserLogin UserLogin { get; set; }
        public virtual ConferenceRoom ConferenceRoom { get; set; }
    }
}
