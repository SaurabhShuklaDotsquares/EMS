using System;
using System.Collections.Generic;

namespace EMS.Data.model
{
    public partial class PmpreferenceSetting
    {
        public int Id { get; set; }
        public int PmpreferenceId { get; set; }
        public int Uid { get; set; }
        public string PreferenceValue { get; set; }

        public virtual Pmpreference Pmpreference { get; set; }
        public virtual UserLogin U { get; set; }
    }
}
