using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ExperienceType
    {
        public int Id { get; set; }
        public string Experience { get; set; }
        public bool? IsActive { get; set; }
    }
}
