using System;
using System.Collections.Generic;

namespace EMS.Data.model
{
    public partial class AnouncementsMaster
    {
        public AnouncementsMaster()
        {
            Anouncements = new HashSet<Anouncements>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string TemplateText { get; set; }
        public bool IsActive { get; set; }
        public DateTime AddDate { get; set; }
        public int AddedBy { get; set; }
        public DateTime ModifyDate { get; set; }
        public int? DisplayOrder { get; set; }

        public virtual UserLogin AddedByNavigation { get; set; }
        public virtual ICollection<Anouncements> Anouncements { get; set; }
    }
}
