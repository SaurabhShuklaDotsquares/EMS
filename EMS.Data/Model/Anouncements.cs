using System;
using System.Collections.Generic;

namespace EMS.Data.model
{
    public partial class Anouncements
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string LinkUrl { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDisplayInFront { get; set; }
        public int? DisplayOrder { get; set; }
        public int AnouncementType { get; set; }
        public DateTime AddDate { get; set; }
        public int AddedBy { get; set; }
        public DateTime ModifyDate { get; set; }
        public string Summary { get; set; }

        public virtual UserLogin AddedByNavigation { get; set; }
        public virtual AnouncementsMaster AnouncementTypeNavigation { get; set; }
    }
}
