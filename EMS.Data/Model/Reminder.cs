using System;
using System.Collections.Generic;

namespace EMS.Data.Model
{
    public partial class Reminder
    {
        public Reminder()
        {
            ReminderUser = new HashSet<ReminderUser>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime? ReminderDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool? IsActive { get; set; }
        public int? Status { get; set; }
        public int? ReminderType { get; set; }
        public bool? ActionTaken { get; set; }
        public int? ActionTakenBy { get; set; }
        public DateTime? ActionTakenOn { get; set; }
        public bool IsExcludeMe { get; set; }
        public DateTime? LastEmail { get; set; }

        public virtual ICollection<ReminderUser> ReminderUser { get; set; }
    }
}
