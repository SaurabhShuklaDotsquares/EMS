using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ConferenceRoom
    {
        public ConferenceRoom()
        {
            ConferenceRoomBooking = new HashSet<ConferenceRoomBooking>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ThemeColor { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int? CompanyOfficeId { get; set; }

        public virtual CompanyOffice CompanyOffice { get; set; }
        public virtual ICollection<ConferenceRoomBooking> ConferenceRoomBooking { get; set; }
    }
}
