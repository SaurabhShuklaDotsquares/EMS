using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class VirtualDeveloperCategory
    {
        public int Id { get; set; }
        public string VirtualDeveloperName { get; set; }
        public bool? IsActive { get; set; }
        public int? Pmuid { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? CreationDate { get; set; }
        public string Color { get; set; }
    }
}
