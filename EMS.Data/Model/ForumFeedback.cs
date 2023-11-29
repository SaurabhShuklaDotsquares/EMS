using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class ForumFeedback
    {
        public int Id { get; set; }
        public int? ForumId { get; set; }
        public int? UserId { get; set; }
        public string Feedback { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }

        public virtual Forums Forum { get; set; }
        public virtual UserLogin User { get; set; }
    }
}
