using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class CompanyOffice
    {
        public CompanyOffice()
        {
            ConferenceRoom = new HashSet<ConferenceRoom>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string OfficeAddress { get; set; }
        public string Country { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<ConferenceRoom> ConferenceRoom { get; set; }
    }
}
