using System;
using System.Collections.Generic;

namespace EMS.Data.model
{
    public partial class Pmpreference
    {
        public Pmpreference()
        {
            PmpreferenceSetting = new HashSet<PmpreferenceSetting>();
        }

        public int Id { get; set; }
        public string PreferenceKey { get; set; }
        public string PreferenceName { get; set; }
        public string PreferenceDescription { get; set; }
        public string PreferenceDataType { get; set; }

        public virtual ICollection<PmpreferenceSetting> PmpreferenceSetting { get; set; }
    }
}
