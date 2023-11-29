using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Kra
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? DesignationId { get; set; }
        public bool? IsActive { get; set; }
        public int? DisplayOrder { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }


        public virtual Designation Designation { get; set; }
    }
}
