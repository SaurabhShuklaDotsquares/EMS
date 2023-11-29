﻿using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class UserActivityLog
    {
        public int ActivityLogId { get; set; }
        public int? Uid { get; set; }
        public DateTime Date { get; set; }
        public int? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Status { get; set; }
        public DateTime DateAdded { get; set; }

        public virtual Project Project { get; set; }
        public virtual UserLogin UserLogin { get; set; }
    }
}
