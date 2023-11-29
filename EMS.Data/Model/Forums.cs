using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Forums
    {
        public Forums()
        {
            ForumFeedback = new HashSet<ForumFeedback>();
            ReadMessage = new HashSet<ReadMessage>();
        }

        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsClosed { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Ip { get; set; }

        public virtual UserLogin User { get; set; }
        public virtual ICollection<ForumFeedback> ForumFeedback { get; set; }
        public virtual ICollection<ReadMessage> ReadMessage { get; set; }
    }
}
