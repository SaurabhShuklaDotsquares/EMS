using System;
using System.Collections.Generic;

namespace EMS.Data.Model
{
    public partial class ReminderUser
    {
        public long Id { get; set; }
        public long? ReminderId { get; set; }
        public int? Uid { get; set; }

        public virtual Reminder Reminder { get; set; }
    }
}
